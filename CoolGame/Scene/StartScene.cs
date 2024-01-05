using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace CoolGame.States
{
    /// <summary>
    /// Represents the start scene of the game, typically displaying the main menu.
    /// </summary>
    public class StartScene : GameScene
	{
		public MenuComponent Menu { get; set; }
		private SpriteBatch sb;
		string[] menuItems = { "Start Game", "High Score","Help",  "About", "Quit" };
        public Texture2D StartMenutex;
        /// <summary>
        /// Initializes a new instance of the StartScene class.
        /// </summary>
        /// <param name="game">The game object this scene is associated with.</param>
        public StartScene(Game game) : base(game)
		{
			Game1 g = (Game1)game;
			this.sb = g._spriteBatch;
            StartMenutex = g.Content.Load<Texture2D>("Images/GameMenu");
            Texture2D button = game.Content.Load<Texture2D>("Controls/Button");
			SpriteFont font = game.Content.Load<SpriteFont>("Fonts/Font");
			SoundEffect Clicksound = game.Content.Load<SoundEffect>("Sounds/Clicksound");
			SoundEffect SelectSound = game.Content.Load<SoundEffect>("Sounds/SelectSound");

			Menu = new MenuComponent(game,this.sb,button,font, Clicksound,SelectSound,menuItems);
			this.Components.Add(Menu);

		}
        /// <summary>
        /// Draws the start scene, including the main menu.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values, used for frame-based animation.</param>
        public override void Draw(GameTime gameTime)
        {
            sb.Begin();
            sb.Draw(StartMenutex, Vector2.Zero, Color.White);
            sb.End();
            base.Draw(gameTime);
        }
    }
}
