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
	/// be careful with this class
	/// each ADML.Character will get its own imagedata, so you had better spawn several of these CarotAdmlCharacter
	/// from one ADML.Character
	/// </summary>
	public class CarotADMLCharacter : ICharacter {

		//data
		AnimRecord[] walkAnims = new AnimRecord[9];
		AnimRecord[] idleAnims = new AnimRecord[9];
		ADML.Character admlchar;

		//state
		Directions d = Directions.s;
		int state = 0; //0 = standing, 1 = walking, 2 = stepping, 3 = animating
		AnimRecord customAnimation;

		Dictionary<string, AnimRecord> animations = new Dictionary<string, AnimRecord>();

		public CarotADMLCharacter(ADML.Character src, string override_image_filename) {
			admlchar = src;

			string srcimage = (override_image_filename!=null)?override_image_filename:src.source;

			//dice up the source image if it hasnt been done yet
			using(Image srcimg = GameEngine.Game.LoadImage(srcimage)) {
				foreach(ADML.View view in src.views.Values) {
					FrameServer fs = new FrameServer(view);
					fs.dice(src.tcol, srcimg);
					view.SetFrameServer(fs);
				}
			}

			tryCreateIdleAnim("l_idle", "l_walk");
			tryCreateIdleAnim("r_idle", "r_walk");
			tryCreateIdleAnim("u_idle", "u_walk");
			tryCreateIdleAnim("d_idle", "d_walk");

			//create the animrecords
			foreach(ADML.Anim anim in admlchar.anims.Values) {
				AnimRecord ar = new AnimRecord();
				ar.admlanim = anim;
				ar.controller = new StandardChrAnimationController(ar.admlanim.str);
				ar.address = int.Parse(ar.admlanim.addr);
				animations[anim.name] = ar;
			}

			//setup the hardcoded animation types
			walkAnims[(int)Directions.w] = findAnim("l_walk");
			walkAnims[(int)Directions.e] = findAnim("r_walk");
			walkAnims[(int)Directions.n] = findAnim("u_walk");
			walkAnims[(int)Directions.s] = findAnim("d_walk");
			idleAnims[(int)Directions.w] = findAnim("l_idle");
			idleAnims[(int)Directions.e] = findAnim("r_idle");
			idleAnims[(int)Directions.n] = findAnim("u_idle");
			idleAnims[(int)Directions.s] = findAnim("d_idle");
			
			if(walkAnims[(int)Directions.w].admlanim.idle != null)
				idleAnims[(int)Directions.w] = animations["l_idle"];
			if(walkAnims[(int)Directions.e].admlanim.idle != null)
				idleAnims[(int)Directions.e] = animations["r_idle"];
			if(walkAnims[(int)Directions.n].admlanim.idle != null)
				idleAnims[(int)Directions.n] = animations["u_idle"];
			if(walkAnims[(int)Directions.s].admlanim.idle != null)
				idleAnims[(int)Directions.s] = animations["d_idle"];
		}

		void tryCreateIdleAnim(string idlekey, string walkkey) {
			if(admlchar.anims.ContainsKey(idlekey)) return;
			if(!admlchar.anims.ContainsKey(walkkey)) return;
			ADML.Anim walkanim = admlchar.anims[walkkey];
			if(walkanim.idle != null) return;
			//ok.. we need to make a new anim
			ADML.Anim idleanim = (ADML.Anim)walkanim.Clone();
			idleanim.name = idlekey;
			admlchar.anims[idlekey] = idleanim;
			idleanim.str = "F" + new V3ChrAnimationScript(idleanim.str).preview().frames[0].frame+"W99999";
		}

		AnimRecord findAnim(string key) {
			if(!animations.ContainsKey(key)) return null;
			return animations[key];
		}

		class AnimRecord {
			public StandardChrAnimationController controller;
			public ADML.Anim admlanim;
			public int address;
		}

		public Image Frame {
			get {
				AnimRecord ar = GetCurrentAnimRecord();
				if(ar == null) return null;
				else return ar.admlanim.view.GetFrameServer<FrameServer>().GetImage(ar.address, ar.controller.frame);
			}
		}

		AnimRecord GetCurrentAnimRecord() {
			switch(state) {
				case 0: return idleAnims[(int)d];
				case 1: return walkAnims[(int)d];
				case 3: return customAnimation;
				default: return null;
			}
		}

		public bool SupportsCommand(CharacterCommands command) {
			if(command == CharacterCommands.Step) return false;
			return true;
		}

		public void Command(CharacterCommands command, params object[] args) {
			customAnimation = null;
			switch(command) {
				case CharacterCommands.Step:
					state = 1;
					d = (Directions)args[0];
					walkAnims[(int)d].controller.reset();
					return;
				case CharacterCommands.Face:
					//if(state == 1 && d == (Directions)args[0]) { }
					//else 
					//state = 0;
					//if(d != (Directions)args[0])
					state = 0;
					if(d != (Directions)args[0]) {
						d = (Directions)args[0];
						if(GetCurrentAnimRecord() != null)
							GetCurrentAnimRecord().controller.reset();
					}
					return;
				case CharacterCommands.Stop: {
						state = 0;
						AnimRecord ar = GetCurrentAnimRecord();
						if(ar != null)
							ar.controller.reset();
					}
					return;
				case CharacterCommands.Walk:
					if(d == (Directions)args[0] && state == 1) { } else {
						state = 1;
						walkAnims[(int)(Directions)args[0]].controller.reset();
						d = (Directions)args[0];
					}
					return;
				case CharacterCommands.Animate:
					if(animations.TryGetValue((string)args[0], out customAnimation)) {
						customAnimation.controller.reset();
						state = 3;
					} else state = 0;
					return;
				default: return;
			}
		}

		public void Tick() {
			AnimRecord ar = GetCurrentAnimRecord();
			if(ar != null)
				ar.controller.tick();
		}
		public Rectangle Hotspot {
			get {
				//return new Rectangle(0, 0, 0, 0);
				AnimRecord ar = GetCurrentAnimRecord();
				if(ar != null) return ar.admlanim.hotspot;
				else return new Rectangle(0, 0, 0, 0);
			}
		}

		public void Dispose() { }

		class FrameServer {

			ADML.View view;
			public FrameServer(ADML.View view) {
				this.view = view;
			}

			int fcx, fcy;
			public Image[,] frames;

			public Image GetImage(int address, int frame) { return frames[address, frame]; }

			public void dice(Color tcol, Image image) {
				fcx = (image.Width - view.pad.Left) / (view.dims.Width + view.pad.Right);
				fcy = (image.Height - view.pad.Top) / (view.dims.Height + view.pad.Bottom);
				frames = new Image[fcy, fcx];
				for(int fy=0; fy<fcy; fy++)
					for(int fx = 0; fx < fcx; fx++) {
						int px = view.pad.Left + (view.dims.Width+view.pad.Right)*fx;
						int py = view.pad.Top + (view.dims.Height+view.pad.Bottom)*fy;
						Image img = GameEngine.Game.NewImage(view.dims.Width, view.dims.Height);
						frames[fy, fx] = img;
						Blitter b = new Blitter(img);
						b.BlitSubrect(image, px, py, img.Width, img.Height, 0, 0);
						img.Resolve();
						img.Alphafy(tcol);
					}


			} //dice

		}
	}


}

#endif