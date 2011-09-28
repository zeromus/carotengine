using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace pr2.CarotEngine {

	//what a mess! but it works
	public class V3Sysfont : FontRendererBase, IDisposable {

		public bool IsVariableWidth = true;

		public void Dispose() {
			//foreach(Image fi in images)
			//    fi.Dispose();
			image.Dispose();
		}

		static V3Sysfont() {
			smal_tbl = new byte[][]
			{ sBlank,
				sYow,  sQuote,    sNum,   sBuck,sPercnt, sCarot, sQuotes, slParen,
			 srParen,    star,    sPtr,  sComma, sMinus,sPeriod,  sSlash,      s0,
				  s1,      s2,      s3,      s4,     s5,     s6,      s7,      s8,
				  s9,  sColon,  ssemic,      ss,    sEq,    sra,  sQuest,     sAT,
				 sbA,     sbB,     sbC,     sbD,    sbE,    sbF,     sbG,     sbH,
				 sbI,     sbJ,     sbK,     sbL,    sbM,    sbN,     sbO,     sbP,
				 sbQ,     sbR,     sbS,     sbT,    sbU,    sbV,     sbW,     sbX,
				 sbY,     sbZ,      ss, sbSlash,     ss, sCarot,     usc,     sch,
				 ssA,     ssB,     ssC,     ssD,    ssE,    ssF,     ssG,     ssH,
				 ssI,     ssJ,     ssK,     ssL,    ssM,    ssN,     ssO,     ssP,
				 ssQ,     ssR,     ssS,     ssT,    ssU,    ssV,     ssW,     ssX,
				 ssY,     ssZ,      ss,  target,  check,  sCopy,  sBlock,     ss};
		}



		public V3Sysfont(int scale, Color color) { init(scale, color, false, false, Color.Black); }
		V3Sysfont() {}
		public static V3Sysfont CreateDropDownAndRight(int scale, Color color, Color color2) {
			V3Sysfont ret = new V3Sysfont();
			ret.init(scale, color, true, true, color2);
			return ret;
		}

		void init(int scale, Color color, bool dropDown, bool dropRight, Color dropShadowColor) {
			int numchars = smal_tbl.Length;
			_ge = GameEngine.Game;
			_scale = scale;
			_color = color;
			//images = new Image[numchars];
			rects = new Rectangle[numchars];
			
			//figure out the character layout 
			int px=0, py=0;
			int ih = 7 * scale;
			if(dropDown) ih++;
			int maxw = 0;
			for(int i = 0; i < numchars; i++) {
				int cw = smal_tbl[i][0];
				int iw = cw * scale;
				if(dropRight) iw++;

				if(px + iw > 2048) {
					py += ih;
					px = 0;
				}

				Rectangle r = new Rectangle(px, py, iw, ih);
				rects[i] = r;
				px += iw;
				maxw = Math.Max(maxw,px);
			}

			//now generate the image
			image = _ge.NewImage(maxw, py+ih);
			Blitter b = new Blitter(image);
			b.Clear(Color.Transparent);
			for(int i = 0;i < numchars;i++) {
				

				Rectangle rect = rects[i];

				//draw the dropshadows
				if(dropDown) {
					rect.Y++;
					rect.Height--;
					
					b.ApplyWindow(rect);
					print_char(b, i, dropShadowColor);
					b.PopWindow();

					rect.Y--;
				}

				if(dropRight) {
					rect.X++;
					rect.Width--;

					b.ApplyWindow(rect);
					print_char(b, i, dropShadowColor);
					b.PopWindow();

					rect.X--;
				}

				if(dropRight && dropDown) {
					rect.X++;
					rect.Y++;
					b.ApplyWindow(rect);
					print_char(b, i, dropShadowColor);
					b.PopWindow();
					rect.X--;
					rect.Y--;
				}

				//draw the main character:
				b.ApplyWindow(rect);
				print_char(b, i, color);
				b.PopWindow();

			}
			image.Cache();
		}

		int CharSize(int c)
		{
			int size = (smal_tbl[c][0] + 1);
			if (!IsVariableWidth)
				size = 5;
			return size;
		}

		public int height { get { return 7 * _scale; } }
		public override int MeasureWidth(string str) {
			int len = 0;
			for(int i = 0; i < str.Length; i++) {
				int c = (int)str[i];
				c -= 32;
				if(c < 0 || c > 96) c = 2;
				len += CharSize(c) * _scale;
			}
			//remove one for the last line spacing
			len -= _scale;
			return len;
		}

		void print_char(Blitter b, int c, Color col) {
			if(c < 0 || c > 96) c = 2;
			byte[] img = smal_tbl[c];
			int imgptr = 1;
			int w = img[0];
			for(int yc = 0; yc < 7; yc++)
				for(int xc = 0; xc < w; xc++) {
					int p = img[imgptr++];
					if(p == 1)
						for(int px = 0; px < _scale; px++)
							for(int py = 0; py < _scale; py++)
								b.SetPixel(xc * _scale + px, yc * _scale + py, col);
				}
		}

		void render_char(Blitter b, ref int x, ref int y, int c) {
			if(c < 32) return;
			c -= 32;
			if(c < 0 || c > 96) c = 2;
			b.BlitSubrectBatched(image, rects[c], x, y);
			x += CharSize(c) * _scale;
			if(c == '*' - 32) x -= _scale;
		}

		bool inBigBatch;
		public void BeginBigBatch(Blitter b)
		{
			b.BeginBatch();
			inBigBatch = true;
		}

		public void EndBigBatch(Blitter b)
		{
			b.ExecuteSubrectBatch(image);
			inBigBatch = false;
		}

		public void renderInternal(Blitter b, int x, int y, string str) {
			int xstart = x;
			if(!inBigBatch) b.BeginBatch();
			str = str.Replace("\r\n", "\n");
			for(int i = 0;i < str.Length;i++) {
				render_char(b, ref x, ref y, (int)str[i]);
				if(str[i] == '\n') {
					y += 10 * _scale;
					x = xstart;
				}
			}
			if (!inBigBatch) b.ExecuteSubrectBatch(image);
		}

		public class StringLayoutInfo {
			public List<string> parts = new List<string>();
			public Size size;
			public bool complete;
		}

		public void renderRectangle(Blitter b, int x, int y, int w, int h, string str) {
			StringLayoutInfo sli = layoutRectangle(w, h, str);
			renderLayout(b, x, y, sli);
		}

		public void renderLayout(Blitter b, int x, int y, StringLayoutInfo sli) {
			int lineheight = _scale*10; //includes line spacing
			foreach(string s in sli.parts) {
				Render(b, x, y, s);
				y += lineheight;
			}
		}

		/// <summary>
		/// formats a string into the specified rectangle.
		/// -1 height means no limit
		/// </summary>
		public StringLayoutInfo layoutRectangle(int w, int h, string str) {
			str = str.Replace("\r\n", "\n");
			StringLayoutInfo ret = new StringLayoutInfo();
			if(h == -1) h = int.MaxValue;

			//special case
			if(str.Length == 0)
				return ret;
			
			int backtrack = 0;
			int xo = 0, yo = 0;
			int cursor = -1;
			int lineheight = _scale*10; //includes line spacing

			ret.parts.Add("");

			for(;;) {
				cursor++;
				//if this character completes the string, or a chunk, then check whether it fits and then output it
				if(cursor == str.Length || str[cursor] == ' ') {
					string chunk = str.Substring(backtrack, cursor-backtrack);
					int chunksize = MeasureWidth(chunk);
					if(xo+chunksize>w) {
						//ohno too wide
						yo += lineheight;
						xo = 0;
						ret.parts.Add("");
					}

					//have we run out of vertical room? theres not room for anything else
					if(yo+10*_scale>h) break;

					//ok we can render it
					//render(b, x+xo, y+yo, chunk);
					ret.parts[ret.parts.Count-1] += chunk + " ";

					//increase width of this line and push out the bounds
					xo += chunksize;
					ret.size.Width = Math.Max(ret.size.Width, xo);

					//if we're continuing this line, we'll want to leave room for a space.
					xo += MeasureWidth(" ");

					//if we finished things off, then bail
					if(cursor == str.Length) {
						ret.complete = true;
						break;
					}

					//advance string cursor
					cursor++;
					backtrack = cursor;
				} else if(str[cursor] == '\n') {
					backtrack = cursor;
					yo += lineheight;
					xo = 0;
					ret.parts.Add("");

					//have we run out of vertical room? theres not room for anything else
					if(yo+lineheight>h) break;
				}
			}

			ret.size.Height = yo + lineheight;
			return ret;
		}

		public override void Render(Blitter b, int x, int y, string str) {
			renderInternal(b, x, y, str);
		}
		//public void renderRight(Blitter b, int x, int y, string str) {
		//    str = str.Replace("\r\n", "\n");
		//    b.beginBatch();
		//    foreach(string s in str.Split('\n')) {
		//        int len = measureWidth(s);
		//        int xo = x - len;
		//        render(b, xo, y, str);
		//        y += 10 * _scale;
		//    }
		//    b.executeSubrectBatch(image);
		//}

		public override Color Color { get { return _color; } set { throw new Exception("Changing color of this font is not supported. please create a new renderer"); } }
		public Image image;
		Rectangle[] rects;
		int _scale;
		GameEngine _ge;

		const int xx = 1;
		const int zz = 0;

		static readonly byte[][] smal_tbl;


		static readonly byte[] sbA = new byte[]
		   {4,
			zz,xx,xx,zz,
			xx,zz,zz,xx,
			xx,zz,zz,xx,
			xx,xx,xx,xx,
			xx,zz,zz,xx,
			xx,zz,zz,xx,
			xx,zz,zz,xx};

		static readonly byte[] ssA = new byte[]
		   {5,
			zz,zz,zz,zz,zz,
			zz,zz,zz,zz,zz,
			zz,xx,xx,zz,zz,
			xx,zz,zz,xx,zz,
			xx,zz,zz,xx,zz,
			xx,zz,zz,xx,zz,
			zz,xx,xx,zz,xx};

		static readonly byte[] sbB = new byte[]
		   {4,
			xx,xx,xx,zz,
			xx,zz,zz,xx,
			xx,zz,zz,xx,
			xx,xx,xx,zz,
			xx,zz,zz,xx,
			xx,zz,zz,xx,
			xx,xx,xx,zz};

		static readonly byte[] ssB = new byte[]
		   {4,
			xx,zz,zz,zz,
			xx,zz,zz,zz,
			xx,xx,xx,zz,
			xx,zz,zz,xx,
			xx,zz,zz,xx,
			xx,zz,zz,xx,
			xx,xx,xx,zz};

		static readonly byte[] sbC = new byte[]
		   {4,
			zz,xx,xx,zz,
			xx,zz,zz,xx,
			xx,zz,zz,zz,
			xx,zz,zz,zz,
			xx,zz,zz,zz,
			xx,zz,zz,xx,
			zz,xx,xx,zz};

		static readonly byte[] ssC = new byte[]
		   {3,
			zz,zz,zz,
			zz,zz,zz,
			zz,xx,xx,
			xx,zz,zz,
			xx,zz,zz,
			xx,zz,zz,
			zz,xx,xx};

		static readonly byte[] sbD = new byte[]
		   {4,
			xx,xx,zz,zz,
			xx,zz,xx,zz,
			xx,zz,zz,xx,
			xx,zz,zz,xx,
			xx,zz,zz,xx,
			xx,zz,xx,zz,
			xx,xx,zz,zz};


		static readonly byte[] ssD = new byte[]
		   {4,
			zz,zz,zz,xx,
			zz,zz,zz,xx,
			zz,xx,xx,xx,
			xx,zz,zz,xx,
			xx,zz,zz,xx,
			xx,zz,zz,xx,
			zz,xx,xx,xx};

		static readonly byte[] sbE = new byte[]
		   {4,
			xx,xx,xx,xx,
			xx,zz,zz,zz,
			xx,zz,zz,zz,
			xx,xx,xx,zz,
			xx,zz,zz,zz,
			xx,zz,zz,zz,
			xx,xx,xx,xx};

		static readonly byte[] ssE = new byte[]
		   {4,
			zz,zz,zz,zz,
			zz,zz,zz,zz,
			zz,xx,xx,zz,
			xx,zz,zz,xx,
			xx,xx,xx,zz,
			xx,zz,zz,zz,
			zz,xx,xx,zz};

		static readonly byte[] sbF = new byte[]
		   {4,
			xx,xx,xx,xx,
			xx,zz,zz,zz,
			xx,zz,zz,zz,
			xx,xx,xx,zz,
			xx,zz,zz,zz,
			xx,zz,zz,zz,
			xx,zz,zz,zz};

		static readonly byte[] ssF = new byte[]
		   {4,
			zz,zz,xx,zz,
			zz,xx,zz,xx,
			zz,xx,zz,zz,
			xx,xx,xx,zz,
			zz,xx,zz,zz,
			zz,xx,zz,zz,
			zz,xx,zz,zz};

		static readonly byte[] sbG = new byte[]
		   {4,
			zz,xx,xx,zz,
			xx,zz,zz,xx,
			xx,zz,zz,zz,
			xx,zz,zz,zz,
			xx,zz,xx,xx,
			xx,zz,zz,xx,
			zz,xx,xx,zz};

		static readonly byte[] ssG = new byte[]
		   {4,
			zz,zz,zz,zz,
			zz,zz,zz,zz,
			zz,xx,xx,zz,
			xx,zz,zz,zz,
			xx,zz,xx,xx,
			xx,zz,zz,xx,
			zz,xx,xx,zz};


		static readonly byte[] sbH = new byte[]
		   {4,
			xx,zz,zz,xx,
			xx,zz,zz,xx,
			xx,zz,zz,xx,
			xx,xx,xx,xx,
			xx,zz,zz,xx,
			xx,zz,zz,xx,
			xx,zz,zz,xx};

		static readonly byte[] ssH = new byte[]
		   {4,
			xx,zz,zz,zz,
			xx,zz,zz,zz,
			xx,xx,xx,zz,
			xx,zz,zz,xx,
			xx,zz,zz,xx,
			xx,zz,zz,xx,
			xx,zz,zz,xx};

		static readonly byte[] sbI = new byte[]
		   {3,
			xx,xx,xx,
			zz,xx,zz,
			zz,xx,zz,
			zz,xx,zz,
			zz,xx,zz,
			zz,xx,zz,
			xx,xx,xx};

		static readonly byte[] ssI = new byte[]
		   {1,
			zz,
			xx,
			zz,
			xx,
			xx,
			xx,
			xx};

		static readonly byte[] sbJ = new byte[]
		   {4,
			zz,zz,zz,xx,
			zz,zz,zz,xx,
			zz,zz,zz,xx,
			zz,zz,zz,xx,
			zz,zz,zz,xx,
			xx,zz,zz,xx,
			zz,xx,xx,zz};

		static readonly byte[] ssJ = new byte[]
		   {3,
			zz,zz,xx,
			zz,zz,zz,
			zz,zz,xx,
			zz,zz,xx,
			zz,zz,xx,
			xx,zz,xx,
			zz,xx,zz};

		static readonly byte[] sbK = new byte[]
		   {4,
			xx,zz,zz,xx,
			xx,zz,zz,xx,
			xx,zz,xx,zz,
			xx,xx,zz,zz,
			xx,xx,xx,zz,
			xx,zz,zz,xx,
			xx,zz,zz,xx};

		static readonly byte[] ssK = new byte[]
		   {3,
			xx,zz,zz,
			xx,zz,zz,
			xx,zz,xx,
			xx,zz,xx,
			xx,xx,zz,
			xx,zz,xx,
			xx,zz,xx};

		static readonly byte[] sbL = new byte[]
		   {3,
			xx,zz,zz,
			xx,zz,zz,
			xx,zz,zz,
			xx,zz,zz,
			xx,zz,zz,
			xx,zz,zz,
			xx,xx,xx};

		static readonly byte[] ssL = new byte[]
		   {3,
			zz,zz,zz,
			zz,zz,zz,
			xx,zz,zz,
			xx,zz,zz,
			xx,zz,zz,
			xx,zz,zz,
			xx,xx,xx};

		static readonly byte[] sbM = new byte[]
		   {5,
			xx,zz,zz,zz,xx,
			xx,xx,zz,xx,xx,
			xx,zz,xx,zz,xx,
			xx,zz,xx,zz,xx,
			xx,zz,zz,zz,xx,
			xx,zz,zz,zz,xx,
			xx,zz,zz,zz,xx};

		static readonly byte[] ssM = new byte[]
		   {5,
			zz,zz,zz,zz,zz,
			zz,zz,zz,zz,zz,
			zz,xx,zz,xx,zz,
			xx,zz,xx,zz,xx,
			xx,zz,xx,zz,xx,
			xx,zz,xx,zz,xx,
			xx,zz,xx,zz,xx};

		static readonly byte[] sbN = new byte[]
		   {4,
			xx,zz,zz,xx,
			xx,zz,zz,xx,
			xx,xx,zz,xx,
			xx,zz,xx,xx,
			xx,zz,zz,xx,
			xx,zz,zz,xx,
			xx,zz,zz,xx};

		static readonly byte[] ssN = new byte[]
		   {4,
			zz,zz,zz,zz,
			zz,zz,zz,zz,
			xx,zz,xx,zz,
			xx,xx,zz,xx,
			xx,zz,zz,xx,
			xx,zz,zz,xx,
			xx,zz,zz,xx};

		static readonly byte[] sbO = new byte[]
		   {4,
			zz,xx,xx,zz,
			xx,zz,zz,xx,
			xx,zz,zz,xx,
			xx,zz,zz,xx,
			xx,zz,zz,xx,
			xx,zz,zz,xx,
			zz,xx,xx,zz};

		static readonly byte[] ssO = new byte[]
		   {4,
			zz,zz,zz,zz,
			zz,zz,zz,zz,
			zz,xx,xx,zz,
			xx,zz,zz,xx,
			xx,zz,zz,xx,
			xx,zz,zz,xx,
			zz,xx,xx,zz};

		static readonly byte[] sbP = new byte[]
		   {4,
			xx,xx,xx,zz,
			xx,zz,zz,xx,
			xx,zz,zz,xx,
			xx,xx,xx,zz,
			xx,zz,zz,zz,
			xx,zz,zz,zz,
			xx,zz,zz,zz};

		static readonly byte[] ssP = new byte[]
		   {4,
			zz,zz,zz,zz,
			zz,zz,zz,zz,
			xx,xx,xx,zz,
			xx,zz,zz,xx,
			xx,zz,zz,xx,
			xx,xx,xx,zz,
			xx,zz,zz,zz};

		static readonly byte[] sbQ = new byte[]
		   {4,
			zz,xx,xx,zz,
			xx,zz,zz,xx,
			xx,zz,zz,xx,
			xx,zz,zz,xx,
			xx,zz,zz,xx,
			xx,zz,xx,zz,
			zz,xx,zz,xx};

		static readonly byte[] ssQ = new byte[]
		   {4,
			zz,zz,zz,zz,
			zz,zz,zz,zz,
			zz,xx,xx,zz,
			xx,zz,zz,xx,
			xx,zz,zz,xx,
			xx,zz,xx,zz,
			zz,xx,zz,xx};

		static readonly byte[] sbR = new byte[]
		   {4,
			xx,xx,xx,zz,
			xx,zz,zz,xx,
			xx,zz,zz,xx,
			xx,xx,xx,zz,
			xx,zz,zz,xx,
			xx,zz,zz,xx,
			xx,zz,zz,xx};

		static readonly byte[] ssR = new byte[]
		   {4,
			zz,zz,zz,zz,
			zz,zz,zz,zz,
			zz,xx,xx,zz,
			xx,zz,zz,xx,
			xx,zz,zz,zz,
			xx,zz,zz,zz,
			xx,zz,zz,zz};

		static readonly byte[] sbS = new byte[]
		   {4,
			zz,xx,xx,zz,
			xx,zz,zz,xx,
			xx,zz,zz,zz,
			zz,xx,xx,zz,
			zz,zz,zz,xx,
			xx,zz,zz,xx,
			zz,xx,xx,zz,};

		static readonly byte[] ssS = new byte[]
		   {3,
			zz,zz,zz,
			zz,zz,zz,
			zz,xx,xx,
			xx,zz,zz,
			zz,xx,zz,
			zz,zz,xx,
			xx,xx,zz};

		static readonly byte[] sbT = new byte[]
		   {3,
			xx,xx,xx,
			zz,xx,zz,
			zz,xx,zz,
			zz,xx,zz,
			zz,xx,zz,
			zz,xx,zz,
			zz,xx,zz};

		static readonly byte[] ssT = new byte[]
		   {3,
			zz,xx,zz,
			zz,xx,zz,
			xx,xx,xx,
			zz,xx,zz,
			zz,xx,zz,
			zz,xx,zz,
			zz,xx,xx};

		static readonly byte[] sbU = new byte[]
		   {3,
			xx,zz,xx,
			xx,zz,xx,
			xx,zz,xx,
			xx,zz,xx,
			xx,zz,xx,
			xx,zz,xx,
			xx,xx,xx};

		static readonly byte[] ssU = new byte[]
		   {3,
			zz,zz,zz,
			zz,zz,zz,
			xx,zz,xx,
			xx,zz,xx,
			xx,zz,xx,
			xx,zz,xx,
			xx,xx,xx};

		static readonly byte[] sbV = new byte[]
		   {3,
			xx,zz,xx,
			xx,zz,xx,
			xx,zz,xx,
			xx,zz,xx,
			xx,zz,xx,
			xx,zz,xx,
			zz,xx,zz};

		static readonly byte[] ssV = new byte[]
		   {3,
			zz,zz,zz,
			zz,zz,zz,
			xx,zz,xx,
			xx,zz,xx,
			xx,zz,xx,
			xx,zz,xx,
			zz,xx,zz};

		static readonly byte[] sbW = new byte[]
		   {5,
			xx,zz,zz,zz,xx,
			xx,zz,zz,zz,xx,
			xx,zz,zz,zz,xx,
			xx,zz,xx,zz,xx,
			xx,zz,xx,zz,xx,
			xx,xx,zz,xx,xx,
			xx,zz,zz,zz,xx};

		static readonly byte[] ssW = new byte[]
		   {5,
			zz,zz,zz,zz,zz,
			zz,zz,zz,zz,zz,
			xx,zz,zz,zz,xx,
			xx,zz,xx,zz,xx,
			xx,zz,xx,zz,xx,
			xx,xx,zz,xx,xx,
			xx,zz,zz,zz,xx};

		static readonly byte[] sbX = new byte[]
		   {5,
			xx,zz,zz,zz,xx,
			xx,zz,zz,zz,xx,
			zz,xx,zz,xx,zz,
			zz,zz,xx,zz,zz,
			zz,xx,zz,xx,zz,
			xx,zz,zz,zz,xx,
			xx,zz,zz,zz,xx};

		static readonly byte[] ssX = new byte[]
		   {3,
			zz,zz,zz,
			zz,zz,zz,
			xx,zz,xx,
			xx,zz,xx,
			zz,xx,zz,
			xx,zz,xx,
			xx,zz,xx};

		static readonly byte[] sbY = new byte[]
		   {3,
			xx,zz,xx,
			xx,zz,xx,
			xx,zz,xx,
			zz,xx,zz,
			zz,xx,zz,
			zz,xx,zz,
			zz,xx,zz};

		static readonly byte[] ssY = new byte[]
		   {3,
			zz,zz,zz,
			zz,zz,zz,
			xx,zz,xx,
			xx,zz,xx,
			zz,xx,zz,
			zz,xx,zz,
			zz,xx,zz};

		static readonly byte[] sbZ = new byte[]
		   {5,
			xx,xx,xx,xx,xx,
			zz,zz,zz,zz,xx,
			zz,zz,zz,xx,zz,
			zz,zz,xx,zz,zz,
			zz,xx,zz,zz,zz,
			xx,zz,zz,zz,zz,
			xx,xx,xx,xx,xx};

		static readonly byte[] ssZ = new byte[]
		   {4,
			zz,zz,zz,zz,
			zz,zz,zz,zz,
			xx,xx,xx,xx,
			zz,zz,zz,xx,
			zz,zz,xx,zz,
			zz,xx,zz,zz,
			xx,xx,xx,xx};

		static readonly byte[] s1 = new byte[]
		   {3,
			zz,xx,zz,
			xx,xx,zz,
			zz,xx,zz,
			zz,xx,zz,
			zz,xx,zz,
			zz,xx,zz,
			xx,xx,xx};

		static readonly byte[] s2 = new byte[]
		   {4,
			zz,xx,xx,zz,
			xx,zz,zz,xx,
			zz,zz,zz,xx,
			zz,zz,zz,xx,
			zz,zz,xx,zz,
			zz,xx,zz,zz,
			xx,xx,xx,xx};

		static readonly byte[] s3 = new byte[]
		   {4,
			xx,xx,xx,xx,
			zz,zz,zz,xx,
			zz,zz,zz,xx,
			zz,xx,xx,xx,
			zz,zz,zz,xx,
			zz,zz,zz,xx,
			xx,xx,xx,xx};

		static readonly byte[] s4 = new byte[]
		   {4,
			xx,zz,xx,zz,
			xx,zz,xx,zz,
			xx,zz,xx,zz,
			xx,xx,xx,xx,
			zz,zz,xx,zz,
			zz,zz,xx,zz,
			zz,zz,xx,zz};

		static readonly byte[] s5 = new byte[]
		   {4,
			xx,xx,xx,xx,
			xx,zz,zz,zz,
			xx,zz,zz,zz,
			xx,xx,xx,zz,
			zz,zz,zz,xx,
			zz,zz,zz,xx,
			xx,xx,xx,zz};

		static readonly byte[] s6 = new byte[]
		   {4,
			zz,xx,xx,zz,
			xx,zz,zz,xx,
			xx,zz,zz,zz,
			xx,xx,xx,zz,
			xx,zz,zz,xx,
			xx,zz,zz,xx,
			zz,xx,xx,zz};

		static readonly byte[] s7 = new byte[]
		   {3,
			xx,xx,xx,
			zz,zz,xx,
			zz,zz,xx,
			zz,xx,zz,
			zz,xx,zz,
			zz,xx,zz,
			zz,xx,zz};

		static readonly byte[] s8 = new byte[]
		   {4,
			zz,xx,xx,zz,
			xx,zz,zz,xx,
			xx,zz,zz,xx,
			zz,xx,xx,zz,
			xx,zz,zz,xx,
			xx,zz,zz,xx,
			zz,xx,xx,zz};

		static readonly byte[] s9 = new byte[]
		   {3,
			xx,xx,xx,
			xx,zz,xx,
			xx,zz,xx,
			xx,xx,xx,
			zz,zz,xx,
			zz,zz,xx,
			xx,xx,xx};

		static readonly byte[] s0 = new byte[]
		   {3,
			xx,xx,xx,
			xx,zz,xx,
			xx,zz,xx,
			xx,zz,xx,
			xx,zz,xx,
			xx,zz,xx,
			xx,xx,xx};

		static readonly byte[] sQuote = new byte[]{3,
					   xx,zz,xx,
					   xx,zz,xx,
					   zz,zz,zz,
					   zz,zz,zz,
					   zz,zz,zz,
					   zz,zz,zz,
					   zz,zz,zz};

		static readonly byte[] sYow = new byte[]{3,
				   zz,xx,zz,
				   xx,xx,xx,
				   xx,xx,xx,
				   xx,xx,xx,
				   zz,xx,zz,
				   zz,zz,zz,
				   zz,xx,zz};

		static readonly byte[] sQuotes = new byte[]{1,
						xx,
						xx,
						zz,
						zz,
						zz,
						zz,
						zz};


		static readonly byte[] sComma = new byte[]{2,
					   zz,zz,
					   zz,zz,
					   zz,zz,
					   zz,zz,
					   zz,zz,
					   zz,xx,
					   xx,zz};

		static readonly byte[] sPeriod = new byte[]{1,
						zz,
						zz,
						zz,
						zz,
						zz,
						zz,
						xx};

		static readonly byte[] sMinus = new byte[]{2,
					   zz,zz,
					   zz,zz,
					   zz,zz,
					   xx,xx,
					   zz,zz,
					   zz,zz,
					   zz,zz};

		static readonly byte[] sQuest = new byte[]{3,
					   xx,xx,xx,
					   zz,zz,xx,
					   zz,zz,xx,
					   zz,zz,xx,
					   zz,xx,xx,
					   zz,zz,zz,
					   zz,xx,zz};

		static readonly byte[] sColon = new byte[]{1,
					   zz,
					   zz,
					   xx,
					   zz,
					   xx,
					   zz,
					   zz};

		static readonly byte[] sch = new byte[]{3,
					zz,xx,zz,
					xx,xx,xx,
					xx,xx,xx,
					xx,xx,xx,
					zz,xx,zz,
					xx,xx,xx,
					xx,xx,xx,};

		static readonly byte[] usc = new byte[]{2,
					zz,zz,
					zz,zz,
					zz,zz,
					zz,zz,
					xx,xx,
					xx,xx,
					xx,xx};

		static readonly byte[] star = new byte[]{5,
					 zz,zz,zz,zz,zz,
					 zz,zz,zz,zz,zz,
					 xx,xx,xx,xx,xx,
					 xx,xx,xx,xx,xx,
					 zz,zz,zz,zz,zz,
					 zz,zz,zz,zz,zz,
					 zz,zz,zz,zz,zz};

		static readonly byte[] ss = new byte[]{2,
				   xx,xx,
				   xx,xx,
				   xx,xx,
				   xx,xx,
				   xx,xx,
				   xx,xx,
				   xx,xx};

		static readonly byte[] sEq = new byte[]{4,
				   zz,zz,zz,zz,
				   xx,xx,xx,xx,
				   zz,zz,zz,zz,
				   zz,zz,zz,zz,
				   xx,xx,xx,xx,
				   zz,zz,zz,zz,
				   zz,zz,zz,zz};

		static readonly byte[] sra = new byte[]{3,
					zz,zz,zz,
					xx,zz,zz,
					xx,xx,zz,
					xx,xx,xx,
					xx,xx,zz,
					xx,zz,zz,
					zz,zz,zz};

		static readonly byte[] slParen = new byte[]{2,
						zz,xx,
						xx,zz,
						xx,zz,
						xx,zz,
						xx,zz,
						xx,zz,
						zz,xx};

		static readonly byte[] srParen = new byte[]{2,
						xx,zz,
						zz,xx,
						zz,xx,
						zz,xx,
						zz,xx,
						zz,xx,
						xx,zz};

		static readonly byte[] ssemic = new byte[]{2,
					   zz,xx,
					   zz,zz,
					   zz,xx,
					   zz,xx,
					   zz,xx,
					   zz,xx,
					   xx,zz};

		static readonly byte[] sSlash = new byte[]{3,
					   zz,zz,zz,
					   zz,zz,xx,
					   zz,zz,xx,
					   zz,xx,zz,
					   zz,xx,zz,
					   xx,zz,zz,
					   xx,zz,zz};

		static readonly byte[] sbSlash = new byte[]{3,
					   zz,zz,zz,
					   xx,zz,zz,
					   xx,zz,zz,
					   zz,xx,zz,
					   zz,xx,zz,
					   zz,zz,xx,
					   zz,zz,xx};

		static readonly byte[] sBlock = new byte[]{3,
						zz,zz,zz,
						zz,zz,zz,
						xx,xx,xx,
						xx,xx,xx,
						xx,xx,xx,
						zz,zz,zz,
						zz,zz,zz};

		static readonly byte[] sBlank = new byte[]{2,
					   zz,zz,
					   zz,zz,
					   zz,zz,
					   zz,zz,
					   zz,zz,
					   zz,zz,
					   zz,zz};

		static readonly byte[] sAT = new byte[]
		{ 5,
		   zz,xx,xx,xx,zz,
		   xx,zz,zz,zz,xx,
		   xx,zz,xx,xx,xx,
		   xx,zz,xx,zz,xx,
		   xx,zz,xx,xx,xx,
		   xx,zz,zz,zz,zz,
		   zz,xx,xx,xx,zz};

		static readonly byte[] sNum = new byte[]
		{ 5,
		  zz,zz,zz,zz,zz,
		  zz,xx,zz,xx,zz,
		  xx,xx,xx,xx,xx,
		  zz,xx,zz,xx,zz,
		  xx,xx,xx,xx,xx,
		  zz,xx,zz,xx,zz,
		  zz,zz,zz,zz,zz};

		static readonly byte[] sBuck = new byte[]
		{5,
			zz,zz,zz,zz,zz,
			zz,zz,zz,zz,zz,
			zz,zz,xx,zz,zz,
			zz,xx,xx,xx,zz,
			xx,xx,xx,xx,xx,
			zz,zz,zz,zz,zz,
			zz,zz,zz,zz,zz};

		static readonly byte[] sPercnt = new byte[]
		{
			5,
			zz,zz,zz,zz,zz,
			xx,zz,zz,zz,xx,
			zz,zz,zz,xx,zz,
			zz,zz,xx,zz,zz,
			zz,xx,zz,zz,zz,
			xx,zz,zz,zz,xx,
			zz,zz,zz,zz,zz};

		static readonly byte[] sCarot = new byte[]
		{ 3,
		  zz,xx,zz,
		  xx,zz,xx,
		  zz,zz,zz,
		  zz,zz,zz,
		  zz,zz,zz,
		  zz,zz,zz,
		  zz,zz,zz};

		static readonly byte[] sCopy = new byte[]
		{ 7,
			zz,xx,xx,xx,xx,xx,zz,
			xx,zz,zz,zz,zz,zz,xx,
			xx,zz,zz,xx,xx,zz,xx,
			xx,zz,xx,zz,zz,zz,xx,
			xx,zz,zz,xx,xx,zz,xx,
			xx,zz,zz,zz,zz,zz,xx,
			zz,xx,xx,xx,xx,xx,zz};

		static readonly byte[] sPtr = new byte[]
		{ 5,
		  xx,zz,zz,zz,zz,
		  xx,xx,zz,zz,zz,
		  xx,xx,xx,zz,zz,
		  xx,xx,xx,xx,zz,
		  xx,xx,xx,xx,xx,
		  zz,zz,xx,zz,zz,
		  zz,zz,zz,xx,zz};

		static readonly byte[] check = new byte[]
		{ 7,
		  xx,zz,zz,zz,zz,zz,xx,
		  zz,xx,zz,zz,zz,xx,zz,
		  zz,zz,xx,zz,xx,zz,zz,
		  zz,zz,zz,xx,zz,zz,zz,
		  zz,zz,xx,zz,xx,zz,zz,
		  zz,xx,zz,zz,zz,xx,zz,
		  xx,zz,zz,zz,zz,zz,xx };

		static readonly byte[] target = new byte[]
		{ 7,
		  zz,zz,zz,xx,zz,zz,zz,
		  zz,zz,xx,zz,xx,zz,zz,
		  zz,xx,zz,zz,zz,xx,zz,
		  xx,zz,zz,xx,zz,zz,xx,
		  zz,xx,zz,zz,zz,xx,zz,
		  zz,zz,xx,zz,xx,zz,zz,
		  zz,zz,zz,xx,zz,zz,zz };

	}
}