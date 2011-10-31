using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

using pr2.Common;

namespace pr2.CarotEngine
{
	public class CarotContentManager : ContentManager
	{

		public CarotContentManager(IServiceProvider services) : base(services) { }
		protected override Stream OpenStream(string assetName)
		{
			//maybe we were asked for an extensionless xnb file
			if (File.Exists(assetName + ".xnb"))
				return base.OpenStream(assetName);
			//nope, its something else
			return pr2.Common.ResourceManager.Open(assetName);
		}

		//#if !XBOX
		//class MyIncludeHandler : CompilerIncludeHandler {
		//    public override System.IO.Stream Open(CompilerIncludeHandlerType includeType, string filename) {
		//        throw new Exception("The method or operation is not implemented.");
		//    }
		//}
		//#endif

		unsafe class PcxImageOutput : pr2.sharppng.IPcxImageOutput
		{
			public Image image;
			bool bColor0;
			GraphicsDevice device;
			public PcxImageOutput(GraphicsDevice device, bool color0)
			{
				this.device = device;
				bColor0 = color0;
			}
			public void WriteData(pr2.sharppng.Pcx pcx)
			{

				Texture2D tex = null;

				try
				{
					tex = new Texture2D(device, pcx.width, pcx.height, false, SurfaceFormat.Color);
					int[] data = new int[pcx.width * pcx.height];
					int dest = 0;
					tex.GetData(data);

					//reformat the palette
					int[] colors = new int[256];
					for (int i = 0; i < 256; i++)
						colors[i] = (int)GameEngine.MakeColor(255, pcx.palette[i * 3], pcx.palette[i * 3 + 1], pcx.palette[i * 3 + 2]).PackedValue;

					//blast into image
					int pad = pcx.bytes_per_line - pcx.width;
					fixed (byte* pixels = pcx.pixels)
					{
						byte* src = pixels;
						if (bColor0)
							for (int y = 0; y < pcx.height; y++)
							{
								for (int x = 0; x < pcx.width; x++)
								{
									byte b = *src++;
									int col = colors[b];
									if (b == 0) col = 0;
									data[dest++] = col;
								}
								src += pad;
							}
						else
							for (int y = 0; y < pcx.height; y++)
							{
								for (int x = 0; x < pcx.width; x++)
									data[dest++] = colors[*src++];
								src += pad;
							}

					}

					tex.SetData<int>(data, 0, data.Length);
					image = new Image(GameEngine.Game.Device, tex);
					tex = null;
				}
				finally
				{
					if (tex != null) tex.Dispose();
				}
			}
		}

		unsafe class LargePngImageOutput : PngImageOutput
		{
			public LargePngImageOutput(bool color0)
				: base(null, color0)
			{
			}
			override public void finish()
			{
			}
		}

		unsafe class PngImageOutput : pr2.sharppng.IPngImageOutput
		{
			public PngImageOutput(GraphicsDevice device, bool color0)
			{
				this.device = device;
				bColor0 = color0;
			}

			virtual public void finish()
			{
				tex = new Texture2D(device, width, height, false, SurfaceFormat.Color);
				tex.SetData(intbuf);
			}

