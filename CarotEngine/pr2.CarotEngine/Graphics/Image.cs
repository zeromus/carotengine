//TODO - get rid of cache saving (render target preserve contents can handle it--it makes render targets work more like plain old textures


using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using pr2;

namespace pr2.CarotEngine
{

	public class BackbufferImage : Image
	{
		internal BackbufferImage(GraphicsDevice device) : base(device) { }
		public override Texture2D getTex()
		{
			throw new Exception("Cannot use screen as source image");
		}
		public override void Resolve()
		{
			//do nothing. this gets called by engine innards by mistake.
		}
		public override void Cache()
		{
			throw new Exception("Cannot cache screen");
		}
		internal override void setTarget()
		{
			device.SetRenderTarget(rt);
		}
	}

	public class LargeImage : IDisposable
	{
		public int Width, Height;
		public int BlocksWidth, BlocksHeight;
		public Image[,] Images;
		public void Dispose()
		{
			for (int y = 0; y < BlocksHeight; y++)
				for (int x = 0; x < BlocksWidth; x++)
				{
					if (Images[x, y] != null)
						Images[x, y].Dispose();
					Images[x, y] = null;
				}
		}

		public void Blit(Blitter b)
		{
			for (int y = 0; y < BlocksHeight; y++)
				for (int x = 0; x < BlocksWidth; x++)
				{
					b.Blit(Images[x, y], x * 2048, y * 2048);
				}
		}
	}

	public class Image : IDisposable
	{

		internal int width, height;
		internal RenderTarget2D rt;

		protected Texture2D tex_cache;
		Texture2D tex_const;
		Texture2D tex_rt;
		//Color[] tex_cache_bytes;
		//bool tex_cache_is_defined = false;
		bool isDirty = false;
		public GraphicsDevice device;
		bool isWrapper = false;

		public void GenerateMipMaps()
		{
			//if (tex_rt == null)
			//    throw new Exception("Image not in suitable condition for using GenerateMipMaps");
			//tex_rt.GenerateMipMaps(TextureFilter.Linear);
		}

		public static Image GetWrapper(Texture2D tex)
		{
			Image ret = new Image();
			ret.tex_const = tex;
			ret.isWrapper = true;
			ret.width = tex.Width;
			ret.height = tex.Height;
			return ret;
		}

		Image() { }

		public Image CloneInNewDevice(GraphicsDevice device)
		{
			Image newimage = new Image(device, this.width, this.height);

			Texture2D srctex = this.getTex();
			int size = this.SizeForTexture(srctex);
			byte[] temp = new byte[size];
			srctex.GetData(temp);
			newimage.allocCache();
			newimage.getTex().SetData(temp);
			return newimage;
		}

		bool IsCurrentRenderTarget()
		{
			foreach (var x in device.GetRenderTargets())
				if (rt == x.RenderTarget)
					return true;
			return false;
		}

		public virtual void Dispose()
		{
			if (isWrapper) return;

			if (IsCurrentRenderTarget())
				Resolve();
			if (rt != null) rt.Dispose();
			if (tex_const != null) tex_const.Dispose();
			if (tex_cache != null) tex_cache.Dispose();
			RemoveFromList();
		}

		//--------list management

		public Vector2 VectorSize { get { return new Vector2(width, height); } }
		public Size Size { get { return new Size(width, height); } }
		public int Width { get { return width; } }
		public int Height { get { return height; } }

		internal static void UnloadAll(GraphicsDevice device)
		{
			//when we have to unload graphics content, render targets need recreating
			lock (imageList)
				foreach (Image image in imageList[device])
					image.rt = null;
		}

		internal static WorkingDictionary<GraphicsDevice, LinkedList<Image>> imageList = new WorkingDictionary<GraphicsDevice, LinkedList<Image>>();
		LinkedListNode<Image> listNode;

		void AddToList()
		{
			listNode = new LinkedListNode<Image>(this);
			lock (imageList)
				imageList[device].AddFirst(listNode);
		}

		void RemoveFromList()
		{
			lock (imageList)
				imageList[device].Remove(listNode);
			listNode = null;
		}
		//------------------------

		//---------------------state management operations

		//public void destroy() {
		//    Dispose();
		//    if(tex_cache != null)
		//        tex_cache.Dispose();
		//}

