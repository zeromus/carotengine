using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using pr2.Common;

namespace pr2.CarotEngine {
public class Blitter : GameEngineComponent {

	public Blitter() {
		grs = new GameEngine.GRS();
		grs.setColor(Alpha, Color);
	}

	public Blitter(Image dest) {
		_dest = dest;
		grs = new GameEngine.GRS();
		grs.ClipEnabled = true;
		grs.dest = dest;
		grs.setColor(Alpha, Color);
		window = new Rectangle(0, 0, dest.Width, dest.Height);
	}

	GameEngine.GRS grs;

	//state
	Image _dest;
	public float Alpha { get { return grs.alpha; } set { grs.alpha = value; } }
	public Color Color { get { return grs.color; } set { grs.color = value; } }

	bool _silhouette;
	/// <summary>
	/// puts the blitter into silhouette mode. this is compatible with all the standard blitters
	/// </summary>
	public bool EnableSilhouette { get { return _silhouette; } set { _silhouette = value; } }

	bool _modulate;
	/// <summary>
	/// puts the blitter into modulate mode. this is compatible with all the standard blitters
	/// </summary>
	public bool EnableModulate { get { return _modulate; } set { _modulate = value; } }

	/// <summary>
	/// the current gradient parameters. use as many as is logical i.e. 2 for a line, 3 for triangle, 4 for quadrilateral
	/// </summary>
	public Color[] gradient = new Color[4];

	/// <summary>
	/// Gets or Sets the source texture, specifically for drawing operations that dont explicitly take textures
	/// But it is always kept up to date with the most recently used texture
	/// </summary>
	public Image Source { get { return grs.src; } set { grs.src = value; } }

	void blitPrepCommon(Image src) {
		grs.src = src;
		
		//this used to change render mode depending on whether alpha was set. 
		//I dont see any reason not to always use the alpha, though. I bet it's the same speed.
		//if(alpha == 1.0f)
		//    grs.renderMode = game.techniques.texture;
		//else {
		//    grs.renderMode = game.techniques.textureAlpha;
		//    grs.alpha = alpha;
		//}

		grs.alpha = Alpha;
		if(_modulate)
			grs.renderMode = game.Techniques.modulate;
		else if(_silhouette)
			grs.renderMode = game.Techniques.silhouette;
		else grs.renderMode = game.Techniques.textureAlpha;

	}

	/// <summary>
	/// updates the current transform with new offsetting values
	/// </summary>
	void updateTransform() {
		grs.transform = Matrix.CreateTranslation(_window.X, _window.Y, 0);
	}

	/// <summary>
	/// Sets the current rendering destination.
	/// TODO - is it a good idea to change this or should we make a new blitter?
	/// </summary>
	public Image Dest {
		get { return _dest; }
		set {
			grs.dest = _dest = value;
			window = new Rectangle(0, 0, Dest.Width, Dest.Height);
		}
	}

	//operations
	public Color TColor = Color.Transparent;
	[Obsolete]
	public Color MakeColor(int r, int g, int b) { return GameEngine.MakeColor(r, g, b); }
	[Obsolete]
	public Color MakeColor(int a, int r, int g, int b) { return GameEngine.MakeColor(a, r, g, b); }
	[Obsolete]
	public Color MakeColor(int x) { return GameEngine.MakeColor(x); }
	public void SetColor(Color c) { Color = c; }
	public void SetColor(int a, Color c) { Color = GameEngine.MakeColor(a, c); }
	public void SetColor(int r, int g, int b) { Color = GameEngine.MakeColor(r, g, b); }
	public void SetColor(int a, int r, int g, int b) { Color = GameEngine.MakeColor(a, r, g, b); }
	public void SetColor(int x) { Color = GameEngine.MakeColor(x); }

	Stack<float> alphaStack = new Stack<float>();
	public void PushAlpha(float alphaFactor) { alphaStack.Push(Alpha); Alpha *= alphaFactor; }
	public void PopAlpha() { Alpha = alphaStack.Pop(); }

	public MatrixStack Transform { get { return grs.UserTransform; } }

	public void Clear(Color c) {
		grs.Activate();
		game.clear(c);
		//ugh no support for window here
	}
	public void Clear(int c) {
		Clear(GameEngine.MakeColor(c));
	}
	public void TClear() {
		Clear(TColor);
	}

	//------------------------------
	//windowing 

