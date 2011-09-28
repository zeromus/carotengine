using System;
using System.Collections.Generic;
using System.Text;

namespace pr2.CarotEngine
{
	public class ScriptGenericEntityAction : ActionScript.IScriptAction
	{
		public enum EntityActions
		{
			Pause, Unpause
		}
		EntityActions action;
		Entity e;

		public ScriptGenericEntityAction(Entity e, EntityActions action)
		{
			this.e = e;
			this.action = action;
		}

		public void invoke()
		{
			switch (action)
			{
				case EntityActions.Pause:
					e.bPaused = true;
					break;
				
				case EntityActions.Unpause:
					e.bPaused = false;
					break;
			}
		}

		public void tick() { }

		public bool Finished
		{
			get { return true; }
		}
	}
}
