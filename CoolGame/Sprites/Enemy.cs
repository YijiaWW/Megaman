using CoolGame.Type;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolGame.Sprites
{
    /// <summary>
    /// Represents an enemy in the game, handling its movement, shooting, animations, and hit responses.
    /// </summary>
    public class Enemy : Animation
    {
        public Vector2 position;
        protected SpriteBatch spriteBatch;
        protected int delay, delayCounter;
        private Texture2D tankEnemyTexture, tankExpolisionTexture;
        private List<Rectangle> tankEnemyFrames, tankExpolisionFrames;
        public List<Rectangle> currentAnimation;
        protected Texture2D currentTexture;
        protected Texture2D bulletTexture;
        protected int frameIndex = 0;
        protected float shootTimer = 0f;
        protected const float shootInterval = 1f;
        protected float moveSpeed = 2f;
        private SpriteEffects spriteEffect = SpriteEffects.None;
        public int HitCount = 0;
        protected List<Bullet> activeBullets = new List<Bullet>();
        protected bool isExploding = false;
        public bool isDestroyed = false;
        protected SoundEffect tankExplosion;
        protected SoundEffect enemyShot;
        private Game1 game;
        public List<Bullet> GetActiveBullets()
        {
            return activeBullets;
        }

        public Rectangle Bounds
        {
            get
            {
                if (frameIndex >= 0 && frameIndex < currentAnimation.Count)
                {
                    Rectangle currentFrame = currentAnimation[frameIndex];

                    return new Rectangle(
                        (int)position.X,
                        (int)position.Y,
                        currentFrame.Width,
                        currentFrame.Height
                    );
                }

                return Rectangle.Empty;
            }
        }
        /// <summary>
        /// Initializes a new instance of the Enemy class.
        /// </summary>
        /// <param name="game">The game object this enemy is associated with.</param>
        /// <param name="spriteBatch">The SpriteBatch used for drawing the enemy.</param>
        /// <param name="position">The starting position of the enemy.</param>
        /// <param name="delay">The delay between animation frames.</param>
        /// <param name="tankEnemyTexture">The texture for the tank enemy.</param>
        /// <param name="tankExpolisionTexture">The texture for the tank explosion.</param>
        /// <param name="bullet">The texture for the bullets.</param>
        /// <param name="tankExplosion">Sound effect for the tank explosion.</param>
        /// <param name="enemyShot">Sound effect for enemy shooting.</param>
        public Enemy(Game game, SpriteBatch spriteBatch, Vector2 position, int delay, Texture2D tankEnemyTexture, Texture2D tankExpolisionTexture, Texture2D bullet, SoundEffect tankExplosion, SoundEffect enemyShot) : base(game)
        {
            this.game = (Game1)game;
            this.spriteBatch = spriteBatch;
            this.position = position;
            this.delay = delay;
            this.tankExplosion = tankExplosion;
            this.tankEnemyTexture = tankEnemyTexture;
            this.tankExpolisionTexture = tankExpolisionTexture;
            this.bulletTexture = bullet;
            tankEnemyFrames = CreateTankFramesList(tankEnemyTexture);
            tankExpolisionFrames = CreateExpolisionTankFramesList(tankExpolisionTexture);
            currentTexture = this.tankEnemyTexture;
            currentAnimation = this.tankEnemyFrames;
            spriteEffect = SpriteEffects.FlipHorizontally;
            this.enemyShot = enemyShot;

            
        }
        /// <summary>
        /// Creates a list of frames for the tank explosion animation.
        /// </summary>
        /// <param name="tankExplosionTexture">The texture containing the explosion animation frames.</param>
        /// <returns>A list of rectangles, each representing a frame in the explosion animation.</returns>
        private List<Rectangle> CreateExpolisionTankFramesList(Texture2D tankExpolisionTexture)
        {
            List<Rectangle> framesList = new List<Rectangle>();

            int[] frameStarts = new int[] { 0, 182, 368, 556, 756, 973, 1142, 1324, 1509, 1677, 1850 };   
            int[] frameWidths = new int[] { 134, 134, 134, 155, 170, 151, 122, 129, 109, 109, 97 };     

            for (int i = 0; i < frameWidths.Length; i++)
            {
                int start = frameStarts[i];
                int width = frameWidths[i];
                framesList.Add(new Rectangle(start, 0, width, tankEnemyTexture.Height));
            }

            return framesList;
        }

        /// <summary>
        /// Creates a list of frames for the tank enemy animation.
        /// </summary>
        /// <param name="tankEnemyTexture">The texture containing the tank enemy animation frames.</param>
        /// <returns>A list of rectangles, each representing a frame in the tank enemy animation.</returns>
        private List<Rectangle> CreateTankFramesList(Texture2D tankEnemyTexture)
        {
            List<Rectangle> framesList = new List<Rectangle>();

            int[] frameStarts = new int[] { 0, 163, 354, 589 }; 
            int[] frameWidths = new int[] { 146, 166, 160, 143 }; 

            for (int i = 0; i < frameWidths.Length; i++)
            {
                int start = frameStarts[i];
                int width = frameWidths[i];
                framesList.Add(new Rectangle(start, 0, width, tankEnemyTexture.Height));
            }

            return framesList;
        }
        /// <summary>
        /// Updates the enemy's state, including movement and shooting.
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
                    ShootBullet();
                    shootTimer = shootInterval;
                }

                frameIndex++;
                if (frameIndex >= tankEnemyFrames.Count)
                {
                    frameIndex = 0;
                }
            }

            if (HitCount >= 3 && !isExploding)
            {
                isExploding = true;
                frameIndex = 0;
                tankExplosion.Play();
                game.actionScene.Megaman.NumberDestroyedEnemy++;
                game.level2ActionScene.Megaman.NumberDestroyedEnemy++;
                currentAnimation = tankExpolisionFrames;
                currentTexture = tankExpolisionTexture;
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
            UpdateHitFlicker(gameTime);
            base.Update(gameTime);
        }
        /// <summary>
        /// Shoots a bullet. This method can be overridden for different enemy types.
        /// </summary>
        protected virtual void ShootBullet()
        {
            Vector2 bulletPosition = new Vector2(position.X, position.Y+22); 
            Vector2 bulletSpeed = new Vector2(-5, 0); 
            Bullet bullet = new Bullet(Game, spriteBatch, bulletTexture, bulletPosition, bulletSpeed, BulletType.EnemyBullet);
            enemyShot.Play();
            activeBullets.Add(bullet);
            Game.Components.Add(bullet);
        }

        /// <summary>
        /// Shoots a bullet in the specified direction. This method can be overridden for different enemy behaviors.
        /// </summary>
        /// <param name="direction">The direction vector in which the bullet will be shot.</param>
        protected virtual void ShootBullet(Vector2 direction)
        {
            Vector2 bulletPosition = new Vector2(position.X, position.Y + 22);
            Vector2 bulletSpeed = new Vector2(-5, 0);
            Bullet bullet = new Bullet(Game, spriteBatch, bulletTexture, bulletPosition, bulletSpeed, BulletType.EnemyBullet);
            enemyShot.Play();
            activeBullets.Add(bullet);
            Game.Components.Add(bullet);
        }
        /// <summary>
        /// Draws the enemy on the screen.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values, used for frame-based animation.</param>
        public override void Draw(GameTime gameTime)
        {
            if (isHitFlickering && !isVisibleDuringFlicker)
            {   
                return;
            }
            spriteBatch.Begin();
            if (!isDestroyed && frameIndex < currentAnimation.Count) // Check to avoid index out of range
            {
                spriteBatch.Draw(
                    currentTexture,
                    position,
                    currentAnimation[frameIndex],
                    Color.White,
                    0f,
                    Vector2.Zero,
                    1f,
                    spriteEffect,
                    0f);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
