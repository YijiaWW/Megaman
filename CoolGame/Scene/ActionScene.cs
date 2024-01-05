using CoolGame.Controls;
using CoolGame.Managers;
using CoolGame.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SharpDX.Direct2D1;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace CoolGame.Scene
{
    /// <summary>
    /// Represents the main action scene of the game where gameplay occurs.
    /// </summary>
    public class ActionScene : GameScene
	{
		public Megaman Megaman;
        protected SpriteFont _font;
        protected SpriteFont _smallFont;
        protected Microsoft.Xna.Framework.Graphics.SpriteBatch sb;
        private List<Enemy> enemies;
        protected CollisionManager collisionManager;
        protected EnemyAndPropManager enemyManager;
        private Texture2D megaManLife;
        private Game1 g;
        public string playerName = "";
        private bool showDeathMessage;
        private string deathMessage = "You're dead. Press enter to return to the game over screen.";
        private Vector2 backgroundPosition1, backgroundPosition2;
        private Texture2D ScrollingBackgroundTexture;
        private Vector2 speed = new Vector2(3, 0);
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionScene"/> class.
        /// </summary>
        /// <param name="game">The main game object this scene is associated with.</param>
        /// <param name="playerName">The name of the player.</param>
        public ActionScene(Game game,string playerName) : base(game)
		{
			g = (Game1)game;
            this.playerName=playerName;
            this.sb = g._spriteBatch;
            _font = g.Content.Load<SpriteFont>("Fonts/Font");
            _smallFont = g.Content.Load<SpriteFont>("Fonts/SmallFont");

            Texture2D runTex = game.Content.Load<Texture2D>("Sprites/RunningMageMan");
			Texture2D standTex = game.Content.Load<Texture2D>("Sprites/StandMageMan");
			Texture2D jumpTex = game.Content.Load<Texture2D>("Sprites/JumpMage");
			Texture2D shotTex = game.Content.Load<Texture2D>("Sprites/ShottingMage");
			Texture2D runAndShotTex = game.Content.Load<Texture2D>("Sprites/RunAndShotMage");
			Texture2D bulletTex = game.Content.Load<Texture2D>("Sprites/bullet");
			Texture2D jumpShotTex = game.Content.Load<Texture2D>("Sprites/JumpShotMage");
            Texture2D tankEnemyTexture = game.Content.Load<Texture2D>("Sprites/TankShot");
            Texture2D tankExpolisionTexture = game.Content.Load<Texture2D>("Sprites/TankExplosion");
            Texture2D bornMageMan = game.Content.Load<Texture2D>("Sprites/BornMageMan");
            Texture2D deathTexture = game.Content.Load<Texture2D>("Sprites/DeathMageMan");
            Texture2D planeTexture = game.Content.Load<Texture2D>("Sprites/PlaneEnemy");

            megaManLife = game.Content.Load<Texture2D>("Sprites/MegaManLife");
            SoundEffect shotSound = game.Content.Load<SoundEffect>("Sounds/GunShot");
            SoundEffect jumpSound = game.Content.Load<SoundEffect>("Sounds/Jump");
            SoundEffect tankExplosion = game.Content.Load<SoundEffect>("Sounds/TankExplosion");
            SoundEffect bulletCollsion = game.Content.Load<SoundEffect>("Sounds/BulletCollsion");
            SoundEffect getHitEnemy = game.Content.Load<SoundEffect>("Sounds/GetHitEnemy");
            SoundEffect getHitMega = game.Content.Load<SoundEffect>("Sounds/GetHitMega");
            SoundEffect enemyShot = game.Content.Load<SoundEffect>("Sounds/EnemyShot");
            SoundEffect deathMega = game.Content.Load<SoundEffect>("Sounds/DeathMega");
            enemies = new List<Enemy>();  
            Megaman = new Megaman(g, sb,new Vector2(0,Game1.ScreenHeight- standTex.Height-50), 4, runTex,standTex, jumpTex, shotTex,
				runAndShotTex, bulletTex, jumpShotTex, shotSound, jumpSound, bornMageMan,deathTexture);
            collisionManager = new CollisionManager(game, Megaman, enemies, bulletCollsion, getHitEnemy, getHitMega, deathMega);
            enemyManager = new EnemyAndPropManager(g, tankEnemyTexture, tankExpolisionTexture, bulletTex,
                                    tankExplosion, enemyShot, collisionManager, planeTexture,10f,15);
            this.Components.Add(enemyManager);
            this.Components.Add(collisionManager);
            this.Components.Add(Megaman);
            enemyManager.IsActive = true;

            ScrollingBackgroundTexture = Game.Content.Load<Texture2D>("Images/Level1Background");
            backgroundPosition1 = Vector2.Zero;
            backgroundPosition2 = new Vector2(ScrollingBackgroundTexture.Width, 0);

        }
        /// <summary>
        /// Draws the action scene content, including characters, background, and UI elements.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values, used for frame-based animation.</param>
        public override void Draw(GameTime gameTime)
        {
            sb.Begin();

            sb.Draw(ScrollingBackgroundTexture, backgroundPosition1, Color.White);
            sb.Draw(ScrollingBackgroundTexture, backgroundPosition2, Color.White);
            if (Megaman.isShotGun)
            {
                float shotGunTime = Megaman.shotGunInterval - Megaman.shotGunTimer;
                string shotGunTimeText = "Shot Gun Time: " + shotGunTime.ToString("0.00") + " seconds";
                sb.DrawString(_smallFont, shotGunTimeText, new Vector2(10, 110), Color.White);
            }
            if (collisionManager.isLevel2 == true) {
                string levelText = "Level 2";
                Vector2 levelTextSize = _font.MeasureString(levelText);
                Vector2 levelTextPosition = new Vector2(
                    (GraphicsDevice.Viewport.Width - levelTextSize.X) / 2,
                    10
                );
                sb.DrawString(_font, levelText, levelTextPosition, Color.Black);
            }
            Vector2 topRightCorner = new Vector2(GraphicsDevice.Viewport.Width - 70, 25);

            int health = Math.Max(Megaman.MegamanMaxCollisionCount - Megaman.megamanCollisionCount, 0);
            string healthText = "  X  " + health.ToString();
            Vector2 healthTextSize = _font.MeasureString(healthText);
            sb.DrawString(_font, healthText, topRightCorner, Color.Black);
            sb.Draw(megaManLife, new Vector2(GraphicsDevice.Viewport.Width - 70- healthTextSize.X, 10), Color.White);

            Vector2 topLeftCorner = new Vector2(10, 10);
            float verticalSpacing = 25;  

            string playerNameText = "Player Name: " + playerName;
            sb.DrawString(_smallFont, playerNameText, topLeftCorner, Color.Black);

            string scoreText = "Player Score: " + Megaman.Score.ToString();
            Vector2 scorePosition = new Vector2(topLeftCorner.X, topLeftCorner.Y + verticalSpacing);
            sb.DrawString(_smallFont, scoreText, scorePosition, Color.Black);

            string destroyedNumText = "Number of enemies destroyed: " + Megaman.NumberDestroyedEnemy.ToString();
            Vector2 destroyedNumPosition = new Vector2(topLeftCorner.X, scorePosition.Y + verticalSpacing);
            sb.DrawString(_smallFont, destroyedNumText, destroyedNumPosition, Color.Black);

            string survivalTimeText = "Survival Time: " + Megaman.survivalTime.ToString("0.00") + " seconds";
            Vector2 survivalTimePosition = new Vector2(topLeftCorner.X, destroyedNumPosition.Y + verticalSpacing);
            sb.DrawString(_smallFont, survivalTimeText, survivalTimePosition, Color.Black);

            if (showDeathMessage)
            {
                Vector2 messageSize = _font.MeasureString(deathMessage);
                Vector2 messagePosition = new Vector2(
                    (GraphicsDevice.Viewport.Width - messageSize.X) / 2,
                    (GraphicsDevice.Viewport.Height - messageSize.Y) / 2
                );
                sb.DrawString(_font, deathMessage, messagePosition, Color.Red);
            }
            sb.End();
            enemyManager.Draw(gameTime);
            base.Draw(gameTime);
        }

        /// <summary>
        /// Updates the action scene, handling character movement, enemy interactions, and scene transitions.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (Megaman.isDead && !showDeathMessage)
            {
                showDeathMessage = true;
                enemyManager.IsActive = false;
                enemyManager.ClearEnemies();
            }
            if (showDeathMessage)
            {
                KeyboardState state = Keyboard.GetState();
                if (state.IsKeyDown(Keys.Enter))
                {
                    g.hideAllScenes();
                    g.overScene = new GameOverScene(g, playerName,Megaman.Score,Megaman.NumberDestroyedEnemy,Megaman.survivalTime);
                    g.Components.Add(g.overScene);
                    g.overScene.show();
                }
            }

            if (!showDeathMessage)
            {
                enemyManager.Update(gameTime);
            }
            backgroundPosition1 -= speed;
            backgroundPosition2 -= speed;

            if (backgroundPosition1.X < -ScrollingBackgroundTexture.Width)
                backgroundPosition1.X = backgroundPosition2.X + ScrollingBackgroundTexture.Width;
            if (backgroundPosition2.X < -ScrollingBackgroundTexture.Width)
                backgroundPosition2.X = backgroundPosition1.X + ScrollingBackgroundTexture.Width;
            base.Update(gameTime);
          
        }

        

    }
}