	/// <summary>
	/// 
	/// </summary>
	public Rectangle window {
		get { return _window; }
		set {
			_window = value;
			grs.clipRectangle = value;
			updateTransform();
		}
	}

	/// <summary>
	/// The width of the current rendering window
	/// </summary>
	public int width { get { return _window.Width; } }

	/// <summary>
	/// The height of the current rendering window
	/// </summary>
	public int height { get { return _window.Height; } }

	/// <summary>
	/// returns window to the default (entire dest image)
	/// </summary>
	public void resetWindow() {
		window = new Rectangle(0, 0, Dest.Width, Dest.Height);
	}

	/// <summary>
	/// the current window
	/// </summary>
	Rectangle _window;

	/// <summary>
	/// the window stack
	/// </summary>
	Stack<Rectangle> windows = new Stack<Rectangle>();

	/// <summary>
	/// applies the specified window to the current window (a subrectangle)
	/// pushes the old window to a stack
	/// </summary>
	public void ApplyWindow(Rectangle newWindow) {
		if(newWindow.Width > _window.Width) throw new ArgumentException("New window width is larger than current window width");
		if(newWindow.Height > _window.Height) throw new ArgumentException("New window height is larger than current window height");
		if(newWindow.Right > _window.Width) throw new ArgumentException("New window right edge is beyond right edge of current window");
		if(newWindow.Bottom > _window.Height) throw new ArgumentException("New window bottom edge is beyond bottom edge of current window");
		if(newWindow.X < 0) throw new ArgumentException("New window X offset cannot be less than 0");
		if(newWindow.Y < 0) throw new ArgumentException("New window Y offset cannot be less than 0");
		windows.Push(_window);
		window = new Rectangle(_window.X + newWindow.X, _window.Y + newWindow.Y, newWindow.Width, newWindow.Height);
	}

	/// <summary>
	/// applies the specified window to the current window (a subrectangle)
	/// pushes the old window to a stack
	/// </summary>
	public void ApplyWindow(int x, int y, int w, int h) { ApplyWindow(new Rectangle(x,y,w,h)); }

	/// <summary>
	/// applies the specified offset to the current window
	/// pushes the old window to a stack
	/// </summary>
	public void ApplyWindow(int x, int y) { ApplyWindow(new Rectangle(x, y, window.Width-x, window.Height-y)); }

	/// <summary>
	/// offsets the current window without pushing
	/// </summary>
	public void OffsetWindow(int x, int y) {
		Rectangle r = _window;
		r.X += x;
		r.Y += y;
		window = r;
	}

	/// <summary>
	/// sets the current window to begin at the provided offset without changing the size
	/// </summary>
	public void SetOffsetWindow(int x, int y) {
		Rectangle r = _window;
		r.X = x;
		r.Y = y;
		window = r;
	}

	/// <summary>
	/// pushes the current window
	/// </summary>
	public void PushWindow() {
		windows.Push(_window);
	}

	/// <summary>
	/// pops the latest pushed window and makes it active
	/// </summary>
	public void PopWindow() {
		window = windows.Pop();
	}

	//public void applyPadding(int left, int top, int right, int bottom)

	//-----------------------

	/// <summary>
	/// Forces this blitter to activate itsself with direct3d, but not do anything
	/// </summary>
	public void forceActivate() {
		grs.Activate();
	}

	public void VLine(int x, int y, int h) {
		RectFill(x, y, 1, h);
	}

	public void HLine(int x, int y, int w) {
		RectFill(x, y, w, 1);
	}

	/// <summary>
	/// renders a line. endpoints inclusive? dunno.
	/// </summary>
	public void Line(Point a, Point b) {
		Line(a.X, a.Y, b.X, b.Y);
	}

	/// <summary>
	/// renders a line. endpoints inclusive? dunno.
	/// </summary>
	public void Line(int x, int y, int x1, int y1) {
		grs.renderMode = game.Techniques.color;
		grs.setColor(Alpha, Color);
		grs.Activate();
		game.RenderLine(x, y, x1, y1);
	}

	public void EngageModulateMode() { grs.renderMode = game.Techniques.modulate; }
	public void EngageTextureMode() { grs.renderMode = game.Techniques.texture; }
	public void EngineColorMode() { grs.renderMode = game.Techniques.color; }

