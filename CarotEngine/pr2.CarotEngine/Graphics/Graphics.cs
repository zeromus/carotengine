//rendertarget infos
//https://forums.microsoft.com/MSDN/ShowPost.aspx?PostID=882859&SiteID=1
//examples
//http://forums.microsoft.com/MSDN/ShowPost.aspx?PostID=1089984&SiteID=1

using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using pr2;
using pr2.CarotEngine;

namespace pr2.CarotEngine {
public partial class GameEngine {

	//shaders
	//public Dictionary<string, PixelShader> pixelShaders = new Dictionary<string, PixelShader>();
	//public Dictionary<string, VertexShader> vertexShaders = new Dictionary<string, VertexShader>();

	public BackbufferImage screen;

	void InitGraphics() {
		//geometry that doesnt take reloading
		InitGeometry();
		Matrices = new MatrixSet(this);
		screen = new BackbufferImage(Device);
	}

	protected void SetResolution(int xres, int yres) {
		screen.width = xres;
		screen.height = yres;
		manager.PreferredBackBufferWidth = xres;
		manager.PreferredBackBufferHeight = yres;
		manager.PreferMultiSampling = false;
		manager.ApplyChanges();

		//UNSURE - I think we should view drawing as always available,
		//but after this one should expect the state to reset
		BeginDraw();
	}

	private void LoadGraphics() {
		//I think we needed to re-grab this here
		Device = manager.GraphicsDevice;

		//these get destroyed, so we need to reload them here
		LoadShaders();
		SimpleVertex.Decl = new VertexDeclaration(SimpleVertex.VertexElements);
		VertexVector2.Decl = new VertexDeclaration(VertexVector2.VertexElements);
	}

	private void UnloadGraphics() {
	}

	//public IDisposable VertexType(IVertexType type) { return new TempVertexDeclaration(Device, type); }

	//class TempVertexDeclaration : IDisposable
	//{
	//    GraphicsDevice device;
	//    VertexDeclaration decl;
	//    public TempVertexDeclaration(GraphicsDevice device, IVertexType type)
	//    {
	//        throw new NotSupportedException("not supported yet..");
	//        //this.device = device;
	//        //this.decl = device.VertexDeclaration;
	//        //device.VertexDeclaration = decl;
	//    }
	//    public void Dispose()
	//    {
	//        throw new NotSupportedException("not supported yet..");
	//        //device.VertexDeclaration = decl;
	//    }
	//}

	
	public CarotRasterizerState RasterizerState = new CarotRasterizerState();
	public CarotBlendState BlendState = new CarotBlendState();
	public CarotDepthStencilState DepthStencilState = new CarotDepthStencilState();
	public CarotSamplerState[] SamplerStates = new CarotSamplerState[] {
		new CarotSamplerState(0), new CarotSamplerState(1), new CarotSamplerState(2), new CarotSamplerState(3)
	};

	/// <summary>
	/// reset the state to something predictable for RenderState rendering.
	/// you may call it at any time to return to a predictable render state
	/// </summary>
	protected void prepareRenderState() {
		//setup vertex shader
		//Device.VertexShader = vertexShaders["Transform"];
		//Device.VertexDeclaration = SimpleVertex.Decl;

		//render state
		RasterizerState.CullMode = CullMode.None;
		DepthStencilState.DepthBufferEnable = false;
		DepthStencilState.DepthBufferWriteEnable = false;
		//renderState.Clipping = false;
		//renderState.LastPixel = true;
		BlendNormal();

		SetPointFiltering();

		//defunct...
		//Device.DepthStencilBuffer = null;
	
		Techniques.clear();
		SetTex(null);
        SetDest(null);
	}

	/// <summary>
	/// Sets alpha blending to normal pre-multiplied state
	/// </summary>
	public void BlendNormal()
	{
		BlendState.ColorDestinationBlend = Blend.InverseSourceAlpha;
		BlendState.AlphaDestinationBlend = Blend.InverseSourceAlpha;
		BlendState.ColorSourceBlend = Blend.One;
		BlendState.ColorBlendFunction = BlendFunction.Add;
		BlendState.ColorWriteChannels = ColorWriteChannels.All;
	}

	/// <summary>
	/// Sets alpha blending to an additive style
	/// </summary>
	public void BlendAdditive()
	{
		BlendState.ColorDestinationBlend = Blend.One;
		BlendState.AlphaDestinationBlend = Blend.One;
		BlendState.ColorSourceBlend = Blend.One;
		BlendState.ColorBlendFunction = BlendFunction.Add;
		BlendState.ColorWriteChannels = ColorWriteChannels.All;
	}

