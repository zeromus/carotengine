using System;
using System.Collections.Generic;
using System.Text;

namespace pr2.CarotEngine.Maps
{
	public class MapScript
	{
		protected Map map;
		protected ScriptActivationArgs args;
		private MapScriptHandler mapScriptHandler;
		private RpgController rpg;


		public void init(Map map, MapScriptHandler mapScriptHandler, RpgController rpgController)
		{
			this.map = map;
			this.rpg = rpgController;
			this.mapScriptHandler = mapScriptHandler;
		}

		public void setArgs(ScriptActivationArgs saa)
		{
			args = saa;
		}

		public void StartScript()
		{
			rpg.StartScript();
		}

		public void ScriptCallback(String scriptName)
		{
			rpg.AddScriptItem(new ScriptCallack(scriptName, rpg.GetCurrentMap().scriptHandler));
		}

		public void Wait(int milliseconds)
		{
			rpg.AddScriptItem(new ScriptWait(ScriptWait.WaitMethod.Milliseconds, milliseconds));
		}

		public void WaitSteps(Entity ent, int numberOfSteps)
		{
			rpg.AddScriptItem(new ScriptWait(ScriptWait.WaitMethod.EntitySteps, ent, numberOfSteps));
		}

		public Entity GetPlayer()
		{
			return rpg.GetPlayer();
		}

		public void AddUserScript(ActionScript.IScriptAction script)
		{
			rpg.AddScriptItem(script);
		}
		
		public void EntitySetFacing(Entity ent, Directions direction)
		{
			rpg.EntitySetFacing(ent, direction);
		}

		public void PauseEntity(Entity ent)
		{
			rpg.AddScriptItem(new ScriptGenericEntityAction(ent, ScriptGenericEntityAction.EntityActions.Pause));
		}

		public void ResumeEntity(Entity ent)
		{
			rpg.AddScriptItem(new ScriptGenericEntityAction(ent, ScriptGenericEntityAction.EntityActions.Unpause));
		}

		public void MoveEntity(Entity ent, String moveScript)
		{
			rpg.ScriptMoveEntity(ent, moveScript);
		}

		public void CreatePlayer(String chr, int tx, int ty)
		{
			rpg.CreatePlayer(chr, tx, ty);
		}

		public void SwitchMap(String map)
		{
			rpg.SwitchMap(map);
		}

		public void SwitchMap(String map, String startLoc)
		{
			rpg.SwitchMap(map, startLoc);
		}

		public void CommitScript()
		{
			rpg.CommitScript();
		}

	}
}
