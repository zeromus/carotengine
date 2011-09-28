using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.IO;
using System.Text;
using pr2;
using pr2.Common;

using Microsoft.Xna.Framework;

namespace pr2.CarotEngine
{
public class Map
{
	public class NotAMapFileException : Exception { public NotAMapFileException(string str) : base(str) {} }
	public class MapFileBrokenException : Exception { public MapFileBrokenException(string str, string file) : base(str+"\n"+file){} }
	
	public class VspFileNotFoundException : Exception { public VspFileNotFoundException(string str) : base("VSP File not found:\n" + str){} }
	public class NotAVspFileException : Exception  { public NotAVspFileException(string str) : base(str) {} }
	public class VspFileBrokenException : Exception { public VspFileBrokenException(string str, string file) : base(str+"\n"+file){} }


    RpgController rpgController;
	public MapScriptHandler scriptHandler;
	public string name,vspfile,musicfile;
	public string rstring,initscript;
	public Point start = new Point(0,0);
	public VSP vsp;
	public TileLayer[] layers;
	public Zonedef[] zonedefs;
	public Entdef[] entdefs;
	public LogicalLayer logicalLayer;
	public int width, height;

	public void initTextures() {
		foreach(Layer l in layers)
			l.initTexture();
	}

	public class LogicalLayer : Layer
	{
		public LogicalLayer(int width, int height)
		{
			this.width = width;
			this.height = height;
		}
		public Layer obs, zone;
		public int readZone(int x, int y) { return zone[y,x]; }
		public int readObs(int x, int y) { return obs[y,x]; }
	}

	public class TileLayer : Layer
	{
		public int lucent;
		public Vector2 parallax = new Vector2(0.0f,0.0f);

		public void loadFromv32stream(BinaryReader br)
		{
			br.BaseStream.Seek(256,SeekOrigin.Current);
			parallax.X = (float)br.ReadDouble();
			parallax.Y = (float)br.ReadDouble();
			width = br.ReadInt16();
			height = br.ReadInt16();
			lucent = br.ReadByte();
			
			tiles = new int[width*height];
			readLayer(16,br);		
		}
	}

	public class Layer : GameEngineComponent
	{
		public int width, height;
		public int[] tiles;
		public Texture2D tex;

		public int this[int y, int x] { get { return getTile(x,y); } set { setTile(x,y,value); } }

		/// <summary>
		/// Initializes the texture for fast rendering.
		/// can be called repeatedly to refresh changes via setTile.
		/// TODO: if we end up doing this a lot, we may want to hold on to the float[] instead of the int[]
		/// </summary>
		public void initTexture() {
			//if(tex == null) 
			//    tex = new Texture2D(game.Device, width, height, 1, TextureUsage.None, SurfaceFormat.Single); //upgrade change: was management mode automatic
			//float[] data = new float[width * height];
			//int j = 0;
			//for(int y = 0; y < height; y++)
			//    for(int x = 0; x < width; x++, j++)
			//        data[j] = (float)tiles[j];
			//tex.SetData(data);
		}

		//todo: do we like this behavior?
		public int getTile(int x, int y)
		{
			if(x<0 || y<0) return -1;
			if(x>=width) return -1;
			if(y>=height) return -1;
			return tiles[width*y+x];
		}

		public void setTile(int x, int y, int t)
		{
			tiles[width*y+x] = t;
		}

		public Layer(int w, int h)
		{
			width = w;
			height = h;
			tiles = new int[width * height];
		}
		
		public Layer() {}

		public void loadSpecialFromv32stream(int size, int w, int h, BinaryReader br)
		{
			width = w;
			height = h;
			tiles = new int[w*h];

			readLayer(size,br);
		}

		protected void readLayer(int size, BinaryReader br)
		{
			BinaryReader zipbr = new BinaryReader(new CodecInputStream(br.BaseStream));

			if(size==8)
				readLayer8(zipbr);
			else
				readLayer16(zipbr);
		}

