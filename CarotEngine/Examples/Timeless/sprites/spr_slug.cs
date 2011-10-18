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
		int slug_x;
		int slug_y;
		anim_t slug_spr;

		void InitializeSlug(int r)
		{
			slug_x = Random(10, 310);
			slug_y = 220;
			spr_state = SPR_SLUG;
			spr_die = systemtime + 600;
			spritetimer = 0;
			switch (r)
			{
				case 0: slug_spr = sluggy; break;
				case 1: slug_spr = sluggy2; break;
			}
		}

		void RenderSlug(Blitter b)
		{
			if (systemtime >= spr_die)
			{
				spr_state = 0;
				return;
			}

			slug_y = 220 - (spritetimer / 2);
			BlitFrameAt(slug_x, ybase + slug_y, slug_spr, pingpong4[systemtime % 42 / 7], b);
		}

	}
}
