using System;
using System.Collections.Generic;
using System.Text;

namespace pr2.CarotEngine {
	public class TileEngineCamera {
		public Entity EntityFollowed = null;

		int w, h;

		public TileEngineCamera(int w, int h) {
			this.w = w; this.h = h;
		}
		public void Attach(Entity entity) {
			EntityFollowed = entity;
		}

		float x = 0;
		float y = 0;

		public void Tick() {
			if(EntityFollowed != null) {
				int sw = 320;
				int sh = 240;

				int mw = w * 16;
				int mh = h * 16;

				int ex = (int)EntityFollowed.x;
				int ey = (int)EntityFollowed.y;

				ex -= sw / 2;
				ey -= sh / 2;

				if(ex < 0) ex = 0;
				if(ey < 0) ey = 0;

				if(ex > mw - sw) ex = mw - sw;
				if(ey > mh - sh) ey = mh - sh;

				x = ex;
				y = ey;
			}
		}

		public float X { get { return x; } set { x = value; } }
		public float Y { get { return y; } set { y = value; } }

	}
}
