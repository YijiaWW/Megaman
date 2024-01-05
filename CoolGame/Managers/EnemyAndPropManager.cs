using CoolGame.Sprites;
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

namespace CoolGame.Managers
{
    /// <summary>
    /// Manages the enemies and props in the game. Responsible for spawning, updating, and managing their lifecycle.
    /// </summary>
    public class EnemyAndPropManager : DrawableGameComponent
    {
        private List<Enemy> activeEnemies; // List of all active enemies in the game.
        private Game1 game; // Reference to the main game object.
        private Texture2D enemyTexture, explosionTexture, bulletTexture; // Textures for enemy, explosion, and bullets.
        private SoundEffect explosionSound, shotSound; // Sound effects for explosion and shooting.
        private CollisionManager collisionManager; // Manages collision detection in the game.
        public bool IsActive { get; set; } = true; // Flag to check if the manager is active.

        // Timers and intervals for enemy spawning mechanics.
        private float enemySpawnTimer = 0f;
        private float enemySpawnInterval;
        private Texture2D planeTexture;
        private float totalSpawnTimer = 0f;
        private float totalSpawnInterval;

        // Textures and timers for game props.
        private Texture2D GamePropLife;
        private Texture2D GamePropBulletShotgun;
        private float lifePropSpawnTimer = 0f;
        private float shotGunPropSpawnTimer = 0f;
        private float shotGunPropSpawnInterval = 35;
        private float lifePropSpawnInterval = 40;
        private Random random = new Random();
        private bool isLevel2; // Flag to check if the game is at level 2.

        // Flags to check if specific types of enemies are destroyed.
        private bool tankEnemyDestroyed = false;
        private bool planeEnemyDestroyed = false;

        /// <summary>
        /// Constructor initializes the Enemy and Prop Manager with required components and textures.
        /// </summary>
        public EnemyAndPropManager(Game1 game, Texture2D enemyTexture, Texture2D explosionTexture,
                            Texture2D bulletTexture, SoundEffect explosionSound, SoundEffect shotSound,
                            CollisionManager collisionManager, Texture2D planeTexture, float enemySpawnInterval, float totalSpawnInterval) : base(game)
        {

            this.game = (Game1)game;
            this.enemyTexture = enemyTexture;
            this.explosionTexture = explosionTexture;
            this.bulletTexture = bulletTexture;
            this.explosionSound = explosionSound;
            this.shotSound = shotSound;
            this.collisionManager = collisionManager;
            this.planeTexture = planeTexture;
            this.enemySpawnInterval = enemySpawnInterval;
            this.totalSpawnInterval = totalSpawnInterval;
            activeEnemies = new List<Enemy>();
            GamePropLife = game.Content.Load<Texture2D>("Sprites/MegaManLife");
            GamePropBulletShotgun = game.Content.Load<Texture2D>("Images/GamePropShotGun");
        }

        /// <summary>
        /// Method to create a new tank enemy.
        /// </summary>
        private void CreateNewEnemy()
        {
            Vector2 position = new Vector2(game.GraphicsDevice.Viewport.Width,
                                           game.GraphicsDevice.Viewport.Height - enemyTexture.Height - 50);
            Enemy newEnemy = new Enemy(game, game._spriteBatch, position, 8,
                                       enemyTexture, explosionTexture, bulletTexture,
                                       explosionSound, shotSound);
            activeEnemies.Add(newEnemy);
            game.Components.Add(newEnemy);
            collisionManager.AddEnemy(newEnemy);
        }
        /// <summary>
        /// Method to create a new game prop, specifically for level 2.
        /// </summary>
        public void CreateNewGameProp()
        {
            if (isLevel2)
            {

                float speed = (float)random.NextDouble() * 1.5f + 1.5f;
                float waveAmplitude = random.Next(60, 100);
                float waveFrequency = (float)random.NextDouble() * 0.03f + 0.07f;

                GameProp newProp = new GameProp(Game, game._spriteBatch, GamePropLife, speed, waveAmplitude, waveFrequency, GamePropType.Life);
                game.Components.Add(newProp);
                collisionManager.AddGameProp(newProp);

            }
            isLevel2 = true;
        }
        /// <summary>
        /// Method to create a new shotgun game prop, specifically for level 2.
        /// </summary>
        public void CreateNewShotGunGameProp()
        {
            if (isLevel2)
            {

                float newspeed = (float)random.NextDouble() * 1.5f + 1.5f;
                float newwaveAmplitude = random.Next(60, 100);
                float newwaveFrequency = (float)random.NextDouble() * 0.03f + 0.07f;
                GameProp newGamePropBulletShotgun = new GameProp(Game, game._spriteBatch, GamePropBulletShotgun, newspeed, newwaveAmplitude, newwaveFrequency, GamePropType.BulletShotgun);
                game.Components.Add(newGamePropBulletShotgun);
                collisionManager.AddGameProp(newGamePropBulletShotgun);
            }

        }
        // <summary>
        /// Updates the enemy spawning intervals.
        /// </summary>
        /// <param name="newSpawnInterval">The new interval time for spawning a single enemy.</param>
        /// <param name="newTotalSpawnInterval">The new interval time for total enemy spawning.</param>
        public void UpdateEnemyManagerParams(float newSpawnInterval, float newTotalSpawnInterval)
        {
            enemySpawnInterval = newSpawnInterval;
            totalSpawnInterval = newTotalSpawnInterval;
        }

