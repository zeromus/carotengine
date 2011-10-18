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

		Image dumb = null;

		struct cnode
		{
			public int angle;
			public int frame;
			public int nextframe;
		}

		cnode[] candles = new cnode[4];
		int cx;
		int ctick;
		int clangle;

		void InitializeCandle()
		{
			if (dumb == null) dumb = NewImage(1, 1);
			spr_state = SPR_CANDLE;
			spr_die = systemtime + 800;

			candles[0].angle = 0;
			candles[0].frame = Random(0, 9);
			candles[1].angle = 90;
			candles[1].frame = Random(0, 9);
			candles[2].angle = 180;
			candles[2].frame = Random(0, 9);
			candles[3].angle = 270;
			candles[3].frame = Random(0, 9);
			candles[0].nextframe = systemtime + Random(8, 12);
			candles[1].nextframe = systemtime + Random(8, 12);
			candles[2].nextframe = systemtime + Random(8, 12);
			candles[3].nextframe = systemtime + Random(8, 12);
			cx = 350;
			ctick = 0;
			clangle = 0;
			spritetimer = 0;
		}


		void RenderCandle(Blitter b)
		{
			if (systemtime >= spr_die)
			{
				spr_state = 0;
				return;
			}

			while (spritetimer != 0)
			{
				spritetimer--;
				ctick++;
			}
			cx = 350 - (ctick / 2);

			clangle += 16;

			int i, x, y, haloframe;
			for (i = 0; i < 4; i++)
			{
				if (systemtime > candles[i].nextframe)
				{
					candles[i].nextframe = systemtime + Random(8, 20);
					candles[i].frame = Random(0, 9);
				}

				x = cx + (sin(candles[i].angle + (clangle / 12)) * 30 / 65535);
				y = ybase + 100 + (sin(systemtime) * 30 / 65535) + (cos(candles[i].angle + (clangle / 12)) * 30 / 65535);
				BlitAt(x, y, candle, b);
				BlitFrameAt(x - 1, y - 14, flame, candles[i].frame, b);
				haloframe = candles[i].frame;
				if (haloframe > 4)
					haloframe -= 5;
				//BlitFrame(0, 0, halo, haloframe, dumb); //wtf?
				//TAdditiveBlit(x - 1 - (144 / 2), y - 14 - (144 / 2), anims[halo].bufimage, screen);
				//TODO additive wtf?
			}
		}
	}
}
