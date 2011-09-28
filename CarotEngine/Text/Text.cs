using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Microsoft.Xna.Framework;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using pr2;

namespace pr2.CarotEngine {

	public static class Text {
		public static Dictionary<string, Color> Colors = new Dictionary<string, Color>();
	}

	public interface IFontRenderer {
		/// <summary>
		/// returns the height of the font
		/// </summary>
		int height { get; }
		/// <summary>
		/// measures the width of the provided string
		/// </summary>
		int MeasureWidth(string str);
		/// <summary>
		/// renders the provided string to the specified blitter
		/// </summary>
		void Render(Blitter b, int x, int y, string str);
		void Render(Blitter b, Point pt, string str);
		/// <summary>
		/// renders the provided string right-aligned to the specified blitter
		/// </summary>
		void RenderRight(Blitter b, int x, int y, string str);
		/// <summary>
		/// The color to be used to render the font. the color can't always be set (e.g. bitmap fonts)
		/// </summary>
		Color Color { get; set; }
	}

	public abstract class FontRendererBase {
		public virtual Color Color { get { return _color; } set { _color = value; } }
		protected Color _color;
		public abstract int MeasureWidth(string str);
		public abstract void Render(Blitter b, int x, int y, string str);
		public void RenderRight(Blitter b, int x, int y, string str) {
			x -= MeasureWidth(str);
			Render(b, x, y, str);
		}
		public void Render(Blitter b, Point pt, string str) {
			Render(b, pt.X, pt.Y, str);
		}
	}



	/// <summary>
	/// A font renderer that does nothing.  And all the characters are tiny.  Like 0x0.  Useful for defaults or something
	/// </summary>
	public class NullFontRenderer : FontRendererBase, IFontRenderer {
		public int height { get { return 0; } }
		public override int MeasureWidth(string str) { return 0; }
		public override void Render(Blitter b, int x, int y, string str) { }
	}

    public class FontRenderer : FontRendererBase, IFontRenderer
    {
        public int spacing = 1;
        public int spaceSize = 4;
        public FontSW font;

        public FontRenderer(FontSW font) { this.font = font; }
        public FontRenderer() { }

        //Dictionary<char, char> accents = new Dictionary<int, char>();
        static char[] accents = new char[512];
        static FontRenderer()
        {
            accents['a'] = 'á';
            accents['e'] = 'ë';
            accents['i'] = 'í';
            accents['o'] = 'ö';
            accents['u'] = 'ú';
        }

        public static IEnumerable EnumerateCharacters(string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                if (c == '`')
                {
                    if (i != str.Length - 1)
                        yield return accents[str[++i]];
                }
                yield return c;
            }
        }

        public static string ReplaceAccents(string str)
        {
            StringBuilder sb = new StringBuilder(str.Length * 2);
            foreach (char c in EnumerateCharacters(str))
                sb.Append(c);
            return sb.ToString();
        }

        public override int MeasureWidth(string str)
        {
            int count = 0;
            bool bTrailingSpacer = false;
            foreach (char c in EnumerateCharacters(str))
            {
                if (c == ' ') { count += spaceSize; bTrailingSpacer = false; }
                else
                {
                    count += font.fontDefinition.characters[c].vw;
                    count += spacing;
                    bTrailingSpacer = true;
                }
            }
            if (bTrailingSpacer) count -= spacing;

            return count;
        }

        public override unsafe void Render(Blitter b, int x, int y, string str)
        {
            b.BeginBatch();

            Image img = font.image;

            FontDefinition.Character[] characters = font.fontDefinition.characters;
            foreach (char c in EnumerateCharacters(str))
            {
                int n = (int)c;
                if (c == ' ') x += spaceSize;
                else
                {
                    int dx = x - characters[n].vx;
                    int dy = y - characters[n].vy;
                    b.BlitSubrectBatched(img, characters[n].x, characters[n].y, characters[n].w, characters[n].h, dx, dy);

                    x += characters[n].vw;
                    x += spacing;
                }
            }

            b.ExecuteSubrectBatch(img);
        }

