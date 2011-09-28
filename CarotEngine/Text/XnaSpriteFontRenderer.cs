using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using pr2;

namespace pr2.CarotEngine {
	public class XnaSpriteFontRenderer : FontRendererBase, IFontRenderer {
		SpriteFont sf;
		public XnaSpriteFontRenderer(string resfile) { this.sf = GameEngine.Game.Content.Load<SpriteFont>(resfile); Init();  }
		public XnaSpriteFontRenderer(SpriteFont sf) { this.sf = sf; Init(); }
		public int height { get { return sf.LineSpacing; } }
		public override int MeasureWidth(string str) {
			return (int)MeasureString(str).X;
		}

		public int ExcessKerning;

		class EmbeddedCommand {
			public int index;
			public string command;
		}

		public Vector2 MeasureString(string text) {
			ProcessedString ps = new ProcessedString(text);
			text = ps.String;
			if(text == null) {
				throw new ArgumentNullException("text");
			}
			if(text.Length == 0) {
				return Vector2.Zero;
			}
			Vector2 zero = Vector2.Zero;
			zero.Y = sf.LineSpacing;
			float num4 = 0f;
			int num3 = 0;
			float z = 0f;
			bool flag = true;
			for(int i = 0; i < text.Length; i++) {
				if(text[i] != '\r') {
					if(text[i] == '\n') {
						//mbg hack: remove the kerning for a ' ' to prevent the last character from wasting space
						zero.X -= ExcessKerning;
						zero.X += Math.Max(z, 0f);
						z = 0f;
						num4 = Math.Max(zero.X, num4);
						zero = Vector2.Zero;
						zero.Y = sf.LineSpacing;
						flag = true;
						num3++;
					} else {
						Vector3 vector2 = this.kerning[this.GetIndexForCharacter(text[i])];
						if(flag) {
							vector2.X = Math.Max(vector2.X, 0f);
						} else {
							zero.X += sf.Spacing + z;
						}
						zero.X += vector2.X + vector2.Y;
						z = vector2.Z;
						Rectangle rectangle = this.croppingData[this.GetIndexForCharacter(text[i])];
						zero.Y = Math.Max(zero.Y, (float)rectangle.Height);
						flag = false;
					}
				}
			}
			zero.X -= ExcessKerning;
			zero.X += Math.Max(z, 0f);
			zero.Y += num3 * sf.LineSpacing;
			zero.X = Math.Max(zero.X, num4);


			return zero;
		}

		class ProcessedString {

			static Regex rxProcess = new Regex("<(.*?)>", RegexOptions.Compiled);

			public ProcessedString(string str) {
				//process the string
				for(; ; ) {
					Match m = rxProcess.Match(str);
					if(!m.Success) break;
					EmbeddedCommand ec = new EmbeddedCommand();
					ec.index = m.Index;
					ec.command = m.Groups[1].Value;
					Commands.Add(ec);
					str = str.Substring(0, m.Index) + str.Substring(m.Index + m.Length, str.Length - (m.Index + m.Length));
				}
				String = str;
			}
			public List<EmbeddedCommand> Commands = new List<EmbeddedCommand>();
			public string String;
		}

