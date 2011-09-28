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

//           Action Safe         Title Safe 
//           vertical horizontal vertical horizontal 
//4:3 SD     3.5%     3.3%       5.0%     6.7%         (total dimension)
//6x4 margin                     13       21           (offset)


namespace Cabedge {
	public partial class Cabedge {

		protected Image img_ui;

		public class UiController {

			public UiController() {
				initMenu();
			}

			/// <summary>
			/// renders a 100-wide name panel
			/// </summary>
			class CharacterListRenderer {
				MeterRenderer mr = new MeterRenderer();
				WindowRenderer wr = new WindowRenderer();
				public void render(Blitter b, Point pt, bool selected, int level, string name) {
					int width = 100;
					int nameWidth = 89;
					int meterWidth = 11;
					int nameStart = meterWidth;
					int height = 22;
					
					b.ApplyWindow(pt.X, pt.Y);
					mr.render(b, new Rectangle(0, 0, meterWidth, height), level);
					if(selected)
						wr.render(b, new Rectangle(nameStart, 0, nameWidth+SelectionOverhang, height), WindowRenderer.Mode.BigSelected);
					else
						wr.render(b, new Rectangle(nameStart, 0, nameWidth, height), WindowRenderer.Mode.Dark);
					cabedge.font_v3_1_black.Render(b, nameStart+6, 8, name);
					b.PopWindow();
				}
			}

			protected static int SelectionOverhang = 16;

			public enum State {
				NoMenu, MainMenu, TextBox
			}
			public State state;

			WindowRenderer wr = new WindowRenderer();
			CharacterListRenderer clr = new CharacterListRenderer();


			class MainMenu : Menu {
			}

			class PartySubMenu : LabelMenu {
				public PartySubMenu(string label) : base(label) { isEnterable = true; }
				public Action delActivate;
				public override void activate() {
					if(delActivate != null)
						delActivate();
				}
			}

			class EnhancementSubMenu : LabelMenu {
				public Enhancement enh;
				public EnhancementSubMenu(Enhancement enh) : base(enh.name) { this.enh = enh; }
				public EnhancementSubMenu() : base("---") { this.enh = null; }
				public override void activate() {
					cabedge.gameController.uiController.textBoxOpen();
					cabedge.gameController.uiController.textBoxText("you clicked on an enhancement",false);
				}
			}

			class EnhancementsSubMenu : LabelMenu {
				public EnhancementsSubMenu()
					: base("Enhancements") {
					isEnterable = true;
				}

				internal override void onPopulate() {
					items.Clear();
					foreach(Enhancement enh in cabedge.party.equippedEnhancements) {
						if(enh == null)
							items.Add(new EnhancementSubMenu());
						else
							items.Add(new EnhancementSubMenu(enh));
					}
				}
			}
			
			class PartyRootMenu : Menu  {
				public PartyRootMenu() {
					inventory = new PartySubMenu("Inventory");
					enhancements = new EnhancementsSubMenu();
					configuration = new PartySubMenu("Configuration");
					items.Add(inventory); items.Add(enhancements); items.Add(configuration);
					isEnterable = true;
				}

				public Menu inventory, enhancements, configuration;
			}

			class CharacterMenu : Menu {
				public Character character;
				public CharacterMenu(Character c) {
					character = c;
				}
				internal override void onPopulate() {
					items.Clear();
					items.Add(new LabelMenu("Magic"));
					items.Add(new LabelMenu("Status"));
				}
			}
			
			PartyRootMenu partyRoot;

			//initializes static menu components
			void initMenu() {
				partyRoot = new PartyRootMenu();
			}

			MainMenu generateMainMenu() {
				MainMenu mainMenu = new MainMenu();
				mainMenu.items.Add(partyRoot);
				foreach(Character c in cabedge.party.chars) {
					mainMenu.items.Add(new CharacterMenu(c));
				}
				return mainMenu;
			}

			void renderEnhancementsMenu(Blitter b) {
				//render the tooltip
				Enhancement selEnh = null;
				if(mainMenuNavigator.submenuAt(2) != null)
					selEnh = (mainMenuNavigator.submenuAt(2) as EnhancementSubMenu).enh;

				if(selEnh != null) {
					b.PushWindow();

					int targetWidth = 140;
					V3Sysfont.StringLayoutInfo sli = cabedge.font_v3_1_black.layoutRectangle(targetWidth-25, -1, selEnh.tooltip);

					b.OffsetWindow(140+1, 33 + locateSubmenu(b.window, mainMenuNavigator.selectionAt(2), 16, sli.size.Height + 20));
					wr.render(b, new Rectangle(0, 0, 25+sli.size.Width+10, 10+sli.size.Height+10), WindowRenderer.Mode.Light);
					cabedge.font_v3_1_black.renderLayout(b, 25, 10, sli);
					b.PopWindow();
				}

				b.PushWindow();
				wr.render(b, new Rectangle(0, 0, 140, 33), WindowRenderer.Mode.Light);
				cabedge.font_v3_2_black.Render(b, 25, 10, "Equipped");
				b.OffsetWindow(0, 33);
				renderItemList(b, partyRoot.enhancements.items, mainMenuNavigator.selectionAt(2), selEnh != null, 130);
				b.PopWindow();
			}

