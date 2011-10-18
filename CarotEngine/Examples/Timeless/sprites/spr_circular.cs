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
		int crc_xc, crc_yc;
		int crc_xr, crc_yr;
		int crc_dir;
		Image crc_sprite;
		int[] cangle = new int[8];


		void InitializeCircular(int r)
		{
			switch (r)
			{
				case 0: crc_sprite = bigheart; break;
				case 1: crc_sprite = bigsmile; break;
				case 2: crc_sprite = evilmask; break;
				case 3: crc_sprite = hourglass; break;
				case 4: crc_sprite = skull; break;
				case 5: crc_sprite = tinyeye; break;
			}

			r = Random(0, 1);
			switch (r)
			{
				case 0: crc_dir = 1; crc_xc = 0 - 60; break;
				case 1: crc_dir = 0 - 1; crc_xc = 380; break;
			}

			crc_yc = Random(80, 120);
			crc_xr = 40;
			crc_yr = 35;

			cangle[0] = 0;
			cangle[1] = 45;
			cangle[2] = 90;
			cangle[3] = 135;
			cangle[4] = 180;
			cangle[5] = 225;
			cangle[6] = 270;
			cangle[7] = 315;

			spr_state = SPR_CIRCULAR;
			spr_die = systemtime + 900;
			spritetimer = 0;
		}


		void RenderCircular(Blitter b)
		{
			if (systemtime >= spr_die)
			{
				spr_state = 0;
				return;
			}

			while (spritetimer > 1)
			{
				spritetimer -= 2;
				crc_xc += crc_dir;
			}

			int i, mx, my;
			int rx, ry;

			rx = crc_xr + (sin(systemtime) * 10 / 65535);
			ry = crc_yr + (cos(systemtime) * 15 / 65535);

			for (i = 0; i < 8; i++)
			{
				mx = sin(cangle[i] + systemtime) * rx / 65535;
				my = cos(cangle[i] + systemtime) * ry / 65535;
				BlitAt(crc_xc + mx, ybase + crc_yc + my, crc_sprite, b);
			}
		}

	}
}
