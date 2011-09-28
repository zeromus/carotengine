using System;
using System.Runtime.InteropServices;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//todo - this whole enchilada needs to be reworked to use pointers
//TODO - there are bugs in here leftover from when i switched to int* from byte*
// check for *4 that are dangerous!

namespace pr2.CarotEngine {

	public class SoftGraphics : IDisposable {
		float _lucent;
		public int tcolor;

		public float lucent { get { return _lucent; } set { _lucent = value; } }
		public int intlucent { get { return 255 - (int)(_lucent * 255.9999f); } }

		public int makeColor(int r, int g, int b) { return unchecked((int)0xFF000000) | ((r << 16) | (g << 8) | b); }
		public void getColor(int c, ref int r, ref int g, ref int b) {
			b = c & 0xff;
			g = (c >> 8) & 0xff;
			r = (c >> 16) & 0xff;
		}

		public SoftGraphics() {
			tcolor = makeColor(255, 0, 255);
			_lucent = 1.0f;
		}

		public void Dispose() { }

		/// <summary>
		/// source data must be no pitch linear
		/// </summary>
		public unsafe Softimage imageFrom24bpp(int w, int h, byte[] data) {
			Softimage img = new Softimage(w,h);
			int* idptr = img.data;
			byte* dptr = (byte*)idptr;
			fixed(byte* csptr = data) {
				byte* sptr = csptr;
				for(int y = 0; y < h; y++) {
					for(int x = 0; x < w; x++) {
					#if XBOX360
						*dptr++ = 255;
						*dptr++ = sptr[2];
						*dptr++ = sptr[1];
						*dptr++ = sptr[0];
						sptr += 3;
					#else
						*dptr++ = sptr[0];
						*dptr++ = sptr[1];
						*dptr++ = sptr[2];
						*dptr++ = 255;
						sptr += 3;
					#endif
					}
					dptr = dptr + (img.w - img.p) * 4;
				}
			}
			return img;
		}

		public Softimage newImage(int w, int h) {
			return new Softimage(w,h);
		}


		bool clip2(ref int x0, ref int y0, ref int xlen, ref int ylen, ref int s, ref int d, [In] int spitch, [In] int dpitch, int cx1, int cy1, int cx2, int cy2) {
			if(x0>cx2 || y0>cy2 || x0+xlen<cx1 || y0+ylen<cy1)
				return true;

			if(x0+xlen>cx2) 
				xlen = cx2-x0+1;

			if (y0+ylen>cy2) 
				ylen = cy2-y0+1;

			if (x0<cx1)
			{
				s +=(cx1-x0);
				xlen-=(cx1-x0);
				x0  =cx1;
			}
			if (y0<cy1)
			{
				s +=(cy1-y0)*spitch;
				ylen-=(cy1-y0);
				y0  =cy1;
			}

			d += (y0 * dpitch) + x0;

			return false;
		}

		bool CLIP2(Softimage src, Softimage dest, ref int x, ref int y, out int xlen, out int ylen, out int s, out int d, out int spitch, out int dpitch) {
			xlen=src.w;
			ylen=src.h;
			s = 0;
			d = 0;
			spitch = src.p;
			dpitch = dest.p;
			if(clip2(ref x, ref y, ref xlen, ref ylen, ref s, ref d, spitch, dpitch, dest.cx, dest.cy, dest.cx1, dest.cy1))
				return true;
			else return false;
		}

		//eh? doesnt mix alpha?
		int mixLucent(int src, int dest, int lucent) {
			int z = 256 - lucent;
			int r = (((src & 0x00FF0000) * z + (dest & 0x00FF0000) * lucent) + 128) & unchecked((int)0xFF000000);
			int g = (((src & 0x0000FF00) * z + (dest & 0x0000FF00) * lucent) + 128) & 0x00FF0000;
			int b = (((src & 0x000000FF) * z + (dest & 0x000000FF) * lucent) + 128) & 0x0000FF00;
			return (r | g | b) >> 8;
		}

