//rendertarget infos
//https://forums.microsoft.com/MSDN/ShowPost.aspx?PostID=882859&SiteID=1
//examples
//http://forums.microsoft.com/MSDN/ShowPost.aspx?PostID=1089984&SiteID=1

using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using pr2;
using pr2.Common;
using pr2.CarotEngine;

namespace pr2.CarotEngine {

/// <summary>
/// manages a set of sprite frames so you can just load it up and pick the frame you want.
/// right now only works with SimpleArchives
/// </summary>
public class SpriteManager {
	public int NumFrames;
	public Image GetFrame(int index) { return frames[index]; }
	public Image this[int index] { get { return GetFrame(index); } }
	internal Image[] frames;

}

public partial class GameEngine {

	/// <summary>
	/// loads a sprite from a SimpleArchive.
	/// </summary>
	public SpriteManager LoadSpriteManager(string fname) {
		Stream stream = ResourceManager.Open(fname);
		using (SimpleArchive sar = new SimpleArchive(stream))
			return LoadSpriteManager(sar);
	}

	/// <summary>
	/// loads a sprite from a SimpleArchive.
	/// </summary>
	public SpriteManager LoadSpriteManager(SimpleArchive sar) {
		SpriteManager ret = new SpriteManager();
		ret.frames = new Image[sar.numFiles];
		ret.NumFrames = sar.numFiles;
		for (int i=0; i<sar.numFiles; i++)
			using (Stream s = sar.open(sar.sortedFiles[i]))
				ret.frames[i] = Game.LoadImage(s);
		return ret;
	}


	/// <summary>
	/// loads a spritemanager by dicing a source image
	/// </summary>
	public SpriteManager LoadSpriteManager(string fname, int width, int height, int xpad, int ypad) {
		using(Image img = LoadImage(fname))
			return LoadSpriteManager(img, width, height, xpad, ypad);
	}

	/// <summary>
	/// loads a spritemanager by dicing a source image, using LoadImage0
	/// </summary>
	public SpriteManager LoadSpriteManager0(string fname, int width, int height, int xpad, int ypad) {
		using(Image img = LoadImage0(fname))
			return LoadSpriteManager(img, width, height, xpad, ypad);
	}

	/// <summary>
	/// loads a spritemanager by dicing a source image
	/// </summary>
	public SpriteManager LoadSpriteManager(Image source, int width, int height, int xpad, int ypad) {
		//TODO - don't do this with blitting. that way we can keep the source texture format. but this will work for now..
		SpriteManager ret = new SpriteManager();
		int framesAcross = source.Width / (width+xpad);
		int framesDown = source.Height / (height+ypad);
		ret.NumFrames = framesAcross*framesDown;
		ret.frames = new Image[ret.NumFrames];
		int ctr=0;
		BlendNone();
		for (int iy=0; iy<framesDown; iy++) {
			for(int ix=0;ix<framesAcross; ix++,ctr++) {
				Image img = NewImage(width, height);
				ret.frames[ctr] = img;
				Blitter b = new Blitter(img);
				//b.Clear(Color.Transparent);
				b.Blit(source, -ix*(width+xpad), -iy*(height+ypad));
				img.Cache();
				img.Premultiply();
			}
		}
		BlendNormal();
		return ret;
	}
}
}