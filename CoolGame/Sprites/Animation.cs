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
    /// Manages animations for game components, including hit flicker effects.
    /// </summary>
    public class Animation : DrawableGameComponent
    {
        protected bool isHitFlickering = false;
        protected float hitFlickerDuration = 1.0f; 
        protected float hitFlickerTimer = 0.0f;
        protected float flickerVisibilityInterval = 0.1f;  
        protected bool isVisibleDuringFlicker = true;
        /// <summary>
        /// Initializes a new instance of the Animation class.
        /// </summary>
        /// <param name="game">The game object this animation is associated with.</param>
        public Animation(Game game) : base(game)
        {
        }
        /// <summary>
        /// Triggers the hit flicker effect.
        /// </summary>
        public void TriggerHitFlicker()
        {
            isHitFlickering = true;
            hitFlickerTimer = hitFlickerDuration;
            isVisibleDuringFlicker = true;
        }

        /// <summary>
        /// Updates the hit flicker effect, controlling its timing and visibility.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected void UpdateHitFlicker(GameTime gameTime)
        {
            if (!isHitFlickering) return;

            hitFlickerTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (hitFlickerTimer <= 0)
            {
                isHitFlickering = false;
                isVisibleDuringFlicker = true;
            }
            else
            {
                if (hitFlickerTimer % flickerVisibilityInterval < flickerVisibilityInterval / 2)
                {
                    isVisibleDuringFlicker = !isVisibleDuringFlicker;
                }
            }
        }

        /// <summary>
        /// Updates the animation's state.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            base.Draw(gameTime);
            UpdateHitFlicker(gameTime);
        }

        /// <summary>
        /// Draws the animation.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values, used for frame-based animation.</param>
        public override void Draw(GameTime gameTime)
        {
            if (!isVisibleDuringFlicker) return;
            base.Draw(gameTime);
        }
    }
}
