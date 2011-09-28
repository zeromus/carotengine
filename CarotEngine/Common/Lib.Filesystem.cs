using System;
using System.IO;

namespace pr2.Common {

	partial class Lib {
		/// <summary>
		/// Writes bytes to a binary file
		/// </summary>
		/// <param name="path">file to be written </param>
		/// <param name="bytes">bytes to be written</param>
		public static void WriteBinaryFile(string path, byte[] bytes) {
			using(Stream s = ResourceManager.Open(path,FileAccess.Write))
				s.Write(bytes, 0, bytes.Length);
		}

		/// <summary>
		/// Ensures that the directory path to the given filename exists,
		/// so that the file can be created without any problems
		/// </summary>
		/// <param name="path">path to create directories for</param>
		public static void ensurePathToFile(string path) {
			Directory.CreateDirectory(new FileInfo(path).DirectoryName);
		}

		/// <summary>
		/// Writes binary data to a tempfile
		/// </summary>
		/// <param name="bytes">binary data to be written</param>
		/// <returns>pathname of the created tempfile</returns>
		public static string WriteBinaryFileToTemp(byte[] bytes) {
			string tmpfile = Path.GetTempFileName();
			WriteBinaryFile(tmpfile, bytes);
			return tmpfile;
		}

		/// <summary>
		/// Reads an entire text file from disk
		/// </summary>
		/// <param name="path">file to be read</param>
		/// <returns>a string containing the contents of the text file</returns>
		public static string ReadTextFile(string path) {
			using(Stream s = ResourceManager.Open(path,FileAccess.Read)) {
				StreamReader sr = new StreamReader(s);
				return sr.ReadToEnd();
			}
			
		}
		public static string ReadTextFile(FileInfo fi) { return ReadTextFile(fi.FullName); }

		/// <summary>
		/// Reads an entire binary file from disk
		/// </summary>
		/// <param name="path">file to be read</param>
		/// <returns>a byte array containing the contents of the binary file</returns>
		public static byte[] ReadBinaryFile(string path) {
			using(Stream s = ResourceManager.Open(path,FileAccess.Read)) {
				byte[] bytes = new byte[(int)s.Length];
				s.Read(bytes, 0, (int)s.Length);
				return bytes;
			}
		}

		/// <summary>
		/// Writes a string to a temp file
		/// </summary>
		/// <param name="text">text to be written</param>
		/// <returns>pathname of the created tempfile</returns>
		public static string WriteTextFileToTemp(string text) {
			string tmpfile = Path.GetTempFileName();
			WriteTextFile(tmpfile, text);
			return tmpfile;
		}

		/// <summary>
		/// Writes a string to a text file
		/// </summary>
		/// <param name="path">file to be written</param>
		/// <param name="text">text to be written</param>
		public static void WriteTextFile(string path, string text) {
			using(Stream s = ResourceManager.Open(path,FileAccess.Write)) {
				StreamWriter sw = new StreamWriter(s);
				sw.Write(text);
			}
		}

		/// <summary>
		/// Writes a memory stream to a file
		/// </summary>
		/// <param name="path">file to be written</param>
		/// <param name="ms">memorystream to be written</param>
		public static void WriteMemoryStream(string path, MemoryStream ms) {
			using(Stream s = ResourceManager.Open(path,FileAccess.Write))
				ms.WriteTo(s);
		}
	}
}