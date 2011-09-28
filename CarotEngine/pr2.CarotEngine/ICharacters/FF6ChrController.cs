using sysdrawing=System.Drawing;

namespace pr2.CarotEngine
{

	//to use me, please supply a chr
	//and an array of 12 ints describing the frames in this order:
	//up_idle, up_lfoot, up_rfoot,
	//dn_idle, dn_lfoot, dn_rfoot,
	//lf_idle, lf_lfoot, lf_rfoot,
	//rt_idle, rt_lfoot, rt_rfoot
	public class FF6ChrController : ICharacter
	{
		IEntityQuery eq;
		V3Chr chr;
		Directions d = Directions.s; 
		int state = 0; //0 = standing, 1 = walking, 2 = stepping
		int counter = 0;
		int step = 0;
		int[] frames;
		sysdrawing.Rectangle hotspotOverride = sysdrawing.Rectangle.Empty;
		
		public FF6ChrController(V3Chr chr, int[] frames) { this.chr = chr; this.frames = frames; }
		public FF6ChrController(V3Chr chr, sysdrawing.Rectangle hotspot, int[] frames) { this.chr = chr; this.hotspotOverride = hotspot; this.frames = frames; }

		int calcPixel() { return (int)((float)counter/eq.ticksPerPixel); }

		//--------------
		public void Dispose() { }
		public FunkImage frame
		{
			get
			{
				//System.Console.WriteLine(state);
				int n = (int)d*3-3;
				if(state == 0) return chr.frames[frames[n]];

				int pixel = calcPixel();
				int ofs = 0;
				if(pixel>=8) ofs = 1;
		
				if(ofs==0) return chr.frames[frames[n]];
				else
				{
					if(step == 0) return chr.frames[frames[n+1]];
					else return chr.frames[frames[n+2]];
				}
				
			}
		}

		public bool supportsCommand(CharacterCommands command) { return true; }
		public void command(CharacterCommands command, params object[] args)
		{
			switch(command)
			{
				case CharacterCommands.SetEntityQuery:
					eq = (IEntityQuery)args[0];
					return;
				case CharacterCommands.Face:
					System.Console.WriteLine("face");
					state = 0;
					d = (Directions)args[0];
					return;
				case CharacterCommands.Walk:
					System.Console.WriteLine("walk");
					state = 1;
					if(d == (Directions)args[0]) step = step==0?1:0;
					else step = 0;
					d = (Directions)args[0];
					counter = 0;
					break;
				case CharacterCommands.Stop:
					System.Console.WriteLine("stop");
					state = 0;
					return;
				case CharacterCommands.Step:
					System.Console.WriteLine("step");
					state = 2;
					if(d == (Directions)args[0]) step = (step+1)&1;
					else step = 0;
					d = (Directions)args[0];
					counter = 0;
					return;
			}
		}

		public void tick()
		{
			if(eq == null) return;

			counter++;
			if(calcPixel()>=16.0f)
			{
				if(state==1) { counter = 0; step = (step+1)&1; }
				if(state==2) state = 0;
			}
		}

		public sysdrawing.Rectangle hotspot { get { if(hotspotOverride == sysdrawing.Rectangle.Empty) return chr.hotspot; else return hotspotOverride; } }
		//public sysdrawing.Rectangle hotspot { get { return new sysdrawing.Rectangle(0,8,32,32); } }
		//--------------
	}

}