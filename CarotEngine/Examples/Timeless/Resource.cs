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

		anim_t sluggy;
		anim_t sluggy2;
		anim_t leaf;
		anim_t leaf2;
		anim_t heart;
		anim_t fish;
		anim_t fish2;
		anim_t flymask;
		anim_t flyskull;
		anim_t flyeye;
		anim_t flyeyeg;
		anim_t flyheart;
		anim_t snow;
		anim_t rain;
		anim_t flame;
		anim_t halo;


		// Sprites & Images

		Image bigeye;
		Image bigeyeg;
		Image bigheart;
		Image bigsmile;
		Image evilmask;
		Image flower;
		Image hourglass;
		Image moon;
		Image planet;
		Image skull;
		Image smalleye;
		Image tinyeye;
		Image smile;
		Image smile2;
		Image sun;
		Image candle;
		Image clock;

		// Background Images

		Image facered;
		Image faceblue;
		Image facegreen;

		Image sphereblue;
		Image spheregreen;
		Image spheregrey;
		Image sphereorange;
		Image spherepurple;

		Image lightning;

		Image bobblue;
		Image bobgreen;
		Image bobred;
		Image bobpurple;

		Image tiledbg0;
		Image tiledbg1;
		Image tiledbg2;
		Image tiledbg3;
		Image tiledbg4;
		Image tiledbg5;
		Image tiledbg6;
		Image tiledbg7;
		Image tiledbg8;
		Image tiledbg9;

		Image grad1;
		Image grad2;
		Image grad3;
		Image grad4;
		Image grad5;
		Image grad6;
		Image grad7;
		Image grad8;

		Image evilgrad1;
		Image evilgrad2;
		Image evilgrad3;
		Image evilgrad4;


		void LoadResources()
		{
			sluggy = LoadAnimation("sprites/sluggy.png", 17, 30);
			sluggy2 = LoadAnimation("sprites/slug_brown.png", 17, 30);
			leaf = LoadAnimation("sprites/leafie.png", 14, 10);
			leaf2 = LoadAnimation("sprites/leaf_green.png", 14, 10);
			heart = LoadAnimation("sprites/hearts.png", 14, 10);
			fish = LoadAnimation("sprites/fishie.png", 19, 17);
			fish2 = LoadAnimation("sprites/fish2.png", 19, 17);
			flymask = LoadAnimation("sprites/flymask.png", 71, 26);
			flyskull = LoadAnimation("sprites/flyskull.png", 71, 26);
			flyeye = LoadAnimation("sprites/flyeye.png", 71, 26);
			flyeyeg = LoadAnimation("sprites/flyeye_green.png", 71, 26);
			flyheart = LoadAnimation("sprites/flyheart.png", 71, 26);
			snow = LoadAnimation("sprites/snow.png", 4, 4);
			rain = LoadAnimation("sprites/raindrop.png", 6, 6);
			flame = LoadAnimation("sprites/flame.png", 8, 8);
			halo = LoadAnimation("sprites/halo.png", 144, 144);

			bigeye = LoadImage0("sprites/bigeye.png");
			bigeyeg = LoadImage0("sprites/bigeye_green.png");
			bigheart = LoadImage0("sprites/bigheart.png");
			bigsmile = LoadImage0("sprites/bigsmile.png");
			evilmask = LoadImage0("sprites/evilmask.png");
			flower = LoadImage0("sprites/flower.png");
			hourglass = LoadImage0("sprites/hourglass.png");
			moon = LoadImage0("sprites/moon.png");
			planet = LoadImage0("sprites/planet.png");
			skull = LoadImage0("sprites/skullofawesome.png");
			smalleye = LoadImage0("sprites/smalleye.png");
			tinyeye = LoadImage0("sprites/tinyeye.png");
			smile = LoadImage0("sprites/smallsmile.png");
			smile2 = LoadImage0("sprites/smile_green.png");
			sun = LoadImage0("sprites/sun.png");
			candle = LoadImage0("sprites/candle.png");
			clock = LoadImage0("sprites/smallclock.png");

			// Background Images

			facered = LoadImage0("background/face_red.png");
			faceblue = LoadImage0("background/face_blue.png");
			facegreen = LoadImage0("background/face_green.png");

			sphereblue = LoadImage0("background/sphereblue.png");
			spheregreen = LoadImage0("background/spheregreen.png");
			spheregrey = LoadImage0("background/spheregrey.png");
			sphereorange = LoadImage0("background/sphereorange.png");
			spherepurple = LoadImage0("background/spherepurple.png");

			lightning = LoadImage0("background/lightning.png");

			bobblue = LoadImage0("background/bobblue.png");
			bobgreen = LoadImage0("background/bobgreen.png");
			bobred = LoadImage0("background/bobred.png");
			bobpurple = LoadImage0("background/bobpurple.png");

			tiledbg0 = LoadImage0("background/tiledbg0.png");
			tiledbg1 = LoadImage0("background/tiledbg1.png");
			tiledbg2 = LoadImage0("background/tiledbg2.png");
			tiledbg3 = LoadImage0("background/tiledbg3.png");
			tiledbg4 = LoadImage0("background/tiledbg4.png");
			tiledbg5 = LoadImage0("background/tiledbg5.png");
			tiledbg6 = LoadImage0("background/tiledbg6.png");
			tiledbg7 = LoadImage0("background/tiledbg7.png");
			tiledbg8 = LoadImage0("background/tiledbg8.png");
			tiledbg9 = LoadImage0("background/tiledbg9.png");

			grad1 = LoadImage0("background/_grad1.png");
			grad2 = LoadImage0("background/_grad2.png");
			grad3 = LoadImage0("background/_grad3.png");
			grad4 = LoadImage0("background/_grad4.png");
			grad5 = LoadImage0("background/_grad5.png");
			grad6 = LoadImage0("background/_grad8.png");
			grad7 = LoadImage0("background/_grad7.png");
			grad8 = LoadImage0("background/_grad8.png");

			evilgrad1 = LoadImage0("background/evilgrad.png");
			evilgrad2 = LoadImage0("background/evilgd1.png");
			evilgrad3 = LoadImage0("background/evilgd2.png");
			evilgrad4 = LoadImage0("background/evilgrad3.png");

			halo.image.Alphafy(Color.Magenta);
		}


		////int rune[9];



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