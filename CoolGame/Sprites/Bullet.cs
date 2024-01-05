using CoolGame.Type;
using Microsoft.Xna.Framework;

using Microsoft.Xna.Framework.Graphics;


using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace CoolGame.Sprites
{
    /// <summary>
    /// Represents a bullet in the game, managing its movement and rendering.
    /// </summary>
    public class Bullet : DrawableGameComponent
    {
        private SpriteBatch spriteBatch;
        private Texture2D BulletTexture;
        private Vector2 position;
        private Vector2 speed;
        public BulletType bulletType;
        private List<Rectangle> MageBulletFrame;
        private List<Rectangle> TankBulletFrame;
        private SpriteEffects spriteEffect = SpriteEffects.None;
        public bool hasHit = false;

        /// <summary>
        /// Gets the bounding box of the bullet, used for collision detection.
        /// </summary>
        public Rectangle Bounds
        {
            get
            {
                Rectangle frame = bulletType == BulletType.MageBullet ? MageBulletFrame[0] : TankBulletFrame[0];

                return new Rectangle(
                    (int)position.X,
                    (int)position.Y,
                    frame.Width,
                    frame.Height
                );
            }
        }
        /// <summary>
        /// Initializes a new instance of the Bullet class.
        /// </summary>
        /// <param name="game">The game object this bullet is associated with.</param>
        /// <param name="spriteBatch">The SpriteBatch used for drawing the bullet.</param>
        /// <param name="bulletTexture">The texture of the bullet.</param>
        /// <param name="position">The starting position of the bullet.</param>
        /// <param name="speed">The speed vector of the bullet.</param>
        /// <param name="bulletType">The type of the bullet.</param>
        /// <param name="spriteEffect">Optional sprite effects.</param>
        public Bullet(Game game, SpriteBatch spriteBatch, Texture2D bulletTexture, Vector2 position, Vector2 speed, BulletType bulletType, SpriteEffects spriteEffect = default) : base(game)
        {
            this.spriteBatch = spriteBatch;
            BulletTexture = bulletTexture;
            this.position = position;
            this.speed = speed;
            this.bulletType = bulletType;
            this.spriteEffect = spriteEffect;
            this.MageBulletFrame = new List<Rectangle> { new Rectangle(211, 0, 49, bulletTexture.Height) };
            this.TankBulletFrame = new List<Rectangle> { new Rectangle(45, 0, 29, bulletTexture.Height) };
            
        }

        /// <summary>
        /// Draws the bullet on the screen.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values, used for frame-based animation.</param>
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            if (bulletType == BulletType.MageBullet)
            {
                spriteBatch.Draw(BulletTexture, position, MageBulletFrame[0], Color.White, 0f, Vector2.Zero, 1f, spriteEffect, 0f);
            }
            else if (bulletType == BulletType.EnemyBullet)
            {
                spriteBatch.Draw(BulletTexture, position, TankBulletFrame[0], Color.White, 0f, Vector2.Zero, 1f, spriteEffect, 0f);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
        /// <summary>
        /// Updates the bullet's state, including its position and removal when necessary.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (hasHit)
            {
                Game.Components.Remove(this);
                return;
            }

            position += speed;
            if (position.X < 0 || position.X > Game.GraphicsDevice.Viewport.Width)
            {
                Game.Components.Remove(this);
                hasHit = true;
            }
            base.Update(gameTime);
        }
    }
}
