using System;
using System.Collections.Generic;
using System.Text;
using pr2.CarotEngine;

namespace pr2.CarotEngine
{
	public class ScriptEntityMove : ActionScript.IScriptAction
	{
		Directions direction;
		int distance;
		Entity entity;
		bool finished;

		public ScriptEntityMove(Entity entity, Directions direction, int distance)
		{
			this.direction = direction;
			this.distance = distance;
			this.entity = entity;
			this.entity.OnMoveFinished += new Entity.EventHandler(moveFinished);
		}

		public void invoke()
		{
			finished = false;
			entity.move(direction, distance);
		}

		private void moveFinished(Entity e)
		{
			finished = true;
		}
		public void tick() 
		{

		}

		public bool Finished
		{
			get
			{
				return finished;
			}
		}
	}
}
