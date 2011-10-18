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
		int flw_x, flw_y;
		int flxdir, flydir;

		void InitializeFlower()
		{
			int z = Random(0, 3);
			switch (z)
			{
				case 0: flw_y = 21; flw_x = 0 - 42; flxdir = 1; flydir = 0; break;
				case 1: flw_y = 179; flw_x = 362; flxdir = 0 - 1; flydir = 0; break;
				case 2: flw_y = 0 - 42; flw_x = 299; flxdir = 0; flydir = 1; break;
				case 3: flw_y = 240; flw_x = 21; flxdir = 0; flydir = 0 - 1; break;
			}
			spr_state = SPR_FLOWER;
			spr_die = systemtime + 900;
			spritetimer = 0;
		}

		void RenderFlower(Blitter b)
		{
			if (systemtime >= spr_die)
			{
				spr_state = 0;
				return;
			}

			while (spritetimer > 1)
			{
				spritetimer -= 2;
				flw_x += flxdir;
				flw_y += flydir;
			}

			b.RotScale(flower, flw_x, ybase + flw_y, Lib.Rads(systemtime * 4 / 3), 1);
		}


	}
}
