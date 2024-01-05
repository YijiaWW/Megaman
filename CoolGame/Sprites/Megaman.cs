using CoolGame.Model;
using CoolGame.Type;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace CoolGame.Sprites
{
    /// <summary>
    /// Represents the player character Megaman, handling movements, shooting, animations, and hit responses.
    /// </summary>
    public class Megaman : Animation
    {
		private SpriteBatch spriteBatch;
		private Texture2D runTexture, standTexture, jumpTexture, shootTexture, runAndShootTexture, jumpShotTexture, bornTexture, deathTexture;
		private Vector2 position;
		private Vector2 shotDimensions;
		private List<Rectangle> runFrames, standFrames, jumpFrames, shootFrames, runAndShootFrames, jumpShotFrames, bornFrames, deathFrames;
		private const int ROWS = 1;
		private const int SHOTCOLUMNS = 3;
		private int frameIndex = 0;
		private int delay, delayCounter;
		private float gravity = 9.8f; // Game's gravity, adjust as needed
		private float jumpVelocity;
		private float jumpSpeed = 7f; // Adjust as needed for jump height
		private bool isJumping;
		private float groundLevel; // The ground level for Megaman to stand on
		private SpriteEffects spriteEffect = SpriteEffects.None;
		private bool facingRight = true;
		public List<Rectangle> currentAnimation;
		private Texture2D currentTexture;
		private Texture2D bulletTexture;
		private float shootTimer = 0f;
		public float shotGunTimer = 0f;
		public float shotGunInterval = 10f;
		private const float shootInterval = 0.3f;
		private KeyboardState keyboardState;
		private KeyboardState previousKeyboardState;
        private SoundEffect jumpSound;
        private SoundEffect shotSound;
        private List<Bullet> activeBullets = new List<Bullet>();
        private bool bornAnimationPlayed = false;
        public bool isDead = false;
        public int megamanCollisionCount = 0;
        public int MegamanMaxCollisionCount = 5;
        public bool scoreSaved = false;
        public int Score = 0;
        public float survivalTime = 0;
        public int NumberDestroyedEnemy = 0;
        private Game1 game;
        public bool isShotGun=false;
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
                    Rectangle frame = currentAnimation[frameIndex];

                    return new Rectangle(
                        (int)(position.X - frame.Width / 2),
                        (int)(position.Y - frame.Height / 2),
                        frame.Width,
                        frame.Height
                    );
                }
                return Rectangle.Empty;
            }
        }
        public Vector2 Position { get => position; set => position = value; }

        /// <summary>
        /// Initializes a new instance of the Megaman class.
        /// </summary>
        /// <param name="game">The game object this character is associated with.</param>
        /// <param name="spriteBatch">The SpriteBatch used for drawing the character.</param>
        /// <param name="position">The starting position of the character.</param>
        /// <param name="delay">The delay between animation frames.</param>
        /// <param name="runTexture">The texture for running animation.</param>
        /// <param name="standTexture">The texture for standing animation.</param>
        /// <param name="jumpTexture">The texture for jumping animation.</param>
        /// <param name="shootTexture">The texture for shooting animation.</param>
        /// <param name="runAndShootTexture">The texture for running and shooting animation.</param>
        /// <param name="bullet">The texture for bullets.</param>
        /// <param name="jumpShotTexture">The texture for jump shooting animation.</param>
        /// <param name="shotSound">Sound effect for shooting.</param>
        /// <param name="jumpSound">Sound effect for jumping.</param>
        /// <param name="bornTexture">The texture for birth animation.</param>
        /// <param name="deathTexture">The texture for death animation.</param>
        public Megaman(Game game, SpriteBatch spriteBatch, Vector2 position, int delay,
                       Texture2D runTexture, Texture2D standTexture, Texture2D jumpTexture,
                       Texture2D shootTexture, Texture2D runAndShootTexture, Texture2D bullet,
                       Texture2D jumpShotTexture, SoundEffect shotSound, SoundEffect jumpSound,
                       Texture2D bornTexture, Texture2D deathTexture) : base(game)
        {
            this.game = (Game1)game;
            this.spriteBatch = spriteBatch;
            this.position = position;
            this.delay = delay;

            // Textures
            this.runTexture = runTexture;
            this.standTexture = standTexture;
            this.jumpTexture = jumpTexture;
            this.shootTexture = shootTexture;
            this.runAndShootTexture = runAndShootTexture;
            this.jumpShotTexture = jumpShotTexture;
            this.shotSound = shotSound;
            this.jumpSound = jumpSound;
            this.bornTexture = bornTexture;
            this.bulletTexture = bullet;
            this.deathTexture = deathTexture;
            // Dimensions for each animation based on its texture and frame count
            shotDimensions = new Vector2(shootTexture.Width / SHOTCOLUMNS, shootTexture.Height / ROWS);

            // Initialize the frame lists
            runFrames = CreateRunFramesList(runTexture);
            standFrames = CreateStandFramesList(standTexture);
            jumpFrames = CreateJumpFramesList(jumpTexture);
            shootFrames = CreateShotFramesList(shootTexture, SHOTCOLUMNS, shotDimensions);
            runAndShootFrames = CreateRunAndShootFramesList(runAndShootTexture);
            jumpShotFrames = CreateJumpShotFramesList(jumpShotTexture);
            bornFrames = CreateBirthAnimationFrames(bornTexture);
            deathFrames = CreateDeathFramesList(deathTexture);
            // Set the ground level to the bottom of the screen

            currentTexture = this.standTexture;
            currentAnimation = this.standFrames;
            groundLevel = game.GraphicsDevice.Viewport.Height - currentAnimation[0].Height-50;
            
        }

        /// <summary>
        /// Triggers the death animation for Megaman.
        /// </summary>
        public void TriggerDeath()
        {
            if (!isDead)
            {
                isDead = true;
                frameIndex = 0; // Start from the first frame of death animation
                currentAnimation = deathFrames;
                currentTexture = deathTexture;
            }
        }
        /// <summary>
        /// Creates a list of rectangles representing frames for the birth animation.
        /// </summary>
        /// <param name="texture">The texture containing the birth animation frames.</param>
        /// <returns>A list of rectangles, each representing a frame in the birth animation.</returns>
        private List<Rectangle> CreateBirthAnimationFrames(Texture2D texture)
        {
            List<Rectangle> framesList = new List<Rectangle>();

            int[] frameStarts = new int[] { 0, 12, 31, 56, 91, 159, 243, 322, 402, 482, 562, 634, 711 };
            int[] frameWidths = new int[] { 11, 19, 25, 35, 69, 85, 80, 80, 80, 80, 80, 82 };

            for (int i = 0; i < frameWidths.Length; i++)
            {
                int start = frameStarts[i];
                int width = frameWidths[i];
                framesList.Add(new Rectangle(start, 0, width, texture.Height));
            }

            return framesList;
        }

        /// <summary>
        /// Creates a list of rectangles representing frames for the death animation.
        /// </summary>
        /// <param name="texture">The texture containing the death animation frames.</param>
        /// <returns>A list of rectangles, each representing a frame in the death animation.</returns>
        private List<Rectangle> CreateDeathFramesList(Texture2D texture)
        {
            List<Rectangle> framesList = new List<Rectangle>();
            int[] frameStarts = new int[] { 0, 80, 151, 223, 301, 380, 457, 539 };
            int[] frameWidths = new int[] { 80, 72, 72, 77, 77, 77, 80, 80 };

            for (int i = 0; i < frameWidths.Length; i++)
            {
                framesList.Add(new Rectangle(frameStarts[i], 0, frameWidths[i], texture.Height));
            }

            return framesList;
        }

        /// <summary>
        /// Creates a list of rectangles representing frames for the jumping animation.
        /// </summary>
        /// <param name="texture">The texture containing the jumping animation frames.</param>
        /// <returns>A list of rectangles, each representing a frame in the jumping animation.</returns>
        private List<Rectangle> CreateJumpFramesList(Texture2D texture)
		{
			List<Rectangle> framesList = new List<Rectangle>();
			int[] frameWidths = new int[] { 88, 88, 85, 74, 71, 74 }; // Specific frame widths
			int x = 0; // Start at the beginning of the row

			for (int i = 0; i < frameWidths.Length; i++)
			{
				int width = frameWidths[i];
				framesList.Add(new Rectangle(x, 0, width, texture.Height / ROWS));
				x += width; // Move to the start of the next frame
			}

			return framesList;
		}

        /// <summary>
        /// Creates a list of rectangles representing frames for the shooting animation.
        /// </summary>
        /// <param name="texture">The texture containing the shooting animation frames.</param>
        /// <param name="columns">The number of columns in the shooting animation texture.</param>
        /// <param name="dimensions">The dimensions of each frame in the texture.</param>
        /// <returns>A list of rectangles, each representing a frame in the shooting animation.</returns>
        private List<Rectangle> CreateShotFramesList(Texture2D texture, int columns, Vector2 dimensions)
		{
			List<Rectangle> framesList = new List<Rectangle>();
			for (int i = 0; i < columns; i++)
			{
				framesList.Add(new Rectangle(i * (int)dimensions.X, 0, (int)dimensions.X, (int)dimensions.Y));
			}
			return framesList;
		}
        private List<Rectangle> CreateJumpShotFramesList(Texture2D texture)
        {
            List<Rectangle> framesList = new List<Rectangle>();
            int[] frameWidths = new int[] { 85, 90, 90, 90 }; 
            int x = 0;  

            for (int i = 0; i < frameWidths.Length; i++)
            {
                int width = frameWidths[i];
                framesList.Add(new Rectangle(x, 0, width, texture.Height / ROWS));
                x += width; 
            }

            return framesList;
        }
        private List<Rectangle> CreateRunFramesList(Texture2D texture)
		{
			List<Rectangle> framesList = new List<Rectangle>();
			// Array with the specific widths of each frame
			int[] frameWidths = new int[] { 74, 65, 58, 68, 85, 81, 65, 53, 80, 87, 83, 61 };
			int x = 0; // Start at the beginning of the row

			for (int i = 0; i < frameWidths.Length; i++)
			{
				int width = frameWidths[i];
				framesList.Add(new Rectangle(x, 0, width, texture.Height / ROWS));
				x += width; // Move to the start of the next frame
			}

			return framesList;
		}
		private List<Rectangle> CreateRunAndShootFramesList(Texture2D texture)
		{
			List<Rectangle> framesList = new List<Rectangle>();
			int[] frameWidths = new int[] { 84, 84, 84, 84, 90, 90, 90, 79, 79, 94, 94, 87 };
			int x = 0; // Start at the beginning of the row

			for (int i = 0; i < frameWidths.Length; i++)
			{
				int width = frameWidths[i];
				framesList.Add(new Rectangle(x, 0, width, texture.Height / ROWS));
				x += width; // Move to the start of the next frame
			}

			return framesList;
		}
		private List<Rectangle> CreateStandFramesList(Texture2D texture)
		{
			List<Rectangle> framesList = new List<Rectangle>();
			int[] frameWidths = new int[] { 78, 78, 78, 78, 72, 68, 68, 68 }; // Frame widths as specified
			int x = 0; // Start at the beginning of the row

			for (int i = 0; i < frameWidths.Length; i++)
			{
				int width = frameWidths[i];
				framesList.Add(new Rectangle(x, 0, width, texture.Height / ROWS));
				x += width; // Move to the start of the next frame
			}

			return framesList;
		}

        /// <summary>
        /// Updates the state of Megaman, handling movements, animations, and shooting.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();
            if (isDead)
            {
                currentAnimation = deathFrames;
                currentTexture = deathTexture;
                delayCounter++;
                if (delayCounter > delay)
                {
                    frameIndex++;
                    if (frameIndex >= currentAnimation.Count)
                    {
                        // Death animation finished, remove Megaman or trigger game over logic
                        Game.Components.Remove(this);
                        return; // Exit the update method
                    }
                    delayCounter = 0;
                }
            }
            else if (!bornAnimationPlayed)
            {
                currentAnimation = bornFrames;
                currentTexture = bornTexture;
                delayCounter++;
                if (delayCounter > delay)
                {
                    frameIndex++;
                    if (frameIndex >= currentAnimation.Count)
                    {
                        bornAnimationPlayed = true;
                        currentAnimation = standFrames;
                        currentTexture = standTexture;
                        frameIndex = 0;
                    }
                    delayCounter = 0;
                }
            }
            else
            {
                if (isShotGun)
                {
                    shotGunTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (shotGunTimer >= shotGunInterval)
                    {
                        isShotGun = false;
                        shotGunTimer = 0f;
                    }
                }
                survivalTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                var previousAnimation = currentAnimation;

                // Jump logic
                if (keyboardState.IsKeyDown(Keys.W) && !isJumping)
                {
                    isJumping = true;
                    jumpVelocity = -jumpSpeed;
                    currentAnimation = jumpFrames;
                    currentTexture = jumpTexture;
                    // Play jump sound
                    jumpSound.Play();
                }

                if (isJumping)
                {
                    position.Y += jumpVelocity;
                    jumpVelocity += gravity * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (position.Y >= groundLevel)
                    {
                        position.Y = groundLevel;
                        isJumping = false;
                        // Set to standing animation if not moving left or right
                        if (!keyboardState.IsKeyDown(Keys.A) && !keyboardState.IsKeyDown(Keys.D))
                        {
                            currentAnimation = standFrames;
                            currentTexture = standTexture;
                        }
                    }
                    else if (keyboardState.IsKeyDown(Keys.J))
                    {
                      
                        // Play jump shooting animation while in the air
                        currentAnimation = jumpShotFrames;
                        currentTexture = jumpShotTexture;
                        HandleShooting(gameTime);
                    }
                }

                // Check for combined running and shooting
                if (keyboardState.IsKeyDown(Keys.D) && keyboardState.IsKeyDown(Keys.J))
                {
                    facingRight = true;
                    currentAnimation = runAndShootFrames;
                    currentTexture = runAndShootTexture;

                    spriteEffect = SpriteEffects.None; // Sprite faces right

                    if (currentAnimation != null && currentAnimation.Count > 0)
                    {
                        frameIndex = Math.Min(frameIndex, currentAnimation.Count - 1);
                        
                        int spriteWidth = currentAnimation[frameIndex].Width;
                        float newPosX = Math.Min(game.GraphicsDevice.Viewport.Width - spriteWidth, position.X + 2);
                        position.X = newPosX;
                    }
                    HandleShooting(gameTime);
                }
                else if (keyboardState.IsKeyDown(Keys.A) && keyboardState.IsKeyDown(Keys.J))
                {
                    facingRight = false;
                    currentAnimation = runAndShootFrames;
                    currentTexture = runAndShootTexture;
                    spriteEffect = SpriteEffects.FlipHorizontally; // Flip sprite to face left
                    float newPosX = Math.Max(0, position.X - 2);
                    position.X = newPosX;
                    HandleShooting(gameTime);
                }
                // Single key movement logic
                else if (keyboardState.IsKeyDown(Keys.A))
                {
                    facingRight = false;
                    float newPosX = Math.Max(0, position.X - 2);  
                    position.X = newPosX;  
                    if (!isJumping) // Only switch to running if not jumping
                    {
                        currentAnimation = runFrames;
                        currentTexture = runTexture;
                        spriteEffect = SpriteEffects.FlipHorizontally; // Flip sprite to face left
                    }
                }
                else if (keyboardState.IsKeyDown(Keys.D))
                {
                    facingRight = true;
                    if (currentAnimation != null && currentAnimation.Count > 0)
                    {
                        frameIndex = Math.Min(frameIndex, currentAnimation.Count - 1);
                        int spriteWidth = currentAnimation[frameIndex].Width;
                        float newPosX = Math.Min(game.GraphicsDevice.Viewport.Width - spriteWidth, position.X + 2);
                        position.X = newPosX;
                    }

                    if (!isJumping) // Only switch to running if not jumping
                    {
                        currentAnimation = runFrames;
                        currentTexture = runTexture;
                        spriteEffect = SpriteEffects.None; 
                    }
                }
                else if (keyboardState.IsKeyDown(Keys.J))
                {
                    if (!isJumping) // Only switch to shooting if not jumping
                    {
                        currentAnimation = shootFrames;
                        currentTexture = shootTexture;
                        HandleShooting(gameTime);
                    }
                }
                else if (!isJumping) // Default to standing if no keys pressed and not jumping
                {
                    currentAnimation = standFrames;
                    currentTexture = standTexture;
                    spriteEffect = facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                }

                // Reset frame index if animation has changed
                if (currentAnimation != previousAnimation)
                {
                    frameIndex = 0;
                }

                // Animation frame update
                delayCounter++;
                if (delayCounter > delay)
                {
                    frameIndex++;
                    if (frameIndex >= currentAnimation.Count) // Check bounds for current animation
                    {
                        frameIndex = 0; // Reset to beginning of animation
                    }
                    delayCounter = 0;
                }
                previousKeyboardState = keyboardState;
            }
            UpdateHitFlicker(gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// Handles the shooting of bullets.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        private void HandleShooting(GameTime gameTime)
		{
			// Update shoot timer
			if (shootTimer > 0)
			{
				shootTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
			}

			// Check for single or continuous shooting
			if (keyboardState.IsKeyDown(Keys.J) && !previousKeyboardState.IsKeyDown(Keys.J))
			{
				// Single shot
				Shoot();
                shotSound.Play();
                shootTimer = shootInterval; // Reset shoot timer for single shot
			}
			else if (keyboardState.IsKeyDown(Keys.J) && shootTimer <= 0)
			{
				// Continuous shooting, but with interval
				Shoot();
                shotSound.Play();
                shootTimer = shootInterval; // Reset shoot timer for continuous shooting
			}
		}

        /// <summary>
        /// Draws Megaman on the screen.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values, used for frame-based animation.</param>
        public override void Draw(GameTime gameTime)
		{
            if (isHitFlickering && !isVisibleDuringFlicker)
            {
                return;
            }
            spriteBatch.Begin();
            if (frameIndex >= 0 && frameIndex < currentAnimation.Count)
            {
                spriteBatch.Draw(
                    currentTexture,
                    Position,
                    currentAnimation[frameIndex],
                    Color.White,
                    0f,
                    Vector2.Zero,
                    1f,
                    spriteEffect,
                    0f
                );
            }
            spriteBatch.End();

			base.Draw(gameTime);
		}

        /// <summary>
        /// Fires a bullet from Megaman's position.
        /// </summary>
        private void Shoot()
		{
			Vector2 bulletSpeed;
			Vector2 bulletPosition;
            float bulletAngle = 30f;
            if (currentAnimation.Count > 0 && frameIndex >= 0 && frameIndex < currentAnimation.Count)
			{
				Rectangle currentFrame = currentAnimation[frameIndex];
				Vector2 megamanSize = new Vector2(currentFrame.Width, currentFrame.Height);

				if (facingRight)
				{
					bulletSpeed = new Vector2(10, 0);
					bulletPosition = new Vector2(position.X + megamanSize.X, position.Y + megamanSize.Y / 2 - 25);

				}
				else
				{
					bulletSpeed = new Vector2(-10, 0);
					bulletPosition = new Vector2(position.X, position.Y + megamanSize.Y / 2-25);
				}
				SpriteEffects effect = facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

				Bullet bullet = new Bullet(Game, spriteBatch, bulletTexture, bulletPosition, bulletSpeed,BulletType.MageBullet ,effect);
                activeBullets.Add(bullet);
                Game.Components.Add(bullet);
                if (isShotGun)
                {
                    Vector2 upperBulletSpeed = RotateVector(bulletSpeed, bulletAngle);
                    Bullet upperBullet = new Bullet(Game, spriteBatch, bulletTexture, bulletPosition, upperBulletSpeed, BulletType.MageBullet, effect);
                    activeBullets.Add(upperBullet);
                    Game.Components.Add(upperBullet);

                    Vector2 lowerBulletSpeed = RotateVector(bulletSpeed, -bulletAngle);
                    Bullet lowerBullet = new Bullet(Game, spriteBatch, bulletTexture, bulletPosition, lowerBulletSpeed, BulletType.MageBullet, effect);
                    activeBullets.Add(lowerBullet);
                    Game.Components.Add(lowerBullet);
                }
            }
		}
        /// <summary>
        /// Rotates a vector by a specified angle.
        /// </summary>
        /// <param name="vector">The vector to be rotated.</param>
        /// <param name="angle">The angle in degrees to rotate the vector.</param>
        /// <returns>The rotated vector.</returns>
        private Vector2 RotateVector(Vector2 vector, float angle)
        {
            float radians = MathHelper.ToRadians(angle);
            float cos = (float)Math.Cos(radians);
            float sin = (float)Math.Sin(radians);
            return new Vector2(vector.X * cos - vector.Y * sin, vector.X * sin + vector.Y * cos);
        }

    }
}