			void renderPartyRoot(Blitter b, bool selActive) {
				b.PushWindow();
				wr.render(b, new Rectangle(0, 0, 200, 66), WindowRenderer.Mode.Light);
				cabedge.font_v3_2_black.Render(b, 25, 15, "XP");
				cabedge.font_v3_2_black.Render(b, 25, 35, "GP");
				cabedge.font_v3_2_black.Render(b, 55, 15, string.Format("{0}/{1}",cabedge.party.xp,cabedge.party.xpt));
				cabedge.font_v3_2_black.Render(b, 55, 35, cabedge.party.gp.ToString());
				cabedge.font_v3_1_black.RenderRight(b, 192, 10, "Level");
				cabedge.font_v3_3_black.RenderRight(b, 192, 20, cabedge.party.level.ToString());
				b.OffsetWindow(0,66);
				renderItemList(b, partyRoot.items, mainMenuNavigator.selectionAt(1), selActive, 190);
				b.PopWindow();
			}

			void renderPlayerRoot(Blitter b) {
				Character c = (mainMenuNavigator.submenuAt(0) as CharacterMenu).character;
				b.PushWindow();
				wr.render(b, new Rectangle(0, 0, 200, 66), WindowRenderer.Mode.Light);
				cabedge.font_v3_2_black.Render(b, 25, 12, c.name);
				cabedge.font_v3_1_black.Render(b, 25, 32, string.Format("{0} / {1}", c.hp, c.baseStats.mhp + c.bonusStats.mhp));
				cabedge.font_v3_1_black.Render(b, 25, 42, c.classname);
				Image portrait = c.getPortrait();
				if(portrait != null)
					b.Blit(portrait, 160, 10);
				b.PopWindow();
			}

			int locateSubmenu(Rectangle rect, int selection, int itemHeight, int submenuHeight) {
				int temp = selection * itemHeight - submenuHeight/2 + itemHeight/2;
				//dont let it go too high
				int yabs = rect.Top + temp;
				if(yabs < 0) temp = 0;
				return temp;
			}
			
			public void renderMainMenu(Blitter b) {

				//title safe area offsets
				int mxofs = 20;
				int myofs = 20;

				int listwidth = 100;
				int rootMenuHeight = 66;

				listwidth++; //leave a gap before the next menus

				//title safe area
				b.ApplyWindow(mxofs, myofs, 640-mxofs*2, 480-myofs*2);

				//identify location of root menu
				int rootMenuVertOfs = locateSubmenu(b.window, mainMenuNavigator.selectionAt(0), 22, rootMenuHeight);

				//top-level party menu
				if(mainMenuNavigator.submenuAt(0) is PartyRootMenu) {
					b.PushWindow();
					b.OffsetWindow(listwidth, rootMenuVertOfs);

					bool selectionA = false;
					//if(mainMenuNavigator.menuAt(2) != null) {
					if(mainMenuNavigator.selectionAt(1) == 1) {
						selectionA = true;
						int submenuOffset = 200+1;
                        b.PushWindow();
                        b.OffsetWindow(submenuOffset, rootMenuHeight + locateSubmenu(b.window, mainMenuNavigator.selectionAt(1), 16, 33));
						renderEnhancementsMenu(b);
                        b.PopWindow();
					}
					
					renderPartyRoot(b, selectionA);
					b.PopWindow();
				} else {
					b.PushWindow();
					b.OffsetWindow(listwidth, rootMenuVertOfs);
					renderPlayerRoot(b);
                    b.PopWindow();
				}

				//--------------
				//main list

				//party item in main list
				if(mainMenuNavigator.submenuAt(0) is PartyRootMenu)
					wr.render(b, new Rectangle(0, 0, 100+SelectionOverhang, 22), WindowRenderer.Mode.BigSelected);
				else
					wr.render(b, new Rectangle(0, 0, 100, 22), WindowRenderer.Mode.Medium);

				string partyText = string.Format("P A R T Y  Lv. {0}",cabedge.party.level);
				cabedge.font_v3_1_black.Render(b, 7, 8, partyText);

				int level = Lib.TimerSin((int)DateTime.Now.Ticks, 12000, 8) + 8;

				int yofs = 22;
				for(int i=0;i<cabedge.party.chars.Count;i++) {
					clr.render(b, new Point(0, yofs), i+1==mainMenuNavigator.selectionAt(0), level, cabedge.party.chars[i].name);
					yofs+=22;
				}

				b.PopWindow();
				//----------------
			}


