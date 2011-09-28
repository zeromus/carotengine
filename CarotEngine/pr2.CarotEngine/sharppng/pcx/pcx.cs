using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace pr2.sharppng {

	public interface IPcxImageOutput
	{
		void WriteData(Pcx png);
	}

	public class Pcx {

		private static readonly byte[] signature = new byte[] { 0x0A, 0x05, 0x01 };

		public static bool IsPcx(Stream stream) {
			for (int i=0; i<3; i++)
				if (stream.ReadByte() != signature[i])
					return false;
			return true;
		}

		public byte manufacturer;
		public byte version;
		public byte encoding;
		public byte bits_per_pixel;
		public ushort xmin;
		public ushort ymin;
		public ushort xmax;
		public ushort ymax;
		public ushort hdpi;
		public ushort vdpi;
		//byte colormap[48];
		public byte reserved;
		public byte num_planes;
		public ushort bytes_per_line;
		public ushort palette_info;
		public ushort h_screen_size;
		public ushort v_screen_size;
		//byte filler[54];

		public byte[] palette;
		public byte[] pixels;
		public int width, height;

		
		  void ReadScanline(Stream stream, int scansize, byte[] outdata, int dofs) {
			  int i=dofs;
			  int end = dofs + scansize;
			  while(i<end) {
				  // read a byte!
				  int data = stream.ReadByte();
				  if(data == -1) throw new Exception("Invalid PCX data");

				  if ((data & 0xC0) != 0xC0) {  // non RLE
					  outdata[i++] = (byte)data;
				  } else {   
					  // RLE

					// read the repeated byte
					int numbytes = data & 0x3F;
					data = stream.ReadByte();
					  if(data == -1) throw new Exception("Invalid PCX data");

					  while(i<end && numbytes--!=0)
						  outdata[i++] = (byte)data;
				  }

			  }
		  }

		public void read(Stream stream, IPcxImageOutput imageOutput) {
			BinaryReader br = new BinaryReader(stream);
			manufacturer = br.ReadByte();
			version = br.ReadByte();
			encoding = br.ReadByte();
			bits_per_pixel = br.ReadByte();
			xmin = br.ReadUInt16();
			ymin = br.ReadUInt16();
			xmax = br.ReadUInt16();
			ymax = br.ReadUInt16();
			hdpi = br.ReadUInt16();
			vdpi = br.ReadUInt16();
			stream.Position += 48;
			reserved = br.ReadByte();
			num_planes = br.ReadByte();
			bytes_per_line = br.ReadUInt16();
			palette_info = br.ReadUInt16();
			h_screen_size = br.ReadUInt16();
			v_screen_size = br.ReadUInt16();
			stream.Position += 54;

			// verify the header

			// we only support RLE encoding
			if (encoding != 1)
				throw new Exception("PCX attempted to load image without RLE encoding. this is unsupported");

			// we only support 8 bits per pixel. but we know how to handle 24bpp so we'll do that later
			if (bits_per_pixel != 8)
				throw new Exception("PCX attempted to load image without 8bpp. this is unsupported");

			//we only support one plane
			if(num_planes != 1)
				throw new Exception("PCX attempted to load image without one plane. this is unsupported. (is this 24bpp?)");

			// create the image structure
			width  = xmax - xmin + 1;
			height = ymax - ymin + 1;

			pixels = new byte[bytes_per_line*height];
			
			// read all of the scanlines
			for (int iy = 0; iy < height; ++iy)
				ReadScanline(stream, bytes_per_line, pixels, bytes_per_line*iy);

			//try to read palette. but to stay as safe as possible, try only to skip forward
			// seek back from the end 769 bytes
			long pos = stream.Length-769;
			if(pos >= stream.Position)
				stream.Seek(stream.Position - pos,SeekOrigin.Current);

			// do we have a palette?
			int has_palette = stream.ReadByte();
			if (has_palette != 12)
				throw new Exception("Couldnt find PCX palette");

			palette = br.ReadBytes(768);

			imageOutput.WriteData(this);
		}

	}
}