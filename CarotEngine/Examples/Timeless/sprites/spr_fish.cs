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
		const int NUM_FISHIES = 10;

		struct fishie
		{
			public int x, y;
			public int angleofs;
		}

		fishie[] fishies = new fishie[NUM_FISHIES];
		anim_t fishspr;


		void InitializeFish(int r)
		{
			int i;
			for (i = 0; i < NUM_FISHIES; i++)
			{
				fishies[i].x = Random(330, 630);
				fishies[i].y = Random(50, 140);
				fishies[i].angleofs = Random(0, 359);
			}

			switch (r)
			{
				case 0: fishspr = fish; break;
				case 1: fishspr = fish2; break;
			}

			spr_state = SPR_FISH;
			spr_die = systemtime + 1400;
			spritetimer = 0;
		}

		void RenderFish(Blitter b)
		{
			if (systemtime >= spr_die)
			{
				spr_state = 0;
				return;
			}

			int i, xmod = 0 - (spritetimer / 2);
			for (i = 0; i < NUM_FISHIES; i++)
				BlitFrameAt(fishies[i].x + xmod, ybase + fishies[i].y + (sin(fishies[i].angleofs + systemtime) * 10 / 65535), fishspr, (fishies[i].angleofs + systemtime) % 20 / 10, b);
		}

	}
}
