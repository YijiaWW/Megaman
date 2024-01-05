using CoolGame.Controls;
using CoolGame.Scene;
using CoolGame.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SharpDX.Direct2D1;
using System;

namespace CoolGame
{
    /// <summary>
    /// The main class for the game, responsible for initializing, updating, and drawing the game scenes.
    /// </summary>
    public class Game1 : Game
	{
        // Fields declarations...
        public GraphicsDeviceManager _graphics;
		public Microsoft.Xna.Framework.Graphics.SpriteBatch _spriteBatch;

		public StartScene startScene;
		public HelpScene helpScene;
		public ActionScene actionScene;
		private HighScoreScene highScoreScene;
		private AboutScene aboutScene;
		private EnterPlayerNameScene enterPlayerNameScene;
        public GameOverScene overScene;
        public Level2ActionScene level2ActionScene;
		private KeyboardState oldState;


        public static float ScreenHeight { get; set; }
		public static float ScreenWidth { get; set; }

        /// <summary>
        /// Constructor for the Game1 class.
        /// </summary>
        public Game1()
		{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
		}
        /// <summary>
        /// Initializes the game settings and scenes.
        /// </summary>
        protected override void Initialize()
		{
			Shared.stage = new Vector2(_graphics.PreferredBackBufferWidth,
			   _graphics.PreferredBackBufferHeight);
			ScreenHeight = _graphics.PreferredBackBufferHeight;
			ScreenWidth = _graphics.PreferredBackBufferWidth;
			// TODO: Add your initialization logic here


			base.Initialize();
		}

        /// <summary>
        /// Handles scene change requests based on button clicks.
        /// </summary>
        /// <param name="sender">The source of the event, typically a button.</param>
        /// <param name="e">Event arguments (not used).</param>
        private void HandleSceneChangeRequested_Click(object sender, EventArgs e)
		{
			// Cast the sender back to a Button to access its properties
			Button clickedButton = sender as Button;

			// Check if the cast was successful
			if (clickedButton != null)
			{
				// Access the Text property of the clicked button
				string buttonText = clickedButton.Text;

				// Implement logic to change the scene based on the button click
				switch (buttonText)
				{
					case "Start Game":
                        hideAllScenes();
						enterPlayerNameScene.show();
						break;
					case "Help":
						hideAllScenes();
						helpScene.show();
						break;
					case "High Score":
						hideAllScenes();
						highScoreScene.show();
						break;
					case "About":
						hideAllScenes();
						aboutScene.show();
						break;
					case "Quit":
						Exit();
						break;
						// Add cases for other buttons
				}
			}
		}
        /// <summary>
        /// Loads game content, sets up scenes, and initializes UI components.
        /// </summary>
        protected override void LoadContent()
		{
			_spriteBatch = new Microsoft.Xna.Framework.Graphics.SpriteBatch(GraphicsDevice);

			level2ActionScene = new Level2ActionScene(this,"");
			this.Components.Add(level2ActionScene);	

            enterPlayerNameScene = new EnterPlayerNameScene(this);
			this.Components.Add(enterPlayerNameScene);

            actionScene = new ActionScene(this, "");
            this.Components.Add(actionScene);

            startScene = new StartScene(this);
			this.Components.Add(startScene);

			helpScene = new HelpScene(this);
			this.Components.Add(helpScene);

			highScoreScene = new HighScoreScene(this);
			this.Components.Add(highScoreScene);

			aboutScene = new AboutScene(this);
			this.Components.Add(aboutScene);

			foreach (Button button in startScene.Menu.buttons)
			{
				button.Click += HandleSceneChangeRequested_Click;
			}
			startScene.show();

			// TODO: use this.Content to load your game content here
			//--background music ----
			Song backgroundMusic = this.Content.Load<Song>("Sounds/BackgroundMusic");
			MediaPlayer.IsRepeating = true;
			MediaPlayer.Play(backgroundMusic);
			//--background music ----
		}

        /// <summary>
        /// Hides all game scenes.
        /// </summary>
        public void hideAllScenes()
		{

			//conditionally work as long as we don't add
			//anything OTHER THAN game scenes to the components

			//foreach (GameScene item in Components)
			//{ 
			//        item.hide();
			//}

			//always work
			foreach (GameComponent item in Components)
			{
				if (item is GameScene)
				{
					//will work
					//GameScene gs = item as GameScene;
					GameScene gs = (GameScene)item;
					gs.hide();
				}
			}
		}
        /// <summary>
        /// Updates the game state, handling scene transitions and input.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
		{
			KeyboardState ks = Keyboard.GetState();
			if (helpScene.Enabled||enterPlayerNameScene.Enabled||highScoreScene.Enabled||aboutScene.Enabled)
			{
				if (oldState.IsKeyUp(Keys.Escape) && ks.IsKeyDown(Keys.Escape))
				{
					hideAllScenes();
					startScene.show();
				}

			}
			oldState = ks;

			
			base.Update(gameTime);
		}
        /// <summary>
        /// Draws the current game scene.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values, used for frame-based animation.</param>
        protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);
			

			base.Draw(gameTime);
		}
	}
}