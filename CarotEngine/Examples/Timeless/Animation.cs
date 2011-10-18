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

		class anim_t {
			public int sizex, sizey;
			public int numframes;
			public int active;
			public Image image;
			public Image bufimage, bufferedframe;
		};

		anim_t LoadAnimation(string filename, int sizex, int sizey) {
			// load up this animation, detect number of frames
			anim_t anim = new anim_t();
			anim.image = LoadImage0(filename);
			anim.active = 1;
			anim.sizex = sizex;
			anim.sizey = sizey;
			anim.numframes = anim.image.Height / sizey;
			anim.bufimage = NewImage(sizex, sizey);
			anim.bufferedframe = null;
			return anim;
		}


		void BlitFrameAt(int x, int y, anim_t anim, int frame, Blitter b)
		{
			if (frame > anim.numframes)
				return;

			//if (frame == anim.bufferedframe)
			int frametop = anim.sizey * frame;
			b.BlitSubrect(anim.image, 0, frametop, anim.sizex, anim.sizey, x, y);
		}

	}
}