		//public void Dispose() {
		//    //if we were using the tex_rt for cache, we need to save it now
		//    //if(tex_rt != null) {
		//    //    Texture2D tex = (tex_rt != null) ? tex_rt : tex_cache;
		//    //    Color[] data = new Color[width * height];
		//    //    tex.GetData(data);
		//    //    tex_cache = new Texture2D(ge.device, width, height, 1, ResourceUsage.None, SurfaceFormat.Color, ResourceManagementMode.Manual);
		//    //    tex_cache.SetData(0, null, data, 0, width * height, SetDataOptions.None);
		//    //    tex_cache_is_defined = true;
		//    //}

		//    //fuck.manual cachetex disposes. we need to save the bytesanyway


		//    //clean everything up except for our cachetex!
		//    //if(tex_cache != null) tex_cache.Dispose();
		//    //tex_cache = null;
		//    if(tex_rt != null) tex_rt.Dispose();
		//    tex_rt = null;
		//    if(rt != null) rt.Dispose();
		//    rt = null;
		//}


		//public void Restore() {
		//    ////if we saved some bytes, restore them into a new cache tex
		//    //if(tex_cache_bytes != null) {
		//    //    tex_cache = new Texture2D(ge.device, width, height, 1, ResourceUsage.None, SurfaceFormat.Color, ResourceManagementMode.Manual);
		//    //    //tex_cache.SetData(0,tex_cache_bytes,new Rectangle(0,0,width,height),0,width*height*4,SetDataOptions.Discard);
		//    //    tex_cache_bytes = null;
		//    //    tex_cache_is_defined = true;
		//    //}
		//    //isDirty = false;
		//}

		public virtual Texture2D getTex()
		{
			//if we have a const texture, return that
			if (tex_const != null) return tex_const;

			//if we have a cached texture, return that
			if (tex_cache != null) return tex_cache;

			//if we have the rt tex, return that
			if (tex_rt != null) return tex_rt;

			//ok we don't have any of those

			throw new Exception("Attempting to use an undefined texture. Maybe you forgot to render anything to the render target");

			////we don't have anything to return. thats an undefined texture.
			////for consistency's sake, lets make a garbage texture and return it
			////OR -- lets crash?
			//tex_cache = new Texture2D(ge.device, width, height, 1, ResourceUsage.None, SurfaceFormat.Color, ResourceManagementMode.Manual);
			//return tex_cache;
		}

#if !XBOX360
		public void PutInClipboard()
		{
			using (System.Drawing.Bitmap bmp = GetWinformsBitmap())
				System.Windows.Forms.Clipboard.SetImage(bmp);
		}
		public System.Drawing.Bitmap GetWinformsBitmap()
		{
			Resolve();
			using (LockCache lc = Lock())
				return lc.Softimage.ToSystemDrawingBitmap();
		}
#endif

		/// <summary>
		/// resolves a render target, making it safe to switch render targets or deal directly with the render target
		/// </summary>
		public virtual void Resolve()
		{
			//do not resolve unless we're dirty.
			//once we're resolved, we are no longer dirty
			if (isDirty)
			{
				//ge.device.ResolveRenderTarget(0); //upgrade change
				device.SetRenderTarget(null);
				invalidate_tex_rt();
				tex_rt = rt;
			}
			isDirty = false;
		}

		void allocCache()
		{
			if (tex_cache == null)
				tex_cache = new Texture2D(device, width, height, false, SurfaceFormat.Color); //upgrade change: was using automatic management 
		}

		public virtual void Cache()
		{
			if (tex_const != null)
				throw new Exception("Cannot cache a constant image");

			//if we dont have a tex_rt, then we haven't resolved. force ourselves to resolvem since this is serious business
			if (tex_rt == null)
			{
				isDirty = true;
				Resolve();
			}

			//create the cache tex if we need it
			allocCache();

			//we might need to resolve if this gets called immediately after rendering
			Resolve();

			//copy the data to the cache
			Color[] data = new Color[width * height];
			tex_rt.GetData(data);
			tex_cache.SetData(data);

			//since we've cached this image, we assume the user wants it in another frame
			//so we are going to throw away tex_rt so that any subsequent usage will use the cached tex
			invalidate_tex_rt();
		}

