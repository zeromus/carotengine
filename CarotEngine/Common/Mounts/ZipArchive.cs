using System;
using System.IO;
using System.Collections;
using pr2.Common;
using zip=ICSharpCode.SharpZipLib.Zip;

namespace pr2.Common {
	namespace Mounts {
		class ZipArchive : ResourceManager.IMount {
			~ZipArchive() {
				if(zf != null) zf.Close();
			}

			zip.ZipFile zf = null;

			public static bool check(string fname) {
				zip.ZipFile zf;
				try {
					zf = new zip.ZipFile(fname);
				} catch(Exception) { return false; }
				zf.Close();
				return true;
			}

			class ZipArchiveStream : Stream {
				Stream zis;
				zip.ZipFile zf;
				zip.ZipEntry ze;
				public ZipArchiveStream(zip.ZipFile zf, zip.ZipEntry ze) {
					this.zis = zf.GetInputStream(ze);
					this.length = ze.Size;
					this.zf = zf;
					this.ze = ze;
				}
				long length, position = 0;

				public override void Close() {
					zis.Close();
					base.Close();
				}

				protected override void Dispose(bool disposing) {
					zis.Dispose();
					base.Dispose(disposing);
				}

				//we dont like our reads to be incomplete. make sure theyre complete here
				public override int Read(byte[] buffer, int offset, int count) {
					int todo = count;
					while(todo > 0) {
						int done = zis.Read(buffer, offset + count - todo, todo);
						if(done == 0) break;
						todo -= done;
					}
					int finished = count - todo;
					position += finished;
					return finished;
				}

				//zis can't seek. do dummy reads instead
				public override long Seek(long offset, SeekOrigin origin) {

					//convert all seekorigin into a seekorigin.current
					if(origin == SeekOrigin.Begin) {
						origin = SeekOrigin.Current;
						position = 0;
						zis = zf.GetInputStream(ze);
					} else if(origin == SeekOrigin.End) {
						origin = SeekOrigin.Current;
						offset = (long)length - offset;
						if(offset > position)
							offset = position - offset;
						else {
							position = 0;
							zis = zf.GetInputStream(ze);
						}
					}

					long todo = offset;
					byte[] buf = new byte[4096];
					while(todo > 0) {
						int work = 4096;
						if(todo < work) work = (int)todo;
						int done = zis.Read(buf, 0, work);
						if(done == 0) throw new Exception("Couldn't complete seek");
						todo -= done;
					}
					position += offset;
					return position;
				}

				public override int ReadByte() {
					if(position == length) return -1;
					else return zis.ReadByte();
				}


				public override void Write(byte[] buffer, int offset, int count) { throw new NotSupportedException(); }
				public override bool CanRead { get { return true; } }
				public override bool CanWrite { get { return false; } }
				public override bool CanSeek { get { return true; } }
				public override void Flush() { throw new NotSupportedException(); }
				public override long Length { get { return length; } }
				public override long Position { get { return position; } set { Seek(value, SeekOrigin.Begin); } }
				public override void SetLength(long value) { throw new NotSupportedException(); }
				public override void WriteByte(byte value) { throw new NotSupportedException(); }
			}

			public ZipArchive(string fname) {
				zf = new zip.ZipFile(fname);
			}

			public Stream open(string fname, FileAccess fa) {
				if(fa != FileAccess.Read) throw new NotSupportedException("Attempted to open a zipfile resource with other than read access");
				fname = fname.Substring(1);
				ICSharpCode.SharpZipLib.Zip.ZipEntry ze = zf.GetEntry(fname);
				ZipArchiveStream zas = new ZipArchiveStream(zf, ze);
				return zas;
			}

			public bool exists(string fname) {
				fname = fname.Substring(1);
				int n = zf.FindEntry(fname, true);
				return n != -1;
			}

			public void delete(string fname) {
				throw new NotSupportedException("Attempted to delete from a zipfile resource");
			}

		}
	}
}