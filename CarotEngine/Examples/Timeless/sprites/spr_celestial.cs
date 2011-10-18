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
	partial class Timeless
	{
		Image clst_sprite;
		int clst_x, clst_y;

		void InitializeCelestial(int r)
		{
			switch (r)
			{
				case 0: clst_sprite = sun; break;
				case 1: clst_sprite = moon; break;
				case 2: clst_sprite = planet; break;
			}

			clst_x = 340;
			clst_y = Random(60, 140);
			spr_state = SPR_CELESTIAL;
			spr_die = systemtime + 1000;
			spritetimer = 0;
		}

		void RenderCelestial(Blitter b)
		{
			if (systemtime >= spr_die)
			{
				spr_state = 0;
				return;
			}

			while (spritetimer > 1)
			{
				spritetimer -= 2;
				clst_x--;
			}

			BlitAt(clst_x, ybase + clst_y + (sin(systemtime / 3) * 15 / 65535), clst_sprite, b);
		}


	}
}
