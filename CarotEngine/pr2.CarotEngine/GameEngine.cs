using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace pr2.CarotEngine
{

	/// <summary>
	/// master base class provides friendly access to global variables
	/// </summary>
	public abstract class GameEngineComponent
	{
		public GameEngineComponent() { }
		internal static GameEngine game;
	}

	public class CarotGraphicsDeviceManager : GraphicsDeviceManager
	{
		public CarotGraphicsDeviceManager(AppFramework app)
			: base(app)
		{
		}

		//see this for more..
		//http://msdn.microsoft.com/en-us/library/bb195022.aspx
	}


	/// <summary>
	/// Derive your game from this. It provides useful APIs which you should view as globals.
	/// </summary>
	partial class GameEngine
	{
		/// <summary>
		/// This is sort of a global reference to the game engine base class instance.
		/// You can use it for a number of global-ish things regarding rendering state and engine services.
		/// (But you should have access to it through your derived game engine class as well).
		/// We may want to rename this GameEngine or some such so that the game can call itself Game (instead of having to name itself)
		/// </summary>
		public static GameEngine Game;

		protected GameEngine()
		{
			GameEngineComponent.game = this;
		}

		public void Run()
		{
			//TODO - this is the source of much trouble. we need a global device, not a global game engine.
			//it may need to be refactored to be a separate xna wapper and game engine (as an optional service)
			//(this is for multiwin support)
			Game = this;

			using (AppFramework app = new AppFramework())
			{
				this.app = app;
				app.IsFixedTimeStep = false;
				InitializeBeforeRun();
				app.Run();
			}
		}

		//these utility functions might should move somewhere else
		public static float Sin(float x) { return (float)Math.Sin(x); }
		public static float Cos(float x) { return (float)Math.Cos(x); }
		public static float Sin(double x) { return (float)Math.Sin(x); }
		public static float Cos(double x) { return (float)Math.Cos(x); }

		public GraphicsDevice Device;
		public CarotGraphicsDeviceManager manager;
		public CarotContentManager Content;

		protected AppFramework app;

		public virtual void Dispose(bool disposing)
		{

		}

		public virtual void InitializeBeforeRun()
		{
			manager = new CarotGraphicsDeviceManager(app);
			Content = new CarotContentManager(app.Services);
			manager.DeviceReset += new EventHandler<EventArgs>(manager_DeviceReset);
			manager.DeviceCreated += new EventHandler<EventArgs>(manager_DeviceCreated);
			manager.DeviceResetting += new EventHandler<EventArgs>(manager_DeviceResetting);
			//manager.SynchronizeWithVerticalRetrace = false;
		}

		void manager_DeviceResetting(object sender, EventArgs e)
		{
			Console.WriteLine("device resetting");
		}

		void InitializeDevice()
		{
			//perform actions necessary when the device resets:

			//since the device reset, our cached state is invalid. make a new cache with defaults
			//RenderState = new CarotRenderState(this);
		}

		void manager_DeviceCreated(object sender, EventArgs e)
		{
			InitializeDevice();
		}

		public virtual Type FindMeAFuckingType(string name) { return null; }

		void manager_DeviceReset(object sender, EventArgs e)
		{
			InitializeDevice();
			//Image.RestoreAll();
		}

		public void Initialize()
		{

			Device = manager.GraphicsDevice;

#if XBOX360
			pr2.Common.ResourceManager.SetContentRoot(new DirectoryInfo(new DirectoryInfo("./Content").FullName));
			pr2.Common.ResourceManager.mountDirectory("", ".");
			pr2.Common.ResourceManager.mountDirectory("", "./Content");
			pr2.Common.ResourceManager.mountDirectory("", "./Content/raw");
#else
			{
				string path = new System.IO.FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location).DirectoryName;
				pr2.Common.ResourceManager.SetContentRoot(new DirectoryInfo(Path.Combine(path, "content")));
				pr2.Common.ResourceManager.mountDirectory("", path);
				pr2.Common.ResourceManager.mountDirectory("", Path.Combine(path, "content"));
				pr2.Common.ResourceManager.mountDirectory("", Path.Combine(path, "content/raw"));
			}
#endif

			pr2.Common.ResourceManager.MountAssemblyResources("/assembly", System.Reflection.Assembly.GetExecutingAssembly(), "pr2.CarotEngine.EmbeddedContent.");

			InitGraphics();

			//this triggers LoadGraphicsContent which needs ResourceManager to be setup
			app.BaseInitialize();

			GameInitialize();
		}

		/// <summary>
		/// just a simple blitter that you can use any way you want if you are feeling too lazy to construct one
		/// </summary>
		public Blitter blitter;


		protected virtual void GameInitialize() { }

		public void LoadGraphicsContent()
		{
			blitter = new Blitter(screen);
			LoadGraphics();
			//Image.LoadAll();
		}

		public void UnloadGraphicsContent()
		{
			UnloadGraphics();
			Image.UnloadAll(Device);
		}

		long ticks = DateTime.Now.Ticks;
		protected int fps = 0;
		int frames = 0;
		public virtual void Update(GameTime gameTime)
		{
			long ts = DateTime.Now.Ticks;
			if (ts - ticks > 10000000)
			{
				ticks = ts;
				fps = frames;
				frames = 0;
				app.Window.Title = fps.ToString() + " fps";
			}
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		public void Render(GameTime gameTime)
		{
			frames++;

			//prepare for rendering
			BeginDraw();

			//render
			Draw(gameTime);
		}

		protected virtual void Draw(GameTime gameTime)
		{
		}

		protected void Exit()
		{
			app.Exit();
		}

	}
}
