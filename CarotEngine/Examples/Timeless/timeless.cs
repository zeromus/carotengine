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
			timeless.run();
		}

		protected override void GameInitialize()
		{
			SetResolution(320, 200, false);
			test = LoadImage0("sprites/hourglass.png");
		}

		Image test;

		public override void Update(GameTime gameTime) {
			if (Microsoft.Xna.Framework.Input.GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
				Exit();
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime) {
			Blitter b = new Blitter(screen);
			b.Clear(Color.Gray);
			b.Blit(test, 100, 100);
		}
	}
}
