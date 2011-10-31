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
		void InitializeBGR7()
		{
			bgrtick = 0;
			bgrtoggle = 0;
			//???? TODO
			//SetClip(0, 0, 255, 255, bg);
			//SetClip(0, 0, 255, 255, bg2);
			//SetClip(0, 0, 255, 255, bg3);
			Blitter b = new Blitter(bg3);
			b.Blit(bg, 0, 0);
			bgrproc = RenderBGR7;
		}

		void RenderBGR7()
		{
			if (bgrtoggle < 5)
			{
				bgrtoggle++;
				return;
			}
			bgrtoggle -= 5;

			if (bgrtick > 100)
			{
				bgrproc = null;
				return;
			}

			Blitter b = new Blitter(bg);
			b.Blit(bg3, 0, 0);
			SetLucent(b,100 - bgrtick);
			b.Blit(bg2, 0, 0);
			bgrtick++;
		}
	}
}
