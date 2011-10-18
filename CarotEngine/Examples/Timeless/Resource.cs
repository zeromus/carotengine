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


		// Animations

		//int sluggy		= LoadAnimation("sprites\sluggy.gif", 17, 30);
		//int sluggy2		= LoadAnimation("sprites\slug_brown.gif", 17, 30);
		//int leaf		= LoadAnimation("sprites\leafie.gif", 14, 10);
		//int leaf2		= LoadAnimation("sprites\leaf_green.gif", 14, 10);
		//int heart		= LoadAnimation("sprites\hearts.gif", 14, 10);
		//int fish		= LoadAnimation("sprites\fishie.gif", 19, 17);
		//int fish2		= LoadAnimation("sprites\fish2.gif", 19, 17);
		//int flymask		= LoadAnimation("sprites\flymask.gif", 71, 26);	
		//int flyskull	= LoadAnimation("sprites\flyskull.gif", 71, 26);
		//int flyeye		= LoadAnimation("sprites\flyeye.gif", 71, 26);
		//int flyeyeg     = LoadAnimation("sprites\flyeye_green.gif", 71, 26);
		//int flyheart	= LoadAnimation("sprites\flyheart.gif", 71, 26);
		//int snow		= LoadAnimation("sprites\snow.gif", 4, 4);
		//int rain		= LoadAnimation("sprites\raindrop.gif", 6, 6);
		//int flame		= LoadAnimation("sprites\flame.gif", 8, 8);
		//int halo		= LoadAnimation("sprites\halo.png", 144, 144);

		//// Sprites & Imagess

		//int bigeye		= LoadImage("sprites\bigeye.gif");
		//int bigeyeg		= LoadImage("sprites\bigeye_green.gif");
		//int bigheart	= LoadImage("sprites\bigheart.gif");
		//int bigsmile	= LoadImage("sprites\bigsmile.gif");
		//int evilmask	= LoadImage("sprites\evilmask.gif");
		//int flower		= LoadImage("sprites\flower.gif");
		//int hourglass	= LoadImage("sprites\hourglass.gif");
		//int moon		= LoadImage("sprites\moon.gif");
		//int planet		= LoadImage("sprites\planet.gif");
		//int skull		= LoadImage("sprites\skullofawesome.gif");
		//int smalleye	= LoadImage("sprites\smalleye.gif");
		//int tinyeye		= LoadImage("sprites\tinyeye.gif");
		//int smile		= LoadImage("sprites\smallsmile.gif");
		//int smile2		= LoadImagE("sprites\smile_green.gif");
		//int sun			= LoadImage("sprites\sun.gif");
		//int candle		= LoadImage("sprites\candle.gif");
		//int clock		= LoadImage("sprites\smallclock.gif");

		////int rune[9];

		//// Background Images

		//int facered     = LoadImage("background\face_red.gif");
		//int faceblue	= LoadImage("background\face_blue.gif");
		//int facegreen	= LoadImage("background\face_green.gif");

		//int sphereblue	= LoadImage("background\sphereblue.gif");
		//int spheregreen = LoadImage("background\spheregreen.gif");
		//int spheregrey	= LoadImage("background\spheregrey.gif");
		//int sphereorange= LoadImage("background\sphereorange.gif");
		//int spherepurple= LoadImage("background\spherepurple.gif");

		//int lightning	= LoadImage("background\lightning.gif");

		//int bobblue		= LoadImage("background\bobblue.gif");
		//int bobgreen	= LoadImage("background\bobgreen.gif");
		//int bobred		= LoadImage("background\bobred.gif");
		//int bobpurple	= LoadImage("background\bobpurple.gif");

		//int tiledbg0	= LoadImage("background\tiledbg0.gif");
		//int tiledbg1	= LoadImage("background\tiledbg1.gif");
		//int tiledbg2	= LoadImage("background\tiledbg2.gif");
		//int tiledbg3	= LoadImage("background\tiledbg3.gif");
		//int tiledbg4	= LoadImage("background\tiledbg4.gif");
		//int tiledbg5	= LoadImage("background\tiledbg5.gif");
		//int tiledbg6	= LoadImage("background\tiledbg6.gif");
		//int tiledbg7	= LoadImage("background\tiledbg7.gif");
		//int tiledbg8	= LoadImage("background\tiledbg8.gif");
		//int tiledbg9	= LoadImage("background\tiledbg9.gif");

		//int grad1		= LoadImage("background\_grad1.png");
		//int grad2		= LoadImage("background\_grad2.png");
		//int grad3		= LoadImage("background\_grad3.png");
		//int grad4		= LoadImage("background\_grad4.png");
		//int grad5		= LoadImage("background\_grad5.png");
		//int grad6		= LoadImage("background\_grad8.png");
		//int grad7		= LoadImage("background\_grad7.png");
		//int grad8		= LoadImage("background\_grad8.png");

		//int evilgrad1	= LoadImage("background\evilgrad.gif");
		//int evilgrad2	= LoadImage("background\evilgd1.pcx");
		//int evilgrad3	= LoadImage("background\evilgd2.pcx");
		//int evilgrad4	= LoadImage("background\evilgrad3.gif");

		//int	fadedown	= LoadImage("background\fadedown.gif");
		//int fadeup		= LoadImage("background\fadeup.gif");
		//int bgalpha     = NewImage(256, 256);

		// Tables
		int[] pingpong3 = new int[4];
		int[] pingpong4 = new int[6];
		int[] leaftbl = new int[20];

		void InitTables() {
			pingpong3[0] = 0;
			pingpong3[1] = 1;
			pingpong3[2] = 2;
			pingpong3[3] = 1;

			pingpong4[0] = 0;
			pingpong4[1] = 1;
			pingpong4[2] = 2;
			pingpong4[3] = 3;
			pingpong4[4] = 2;
			pingpong4[5] = 1;

			leaftbl[0] = 0;
			leaftbl[1] = 1;
			leaftbl[2] = 2;
			leaftbl[3] = 3;
			leaftbl[4] = 4;
			leaftbl[5] = 4;
			leaftbl[6] = 4;
			leaftbl[7] = 4;
			leaftbl[8] = 4;
			leaftbl[9] = 4;
			leaftbl[10] = 4;
			leaftbl[11] = 3;
			leaftbl[12] = 2;
			leaftbl[13] = 1;
			leaftbl[14] = 0;
			leaftbl[15] = 0;
			leaftbl[16] = 0;
			leaftbl[17] = 0;
			leaftbl[18] = 0;
			leaftbl[19] = 0;

			/*	rune[0]	= LoadImage("sprites\ele-anti.gif");
				rune[1]	= LoadImage("sprites\ele-arse.gif");
				rune[2]	= LoadImage("sprites\ele-gold.gif");
				rune[3]	= LoadImage("sprites\ele-iron.gif");
				rune[4]	= LoadImage("sprites\ele-merc.gif");
				rune[5]	= LoadImage("sprites\ele-phos.gif");
				rune[6]	= LoadImage("sprites\ele-pota.gif");
				rune[7]	= LoadImage("sprites\ele-silv.gif");
				rune[8]	= LoadImage("sprites\ele-sulf.gif");
			*/
			InitializeTintTable();
		}
	}
}