using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using pr2;

namespace pr2.CarotEngine {
partial class GameEngine {

	//TODO this is not optimal.. there will be too many multiplies, sets, etc.
	public class MatrixSet {
		GameEngine ge;
		public MatrixSet(GameEngine ge) {
			this.ge = ge;
		}
		Matrix _world, _view, _projection, _texture = Matrix.Identity;
		public Matrix world { get { return _world; } set { _world = value; set(); } }
		public Matrix view { get { return _view; } set { _view = value; set(); } }
		public Matrix projection { get { return _projection; } set { _projection = value; set(); } }
		public Matrix Texture { get { return _texture; } set { _texture = value; setTex(); } }

        public Matrix Concatenation { get { return _world * _view * _projection; } }

		public void set(Matrix world, Matrix view, Matrix projection, Matrix texture) {
			_world = world;
			_view = view;
			_projection = projection;
			set();
			_texture = texture;
			setTex();
		}

		public void set(Matrix world, Matrix view, Matrix projection) {
			_world = world;
			_view = view;
			_projection = projection;
			set();
		}

		public void Push() {
			set();
		}

		void set() {
			ge.shaders.Parameters["worldViewProjection"].SetValue(_world * _view * _projection);
		}

		void setTex() {
			ge.shaders.Parameters["matrixTexture"].SetValue(_texture);
		}
	}

	protected MatrixSet Matrices;

	/// <summary>
	/// generates a proper 2d othographic projection for the given destination size
	/// </summary>
	public Matrix generatePixelPerfectProjectionTransform(int w, int h) {
		Matrix ret = Matrix.Identity;
		ret.M11 = 2.0f / (float)w;
		ret.M22 = 2.0f / (float)h;
		return ret;
	}

	/// <summary>
	/// generates a proper view transform for a standard 2d ortho projection, including half-pixel jitter and
	/// re-establishing of a normal 2d graphics top-left origin
	/// </summary>
	public Matrix generatePixelPerfectViewTransform(int w, int h) {
		Matrix ret = Matrix.Identity;
		ret.M22 = -1.0f;
		ret.M41 = -w * 0.5f - 0.5f;
		ret.M42 = h * 0.5f + 0.5f;
		return ret;
	}
	/// <summary>
	/// calculates a picking matrix for you to post-multiply onto your projection matrix for use in picking operations.
	/// sx/sy are the range that the mx/my values can cover. This is built for use with a 1x1 picking viewport.
	/// </summary>
	public Matrix GeneratePickingMatrix(int sx, int sy, int mx, int my) {
		//THE CRAZY PICKING LOGIC
		//i cant explain it completely. the plan is to look at the bottom 2 as still in viewport coords..
		//so since we are using a 1x1 picking viewport, we need to make it look like the 640x480 that was
		//used to render the screen.  we also need to make sure that mouse cursor gets transformed to the top left
		//of the render area, so it falls into the 1x1 viewport..
		//but oh god so confusing.

		return Matrix.CreateTranslation(mx * -2 / (float)sx, my * 2 / (float)sy, 0)
			* Matrix.CreateScale(sx, sy, 1)
			* Matrix.CreateTranslation(sx, -sy, 0);
	}

	/// <summary>
	/// Applies picking logic to the current transforms. If anything alters them, the picking logic will be lost.
	/// It doesnt matter that its lost since it will break anyway, I think, if the projections get altered
	/// </summary>
	public void applyPickingMatrix(int sx, int sy, int mx, int my) {
		//device.Transform.Projection = device.Transform.Projection * generatePickingMatrix(sx, sy, mx, my);
		//TODO
	}

	/// <summary>
	/// sets the current world transform
	/// </summary>
	public void World(Matrix matrix) {
		Matrices.world = matrix;
	}

	/// <summary>
	/// gets the current world transform
	/// </summary>
	public Matrix World() { return Matrices.world; }

	/// <summary>
	/// clears the world transform
	/// </summary>
	public void WorldClear() {
		Matrices.world = Matrix.Identity;
	}

	/// <summary>
	/// sets the current world transform
	/// </summary>
	public void World(MatrixStack ms) {
		Matrices.world = ms.Top;
	}




}
}