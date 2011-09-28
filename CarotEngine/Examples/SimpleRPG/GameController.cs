using System;
using System.IO;
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

		//public class GameController {
			
		//    public void draw() {
		//        DateTime dtNow = DateTime.Now;
		//        Record todo = null;
		//        for(;;) {
		//            if(records.Count == 0) break;
		//            if((DateTime.Now - records.Peek().timestamp).TotalMilliseconds >= msDelay) {
		//                todo = records.Dequeue();
		//            } else break;
		//        }

		//        Blitter b = new Blitter(cabedge.screen);
		//        b.clear(Color.Black);
		//        if(todo != null) {
		//            if(last != null) {
		//                dx = todo.ms.X - last.ms.X;
		//                dy = todo.ms.Y - last.ms.Y;
		//                dt = (int)(todo.timestamp - last.timestamp).TotalMilliseconds;
		//                zx = dx * dt * msActual / 1000;
		//                zy = dy * dt * msActual / 1000;
		//            }

		//            last = todo;
		//        }

		//        MouseState ms = Mouse.GetState();
		//        if(Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Keys.LeftShift)) {
		//            b.blit(cursor, ms.X, ms.Y);
		//            cabedge.font_v3_4.render(b, 4, 4, "lag disabled");
		//        } else {
		//            //if we are using motion prediction, bump the cursor ahead
		//            if(Keyboard.GetState().IsKeyDown(Keys.Tab)) {
		//                cabedge.font_v3_4.render(b, 4, 4, string.Format("Motion predict: {0} {1}", zx, zy));
		//                b.blit(cursor, last.ms.X + zx, last.ms.Y + zy);
		//            } else {
		//                cabedge.font_v3_4.render(b, 4, 4, string.Format("lag {0}ms", msDelay));
		//                if(Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Keys.LeftControl))
		//                    b.blit(cursor_tint, ms.X, ms.Y);
		//                if(last != null) b.blit(cursor, last.ms.X, last.ms.Y);
		//            }
		//        }
		//    }

		//    Record last;
		//    int dx, dy, dt, zx, zy;
		//    int msDelay;
		//    int msActual;

		//    class Record {
		//        public MouseState ms;
		//        public DateTime timestamp;
		//    }

		//    Queue<Record> records = new Queue<Record>();
		//    bool initialized;
		//    Image cursor;
		//    Image cursor_tint;
		//    Image cursor_tint_blue;

		//    public void update(int ticks) {
		//        if(!initialized) {
		//            string[] lines = File.ReadAllLines("lag.txt");
		//            msDelay = int.Parse(lines[0]);
		//            msActual = int.Parse(lines[1]);
		//            cursor = cabedge.loadImage("content/cursor.png");
		//            cursor.alphafy(Color.Cyan);
		//            cursor_tint = cabedge.loadImage("content/cursor_tint.png");
		//            cursor_tint.alphafy(Color.Cyan);
		//            cursor_tint_blue = cabedge.loadImage("content/cursor_tint_blue.png");
		//            cursor_tint_blue.alphafy(Color.Cyan);
		//            initialized = true;
		//        }

		//        MouseState ms = Microsoft.Xna.Framework.Input.Mouse.GetState();
		//        Record r = new Record();
		//        r.ms = ms;
		//        r.timestamp =DateTime.Now;
		//        records.Enqueue(r);
		//    }
		//}

		public class GameController {
			public enum State {
				Loading, MainMenu, InGame
			}
			public State state = State.Loading;
			public int ctr;
			public int ctr_div;
			float boost_v, boost_s;

			public Image rpgBackbuffer;
			public UiController uiController;
			protected RpgController rpgController;
			//protected PlayerInput player1 = new PlayerInput(PlayerIndex.One);
			const string pr2string = "p i - r - s q u a r e d";

			public void draw() {
				Blitter b = new Blitter(cabedge.screen);
				switch(state) {

					case State.MainMenu:
						b.Clear(Color.Black);
						break;

					case State.InGame:
						rpgController.draw(rpgBackbuffer);
						b.StretchBlit(rpgBackbuffer, 0, 0, 640, 480);
						uiController.draw();
						break;

				}
			}
			Entity man;


			public void tick() {
				switch(state) {
					case GameController.State.Loading:
						rpgBackbuffer = cabedge.NewImage(320, 240);
						uiController = new UiController();
						rpgController = new RpgController(cabedge.player1);

						//rpgController.SwitchMap("raw/dock.map");
                        rpgController.SwitchMap("raw/city01.map");
						rpgController.CreatePlayer("raw/man4.chr", 7, 15);
						man = rpgController.CreateEntity("raw/man3.chr", 8, 11);
						man.bCanHurry = false;
						man.activationScript = "Test1";
						cabedge.loading();

						state = State.InGame;
						goto case GameController.State.InGame;


					case State.MainMenu:
						goto case State.InGame;

					case State.InGame:
						rpgController.tick();
						uiController.tick();
						/*if(player1.Confirm) {
							player1.Confirm.unpress();
							uiController.textOpen();
						}*/
						break;
				}
			}

			public void update(int ticks) {
				cabedge.player1.Update();

				if(uiController != null) {
					if(uiController.state == UiController.State.NoMenu && cabedge.player1.MenuX) {
						cabedge.player1.MenuX.Unpress();
						uiController.enterMainMenu();
					}
					uiController.update(ticks);
				}

				for(int i=0;i<ticks;i++)
					tick();
			}
		}

		public GameController gameController = new GameController();


	}
}



					//case GameController.State.Intro:
					//    ctr_div++;
					//    if(ctr_div != 3) break;
					//    ctr_div = 0;
					//    ctr++; //472

					//    //bouncy r

					//    if(cabedge.player1.Up) {
					//        cabedge.player1.Up.unpress();
					//        boost_v = 2;
					//    }

					//    boost_s += boost_v;
					//    boost_v-=0.1f;
					//    if(boost_s < 0) {
					//        boost_v = 0;
					//        boost_s = 0;
					//    }

					//    //---------

					//    if(ctr == 1200)
					//        state = State.MainMenu;
					//    break;


//case State.Intro:
//                    b.clear(Color.Black);

//                    int strlen = pr2string.Length;
//                    for(int i=0;i<strlen;i++) {
//                        int x = cabedge.font_v3_6.measureWidth(pr2string.Substring(0,i+1)) - 500 + ctr;
//                        int y = 0;
//                        bool falling = false;
//                        if(x > 480) {
//                            falling = true;
//                            x -= 480;
//                            y = x*x/100;
//                            x += 480;
//                        }
//                        if(i == 6) {
//                            if(!falling)
//                                y -= (int)boost_s;
//                            Console.WriteLine(boost_s);
//                        }

//                        cabedge.font_v3_6.render(b, x, y+200, pr2string.Substring(i, 1));
//                    }
					
//                    break;