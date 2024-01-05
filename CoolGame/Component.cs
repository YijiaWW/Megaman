using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolGame
{
    /// <summary>
    /// Serves as the base class for all game components, providing a framework for updating logic and rendering.
    /// </summary>
    public abstract class Component
    {
        /// <summary>
        /// Draws the game component.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values, used for frame-based animation.</param>
        /// <param name="spriteBatch">The SpriteBatch used for drawing sprites in 2D.</param>
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
        /// <summary>
        /// Updates the game component's state.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public abstract void Update(GameTime gameTime);
    }
}
