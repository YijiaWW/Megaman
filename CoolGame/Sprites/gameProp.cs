using CoolGame.Type;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolGame.Sprites
{
    /// <summary>
    /// Represents a game prop in the game, handling its rendering and movement.
    /// </summary>
    public class GameProp : DrawableGameComponent
    {
        private Texture2D texture;
        private Vector2 position;
        private float speed;
        private float waveAmplitude;
        private float waveFrequency;
        private float wavePhase;
        private SpriteBatch spriteBatch;
        public GamePropType gamePropType;
        public bool hasUsed;

        /// <summary>
        /// Gets the bounding box of the game prop, used for collision detection.
        /// </summary>
        public Rectangle Bounds
        {
            get
            {
                return new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
            }
        }

        /// <summary>
        /// Initializes a new instance of the GameProp class.
        /// </summary>
        /// <param name="game">The game object this prop is associated with.</param>
        /// <param name="spriteBatch">The SpriteBatch used for drawing the prop.</param>
        /// <param name="texture">The texture of the prop.</param>
        /// <param name="speed">The horizontal speed of the prop.</param>
        /// <param name="waveAmplitude">The amplitude of the wave-like vertical movement.</param>
        /// <param name="waveFrequency">The frequency of the wave-like vertical movement.</param>
        /// <param name="gamePropType">The type of the game prop.</param>   
        public GameProp(Game game, SpriteBatch spriteBatch, Texture2D texture, float speed, float waveAmplitude, float waveFrequency, GamePropType gamePropType) : base(game)
        {
            this.spriteBatch = spriteBatch;
            this.texture = texture;
            this.speed = speed;
            this.waveAmplitude = waveAmplitude;
            this.waveFrequency = waveFrequency;
            this.gamePropType = gamePropType;
            this.position = new Vector2(game.GraphicsDevice.Viewport.Width, Game1.ScreenHeight /4 - texture.Height/2);
            wavePhase = 0f;
        }

        /// <summary>
        /// Draws the game prop on the screen.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values, used for frame-based animation.</param>
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(texture, position, Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        /// <summary>
        /// Updates the game prop's state, including its position and wave-like movement.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            position.X -= speed;
            wavePhase += waveFrequency;
            float waveOffset = waveAmplitude * (float)Math.Sin(wavePhase);
            position.Y = Game1.ScreenHeight / 2 - texture.Height / 2 + waveOffset;

            if (position.X + texture.Width < 0)
            {
                Game.Components.Remove(this);
                hasUsed = true;
            }

            base.Update(gameTime);
        }
    }
}

