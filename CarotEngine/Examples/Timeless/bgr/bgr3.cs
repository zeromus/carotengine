using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using pr2.CarotEngine;
using pr2.Common;

namespace Timeless {
	partial class Timeless {

		void InitializeBGR3()
		{
			bgrtick = 0;
			bgrtoggle = 0;
			bgrproc = () => RenderBGR3();
		}

		void RenderBGR3()
		{
			if (bgrtoggle < 1)
			{
				bgrtoggle++;
				return;
			}
			bgrtoggle -= 1;

			if (bgrtick > 200)
			{
				bgrproc = null;
				return;
			}

			Blitter b = new Blitter(bg);
			b.Blit(bg2, 0, 0);
			b.Alpha = (bgrtick / 2)/100.0f;
			b.RectFill(0, 0, 256, 256);
			bg.Cache();

			bgrtick++;
		}
		
	}
}