			bool bColor0;
			public Texture2D tex;
			GraphicsDevice device;
			public int[] intbuf;
			int linectr;
			public int width, height;
			pr2.sharppng.Png png;
			public void start(pr2.sharppng.Png png)
			{
				this.png = png;
				this.width = png.ihdr.width;
				this.height = png.ihdr.height;
				intbuf = new int[width * height];
			}
			public void writeLine(byte[] data, int offset)
			{
				fixed (byte* pdata = data)
				fixed (int* pintbuf = intbuf)
				{
					byte* pdst = (byte*)(pintbuf + linectr * width);
					byte* psrc = pdata + offset;
					if (png.ihdr.colortype == pr2.sharppng.ColorType.RGB_ALPHA && png.ihdr.bitdepth == 8)
						for (int x = 0; x < width; x++)
						{
#if XBOX360
								//*(int*)pdst = *(int*)psrc;
								//pdst += 4;
								*pdst++ = psrc[3];
								*pdst++ = psrc[2];
								*pdst++ = psrc[1];
								*pdst++ = psrc[0];
#else
							*pdst++ = psrc[0];
							*pdst++ = psrc[1];
							*pdst++ = psrc[2];
							*pdst++ = psrc[3];
#endif
							psrc += 4;
						}
					else if (png.ihdr.colortype == pr2.sharppng.ColorType.RGB && png.ihdr.bitdepth == 8)
						for (int x = 0; x < width; x++)
						{
#if XBOX360
								*pdst++ = 0xFF;
								*pdst++ = psrc[2];
								*pdst++ = psrc[1];
								*pdst++ = psrc[0];
#else
							*pdst++ = psrc[0];
							*pdst++ = psrc[1];
							*pdst++ = psrc[2];
							*pdst++ = 0xFF;
#endif
							psrc += 3;
						}
					else if (bColor0 && png.ihdr.colortype == pr2.sharppng.ColorType.PALETTE && png.ihdr.bitdepth == 4)
					{
						int xmax = width >> 1;
						for (int x = 0; x < xmax; x++)
						{
							int b = psrc[0] >> 4;
							if (b == 0)
							{
								*((int*)pdst) = 0;
								pdst += 4;
							}
							else
							{
#if XBOX360
									*pdst++ = 0xFF;
									*pdst++ = png.palette[b*3+2];
									*pdst++ = png.palette[b*3+1];
									*pdst++ = png.palette[b*3+0];
#else
								*pdst++ = png.palette[b * 3 + 0];
								*pdst++ = png.palette[b * 3 + 1];
								*pdst++ = png.palette[b * 3 + 2];
								*pdst++ = 0xFF;
#endif
							}
							b = psrc[0] & 0x0F;
							if (b == 0)
							{
								*((int*)pdst) = 0;
								pdst += 4;
							}
							else
							{
#if XBOX360
									*pdst++ = 0xFF;
									*pdst++ = png.palette[b*3+2];
									*pdst++ = png.palette[b*3+1];
									*pdst++ = png.palette[b*3+0];
#else
								*pdst++ = png.palette[b * 3 + 0];
								*pdst++ = png.palette[b * 3 + 1];
								*pdst++ = png.palette[b * 3 + 2];
								*pdst++ = 0xFF;
#endif
							}
							psrc++;
						}
						//do the last pixel
						if ((width & 1) != 0)
						{
							int b = psrc[0] & 0x0F;
#if XBOX360
								if (b==0) *pdst++ = 0x00;
								else *pdst++ = 0xFF;
								*pdst++ = png.palette[b*3+2];
								*pdst++ = png.palette[b*3+1];
								*pdst++ = png.palette[b*3+0];
#else
							*pdst++ = png.palette[b * 3 + 0];
							*pdst++ = png.palette[b * 3 + 1];
							*pdst++ = png.palette[b * 3 + 2];
							if (b == 0) *pdst++ = 0x00;
							else *pdst++ = 0xFF;
#endif
						}
					}
					else if (png.ihdr.colortype == pr2.sharppng.ColorType.PALETTE && png.ihdr.bitdepth == 4)
					{
						int xmax = width >> 1;
						for (int x = 0; x < xmax; x++)
						{
							int b = psrc[0] & 0x0F;
#if XBOX360
								*pdst++ = 0xFF;
								*pdst++ = png.palette[b*3+2];
								*pdst++ = png.palette[b*3+1];
								*pdst++ = png.palette[b*3+0];
#else
							*pdst++ = png.palette[b * 3 + 0];
							*pdst++ = png.palette[b * 3 + 1];
							*pdst++ = png.palette[b * 3 + 2];
							*pdst++ = 0xFF;
#endif
							b = psrc[0] >> 4;
#if XBOX360
								*pdst++ = 0xFF;
								*pdst++ = png.palette[b*3+2];
								*pdst++ = png.palette[b*3+1];
								*pdst++ = png.palette[b*3+0];
#else
							*pdst++ = png.palette[b * 3 + 0];
							*pdst++ = png.palette[b * 3 + 1];
							*pdst++ = png.palette[b * 3 + 2];
							*pdst++ = 0xFF;
#endif
							psrc++;
						}
						//do the last pixel
						if ((width & 1) != 0)
						{
							int b = psrc[0] & 0x0F;
#if XBOX360
								*pdst++ = 0xFF;
								*pdst++ = png.palette[b*3+2];
								*pdst++ = png.palette[b*3+1];
								*pdst++ = png.palette[b*3+0];
#else
							*pdst++ = png.palette[b * 3 + 0];
							*pdst++ = png.palette[b * 3 + 1];
							*pdst++ = png.palette[b * 3 + 2];
							*pdst++ = 0xFF;
#endif
						}
					}
					else if (bColor0 && png.ihdr.colortype == pr2.sharppng.ColorType.PALETTE && png.ihdr.bitdepth == 1)
					{
						//if we subtract here then bobgreen wont work. 
						//does this have something to do with the byte endianness?
						//please make a note here to study further when we find an image that this breaks
						//psrc--; //??
						int xmax = width;
						int x = 0;
						for (; ; )
						{
							int p = psrc[0];
							psrc++;
							for (int i = 0; i < 8; i++)
							{
								int b = (p & 0x80) >> 7;
								p <<= 1;
								if (b == 0)
								{
									*((int*)pdst) = 0;
									pdst += 4;
								}
								else
								{
#if XBOX360
										*pdst++ = 0xFF;
										*pdst++ = png.palette[b*3+2];
										*pdst++ = png.palette[b*3+1];
										*pdst++ = png.palette[b*3+0];
#else
									*pdst++ = png.palette[b * 3 + 0];
									*pdst++ = png.palette[b * 3 + 1];
									*pdst++ = png.palette[b * 3 + 2];
									*pdst++ = 0xFF;
#endif
								}
								x++;
								if (x == width) break;
							}
							if (x == width) break;
						}
					}
					else if (bColor0 && png.ihdr.colortype == pr2.sharppng.ColorType.PALETTE && png.ihdr.bitdepth == 2)
					{
						int xmax = width;
						int x = 0;
						for (; ; )
						{
							int p = psrc[0];
							psrc++;
							for (int i = 0; i < 4; i++)
							{
								int b = (p & 0xC0) >> 6;
								p <<= 2;
								if (b == 0)
								{
									*((int*)pdst) = 0;
									pdst += 4;
								}
								else
								{
#if XBOX360
										*pdst++ = 0xFF;
										*pdst++ = png.palette[b*3+2];
										*pdst++ = png.palette[b*3+1];
										*pdst++ = png.palette[b*3+0];
#else
									*pdst++ = png.palette[b * 3 + 0];
									*pdst++ = png.palette[b * 3 + 1];
									*pdst++ = png.palette[b * 3 + 2];
									*pdst++ = 0xFF;
#endif
								}
								x++;
								if (x == width) break;
							}
							if (x == width) break;
						}
					}
					else if (png.ihdr.colortype == pr2.sharppng.ColorType.PALETTE && png.ihdr.bitdepth == 1)
					{
						psrc--; //??
						int xmax = width;
						int x = 0;
						for (; ; )
						{
							int p = psrc[0];
							psrc++;
							for (int i = 0; i < 8; i++)
							{
								int b = (p & 0x80) >> 7;
								p <<= 1;
#if XBOX360
									*pdst++ = 0xFF;
									*pdst++ = png.palette[b*3+2];
									*pdst++ = png.palette[b*3+1];
									*pdst++ = png.palette[b*3+0];
#else
								*pdst++ = png.palette[b * 3 + 0];
								*pdst++ = png.palette[b * 3 + 1];
								*pdst++ = png.palette[b * 3 + 2];
								*pdst++ = 0xFF;
#endif
								x++;
								if (x == width) break;
							}
							if (x == width) break;
						}
					}
					else if (png.ihdr.colortype == pr2.sharppng.ColorType.PALETTE && png.ihdr.bitdepth == 2)
					{
						int xmax = width;
						int x = 0;
						for (; ; )
						{
							int p = psrc[0];
							psrc++;
							for (int i = 0; i < 4; i++)
							{
								int b = (p & 0xC0) >> 6;
								p <<= 2;
#if XBOX360
									*pdst++ = 0xFF;
									*pdst++ = png.palette[b*3+2];
									*pdst++ = png.palette[b*3+1];
									*pdst++ = png.palette[b*3+0];
#else
								*pdst++ = png.palette[b * 3 + 0];
								*pdst++ = png.palette[b * 3 + 1];
								*pdst++ = png.palette[b * 3 + 2];
								*pdst++ = 0xFF;
#endif
								x++;
								if (x == width) break;
							}
							if (x == width) break;
						}
					}
					else if (bColor0 && png.ihdr.colortype == pr2.sharppng.ColorType.PALETTE && png.ihdr.bitdepth == 8)
					{
						for (int x = 0; x < width; x++)
						{
							int b = psrc[0];
							if (b == 0)
							{
								*((int*)pdst) = 0;
								pdst += 4;
							}
							else
							{
#if XBOX360
									*pdst++ = 0xFF;
									*pdst++ = png.palette[b*3+2];
									*pdst++ = png.palette[b*3+1];
									*pdst++ = png.palette[b*3+0];
#else
								*pdst++ = png.palette[b * 3 + 0];
								*pdst++ = png.palette[b * 3 + 1];
								*pdst++ = png.palette[b * 3 + 2];
								*pdst++ = 0xFF;
#endif
							}
							psrc++;
						}
					}
					else if (png.ihdr.colortype == pr2.sharppng.ColorType.PALETTE && png.ihdr.bitdepth == 8)
					{
						for (int x = 0; x < width; x++)
						{
							int b = psrc[0];
#if XBOX360
								*pdst++ = 0xFF;
								*pdst++ = png.palette[b*3+2];
								*pdst++ = png.palette[b*3+1];
								*pdst++ = png.palette[b*3+0];
#else
							*pdst++ = png.palette[b * 3 + 0];
							*pdst++ = png.palette[b * 3 + 1];
							*pdst++ = png.palette[b * 3 + 2];
							*pdst++ = 0xFF;
#endif
							psrc++;
						}
					}
					else if (bColor0 && png.ihdr.colortype == pr2.sharppng.ColorType.GRAY && png.ihdr.bitdepth == 8)
					{
						for (int x = 0; x < width; x++)
						{
							byte b = psrc[0];
							if (b == 0)
							{
								*((int*)pdst) = 0;
								pdst += 4;
							}
							else
							{
#if XBOX360
									*pdst++ = 0xFF;
									*pdst++ = b;
									*pdst++ = b;
									*pdst++ = b;
#else
								*pdst++ = b;
								*pdst++ = b;
								*pdst++ = b;
								*pdst++ = 0xFF;
#endif
							}
							psrc++;
						}

					}
					else if (png.ihdr.colortype == pr2.sharppng.ColorType.GRAY && png.ihdr.bitdepth == 8)
					{
						for (int x = 0; x < width; x++)
						{
							byte b = psrc[0];
#if XBOX360
								*pdst++ = 0xFF;
								*pdst++ = b;
								*pdst++ = b;
								*pdst++ = b;
#else
							*pdst++ = b;
							*pdst++ = b;
							*pdst++ = b;
							*pdst++ = 0xFF;
#endif
							psrc++;
						}
					}
					else throw new Exception("Unsupported png pixel format");
				}

				linectr++;
			}

		}