	/// <summary>
	/// bypasses alpha blending (doesnt actually disable it though, be aware in case theres a difference)
	/// </summary>
	public void BlendNone()
	{
		BlendState.ColorDestinationBlend = Blend.Zero;
		BlendState.AlphaDestinationBlend = Blend.Zero;
		BlendState.ColorSourceBlend = Blend.One;
		BlendState.ColorBlendFunction = BlendFunction.Add;
		BlendState.ColorWriteChannels = ColorWriteChannels.All;
	}

	protected void BeginDraw() {
		prepareRenderState();
		_setDest(screen, true);
		SetTex(null);
	}


	/// <summary>
	/// sets min and mag filtering to nearest
	/// </summary>
	public void SetPointFiltering() { SetPointFiltering(0); }

	/// <summary>
	/// sets min and mag filtering to nearest
	/// </summary>
	public void SetPointFiltering(int sampler) {
		SamplerStates[sampler].Filter = TextureFilter.Point;
	}

	/// <summary>
	/// sets min and mag filtering to linear
	/// </summary>
	public void SetLinearFiltering() { 
		setLinearFiltering(0);
	}

	/// <summary>
	/// sets min and mag filtering to linear
	/// </summary>
	public void setLinearFiltering(int sampler) {
		SamplerStates[sampler].Filter = TextureFilter.Linear;
	}

	
	Effect shaders;
	//loads shaders that we want specific control over, outside the purview of techniques
	void LoadShaders() {
		shaders = Content.GetEffect("shaders/shaders.xnb");
		Techniques.init(shaders);
		shaders.Parameters["master_texflip_h"].SetValue(false);
		shaders.Parameters["master_texflip_v"].SetValue(false);
	}

	public Image NewImage(Size size) { return NewImage(size.Width, size.Height); }

	public Image NewImage(int w, int h) {
		return new Image(Device, w, h);
	}

	public Image LoadSoftimage(Softimage si) {
		Image img = new Image(Device, si.Width, si.Height);
		img.loadSoftimage(si);
		return img;
	}

	/// <summary>
	/// loads an image from the content manager
	/// </summary>
	public Image LoadImage(string name) {
		return Content.LoadImage(Device, name);
	}

	/// <summary>
	/// loads an image from the content manager
	/// </summary>
	public Image LoadImage(Stream stream) {
		return Content.LoadImage(Device, stream);
	}

	/// <summary>
	/// loads an 8bit image directly. color 0 will become transparent. (only pngs are supported right now)
	/// </summary>
	public Image LoadImage0(string name) {
		return Content.LoadImage0(this, name);
	}

	/// <summary>
	/// loads an 8bit image directly. color 0 will become transparent. (only pngs are supported right now)
	/// </summary>
	public LargeImage LoadLargeImage0(string name)
	{
		return Content.LoadLargeImage0(this, name);
	}


	/// <summary>
	/// loads, using a provided tcolor, the 32bpp raw image data with the specified pointer and dimensions. pitch should equal width
	/// </summary>
	unsafe internal Image LoadImage32(int width, int height, void* data, Color tcolor) {
		SoftGraphics sg = new SoftGraphics();
		Softimage si = sg.newImage(width, height);
		si.load((int*)data);
		int* dptr = si.data;
		pr2.Common.Lib.alpha32(dptr, width * height, toArgb(tcolor));
		Image img = NewImage(width, height);
		img.loadSoftimage(si);
		si.Dispose();
		sg.Dispose();
		return img;
	}

	/// <summary>
	/// loads, using a provided tcolor, the 24bpp raw image data with the specified pointer and dimensions. pitch should equal width
	/// </summary>
	unsafe internal Image LoadImage24(int width, int height, void* data, Color tcolor) {
		SoftGraphics sg = new SoftGraphics();
		Softimage si = sg.newImage(width, height);
		int* dptr = si.data;
		pr2.Common.Lib.load24(dptr, data, width * height, toArgb(tcolor));
		Image img = NewImage(width, height);
		img.loadSoftimage(si);
		si.Dispose();
		sg.Dispose();
		return img;
	}

	Image dest;

