using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace pr2.sharppng {

	sealed class CRCInputStream : Stream {
		
		//----------------stream implementations----------
		public override void Flush() { BaseStream.Flush(); }
		public override int Read(byte[] buffer, int offset, int count) {
			int read = BaseStream.Read(buffer, offset, count);
			crc.Update(buffer, 0, read);
			byteCount += read;
			return read;
		}
		public override int ReadByte() {
			int val = BaseStream.ReadByte();
			if(val != -1)
				crc.Update(val);
			byteCount++;
			return val;
		}

		public override long Seek(long offset, SeekOrigin origin) { return BaseStream.Seek(offset, origin); }
		public override void SetLength(long value) { BaseStream.SetLength(value); }
		public override void Write(byte[] buffer, int offset, int count) { throw new NotImplementedException(); }
		public override void WriteByte(byte value) { throw new NotImplementedException(); }

		public override bool CanRead { get { return true; } }
		public override bool CanSeek { get { return BaseStream.CanSeek; } }
		public override bool CanTimeout { get { return BaseStream.CanTimeout; } }
		public override bool CanWrite { get { return false; } }
		public override long Length { get { return BaseStream.Length; } }
		public override long Position { get { return BaseStream.Position; } set { BaseStream.Position = value; } }
		public override int ReadTimeout { get { return BaseStream.ReadTimeout; } set { BaseStream.ReadTimeout = value; } }
		public override int WriteTimeout { get { return BaseStream.WriteTimeout; } set { BaseStream.WriteTimeout = value; } }


		//-------------------------------------------------

		private ICSharpCode.SharpZipLib.Checksums.Crc32 crc = new ICSharpCode.SharpZipLib.Checksums.Crc32();
		Stream BaseStream;
		int byteCount;

		public long Value { get { return crc.Value; } }
		
		public CRCInputStream(Stream stream) {
			BaseStream = stream;
		}

		public void resetCrc() {
			byteCount = 0;
			crc.Reset(); //mbg
		}

		//do we need this?
		public int count { get { return byteCount; } }
	}
}
