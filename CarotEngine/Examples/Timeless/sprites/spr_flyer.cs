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
		anim_t flyer_sprite;
		int fly_x, fly_y;
		int fly_dir;

		void InitializeFlyer(int r)
		{
			switch (r)
			{
				case 0: flyer_sprite = flyeye; break;
				case 1: flyer_sprite = flymask; break;
				case 2: flyer_sprite = flyskull; break;
				case 3: flyer_sprite = flyheart; break;
				case 4: flyer_sprite = flyeyeg; break;
			}

			r = Random(0, 1);
			switch (r)
			{
				case 0: fly_dir = 0 - 1; fly_y = 220; break;
				case 1: fly_dir = 1; fly_y = 0 - 20; break;
			}

			fly_x = Random(50, 270);
			spr_state = SPR_FLY;
			spr_die = systemtime + 1000;
			spritetimer = 0;
		}

		void RenderFlyer(Blitter b)
		{
			if (systemtime >= spr_die)
			{
				spr_state = 0;
				return;
			}

			int my = fly_y + ((spritetimer / 4) * fly_dir);
			BlitFrameAt(fly_x + (sin(systemtime / 2) * 60 / 65535), ybase + my, flyer_sprite, pingpong3[systemtime % 28 / 7], b);
		}


	}
}
