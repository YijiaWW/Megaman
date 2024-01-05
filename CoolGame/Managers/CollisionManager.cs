using CoolGame.Model;
using CoolGame.Sprites;
using CoolGame.Type;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolGame.Managers
{
    /// <summary>
    /// Manages collision detection and response in the game, including interactions between the player, enemies, bullets, and game props.
    /// </summary>
    public class CollisionManager : DrawableGameComponent
    {
        private Megaman megaman;
        private List<Enemy> enemies;
        private List<GameProp> gameProp;
        private SoundEffect bulletCollsion;
        private SoundEffect getHitEnemy;
        private SoundEffect getHitMega;
        private SoundEffect deathMega;
        private Game1 g;
        public bool isLevel2;
        /// <summary>
        /// Initializes a new instance of the CollisionManager class.
        /// </summary>
        /// <param name="game">The game object this component is associated with.</param>
        /// <param name="megaman">The player character Megaman.</param>
        /// <param name="enemies">A list of enemies in the game.</param>
        /// <param name="bulletCollsion">Sound effect for bullet collision.</param>
        /// <param name="getHitEnemy">Sound effect when an enemy is hit.</param>
        /// <param name="getHitMega">Sound effect when Megaman is hit.</param>
        /// <param name="deathMega">Sound effect for Megaman's death.</param>
        public CollisionManager(Game game, Megaman megaman, List<Enemy> enemies, SoundEffect bulletCollsion, SoundEffect getHitEnemy, SoundEffect getHitMega, SoundEffect deathMega) : base(game)
        {   
            g=(Game1)game;
            this.bulletCollsion = bulletCollsion;
            this.megaman = megaman;
            this.enemies = enemies;
            this.getHitEnemy = getHitEnemy;
            this.getHitMega = getHitMega;
            this.deathMega = deathMega;
            gameProp = new List<GameProp>();

        }
        /// <summary>
        /// Adds an enemy to the collision manager for collision detection.
        /// </summary>
        /// <param name="enemy">The enemy to be added.</param>
        public void AddEnemy(Enemy enemy)
        {
            enemies.Add(enemy);
        }
        /// <summary>
        /// Adds a game prop to the collision manager for collision detection.
        /// </summary>
        /// <param name="Prop">The game prop to be added.</param>
        public void AddGameProp(GameProp Prop)
        {
            gameProp.Add(Prop);
        }

        /// <summary>
        /// Updates the collision manager, checking and handling collisions.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // Retrieve active bullets shot by Megaman
            var megamanBullets = megaman.GetActiveBullets().ToList();

            // Retrieve active bullets shot by all enemies
            var enemyBullets = enemies.SelectMany(enemy => enemy.GetActiveBullets()).ToList();

            // Check for collision between Megaman and enemy bullets
            foreach (var bullet in enemyBullets.ToList())
            {
                if (!Game.Components.Contains(bullet) || bullet.hasHit) continue;  // Skip bullets that are no longer active or have already hit something

                if (bullet.bulletType == BulletType.EnemyBullet && megaman.Bounds.Intersects(bullet.Bounds))
                {
                    bullet.hasHit = true;

                    megaman.megamanCollisionCount++;
                    if (megaman.megamanCollisionCount >= megaman.MegamanMaxCollisionCount)
                    {
                        deathMega.Play();
                        megaman.TriggerDeath();
                        SaveScoreWhenDead();
                    }
                    else
                    {
                        getHitMega.Play();
                        megaman.TriggerHitFlicker();
                    }

                    // Remove bullet from game components and active bullets list
                    Game.Components.Remove(bullet);
                    enemyBullets.Remove(bullet);
                }
            }

            // Check for collision between enemies and Megaman's bullets
            foreach (var enemy in enemies)
            {
                foreach (var bullet in megamanBullets.ToList())
                {
                    if (!bullet.hasHit && bullet.bulletType == BulletType.MageBullet && enemy.Bounds.Intersects(bullet.Bounds))
                    {
                        bullet.hasHit = true;

                        if (enemy.HitCount < 3)
                        {
                            megaman.Score++;
                            enemy.TriggerHitFlicker();
                        }
                        enemy.HitCount++;
                        if (enemy.HitCount >= 3)
                        {
                            enemy.TriggerHitFlicker();  
                        }
                        getHitEnemy.Play();
                        Game.Components.Remove(bullet);
                        megamanBullets.Remove(bullet);
                    }
                }
            }

            // Check for collision between Megaman's bullets and enemy bullets
            foreach (var bullet1 in megamanBullets.ToList())
            {
                if (!Game.Components.Contains(bullet1)) continue;  // Skip bullets not in the game components

                foreach (var bullet2 in enemyBullets.ToList())
                {
                    if (!Game.Components.Contains(bullet2)) continue;  // Skip bullets not in the game components

                    if (bullet1.Bounds.Intersects(bullet2.Bounds))
                    {
                        // Logic when bullets collide
                        bulletCollsion.Play();
                        bullet1.hasHit = true;
                        bullet2.hasHit = true;

                        // Remove both bullets from game components and active bullets list
                        Game.Components.Remove(bullet1);
                        Game.Components.Remove(bullet2);
                        megamanBullets.Remove(bullet1);
                        enemyBullets.Remove(bullet2);
                    }
                }
            }
            if (isLevel2&& gameProp !=null)
            {
                foreach (var prop in gameProp.ToList())
                {
                    if (megaman.Bounds.Intersects(prop.Bounds) && prop.gamePropType == GamePropType.Life && !prop.hasUsed)
                    {
                        megaman.MegamanMaxCollisionCount++;
                        prop.hasUsed = true;
                        Game.Components.Remove(prop);
                        gameProp.Remove(prop);
                    }
                    else if(megaman.Bounds.Intersects(prop.Bounds) && prop.gamePropType == GamePropType.BulletShotgun && !prop.hasUsed) 
                    {
                        megaman.isShotGun = true;
                        megaman.shotGunTimer = 0;
                        prop.hasUsed = true;
                        Game.Components.Remove(prop);
                        gameProp.Remove(prop);
                    }
                }
            }
           
            base.Update(gameTime);
        }

        /// <summary>
        /// Saves the score when Megaman dies.
        /// </summary>
        public void SaveScoreWhenDead()
        {
            if (megaman.isDead && !megaman.scoreSaved)
            {
                Score score = new Score
                {
                    PlayerName = g.actionScene.playerName,
                    Value = megaman.Score,
                    NumberDestroyedEnemy = megaman.NumberDestroyedEnemy,
                    survivalTime = megaman.survivalTime,
                };

                ScoreManager scoreManager = ScoreManager.Load();
                scoreManager.Add(score);
                ScoreManager.Save(scoreManager);

                megaman.scoreSaved = true; 
            }
        }
        /// <summary>
        /// Draws the collision-related visuals, if any.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values, used for frame-based animation.</param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