		public override void Render(Blitter b, int x, int y, string str) {

			ProcessedString ps = new ProcessedString(str);

			int curCmd = ps.Commands.Count>0?0:-1;

			bool modulate = b.EnableModulate;
			b.EnableModulate = true;
			Color col = b.Color;
			b.Color = _color;
			Image wrapImage = Image.GetWrapper(textureValue);
			Vector2 textblockPosition = new Vector2(x, y);
			Vector2 vector2 = new Vector2();
			//Matrix matrix;
			//Matrix matrix2;
			//if(str == null) {
			//	throw new ArgumentNullException("text");
			//}
			//if(spriteBatch == null) {
			//	throw new ArgumentNullException("spriteBatch");
			//}
			//Matrix.CreateRotationZ(rotation, out matrix2);
			//Matrix.CreateTranslation(-origin.X * scale.X, -origin.Y * scale.Y, 0f, out matrix);
			//Matrix.Multiply(ref matrix, ref matrix2, out matrix2);
			int num2 = 1;
			float num4 = 0f;
			bool flag = true;
			//if((spriteEffects & SpriteEffects.FlipHorizontally) == SpriteEffects.FlipHorizontally) {
			//    num4 = sf.MeasureString(text).X * scale.X;
			//    num2 = -1;
			//}
			//if((spriteEffects & SpriteEffects.FlipVertically) == SpriteEffects.FlipVertically) {
			//    vector2.Y = (sf.MeasureString(text).Y - sf.LineSpacing) * scale.Y;
			//} else {
				vector2.Y = 0f;
			//}
			vector2.X = num4;
			for(int i = 0; i < ps.String.Length; i++) {
				//execute commands
				while(curCmd != -1 && curCmd < ps.Commands.Count && i == ps.Commands[curCmd].index) {
					EmbeddedCommand cmd = ps.Commands[curCmd];
					curCmd++;
					if(cmd.command.StartsWith("col")) {
						string whichColor = cmd.command.Split('=')[1];
						if(whichColor == "def")
							b.Color = _color;
						else 
							b.Color = Text.Colors[whichColor];
					}
				}
				char character = ps.String[i];
				switch(character) {
					case '\r':
						break;

					case '\n':
						flag = true;
						vector2.X = num4;
						//if((spriteEffects & SpriteEffects.FlipVertically) == SpriteEffects.FlipVertically) {
						//    vector2.Y -= sf.LineSpacing * scale.Y;
						//} else {
						//    vector2.Y += sf.LineSpacing * scale.Y;
						//}
						vector2.Y += sf.LineSpacing;
						break;

					default: {
							int indexForCharacter = this.GetIndexForCharacter(character);
							Vector3 vector3 = this.kerning[indexForCharacter];
							if(flag) {
								vector3.X = Math.Max(vector3.X, 0f);
							} else {
								//vector2.X += (sf.Spacing * scale.X) * num2;
								vector2.X += (sf.Spacing) * num2;
							}
							//vector2.X += (vector3.X * scale.X) * num2;
							vector2.X += (vector3.X) * num2;
							Rectangle rectangle = this.glyphData[indexForCharacter];
							Rectangle rectangle2 = this.croppingData[indexForCharacter];
							//if((spriteEffects & SpriteEffects.FlipVertically) == SpriteEffects.FlipVertically) {
							//    rectangle2.Y = (sf.LineSpacing - rectangle.Height) - rectangle2.Y;
							//}
							//if((spriteEffects & SpriteEffects.FlipHorizontally) == SpriteEffects.FlipHorizontally) {
							//    rectangle2.X -= rectangle2.Width;
							//}
							Vector2 position = vector2;
							//position.X += rectangle2.X * scale.X;
							//position.Y += rectangle2.Y * scale.Y;
							position.X += rectangle2.X;
							position.Y += rectangle2.Y;
							//Vector2.Transform(ref position, ref matrix2, out position);
							position += textblockPosition;
							//spriteBatch.Draw(this.textureValue, position, new Rectangle?(rectangle), color, rotation, Vector2.Zero, scale, spriteEffects, depth);
							b.BlitSubrect(wrapImage, rectangle.Left, rectangle.Top, rectangle.Width, rectangle.Height, (int)position.X, (int)position.Y);
							flag = false;
							//vector2.X += ((vector3.Y + vector3.Z) * scale.X) * num2;
							vector2.X += ((vector3.Y + vector3.Z)) * num2;
							break;
						}
				}
			}

			b.Color = col;
			b.EnableModulate = modulate;
		}

		void Init() {
			Type t = typeof(SpriteFont);
			System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
			d_GetIndexForCharacter = (D_GetIndexForCharacter)Delegate.CreateDelegate(typeof(D_GetIndexForCharacter), sf, t.GetMethod("GetIndexForCharacter", flags));
			kerning = (List<Vector3>)t.GetField("kerning", flags).GetValue(sf);
			glyphData = (List<Rectangle>)t.GetField("glyphData", flags).GetValue(sf);
			textureValue = (Texture2D)t.GetField("textureValue", flags).GetValue(sf);
			croppingData = (List<Rectangle>)t.GetField("croppingData", flags).GetValue(sf);
		}

		List<Rectangle> croppingData;
		List<Rectangle> glyphData;
		List<Vector3> kerning;
		public Texture2D textureValue;
		delegate int D_GetIndexForCharacter(char character);
		D_GetIndexForCharacter d_GetIndexForCharacter;
		private int GetIndexForCharacter(char character) {
			return d_GetIndexForCharacter(character);
		}


		internal void InternalDraw(string text, SpriteBatch spriteBatch, Vector2 textblockPosition, Color color, float rotation, Vector2 origin, ref Vector2 scale, SpriteEffects spriteEffects, float depth) {
		}


	}


}