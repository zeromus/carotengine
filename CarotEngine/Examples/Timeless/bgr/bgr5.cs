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

namespace Timeless
{
	partial class Timeless
	{

		Image bgr5img;

		void InitializeBGR5(int i)
		{
			bgrtick = 0;
			bgrtoggle = 0;
			bgrproc = RenderBGR5;

			switch (i)
			{
				case 0: bgr5img = tiledbg0; break;
				case 1: bgr5img = tiledbg1; break;
				case 2: bgr5img = tiledbg2; break;
				case 3: bgr5img = tiledbg3; break;
				case 4: bgr5img = tiledbg4; break;
				case 5: bgr5img = tiledbg5; break;
				case 6: bgr5img = tiledbg6; break;
				case 7: bgr5img = tiledbg7; break;
				case 8: bgr5img = tiledbg8; break;
				case 9: bgr5img = tiledbg9; break;
			}
		}


		void RenderBGR5()
		{
			if (bgrtoggle < 4)
			{
				bgrtoggle++;
				return;
			}
			bgrtoggle -= 4;

			if (bgrtick > 63)
			{
				bgrproc = null;
				return;
			}

			int y = bgrtick / 8;
			int x = bgrtick % 8;
			Blitter b = new Blitter(bg);
			b.Blit(bgr5img, x * 32, y * 32);
			bgrtick++;
		}
	}
}
