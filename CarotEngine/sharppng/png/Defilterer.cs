using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace pr2.sharppng {

	class Defilterer {
		public Defilterer(Stream stream, int bitDepth, int samples, int width) {
			this.stream = stream;
			this.bitDepth = bitDepth;
			this.samples = samples;
			this.width = width;
			//this.pp = pp;
			bpp = Math.Max(1, (bitDepth * samples) >> 3);
		}

		void fillBuffer(byte[] buf, int off, int len)
		{
			if(len == 1) {
				int temp = stream.ReadByte();
				if(temp == -1) throw new EndOfStreamException();
				buf[0] = (byte)temp;
				return;
			} else {
				int ctr = 0;
				do {
					int temp = stream.Read(buf, off + ctr, len - ctr);
					if(temp == 0) throw new EndOfStreamException();
					ctr += temp;
				} while(ctr < len);
			}
		}

		Stream stream;
		int width;
		int bitDepth;
		int samples;
		//private PixelProcessor pp;
		int bpp;

		private static void defilter(byte[] cur, byte[] prev, int bpp, int filterType) {
			int rowSize = cur.Length;
			int xc, xp;
			switch (filterType) {
			case 0: // None
				break;
			case 1: // Sub
				for (xc = bpp, xp = 0; xc < rowSize; xc++, xp++)
					cur[xc] = (byte)(cur[xc] + cur[xp]);
				break;
			case 2: // Up
				for (xc = bpp; xc < rowSize; xc++)
					cur[xc] = (byte)(cur[xc] + prev[xc]);
				break;
			case 3: // Average
				for (xc = bpp, xp = 0; xc < rowSize; xc++, xp++)
					cur[xc] = (byte)(cur[xc] + ((0xFF & cur[xp]) + (0xFF & prev[xc])) / 2);
				break;
			case 4: // Paeth
				for (xc = bpp, xp = 0; xc < rowSize; xc++, xp++) {
					byte L = cur[xp];
					byte u = prev[xc];
					byte nw = prev[xp];
					int a = 0xFF & L; //  inline byte->int
					int b = 0xFF & u; 
					int c = 0xFF & nw; 
					int p = a + b - c;
					int pa = p - a; if (pa < 0) pa = -pa; // inline Math.abs
					int pb = p - b; if (pb < 0) pb = -pb; 
					int pc = p - c; if (pc < 0) pc = -pc;
					int result;
					if (pa <= pb && pa <= pc) {
						result = a;
					} else if (pb <= pc) {
						result = b;
					} else {
						result = c;
					}
					cur[xc] = (byte)(cur[xc] + result);
				}
				break;
			default:
				throw new Exception("Unrecognized filter type " + filterType);
			}
		}


		public void defilter(int xOffset, int yOffset,
								int xStep, int yStep,
								int passWidth, int passHeight, IPngImageOutput output)
		{
        if (passWidth == 0 || passHeight == 0)
            return;

        int bytesPerRow = (bitDepth * samples * passWidth + 7) / 8;
        bool isShort = bitDepth == 16;
        
        int rowSize = bytesPerRow + bpp;
        byte[] prev = new byte[rowSize];
        byte[] cur = new byte[rowSize];

        for (int srcY = 0, dstY = yOffset; srcY < passHeight; srcY++, dstY += yStep) {
            int filterType = stream.ReadByte();
			if(filterType == -1)
				throw new EndOfStreamException();
            fillBuffer(cur, bpp, bytesPerRow);
            defilter(cur, prev, bpp, filterType);
			output.writeLine(cur, bpp);

            byte[] tmp = cur;
            cur = prev;
            prev = tmp;
        }
	}
	}
}
