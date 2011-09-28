using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace pr2.CarotEngine {
	partial class GameEngine {

		SimpleVertex[] vtxSprite;
		SimpleVertex[] vtxSpriteClipped;
		SimpleVertex[] vtxTriangles;


		void InitGeometry() {
			vtxSprite = new SimpleVertex[4];
			vtxSprite[0] = new SimpleVertex(0.0f, 0.0f, 0.0f, 0.0f);
			vtxSprite[1] = new SimpleVertex(0.0f, 0.0f, 1.0f, 0.0f);
			vtxSprite[2] = new SimpleVertex(0.0f, 0.0f, 0.0f, 1.0f);
			vtxSprite[3] = new SimpleVertex(0.0f, 0.0f, 1.0f, 1.0f);
			vtxSpriteClipped = new SimpleVertex[4];
			vtxSpriteClipped[0] = new SimpleVertex(0.0f, 0.0f, 0.0f, 0.0f);
			vtxSpriteClipped[1] = new SimpleVertex(0.0f, 0.0f, 1.0f, 0.0f);
			vtxSpriteClipped[2] = new SimpleVertex(0.0f, 0.0f, 0.0f, 1.0f);
			vtxSpriteClipped[3] = new SimpleVertex(0.0f, 0.0f, 1.0f, 1.0f);
			vtxTriangles = new SimpleVertex[4];
			vtxTriangles[0] = new SimpleVertex(0.0f, 0.0f, 0.0f, 0.0f);
			vtxTriangles[1] = new SimpleVertex(0.0f, 0.0f, 1.0f, 0.0f);
			vtxTriangles[2] = new SimpleVertex(0.0f, 0.0f, 0.0f, 1.0f);
			vtxTriangles[3] = new SimpleVertex(0.0f, 0.0f, 1.0f, 1.0f);
		}

		//----------
		//BASIC RENDERING

		public void clear(Color c) {
			Device.Clear(c);
		}

		public void RenderPoint(int x, int y) {
			//why +1?? gah 3d is so whacked
			//vtxSprite[0].x = x + 1;
			//vtxSprite[0].y = y + 1;
			//Device.DrawUserPrimitives(PrimitiveType., vtxSprite,0, 1);
			RenderRectangle(x, y, 1, 1);
		}

		//todo: experiment is it quicker to set constants and send the same sprite, or to change the sprite each time
		public void RenderRectangle(float x, float y, float width, float height) {
			vtxSprite[0].pos.X = x;
			vtxSprite[0].pos.Y = y;
			vtxSprite[1].pos.X = x + width;
			vtxSprite[1].pos.Y = y;
			vtxSprite[2].pos.X = x;
			vtxSprite[2].pos.Y = y + height;
			vtxSprite[3].pos.X = x + width;
			vtxSprite[3].pos.Y = y + height;
			Device.DrawUserPrimitives<SimpleVertex>(PrimitiveType.TriangleStrip, vtxSprite, 0, 2);
		}


		public void RenderRectangle(int x, int y, int width, int height) {
			vtxSprite[0].x = x;
			vtxSprite[0].y = y;
			vtxSprite[1].x = x + width;
			vtxSprite[1].y = y;
			vtxSprite[2].x = x;
			vtxSprite[2].y = y + height; 
			vtxSprite[3].x = x + width;
			vtxSprite[3].y = y + height;
			Device.DrawUserPrimitives<SimpleVertex>(PrimitiveType.TriangleStrip, vtxSprite, 0, 2);
		}

		public void RenderRectangleGradient(int x, int y, int width, int height, Color[] colors) {
			vtxSprite[0].col = colors[0];
			vtxSprite[1].col = colors[1];
			vtxSprite[2].col = colors[2];
			vtxSprite[3].col = colors[3];
			RenderRectangle(x, y, width, height);
		}

		public void RenderLine(int x0, int y0, int x1, int y1) { RenderLine(x0, y0, 0, x1, y1, 0); }
		public void RenderLine(float x0, float y0, float x1, float y1) { RenderLine(x0, y0, 0, x1, y1, 0); }
		public void RenderLine(float x0, float y0, float z0, float x1, float y1, float z1) {
			vtxSprite[0].x = x0;
			vtxSprite[0].y = y0;
			vtxSprite[0].z = z0;
			vtxSprite[1].x = x1;
			vtxSprite[1].y = y1;
			vtxSprite[1].z = z1;
			Device.DrawUserPrimitives<SimpleVertex>(PrimitiveType.LineList, vtxSprite, 0, 1);
			//the sprite stuff is optimized not to have to set z to 0 over and over--so restore it here
			vtxSprite[0].z = 0;
			vtxSprite[1].z = 0;
		}

		public void RenderTriangle(float x0, float y0, float x1, float y1, float x2, float y2) {
			vtxTriangles[0].x = x0;
			vtxTriangles[0].y = y0;
			vtxTriangles[1].x = x1;
			vtxTriangles[1].y = y1;
			vtxTriangles[2].x = x2;
			vtxTriangles[2].y = y2;
			Device.DrawUserPrimitives<SimpleVertex>(PrimitiveType.TriangleList, vtxTriangles, 0, 1);
		}

		public void RenderQuadrilateral(float x0, float y0, float x1, float y1, float x2, float y2, float x3, float y3) {
			vtxTriangles[0].x = x0;
			vtxTriangles[0].y = y0;
			vtxTriangles[1].x = x1;
			vtxTriangles[1].y = y1;
			vtxTriangles[2].x = x3;
			vtxTriangles[2].y = y3;
			vtxTriangles[3].x = x2;
			vtxTriangles[3].y = y2;
			Device.DrawUserPrimitives<SimpleVertex>(PrimitiveType.TriangleStrip, vtxTriangles, 0, 2);
		}

		public void RenderQuadrilateral(int x0, int y0, int x1, int y1, int x2, int y2, int x3, int y3) {
			RenderQuadrilateral((float)x0, y0, x1, y1, x2, y2, x3, y3);
		}

		public void RenderRectangleClipped(float x, float y, float sx, float sy, float w, float h, float tw, float th) {
			RenderRectangleClipped(x, y, sx, sy, w, h, tw, th, w, h);
		}
		public void RenderRectangleClipped(int x, int y, int sx, int sy, int w, int h, int tw, int th) {
			RenderRectangleClipped(x, y, sx, sy, w, h, tw, th, w, h);
		}
		public void RenderRectangleClipped(float x, float y, float sx, float sy, float w, float h, float tw, float th, float dw, float dh) {
			vtxSpriteClipped[0].x = x;
			vtxSpriteClipped[0].y = y;
			vtxSpriteClipped[0].tu = sx;
			vtxSpriteClipped[0].tv = sy;
			vtxSpriteClipped[1].x = x + dw;
			vtxSpriteClipped[1].y = y;
			vtxSpriteClipped[1].tu = sx + w;
			vtxSpriteClipped[1].tv = sy;
			vtxSpriteClipped[2].x = x;
			vtxSpriteClipped[2].y = y + dh;
			vtxSpriteClipped[2].tu = sx;
			vtxSpriteClipped[2].tv = sy + h;
			vtxSpriteClipped[3].x = x + dw;
			vtxSpriteClipped[3].y = y + dh;
			vtxSpriteClipped[3].tu = sx + w;
			vtxSpriteClipped[3].tv = sy + h;

			vtxSpriteClipped[0].tu /= tw;
			vtxSpriteClipped[0].tv /= th;
			vtxSpriteClipped[1].tu /= tw;
			vtxSpriteClipped[1].tv /= th;
			vtxSpriteClipped[2].tu /= tw;
			vtxSpriteClipped[2].tv /= th;
			vtxSpriteClipped[3].tu /= tw;
			vtxSpriteClipped[3].tv /= th;

			Device.DrawUserPrimitives(PrimitiveType.TriangleStrip, vtxSpriteClipped,0,2);
		}

		//--------------------------------------------------------
		//BATCHING

		//is it wise to keep these allocated indefinitely? not in general, 
		//but in this case, probably given the use case

		SimpleVertex[] _vtxRectClipped = new SimpleVertex[96];
		int _vtxRectClippedCount;
		public void BatchRectangleClipped(int x, int y, int sx, int sy, int w, int h, int dw, int dh, int tw, int th) {
			if(_vtxRectClippedCount == _vtxRectClipped.Length)
				_vtxRectClipped = pr2.Common.Lib.expandArray(_vtxRectClipped);

			float tul = sx / (float)tw;
			float tur = (sx + w) / (float)tw;
			float tvt = sy / (float)th;
			float tvb = (sy + h) / (float)th;

			_vtxRectClipped[_vtxRectClippedCount + 0].x = x;
			_vtxRectClipped[_vtxRectClippedCount + 0].y = y;
			_vtxRectClipped[_vtxRectClippedCount + 0].tu = tul;
			_vtxRectClipped[_vtxRectClippedCount + 0].tv = tvt;
			_vtxRectClipped[_vtxRectClippedCount + 1].x = x + dw;
			_vtxRectClipped[_vtxRectClippedCount + 1].y = y;
			_vtxRectClipped[_vtxRectClippedCount + 1].tu = tur;
			_vtxRectClipped[_vtxRectClippedCount + 1].tv = tvt;
			_vtxRectClipped[_vtxRectClippedCount + 2].x = x + dw;
			_vtxRectClipped[_vtxRectClippedCount + 2].y = y + dh;
			_vtxRectClipped[_vtxRectClippedCount + 2].tu = tur;
			_vtxRectClipped[_vtxRectClippedCount + 2].tv = tvb;

			_vtxRectClipped[_vtxRectClippedCount + 3] = _vtxRectClipped[_vtxRectClippedCount + 2];

			_vtxRectClipped[_vtxRectClippedCount + 4].x = x;
			_vtxRectClipped[_vtxRectClippedCount + 4].y = y + dh;
			_vtxRectClipped[_vtxRectClippedCount + 4].tu = tul;
			_vtxRectClipped[_vtxRectClippedCount + 4].tv = tvb;

			_vtxRectClipped[_vtxRectClippedCount + 5] = _vtxRectClipped[_vtxRectClippedCount + 0];

			_vtxRectClippedCount += 6;
		}

		public void BatchRectangle(float x, float y, float sw, float sh)
		{
			if (_vtxRectClippedCount == _vtxRectClipped.Length)
				_vtxRectClipped = pr2.Common.Lib.expandArray(_vtxRectClipped);


			_vtxRectClipped[_vtxRectClippedCount + 0] = vtxSprite[0];
			_vtxRectClipped[_vtxRectClippedCount + 1] = vtxSprite[1];
			_vtxRectClipped[_vtxRectClippedCount + 2] = vtxSprite[2];
			_vtxRectClipped[_vtxRectClippedCount + 4] = vtxSprite[3];

			_vtxRectClipped[_vtxRectClippedCount + 0].pos.X = x;
			_vtxRectClipped[_vtxRectClippedCount + 0].pos.Y = y;
			_vtxRectClipped[_vtxRectClippedCount + 1].pos.X = x + sw;
			_vtxRectClipped[_vtxRectClippedCount + 1].pos.Y = y;
			_vtxRectClipped[_vtxRectClippedCount + 2].pos.X = x;
			_vtxRectClipped[_vtxRectClippedCount + 2].pos.Y = y + sh;
			
			_vtxRectClipped[_vtxRectClippedCount + 3] = _vtxRectClipped[_vtxRectClippedCount + 2];

			_vtxRectClipped[_vtxRectClippedCount + 4].pos.X = x + sw;
			_vtxRectClipped[_vtxRectClippedCount + 4].pos.Y = y + sh;

			_vtxRectClipped[_vtxRectClippedCount + 5] = _vtxRectClipped[_vtxRectClippedCount + 1];

			_vtxRectClippedCount += 6;
		}

		public void BatchRectangleClipped(int x, int y, int sx, int sy, int w, int h, int tw, int th) {
			BatchRectangleClipped(x, y, sx, sy, w, h, w, h, tw, th);
		}

		//int tw, th;
		public void BatchExecuteRectangleClipped() {
			if(_vtxRectClippedCount == 0) return;
			Device.DrawUserPrimitives(PrimitiveType.TriangleList, _vtxRectClipped, 0, _vtxRectClippedCount / 3);
		}

		public void ClearBatches() {
			_vtxRectClippedCount = 0;
		}

		//--------------------------------------------------------

	}
}
