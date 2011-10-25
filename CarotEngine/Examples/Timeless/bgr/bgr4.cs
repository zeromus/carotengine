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

		void InitializeBGR4(int f)
		{
			Image myimg = null;

			Blitter b = new Blitter(bg2);
			b.Clear(Color.Black);

			switch (f)
			{
				case 0: myimg = facered; break;
				case 1: myimg = faceblue; break;
				case 2: myimg = facegreen; break;
				case 3: myimg = sphereblue; break;
				case 4: myimg = spheregreen; break;
				case 5: myimg = sphereorange; break;
				case 6: myimg = spherepurple; break;
			}

			myimg = spherepurple;

			for (int i = 0; i < 30; i++)
			{
				f = Random(0, 2);
				int dw = -1, dh = -1;
				switch (f)
				{
					case 0: dw = 64; dh = 64; break;
					case 1: dw = 32; dh = 32; break;
					case 2: dw = 16; dh = 16; break;
				}
				int x=Random(0, 255), y=Random(0, 255);
				ScaleWrapBlit(x,y, myimg, dw, dh, bg2);
			}
			InitializeBGR3();
			bg2.Cache();
		}
		
	}
}
