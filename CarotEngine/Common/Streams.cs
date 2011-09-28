using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace pr2.Common
{
	/// <summary>
	/// A stream wrapping the specified sub-section of the provided stream.
	/// Please dont use this with files > 2GB
	/// </summary>
	class SubInputStream : Stream
	{
		Stream BaseStream;
		int length;
		int position;
		long start;
		bool own;

		public override void Close() {
			if(own) {
				if(BaseStream != null)
					BaseStream.Dispose();
				BaseStream = null;
			}
		}

		public SubInputStream(Stream stream, long position, int length, bool own) {
			this.BaseStream = stream;
			this.length = length;
			start = position;
			this.own = own;
		}

		public SubInputStream(Stream stream, long position, int length)
		{
			this.BaseStream = stream;
			this.length = length;
			start = position;
			this.own = false;
		}

		public override void Flush() { throw new NotImplementedException(); }
		public override int Read(byte[] buffer, int offset, int count)
		{
			int todo = Math.Min(length - position, count);
			int read = BaseStream.Read(buffer, offset, todo);
			position += read;
			return read;
		}
		public override int ReadByte()
		{
			if (position == length) return -1;
			int val = BaseStream.ReadByte();
			position++;
			return val;
		}

		void _seek() {
			if(position<0) position = 0;
			if(position>length) position = length;
			BaseStream.Position = start + position;
		}

		public override long Seek(long offset, SeekOrigin origin) {
			switch(origin) {
				case SeekOrigin.Begin:
					position = (int)offset;
					_seek();
					return position;
				case SeekOrigin.Current:
					position += (int)offset;
					_seek();
					return position;
				case SeekOrigin.End:
					position = length+(int)offset;
					_seek();
					return position;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		public override void SetLength(long value) { throw new NotImplementedException(); }
		public override void Write(byte[] buffer, int offset, int count) { throw new NotImplementedException(); }
		public override void WriteByte(byte value) { throw new NotImplementedException(); }

		public override bool CanRead { get { return true; } }
		public override bool CanSeek { get { return false; } }
		public override bool CanTimeout { get { return BaseStream.CanTimeout; } }
		public override bool CanWrite { get { return false; } }
		public override long Length { get { return length; } }
		public override long Position { get { return position; } set { Seek(value, SeekOrigin.Begin); } }
		public override int ReadTimeout { get { return BaseStream.ReadTimeout; } set { BaseStream.ReadTimeout = value; } }
		public override int WriteTimeout { get { return BaseStream.WriteTimeout; } set { BaseStream.WriteTimeout = value; } }

	}
}
