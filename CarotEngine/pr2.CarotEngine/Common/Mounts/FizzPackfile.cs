using System;
using System.IO;
using System.Collections.Generic;
using pr2.Common;


namespace pr2.Common.Mounts {

	class FizzPackfile : ResourceManager.IMount {
		Stream stream;
		int fileCount, nameTableLength;

		static uint FastHash(string str) {
			int result = 0;
			for(int i=0; i<str.Length; i++) {
				char c = str[i];
				if(c == '*')
					return (uint)(result ^ FastHash(str.Substring(i+1)));
				else result = (result * 31 + ((byte)c &  0x5F));
			}
			return (uint)result;
		}

		class FileRecord {
			public uint hash;
			public int offset;
			public int nameOffset;
			public int length;
		}

		public static bool check(string fname) {
			if(!File.Exists(fname)) return false;
			using(Stream s = File.OpenRead(fname)) {
				BinaryReader br = new BinaryReader(s);
				if(br.ReadUInt32() == 0xdeadba11)
					return true;
			}
			return false;
		}


		public Stream open(string fname, FileAccess fa) {
			if(fa != FileAccess.Read)
				throw new NotSupportedException();
			FileRecord fr=null;
			if(!fileRecords.TryGetValue(FastHash(fname),out fr))
				throw new FileNotFoundException();
			FileStream fs = new FileStream(this.fname,FileMode.Open,FileAccess.Read,FileShare.Read);
			fs.Position = fr.offset;
			return new SubInputStream(fs, fr.offset, fr.length);
		}
		public bool exists(string fname) {
			return fileRecords.ContainsKey(FastHash(fname));
		}
		public void delete(string fname) {
			throw new NotSupportedException();
		}

		Dictionary<uint, FileRecord> fileRecords = new Dictionary<uint, FileRecord>();
		string fname;

		public FizzPackfile(string fname) {
			this.fname = fname;

			using(stream = File.Open(fname, FileMode.Open, FileAccess.Read, FileShare.Read)) {
				BinaryReader br = new BinaryReader(stream);
				if(br.ReadUInt32() != 0xdeadba11)
					throw new Exception("Invalid FizzPackfile");
				if(br.ReadUInt32() != 4)
					throw new Exception("Invalid FizzPackfile");

				fileCount = br.ReadInt32();
				nameTableLength = br.ReadInt32();

				List<FileRecord> temp = new List<FileRecord>();
				for(int i=0; i<fileCount; i++) {
					FileRecord fr = new FileRecord();
					fr.hash = br.ReadUInt32();
					fr.offset = br.ReadInt32();
					fileRecords[fr.hash] = fr;
					temp.Add(fr);
				}

				for(int i=0; i<fileCount; i++)
					temp[i].nameOffset = br.ReadInt32();

				//read the lengths
				for(int i=0; i<fileCount; i++) {
					stream.Position = temp[i].offset;
					temp[i].length = br.ReadInt32();
					temp[i].offset += 4;
				}
					
				
			}
		}
	}

}
