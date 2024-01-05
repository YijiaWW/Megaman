using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolGame.Controls
{
    /// <summary>
    /// Represents a clickable button component in the game.
    /// </summary>
    public class Button : Component
    {
        #region Fields
        private MouseState _currentMouse;
        private SpriteFont _font;
        private bool _isHovering;
        private MouseState _previousMouse;
        private Texture2D _texture;
        private bool _selectSoundPlayed;
        #endregion

        /// <summary>
        /// Occurs when the button is clicked.
        /// </summary>
        public event EventHandler Click;

        /// <summary>
        /// Gets a value indicating whether the button was clicked.
        /// </summary>
        public bool Clicked { get; private set; }

        /// <summary>
        /// Gets or sets the color of the button text.
        /// </summary>
        public Color PenColour { get; set; }

        /// <summary>
        /// Gets or sets the position of the button.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Gets the rectangle that represents the button's boundaries.
        /// </summary>
        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);
            }
        }

        /// <summary>
        /// Gets or sets the text displayed on the button.
        /// </summary>
        public string Text { get; set; }

		private SoundEffect _selectSound;
		private SoundEffect _clickSound;

        /// <summary>
        /// Initializes a new instance of the Button class with textures and fonts.
        /// </summary>
        /// <param name="texture">The texture of the button.</param>
        /// <param name="font">The font for the button text.</param>
        /// <param name="selectSound">The sound effect played when the button is selected.</param>
        /// <param name="clickSound">The sound effect played when the button is clicked.</param>
        public Button(Texture2D texture, SpriteFont font, SoundEffect selectSound, SoundEffect clickSound)
        {
            _texture = texture;
            _font = font;
            PenColour = Color.Black;
            _selectSound = selectSound;
			_clickSound = clickSound;

		}

        /// <summary>
        /// Draws the button on the screen.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="spriteBatch">The SpriteBatch used for drawing sprites.</param>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var colour = Color.White;
            if (_isHovering)
                colour = Color.Gray;
            spriteBatch.Draw(_texture, Rectangle, colour);
            if (!string.IsNullOrEmpty(Text))
            {
                var x = (Rectangle.X + (Rectangle.Width / 2)) - (_font.MeasureString(Text).X / 2);
                var y = (Rectangle.Y + (Rectangle.Height / 2)) - (_font.MeasureString(Text).Y / 2); 
                spriteBatch.DrawString(_font, Text, new Vector2(x, y), PenColour);
            }
        }

        /// <summary>
        /// Updates the button's state, including hover and click detection.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
		{
			_previousMouse = _currentMouse;
			_currentMouse = Mouse.GetState();

			var mouseRectangle = new Rectangle(_currentMouse.X, _currentMouse.Y, 1, 1);

			_isHovering = false;

			if (mouseRectangle.Intersects(Rectangle))
			{
				if (!_selectSoundPlayed)
				{
					_selectSound.Play();
					_selectSoundPlayed = true;
				}

				_isHovering = true;

				if (_currentMouse.LeftButton == ButtonState.Released && _previousMouse.LeftButton == ButtonState.Pressed)
				{
					_clickSound.Play();
					Click?.Invoke(this, new EventArgs());
				}
			}
			else
			{
				// Reset the flag when the mouse is not over the button
				_selectSoundPlayed = false;
			}
		}
	}
}