		/// <summary>
		/// Loads a png from the specified stream, optionally with color0=transparent
		/// </summary>
		Image LoadPNG(GraphicsDevice device, Stream stream, bool color0)
		{
			pr2.sharppng.Png png = new pr2.sharppng.Png();
			PngImageOutput imageOutput = new PngImageOutput(device, color0);
			png.read(stream, imageOutput);
			return new Image(GameEngine.Game.Device, imageOutput.tex);
		}


		LargeImage LoadLargePNG(GraphicsDevice device, Stream stream, bool color0)
		{
			pr2.sharppng.Png png = new pr2.sharppng.Png();
			LargePngImageOutput imageOutput = new LargePngImageOutput(color0);
			png.read(stream, imageOutput);
			LargeImage ret = new LargeImage();
			ret.Width = imageOutput.width;
			ret.Height = imageOutput.height;
			ret.BlocksWidth = ret.Width / 2048;
			if (ret.Width > ret.BlocksWidth * 2048) ret.BlocksWidth++;
			ret.BlocksHeight = ret.Height / 2048;
			if (ret.Height > ret.BlocksHeight * 2048) ret.BlocksHeight++;
			ret.Images = new Image[ret.BlocksWidth, ret.BlocksHeight];
			for (int y = 0; y < ret.BlocksHeight; y++)
				for (int x = 0; x < ret.BlocksWidth; x++)
				{
					int px = x * 2048;
					int py = y * 2048;
					int xtodo = ret.Width - px;
					if (xtodo > 2048) xtodo = 2048;
					int ytodo = ret.Height - py;
					if (ytodo > 2048) ytodo = 2048;
					int[] intbuf = new int[xtodo * ytodo];
					for (int qy = 0, ctr = 0; qy < ytodo; qy++)
						for (int qx = 0; qx < xtodo; qx++, ctr++)
						{
							intbuf[ctr] = imageOutput.intbuf[(py + qy) * ret.Width + px + qx];
						}

					var tex = new Texture2D(device, xtodo, ytodo, false, SurfaceFormat.Color);
					tex.SetData(intbuf);
					ret.Images[x, y] = new Image(GameEngine.Game.Device, tex);
				}

			//return new Image(GameEngine.Game.Device, imageOutput.tex);
			return ret;
		}

