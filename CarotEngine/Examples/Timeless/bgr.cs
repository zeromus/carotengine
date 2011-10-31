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

		Action bgrproc = null;
		int bgrstart;
		int bgrtick, bgrtoggle, bgrdie;
		//int colormixer = NewImage(1, 1);

		Color ColorMorph(Color col1, Color col2, int step, int totalsteps)
		{
			float factor = step / (float)totalsteps;
			var vcol1 = col1.ToVector4();
			var vcol2 = col2.ToVector4();
			var result = vcol1 * (1-factor) + vcol2 * factor;
			result.W = 1;
			return new Color(result);

			//SetPixel(0, 0, col1, colormixer);
			//SetLucent(100 - (step * 100 / totalsteps));
			//SetPixel(0, 0, col2, colormixer);
			//SetLucent(0);
			//return GetPixel(0, 0, colormixer);
		}
		
	}
}
