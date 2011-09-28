using System;
using System.IO;
using System.Collections.Generic;
using pr2.Common;

//OVERRIDE flag is ignored
//this is a concept we do not have in our resourcemanager
//we could add it later via a system where a IMount can return
//a class that contains a stream and an overrideable flag
//and if the overrideable flag is set upon return from the
//IMount, the resourcemanager will attempt to find it somewhere else
//or something

namespace pr2.Common {
	namespace Mounts {
		class VergePakfile : ResourceManager.IMount {
			class Record {
				public int size, ofs;
				public bool extractable, _override;
			}

			~VergePakfile() {
				if(s != null) s.Close();
			}

			class PakfileStream : Stream {
				Record r;
				Stream s;
				int pos = 0;

				public PakfileStream(Stream s, Record r) { this.s = s; this.r = r; }

				public override int Read(byte[] buffer, int offset, int count) {
					count = System.Math.Min(count, r.size - pos);
					if(count == 0) return 0;
					lock(s) {
						s.Position = r.ofs + pos;
						s.Read(buffer, offset, count);
						pos += count;
						return count;
					}
				}
				public override void Write(byte[] buffer, int offset, int count) { throw new NotSupportedException(); }
				public override bool CanRead { get { return true; } }
				public override bool CanWrite { get { return false; } }
				public override bool CanSeek { get { return true; } }
				public override void Flush() { }
				public override long Length { get { return r.size; } }
				public override long Position { get { return pos; } set { pos = (int)value; } }
				public override long Seek(long offset, SeekOrigin origin) {
					switch(origin) {
						case SeekOrigin.Begin: pos = (int)offset; break;
						case SeekOrigin.Current: pos += (int)offset; break;
						case SeekOrigin.End: pos = r.size; break;
					}
					pos = System.Math.Max(0, System.Math.Min(r.size, pos));
					return pos;
				}
				public override void SetLength(long value) { throw new NotSupportedException(); }
				public override void WriteByte(byte value) { throw new NotSupportedException(); }
				public override int ReadByte() {
					if(pos == r.size) return -1;
					lock(s) {
						s.Position = r.ofs + pos;
						pos++;
						return s.ReadByte();
					}
				}
			}

			class DecryptingStreamAdapter : Stream {
				Stream s;
				public DecryptingStreamAdapter(Stream s) { this.s = s; }

				public override int Read(byte[] buffer, int offset, int count) {
					for(int i = 0; i < count; i++) {
						int n = ReadByte();
						if(n == -1) return i;
						buffer[offset + i] = (byte)n;
					}
					return count;
				}
				public override void Write(byte[] buffer, int offset, int count) { s.Write(buffer, offset, count); }
				public override bool CanRead { get { return s.CanRead; } }
				public override bool CanWrite { get { return s.CanWrite; } }
				public override bool CanSeek { get { return s.CanSeek; } }
				public override void Flush() { s.Flush(); }
				public override long Length { get { return s.Length; } }
				public override long Position { get { return s.Position; } set { s.Position = value; } }
				public override long Seek(long offset, SeekOrigin origin) { return s.Seek(offset, origin); }
				public override void SetLength(long value) { s.SetLength(value); }
				public override void WriteByte(byte value) { s.WriteByte(value); }
				public override int ReadByte() {
					int n = s.ReadByte();
					if(n == -1) return -1;
					return (byte)~n;
				}
			}

			Dictionary<string, object> files = new Dictionary<string, object>();
			Stream s = null;

			public static bool check(string fname) {
				if(!File.Exists(fname)) return false;
				Stream s = File.OpenRead(fname);
				try {
					checkHeader(s);
					s.Close();
					return true;
				} catch(Exception) { s.Close(); return false; }
			}

			static void checkHeader(Stream s) {
				string str = StringBufferReader.read(s, 6);
				if(str != "V3PAK")
					throw new Exception("Pakfile signature invalid; are you sure this is a v3 pakfile?");

				BinaryReader br = new BinaryReader(s);
				byte version = br.ReadByte();
				if(version != 1)
					throw new Exception("Pakfile header invalid; expected version 1; received " + version.ToString());
			}

			public VergePakfile(string fname) {
				s = File.OpenRead(fname);
				BinaryReader br = new BinaryReader(s);

				checkHeader(s);

				int numFiles = br.ReadInt32();

				DecryptingStreamAdapter dsa = new DecryptingStreamAdapter(s);
				BinaryReader brdsa = new BinaryReader(dsa);
				for(int i = 0; i < numFiles; i++) {
					Record r = new Record();
					string f = "/" + StringBufferReader.read(dsa, 256);
					f = f.Replace("\\", "/");
					r.size = brdsa.ReadInt32();
					r.ofs = brdsa.ReadInt32() + (11 + 266 * numFiles); //hardcoded offset calculation formula
					r.extractable = brdsa.ReadByte() == 1;
					r._override = brdsa.ReadByte() == 1;
					files[f] = r;
				}
			}
			public Stream open(string fname, FileAccess fa) {
				if(fa != FileAccess.Read) throw new NotSupportedException("Attempted to open a verge pakfile resource with other than read access");
				return new PakfileStream(s, (Record)files[fname]);
			}


			public bool exists(string fname) {
				return (Record)files[fname] != null;
			}

			public void delete(string fname) {
				throw new NotSupportedException("Attempted to delete from a verge pakfile resource");
			}
		}
	}
}