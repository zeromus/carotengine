using System;
using System.Globalization;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using pr2.Common;

namespace pr2.CarotEngine
{

	public class Dimmer
	{
		Fx32 currLevel = 0;
		Fx32 targetLevel = 0;
		Fx32 speed = 0;
		bool running = false;

		public void FadeTo(Fx32 level, int ticks)
		{
			targetLevel = level;
			if (ticks == 0)
			{
				currLevel = targetLevel;
				running = false;
			}
			else
			{
				speed = ((currLevel - targetLevel) / ticks).Abs();
				running = true;
			}
		}

		public bool IsFinished { get { return !running; } }

		public void Update()
		{
			if (!running) return;
			Tick();
		}

		void Tick()
		{
			if (!running) return;
			currLevel = Lib.MoveValueTowards(currLevel, targetLevel, speed);
			if (currLevel == targetLevel) running = false;
		}

		public Fx32 CurrLevel { get { return GetCurrLevel(); } }

		Fx32 GetCurrLevel()
		{
			Fx32 level = Fx32.Max(currLevel, -1);
			level = Fx32.Min(level, 1);
			return level;
		}
	}
}