			MenuNavigator mainMenuNavigator;
			
			/// <summary>
			/// this is called from the game controller when we should enter the main menu
			/// </summary>
			public void enterMainMenu() {
				state = State.MainMenu;
				mainMenuNavigator = new MenuNavigator();
				mainMenuNavigator.root = generateMainMenu();
				mainMenuNavigator.start();
				mainMenuNavigator.activate();
			}

			public enum TextBoxState {
				Opening, Drawing, Scrolling, Waiting, Closing, Closed
			}

			TextBoxState tb_state;
			string tb_text;
			bool tb_more;
			
			public void textBoxOpen() {
				state = State.TextBox;
				tb_state = TextBoxState.Opening;
			}
			public void textBoxClose() {
				tb_state = TextBoxState.Closed;
				state = State.NoMenu;
			}

			public void textBoxText(string text, bool more) {
				tb_more = more;
				tb_text = text;
				tb_state = TextBoxState.Waiting;
			}

			public bool isTextBoxWaiting() { return tb_state == TextBoxState.Waiting; }

			public TextBoxState textBoxQuery() {
				return tb_state;
			}

			
			void renderItemList(Blitter b, List<Menu> items, int selIndex, bool selActive, int width) {
				int indent = 10;

				//indent the list
				b.OffsetWindow(indent, 0);
				
				int overallWidth = width + SelectionOverhang;

				//the tie
				FrameRenderer fr = new FrameRenderer(cabedge.img_ui, new Chunk[]{
				    new Chunk(75, 82, 4, 3),
				    new Chunk(75, 86, 4, 42),
				    new Chunk(75, 129, 4, 3),
				    new Chunk(80, 131, 1, 1),
				    Chunk.empty,
				    Chunk.empty,
				    Chunk.empty,
				    new Chunk(80, 82, 1, 1)});

				//the bar
				FrameRenderer fr_gray = new FrameRenderer(cabedge.img_ui, new Chunk[]{
				    new Chunk(94, 83, 5, 6),
				    new Chunk(94, 90, 5, 1),
				    new Chunk(94, 92, 5, 6),
				    new Chunk(100, 92, 1, 6),
				    new Chunk(102, 92, 5, 6),
				    new Chunk(102, 90, 5, 1),
				    new Chunk(102, 83, 5, 6),
				    new Chunk(100, 83, 1, 6)});

				FrameRenderer fr_sel = fr_gray.offset(0, 17);

				//the selActive renderer is same as green but with an endcap
				FrameRenderer fr_selActive = fr_sel.clone();
				fr_selActive.NE = new Chunk(108, 100, 5, 5);
				fr_selActive.E = new Chunk(108, 106, 5, 3);
				fr_selActive.SE = new Chunk(108, 110, 5, 5);

				//render the tie (the left,top,and bottom border)
				int tie_height = 16*items.Count+2;
				fr.render(b,new Rectangle(0,0,width,tie_height));

				//render items
				for(int i=0;i<items.Count;i++) {
					int x = 4;
					int y = i*16+1;
					int w = width-4;
					int h = 16;

					//(render fill first so the green endcap can overlap)
					if(i == selIndex) {
						b.Color = MakeColor(225, 255, 208);
						if(selActive) {
							b.RectFill(x+5, y+6, w-10+SelectionOverhang, h-12);
							fr_selActive.render(b, new Rectangle(x, y, w + SelectionOverhang, h));
						} else {
							b.RectFill(x+5, y+6, w-10, h-12);
							fr_sel.render(b, new Rectangle(x, y, w, h));
						}
					} else {
						b.Color = MakeColor(232, 232, 232);
						b.RectFill(x+5, y+6, w-10, h-12);
						fr_gray.render(b, new Rectangle(x, y, w, h));
					}

					if(items[i].label != null) {
						int textOffset = 5;
						//offset for aesthetics and/or icon
						textOffset += 8;
						cabedge.font_v3_1_black.Render(b, x+textOffset, y+5, items[i].label);
					}
				}


			}

			
			public void draw() {
				Blitter b = new Blitter(cabedge.screen);
				switch(state) {
					case State.TextBox:
						wr.render(b, new Rectangle(141, 271, 416, 187), WindowRenderer.Mode.Light);
						if(!string.IsNullOrEmpty(tb_text))
							cabedge.font_v3_2_black.Render(b, 150, 280, tb_text);
						//b.blit(cabedge.font_v3_2_black.image, 150, 280);
						//more cursor
						if(tb_state == TextBoxState.Waiting && tb_more && DateTime.Now.Millisecond<500)
							cabedge.font_v3_2_black.Render(b, 525, 430, "...");
						return;

					case State.NoMenu:
						return;

					case State.MainMenu:
						//for darkening the background (looks nice but makes it hard to debug pixels)
						//b.color = makeColor(180, 0, 0, 0);
						//b.rectFill(0, 0, 640, 480);
						renderMainMenu(b);
						break;
				}
			}
			
