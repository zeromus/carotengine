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

		Image bgr11spr;
		void InitializeBGR11(int r)
		{
			bgrtick = 0;
			bgrtoggle = 0;
			bgrproc = RenderBGR11;

			switch (r)
			{
				case 0: bgr11spr = bobblue; break;
				case 1: bgr11spr = bobred; break;
				case 2: bgr11spr = bobgreen; break;
				case 3: bgr11spr = bobpurple; break;
			}
		}

		void RenderBGR11()
		{
			if (bgrtoggle < 1)
			{
				bgrtoggle++;
				return;
			}
			bgrtoggle -= 1;

			if (bgrtick > 512)
			{
				bgrproc = null;
				return;
			}
			int x, y;
			Blitter b = new Blitter(bg);
			if (bgrtick < 256)
			{
				x = 50 + (sin(bgrtick * 1536 / 360) * 30 / 65535);
				y = bgrtick;
				SetLucent(b, 85);
				BlitWrap(b, bgr11spr, x, y);
				SetLucent(b, 0);
			}
			else if (bgrtick < 512)
			{
				x = 178 + (sin(bgrtick * 1536 / 360) * 30 / 65535);
				y = 256 - (bgrtick - 256);
				SetLucent(b, 85);
				BlitWrap(b, bgr11spr, x, y);
				SetLucent(b, 0);
			}
			bgrtick++;
		}
	}
}