		/// <summary>
		/// Loads a pcx from the specified stream, optionally with color0=transparent
		/// </summary>
		Image LoadPCX(GraphicsDevice device, Stream stream, bool color0)
		{
			pr2.sharppng.Pcx pcx = new pr2.sharppng.Pcx();
			PcxImageOutput imageOutput = new PcxImageOutput(device, color0);
			pcx.read(stream, imageOutput);
			return imageOutput.image;
		}

		/// <summary>
		/// Loads a png from the specified stream
		/// </summary>
		Image LoadPNG(GraphicsDevice device, Stream stream)
		{
			return LoadPNG(device, stream, false);
		}

		/// <summary>
		/// Loads a png from the specified path
		/// </summary>
		Image LoadPNG(GraphicsDevice device, string path)
		{
			using (Stream stream = OpenStream(path))
				return LoadPNG(device, stream);
		}

		/// <summary>
		/// Loads palettized image with color0=transparent from the specified stream.
		/// </summary>
		public Image LoadImage0(GraphicsDevice device, Stream stream)
		{
			bool ispng = pr2.sharppng.Png.IsPng(stream);
			stream.Position = 0;
			if (ispng) return LoadPNG(device, stream, true);

			bool ispcx = pr2.sharppng.Pcx.IsPcx(stream);
			stream.Position = 0;
			if (ispcx) return LoadPNG(device, stream, true);

			throw new Exception("Must call LoadImage0 on supported filetype (pcx,png)");
		}

