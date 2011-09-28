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


namespace Cabedge {
	public partial class Cabedge {


		abstract class RectangleRenderer {
			public abstract void render(Blitter b, Rectangle area);
		}

		class Chunk {
			public Chunk Clone() {
				Chunk c = new Chunk();
				c.tile = tile;
				c.rect = rect;
				c._isEmpty = _isEmpty;
				return c;
			}
			public Chunk(int x, int y, int w, int h) { this.rect = new Rectangle(x, y, w, h); }
			public Chunk() { this.rect = new Rectangle(0, 0, 0, 0); _isEmpty = true; }
			public Rectangle rect;
			public bool tile; //if its false, stretch
			public int w { get { return rect.Width; } }
			public int h { get { return rect.Height; } }
			public static Chunk empty = new Chunk();
			public bool isEmpty { get { return _isEmpty; } }
			bool _isEmpty;
		}

		class FrameRenderer : RectangleRenderer {
			public Chunk NW, W, SW, S, SE, E, NE, N;
			Image img;

			public FrameRenderer clone() {
				return new FrameRenderer(this);
			}

			/// <summary>
			/// clones!
			/// </summary>
			public FrameRenderer(FrameRenderer fr) {
				NW = fr.NW.Clone();
				W = fr.W.Clone();
				SW = fr.SW.Clone();
				S = fr.S.Clone();
				SE = fr.SE.Clone();
				E = fr.E.Clone();
				NE = fr.NE.Clone();
				N = fr.N.Clone();
				img = fr.img;
			}

			/// <summary>
			/// Make a new frame renderer exactly the same as this one but offset a bit
			/// </summary>
			public FrameRenderer offset(int x, int y) {
				FrameRenderer ret = new FrameRenderer(this);
				ret.NW.rect.Offset(x, y);
				ret.W.rect.Offset(x, y);
				ret.SW.rect.Offset(x, y);
				ret.S.rect.Offset(x, y);
				ret.SE.rect.Offset(x, y);
				ret.E.rect.Offset(x, y);
				ret.NE.rect.Offset(x, y);
				ret.N.rect.Offset(x, y);
				return ret;
			}

			public FrameRenderer(Image img, Chunk[] parts) {
				this.img = img;
				NW = parts[0];
				W = parts[1];
				SW = parts[2];
				S = parts[3];
				SE = parts[4];
				E = parts[5];
				NE = parts[6];
				N = parts[7];
			}
			public override void render(Blitter b, Rectangle area) {
				int w = area.Width;
				int h = area.Height;


				b.ApplyWindow(area);
				b.BeginBatch();
				//are any of the empty checks necessary?
				if(!NW.isEmpty) b.BlitSubrectBatched(img, NW.rect, 0, 0);
				if(!W.isEmpty) b.BlitSubrectBatched(img, W.rect, 0, NW.h, W.w, h-NW.h-SW.h);
				if(!SW.isEmpty) b.BlitSubrectBatched(img, SW.rect, 0, h - SW.h);
				if(!S.isEmpty) b.BlitSubrectBatched(img, S.rect, SW.w, h-S.h, w - SW.w-SE.w, S.h);
				if(!SE.isEmpty) b.BlitSubrectBatched(img, SE.rect, w-SE.w, h-SE.h);
				if(!E.isEmpty) b.BlitSubrectBatched(img, E.rect, w-E.w, NE.h, E.w, h-NE.h - SE.h);
				if(!NE.isEmpty) b.BlitSubrectBatched(img, NE.rect, w-NE.w, 0);
				if(!N.isEmpty) b.BlitSubrectBatched(img, N.rect, NW.w, 0, w - NW.w - NE.w, N.h);
				b.ExecuteSubrectBatch(img);
				b.PopWindow();
			}
		}

		//TODO LATER - tiling? support for tiny windows? this will freak out if its too small
		class WindowRenderer {

			FrameRenderer frame;
			FrameRenderer frame_character_selected;

