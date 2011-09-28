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
	partial class Timeless {

		Color tint_color;
		Color[] tcolors = new Color[4];

		/*************************** code ***************************/

		void InitializeTintTable()
		{
			tcolors[0] = MakeColor(119, 243, 131);
			tcolors[1] = MakeColor(206, 0, 242);
			tcolors[2] = MakeColor(128, 112, 243);
			tcolors[3] = MakeColor(255, 120, 0);	
		}

		void InitializeTintLens(int z)
		{
			lens_effect = LE_TINT;
			tint_color = tcolors[z];
		}

		void RenderTintLens()
		{
			//todo
			//SetLucent(50);
			//CircleFill(lens_x, ybase+lens_y, 24, 20, tint_color, screen);
			//SetLucent(0);
			//Circle(lens_x, ybase+lens_y, 24, 20, 0, screen);
		}

	}
}