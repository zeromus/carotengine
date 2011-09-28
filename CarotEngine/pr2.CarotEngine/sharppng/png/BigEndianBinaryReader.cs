using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace pr2.sharppng {
	
	class BigEndianBinaryReader : BinaryReader {

		public BigEndianBinaryReader(Stream input) : base(input) { }
		public BigEndianBinaryReader(Stream input, Encoding encoding) : base(input, encoding) { }
		//public virtual void Close();
		//protected virtual void Dispose(bool disposing);
		//protected virtual void FillBuffer(int numBytes);
		//private int InternalReadChars(char[] buffer, int index, int count);
		//private int InternalReadOneChar();
		//public virtual int PeekChar();
		//public virtual int Read();
		//public virtual int Read(byte[] buffer, int index, int count);
		//public virtual int Read(char[] buffer, int index, int count);
		public override bool ReadBoolean() { throw new Exception("not implemented"); }
		//public virtual byte ReadByte();
		public override byte[] ReadBytes(int count) { throw new Exception("not implemented"); }
		public override char ReadChar() { throw new Exception("not implemented"); }
		public override char[] ReadChars(int count) { throw new Exception("not implemented"); }
		//public override decimal ReadDecimal() { throw new Exception("not implemented"); }
		public override double ReadDouble() { throw new Exception("not implemented"); }
		byte[] buf = new byte[8];
		public override short ReadInt16() {
			Read(buf, 0, 2);
			return (short)((this.buf[0] << 8) | (this.buf[1] << 0));
		}
		public override int ReadInt32() {
			Read(buf, 0, 4);
			return ((((this.buf[0] << 24) | (this.buf[1] << 16)) | (this.buf[2] << 8)) | (this.buf[3] << 0));

		}
		public override long ReadInt64() {
			throw new Exception("not implemented");
		}
		//public override sbyte ReadSByte() {}
		public override float ReadSingle() { throw new Exception("not implemented"); }
		public override string ReadString() { throw new Exception("not implemented"); }
		public override ushort ReadUInt16() {
			return (ushort)ReadInt16();
		}
		public override uint ReadUInt32() {
			return (uint)ReadUInt32();
		}
		public override ulong ReadUInt64() { throw new Exception("not implemented"); }
		//void IDisposable.Dispose();
	}
}