		void invalidate_tex_rt()
		{
			//oddity: dont have to dispose these.
			//in fact, you _can't_ dispose them or else there will be crashes
			//if(tex_rt != null) tex_rt.Dispose();
			tex_rt = null;
		}

		int SizeForTexture(Texture2D tex)
		{
			int pixels = tex.Width * tex.Height;
			switch (tex.Format)
			{
				case SurfaceFormat.Color: return 4 * pixels;
				case SurfaceFormat.Bgr565: return 2 * pixels;
				case SurfaceFormat.Bgra5551: return 2 * pixels;
				case SurfaceFormat.Bgra4444: return 2 * pixels;
				case SurfaceFormat.Dxt1: return pixels * 4 / 6;
				case SurfaceFormat.Dxt3: return pixels;
				case SurfaceFormat.Dxt5: return pixels;
				case SurfaceFormat.NormalizedByte2: return 2 * pixels;
				case SurfaceFormat.NormalizedByte4: return 4 * pixels;
				case SurfaceFormat.Rgba1010102: return 4 * pixels;
				case SurfaceFormat.Rg32: return 4 * pixels;
				case SurfaceFormat.Rgba64: return 8 * pixels;
				case SurfaceFormat.Alpha8: return pixels;
				case SurfaceFormat.Single: return 4 * pixels;
				case SurfaceFormat.Vector2: return 8 * pixels;
				case SurfaceFormat.Vector4: return 16 * pixels;
				case SurfaceFormat.HalfSingle: return 2 * pixels;
				case SurfaceFormat.HalfVector2: return 4 * pixels;
				case SurfaceFormat.HalfVector4: return 8 * pixels;
				case SurfaceFormat.HdrBlendable: throw new InvalidOperationException("unknown..");
				default: throw new InvalidOperationException("Impossible texture format");
			}
		}


		/// <summary>
		/// sets the image as the current render target
		/// </summary>
		internal virtual void setTarget()
		{

			//if we're already dirty, then this is an unexpected scenario
			if (isDirty)
				throw new Exception("Unexpected scenario: setTarget() on dirty image");

			//assume we're going to be dirty
			isDirty = true;

			//cannot set const targets as texture
			if (tex_const != null)
				throw new Exception("Cannot set a constant image as target ");

			//if we don't have a rt, create it
			if (rt == null)
				rt = new RenderTarget2D(device, width, height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);


			//INTERESTING CODE ALERT
			//--this code does not work right when it is put here. the render target will get clobbered or restored according to the RenderTargetUsage
			//independent of what we do here.
			//you can't use this after SetRenderTarget() because it will be unable to call SetData() on a bound render target
			//if (tex_cache != null)
			//{
			//    int size = SizeForTexture(tex_cache);
			//    byte[] temp = new byte[size];
			//    tex_cache.GetData(temp);
			//    rt.SetData(temp);
			//}

			//go ahead and set the target
			device.SetRenderTarget(rt);

			//INTERESTING CODE ALERT
			//--this code may be useful if we decide we dont want to use RenderTargetUsage.PreserveContents (but I bet we'll want to)
			//if (tex_cache != null)
			//    using (SpriteBatch sb = new SpriteBatch(this.device))
			//    {
			//        sb.Begin();
			//        sb.Draw(tex_cache, new Vector2(0, 0), Color.White);
			//        sb.End();
			//    }


			//since we're rendering to this image, we assume the cache will not longer be valid
			if (tex_cache != null) tex_cache.Dispose();
			tex_cache = null;

			////if we have the rt texture and we're running in windows, return the rt texture
			////(no need to bring in the cache)
			//if(tex_rt != null && tex_rt.ResourceManagementMode == ResourceManagementMode.Manual) {
			//    ge.device.SetRenderTarget(0,rt);
			//    return;
			//}

			////otherwise we need to upload the cached texture to the rt:

			////but actually if we're using the tex_cache and its undefined, dont bother
			//if(tex_const == null && !tex_cache_is_defined) {
			//    ge.device.SetRenderTarget(0, rt);
			//    return;
			//}

			////copy current data to the rt. this is unfortunately complicated since theres
			////no backdoor to writing to a rt. we have to actually render to it
			//using(SpriteBatch sb = new SpriteBatch(ge.device)) {
			//    ge.device.SetRenderTarget(0, rt);
			//    VertexDeclaration vd = ge.device.VertexDeclaration;
			//    PixelShader ps = ge.device.PixelShader;
			//    VertexShader vs = ge.device.VertexShader;
			//    ge.device.PixelShader = null;
			//    ge.device.VertexShader = null;
			//    sb.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.SaveState);
			//    sb.Draw(getTex(), new Vector2(0, 0), Color.White);
			//    sb.End();
			//    ge.device.VertexDeclaration = vd;
			//    ge.device.PixelShader = ps;
			//    ge.device.VertexShader = vs;
			//}
		}

