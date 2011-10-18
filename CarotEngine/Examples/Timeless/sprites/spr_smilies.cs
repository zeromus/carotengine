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
		const int NUM_SMILIES = 35;

		struct smilie
		{
			public int x, y;
		}
		smilie[] smilies = new smilie[NUM_SMILIES];
		int xmin;
		Image smilespr;


		void InitializeSmilies(int t)
		{
			int i;
			for (i = 0; i < NUM_SMILIES; i++)
			{
				smilies[i].x = Random(0 - 200, 0);
				smilies[i].y = Random(10, 190);
			}

			switch (t)
			{
				case 0: smilespr = smile; break;
				case 1: smilespr = smile2; break;
			}

			xmin = 0 - 200;
			spr_state = SPR_SMILIES;
			spr_die = systemtime + 500;
			spritetimer = 0;
		}

		void RenderSmilies(Blitter b)
		{
			if (systemtime >= spr_die)
			{
				spr_state = 0;
				return;
			}

			while (spritetimer != 0)
			{
				spritetimer--;
				xmin += 2;
			}

			int i;
			for (i = 0; i < NUM_SMILIES; i++)
			{
				if (smilies[i].x < xmin)
					smilies[i].x += 200;
				BlitAt(smilies[i].x, ybase + smilies[i].y, smilespr, b);
			}
		}
	}
}
