using CoolGame.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolGame
{
    /// <summary>
    /// Represents a menu component in the game, containing a list of buttons.
    /// </summary>
    public class MenuComponent : DrawableGameComponent
	{
		private SpriteBatch sb;
		public List<Button> buttons;
		private Vector2 position;
        private const int ButtonLineSpacing = 60;
        private const int PositionOffsetX = 150;

        /// <summary>
        /// Initializes a new instance of the MenuComponent class.
        /// </summary>
        /// <param name="game">The game object this component is associated with.</param>
        /// <param name="sb">The SpriteBatch used for drawing.</param>
        /// <param name="tex">The texture for the buttons.</param>
        /// <param name="font">The font for the button text.</param>
        /// <param name="clicksound">The sound effect played on button click.</param>
        /// <param name="SelectSound">The sound effect played when a button is selected.</param>
        /// <param name="menus">An array of menu item names.</param>
        public MenuComponent(Game game,
			SpriteBatch sb, Texture2D tex, SpriteFont font,SoundEffect clicksound,SoundEffect SelectSound, string[] menus) : base(game)
		{
			this.sb = sb;
			buttons = new List<Button>();
            int LineSpacing = ButtonLineSpacing;
            position = new Vector2(Shared.stage.X / 4 - PositionOffsetX, Shared.stage.Y / 4);


            for (int i = 0; i < menus.Length; i++)
			{
				var button = new Button(tex,font, SelectSound, clicksound);
				button.Text = menus[i];
				button.Position = new Vector2(position.X, position.Y + i * LineSpacing);
				buttons.Add(button);
			}
		}

        /// <summary>
        /// Updates the menu component, including its buttons.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
		{	
			foreach (var button in buttons)
			{
				button.Update(gameTime);
			}
			base.Update(gameTime);
		}
        /// <summary>
        /// Draws the menu component and its buttons.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values, used for frame-based animation.</param>
        public override void Draw(GameTime gameTime)
		{
			Vector2 tempPos = position;
			sb.Begin();

			foreach (var button in buttons)
			{
				button.Draw(gameTime, sb);
			}
			sb.End();

			base.Draw(gameTime);
		}
	}
}