	/// <summary>
	/// renders a line. endpoints inclusive? dunno.
	/// </summary>
	public void Line(float x, float y, float x1, float y1) {
		grs.renderMode = game.Techniques.color;
		grs.setColor(Alpha, Color);
		grs.Activate();
		game.RenderLine(x, y, x1, y1);
	}

	public void Line(Vector2 a, Vector2 b) {
		Line(a.X, a.Y, b.X, b.Y);
	}

	/// <summary>
	/// renders a quadrilateral. hopefully with the endpoints included
	/// </summary>
	public void Quadrilateral(int x0, int y0, int x1, int y1, int x2, int y2, int x3, int y3) {
		grs.setColor(Alpha, Color);
		grs.Activate();
		game.RenderQuadrilateral(x0, y0, x1, y1, x2, y2, x3, y3);
	}

	/// <summary>
	/// renders a quadrilateral. hopefully with the endpoints included
	/// </summary>
	public void Quadrilateral(Vector2[] points) {
		//if(points.Length != 4) throw new Exception("Sent wrong number of points to Quadrilateral()");
		grs.setColor(Alpha, Color);
		grs.Activate();
		game.RenderQuadrilateral(points[0].X, points[0].Y, points[1].X, points[1].Y, points[2].X, points[2].Y, points[3].X, points[3].Y);
	}
	/// <summary>
	/// renders a triangle. hopefully with the endpoints included
	/// </summary>
	public void Triangle(Vector2[] points) {
		if(points.Length != 3) throw new Exception("Sent wrong number of points to Triangle()");
		Triangle(points[0].X, points[0].Y, points[1].X, points[1].Y, points[2].X, points[2].Y);
	}

	/// <summary>
	/// renders a triangle. hopefully with the endpoints included
	/// </summary>
	public void Triangle(float x0, float y0, float x1, float y1, float x2, float y2) {
		grs.setColor(Alpha, Color);
		grs.Activate();
		game.RenderTriangle(x0, y0, x1, y1, x2, y2);
	}

	/// <summary>
	/// fills the specified rectangle
	/// </summary>
	public void RectFill(int x, int y, int w, int h) {
		RectFill((float)x, (float)y, (float)w, (float)h);
	}

	/// <summary>
	/// fills the specified rectangle
	/// </summary>
	public void RectFill(Rectangle rect) {
		RectFill((float)rect.X, (float)rect.Y, (float)rect.Width, (float)rect.Height);
	}

	/// <summary>
	/// fills the specified rectangle
	/// </summary>
	public void RectFill(float x, float y, float w, float h) {
		if (isInBatch)
		{
			game.BatchRectangle(x, y, w, h);
		}
		else
		{
			grs.renderMode = game.Techniques.color;
			grs.setColor(Alpha, Color);
			grs.Activate();
			game.RenderRectangle(x, y, w, h);
		}
	}

	/// <summary>
	/// fills the specified rectangle
	/// </summary>
	public void RectFill(double x, double y, double w, double h) {
		RectFill((float)x, (float)y, (float)w, (float)h);
	}

	/// <summary>
	/// draws a 1px-wide rectangle
	/// </summary>
	public void Rect(int x, int y, int w, int h) {
		VLine(x, y, h);
		VLine(x + w - 1, y, h);
		HLine(x + 1, y, w - 2);
		HLine(x + 1, y + h - 1, w - 2);
	}

	/// <summary>
	/// fills the specified rectangle with the current gradient.
	/// this also has the possible distinction of being faster than doing a normal rectfill after changing the color
	/// because it uses the vertex color and doesnt change the shader parameter
	/// </summary>
	public void RectGradient(int x, int y, int w, int h) {
		grs.renderMode = game.Techniques.gradient;
		grs.Activate();
		game.RenderRectangleGradient(x, y, w, h, gradient);
	}

	/// <summary>
	/// sets the specified pixel to the current color. this should not BUT CURRENTLY DOES respect scaling and rotating
	/// </summary>
	public void SetPixel(int x, int y) {
		grs.renderMode = game.Techniques.color;
		grs.setColor(Alpha, Color);
		grs.Activate();
		game.RenderPoint(x, y);
	}

	/// <summary>
	/// sets the specified pixel to the specified color. this should not BUT CURRENTLY DOES respect scaling and rotating
	/// </summary>
	public void SetPixel(int x, int y, Color c) {
		Color temp = Color;
		Color = c;
		SetPixel(x, y);
		Color = temp;
	}