		public unsafe void blit(int x, int y, Softimage src, Softimage dest) {
			int xlen, ylen, s, d, spitch, dpitch;
			if(CLIP2(src, dest, ref x, ref y, out xlen, out ylen, out s, out d, out spitch, out dpitch))
				return;

			int luc = intlucent;
			if(luc != 0) {
				for(; ylen > 0; ylen--)
					for(x = 0; x < xlen; x++)
						dest.data[d++] = mixLucent(src.data[s++], dest.data[d], luc);
				s += spitch;
				d += dpitch;
			} else {
				xlen *= 4;

				while(ylen-- > 0) {
					pr2.Common.Lib.memcpy(dest.data+d, src.data+s, xlen);
					s += spitch;
					d += dpitch;
				}
			}
		}
	
		public void blitSubrect(int sx, int sy, int sw, int sh, int dx, int dy, Softimage src, Softimage dest) {
			Rectangle oldclip = dest.clip;
			dest.setClip(dx, dy, sw, sh);
			blit(dx - sx, dy - sy, src, dest);
			dest.clip = oldclip;
		}
	}


	public unsafe class Softimage : IDisposable {
		internal int w, h, p;
		internal int* data;
		int[] arr;
		internal int cx, cy; 
		internal int cx1, cy1;
		//int cw,ch
		Rectangle _clip;

		//SoftGraphics g;
		GCHandle hnd;
		bool isHandleUsed;
		

		public Rectangle clip { get { return _clip; } set { setClip(value.X, value.Y, value.Width, value.Height); } }
		public int Width { get { return w; } }
		public int Height { get { return h; } }
		public int pitch { get { return p; }  }

		/// <summary>
		/// makes a new softimage with the given dimensions
		/// </summary>
		public Softimage(int width, int height) {
			p = w = width;
			h = height;
			alloc();
			initClip();
		}

		void alloc() {
			arr = new int[w * h];
			lockArray();
		}

		void lockArray() {
			hnd = GCHandle.Alloc(arr, GCHandleType.Pinned);
			isHandleUsed = true;
			data = (int*)hnd.AddrOfPinnedObject().ToPointer();
		}

		/// <summary>
		/// clones a softimage
		/// </summary>
		Softimage(Softimage source) {
			p = w = source.w;
			h = source.h;
			alloc();
			source.dump(data);
			initClip();
		}

		/// <summary>
		/// wraps a buffer with a softimage
		/// </summary>
		public Softimage(int[] data, int width, int pitch, int height) {
			this.arr = data;
			lockArray();
			p = pitch;
			w = width;
			h = height;
		}

		/// <summary>
		/// wraps a buffer with a softimage
		/// </summary>
		public Softimage(IntPtr data, int width, int pitch, int height) {
			this.data = (int*)data.ToPointer();
			p = pitch;
			w = width;
			h = height;
		}

