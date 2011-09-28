using System.Drawing;
using System;
using dx = Microsoft.DirectX;
using d3d = Microsoft.DirectX.Direct3D;
using pr2.Common;

namespace pr2.CarotEngine
{

	public class Rocky : d3dChrController
	{
		XCharacter model;
		public Rocky(GraphicsHW g) : base(g,true,128,128,192,192)
		{
			hotspot = new Rectangle(48, 80, 32, 32);
			model = new XCharacter(g, @"/ApplicationFolder/rocky.x");
		}
		
		Directions facing = Directions.s;

		public override bool supportsCommand(CharacterCommands cc) { return true; }
		public override void command(CharacterCommands cc, params object[] args)
		{
			switch(cc)
			{
				case CharacterCommands.Face:
					facing = (Directions)args[0];
					break;
			}
		}

		public override void tick() { }

		protected override void render(FunkImage dest)
		{
			g.begin3d();
			g.setDestAndDepthBuffer(dest);
			g.clearDest();
			g.setRenderMode(GraphicsHW.RenderMode.Texture);
			//??
			g.device.Transform.View = Microsoft.DirectX.Matrix.LookAtLH(new Microsoft.DirectX.Vector3(0, 0, -50), new Microsoft.DirectX.Vector3(0, 0, 0), new Microsoft.DirectX.Vector3(0, 1, 0));
			g.device.Transform.Projection = dx.Matrix.PerspectiveFovLH(1.04719755f, 1.33333f, 1, 1000);
			//??
			//g.device.Transform.World = dx.Matrix.Scaling(1, 1, 1);
			g.device.Transform.World = dx.Matrix.Identity;
			//g.device.Transform.World = dx.Matrix.Scaling(20, 20, 20);
			g.device.RenderState.AlphaBlendEnable = false;
			g.device.RenderState.CullMode = Microsoft.DirectX.Direct3D.Cull.CounterClockwise;
			g.device.RenderState.ZBufferEnable = true;
			g.device.RenderState.Lighting = false;
			g.setLinearFiltering();

			MatrixStack ms = new MatrixStack(g.device.Transform.World);
			ms.Scale(1, 1, -1);
			ms.RotateX(Lib.rads(45));
			ms.Translate(0, -10, 0);
			ms.Scale(1.2f);
			if(facing == Directions.w) ms.RotateAxisLocal(0, 0, 1, Lib.rads(90));
			else if(facing == Directions.s) ms.RotateAxisLocal(0, 0, 1, Lib.rads(180));
			else if(facing == Directions.e) ms.RotateAxisLocal(0, 0, 1, Lib.rads(270));
			g.device.Transform.World = ms.Top;

			g.setRenderColor(1, Color.SpringGreen);
			g.setRenderMode(GraphicsHW.RenderMode.Color);
			//g.renderLine(0, 0, 0, 0, 0, 20);
			//g.renderLine(-4, 0, 0, 4, 0, 0);
			//g.renderLine(0, 4, 0, 0, -4, 0);
			g.setRenderMode(GraphicsHW.RenderMode.Texture);
			model.Render();
		}
	}

	public class Sparrow : d3dChrController {

		Model model;
		int counter = 0;

		public Sparrow(GraphicsHW g) : base(g,true,64,64,96,96) {
			hotspot = new Rectangle(16, 32, 32, 16);
			model = MilkshapeModel.load(g, "sparrow.ms3d");
		}

		const float north = 0;
		const float south = (float)Math.PI;
		const float east = (float)Math.PI/2;
		const float west = (float)Math.PI * 3.0f / 2.0f;

		public override bool supportsCommand(CharacterCommands cc) { return true; }
		public override void command(CharacterCommands cc, params object[] args) {
			switch(cc) {
				case CharacterCommands.Face:
					Directions d = (Directions)args[0];
					switch(d) {
							//there are cleverer ways to do this
							//like take the difference one way and if its greater than pi, invert delta
						case Directions.e:
							target = east;
							if(angle > east && angle < west) delta = -1;
							else delta = 1;
							break;
						case Directions.n:
							target = north;
							if(angle > south) delta = 1;
							else delta = -1;
							break;
						case Directions.s:
							target = south;
							if(angle < south) delta = 1;
							else delta = -1;
							break;
						case Directions.w:
							target = west;
							if(angle < west && angle > east) delta = 1;
							else delta = -1;
							break;
					}
					break;
			}
		}

		public override void tick() { 
			counter++;
			float diff = Math.Abs(target - angle);
			if((diff < 2.0f*(float)Math.PI && diff > rotspeed) || diff > rotspeed + 2.0f*Math.PI) {
				angle += delta * rotspeed;
				angle %= 2.0f * (float)Math.PI;
			} else angle = target;
		}

		const float rotspeed = 0.012f;
		float target;
		float delta;
		float angle = south;

