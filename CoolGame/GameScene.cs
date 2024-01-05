using CoolGame.Sprites;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolGame
{
    /// <summary>
    /// Represents a base class for different scenes in the game, such as menus or levels.
    /// </summary>
    public abstract class GameScene : DrawableGameComponent
    {
        private List<GameComponent> components;
        /// <summary>
        /// Gets or sets the components of the game scene.
        /// </summary>
        public List<GameComponent> Components { get => components; set => components = value; }

        /// <summary>
        /// Shows the game scene, making it visible and active.
        /// </summary>
        public virtual void show()
        {
            this.Enabled = true;
            this.Visible = true;
        }
        /// <summary>
        /// Hides the game scene, making it invisible and inactive.
        /// </summary>
        public virtual void hide()
        {
            this.Enabled = false;
            this.Visible = false;
        }
        /// <summary>
        /// Initializes a new instance of the GameScene class.
        /// </summary>
        /// <param name="game">The game object this scene is associated with.</param>
        protected GameScene(Game game) : base(game)
        {
            components = new List<GameComponent>();
            hide();

        }
        /// <summary>
        /// Updates the game scene and its components.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            foreach (GameComponent item in components)
            {
                if (item.Enabled)
                {
                    item.Update(gameTime);
                }
            }

            base.Update(gameTime);
        }
        /// <summary>
        /// Draws the game scene and its drawable components.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values, used for frame-based animation.</param>
        public override void Draw(GameTime gameTime)
        {
            foreach (GameComponent item in components)
            {
                if (item is DrawableGameComponent)
                {
                    DrawableGameComponent comp = (DrawableGameComponent)item;
                    if (comp.Visible )
                    {
                        comp.Draw(gameTime);
                    }
                }
            }

            base.Draw(gameTime);
        }
    }
}
