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

		const int LM_NONE			=0;
		const int LM_VERT			=1;
		const int LM_HORIZ		=2;
		const int LM_BOUNCE		=3;

		const int LE_TINT			=1;
		const int LE_GREY			=2;
		const int LE_BENEATH		=3;
		const int LE_SCALE		=4;
		const int LE_MOSAIC       =5;

		int lens_state, lens_effect;
		int lens_die, lens_dir;
		int lens_x, lens_y;

		void RenderLens() {
			//todo
			//int s;

			//if (lens_state = 0)
			//    return;

			//switch (lens_state) {
			//    case LM_VERT: VertMove();
			//    case LM_HORIZ: HorizMove();
			//    case LM_BOUNCE: BounceMove();
			//}
			//switch (lens_effect) {
			//    case LE_TINT: RenderTintLens();
			//    case LE_BENEATH: RenderBeneathLens();
			//    case LE_SCALE: RenderScaleLens();
			//    case LE_GREY: RenderGreyLens();
			//    case LE_MOSAIC: RenderMosaicLens();
			//}
		}

	}
}