		protected override void render(FunkImage dest) {

			g.begin3d();

			g.setDestAndDepthBuffer(dest);
			g.clearDest();
			
			
			g.setRenderMode(GraphicsHW.RenderMode.Texture);
			g.setLinearFiltering();
			g.device.RenderState.CullMode = d3d.Cull.CounterClockwise;
			g.device.RenderState.ZBufferEnable = true;
			g.device.VertexFormat = GraphicsHW.FVFNormal.format;
			//g.device.SetTexture(0, model.tex);
			g.setTex(model.tex);
			
			//float rads = DateTime.Now.Ticks % 5000000 * 2.0f * 3.14f / 5000000f;
			float rads = DateTime.Now.Ticks % 5000000 * 2.0f * 3.14f / 5000000f;
			g.device.Transform.View = dx.Matrix.LookAtLH(new Microsoft.DirectX.Vector3(0, 0, -200), new dx.Vector3(0, 0, 0), new dx.Vector3(0, 1, 0));
			g.device.Transform.Projection = dx.Matrix.PerspectiveFovLH(3.14f*2.0f*60.0f/360.0f, 1, 1, 1000);
			g.device.Transform.World = dx.Matrix.Identity;

			g.device.Transform.World *= dx.Matrix.RotationY(angle - 3.14f);

			g.device.Transform.World *= dx.Matrix.RotationX(-2.0f * 3.14f / 8);
			g.device.Transform.World *= dx.Matrix.Scaling(200 / model.scale, 200 / model.scale, 1);
			g.device.Transform.World *= dx.Matrix.Translation(0, (float)Math.Sin(rads)*5.0f, 0);

			model.render();
		}

		public override void Dispose() {
			model.Dispose();
			base.Dispose();
		}
		
	}

	public class SquareBob : d3dChrController {

		GraphicsHW.FVF[] model;

		public SquareBob(GraphicsHW g) : base(g,false,16,32,32,64) {
			hotspot = new Rectangle(0, 16, 16, 16);

			model = new GraphicsHW.FVF[4];
			int i = 0;
			model[i++] = new GraphicsHW.FVF(-1.0f, -1.0f, 0.0f); //back
			model[i++] = new GraphicsHW.FVF(1.0f, -1.0f, 0.0f);
			model[i++] = new GraphicsHW.FVF(1.0f, 1.0f, 0.0f);
			model[i++] = new GraphicsHW.FVF(-1.0f, 1.0f, 0.0f);
			model[0].color = model[1].color = model[2].color = model[3].color = Color.FromArgb(255, 0, 0, 0).ToArgb();
		}

		public override bool supportsCommand(CharacterCommands cc) { return true;  }
		public override void tick() { }
		public override void command(CharacterCommands cc, params object[] args) {}

		protected override void render(FunkImage dest) {
			g.setDest(dest);
			dest.clear(0);
			g.setRenderMode(GraphicsHW.RenderMode.Gradient);
			g.device.Transform.Projection = g.generatePixelPerfectProjectionTransform(16, 32);
			g.device.Transform.View = g.generatePixelPerfectViewTransform(16, 32);
			g.device.Transform.World = Microsoft.DirectX.Matrix.Identity;
			g.device.Transform.World *= Microsoft.DirectX.Matrix.RotationZ(DateTime.Now.Ticks % 10000000 * 2.0f * 3.14f / 10000000f);
			g.device.Transform.World *= Microsoft.DirectX.Matrix.Scaling(6, 6, 1);
			g.device.Transform.World *= Microsoft.DirectX.Matrix.Translation(8, 24, 0);
			g.device.Transform.World *= Microsoft.DirectX.Matrix.Translation(0, -(float)Math.Abs(Math.Sin(DateTime.Now.Ticks % 8000000 * 2.0f * 3.14f / 8000000f)) * 12.0f, 0);
			g.device.DrawUserPrimitives(Microsoft.DirectX.Direct3D.PrimitiveType.TriangleFan, 2, model);
			g.device.Transform.World = Microsoft.DirectX.Matrix.Identity;
			//g.resetState();
		}
	}

	public abstract class d3dChrController : ICharacter {

		/// <summary>
		/// toggle antialiasing
		/// </summary>
		public bool antialias;

		protected Rectangle hotspot;

		/// <summary>
		/// the graphics engine to use for rendering
		/// </summary>
		protected GraphicsHW g;

		/// <summary>
		/// render your frame into the specified image. if you asked for it to be antialiased, then its twice as big as the size you specified.
		/// however, you can establish a projection transform with the desired size and you wont even have to know it.
		/// </summary>
		protected abstract void render(FunkImage dest);
			
		/// <summary>
		/// frame rendering buffers
		/// </summary>
		FunkImage image, image2x;

		int w, h, w2x, h2x;
		Blitter b;

		/// <summary>
		/// pass in true if you want to enable antialiasing. it will then default to on
		/// </summary>
		protected d3dChrController(GraphicsHW g, bool antialias, int w, int h, int w2x, int h2x) {
			this.w = w; this.h = h;
			this.antialias = antialias;
			image = g.newImage(w,h);
			if(antialias && !Config.disableModelAntialiasing) {
				b = new Blitter(image);
				this.w2x = w2x; this.h2x = h2x;
				image2x = g.newImage(w2x,h2x);
			}
			this.g = g;
		}

		//ICharacter interface 
		Rectangle ICharacter.hotspot { get { return hotspot; } }
		public virtual void Dispose() { image.Dispose(); if(image2x != null) image2x.Dispose();  }
		public abstract bool supportsCommand(CharacterCommands cc);	
		public abstract void tick();
		public abstract void command(CharacterCommands cc, params object[] args);
		

		public FunkImage frame {
			get {
				//todo - at some point this broke
				//if(antialias && image2x != null) {
				//    render(image2x);
				//    image.clear(0);
				//    b.magiblur(image2x, w, h);
				//} else 
					render(image);
				return image;
			}
		}



	}
}