        /// <summary>
        /// Method to create a new plane enemy.
        /// </summary>
        private void CreateNewPlaneEnemy()
        {
            Vector2 position = new Vector2(game.GraphicsDevice.Viewport.Width, 200); // Height for plane enemy
            PlaneEnemy newPlaneEnemy = new PlaneEnemy(game, game._spriteBatch, position, 8,
                                                      enemyTexture, explosionTexture, bulletTexture,
                                                    explosionSound, shotSound, planeTexture);
            activeEnemies.Add(newPlaneEnemy);
            game.Components.Add(newPlaneEnemy);
            collisionManager.AddEnemy(newPlaneEnemy);

        }

        /// <summary>
        /// Clears all enemies from the scene.
        /// </summary>
        public void ClearEnemies()
        {
            foreach (var enemy in activeEnemies)
            {
                game.Components.Remove(enemy);
            }
            activeEnemies.Clear();
        }
        /// <summary>
        /// Updates the game component each frame, managing the spawning and lifecycle of enemies and props.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (!IsActive)
                return;

            // Update the total spawning timer
            totalSpawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Check if it's time to spawn both enemies
            if (totalSpawnTimer >= totalSpawnInterval)
            {
                CreateNewEnemy();
                CreateNewPlaneEnemy();
                totalSpawnTimer = 0f;
            }

            // Check if tank enemy needs to be respawned
            if (tankEnemyDestroyed)
            {
                enemySpawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (enemySpawnTimer >= enemySpawnInterval)
                {
                    CreateNewEnemy();
                    tankEnemyDestroyed = false;
                    enemySpawnTimer = 0f;
                }
            }

            // Check if plane enemy needs to be respawned
            if (planeEnemyDestroyed)
            {
                enemySpawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (enemySpawnTimer >= enemySpawnInterval)
                {
                    CreateNewPlaneEnemy();
                    planeEnemyDestroyed = false;
                    enemySpawnTimer = 0f;
                }
            }
            if (isLevel2)
            {
                lifePropSpawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (lifePropSpawnTimer >= lifePropSpawnInterval)
                {
                    CreateNewGameProp();
                    lifePropSpawnTimer = 0f;
                }
            }
            if (isLevel2)
            {
                shotGunPropSpawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (shotGunPropSpawnTimer >= shotGunPropSpawnInterval)
                {
                    CreateNewShotGunGameProp();
                    shotGunPropSpawnTimer = 0f;
                }
            }

            // Iterate and update each active enemy
            for (int i = activeEnemies.Count - 1; i >= 0; i--)
            {
                if (activeEnemies[i].isDestroyed)
                {
                    game.Components.Remove(activeEnemies[i]);
                    activeEnemies.RemoveAt(i);
                }
            }

            base.Update(gameTime);
        }
        /// <summary>
        /// Draws the active enemies on the screen each frame.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values, used for frame-based animation.</param>
        public override void Draw(GameTime gameTime)
        {
            foreach (var enemy in activeEnemies)
            {
                enemy.Draw(gameTime);
            }

            base.Draw(gameTime);
        }
    }


}