		//----loading/constructos

		/// <summary>
		/// makes a new empty image
		/// </summary>
		internal Image(GraphicsDevice device, int w, int h)
		{
			this.device = device;
			this.width = w;
			this.height = h;
			AddToList();
		}

		/// <summary>
		/// creates an image which will wrap the provided texture. beware that some of the code
		/// expects it to be ColorFormat.Color.. it will work fine as long as you dont need to do
		/// something advanced with it
		/// </summary>
		public Image(GraphicsDevice device, Texture2D tex)
		{
			this.device = device;
			tex_const = tex;
			width = tex.Width;
			height = tex.Height;
			AddToList();
		}

		internal Image(GraphicsDevice device)
		{
			this.device = device;
			AddToList();
		}

		//----------operations

		/// <summary>
		/// makes it prettier to lock the texture.
		/// obviously not the best performance.. some of these things might be better done as rendering ops.
		/// at least this makes it easier to do stuff without worrying about screwing up the rendering state
		/// 
		/// TODO - improve semantically, maybe interact with softimage to make an autodisposing softimage
		/// </summary>
		public unsafe class LockCache : IDisposable
		{
			public int* data;
			public int[] buf;
			public Softimage Softimage;

			Rectangle rect;
			bool rdonly;

			public void Dispose()
			{
				gchandle.Free();
				if (!rdonly)
					tex.SetData(0, rect, buf, 0, buf.Length);
			}

			public LockCache(Image img, Rectangle rect, bool rdonly)
			{
				this.rdonly = rdonly;
				this.rect = rect;
				this.img = img;
				//tex = img.tex_cache;
				tex = img.getTex(); //mbg 2/24/07 - changed this. not sure whether it makes sense
				buf = new int[rect.Width * rect.Height];
				//tex.GetData(buf);
				tex.GetData(0, rect, buf, 0, buf.Length);
				gchandle = GCHandle.Alloc(buf, GCHandleType.Pinned);
				data = (int*)gchandle.AddrOfPinnedObject().ToPointer();
				//todo -- cant softimage use a pointer? pretty please?
				Softimage = new Softimage(buf, img.width, img.width, img.height);
			}

			GCHandle gchandle;
			Texture2D tex;
			Image img;
		}

		public LockCache Lock()
		{
			return Lock(new Rectangle(0, 0, width, height));
		}

		public LockCache Lock(Rectangle rect)
		{
			return new LockCache(this, rect, false);
		}

		public LockCache LockReadonly()
		{
			return LockReadonly(new Rectangle(0, 0, width, height));
		}

		public LockCache LockReadonly(Rectangle rect)
		{
			return new LockCache(this, rect, true);
		}


		public unsafe void Alphafy(Color tcolor)
		{
			using (LockCache lc = Lock())
				pr2.Common.Lib.alpha32(lc.data, width * height, GameEngine.toArgb(tcolor));
		}

		/// <summary>
		/// premultiplies the color channel with the alpha channel. You shouldn't do this more than once per image!
		/// </summary>
		public unsafe void Premultiply()
		{
			using (LockCache lc = Lock())
				pr2.Common.Lib.Premultiply(lc.data, width * height);
		}

		public unsafe void loadSoftimage(Softimage si)
		{
			if (tex_const != null) throw new Exception("Cannot loadSoftimage on a constant texture");

			invalidate_tex_rt();
			allocCache();

			int[] buf = new int[width * height];
			fixed (int* pbuf = buf)
				si.dump(pbuf);
			tex_cache.SetData(buf);
		}

	}
}