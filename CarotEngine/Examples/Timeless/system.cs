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

		// Configuration variables

		int ybase, vidmode;

		// Global background/effect vars

		Image bg, bg2, bg3;

		// scrollspeed state variables

		int scrollofs;
		int scrollspeed = 8, targetspeed = 8;
		int scrollpath, nextscroll;

		// Skew state variables

		int skewofs;
		int skewpath, nextskew;
		int skewlinesleft;

		// Timers

		int skewtimer, scrolltimer, bgrtimer, spritetimer, lenstimer;

		//--------------------------------------------------------------
		//here was autoexec
		//--------------------------------------------------------------



		void CheckscrollPath()
		{
			if (systemtime > nextscroll)
			{
				if (scrollpath == 0) { scrollpath = 1; nextscroll = systemtime + 1100; targetspeed = 0 - 20; return; }
				if (scrollpath == 1) { scrollpath = 2; nextscroll = systemtime + 1300; targetspeed = 16; return; }
				if (scrollpath == 2) { scrollpath = 3; nextscroll = systemtime + 1100; targetspeed = 0 - 20; return; }
				if (scrollpath == 3) { scrollpath = 0; nextscroll = systemtime + 1300; targetspeed = 8; return; }
			}
		}

		void CalculateScroll()
		{
			CheckscrollPath();

			while (scrolltimer != 0)
			{
				if (scrollspeed < targetspeed) scrollspeed++;
				if (scrollspeed > targetspeed) scrollspeed--;
				scrollofs += scrollspeed;
				scrolltimer--;
			}
		}

		void CheckSkewPath()
		{
			if (systemtime > nextskew)
			{
				skewlinesleft = 100;
				skewtimer = 0;

				if (skewpath == 0) { skewpath = 1; nextskew = systemtime + 2000; return; }
				if (skewpath == 1) { skewpath = 2; nextskew = systemtime + 1500; return; }
				if (skewpath == 2) { skewpath = 3; nextskew = systemtime + 1000; return; }
				if (skewpath == 3) { skewpath = 0; nextskew = systemtime + 1500; return; }
			}
		}

		void CheckSkew()
		{
			int skewval = 0;
			CheckSkewPath();

			while ((skewlinesleft != 0) && skewtimer > 1)
			{
				skewtimer -= 2;
				switch (skewpath)
				{
					case 0: skewval = 1; break;
					case 1: skewval = 0 - 3; break;
					case 2: skewval = 5; break;
					case 3: skewval = 0; break;
				}
				_skewlines[100 - skewlinesleft] = skewval;
				skewlinesleft--;
			}
		}


		void UpdateBackground()
		{
			while (bgrtimer != 0)
			{
				bgrtimer--;
				if(bgrproc != null) bgrproc();
			}
		}

		void MyTimer()
		{
			scrolltimer++;
			skewtimer++;
			bgrtimer++;
			spritetimer++;
			lenstimer++;
			scripttimer++;
		}

		void ScaleWrapBlit(int dx, int dy, Image srcimg, int dw, int dh, Image dest)
		{
			Image img = NewImage(dw, dh);
			Blitter b = new Blitter(img);
			b.TClear();
			b.ScaleBlit(srcimg, 0, 0, dw, dh);
			int mx = dx - (dw / 2);
			int my = dy - (dh / 2);
			if (mx < 0) mx += 255;
			if (my < 0) my += 255;
			b = new Blitter(dest);
			BlitWrap(b, img, mx, my);
			img.Dispose();
		}

		//void ScaleTo(int w, int h, int srcimg, int destimg)


		void StaticInitializers()
		{
			bg = NewImage(256, 256);
			Blitter b = new Blitter(bg);
			b.Clear(Color.Black);
			bg2 = NewImage(256, 256);
			bg3 = NewImage(256, 256);
			LoadResources();
		}

		void Autoexec()
		{
			vidmode = 0;
			switch (vidmode)
			{
				case 0: ybase = 0; break;
				case 1: ybase = 20; break;
				case 2: ybase = 30; break;
			}
			//TODO
			//SetClip(0, ybase, 319, 199+ybase, screen);
				
			//TODO
			//int r = Random(0,5);
			//switch (r)
			//{
			//    case 0:	PlayMusic("music\rain.s3m");
			//    case 1: PlayMusic("music\zcs-02-intro.xm");
			//    case 2: PlayMusic("music\zk-jdood.it");
			//    case 3:	PlayMusic("music\verge6.it");
			//    case 4: PlayMusic("music\lib-grey.xm");
			//    case 5: PlayMusic("music\sully-columns.it");
			//}
			//cursong = r;

			InitTables();
			//HookTimer("MyTimer"); //handled by Update()
			
			scrollpath = 0;
			nextscroll = systemtime+700;
			nextskew = systemtime+800;

			int i;
			for (i=0; i<100; i++)
				_skewlines[i] = 1;

			var b = new Blitter(bg);
			b.SetColor(Color.Transparent);
			b.RectFill(0, 0, 256, 256);

			//InitScript();
		}


		void Tick()
		{
			//UpdateControls();
			CalculateScroll();
			CheckSkew();
			UpdateBackground();
			CheckScript();
		}
	

	}
}
