using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace pr2.CarotEngine
{
	public class ActionScript
	{
		private List<IScriptAction> actions;
		private int cursor = 0;
		private IScriptAction current;
		private bool isRunning = false;
		private bool bLoop = false;
		
		public bool IsRunning
		{
			get { return isRunning; }
		}

		public bool Loop
		{
			get { return bLoop; }
			set { bLoop = value; }
		}

		public interface IScriptAction
		{
			void invoke();
			void tick();
			bool Finished { get; }
		}
		
		public ActionScript()
		{
			actions = new List<IScriptAction>();
			isRunning = false;
		}

		public void add(IScriptAction action)
		{
			actions.Add(action);
		}

		public void addRange(List<IScriptAction> actions)
		{
			this.actions.AddRange(actions);
		}

		public void begin()
		{
			cursor = 0;
			isRunning = true;
		}

		public void tick()
		{
			if (isRunning)
			{
				if (current == null || current.Finished)
				{
					if (cursor >= actions.Count)
					{
						if (bLoop && actions.Count > 0)
						{
							cursor = 0;
						}
						else
						{
							isRunning = false;
						}
					}
					else
					{
						current = actions[cursor++];
						current.invoke();
					}
				}
				else
					current.tick();
			}
		}
		public static List<IScriptAction> fromEntityMoveScript(Entity e, String movecode)
		{
			Regex rx = new Regex("([uldrbw])([0-9]+)?", RegexOptions.IgnoreCase);
			MatchCollection mc = rx.Matches(movecode);
			List<IScriptAction> actions = new List<IScriptAction>();

			foreach (Match m in mc)
			{
				//Console.WriteLine("{0}, {1}", m.Groups[1].Value, m.Groups[2].Value);
				String code = m.Groups[1].Value.ToLower();

				if (code == "r" || code == "l" || code == "u" || code == "d")
				{
					if (m.Groups.Count == 3)
					{
						int dist = int.Parse(m.Groups[2].Value) * 16;
						Directions dir = Directions.None;

						if (code == "r") dir = Directions.e;
						if (code == "l") dir = Directions.w;
						if (code == "u") dir = Directions.n;
						if (code == "d") dir = Directions.s;

						ScriptEntityMove sem = new ScriptEntityMove(e, dir, dist);
						actions.Add(sem);
					}
				}
				else if (code == "w")
				{
					if (m.Groups.Count == 3)
					{
						int wait = int.Parse(m.Groups[2].Value);
						ScriptWait sw = new ScriptWait(ScriptWait.WaitMethod.Milliseconds, wait);
						actions.Add(sw);
					}
				}
				else if (code == "b")
				{
					//script.Loop = true;
				}
			}

			return actions;
		}
	}
}
