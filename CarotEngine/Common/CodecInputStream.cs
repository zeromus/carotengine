using System;
using System.Collections;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace pr2.Common
{

	//this class may be used to read verge a_codec buffers
	//simply create one of these with the input stream youre using
	//and try not to read past the end of the decoded data

	//NOTE: position and length may not work properly. ill have to fix them
	//if anyone ever wants to rely on them
	public class CodecInputStream : Stream
	{
		//ICSharpCode.SharpZipLib.Zip.Compression.Streams.InflaterInputStream iis;
		Stream iis;
		public CodecInputStream(Stream baseStream)
		{
			BinaryReader br = new BinaryReader(baseStream);
			decompressedSize = br.ReadInt32();
			compressedSize = br.ReadInt32();
			byte[] inbuf = new byte[compressedSize];
			baseStream.Read(inbuf, 0, compressedSize);
			Inflater inflater = new Inflater(false);
			inflater.SetInput(inbuf);
			byte[] buf = new byte[decompressedSize];
			inflater.Inflate(buf);
			this.iis = new MemoryStream(buf);
		}
		public int decompressedSize;
		public int compressedSize;
		
		public override int Read(byte[] buffer, int offset, int count) { return iis.Read(buffer,offset,count); }
		public override void Write(byte[] buffer, int offset, int count) { throw new NotSupportedException(); }
		public override bool CanRead { get { return true; } }
		public override bool CanWrite { get { return false; } }
		public override bool CanSeek { get { return false; } }
		public override void Flush() {}
		public override long Length { get { return (long)decompressedSize; } }
		public override long Position { get { return iis.Position; } set { throw new NotSupportedException(); } }
		public override long Seek(long offset, SeekOrigin origin) { throw new NotSupportedException(); }
		public override void SetLength(long value) { throw new NotSupportedException(); }
		public override void WriteByte(byte value) { throw new NotSupportedException(); }
		public override int ReadByte() { return iis.ReadByte(); }


	}
}