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
		int scripttimer;
		int nextscripttime;

		int bgr_rstate = 2;     // start 
		int bgr_rtime;


		void CheckScript()
		{
			int r;

			if (spr_state == 0)
			{
				r = Random(1, 10);
				switch (r)
				{
					case 1: InitializeSlug(Random(0, 1)); break;
					case 2: InitializeFlyer(Random(0, 4)); break;
					case 3: InitializeFlower(); break;
					case 4: InitializeCelestial(Random(0, 2)); break;
					case 5: InitializeCircular(Random(0, 5)); break;
					case 6: InitializeFish(Random(0, 1)); break;
					case 7: InitializeSmilies(Random(0, 1)); break;
					case 8: InitializeLeaves(Random(0, 2)); break;
					case 9: InitializeWeather(Random(0, 1)); break;
					case 10: InitializeCandle(); break;

				}
			}

			//if (lens_state = 0)
			//{
			//    r = Random(0,2);
			//    switch (r)
			//    {
			//        case 0: InitializeVertMove();
			//        case 1: InitializeHorizMove();
			//        case 2: InitializeBounceMove();
			//    }

			//    r = Random(0, 4);
			//    switch (r)
			//    {
			//        case 0: InitializeTintLens(Random(0,3));
			//        case 1: InitializeBeneathLens();
			//        case 2: InitializeScaleLens();
			//        case 3: InitializeGreyLens(Random(1,3));
			//        case 4: InitializeMosaicLens();
			//    }
			//}

			if (bgrproc == null)
			{
				if (bgr_rtime < 0)
				{
					bgr_rtime = systemtime + 200;
					return;
				}
				if (systemtime < bgr_rtime)
					return;

				//HACK
				bgr_rstate = 0;

				bgr_rtime = 0 - 1;
				switch (bgr_rstate)
				{
					case 0:     // base level effects
						r = Random(0, 7);
						//switch (r)
						//{
						//    case 0: 
						//InitializeBGR8(Random(0, 4)); bgr_rstate = 1;
						//    case 1: 
						//InitializeBGR5(Random(0, 8)); bgr_rstate = 4;
						//    case 2: 
						//			InitializeBGR6(Random(0, 8)); bgr_rstate = 4;
						//    case 3: 
						//			InitializeBGR12(Random(0, 3)); bgr_rstate = 1;
						//    case 4: InitializeBGR10(Random(0, 7)); bgr_rstate = 1;
						//            InitializeBGR7();
						//    case 5: InitializeBGR10(Random(0, 7)); bgr_rstate = 1;
						//            InitializeBGR9();
						//    case 6: InitializeBGR2(); bgr_rstate = 2; bgr_rtime = 1;
						//    case 7: InitializeBGR2(); bgr_rstate = 2; bgr_rtime = 1;
						//}	
						break;

					case 1:     // top level/modifier effects				
						//r = Random(0, 4);
						//switch (r)
						//{
						//    case 0: bgr_rstate = 0;
						//    case 1: InitializeBGR13(Random(0,4)); bgr_rstate = 0;
						//    case 2: InitializeBGR11(Random(0,3)); bgr_rstate = 0;
						//    case 3: InitializeBGR15(); bgr_rstate = 0;
						//    case 4: bgr_rstate = 0;
						//}			
						break;
					case 2:		// Black screen effects				
						r = Random(0, 3);
						switch (r)
						{
						  case 0: InitializeBGR4(Random(0, 6)); bgr_rstate = 1; break;
						  case 1: InitializeBGR4(Random(0, 6)); bgr_rstate = 1; break;
							case 2: InitializeBGR11(Random(0, 3)); bgr_rstate = 0; break;
							case 3: InitializeBGR0(); bgr_rstate = 3; bgr_rtime = 1; break;
						}
						break;
					case 3:		// old-school! original timeless intro sequence
					//InitializeBGR1();
					//bgr_rstate = 0;			
					//break;
					case 4: 	// top level/modifier effects, except for shadebobs
						//r = random(0, 2);
						//switch (r)
						//{
						//    case 0: bgr_rstate = 0;
						//    case 1: InitializeBGR13(Random(0,4)); bgr_rstate = 0;
						//    case 2: InitializeBGR4(Random(3,6)); bgr_rstate = 0;
						//}
						break;
				}
			}
		}
	}
}