		/// <summary>
		/// Loads palettized image with color0=transparent from the specified path.
		/// </summary>
		public Image LoadImage0(GameEngine game, string path)
		{
			string ext = Path.GetExtension(path).ToLower();
			switch (ext)
			{
				case ".pcx":
					using (Stream stream = OpenStream(path))
						return LoadPCX(game.Device, stream, true);

				case ".png":
					using (Stream stream = OpenStream(path))
						return LoadPNG(game.Device, stream, true);

				default: throw new Exception("Must call LoadImage0 on supported filetype (pcx,png)");
			}
		}

		/// <summary>
		/// Loads palettized image with color0=transparent from the specified path.
		/// </summary>
		public LargeImage LoadLargeImage0(GameEngine game, string path)
		{
			string ext = Path.GetExtension(path).ToLower();
			switch (ext)
			{
				case ".png":
					using (Stream stream = OpenStream(path))
						return LoadLargePNG(game.Device, stream, true);

				default: throw new Exception("Must call LoadLargeImage0 on supported filetype (png)");
			}
		}

		/// <summary>
		/// Loads an image from the specified stream
		/// </summary>
		public Image LoadImage(GraphicsDevice device, Stream stream)
		{
			bool ispng = pr2.sharppng.Png.IsPng(stream);
			stream.Position = 0;
			if (ispng) return LoadPNG(device, stream, true);

			bool ispcx = pr2.sharppng.Pcx.IsPcx(stream);
			stream.Position = 0;
			if (ispcx) return LoadPNG(device, stream, true);

			Texture2D tex = GetTexture(device, stream);
			if (tex == null) throw new Exception("boo hoo");
			return new Image(device, tex);
		}

