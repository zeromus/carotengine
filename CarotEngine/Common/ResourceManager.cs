using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;

//this system is case sensitive in places. a pain, i know. gahhhhh

namespace pr2.Common
{
	public class ResourceManager
	{
		//if you set this flag, the resourcemanager will be disabled
		//and all calls to ResourceManager.open() will just work
		//with system filesystem paths
		public static bool bDisabled = false;

		static ResourceManager() {
		}

		/// <summary>
		/// returns the directory for the specified path.
		/// useful for finding textures in the same path as a model
		/// TODO - why cant we just use Path.GetDirectoryName etc.?
		/// </summary>
		public static string getDirectoryOf(string fname) {
			fname = fname.Replace('\\','/');
			int idx = fname.LastIndexOf('/');
			if(idx == -1) return "/";
			else return pr2.Common.Str.Left(fname, idx);
		}

		/// <summary>
		/// returns a filename which accompanies the basispath i.e. /folder/test.map and test.vsp
		/// </summary>
		public static string getAccompanyingFilename(string basispath, string fname) {
			return Path.Combine(getDirectoryOf(basispath), fname);
		}

		/// <summary>
		/// the interface for different types of filesystem mounts
		/// </summary>
		public interface IMount {
			Stream open(string fname, FileAccess fa);
			bool exists(string fname);
			void delete(string fname);
		}

		class Record
		{
			public IMount mount;
			public string mountPath;
			public Record(IMount mount, string mountPath) { this.mount = mount; this.mountPath = mountPath; }
		}
		private static List<object> records = new List<object>();

		public static void mount(string path) { mount("",path); }

		public static void mount(string mountPath, string path)
		{
			if(Mounts.Filesystem.check(path))
				mountDirectory(mountPath, path);
			else if(Mounts.VergePakfile.check(path))
				mountVergePakfile(mountPath, path);
			//else if(Mounts.ZipArchive.check(path))
			//    mountZipArchive(mountPath, path);
			else if(Mounts.FizzPackfile.check(path))
				mountFizzPackfile(mountPath, path);
		}

		public static void MountAssemblyResources(string mountPath, System.Reflection.Assembly asm, string resPrefix)
		{
			records.Add(new Record(new Mounts.AssemblyResources(asm, resPrefix), mountPath));
		}

		//public static void mountZipArchive(string mountPath, string fname)
		//{
		//    try
		//    {
		//        Mounts.ZipArchive za = new Mounts.ZipArchive(fname);
		//        records.Add(new Record(za,mountPath));
		//    }
		//    catch(Exception e)
		//    {
		//        throw new MountFailedException("mountZipArchive() could not mount the given ziparchive:\n\t" + new FileInfo(fname).FullName + "\n" + "Here is what went wrong:\n\n "+ e.ToString());
		//    }			
		//}

		static DirectoryInfo contentRoot;
		public static void SetContentRoot(DirectoryInfo root)
		{
			contentRoot = root;
		}
		public static DirectoryInfo ContentRoot { get { return contentRoot; } }



		public static void mountIMount(string mountPath, IMount mount) {
			records.Add(new Record(mount, mountPath));
		}

		public static void mountDirectory(string mountPath, string dir)
		{
			DirectoryInfo di = new DirectoryInfo(dir);
			if(!di.Exists) throw new MountFailedException("Given directory does not exist:\n\t" + dir);
			records.Add(new Record(new Mounts.Filesystem(di),mountPath));
		}
		public static void mountVergePakfile(string mountPath, string fname) {
			try
			{
				Mounts.VergePakfile vpk = new Mounts.VergePakfile(fname);
				records.Add(new Record(vpk,mountPath));
			}catch(Exception e){
				throw new MountFailedException("mountVergePakfile() could not mount the given pakfile:\n\t" + new FileInfo(fname).FullName + "\n" + "Here is what went wrong:\n\n "+ e.ToString());
			}
		}

		public static void mountFizzPackfile(string mountPath, string fname) {
			try {
				Mounts.FizzPackfile fpk = new Mounts.FizzPackfile(fname);
				records.Add(new Record(fpk, mountPath));
			}
			catch(Exception e) {
				throw new MountFailedException("mountFizzPackfile() could not mount the given pakfile:\n\t" + new FileInfo(fname).FullName + "\n" + "Here is what went wrong:\n\n "+ e.ToString());
			}
		}

		class MountFailedException : Exception { public MountFailedException(string str) : base("ResourceManager mount operation failed with the following problem:\n\n"+str){} }


		static bool bDisabledOld;
		public static void OverrideEnableBegin(bool value)
		{
			bDisabledOld = bDisabled;
			bDisabled = !value;
			System.Threading.Monitor.Enter(typeof(ResourceManager));
		}

		public static void OverrideEnableEnd()
		{
			bDisabled = bDisabledOld;
			System.Threading.Monitor.Exit(typeof(ResourceManager));
		}

		public static bool Exists(string fname)
		{
			try
			{
				Stream s = Open(fname);
				s.Close();
			}
			catch(FileNotFoundException) { return false; }
			return true;
		}

		public static void Delete(string fname)
		{
		}

		public static string ReadString(string fname) 
		{
			Stream s;
			string str;
			try { s = Open(fname); } 
			catch { return null; }
			try 
			{
				str = new System.IO.StreamReader(s).ReadToEnd();
				s.Close();
			}
			catch { return null; }
			finally { s.Close(); }
			return str;
		}

		public static Stream Open(string fname) { return Open(fname, FileAccess.Read); }

		public static Stream Open(string fname, FileAccess fa)
		{
			/////////
			//todo: this is shitty logic
			//if(bDisabled) return File.Open(fname,FileMode.OpenOrCreate,fa);
			/////////

			//if we detect a hardcoded physical filesystem path, then open it directly
			if(fname[1] == ':' && fname[2] == '\\')
				if(fa == FileAccess.ReadWrite || fa == FileAccess.Write)
					return File.Open(fname, FileMode.OpenOrCreate, fa,FileShare.Read);
				else
					return File.Open(fname, FileMode.Open, fa,FileShare.Read);

			if(fname[0] != '/') fname = "/" + fname;
			fname = fname.Replace('\\', '/');

			foreach(Record r in records)
			{
				if(fname.IndexOf(r.mountPath)!=0) continue;
				string newfname = fname.Substring(r.mountPath.Length);
				if(fa != FileAccess.Read || r.mount.exists(newfname)) return r.mount.open(newfname,fa);
			}

			throw new FileNotFoundException("[resource]: " + fname);
		}
	}
}