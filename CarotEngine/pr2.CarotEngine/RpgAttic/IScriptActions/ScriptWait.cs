using System;
using System.Collections.Generic;
using System.Text;

namespace pr2.CarotEngine
{
	class ScriptWait : ActionScript.IScriptAction
	{
		public enum WaitMethod
		{
			Milliseconds, EntitySteps
		}

		int goal;
		int counter;
		bool running;
		Entity ent;
		int steps;
		WaitMethod method;

		public ScriptWait(WaitMethod method, params object[] args)
		{
			if (method == WaitMethod.Milliseconds)
			{
				goal = (int)args[0];
			}
			else if (method == WaitMethod.EntitySteps)
			{
				ent = (Entity)args[0];
				steps = (int)args[1];
			}
			this.method = method;
		}
		
		public void invoke()
		{
			running = true;
			if (method == WaitMethod.EntitySteps)
			{
				counter = ent.stepsTaken;
				goal = ent.stepsTaken + steps;
			}
			else if (method == WaitMethod.Milliseconds)
			{
				counter = 0;
			}
		}

		public void tick()
		{
			if (running)
			{
				if (method == WaitMethod.Milliseconds)
					counter++;
				else if (method == WaitMethod.EntitySteps)
					counter = ent.stepsTaken;
			}
		}

		public bool Finished
		{
			get
			{
				return (counter >= goal);
			}
		}
	}
}