	/// <summary>
	/// sets the specified pixel to the specified color. this should not BUT CURRENTLY DOES respect scaling and rotating
	/// </summary>
	public void SetPixel(int x, int y, int c) {
		Color temp = Color;
		SetColor(c);
		SetPixel(x, y);
		Color = temp;
	}

	//---------batch

	bool isInBatch;

	/// <summary>
	/// begins a new blit batch
	/// </summary>
	public void BeginBatch() {
		game.ClearBatches();
		isInBatch = true;
	}
	/// <summary>
	/// executes the current blit batch
	/// </summary>
	public void ExecuteSubrectBatch(Image src) {
		blitPrepCommon(src);
		grs.Activate();
		game.BatchExecuteRectangleClipped();
		isInBatch = false;
	}

	public void ExecuteFillRectBatch()
	{
		grs.renderMode = game.Techniques.color;
		grs.setColor(Alpha, Color);
		grs.Activate();
		game.BatchExecuteRectangleClipped();
		isInBatch = false;
	}

	/// <summary>
	/// batches the specified subrectangle of an image for blitting
	/// </summary>
	public void BlitSubrectBatched(Image src, int sx, int sy, int sw, int sh, int dx, int dy) {
		int w = sw, h = sh;
		game.BatchRectangleClipped(dx, dy, sx, sy, w, h, src.Width, src.Height);
	}

	/// <summary>
	/// batches the specified subrectangle of an image for blitting
	/// </summary>
	public void BlitSubrectBatched(Image src, Rectangle r, int dx, int dy) {
		game.BatchRectangleClipped(dx, dy, r.X, r.Y, r.Width, r.Height, src.Width, src.Height);
	}

	/// <summary>
	/// batches the specified subrectangle of an image for blitting (with stretch)
	/// </summary>
	public void BlitSubrectBatched(Image src, Rectangle r, int dx, int dy, int dw, int dh) {
		game.BatchRectangleClipped(dx, dy, r.X, r.Y, r.Width, r.Height, dw, dh, src.Width, src.Height);
	}

	/// <summary>
	/// immediately blits the specified subtrectangle of an image
	/// </summary>
	public void BlitSubrect(Image src, float sx, float sy, float sw, float sh, float dx, float dy) {
		blitPrepCommon(src);
		float w = sw, h = sh;
		grs.Activate();
		game.RenderRectangleClipped(dx, dy, sx, sy, w, h, src.Width, src.Height);
	}

	/// <summary>
	/// immediately blits the specified subtrectangle of an image
	/// </summary>
	public void BlitSubrect(Image src, int sx, int sy, int sw, int sh, int dx, int dy) {
		blitPrepCommon(src);
		int w = sw, h = sh;
		grs.Activate();
		game.RenderRectangleClipped(dx, dy, sx, sy, w, h, src.Width, src.Height);
	}


	/// <summary>
	/// immediately blits the specified subtrectangle of an image (with stretch)
	/// </summary>
	public void BlitSubrect(Image src, int sx, int sy, int sw, int sh, int dx, int dy, int dw, int dh) {
		blitPrepCommon(src);
		int w = sw, h = sh;
		grs.Activate();
		game.RenderRectangleClipped(dx, dy, sx, sy, w, h, src.Width, src.Height, dw, dh);
	}

	/// <summary>
	/// blits the specified image with stretching, verge-style
	/// </summary>
	public void StretchBlit(Image src, int x, int y, int dw, int dh) {
		blitPrepCommon(src);
		grs.Activate();
		game.RenderRectangle(x, y, dw, dh);
	}

	/// <summary>
	/// blits the specified image with stretching, verge-style
	/// </summary>
	public void StretchBlit(Image src, Rectangle r) {
		blitPrepCommon(src);
		grs.Activate();
		game.RenderRectangle(r.Left, r.Top, r.Width, r.Height);
	}

