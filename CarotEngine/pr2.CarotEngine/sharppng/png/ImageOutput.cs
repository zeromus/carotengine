using System;
using System.Collections.Generic;
using System.Text;

namespace pr2.sharppng {
	public interface IPngImageOutput {
		void start(Png png);
		void writeLine(byte[] data, int offset);
		void finish();
	}

    //class SysdrawingImageOutput : IPngImageOutput {

    //    public void start(int width, int height, ColorType colortype, int bitdepth) {
    //        switch(colortype) {
    //            case ColorType.GRAY:
    //                throw new NotSupportedException(); //support these later
    //            case ColorType.GRAY_ALPHA:
    //                throw new NotSupportedException(); //support these later
    //            case ColorType.PALETTE:
    //                throw new NotSupportedException(); //support these later
    //            case ColorType.RGB:
    //                if(bitdepth == 16) throw new NotSupportedException();
    //                else pf = PixelFormat.Format24bppRgb;
    //                break;
    //            case ColorType.RGB_ALPHA:
    //                //if(bitdepth == 16) throw new NotSupportedException();
    //                //else pf = PixelFormat.Format32bppArgb;
    //                throw new NotSupportedException(); //support these later
    //                break;
    //        }

    //        bmp = new Bitmap(width, height, pf);
    //        bmpdata = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, pf);
    //    }

    //    public void finish() {
    //        bmp.UnlockBits(bmpdata);
    //    }

    //    public void writeLine(byte[] data, int offset) {
    //        IntPtr lineptr = new IntPtr(bmpdata.Scan0.ToInt32() + linecounter * bmpdata.Stride);
    //        switch(pf) {
    //            case PixelFormat.Format32bppArgb:
    //                throw new NotSupportedException(); //support these later
    //                break;
    //            case PixelFormat.Format24bppRgb: {
    //                //swap r and b
    //                byte[] temp = new byte[bmp.Width*3];
    //                for(int i=0,x=0;i<bmp.Width;i++,x+=3) {
    //                    temp[x] = data[x+offset+2];
    //                    temp[x+1] = data[x+offset+1];
    //                    temp[x+2] = data[x+offset];
    //                }
    //                System.Runtime.InteropServices.Marshal.Copy(temp, 0, lineptr, bmp.Width*3);
    //                break;
    //            }
    //        }
    //        linecounter++;
    //    }

    //    int linecounter;
    //    public Bitmap bmp;
    //    PixelFormat pf;
    //    BitmapData bmpdata;
    //}
}