        public int height { get { return font.fontDefinition.Height; } }

    }

	//public class FontHW : FontRendererBase, GraphicsHW.IUnmanagedResource, IFontRenderer {
	//    GraphicsHW g;
	//    d3d.Font font;
	//    sd.Font sdfont;
	//    int _height;
	//    static Common.StaticByteBuffer _buffer = new pr2.Common.StaticByteBuffer();

	//    /// <summary>
	//    /// initializes a FontHW with the given gdi+ font resource
	//    /// </summary>
	//    public FontHW(GraphicsHW g, sd.Font font) {
	//        sdfont = (sd.Font)font.Clone();
	//        this.font = new d3d.Font(g.device, font);
	//        _height = (int)Math.Ceiling(font.Size);
	//        this.g = g;
	//        g.registerResource(this);
	//    }

	//    public int height { get { return _height; } }

	//    public void Dispose() {
	//        font.Dispose();
	//        g.unregisterResource(this);
	//    }
	//    public void Reset() { font.OnResetDevice(); }
	//    public void Lost() { font.OnLostDevice(); }

	//    /// <summary>
	//    /// renders a string with the current render color 
	//    /// </summary>
	//    public override void render(Blitter b, int x, int y, string str) {
	//        render(b, _color, x, y, str);
	//    }

	//    /// <summary>
	//    /// renders a string in the specified color
	//    /// </summary>
	//    void render(Blitter b, sd.Color col, int x, int y, string str) {
	//        sd.Color old = b.color;
	//        b.color = col;
	//        b.forceActivate();
	//        //int test = font.DrawText(null, FontRenderer.ReplaceAccents(str), x+b.ox, y+b.oy, col);
	//        int test = font.DrawText(null, FontRenderer.ReplaceAccents(str), x, y, col);
	//        //TODO not sure about proper ox/oy behavior her
	//        b.color = old;
	//    }

	//    /// <summary>
	//    /// measures the width of the given string
	//    /// </summary>
	//    public override int measureWidth(string str) {
	//        //sd.Size sz;
	//        //sz = System.Windows.Forms.TextRenderer.MeasureText("i", sdfont, new sd.Size(0, 0), System.Windows.Forms.TextFormatFlags.NoPadding | System.Windows.Forms.TextFormatFlags.NoPrefix);
	//        //sz = System.Windows.Forms.TextRenderer.MeasureText(str, sdfont);
	//        //return sz.Width;
	//        return font.MeasureString(null, FontRenderer.ReplaceAccents(str), d3d.DrawTextFormat.None, sd.Color.Empty).Width;
	//        //this measurement is acting up :(
	//    }


	//}

    public class FontSW
    {
        public FontDefinition fontDefinition;
        public Image image;
        public FontSW(FontDefinition fd, Image img) { fontDefinition = fd; image = img; }

        public static FontSW v3import(string fname, bool bVariableWidth)
        {
            Image img = GameEngine.Game.LoadImage(fname);
            img.Alphafy(Color.Magenta);
            return new FontSW(V3FontDefinitionImporter.Import(img, bVariableWidth), img);
        }

        public static FontSW FizzImport(string fname, char startChar)
        {
            Image img = GameEngine.Game.LoadImage0(fname);
            return new FontSW(FizzFontDefinitionImporter.Import(img, startChar), img);
        }
    }

	public class FontDefinition
	{
		public Character[] characters = new Character[512];
		public struct Character
		{
			public int x, y, w, h;
			public int vx, vy, vw, vh;
		}
		public int defaultSpacing, defaultSpaceSize, Height;

		//public void load(Stream s)
		//{
		//    XDocument doc = XDocument.Parse(new StringReader().ReadToEnd());
		//    xmld.Load(s);
		//    XmlSerializer xmls = new XmlSerializer(typeof(Character[]));
		//    if (xmld.ChildNodes.Count == 0) throw new Exception("Malformed FontDefinition");
		//    if (xmld.ChildNodes[0].Name != "FontDefinition") throw new Exception("Malformed FontDefinition");
		//    if (xmld.ChildNodes[0].Attributes["version"].Value != "1") throw new Exception("Malformed FontDefinition");
		//    foreach (XmlAttribute xmla in xmld.ChildNodes[0].Attributes)
		//    {
		//        switch (xmla.Name)
		//        {
		//            case "defaultSpacing": defaultSpacing = int.Parse(xmla.Value); break;
		//            case "defaultSpaceSize": defaultSpaceSize = int.Parse(xmla.Value); break;
		//        }
		//    }
		//    foreach (XmlNode xmln in xmld.ChildNodes[0].ChildNodes)
		//    {
		//        switch (xmln.Name)
		//        {
		//            case "characters":
		//                {
		//                    XmlNodeReader xmlnr = new XmlNodeReader(xmln.FirstChild);
		//                    characters = (Character[])xmls.Deserialize(xmlnr);
		//                    break;
		//                }
		//        }
		//    }
		//}

		//public void save(Stream s)
		//{
		//    XmlWriter xmlw = new XmlTextWriter(new StreamWriter(s));
		//    xmlw.WriteStartElement("FontDefinition");
		//    xmlw.WriteAttributeString("version", "1");
		//    xmlw.WriteAttributeString("defaultSpacing", defaultSpacing.ToString());
		//    xmlw.WriteAttributeString("defaultSpaceSize", defaultSpaceSize.ToString());
		//    xmlw.WriteWhitespace("\r\n");
		//    xmlw.WriteStartElement("characters");
		//    XmlSerializer xmls = new XmlSerializer(typeof(Character[]));
		//    xmls.Serialize(xmlw, characters);
		//    xmlw.WriteEndElement();
		//    xmlw.WriteEndElement();
		//    xmlw.Flush();
		//}
	}

	//public class FontZ {
	//    public struct Character {
	//        public int x, y;
	//        public int width, height;
	//        public int logicalWidth, logicalHeight;
	//        public int logicalX, logicalY;
	//        public bool valid;
	//    }

	//    public Character[] characters = new Character[512];
	//    private readonly char[] characterLayout = new char[]
	//        {
	//            'A', 'B', 'C', 'D', 'E', 'F', 'G', 
	//            'H', 'I', 'J', 'K', 'L', 'M', 'N', 
	//            'O', 'P', 'Q', 'R', 'S', 'T', 'U', 
	//            'V', 'W', 'X', 'Y', 'Z', 'a', 'b',
	//            'c', 'd', 'e', 'f', 'g', 'h', 'i',
	//            'j', 'k', 'l', 'm', 'n', 'o', 'p',
	//            'q', 'r', 's', 't', 'u', 'v', 'w',
	//            'x', 'y', 'z'
	//        };

	//    public FontZ() {
	//        for(int i = 0; i < characters.Length; i++) {
	//            characters[i] = new Character();
	//            characters[i].valid = false;
	//        }
	//    }

	//    int _height;
	//    FunkImage _image;
	//    public FunkImage Image { get { return _image; } }
	//    public int Height { get { return _height; } }

	//    public FontZ(FunkImage img) {
	//        _image = img;
	//        int char_ofs = 0;
	//        int y_curs = 0;

	//        using(SoftimageWrapper siw = new SoftimageWrapper(img)) {
	//            Softimage si = siw.softimage;
	//            while(true) {
	//                // find first blue pixel
	//                int x = 0, y = y_curs;
	//                int bp = -1;
	//                int rh = -1;

	//                while(y < si.h) {
	//                    if((uint)si.getPixel(0, y) == 0xFF0000FF) {
	//                        if(bp == -1)
	//                            bp = y;
	//                        else {
	//                            rh = y - bp + 1;
	//                            break;
	//                        }
	//                    }
	//                    y++;
	//                }

	//                if(bp == -1 || rh == -1 || (bp + rh * 3) > si.h) {
	//                    break;
	//                }

	//                Console.WriteLine("row starts at {0}, row height is {1} and image height is {2}", bp, rh, si.h);

	//                Console.WriteLine("determining physical coordinates ... ");

	//                List<int> physXCoords = new List<int>();
	//                List<int> logicalXCoords = new List<int>();
	//                List<int> logicalYCoords = new List<int>();

	//                int physY1 = -1,
	//                    physY2 = -1,
	//                    logicalY1 = -1;

	//                x = 0;
	//                while(x < si.w) {
	//                    if((uint)si.getPixel(x, bp + 1) == 0xFF00FFFF)
	//                        physXCoords.Add(x);
	//                    if((uint)si.getPixel(x, bp) == 0xFFFF0000) {
	//                        logicalXCoords.Add(x);
	//                        if((logicalXCoords.Count & 1) == 1) {
	//                            int ly = -1;
	//                            y = bp + 2;
	//                            while(y < si.h) {
	//                                if((uint)si.getPixel(x, y) == 0xFFFF0000) {
	//                                    ly = y;
	//                                    break;
	//                                }
	//                                y++;
	//                            }
	//                            if(ly == -1) {
	//                                throw new Exception("Error parsing pixel data in font.  Did not find logical-y.");
	//                            }
	//                            logicalYCoords.Add(y);
	//                        }
	//                    }
	//                    x++;
	//                }

	//                y = bp;
	//                while(y <= bp + rh) {
	//                    if((uint)si.getPixel(2, y) == 0xFF00FFFF) {
	//                        if(physY1 == -1)
	//                            physY1 = y;
	//                        else if(physY2 == -1)
	//                            physY2 = y;
	//                    }
	//                    if((uint)si.getPixel(1, y) == 0xFFFF0000) {
	//                        if(logicalY1 == -1) {
	//                            logicalY1 = y;
	//                        }
	//                    }
	//                    y++;
	//                }

	//                if((physXCoords.Count & 1) == 1 ||
	//                    physXCoords.Count == 0 ||
	//                    (logicalXCoords.Count & 1) == 1 ||
	//                    logicalXCoords.Count == 0 ||
	//                    logicalXCoords.Count != physXCoords.Count ||
	//                    physY1 == -1 || physY2 == -1 || logicalY1 == -1)
	//                    throw new Exception("Unable to read font pixel data.");

	//                Console.WriteLine("Found {0} horizontal coordinates.", physXCoords.Count);
	//                _height = _height > (physY2 - physY1 + 1) ? _height : (physY2 - physY1 + 1);
	//                for(int i = 0; i < logicalXCoords.Count / 2; i++, char_ofs++) {
	//                    characters[characterLayout[char_ofs]].width = physXCoords[i * 2 + 1] - physXCoords[i * 2] + 1;
	//                    characters[characterLayout[char_ofs]].height = physY2 - physY1 + 1;
	//                    characters[characterLayout[char_ofs]].x = physXCoords[i * 2];
	//                    characters[characterLayout[char_ofs]].y = rh + physY1;
	//                    characters[characterLayout[char_ofs]].logicalWidth = logicalXCoords[i * 2 + 1] - logicalXCoords[i * 2];
	//                    characters[characterLayout[char_ofs]].logicalHeight = logicalYCoords[i] - logicalY1;
	//                    characters[characterLayout[char_ofs]].logicalX = logicalXCoords[i * 2] - physXCoords[i * 2];
	//                    characters[characterLayout[char_ofs]].logicalY = logicalYCoords[i] - rh;
	//                    characters[characterLayout[char_ofs]].valid = true;

	//                    characters[256 + characterLayout[char_ofs]].width = physXCoords[i * 2 + 1] - physXCoords[i * 2] + 1;
	//                    characters[256 + characterLayout[char_ofs]].height = physY2 - physY1 + 1;
	//                    characters[256 + characterLayout[char_ofs]].x = physXCoords[i * 2];
	//                    characters[256 + characterLayout[char_ofs]].y = rh * 2 + physY1;
	//                    characters[256 + characterLayout[char_ofs]].logicalWidth = logicalXCoords[i * 2 + 1] - logicalXCoords[i * 2];
	//                    characters[256 + characterLayout[char_ofs]].logicalHeight = logicalYCoords[i] - logicalY1;
	//                    characters[256 + characterLayout[char_ofs]].logicalX = logicalXCoords[i * 2] - physXCoords[i * 2];
	//                    characters[256 + characterLayout[char_ofs]].logicalY = logicalYCoords[i] - rh;
	//                    characters[256 + characterLayout[char_ofs]].valid = true;

	//                    Console.WriteLine("Building character {0}, width: {1}, height: {2}, x: {3}, y: {4}, yact: {5}", characterLayout[char_ofs], characters[characterLayout[char_ofs]].width, characters[characterLayout[char_ofs]].height, characters[256 + characterLayout[char_ofs]].x, characters[256 + characterLayout[char_ofs]].y, characters[256 + characterLayout[char_ofs]].y);
	//                }

	//                _height = _height > (physY2 - physY1 + 1) ? _height : (physY2 - physY1 + 1);
	//                y_curs = rh * 2 + physY2 + 1;
	//            }
	//        }
	//    }
	//}

	//public class FontRendererZ : FontRendererBase, IFontRenderer {
	//    public int spacing = 3;
	//    public int spaceSize = 12;
	//    public FontZ font;

	//    public FontRendererZ(FontZ font) { this.font = font; }
	//    public FontRendererZ() { }

	//    public static IEnumerable EnumerateCharacters(string str) {
	//        for(int i = 0; i < str.Length; i++) {
	//            char c = str[i];
	//            yield return c;
	//        }
	//    }

	//    public override int measureWidth(string str) {
	//        int count = 0;
	//        bool bTrailingSpacer = false;
	//        int i = 0;
	//        foreach(char c in EnumerateCharacters(str)) {
	//            if(c == ' ') { count += spaceSize; bTrailingSpacer = false; } else {
	//                if(i == 0)
	//                    count += font.characters[c].width;
	//                else if(i == str.Length - 1)
	//                    count += font.characters[c].logicalWidth + (font.characters[c].width - font.characters[c].logicalWidth) / 2;
	//                else
	//                    count += font.characters[c].logicalWidth;
	//                count += spacing;
	//                bTrailingSpacer = true;
	//            }
	//            i++;
	//        }
	//        if(bTrailingSpacer) count -= spacing;

	//        return count;
	//    }

	//    public unsafe override void render(Blitter b, int x, int y, string str) {
	//        b.beginBatch();

	//        FunkImage img = font.Image;
	//        FontZ.Character[] characters = font.characters;

	//        int startx = x;
	//        int starty = y;

	//        foreach(char c in EnumerateCharacters(str)) {
	//            int n = (int)c;
	//            if(c == ' ') x += spaceSize;
	//            else {
	//                int dx = x;
	//                int dy = y + height - characters[n].height;

	//                b.blitSubrectBatched(img, characters[n].x, characters[n].y, characters[n].width, characters[n].height, dx, dy);

	//                x += characters[n].logicalWidth;
	//                x += spacing;
	//            }
	//        }

	//        x = startx;
	//        y = starty;

	//        foreach(char c in EnumerateCharacters(str)) {
	//            int n = (int)c;
	//            if(c == ' ') x += spaceSize;
	//            else {
	//                int dx = x;
	//                int dy = y + height - characters[n].height;

	//                b.blitSubrectBatched(img, characters[256 + n].x, characters[256 + n].y, characters[256 + n].width, characters[256 + n].height, dx, dy);

	//                x += characters[256 + n].logicalWidth;
	//                x += spacing;
	//            }
	//        }

	//        b.executeSubrectBatch(img);
	//    }

	//    public int height { get { return font.Height; } }

	//}

	public class FizzFontDefinitionImporter {
		public static FontDefinition Import(Image img, char startChar) {
			using(Image.LockCache cache = img.LockReadonly()) {
				Softimage si = cache.Softimage;
				int col0 = si.GetPixel(0, 0);

				FontDefinition fd = new FontDefinition();
				fd.Height = si.Height-1;

				bool mode = false;
				int startx=0;
				int x = 0;
				while(x<si.Width) {
					x++;
					if(
						(!mode && si.GetPixel(x, 0) != col0)
						|| (mode && si.GetPixel(x, 0) == col0)
						) {
						mode = !mode;
						//this is the first pixel of a new character
						int idx = (int)startChar;
						startChar++;
						fd.characters[idx].x = startx;
						fd.characters[idx].y = 1;
						fd.characters[idx].vx = 0;
						fd.characters[idx].vy = 0;
						fd.characters[idx].w = x-startx;
						fd.characters[idx].h = fd.Height;
						fd.characters[idx].vw = x-startx;
						fd.characters[idx].vh = fd.Height;
						startx = x;
					}
				}

				return fd;
			}
		}
	}

	public class V3FontDefinitionImporter {
		static char[] tbl = new char[]{
	        (char)0,
	        '!','"','#','$','%','&','\'','(',
	        ')','*','+',',','-','.','/','0',
	        '1','2','3','4','5','6','7','8',
	        '9',':',';','<','=','>','?','@',
	        'A','B','C','D','E','F','G','H',
	        'I','J','K','L','M','N','O','P',
	        'Q','R','S','T','U','V','W','X',
	        'Y','Z','[','\\',']','^','_','`',
	        'a','b','c','d','e','f','g','h',
	        'i','j','k','l','m','n','o','p',
	        'q','r','s','t','u','v','w','x',
	        'y','z','{','|','}','~'			  };

		public static FontDefinition Import(Image img, bool bVariableWidth) {
			using(Image.LockCache cache = img.LockReadonly()) {
				Softimage si = cache.Softimage;
				int bgcol = si.GetPixel(0, 0);

				int w, h;
				for(w = 1;w < si.w;w++)
					if(bgcol == si.GetPixel(w, 1))
						break;
				for(h = 1;h < si.h;h++)
					if(bgcol == si.GetPixel(1, h))
						break;
				w--;
				h--;

				int subsets = si.Height / ((h * 5) + 4);

				FontDefinition fd = new FontDefinition();
				fd.Height = h;
				int y = 0, x = 0;
				for(int i = 0;i < tbl.Length;i++) {
					if(tbl[i] != 0) {
						fd.characters[(int)tbl[i]].x = 1 + x * (w + 1);
						fd.characters[(int)tbl[i]].y = 1 + y * (h + 1);
						fd.characters[(int)tbl[i]].vx = 0;
						fd.characters[(int)tbl[i]].vy = 0;
						fd.characters[(int)tbl[i]].w = w;
						fd.characters[(int)tbl[i]].h = h;
						fd.characters[(int)tbl[i]].vw = w;
						fd.characters[(int)tbl[i]].vh = h;
					}
					x++;
					if(x == 20) { y++; x = 0; }
				}

				if(bVariableWidth) {
					for(int i = 1;i < tbl.Length;i++) {
						int zx;
						for(zx = fd.characters[(int)tbl[i]].w - 1;zx >= 0;zx--) {
							int zy;
							for(zy = 0;zy < fd.characters[(int)tbl[i]].h;zy++) {
								int pixel = si.GetPixel(fd.characters[(int)tbl[i]].x + zx, fd.characters[(int)tbl[i]].y + zy);
								if((pixel & 0xFF000000) != 0)
									break;
							}
							if(zy != fd.characters[(int)tbl[i]].h)
								break;
						}
						fd.characters[(int)tbl[i]].vw = zx + 1;
					}
				}
				return fd;
			} //using softimagewrapper
		}

	}


}