		unsafe protected void readLayer8(BinaryReader br)
		{
			
			fixed(int *ptr = tiles)
			{
				int *ptr2 = ptr;
				for(int i=0;i<width*height;i++)
					*ptr2++ = br.ReadByte();
			}
		}

		unsafe protected void readLayer16(BinaryReader br)
		{
			fixed(int *ptr = tiles)
			{
				int *ptr2 = ptr;
				for(int i=0;i<width*height;i++)
					*ptr2++ = br.ReadInt16();
			}
		}

	}

	public class Zonedef
	{
		public string name, script;
		public int percent,delay,method;
	}

    // Overkill: Stores VSP animation info.
    public class VSPAnimation
    {
		public const int ANIMATION_FORWARD = 0;
		public const int ANIMATION_REVERSE = 1;
		public const int ANIMATION_RANDOM = 2;
        public const int ANIMATION_PINGPONG = 3;

        public string name;
        public int start, end;
        public int delay;
        public int mode;

        public int counter;

        public VSPAnimation(string name, int start, int end, int delay, int mode)
        {
            this.name = name;
            this.start = start;
            this.end = end;
            this.delay = delay;
            this.mode = mode;

            counter = 0;
        }

    }


	public class VSP
	{

		public Image tiles;
        public Image obs;
        public VSPAnimation[] animation;
        public int[] tileIndex;
        public bool[] flipped;

		/// <summary>
		/// the number of tiles wide and high the tiledata image is
		/// </summary>
		public int tilesW, tilesH;

		unsafe public VSP(string fname)
		{
			Stream s = ResourceManager.Open(fname);
			BinaryReader br = new BinaryReader(s);

			//signature
			if(br.ReadInt32() != 5264214)
				throw new NotAVspFileException(fname);

			//version
			int version = br.ReadInt32();
			if(version != 6)
				throw new VspFileBrokenException("Wrong version; expected 6; received " + version.ToString(),fname);

			//tilesize
			int tilesize = br.ReadInt32();
			if(tilesize != 16)
				throw new VspFileBrokenException("Wrong tilesize; expected 16; received " + tilesize.ToString(),fname);

			//miscstuff
			int format = br.ReadInt32();
			int numTiles = br.ReadInt32();
			int compression = br.ReadInt32();

            tileIndex = new int[numTiles];
            flipped = new bool[numTiles];
            for (int i = 0; i < numTiles; i++)
            {
                tileIndex[i] = i;
                flipped[i] = false;
            }

			//find the size of texture we need.
			//our theory is that the minimal texture memory wastage is achieved by finding the smallest
			//square that can contain all the tiles
			int square = (int)Math.Sqrt(numTiles);
			if(square * square != numTiles) square++;
			tilesW = tilesH = square;
			
			tilesW = 128;
			tilesH = (numTiles / tilesW) + 1;

			BinaryReader zipbr = new BinaryReader(new CodecInputStream(s));
			byte[] tilebuf = zipbr.ReadBytes(numTiles*16*16*3);

            SoftGraphics sg = new SoftGraphics();
			//dice it up.
			Softimage si = sg.newImage(tilesW<<4,tilesH<<4);
			Softimage siVsp = sg.imageFrom24bpp(16, 16 * numTiles, tilebuf);
			for(int ty = 0, t = 0; ty < tilesH; ty++)
				for(int tx = 0; tx < tilesW; tx++, t++)
					//for(int i=0;i<numTiles;i++)
					if(t >= numTiles) break;
					else //sg.blitSubrect(0,i*16,16,16,(i&127)<<4,(i>>7)<<4,siVsp,si);
						sg.blitSubrect(0,t*16,16,16,tx<<4,ty<<4,siVsp,si);
			tiles = GameEngine.Game.LoadSoftimage(si);
			tiles.Alphafy(Color.Magenta);
			si.Dispose();
			siVsp.Dispose();

            // Animations
            int animationCount = br.ReadInt32();

            animation = new VSPAnimation[animationCount];
            for (int i = 0; i < animationCount; i++)
            {
                string name = StringBufferReader.read(s, 256);
                // Start index
                int start = br.ReadInt32();
                // End index
                int end = br.ReadInt32();
                // Delay between frames
                int delay = br.ReadInt32() * 10;
                // Animation mode.
                int mode = br.ReadInt32();
                animation[i] = new VSPAnimation(name, start, end, delay, mode);
            }

            // Obstruction data
            int obsCount = br.ReadInt32();

            zipbr = new BinaryReader(new CodecInputStream(s));
            byte[] obspixels = zipbr.ReadBytes(obsCount * 16 * 16);

			Softimage siObs = sg.newImage(2048, ((numTiles + 128) >> 7) << 4);
			for (int tile = 0; tile < obsCount; tile++) {
			    Softimage tileImage = sg.newImage(16, 16);
			    for (int i = 0; i < 16 * 16; i++) {
			        int idx = tile * 16 * 16 + i;
					int pixel = (obspixels[idx] != 0 ? sg.makeColor(255, 255, 255) : 0);
					tileImage.putPixel(i % tilesize, i / tilesize, pixel);
				}
			    sg.blitSubrect(0, 0, 16, 16, (tile & 127) << 4, (tile >> 7) << 4, tileImage, siObs);
			    tileImage.Dispose();
			}
			obs = GameEngine.Game.LoadSoftimage(siObs);
			obs.Alphafy(Color.Black);
			siObs.Dispose();

			br.Close();
		
		}

