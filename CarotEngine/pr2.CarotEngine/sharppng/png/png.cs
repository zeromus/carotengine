using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace pr2.sharppng {

	public struct IHDR {
		public int width, height;
		public byte bitdepth;
		public ColorType colortype;
		public byte method, filter, interlace;
		internal void read(BigEndianBinaryReader reader) {
			width = reader.ReadInt32();
			height = reader.ReadInt32();
			bitdepth = reader.ReadByte();
			colortype = (ColorType)reader.ReadByte();
			method = reader.ReadByte();
			filter = reader.ReadByte();
			interlace = reader.ReadByte();
		}

		public int getSamples() {
			switch(colortype) {
				case ColorType.GRAY_ALPHA: return 2;
				case ColorType.RGB: return 3;
				case ColorType.RGB_ALPHA: return 4;
			}
			return 1;
		}

		public bool isInterlaced() { return interlace != 0; }


	}

    public enum ColorType : byte
    {
		/** {@link #IHDR IHDR}: Grayscale color type */
		GRAY = 0,
		/** {@link #IHDR IHDR}: Grayscale+alpha color type */
		GRAY_ALPHA = 4,
		/** {@link #IHDR IHDR}: Palette color type */
		PALETTE = 3,
		/** {@link #IHDR IHDR}: RGB color type */
		RGB = 2,
		/** {@link #IHDR IHDR}: RGBA color type */
		RGB_ALPHA = 6
	}

    public class Png
    {

		public static bool IsPng(Stream stream) {
			for (int i=0; i<8; i++)
				if (stream.ReadByte() != signature[i])
					return false;
			return true;
		}

		const int type_IEND = 0x49454E44;
		const int type_IHDR = 0x49484452;
		const int type_PLTE = 0x504C5445;
		const int type_IDAT = 0x49444154;

		const int type_gAMA = 0x67414d41;
		const int type_tEXt = 0x74455874;
		const int type_tRNS = 0x74524E53;
		const int type_pHYs = 0x70485973;
		const int type_tIME = 0x74494D45;
		const int type_bKGD = 0x624b4744;
		const int type_cHRM = 0x6348524d;
		const int type_hIST = 0x68495354;
		const int type_iCCP = 0x69434350;
		const int type_iTXt = 0x69545874;
		const int type_sBIT = 0x73424954;
		const int type_sPLT = 0x73504c54;
		const int type_sRGB = 0x73524742;
		const int type_zTXt = 0x7a545874;
		const int type_oFFs = 0x6f464673;
		const int type_pCAL = 0x7043414c;
		const int type_sCAL = 0x7343414c;
		const int type_gIFg = 0x67494667;
		const int type_gIFx = 0x67494678;
		const int type_sTER = 0x73544552;

		private static readonly byte[] signature = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 };

		public IHDR ihdr = new IHDR();
		public byte[] palette;
		IPngImageOutput imageOutput;


		void readPLTE(BigEndianBinaryReader reader, int length)
		{
			if(length == 0)
				throw new BadImageFormatException("PLTE chunk cannot be empty");
			if(length % 3 != 0)
				throw new BadImageFormatException("PLTE chunk length indivisible by 3: " + length);
			
			int size = length / 3;
			if(size > 256)
				throw new BadImageFormatException("Too many palette entries: " + size);

			switch (ihdr.colortype)
			{
				case ColorType.PALETTE:
					if (size > (1 << ihdr.bitdepth))
						throw new BadImageFormatException("Too many palette entries: " + size);
					palette = new byte[length];
					reader.Read(palette, 0, length);
					break;
				default:
					throw new BadImageFormatException("PLTE chunk found in non-palette colorformat image");
			}
		}

		void readImagedata(BigEndianBinaryReader reader, int length) {

			int width = ihdr.width;
			int height = ihdr.height;
			int bitDepth = ihdr.bitdepth;
			int samples = ihdr.getSamples();

			bool interlaced = ihdr.isInterlaced();

			SubInputStream sis = new SubInputStream(reader.BaseStream, length);
			InflaterInputStream iis = new InflaterInputStream(sis);
			Defilterer d = new Defilterer(iis, bitDepth, samples, width);

			//todo: interlacing
			imageOutput.start(this);
			d.defilter(0, 0, 1, 1, width, height, imageOutput);
			imageOutput.finish();

			
		}

		public void read(Stream stream, IPngImageOutput imageOutput) {
			this.imageOutput = imageOutput;
			CRCInputStream crcstream = new CRCInputStream(stream);
			BigEndianBinaryReader reader = new BigEndianBinaryReader(crcstream);

			//check signature
			if(!IsPng(stream))
				throw new BadImageFormatException("signature invalid");

			//sort of an inefficient way to do it, but at 3:30am itll do fine
			MemoryStream msIDAT = new MemoryStream();

			int state = 0;
			for(; ; ) {
				bool skipChunk = false;
				
				int chunkLen = reader.ReadInt32();
				crcstream.resetCrc();
				int chunkType = reader.ReadInt32();

				//expect an IHDR
				if(state == 0) {
					if(chunkType != type_IHDR) throw new BadImageFormatException("did not encounter initial IHDR");
					ihdr.read(reader);
					//validate ihdr: do we support interlaced images for now?
					state = 1;
				} else 
				if(state == 1) {
					switch(chunkType) {
						case type_IEND:
							//actually do the imagedata read now
							msIDAT.Position = 0;
							readImagedata(new BigEndianBinaryReader(msIDAT), (int)msIDAT.Length);
							state = 2;
							break;
						case type_IHDR:
							throw new BadImageFormatException("encountered supernumerary IHDR");
						case type_IDAT:
							byte[] buf = new byte[chunkLen];
							reader.BaseStream.Read(buf, 0, chunkLen);
							msIDAT.Write(buf, 0, chunkLen);
							break;
						case type_PLTE:
							readPLTE(reader, chunkLen);
							break;

						default: {
							skipChunk = true;
							byte[] bytes = System.BitConverter.GetBytes(chunkType);
							//debug: which chunk was skipped?
							//Console.WriteLine("{0}{1}{2}{3}", (char)bytes[3], (char)bytes[2], (char)bytes[1], (char)bytes[0]);
						}
						break;
					}
				}

				//if we skipped the chunk, skip the bytes.
				if(skipChunk) {
					reader.BaseStream.Position += chunkLen + 4; //+4 for the crc
				} else {
					//if we didnt skip it, do the crc validation
					if(!skipChunk) {
						int actualCrc = (int)crcstream.Value;
						int targetCrc = reader.ReadInt32();
						if(actualCrc != targetCrc)
							throw new BadImageFormatException("Crc failure");
					}
				}

				if(state == 2) break;
			}

		}
	}
}
