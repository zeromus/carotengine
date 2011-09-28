using System;
using System.Collections;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace pr2.CarotEngine
{
	public class V3ChrController : ICharacter
	{
		V3Chr chr;
		Directions d = Directions.s; 
		int state = 0; //0 = standing, 1 = walking, 2 = stepping, 3 = animating

		StandardChrAnimationController customAnimation;

		public StandardChrAnimationController[] walkAnims = new StandardChrAnimationController[9];

		public V3ChrController() { }
		public V3ChrController(V3Chr chr)
		{
			this.chr = chr;
			for(int i=1;i<9;i++)
				walkAnims[i] = new StandardChrAnimationController(chr.anims[i]);
		}

		//--------------
		public void Dispose() { }

		public Image Frame 
		{
			get
			{
				if(state == 0) return chr.frames[chr.idleFrames[(int)d]];
				if(state == 1) return chr.frames[walkAnims[(int)d].frame];
				if(state == 3) return chr.frames[customAnimation.frame];
				throw new Exception("wtf");
			}
		}

		public bool SupportsCommand(CharacterCommands command)
		{
			if(command == CharacterCommands.Step) return false;
			return true;
		}
		public void Command(CharacterCommands command, params object[] args)
		{
			customAnimation = null;
			switch(command)
			{
				case CharacterCommands.Step:
					state = 1;
					d = (Directions)args[0];
					walkAnims[(int)d].reset();
					return;
				case CharacterCommands.Face:
					//if(state == 1 && d == (Directions)args[0]) { }
					//else 
					//state = 0;
					if(d != (Directions)args[0])
						state = 0;
					d = (Directions)args[0];
					return;
				case CharacterCommands.Stop:
					state = 0;
					
					return;
				case CharacterCommands.Walk:
					if(d == (Directions)args[0] && state == 1) { }
					else {
						state = 1;
						walkAnims[(int)(Directions)args[0]].reset();
						d = (Directions)args[0];
					}
					return;
				case CharacterCommands.Animate:
					state = 3;
					customAnimation = new StandardChrAnimationController((string)args[0]);
					customAnimation.reset();
					return;
				default: return;
			}
		}

		public void Tick()
		{
			if(state == 1)
				walkAnims[(int)d].tick();
			if(customAnimation != null)
				customAnimation.tick();
		}

		public Rectangle Hotspot { get { return chr.hotspot; } }
		//--------------

		
	}

}