        // Overkill: Updates a vsp animation by one tick
        public void UpdateTileAnimation(VSPAnimation animation)
        {
            animation.counter--;
            if (animation.counter > 0) return;
            switch (animation.mode)
            {
                case VSPAnimation.ANIMATION_FORWARD:
                    {
                        for (int i = animation.start; i <= animation.end; i++)
                        {
                            tileIndex[i]++;
                            if (tileIndex[i] > animation.end)
                            {
                                tileIndex[i] = animation.start;
                            }
                        }
                        break;
                    }
                case VSPAnimation.ANIMATION_REVERSE:
                    {
                        for (int i = animation.start; i <= animation.end; i++)
                        {
                            tileIndex[i]--;
                            if (tileIndex[i] < animation.start)
                            {
                                tileIndex[i] = animation.end;
                            }
                        }
                        break;
                    }
                case VSPAnimation.ANIMATION_RANDOM:
                    {
                        // Whatever. Nobody uses random animations anyways
                        break;
                    }
                case VSPAnimation.ANIMATION_PINGPONG:
                    {
                        for (int i = animation.start; i <= animation.end; i++)
                        {
                            if (flipped[i] && tileIndex[i] <= animation.start)
                            {
                                flipped[i] = false;
                            }
                            else if (!flipped[i] && tileIndex[i] >= animation.end)
                            {
                                flipped[i] = true;
                            }
                            if (flipped[i])
                            {
                                tileIndex[i]--;
                            }
                            else
                            {
                                tileIndex[i]++;
                            }
                        }
                        break;
                    }
            }
            animation.counter = animation.delay;
        }

        // Overkill: Updates all vsp animations by one tick
        public void Update()
        {
            for (int i = 0; i < animation.GetLength(0); i++)
            {
                UpdateTileAnimation(animation[i]);
            }
        }
    }

	public struct Entdef
	{
		public int index;
		public int x,y;
		public Directions facing;
		public bool obstructable, obstruction, autoface;
		public int speed;
		public int mode;
		public int x1,y1,x2,y2;
		public int wanderDelay;
		public string moveScript,chrname,descr,script;
	}


