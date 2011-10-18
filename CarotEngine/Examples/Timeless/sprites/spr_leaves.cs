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
		const int NUM_LEAVES = 35;

		struct myleaf
		{
			public int x, y;
			public int angle;
			public int starttime, endtime;
		}
		myleaf[] leaves = new myleaf[NUM_LEAVES];

		anim_t lspr;


		void InitializeLeaves(int i)
		{
			switch (i)
			{
				case 0: lspr = leaf; break;
				case 1: lspr = leaf2; break;
				case 2: lspr = heart; break;
			}

			for (i = 0; i < NUM_LEAVES; i++)
			{
				leaves[i].x = Random(0, 320) * 16;
				leaves[i].y = Random(0, 200) * 16;
				leaves[i].angle = Random(0, 359);
				leaves[i].starttime = systemtime + Random(0, 150);
				leaves[i].endtime = systemtime + Random(650, 800);
			}

			spr_state = SPR_LEAVES;
			spr_die = systemtime + 800;
			spritetimer = 0;
		}
		void RenderLeaves(Blitter b)
		{
			if (systemtime >= spr_die)
			{
				spr_state = 0;
				return;
			}

			int i;
			while (spritetimer != 0)
			{
				spritetimer--;
				for (i = 0; i < NUM_LEAVES; i++)
				{
					switch (leaftbl[(leaves[i].angle + systemtime) % 140 / 7])
					{
						case 0: leaves[i].x -= 4; leaves[i].y += 4; break;
						case 1: leaves[i].x -= 6; leaves[i].y += 8; break;
						case 2: leaves[i].x -= 8; leaves[i].y += 10; break;
						case 3: leaves[i].x -= 8; leaves[i].y += 12; break;
						case 4: leaves[i].x -= 16; leaves[i].y += 8; break;
					}
					if (leaves[i].x / 16 < 0 - 14) leaves[i].x = 334 * 16;
					if (leaves[i].y / 16 > 214) leaves[i].y = 0 - 14 * 16;
				}
			}

			for (i = 0; i < NUM_LEAVES; i++)
			{
				if (systemtime > leaves[i].starttime && systemtime < leaves[i].endtime)
					BlitFrameAt(leaves[i].x / 16, ybase + (leaves[i].y / 16), lspr, leaftbl[(leaves[i].angle + systemtime) % 140 / 7], b);
			}
		}
	}
}
