//MEANING OF PADDING IS SUBJECT TO CHANGE
//architecture:
//a view can bind to a bitmap which it can then use to serve frames

#if !XBOX

using System;
using System.Collections.Generic;
using sd=System.Drawing;
using sdi=System.Drawing.Imaging;
using System.Windows.Forms;
using System.Xml;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace pr2.CarotEngine {

	/// <summary>
	/// Asset Description Markup Language
	/// </summary>
	public class ADML {

		//public interface IFrameServer {
		//    Image GetImage(int address, int frame);
		//}


		/// <summary>
		/// A basic character
		/// </summary>
		public class Character {
			public string name,source,animtype;
			public Color tcol;
			public Dictionary<string, View> views = new Dictionary<string,View>();
			public Dictionary<string, Anim> anims = new Dictionary<string, Anim>();
		}

		/// <summary>
		/// Represents an animation. Belongs to characters
		/// </summary>
		public class Anim : ICloneable {
			public string name;
			public Anim idle;
			public string addr, str;
			public View view;
			public Rectangle hotspot;
			public Character character;

			public object Clone() {
				Anim ret = new Anim();
				ret.name = name;
				ret.idle = idle;
				ret.addr = addr;
				ret.str = str;
				ret.view = view;
				ret.hotspot = hotspot;
				ret.character = character;
				return ret;
			}
		}

		/// <summary>
		/// Views belong to characters. for now there is only row view!
		/// </summary>
		public class View {
			public string name;
			public Padding pad;
			public Size dims;
			public Character character;

			object frameServer;
			public void SetFrameServer(object o) { frameServer = o; }
			public T GetFrameServer<T>() { return (T)frameServer; }
		}

		public virtual void trace(string format, params object[] args) { Console.WriteLine(format, args); }
		public virtual void error(string format, params object[] args) { throw new CompileException(string.Format(format,args)); }
		public virtual void error(Exception inner, string format, params object[] args) { throw new CompileException(string.Format(format, args),inner); }

		class CompileException : Exception { 
			public CompileException(string str) : base(str) { }
			public CompileException(string str, Exception inner) : base(str,inner) { }
		}

		string selectString(XmlNode node, string xpath) {
			XmlNode result = node.SelectSingleNode(xpath);
			if(result == null) return null;
			else return result.Value;
		}

		int[] parseIntList(string list) {
			return Array.ConvertAll<string, int>(list.Split(','), delegate(string str) { return int.Parse(str); });
		}

		public class CharacterSet : Dictionary<string, Character> {
			public Character First;
		}

		public static CharacterSet compile(string xml) { return new ADML()._compile(xml); }

		public CharacterSet _compile(string xml) {

			CharacterSet characters = new CharacterSet();

			try {

				XmlDocument xmldoc = new XmlDocument();
				xmldoc.LoadXml(xml);

				//query characters
				int charctr = 0;
				foreach(XmlNode xcharacter in xmldoc.DocumentElement.SelectNodes("character")) {
					charctr++;
					Character character = new Character();
					if(charctr==1)
						characters.First = character;

					character.name = selectString(xcharacter, "@name");
					character.source = selectString(xcharacter, "@source");
					character.animtype = selectString(xcharacter, "@animtype")??"v3";
					string tcol = selectString(xcharacter, "@tcol");
					string char_hotspot = selectString(xcharacter, "@hotspot");

					
					//validate required fields
					if(string.IsNullOrEmpty(character.name)) 
						if(string.IsNullOrEmpty(character.source))
							error("Character {0} has no name or source", charctr);
						else 
							error("Character {0} (with source `{1}`) has no name", charctr, character.source);
					//(validate name uniqueness)
					if(characters.ContainsKey(character.name)) error("Character {0} has a duplicate name [{1}]",charctr,character.name);
					if(string.IsNullOrEmpty(character.source)) error("Character `{0}` has no source", character.name);
					if(string.IsNullOrEmpty(tcol)) error("Character `{0}` has no tcol", character.name);

					//parse the tcol
					try {
						int[] components = parseIntList(tcol);
						character.tcol = new Color((byte)components[0], (byte)components[1], (byte)components[2]);
					} catch(Exception ex) { error(ex,"Malformed tcol in character `{0}` [{1}]", character.name,tcol); }

					//add the character
					characters[character.name] = character;


					//query views
					int viewctr = 0;
					foreach(XmlNode xview in xcharacter.SelectNodes("view")) {
						viewctr++;
						View view = new View();
						view.character = character;

						view.name = selectString(xview, "@name");
						string type = selectString(xview, "@type");
						string pad = selectString(xview, "@pad");
						string dims = selectString(xview, "@dims");
						
						//validate required fields
						if(string.IsNullOrEmpty(view.name)) error("View {0} of character `{1}` has no name",viewctr,character.name);
						//(validate name uniqueness)
						if(character.views.ContainsKey(view.name)) error("View {0} of character `{1}` has a duplicate name [{2}]",viewctr,character.name,view.name);
						if(string.IsNullOrEmpty(type)) error("View `{0}` of character `{1}` has no type",view.name,character.name);
						if(string.IsNullOrEmpty(pad)) error("View `{0}` of character `{1}` has no pad", view.name, character.name);
						if(string.IsNullOrEmpty(dims)) error("View `{0}` of character `{1}` has no dims", view.name, character.name);

						//parse the padding
						try {
							int[] components = parseIntList(pad);
							view.pad = new Padding(components[0], components[1], components[2], components[3]);
						} catch(Exception ex) { error(ex, "Malformed pad in view `{0}` [{1}]", view.name, pad); }

						//parse the dims
						try {
							int[] components = parseIntList(dims);
							view.dims = new Size(components[0], components[1]);
						} catch(Exception ex) { error(ex, "Malformed dims in view `{0}` [{1}]", view.name, dims); }

						//add the view
						character.views[view.name] = view;
					}

					//ensure theres a default view
					if(!character.views.ContainsKey("default")) error("No default view in character `{0}`; sorry, right now this is necessary!", character.name);

					//tracks all the idles for resolution after all the anims are parsed
					Dictionary<Anim, string> _idles = new Dictionary<Anim, string>();

					//query anims
					int animctr = 0;
					foreach(XmlNode xanim in xcharacter.SelectNodes("anim")) {
						animctr++;
						Anim anim = new Anim();
						anim.character = character;

						anim.name = selectString(xanim, "@name");
						anim.addr = selectString(xanim, "@addr");
						anim.str = selectString(xanim, "@str");
						string idlename = selectString(xanim, "@idle");
						if(!string.IsNullOrEmpty(idlename)) _idles[anim] = idlename;
						string strhotspot = selectString(xanim, "@hotspot") ?? char_hotspot;

						//parse the hotspot
						if(!string.IsNullOrEmpty(strhotspot)) {
							try {
								int[] components = parseIntList(strhotspot);
								anim.hotspot = new Rectangle(components[0], components[1], components[2], components[3]);
							} catch(Exception) { error("Anim `{0}` of character `{1}` has invalid hotspot", anim.name, character.name); }
						}

						//validate required fields
						if(string.IsNullOrEmpty(anim.name)) error("Anim {0} of character `{1}` has no name", animctr, character.name);
						//(validate name uniqueness)
						if(character.anims.ContainsKey(anim.name)) error("Anim {0} of character `{1}` has a duplicate name [{2}]",animctr,character.name,anim.name);
						if(string.IsNullOrEmpty(anim.addr)) error("Anim `{0}` of character `{1}` has no addr", anim.name, character.name);
						if(string.IsNullOrEmpty(anim.str)) error("Anim `{0}` of character `{1}` has no str", anim.name, character.name);

						//resolve the view
						//for now it had better always be the default view
						anim.view = character.views["default"];

						//add the view
						character.anims[anim.name] = anim;
					}

					//resolve the idle names
					foreach(KeyValuePair<Anim, string> kvp in _idles)
						if(character.anims.ContainsKey(kvp.Value))
							kvp.Key.idle = character.anims[kvp.Value];
						else
							error("Anim `{0}` points to a nonexisting idle anim [{1}]", kvp.Key.name, kvp.Value);
				}
			} catch(CompileException cex) {
				trace("ADML error: " + cex.Message);
				throw cex;
			}

			return characters;
		}
	}
}

#endif