		/// <summary>
		///  Loads an image from the specified path
		/// </summary>
		public Image LoadImage(GraphicsDevice device, string path)
		{
			string ext = Path.GetExtension(path).ToLower();
			switch (ext)
			{
				case ".pcx":
					using (Stream stream = OpenStream(path))
						return LoadPCX(device, stream, false);

				case ".png":
					using (Stream stream = OpenStream(path))
						return LoadPNG(device, stream, false);

				case ".xnb":
					Texture2D tex = GetTexture(device, path);
					if (tex == null) throw new Exception("boo hoo");
					return new Image(device, tex);

				default: throw new Exception("Must call LoadImage on supported filetype (pcx,png,xnb)");
			}
		}

		public Effect GetEffect(string path)
		{
			//#if XNA
			//xna:
			return Load<Effect>(path);
			//#else
			////win:
			//if using this, you will need to pass in a GraphicsDevice
			//CompiledEffect ce  = Effect.CompileEffectFromSource(File.ReadAllText(path), new CompilerMacro[] { }, new MyIncludeHandler(), CompilerOptions.None, TargetPlatform.Windows);
			//return new Effect(device, ce.GetEffectCode(), CompilerOptions.None, new EffectPool());
			//#endif
		}

		public Texture2D GetTexture(GraphicsDevice device, Stream stream)
		{
			//TextureCreationParameters tcp;
			//long temp = stream.Position;
			//tcp = Texture2D.GetCreationParameters(device, stream);
			//stream.Position = temp;
			//tcp.Format = SurfaceFormat.Color;
			//return Texture2D.FromFile(device, stream, tcp);
			throw new NotSupportedException();
		}


		public Texture2D GetTexture(GraphicsDevice device, string path)
		{
			//#if XNA
			//return Load<Texture2D>(path);
			//#else

			////win:
			////we have to do some juggling to force the texture to load as argb. :(
			//TextureCreationParameters tcp;
			//using (Stream s = ResourceManager.open(path))
			//    tcp = Texture2D.GetCreationParameters(device, s);

			//tcp.Format = SurfaceFormat.Color;
			//using (Stream s = ResourceManager.open(path))
			//    return Texture2D.FromFile(device, s, tcp);

			//#endif

			return Load<Texture2D>(path);
		}
	}
}
