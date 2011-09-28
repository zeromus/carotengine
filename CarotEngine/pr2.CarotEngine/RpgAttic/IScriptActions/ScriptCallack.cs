using System;
using System.Collections.Generic;
using System.Text;

namespace pr2.CarotEngine
{
	class ScriptCallack : ActionScript.IScriptAction
	{
		public bool Finished { get { return true; } }
		public void tick() { }

		MapScriptHandler scriptHandler;
		String scriptName;

		public ScriptCallack(String scriptName, MapScriptHandler scriptHandler)
		{
			this.scriptHandler = scriptHandler;
			this.scriptName = scriptName;
		}

		public void invoke()
		{
			scriptHandler.InvokeDirect(scriptName);
		}
	}
}
