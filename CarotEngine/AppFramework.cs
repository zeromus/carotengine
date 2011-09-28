using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace pr2.CarotEngine {

	public class AppFramework : Game {

		GameEngine game;

		public AppFramework() {
			game = GameEngine.Game;
			
		}

		protected override void Initialize() {
			game.Initialize();
		}

		public void BaseInitialize() {
			base.Initialize();
		}

		protected override void LoadContent() {
			game.LoadGraphicsContent();
		}

		protected override void UnloadContent() {
			game.UnloadGraphicsContent();
		}

		protected override void Update(GameTime gameTime) {
			base.Update(gameTime);
			game.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime) {
			base.Draw(gameTime); //dunno what to do with this
			game.Render(gameTime);
		}

		protected override void Dispose(bool disposing) {
			game.Dispose(disposing);
			base.Dispose(disposing);
		}
	}

}