	/// <summary>
	/// Sets the current rendering target. 
	/// Also, unless we are in CustomRender mode, this wipes out the current Matrices and sets a pixel perfect projection and view (to make the rendering state suitable for rendering to this destination with pixel perfection)
	/// </summary>
	public void SetDest(Image newdest) { _setDest(newdest, false); }
	void _setDest(Image newdest, bool force) {
		if(dest == newdest && !force) return;

        if (newdest == null)
        {
            dest = null;
            return;
        }

		//resolve the current target (if there is one)
		if(dest != null) dest.Resolve();

		dest = newdest;

		int w, h;
		dest.setTarget();
		w = dest.Width;
		h = dest.Height;

        if (activeGRS != null)
            InstallPixelPerfectTransforms();

	}

	/// <summary>
	/// Sets current view and projection transform to matrices suitable for rendering to the current destination with pixel perfection
	/// </summary>
    public void InstallPixelPerfectTransforms()
    {
        Matrices.set(Matrix.Identity, GeneratePixelPerfectViewTransform(dest.Width, dest.Height), GeneratePixelPerfectProjectionTransform(dest.Width, dest.Height), Matrix.Identity);
    }

	Image tex;

	/// <summary>
	/// Sets the current source texture for rendering
	/// </summary>
	public void SetTex(Image newtex) {
		if(tex == newtex) return;
		tex = newtex;
		if(tex == null)
			Device.Textures[0] = null;
		else Device.Textures[0] = tex.getTex();
	}

	//----render state

	GRS activeGRS;
	float[] constant4 = new float[4];
	void pushGRS(GRS grs, bool force) {
		//push dest
		if(grs.isDestDirty || force) {
			SetDest(grs._dest);
			if(grs.pick)
				applyPickingMatrix(1, 1, grs.pick_x, grs.pick_y);
		}

		//push render mode
		if(grs.isRenderModeDirty || force) {
			//if(grs.pick) {
			//    setRenderMode(RenderMode.Color);
			//    device.RenderState.AlphaBlendEnable = false;
			//} else {
			if(grs._renderMode != null) {
				grs._renderMode.Apply();
				grs.isSrcDirty = true;
				//setting the render mode will often reset the source texture to null
				//force it to be restored here
			}
			//}
		}

		//push color
		if(grs.isColorDirty || force)
			if(grs.pick)
				SetRenderColor(grs.pick_val);
			else
				SetRenderColor(grs._alpha, grs._color);

		//push src
		if(grs.isSrcDirty || force)
			SetTex(grs._src);

		//push transform
		if(grs.IsTransformDirty || force)
			World(grs.GetTransform());

		//push clipping
		if(grs.isClipDirty || force)
			setClip(grs._clipEnabled, grs._clipRectangle);


		grs.undirty();
	}

	/// <summary>
	/// sets the specified render state to active and ensures that its state is pushed into the hardware.
	/// may be null if you are going to control d3d manually.
	/// </summary>
	public void setActiveRenderState(GRS grs) {
		//if the new active state is the pre-existing active state, just unforceingly push it
		if(activeGRS == grs) {
			if(grs != null) pushGRS(activeGRS, false);
			goto end;
		}
		//if the current render state is null, then we need to reset the 3d engine to the prerequisites for GRS
		if(activeGRS == null && grs != null) prepareRenderState();
		activeGRS = grs;
		if(activeGRS == null) return;
		pushGRS(activeGRS, true);
	end:
		currTechnique.Refresh();
		RasterizerState.Apply();
		BlendState.Apply();
		DepthStencilState.Apply();
		for(int i=0;i<SamplerStates.Length;i++)
			SamplerStates[i].Apply();
	}

	/// <summary>
	/// Sets the current rendering color directly. Alpha is set to 1.0
	/// </summary>
	public void SetRenderColor(int color) {
		//device.SetPixelShaderConstant(0, makeColor(color).ToVector4());
		//TODO

		_currColor.PackedValue = (uint)color;
		_currAlpha = 1.0f;
		currTechnique.SetColor(_currAlpha, _currColor);
	}

	/// <summary>
	/// Sets the current rendering source color and alpha modulate factor
	/// </summary>
	public void SetRenderColor(float alpha, Color color) {
		//float[] constant4 = new float[4];

		//if(activeGRS != null && activeGRS.renderMode == RenderMode.TextureAlpha)
		//    constant4[3] = (alpha / 255.0f);
		//else {
		//    constant4[0] = color.R / 255.0f; constant4[1] = color.G / 255.0f;
		//    constant4[2] = color.B / 255.0f; constant4[3] = (color.A / 255.0f) * alpha;
		//}
		//shaders.Parameters["fixedColor"].SetValue(constant4);
		//if(currPass != null)
		//	shaders.CommitChanges();
		_currColor = color;
		_currAlpha = alpha;
		currTechnique.SetColor(alpha, color);
	}
	float _currAlpha;
	Color _currColor;

