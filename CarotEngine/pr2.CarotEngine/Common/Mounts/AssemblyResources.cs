using System;
using System.IO;
using System.Reflection;

namespace pr2.Common {
	namespace Mounts {
		class AssemblyResources : ResourceManager.IMount {
			Assembly asm;

			public AssemblyResources(Assembly asm, string resPrefix) { this.asm = asm; this.resPrefix = resPrefix; }
			string resPrefix;
			public Stream open(string fname, FileAccess fa) {
				if(fa != FileAccess.Read) throw new NotSupportedException("Attempted to open an assembly resource with other than read access");
				return asm.GetManifestResourceStream(resPrefix + fname.Substring(1));
			}

			public bool exists(string fname) {
				return asm.GetManifestResourceStream(resPrefix + fname.Substring(1)) != null;
			}

			public void delete(string fname) {
				throw new NotSupportedException("Attempted to delete an assembly resource");
			}
		}
	}
}