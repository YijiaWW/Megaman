using CoolGame.Type;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct2D1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolGame.Sprites
{
    /// <summary>
    /// Represents a plane enemy in the game, handling its movement, shooting, and rendering.
    /// </summary>
    public class PlaneEnemy : Enemy
    {
        private Vector2 shootDirection;
        private const float planeFlyingHeight = 150;
        private float bulletSpeedMagnitude = 5f;
        private float moveSpeed = 3f;
        private Game1 game;

        /// <summary>
        /// Initializes a new instance of the PlaneEnemy class.
        /// </summary>
        /// <param name="game">The game object this enemy is associated with.</param>
        /// <param name="spriteBatch">The SpriteBatch used for drawing sprites.</param>
        /// <param name="position">The starting position of the plane enemy.</param>
        /// <param name="delay">The delay between animation frames.</param>
        /// <param name="tankEnemyTexture">The texture for the tank enemy.</param>
        /// <param name="tankExpolisionTexture">The texture for the tank explosion.</param>
        /// <param name="bullet">The texture for the bullets.</param>
        /// <param name="tankExplosion">Sound effect for the tank explosion.</param>
        /// <param name="enemyShot">Sound effect for enemy shooting.</param>
        /// <param name="planeTexture">The texture for the plane enemy.</param>
        public PlaneEnemy(Game game, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Vector2 position,
            int delay, Texture2D tankEnemyTexture, Texture2D tankExpolisionTexture, Texture2D bullet, 
            SoundEffect tankExplosion, SoundEffect enemyShot, Texture2D planeTexture) : base(game, spriteBatch, position, delay, tankEnemyTexture, tankExpolisionTexture, bullet, tankExplosion, enemyShot)
        {
            this.game = (Game1)game;
            this.position.Y = planeFlyingHeight;
            currentTexture = planeTexture;
            shootDirection = new Vector2(-1, 1);
            shootDirection.Normalize();
        }
        /// <summary>
        /// Updates the plane enemy's state, including movement and shooting.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (!isDestroyed && !isExploding)
            {
                position.X -= moveSpeed;
                if (position.X < 0)
                {
                    position.X = Game.GraphicsDevice.Viewport.Width;
                }

                shootTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (shootTimer <= 0)
                {
                    ShootBullet(shootDirection);
                    shootTimer = shootInterval;
                }

                UpdateHitFlicker(gameTime);
            }

            if (HitCount >= 3 && !isExploding)
            {
                isExploding = true;
                frameIndex = 0;
                tankExplosion.Play();
                game.actionScene.Megaman.NumberDestroyedEnemy++;
                activeBullets.Clear();
            }

            if (isExploding)
            {
                delayCounter++;
                if (delayCounter > delay)
                {
                    frameIndex++;
                    if (frameIndex >= currentAnimation.Count)
                    {
                        isDestroyed = true;
                        Game.Components.Remove(this);
                    }
                    delayCounter = 0;
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Shoots a bullet in the specified direction.
        /// </summary>
        /// <param name="direction">The direction in which to shoot the bullet.</param>
        protected override void ShootBullet(Vector2 direction)
        {
            Vector2 bulletPosition = new Vector2(position.X, position.Y);
            Vector2 bulletSpeed = direction * bulletSpeedMagnitude;
            Bullet bullet = new Bullet(Game, spriteBatch, bulletTexture, bulletPosition, bulletSpeed, BulletType.EnemyBullet);
            enemyShot.Play();
            activeBullets.Add(bullet);
            Game.Components.Add(bullet);
        }

        /// <summary>
        /// Draws the plane enemy on the screen.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values, used for frame-based animation.</param>
        public override void Draw(GameTime gameTime)
        {
            if (isHitFlickering && !isVisibleDuringFlicker)
            {
                return;
            }
            spriteBatch.Begin();
            spriteBatch.Draw(currentTexture, position, Color.White);
            spriteBatch.End();
        }
    }
}