		#if !XBOX360
		public System.Drawing.Bitmap ToSystemDrawingBitmap() {
			System.Drawing.Bitmap ret = new System.Drawing.Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			System.Drawing.Imaging.BitmapData bmpdata = ret.LockBits(new System.Drawing.Rectangle(0, 0, Width, Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			Softimage dest = new Softimage(bmpdata.Scan0, Width, bmpdata.Stride, Height);
			dump(dest.data);
			swapBytes(dest.data);
			ret.UnlockBits(bmpdata);
			return ret;
		}
		public Softimage(System.Drawing.Bitmap bmp) {
			p = w = bmp.Width;
			h = bmp.Height;
			alloc();

			System.Drawing.Imaging.BitmapData bmpdata = bmp.LockBits(new System.Drawing.Rectangle(0, 0, Width, Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			Softimage src = new Softimage(bmpdata.Scan0, Width, bmpdata.Stride/4, Height);
			src.dump(data);
			bmp.UnlockBits(bmpdata);
		}
		#endif
		

		///// <summary>
		///// wraps a buffer with a softimage
		///// </summary>
		//public Softimage(int[] data, int width, int pitch, int height) {

		//}

		
		//not used yet
		//Softimage shell(int x, int y, int w, int h)
		//{
		//    if(x+w>this->w || y+h>this->h) throw new Exception("x/y+w/h greater than original image dimensions in shellImage");
		//    return Softimage(this,x,y,w,h);
		//}

		Softimage copy()
		{
			return new Softimage(this);
		}

		/// <summary>
		/// loads image data from supplied int pointer. it had better have the same pitch as width
		/// </summary>
		public void load(int* src) {
			int* cdptr = data;
			int *dptr = cdptr;
			if(w == p) {
				int len = w*h;
				for(int i=0;i<len;i++)
					*src++ = *dptr++;
			} else {
				int d = 0, s = 0;
				int len = w, sadd = w, dadd = p;
				int y = h;
				while(y-- != 0) {
					for(int i = 0; i < len; i++)
						*src++ = *dptr++;
					s += sadd-len;
					d += dadd-len;
				}
			}
		}

		/// <summary>
		/// loads image data from supplied int array. it had better have the same pitch as width
		/// </summary>
		public void load(int[] src) {
			fixed(int* sptr = src)
				load(sptr);
		}

		void swapBytes(int* dest)
		{
			int d = 0, s = 0;
			int len = w * 4, sadd = p * 4, dadd = w * 4;
			int y = h;
			while (y-- != 0)
			{
				for (int x = 0; x < w; x++)
				{
					byte a = ((byte*)data + s)[x * 4 + 0];
					byte b = ((byte*)data + s)[x * 4 + 1];
					byte c = ((byte*)data + s)[x * 4 + 2];
					byte e = ((byte*)data + s)[x * 4 + 3];
					((byte*)dest + d)[x * 4 + 0] = c;
					((byte*)dest + d)[x * 4 + 1] = b;
					((byte*)dest + d)[x * 4 + 2] = a;
					((byte*)dest + d)[x * 4 + 3] = e;
				}
				s += sadd;
				d += dadd;
			}
		}

		/// <summary>
		/// dumps image data to the supplied int pointer. 
		/// </summary>
		public void dump(int* dest) {
			bool easy = (w == p);
			#if !XBOX360
				//easy = false;
			#endif
			if(easy)			
				pr2.Common.Lib.memcpy(dest, data, w*h*4);
			else {
				int d = 0, s = 0;
				int len = w * 4, sadd = p * 4, dadd = w * 4;
				int y = h;
				while(y-- != 0) {
					#if XBOX360
						pr2.Common.Lib.memcpy(dest+d, data+s, len);
					#else
						for(int x=0;x<w;x++)
						{
							((byte*)dest+d)[x*4+0] = ((byte*)data+s)[x*4+3];
							((byte*)dest+d)[x*4+1] = ((byte*)data+s)[x*4+2];
							((byte*)dest+d)[x*4+2] = ((byte*)data+s)[x*4+1];
							((byte*)dest+d)[x*4+3] = ((byte*)data+s)[x*4+0];
						}
					#endif
					s += sadd;
					d += dadd;
				}
			}
		}

		void initClip() { setClip(0, 0, w, h); }

		public void setClip(int x, int y, int w, int h) {
			if(x<0) x=0;
			if(y<0) y=0;
			cx = x; cy = y; 
			cx1 = cx+w-1;
			cy1 = cy+h-1;
			if(cx1>=this.w) cx1=this.w-1;
			if(cy1>=this.h) cy1=this.h-1;
			//cw = cx1-cx+1;
			//ch = cy1-cy+1;

			
			
			_clip = new Rectangle(x,y,w,h);
		}

		public void putPixel(int x, int y, int color) {
			if(x < cx) return;
			if(y < cy) return;
			if(x > cx1) return;
			if(y > cy1) return;
			data[y * p + x] = color;
		}

		public int GetPixel(int x, int y) {
			//mbg 14-sep-07 I noticed this here. why in the hell would you want this??
			//if(x < 0) return 0;
			//if(y < 0) return 0;
			//if(x >= w) return 0;
			//if(y >= h) return 0;
			return data[y * p + x];
		}

		~Softimage() {
			if(isHandleUsed)
				hnd.Free();
			isHandleUsed = false;
		}

		//bool bDisposed = false;
		public void Dispose() {
		//	bDisposed = true;
		}


	}

}