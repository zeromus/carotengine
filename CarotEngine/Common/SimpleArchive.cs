using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace pr2.Common
{
    /// <summary>
    /// A simple file archive useful for combining a few files into a package.
	/// This should be used to access no more than one file at a time.
    /// </summary>
	public class SimpleArchive : IDisposable
	{
		/// <summary>
		/// Pass in the stream you want to read from. The stream needs to be seekable and begin at position=0.
		/// the SimpleArchive will take ownership of the stream and close it when the SimpleArchive is disposed.
		/// Do not reuse the stream.
		/// </summary>
		public SimpleArchive(Stream stream) {
			this.stream = stream;
			BinaryReader br = new BinaryReader(stream);
			numFiles = br.ReadInt32();
			for (int i = 0; i < numFiles; i++)
			{
				string str = br.ReadString();
				int ofs = br.ReadInt32();
				int len = br.ReadInt32();
				File f = new File(ofs, len);
				files[str] = f;
				sortedFiles.Add(f);
			}
		}

		public void Dispose() { stream.Dispose(); }
		
		Stream stream;

		public Stream open(File file) {
			stream.Position = file.ofs;
			return new SubInputStream(stream, file.ofs, file.length, false);
		}

		public Stream open(string fname)
		{
			File file;
			if (!files.TryGetValue(fname, out file))
				throw new FileNotFoundException();
			return open(file);
		}
		
		/// <summary>
		/// all contained files
		/// </summary>
		public Dictionary<string, File> files = new Dictionary<string, File>();

		/// <summary>
		/// all contained files, ordered as they were found
		/// </summary>
		public List<File> sortedFiles = new List<File>();

		public int numFiles;

		public struct File
		{
			public File(int ofs, int length) { this.ofs = ofs; this.length = length; }
			public int ofs, length;
		}
	}

	public class SimpleArchiver
	{
		public void add(string fname, byte[] data)
		{
			files[fname] = data;
		}

		Dictionary<string, byte[]> files = new Dictionary<string, byte[]>();

		public void dump(Stream stream)
		{
			
			List<byte[]> blobs = new List<byte[]>();
			
			//write the header once to count the size
			MemoryStream msHeader = new MemoryStream();
			BinaryWriter bw = new BinaryWriter(msHeader);
			bw.Write(files.Count);
			foreach(KeyValuePair<string,byte[]> kvp in files)
			{
				bw.Write(kvp.Key);
				bw.Write(0);
				bw.Write(kvp.Value.Length);
				blobs.Add(kvp.Value);
			}

			//write the header for real but fix up the offsets
			bw = new BinaryWriter(stream);
			bw.Write(files.Count);
			int cursor = (int)msHeader.Length;
			foreach(KeyValuePair<string, byte[]> kvp in files) {
				bw.Write(kvp.Key);
				bw.Write(cursor);
				bw.Write(kvp.Value.Length);
				cursor += kvp.Value.Length;
			}

			//write the blobs
			for (int i = 0; i < blobs.Count; i++)
				bw.Write(blobs[i]);
			bw.Flush();
		}
	}
}
