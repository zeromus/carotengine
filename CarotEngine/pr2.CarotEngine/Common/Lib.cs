using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace pr2.Common {

	public static partial class Lib {

		public static Fx32 MoveValueTowards(Fx32 val, Fx32 target, Fx32 incr)
		{
			incr = incr.Abs();
			Fx32 delta = (target-val).Abs();
			bool lt = val<target;
			bool gt = val>target;
			Fx32 old = val;
			if(lt) val += incr;
			else if(gt) val -= incr;
			Fx32 newDelta = (target - val).Abs();
			if((old<=target && val>=target || old>=target && val<=target))
				val = target;
			return val;
		}

		public static float DistanceRads(float start, float target) {
			float upwards, downwards;
			if(start==target) return 0;
			if(start<target) upwards = target-start;
			else upwards = TwoPi - start + target;
			if(start<target) downwards = start + (TwoPi-target);
			else downwards = start-target;
			if(upwards < downwards)
				return upwards;
			else return downwards;

		}

		public static int DirectionTowardsNearestRads(float start, float target) {
			float upwards, downwards;
			if(start==target) return 0;
			if(start<target) upwards = target-start;
			else upwards = TwoPi - start + target;
			if(start<target) downwards = start + (TwoPi-target);
			else downwards = start-target;
			if(upwards < downwards)
				return 1;
			else return -1;
		}

		public static float MoveAngleTowardsNearestRads(float start, float delta, float target) {
			if(DirectionTowardsNearestRads(start,target) == 1)
				return MoveAngleTowardsRads(start, delta, target);
			else return MoveAngleTowardsRads(start, -delta, target);
		}

		public static float MoveAngleTowardsRads(float start, float delta, float target) {
			if(target == start) return start;
			float ret = AddRads(start, delta);
			if(delta>0) {
				if(start>target) {
					if(ret>target && ret<start)
						return target;
				} else {
					if(ret>target || ret < start)
						return target;
				}
			} else {
				if(start>target) {
					if(ret<target || ret > start)
						return target;
				} else
					if(ret<target && ret>start)
						return target;
			}
			return ret;
		}

		public static float AddRads(float val, float delta) {
			val += delta;
			val += TwoPi;
			val %= (2*Pi);
			return val;
		}

		private static float TwoPi = (float)Math.PI*2;
		private static float Pi = (float)Math.PI;

		/// <summary>
		/// converts a glob pattern into a regex pattern;
		/// NOT TESTED
		/// </summary>
		public static string glob(string pattern) {
			//we dont want backslashes in input pattern
			pattern = pattern.Replace(@"\",@"\\");
			//we dont want . in input pattern
			pattern = pattern.Replace(@".",@"\.");
			//we dont want [ or ] in input pattern
			pattern = pattern.Replace(@"[",@"\[");
			pattern = pattern.Replace(@"]",@"\]");
			//we dont want ( or ) in input pattern
			pattern = pattern.Replace(@"(",@"\(");
			pattern = pattern.Replace(@")",@"\)");
			//we dont want { or } in input pattern
			pattern = pattern.Replace(@"{",@"\{");
			pattern = pattern.Replace(@"}",@"\}");
			//replace valid wildcards with regexp equivalents
			pattern = pattern.Replace('?', '.').Replace("*", ".*?");

			return pattern;
		}

		/// <summary>
		/// Compares two byte arrays with a semantics similar to strings
		/// </summary>
		public static bool compareBytes(byte[] a, byte[] b) {
			if(a == null) if(b == null) return true;
				else return false;
			else if(b == null) return false;

			if(a.Length != b.Length) return false;
			int cnt = a.Length;
			for(int i = 0; i < cnt; i++) if(a[i] != b[i]) return false;
			return true;
		}

		/// <summary>
		/// converts degrees to radians
		/// </summary>
		public static float Rads(float degrees) {
			return degrees / 360.0f * (float)((float)Math.PI * 2.0f);
		}
		/// <summary>
		/// converts degrees to radians
		/// </summary>
		public static double Rads(double degrees) {
			return degrees / 360.0d * (double)((double)Math.PI * 2.0d);
		}

		/// <summary>
		/// converts radians to degrees
		/// </summary>
		public static float Degrees(float rads) {
			return rads / 2.0f / (float)Math.PI * 360.0f;
		}

		/// <summary>
		/// converts radians to degrees
		/// </summary>
		public static double Degrees(double rads) {
			return rads / 2.0d / (double)Math.PI * 360.0d;
		}

		/// <summary>
		/// returns the scale*sin(phase) with phase determined by timer and rate
		/// </summary>
		public static int TimerSin(int timer, int rate, int scale) {
			return TimerSin(timer, rate, -scale, scale);
		}

		/// <summary>
		/// returns the scale*sin(phase) with phase determined by timer and rate
		/// </summary>
		public static double TimerSin(int timer, double rate, double scale) {
			return TimerSin(timer, rate, -scale, scale);
		}

		/// <summary>
		/// returns the scale*sin(phase) with phase determined by timer and rate
		/// </summary>
		public static float TimerSin(int timer, float rate, float scale) {
			return (float)TimerSin(timer, rate, -scale, scale);
		}


		/// <summary>
		/// returns sin(phase) with phase determined by timer and rate, scaled to the specified range
		/// </summary>
		public static double TimerSin(int timer, double rate, double min, double max) {
			double range = ((max - min) + 0.999) / 2.0;
			double phase = (double)(timer % rate) / rate * Math.PI * 2.0;
			double sin = Math.Sin(phase);
			return ((range * sin) + range) + min;
		}

		/// <summary>
		/// returns sin(phase) with phase determined by timer and rate, scaled to the specified range
		/// </summary>
		public static float TimerSin(int timer, float rate, float min, float max) {
			float range = ((max - min)) / 2.0f;
			float phase = (timer % rate) / rate * (float)Math.PI * 2.0f;
			float sin = (float)Math.Sin(phase);
			return ((range * sin) + range) + min;
		}

		/// <summary>
		/// returns sin(phase) with phase determined by timer and rate, scaled to the specified range
		/// </summary>
		public static int TimerSin(int timer, int rate, int min, int max) {
			return (int)TimerSin(timer, rate, (float)min, (float)max);
		}

		/// <summary>
		/// Swaps two things. NOW! ACTUALLY WORKS!
		/// </summary>
		public static void swap<T>(ref T x, ref T y) {
			T temp = x;
			x = y;
			y = temp;
		}

		//public static Color mixColors(Color a, Color b, float ratio) {
		//    int nr = a.R + (int)((float)(b.R - a.R) * ratio);
		//    int ng = a.G + (int)((float)(b.G - a.G) * ratio);
		//    int nb = a.B + (int)((float)(b.B - a.B) * ratio);
		//    int na = a.A + (int)((float)(b.A - a.A) * ratio);
		//    return Color.FromArgb(na, nr, ng, nb);
		//}

		/// <summary>
		/// logs the given text. if logging is not enabled, this will silently do nothing
		/// </summary>
		public static void log(string format, params object[] args) { Log.log(format, args); }

		//TODO --------- add all of the below into a list manager helper class

		/// <summary>
		/// this function returns the appropriate scroll offset for a list
		/// with the given current position, number of items,
		/// size of viewable area (in numbers of items), and index of center of viewable area
		/// (e.g., size=5, center=2)
		/// </summary>
		public static int listMagic(int cur, int num, int size, int center) {
			//return max(0,min(num-size,cur-center));
			int a = num-size;
			int b = cur-center;
			int c;
			if(a<b) c = a; else c = b;
			if(c<0) return 0; else return c;
		}

		/// <summary>
		/// increments a value, clipping to the given min and max
		/// </summary>
		public static void listClip(ref int src, int add, int min, int max) { src = listClip(src, add, min, max); }

		/// <summary>
		/// increments a value, clipping to the given min and max
		/// </summary>
		public static int listClip(int src, int add, int min, int max) {
			int s = src;
			s += add;
			if(s>max) s = max;
			else if(s<min) s = min;
			src = s;
			return src;
		}

		/// <summary>
		/// increments a value, wrapping through the given minimums and maximums
		/// this seems crappy somehow but im too busy to figure it out
		/// </summary>
		public static void listWrap(ref int src, int add, int min, int max) { src = listWrap(src, add, min, max); }

		/// <summary>
		/// increments a value, wrapping through the given minimums and maximums
		/// this seems crappy somehow but im too busy to figure it out
		/// </summary>
		public static int listWrap(int src, int add, int min, int max) {
			int s = src;
			int range = max - min + 1;
			s += add;
			bool cont = true;
			while(cont) {
				if(s>max) s -= range;
				else if(s<min) s += range;
				else cont = false;
			}
			src = s;
			return src;
		}

	

		public static T[] expandArray<T>(T[] arr) {
			Array.Resize(ref arr, arr.Length * 2);
			return arr;
		}

		/// <summary>
		/// changes 24bpp to 32bpp and replaces tcolors with color zero (black transparent)
		/// </summary>
		public unsafe static void load24(void *dest, void *src, int size, int tcolor) {
		    int *idest = (int *)dest;
		    int *destmax = idest + size;
		    byte *bsrc = (byte*)src;
		    int tcol = tcolor&0x00FFFFFF;
		    for(;idest<destmax;idest++,bsrc+=3) {
				int col = (bsrc[2]<<16)|(bsrc[1]<<8)|bsrc[0];
		        if(col==tcol)
		            *idest = 0;
				else *idest = col | unchecked((int)0xFF000000);
		    }					
		}

		/// <summary>
		/// replaces tcolors with color zero (black transparent)
		/// </summary>
		public static unsafe void alpha32(void *data, int size, int tcolor) {
			int *idata = (int *)data;
			int *datamax = idata+size;
			int col = tcolor;
			for(; idata < datamax; idata++)
				if(*idata == col)
					*idata = 0;
		}

		/// <summary>
		/// premultiplies a color channel with an alpha channel.
		/// </summary>
		public static unsafe void Premultiply(void *data, int size) {
			uint* nsrc = (uint*)data;
			for(int i=0;i<size;i++) {
				uint col = *nsrc;
				uint a = col & 0xFF;
				uint x = (((col>>24)&0xFF)*a)>>8;
				uint y = (((col>>16)&0xFF)*a)>>8;
				uint z = (((col>>8)&0xFF)*a)>>8;
				col = (x<<24)|(y<<16)|(z<<8)|a;
				*nsrc++ = col;
			}			
		}

		/// <summary>
		/// turns all pixels opaque
		/// </summary>
		public static unsafe void dealpha32(void* data, int size) {
			int* idata = (int*)data;
			int* datamax = idata + size;
			for(; idata < datamax; idata++)
				*idata |= unchecked((int)0xFF000000);
		}

		//not being used
		//transforms 8bit input + palette to a 32bpp output
		//with color0 from the input going to transparent
		//static void alpha8(void *dest, void *src, void *palette, int size)
		//{
		//    int *idest = (int *)dest;
		//    int *destmax = idest + size;
		//    unsigned char *bsrc = (unsigned char *)src;
		//    unsigned char *bpal = (unsigned char *)palette;
		//    for(;idest<destmax;idest++,bsrc++) {
		//        *idest = (*(int *)(bpal+ *bsrc*3)) & 0x00FFFFFF;
		//        if(*bsrc)
		//            *idest |= 0xFF000000;
		//    }
		//}

		//#if !XBOX360
		//public unsafe static void memcpy(void* dest, void* src, int len) {			
		//    Interop.RtlMoveMemory(dest,src,len);
		//}

		//public static unsafe void memset(void* dest, byte value, int count) {
		//    Interop.RtlFillMemory(dest, count, value);
		//}
		//#else

		//todo - faster
		public unsafe static void memcpy(byte* dest, byte* src, int len) {
			for(int i=0; i<len; i++)
				*dest++ = *src++;
		}
		public unsafe static void memcpy(void* dest, void* src, int len) {
			memcpy((byte*)dest,(byte*)src,len);
		}

		public static unsafe void memset(void* dest, byte value, int count) {
			byte* bdest = (byte*)dest;
			for(int i=0; i<count; i++)
				*bdest++ = value;
		}

		/// <summary>
		/// integer ROL function
		/// </summary>
		public static int RotateLeft(int value, int nBits) {
			nBits = nBits % 0x20;
			return ((value << (nBits & 0x1f)) | (value >> ((0x20 - nBits) & 0x1f)));
		}


		/// <summary>
		/// regular grump with different args and all float. ratio is width divided by height
		/// </summary>
		public static void grump(ref float w, ref float h, float ratio) {
			float iw, ih;

			//target width, given the height
			float tw = h*ratio;

			if(tw > w) {
				//if the target width is too wide, then we need to squeeze horizontally to fit
				//which means letterbox
				iw = w;
				ih = w / ratio;
			} else {
				//otherwise, the target width equals or is less than the horizontal room available.
				//if thats the case, we may need to sidebar
				ih = h;
				iw = tw;
			}

			w = iw;
			h = ih;
		}

		public static void grump(ref int w, ref int h, int xres, int yres)
		{
			int iw,ih;

			float ratio = (float)w / xres;
			float ph = ratio * (float)yres;

			if((int)ph>h) {
				ratio = (float)h / (float)yres;
				ih = h;
				iw = (int)(ratio * (float)xres);
			} else {
				ih = (int)ph;
				iw = w;
			}

			w = iw;
			h = ih;
		}

		/// <summary>
		/// returns the GCD of two numbers
		/// </summary>
		public static long gcd(long u, long v) {
			int k = 0;
			if(u == 0)
				return v;
			if(v == 0)
				return u;
			while((u & 1) == 0 && (v & 1) == 0) { /* while both u and v are even */
				u >>= 1;   /* shift u right, dividing it by 2 */
				v >>= 1;   /* shift v right, dividing it by 2 */
				k++;       /* add a power of 2 to the final result */
			}
			/* At this point either u or v (or both) is odd */
			do {
				if((u & 1) == 0)      /* if u is even */
					u >>= 1;           /* divide u by 2 */
				else if((v & 1) == 0) /* else if v is even */
					v >>= 1;           /* divide v by 2 */
				else if(u >= v)       /* u and v are both odd */
					u = (u - v) >> 1;
				else                   /* u and v both odd, v > u */
					v = (v - u) >> 1;
			} while(u > 0);
			return v << k;  /* returns v * 2^k */
		}

		/// <summary>
		/// returns the GCD of the set of values
		/// </summary>
		public static long gcd(IEnumerable<long> values) {
			List<long> a = new List<long>();
			List<long> b = new List<long>();
			foreach(long x in values)
				if(!a.Contains(x))
					foreach(long y in values) {
							a.Add(x);
							b.Add(y);
						}
			if(a.Count == 0)
				return 0;
			
			long min = gcd(a[0],b[0]);
			for(int i = 1; i < a.Count; i++) {
				long temp = gcd(a[i], b[i]);
				if(temp<min) min = temp;
			}
			return min;
		}


		/// <summary>
		/// Gets the path to a file accompanying the file specified by the first path
		/// (for finding vsps in the same folder as maps, for example)
		/// </summary>
		public static string GetAccompanyingFilePath(string path, string fname) {
			int separator = path.Replace('\\','/').LastIndexOf('/');
			if(separator != -1)
			    fname = path.Substring(0, separator + 1) + fname;
			return fname;
		}


		///// <summary>
		///// Registers a filetype extension in a fairly universal way
		///// </summary>
		//public static void RegisterFileType(string Extension, string Description) {
		//    string keyed = Extension.Substring(1) + "_file";

		//    RegistryKey rk = Registry.ClassesRoot.CreateSubKey(keyed);
		//    rk.SetValue("", Description);
		//    rk.Close();

		//    string fname = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
		//    rk = Registry.ClassesRoot.CreateSubKey(keyed + "\\DefaultIcon");
		//    rk.SetValue("", fname);

		//    rk = Registry.ClassesRoot.CreateSubKey(keyed + "\\shell\\open\\command");
		//    rk.SetValue("", "\"" + System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName + "\" \"%1\"");
		//    rk.Close();

		//    rk = Registry.ClassesRoot.CreateSubKey(Extension);
		//    rk.SetValue("", keyed);
		//    rk.Close();
		//}


		/// <summary>
		/// Casts an enumerator using `as` (possibly returning many nulls from the converted enumerator)
		/// </summary>
		public static IEnumerable<TO> castIEnumerablesAs<TO>(IEnumerable from) where TO : class {
			return new LightweightEnumerable<TO>(from);
		}

		class LightweightEnumerable<TO> : IEnumerable<TO> where TO : class {
			IEnumerable _fromEnumerable;
			public LightweightEnumerable(IEnumerable fromEnumerable) { _fromEnumerable = fromEnumerable; }
			IEnumerator<TO> IEnumerable<TO>.GetEnumerator() { return new Enumerator(_fromEnumerable.GetEnumerator()); }
			IEnumerator IEnumerable.GetEnumerator() { return new Enumerator(_fromEnumerable.GetEnumerator()); }
			class Enumerator : IEnumerator<TO> {
				IEnumerator _fromEnumerator;
				public Enumerator(IEnumerator fromEnumerator) { _fromEnumerator = fromEnumerator; }
				TO IEnumerator<TO>.Current { get { return _fromEnumerator.Current as TO; } }
				void IDisposable.Dispose() { }
				object IEnumerator.Current { get { return _fromEnumerator.Current as TO; } }
				bool IEnumerator.MoveNext() { return _fromEnumerator.MoveNext(); }
				void IEnumerator.Reset() { _fromEnumerator.Reset(); }
			}
		}

	}
}