	/// <summary>
	/// Performs a rotscale centered about the center of the source. angle is in radians.
	/// </summary>
	public void RotScale(Image src, int x, int y, float angle) {
		RotScale(src, x, y, angle, 1, 1, src.Width * 0.5f, src.Height * 0.5f);
	}
	/// <summary>
	/// This overload is the most verge-like, but be sure to note the radian units of the angle parameter and the fact that the scale is not multiplied by 1000!
	/// Performs a rotscale centered about the center of the source and scaled by the provided factor. angle is in radians.
	/// </summary>
	public void RotScale(Image src, int x, int y, float angle, float scale) {
		RotScale(src, x, y, angle, scale, scale, src.Width * 0.5f, src.Height * 0.5f);
	}
	/// <summary>
	/// performs a rotscale centered around the specified point in the source image. angle is in radians. 
	/// </summary>
	public void RotScale(Image src, int x, int y, float angle, float xc, float yc) {
		RotScale(src, x, y, angle, 1, 1, xc, yc);
	}
	/// <summary>
	/// performs a rotscale centered around the specified point in the source image and scaled by the provided factor. angle is in radians.
	/// </summary>
	public void RotScale(Image src, int x, int y, float angle, float scale, float xc, float yc) {
		RotScale(src, x, y, angle, scale, scale, xc, yc);
	}
	/// <summary>
	/// performs a rotscale centered around the specified point in the source image and scaled by the provided x/y factors. angle is in radians.
	/// </summary>
	public void RotScale(Image src, int x, int y, float angle, float xscale, float yscale, float xc, float yc)
	{
		MatrixStack ms = new MatrixStack();
		ms.Translate(-xc, -yc, 0);
		ms.Scale(xscale, yscale, 1);
		ms.RotateAxis(vectorRotZ, angle);
		ms.Translate(_window.X + x, _window.Y + y, 0);
		grs.transform = ms.Top;
		Blit(src, 0, 0);
		updateTransform();
	}
	static Vector3 vectorRotZ = new Vector3(0, 0, 1);

	public void ScaleBlit(Image src, int x, int y, int dw, int dh)
	{
		blitPrepCommon(src);
		grs.Activate();
		game.RenderRectangle(x, y, dw, dh);
	}

	/// <summary>
	/// just a plain old blit that knows how to flip and mirror itself
	/// </summary>
	public void FlipBlit(Image src, int x, int y, bool fx, bool fy)
	{
		FlipBlit(src, (float)x, (float)y, fx, fy);
	}

	/// <summary>
	/// just a plain old blit that knows how to flip and mirror itself
	/// </summary>
	public void FlipBlit(Image src, FxVector3 vec, bool fx, bool fy)
	{
		FlipBlit(src, vec.x.toFloat(), vec.y.toFloat(), fx, fy);
	}


	/// <summary>
	/// just a plain old blit that knows how to flip and mirror itself
	/// </summary>
	public void FlipBlit(Image src, float x, float y, bool fx, bool fy) {
		game.Techniques.SetTexFlip(fx, fy);
		Blit(src, x, y);
		game.Techniques.SetTexFlip(false, false);
	}


	/// <summary>
	/// just a plain old blit that knows how to flip and mirror itself
	/// </summary>
	public void FlipBlit(Image src, Vector2 pos, bool fx, bool fy) {
		FlipBlit(src, pos.X, pos.Y, fx, fy);
	}

	public void pickValue(int val) {
		grs.setPickValue(val);
	}

	public void pickBegin(int x, int y) {
		grs.enablePick(x, y);
	}

	public void pickEnd() {
		grs.disablePick();
	}

	public void Activate() {
		grs.Activate();
	}

	/// <summary>
	/// just a plain old blit
	/// </summary>
	public void Blit(Image src, float x, float y) {
		blitPrepCommon(src);
		grs.Activate();
		game.RenderRectangle(x, y, src.Width, src.Height);
	}

	/// <summary>
	/// just a plain old blit
	/// </summary>
	public void Blit(Image src)
	{
		Blit(src, 0, 0);
	}

	/// <summary>
	/// just a plain old blit
	/// </summary>
	public void Blit(Image src, FxVector3 vec)
	{
		Blit(src, vec.x.toFloat(), vec.y.toFloat());
	}

	/// <summary>
	/// just a plain old blit
	/// </summary>
	public void Blit(Image src, int x, int y) {
		blitPrepCommon(src);
		grs.Activate();
		game.RenderRectangle(x, y, src.Width, src.Height);
	}

	/// <summary>
	/// just a plain old blit
	/// </summary>
	public void Blit(Image src, Vector2 v) {
		Blit(src, v.X, v.Y);
	}

	/// <summary>
	/// just a plain old blit
	/// </summary>
	public void Blit(Image src, Point pt) {
		Blit(src, pt.X, pt.Y);
	}

}

}