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

		int spr_state = 0;
		int spr_die;

		const int SPR_NONE = 0;
		const int SPR_SLUG = 1;
		const int SPR_FLY = 2;
		const int SPR_FLOWER = 3;
		const int SPR_CELESTIAL = 4;
		const int SPR_CIRCULAR = 5;
		const int SPR_FISH = 6;
		const int SPR_SMILIES = 7;
		const int SPR_LEAVES = 8;
		const int SPR_WEATHER = 9;
		const int SPR_CANDLE = 10;


		void RenderSprites(Blitter b)
		{
			int s;

			if (spr_state == 0)
				return;

			switch (spr_state)
			{
				case SPR_SLUG: RenderSlug(b); break;
				//case SPR_FLY: RenderFlyer(); break;
				//case SPR_FLOWER: RenderFlower() break;
				//case SPR_CELESTIAL: RenderCelestial(); break;
				//case SPR_CIRCULAR: RenderCircular(); break;
				//case SPR_FISH: RenderFish(); break;
				//case SPR_SMILIES: RenderSmilies(); break;
				//case SPR_LEAVES: RenderLeaves(); break;
				//case SPR_WEATHER: RenderWeather() break;
				//case SPR_CANDLE: RenderCandle(); break;
			}
		}
	}
}