			public void tick() {
			}

			public void update(int ticks) {
				PlayerInput input = cabedge.player1;
				switch(state) {

					case State.TextBox:
						if(input.Confirm) {
                            input.Confirm.Unpress();
							goto endPage;
						}
						if(input.Cancel) {
							input.Cancel.Unpress();
							goto endPage;
						}
						break;
					endPage:
						tb_state = TextBoxState.Closed;
						state = State.NoMenu;
						goto case State.NoMenu;

					case State.NoMenu:
						break;
	
					case State.MainMenu:
						if(input.Confirm) {
                            input.Confirm.Unpress();
							mainMenuNavigator.activate();
						}

                        if (input.MenuX)
                        {
                            input.MenuX.Unpress();
							state = State.NoMenu;
							goto case State.NoMenu;
						}

						if(input.Cancel) {
                            input.Cancel.Unpress();
							mainMenuNavigator.exit();
							if(!mainMenuNavigator.IsInMenu) {
								state = State.NoMenu;
								goto case State.NoMenu;
							}
						}
						if(input.Down) {
                            input.Down.Unpress();
							mainMenuNavigator.next();

						} else if(input.Up) {
                            input.Up.Unpress();
							mainMenuNavigator.prev();
						}
						break;
				}

				for(int i=0;i<ticks;i++)
					tick();
			}
		}
	}
}


//class Window {
//                public enum State {
//                    Closed, Opening, Open, Closing
//                }
//                public State state;

//                public void open(Point loc, Size size) {
//                    this.size = size;
//                    this.loc = loc;
//                    state = State.Opening;
//                    openctr = 0;
//                }

//                const int openticks = 400;
//                public int openctr;

//                Point loc;
//                Size size;
				
//                public void tick() {
//                    switch(state) {
//                    case State.Opening:
//                        openctr++;
//                        if(openctr == openticks) {
//                            state = State.Open;
//                            goto case State.Open;
//                        }
//                        break;
//                    case State.Open:
//                        break;
//                    }
//                }

//                public void draw(Blitter b) {
//                    switch(state) {
//                        case State.Opening:
//                        case State.Closing:
//                            int phase1 = openctr;
//                            int phase2 = openctr - openticks/2;
//                            int h, w;

//                            //calculate window opening
//                            if(phase2<0) {
//                                Console.WriteLine(phase1);
//                                //in phase 1
//                                if(state == State.Opening) {
//                                    h=16;
//                                    w = (size.Width * phase1)/openticks*2;
//                                } else {
//                                    h= size.Height - ((size.Height-16) * phase1)/openticks*2 + 16;
//                                    w = size.Width;
//                                }
//                            } else {
//                                if(state == State.Opening) {
//                                    w = size.Width;
//                                    h = ((size.Height-16) * phase2)/openticks*2 + 16;
//                                } else {
//                                    w = size.Width - (size.Width * phase2)/openticks*2;
//                                    h = 16;
//                                }
//                            }

//                            int x = (loc.X + size.Width/2) - w/2;
//                            int y = (loc.Y + size.Height/2) - h/2;

//                            b.color = Cabedge.makeColor(200, 5,118,165);
//                            b.rectFill(x, y, w, h);
//                            break;
//                        case State.Open:
//                            //b.color = Cabedge.makeColor(200, 5, 118, 165);
//                            //b.rectFill(loc.X,loc.Y, size.Width, size.Height);
//                            WindowRenderer rr = new WindowRenderer();
//                            rr.render(b, new Rectangle(loc.X, loc.Y, size.Width, size.Height));
//                            break;
//                    }
//                }
//            }
//Window textBox = new Window();