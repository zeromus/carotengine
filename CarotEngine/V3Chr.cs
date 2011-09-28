using System;
using System.IO;
using System.Collections;
using pr2.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace pr2.CarotEngine {

	public class V3Chr {
		public class NotAChrFileException : Exception { public NotAChrFileException(string str) : base(str) { } }
		public class ChrFileBrokenException : Exception { public ChrFileBrokenException(string str, string file) : base(str + "\n" + file) { } }

		static pr2.Common.StaticByteBuffer loadbuf = new pr2.Common.StaticByteBuffer();

		public Image[] frames;
		public Rectangle hotspot;

		public int[] idleFrames = new int[9];
		public string[] anims = new string[11];


		public V3Chr() { }

		public unsafe V3Chr(string fname) {
			Stream s = ResourceManager.Open(fname);
			BinaryReader br = new BinaryReader(s);

			//signature
			if(br.ReadInt32() != 5392451)
				throw new NotAChrFileException(fname);

			//version
			int version = br.ReadInt32();
			if(version != 5)
				throw new ChrFileBrokenException("Wrong version; expected 5; received " + version.ToString(), fname);

			//bpp
			int bpp = br.ReadInt32();
			if(bpp != 24 && bpp != 32)
				throw new ChrFileBrokenException("Wrong bpp; expected 24 or 32; received " + bpp.ToString(), fname);

			int flags;
			flags = br.ReadInt32();
			int tcol;
			int fxsize, fysize, framecount;
			int customscripts;
			int compression;

			//todo - handle this gracefully
			tcol = br.ReadInt32();

			int hx = br.ReadInt32();
			int hy = br.ReadInt32();
			int hw = br.ReadInt32();
			int hh = br.ReadInt32();
			hotspot = new Rectangle(hx, hy, hw, hh);

			fxsize = br.ReadInt32();
			fysize = br.ReadInt32();
			framecount = br.ReadInt32();

			idleFrames[(int)Directions.s] = br.ReadInt32();
			idleFrames[(int)Directions.n] = br.ReadInt32();
			idleFrames[(int)Directions.w] = br.ReadInt32();
			idleFrames[(int)Directions.e] = br.ReadInt32();
			foreach(string str in
				new string[] { "s", "n", "w", "e", "nw", "ne", "sw", "se" }) {
				int strlen = br.ReadInt32();
				anims[(int)Enum.Parse(typeof(Directions), str, false)] = StringBufferReader.read(s, strlen + 1);
			}
			customscripts = br.ReadInt32();
			compression = br.ReadInt32();

			int incr = fxsize * fysize * (bpp / 8);
			int bufsize = framecount * incr;
			frames = new Image[framecount];
			lock(loadbuf) {
				byte[] framedata = loadbuf.read(new CodecInputStream(s), bufsize);
				fixed(byte* frameptr = framedata)
					for(int i = 0; i < framecount; i++) {
						if(bpp == 24)
							frames[i] = GameEngine.Game.LoadImage24(fxsize, fysize, frameptr + i * incr, Color.Magenta);
						else if(bpp == 32)
							frames[i] = GameEngine.Game.LoadImage32(fxsize, fysize, frameptr + i * incr, Color.Magenta);
					}
			}

			s.Close();

		}
	}

}