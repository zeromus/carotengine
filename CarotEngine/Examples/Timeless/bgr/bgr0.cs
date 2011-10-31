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

		void InitializeBGR0()
		{
			bgrtick = 0;
			bgrtoggle = 0;
			bgrproc = RenderBGR0;
		}

		void RenderBGR0()
		{
			/*	if (bgrtoggle < 1)
				{
					bgrtoggle++;
					return;
				}
				bgrtoggle-=1;*/

			if (bgrtick > 600)
			{
				bgrproc = null;
				return;
			}

			int z = Random(40, 255);
			Blitter b = new Blitter(bg);
			b.Color = MakeColor(z, z, z);
			b.SetPixel(Random(0, 255), Random(0, 255));
			bgrtick++;

		}
	}
}
