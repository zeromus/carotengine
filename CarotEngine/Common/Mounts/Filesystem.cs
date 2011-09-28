using System;
using System.IO;

namespace pr2.Common {
	namespace Mounts {
		class Filesystem : ResourceManager.IMount {
			public static bool check(string fname) {
				if(Directory.Exists(fname)) return true;
				return false;
			}

			DirectoryInfo di;
			public Filesystem(DirectoryInfo di) { this.di = di; }
			public Stream open(string fname, FileAccess fa) {
				FileInfo fi = new FileInfo(di.FullName + fname);
				if(fa == FileAccess.Read)
					return fi.Open(FileMode.Open, fa, FileShare.Read);
				else
					return fi.Open(FileMode.OpenOrCreate, fa, FileShare.Read);
			}

			public bool exists(string fname) {
				FileInfo fi = new FileInfo(di.FullName + fname);
				return fi.Exists;
			}

			public void delete(string fname) {
				FileInfo fi = new FileInfo(di.FullName + fname);
				fi.Delete();
			}
		}
	}
}