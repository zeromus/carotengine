using System;
using System.Collections.Generic;
using System.Text;

namespace pr2.CarotEngine
{
	public partial class RpgController
	{
		public void StartScript()
		{
			workingScript = new ActionScript();
		}
		
		public void ScriptMoveEntity(Entity ent, String moveScript)
		{
			workingScript.addRange(ActionScript.fromEntityMoveScript(ent, moveScript));
		}

		public void AddScriptItem(ActionScript.IScriptAction action)
		{
			workingScript.add(action);
		}

		public void CommitScript()
		{
			if (workingScript != null)
			{
				workingScript.begin();
				this.scripts.Add(workingScript);
				workingScript = null;
			}
		}
	}
}
