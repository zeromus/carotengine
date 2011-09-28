using System;
using System.Collections.Generic;
using System.Text;

namespace pr2.CarotEngine.IScriptActions
{
	class ScriptCameraMoveTo : ActionScript.IScriptAction
	{
		bool running;
		int dx, dy;
		TileEngineCamera cam;
		float time;

		public ScriptCameraMoveTo(int x, int y, float elapsedTime, TileEngineCamera camera)
		{
			dx = x;
			dy = y;
			time = elapsedTime;
			cam = camera;
		}
		
		public void invoke()
		{
			running = true;
			//throw new Exception("The method or operation is not implemented.");
		}

		public void tick()
		{
			if (running)
			{
				if (cam != null)
				{
				}
				else
				{
					running = false;
				}
			}		
		}

		public bool Finished
		{
			get {
				return !running;
			}
		}
	}
}