			public WindowRenderer() {
				Chunk[] bigFrame = new Chunk[] {
						new Chunk(72, 25, 4, 6), //nw
						new Chunk(72, 36, 4, 10), //w
						new Chunk(72, 49, 4, 6), //sw
						new Chunk(82, 51, 82, 4), //s
						new Chunk(170, 49, 4, 6), //se
						new Chunk(170, 36, 4, 10), //e
						new Chunk(170, 25, 4, 6), //ne
						new Chunk(82, 25, 82, 4), //n
					};

				frame = new FrameRenderer(cabedge.img_ui, bigFrame);

				//selected character is same as basic frame, but with a different end piece
				frame_character_selected = new FrameRenderer(cabedge.img_ui, bigFrame);
				frame_character_selected.NE = new Chunk(142, 29, 7, 10);
				frame_character_selected.E = new Chunk(142, 39, 7, 2);
				frame_character_selected.SE = new Chunk(142, 41, 7, 10);
			}

			public enum Mode {
				Light, Medium, Dark, Selected, BigSelected
			}

			public void render(Blitter b, Rectangle area, Mode mode) {
				int w = area.Width;
				int h = area.Height;

				Image img = cabedge.img_ui;

				//adjustments for alternate interior borders
				int int_light_adj = 0;
				int int_mid_adj = 13;
				int int_dark_adj = 27;
				int int_sel_adj = 33;

				Color int_light_color = new Color(232, 232, 232);
				Color int_mid_color = new Color(185, 185, 185);
				Color int_dark_color = new Color(98, 98, 98);
				Color int_sel_color = new Color(225, 255, 209);

				Rectangle int_top = new Rectangle(105, 33, 2, 3);
				Rectangle int_bot = new Rectangle(105, 44, 2, 3);

				//pick one
				int int_adj;
				Color int_color;
				switch(mode) {
					case Mode.Light: int_adj = int_light_adj; int_color = int_light_color; break;
					case Mode.Medium: int_adj = int_mid_adj; int_color = int_mid_color; break;
					case Mode.Dark: int_adj = int_dark_adj; int_color = int_dark_color; break;
					case Mode.Selected:
					case Mode.BigSelected:
						int_adj = int_sel_adj;
						int_color = int_sel_color;
						break;
					default: throw new ArgumentException();
				}

				int_top.Offset(int_adj, 0);
				int_bot.Offset(int_adj, 0);

				if(mode == Mode.BigSelected)
					frame_character_selected.render(b, area);
				else
					frame.render(b, area);

				b.ApplyWindow(area);
				b.BeginBatch();
				if(mode == Mode.BigSelected) {
					b.BlitSubrectBatched(img, int_top, 4, 4, w-11, 3); //use these for bulbuous piece
					b.BlitSubrectBatched(img, int_bot, 4, h-4-3, w-11, 3);
				} else {
					b.BlitSubrectBatched(img, int_top, 4, 4, w-8, 3);
					b.BlitSubrectBatched(img, int_bot, 4, h-4-3, w-8, 3);
				}
				b.ExecuteSubrectBatch(img);

				b.Color = int_color;
				if(mode == Mode.BigSelected)
					b.RectFill(4, 7, w-10, h-14);  //use these for bulbuous piece
				else
					b.RectFill(4, 7, w-8, h-14);

				b.PopWindow();
			}
		}


		class MeterRenderer {
			/// <summary>
			/// level can be 0..17. lets let 17 be 100% full, anything less is not full. 0 is totally unfull. i think this leaves us a range of 15?
			/// </summary>
			public void render(Blitter b, Rectangle area, int level) {
				Rectangle meter_fill = new Rectangle(35, 32, 5, 1);
				Rectangle meter_top = new Rectangle(35, 29, 5, 3);

				int meter_green = 0;
				int meter_red = 7;
				int meter_orange = 14;

				//adjust for color

				b.ApplyWindow(area);
				b.BlitSubrect(cabedge.img_ui, 16, 27, 11, 22, 0, 0);
				if(level>0) {
					//the top of the meter shall be at the meter level
					int top_height = level;
					if(top_height > 3) top_height = 3;
					b.BlitSubrect(cabedge.img_ui, meter_top.X, meter_top.Y, meter_top.Width, top_height, 3, 3+17-level);
					//the remainder of the meter shall be beneath the meter top part
					int fill_height = level - 3;
					if(fill_height > 0)
						b.BlitSubrect(cabedge.img_ui, meter_fill.X, meter_fill.Y, meter_fill.Width, meter_fill.Height, 3, 3+17-fill_height, 5, fill_height);
				}

				b.PopWindow();
			}
		}

	}
}