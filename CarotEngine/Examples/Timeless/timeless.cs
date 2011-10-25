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
		Blitter bCurr;

		/// <summary>
		/// Provided as a convenience for similarity to verge, this merely calls SetTextureWrap(true) and then does a blit
		/// (to be moved into Blitter eventually)
		/// </summary>
		void WrapBlit(Blitter b, Image src, int x, int y)
		{
			SetTextureWrap(true);
			b.Blit(src, x, y);
			SetTextureWrap(false);
		}

		/// <summary>
		/// Provided as a convenience for similarity to verge, this merely calls Blit in the correct pattern to simulate the desired result
		/// </summary>
		void BlitWrap(Blitter b, Image src, int x, int y)
		{
			int ox = x + src.Width - b.Dest.Width;
			int oy = y + src.Height - b.Dest.Height;
			b.Blit(src, x, y);
			if(ox>0) b.Blit(src, -ox, y);
			if (oy > 0) b.Blit(src, x, -oy);
			if(ox>0 && oy>0) b.Blit(src, -ox, -oy);
		}

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
			app.IsFixedTimeStep = true;
			app.TargetElapsedTime = TimeSpan.FromMilliseconds(10);
			SetResolution(320, 200);
			StaticInitializers();
			Autoexec();
		}


		public override void Update(GameTime gameTime)
		{
			bCurr = new Blitter(screen);
			base.Update(gameTime); 
			if (Microsoft.Xna.Framework.Input.GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) Exit();
			systemtime++;
			MyTimer();
			Tick();
		}


		protected override void Draw(GameTime gameTime) {
			Blitter b = new Blitter(screen);
			bCurr = b;
			b.Clear(Color.Gray);
			//SUPERSECRETTHINGY
			WrapBlit(b, bg, 0, 0);
			RenderSprites(b);
			app.Window.Title = systemtime.ToString();
			//b.ScaleBlit(spherepurple, 0, 0, 100,100); //test
		}
	}
}
