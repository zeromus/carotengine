using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using pr2.CarotEngine;
using pr2.Common;

namespace Timeless {

	public partial class Timeless : GameEngine {

		/// <summary>
		/// global timeless instance
		/// </summary>
		protected static Timeless timeless;

		static void Main(string[] args) {
			timeless = new Timeless();
			timeless.Run();
		}

		int systemtime;

		Random rnd = new Random();

		int Random(int min, int max)
		{
			return rnd.Next(min, max + 1);
		}

		int sin(int degrees)
		{
			double s = Math.Sin(Lib.Rads(degrees));
			return (int)(s * 65535);
		}

		int cos(int degrees)
		{
			double s = Math.Cos(Lib.Rads(degrees));
			return (int)(s * 65535);
		}

		protected override void GameInitialize()
		{
			SetResolution(320, 200);
			StaticInitializers();
			Autoexec();
		}


		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime); 
			if (Microsoft.Xna.Framework.Input.GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) Exit();
			systemtime++;
			MyTimer();
			Tick();
		}


		protected override void Draw(GameTime gameTime) {
			Blitter b = new Blitter(screen);
			b.Clear(Color.Gray);
			RenderSprites(b);
			
		}
	}
}
