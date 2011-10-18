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
		const int NUM_NODES = 65;

		struct wnode
		{
			public int x, y;
			public int layer;
			public int starttime, endtime;
		}
		wnode[] wnodes = new wnode[NUM_NODES];

		anim_t wspr;


		void InitializeWeather(int i)
		{
			switch (i)
			{
				case 0: wspr = rain; break;
				case 1: wspr = snow; break;
			}

			for (i = 0; i < NUM_NODES; i++)
			{
				wnodes[i].x = Random(0, 320);
				wnodes[i].y = Random(0, 200);
				wnodes[i].layer = Random(0, 2);
				wnodes[i].starttime = systemtime + Random(0, 100);
				wnodes[i].endtime = systemtime + Random(700, 800);
			}

			spr_state = SPR_WEATHER;
			spr_die = systemtime + 800;
			spritetimer = 0;
		}

		void RenderWeather(Blitter b)
		{
			if (systemtime >= spr_die)
			{
				spr_state = 0;
				return;
			}

			int i, j;
			while (spritetimer > 1)
			{
				spritetimer -= 2;
				for (i = 0; i < NUM_NODES; i++)
				{
					for (j = 0; j <= wnodes[i].layer; j++)
					{
						wnodes[i].x--;
						wnodes[i].y++;
					}
					if (wnodes[i].x < 0 - 14) wnodes[i].x = 334;
					if (wnodes[i].y > 214) wnodes[i].y = 0 - 14;
				}
			}

			for (i = 0; i < NUM_NODES; i++)
			{
				if (systemtime > wnodes[i].starttime && systemtime < wnodes[i].endtime)
					BlitFrameAt(wnodes[i].x, ybase + wnodes[i].y, wspr, wnodes[i].layer, b);
			}
		}
	}
}