	/// <summary>
	/// Enables or disables destination clipping using the specified rectangle
	/// </summary>
	public void setClip(bool enabled, Rectangle rect) {
		RasterizerState.ScissorTestEnable = enabled;
		//if(enabled)
		//    if(Device.ScissorRectangle != rect)
		//        Device.ScissorRectangle = rect;
		//TODO
	}

	/// <summary>
	/// Call this when you are about to perform custom rendering without a blitter.
	/// Youll be flying by the seat of your pants, then, but when you're done,
	/// the game engine will be able to recover its state for normal rendering
	/// </summary>
	public void CustomRender() {
		setActiveRenderState(null);
		Techniques.clear();
	}

	//----------depth buffer management----------

	/// <summary>
	/// please do not dispose the depth buffer.
	/// </summary>
	//public DepthStencilBuffer GetDepthBuffer(int width, int height, DepthFormat format) {
	//    if(width > depthBuffer_width || height > depthBuffer_height || _depthBuffer == null) {
	//        if(_depthBuffer != null) _depthBuffer.Dispose();
	//        _depthBuffer = new DepthStencilBuffer(Device, width, height, format);

	//    }
	//    return _depthBuffer;
	//}

	//DepthStencilBuffer _depthBuffer;
	//int depthBuffer_width = 0, depthBuffer_height = 0;

	//public DepthStencilBuffer GetCurrentDepthBuffer() { return Device.DepthStencilBuffer; }

	///// <summary>
	///// creates and activates a depth buffer of the given width and height
	///// </summary>
	//public void SetDepthBuffer(int width, int height, DepthFormat format) {
	//    Device.DepthStencilBuffer = GetDepthBuffer(width, height, format);
	//}

	///// <summary>
	///// creates and activates a depth buffer to match the current screen
	///// </summary>
	//public void SetDepthBuffer(DepthFormat format) {
	//    SetDepthBuffer(screen.Width, screen.Height, format);
	//}

	//public void RemoveDepthBuffer() {
	//    Device.DepthStencilBuffer = null;
	//}


	//----------
	//misc..

#if !XBOX360
	/// <summary>
	/// returns null if there is no image. be sure to dispose!
	/// </summary>
	public Image GetImageFromClipboard() {
		System.Drawing.Image img = System.Windows.Forms.Clipboard.GetImage();
		if(img == null) return null;
		Image ret = NewImage(img.Width, img.Height);
		using(System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(img))
			using(Softimage si = new Softimage(bmp)) {
				ret.loadSoftimage(si);
			}

		return ret;
	}
#endif


	//misc utils to move or remove
	//TODO - reduce the number of these we need and reconcile with builtin Color philosophies and methods
	public static Color MakeColor(int r, int g, int b) { return new Color((byte)r, (byte)g, (byte)b); }
	public static Color MakeColor(int a, int r, int g, int b) { return new Color((byte)r, (byte)g, (byte)b, (byte)a); }
	public static Color MakeColor(int x) { 
		return new Color((byte)(x & 0xFF), (byte)((x & 0xFF00) >> 8), (byte)((x & 0xFF0000) >> 16), (byte)(((uint)x & 0xFF000000) >> 24));
	}
	public static Color MakeColor(int a, Color c) { return new Color(c.R, c.G, c.B, (byte)a); }
	public static Color MakeColor(float a, Color c) { return new Color(c.R, c.G, c.B, (byte)(a*255)); }
	public static Color MakeColor(double a, Color c) { return new Color(c.R, c.G, c.B, (byte)(a*255)); }
	public static Color MakeColor(double a, int r, int g, int b) { return new Color((byte)r, (byte)g, (byte)b, (byte)(a*255)); }
	public static Color MakeColor(float a, int r, int g, int b) { return new Color((byte)r, (byte)g, (byte)b, (byte)(a*255)); }
	public static int toArgb(Color col) { 
		return ((int)col.R) | ((int)col.G << 8) | ((int)col.B << 16) | ((int)col.A << 24);
	}
}

}