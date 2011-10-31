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

		Color bgr8_col1, bgr8_col2, bgr8_col3;

		void InitializeBGR8(int r)
		{
			bgrtick = 0;
			bgrtoggle = 0;
			bgrproc = RenderBGR8;

			bgr8_col1 = MakeColor(255, 255, 255);
			bgr8_col3 = MakeColor(0, 0, 0);

			switch (r)
			{
				case 0: bgr8_col2 = MakeColor(0, 255, 0); break;
				case 1: bgr8_col2 = MakeColor(182, 0, 182); break;//  bgr8_col1=RGB(252,102,252); bgr8_col2=RGB(150,0,150);
				case 2: bgr8_col2 = MakeColor(0, 0, 255); break;//   bgr8_col1=RGB(116,221,255); bgr8_col2=RGB(0,158,210); bgr8_col3=RGB(0,87,116);
				case 3: bgr8_col2 = MakeColor(255, 0, 0); break;//  bgr8_col1=RGB(255,229,83);  bgr8_col2=RGB(255,132,0);
				case 4: bgr8_col2 = MakeColor(255, 192, 0); break;
			}
		}

		void RenderBGR8()
		{
			if (bgrtoggle < 2)
			{
				bgrtoggle++;
				return;
			}
			bgrtoggle -= 2;

			if (bgrtick > 128)
			{
				bgrproc = null;
				return;
			}


			Blitter b = new Blitter(bg);
			if (bgrtick < 64)
			{
				b.Color = ColorMorph(bgr8_col1, bgr8_col2, bgrtick, 64);
				b.Line(0, 128 - bgrtick, 255, 128 - bgrtick);
				b.Color = ColorMorph(bgr8_col1, bgr8_col2, bgrtick, 64);
				b.Line(0, 128 + bgrtick, 255, 128 + bgrtick);
			}
			else
			{
				b.Color = ColorMorph(bgr8_col2, bgr8_col3, bgrtick - 64, 64);
				b.Line(0, 128 - bgrtick, 255, 128 - bgrtick);
				b.Color = ColorMorph(bgr8_col2, bgr8_col3, bgrtick - 64, 64);
				b.Line(0, 128 + bgrtick, 255, 128 + bgrtick);
			}

			bgrtick++;
		}
	}
}