	public Map(string fname, RpgController rpgController)
	{
		scriptHandler = new MapScriptHandler(fname, this, rpgController);
		Stream s = ResourceManager.Open(fname);
		BinaryReader br = new BinaryReader(s);
		
		//signature
		string str = StringBufferReader.read(s,6);
		if(str != "V3MAP")
			throw new NotAMapFileException(fname);

		//version
		int version = br.ReadInt32();
		if(version != 2) throw new MapFileBrokenException("Wrong version; expected 2; received " + version.ToString(),fname);

		try
		{

			//vc offset (skipped)
			br.ReadInt32();

			//mapname,vspname,musicname,renderstring,startupscript
			name = StringBufferReader.read(s,256);
			vspfile = StringBufferReader.read(s,256);
			musicfile = StringBufferReader.read(s,256);
			rstring = StringBufferReader.read(s,256);
			initscript = StringBufferReader.read(s,256);

			//startloc
			start.X = br.ReadInt16();
			start.Y = br.ReadInt16();

			//layers
			int numLayers = br.ReadInt32();
			layers = new TileLayer[numLayers];
			for(int i=0;i<numLayers;i++)
			{
				TileLayer tl = layers[i] = new TileLayer();
				tl.loadFromv32stream(br);
			}

			width = layers[0].width;
			height = layers[0].height;

			//special layers
			logicalLayer = new LogicalLayer(width,height);
			logicalLayer.obs = new Layer(); logicalLayer.obs.loadSpecialFromv32stream(8,width,height,br);
			logicalLayer.zone = new Layer(); logicalLayer.zone.loadSpecialFromv32stream(16,width,height,br);

			//zones
			int numZonedefs = br.ReadInt32();
			zonedefs = new Zonedef[numZonedefs];
			for(int i=0;i<numZonedefs;i++)
			{
				zonedefs[i] = new Zonedef();
				zonedefs[i].name = StringBufferReader.read(s,256);
				zonedefs[i].script = StringBufferReader.read(s,256);
				zonedefs[i].percent = br.ReadByte();
				zonedefs[i].delay = br.ReadByte();
				zonedefs[i].method = br.ReadByte();
			}

			//entities
			
			int numEntdefs = br.ReadInt32();
			entdefs = new Entdef[numEntdefs];
			for(int i=0;i<numEntdefs;i++)
			{
				entdefs[i] = new Entdef();
				entdefs[i].index = i;
				entdefs[i].x = br.ReadInt16();
				entdefs[i].y = br.ReadInt16();
				entdefs[i].facing = (Directions)br.ReadByte();
				if(entdefs[i].facing == Directions.None) entdefs[i].facing = Directions.s;
				entdefs[i].obstructable = br.ReadByte()==1;
				entdefs[i].obstruction = br.ReadByte()==1;
				entdefs[i].autoface = br.ReadByte()==1;
				entdefs[i].speed = br.ReadInt16(); //convert this to carot ticks?
				br.ReadByte();//activation mode
				entdefs[i].mode = br.ReadByte();
				entdefs[i].x1 = br.ReadInt16();
				entdefs[i].y1 = br.ReadInt16();
				entdefs[i].x2 = br.ReadInt16();
				entdefs[i].y2 = br.ReadInt16();
				entdefs[i].wanderDelay = br.ReadInt16();//convert this to carot ticks?
				br.ReadInt32();//wtf?
				entdefs[i].moveScript = StringBufferReader.read(s,256);
				entdefs[i].chrname = StringBufferReader.read(s,256);
				entdefs[i].descr = StringBufferReader.read(s,256);
				entdefs[i].script = StringBufferReader.read(s,256);
			}
		}
		catch(Exception e)
		{
			throw new MapFileBrokenException("Something is wrong with the mapfile.  Here's what happened:\n" + e.Message,fname);
		}

		//load up the vsp
		try
		{
			vsp = new VSP(Lib.GetAccompanyingFilePath(fname,vspfile));
		}
		catch(FileNotFoundException e) {throw new VspFileNotFoundException(e.Message); }
		

		br.Close();

	}


}
}