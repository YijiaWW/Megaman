using CoolGame.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

namespace CoolGame.Scene
{
    /// <summary>
    /// Represents the game over scene, displayed when the player has finished a level or the game.
    /// </summary>
    public class GameOverScene : GameScene
    {
        private Game1 Game;
        private SpriteBatch sb;
        private string playerName;
        private int Score;
        private int NumberDestroyedEnemy;
        private float SurvivalTime;
        private SpriteFont font;
        private Song GameOverBackgroundMusic;
        private Song BackgroundMusic;
        private Texture2D ScrollingBackgroundTexture;
        private Vector2 backgroundPosition1, backgroundPosition2;
        private Vector2 speed;
        public List<Button> buttons;
        private const float BackgroundScrollSpeedX = 3.0f;
        private const float BackgroundScrollSpeedY = 0.0f;
        private const int ButtonVerticalSpacing = 30;
        private const int ScorePositionOffset = 20;
        private const int SurvivalTimePositionOffset = 40;
        /// <summary>
        /// Initializes a new instance of the GameOverScene class.
        /// </summary>
        /// <param name="game">The main game object this scene is associated with.</param>
        /// <param name="playerName">The name of the player.</param>
        /// <param name="score">The player's score.</param>
        /// <param name="numberDestroyedEnemy">The number of enemies destroyed by the player.</param>
        /// <param name="survivalTime">The time the player survived in the game.</param>
        public GameOverScene(Game game, string playerName, int score, int numberDestroyedEnemy, float survivalTime) : base(game)
        {
            Game = (Game1)game;
            this.sb = Game._spriteBatch;
            this.playerName = playerName;
            NumberDestroyedEnemy = numberDestroyedEnemy;
            SurvivalTime = survivalTime;
            speed = new Vector2(BackgroundScrollSpeedX, BackgroundScrollSpeedY);
            Score = score;
            font = Game.Content.Load<SpriteFont>("Fonts/Font");
            GameOverBackgroundMusic = Game.Content.Load<Song>("Sounds/GameOverBackgroundMusic");
            BackgroundMusic = Game.Content.Load<Song>("Sounds/BackgroundMusic");

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(GameOverBackgroundMusic);

            ScrollingBackgroundTexture = Game.Content.Load<Texture2D>("Images/ScrollingBackground");
            backgroundPosition1 = Vector2.Zero;
            backgroundPosition2 = new Vector2(ScrollingBackgroundTexture.Width, 0);


            buttons = new List<Button>
    {
        new Button(Game.Content.Load<Texture2D>("Controls/Button"), font,
            Game.Content.Load<SoundEffect>("Sounds/Clicksound"), Game.Content.Load<SoundEffect>("Sounds/SelectSound"))
        {
            Text = "Main Menu"
        },new Button(Game.Content.Load<Texture2D>("Controls/Button"), font,
            Game.Content.Load<SoundEffect>("Sounds/Clicksound"), Game.Content.Load<SoundEffect>("Sounds/SelectSound"))
        {
            Text = "Next Level"
        },
        new Button(Game.Content.Load<Texture2D>("Controls/Button"), font,
            Game.Content.Load<SoundEffect>("Sounds/Clicksound"), Game.Content.Load<SoundEffect>("Sounds/SelectSound"))
        {
            Text = "Exit Game"
        }

    };

            const int verticalSpacing = 30;
            Vector2 playerScoreTextSize = font.MeasureString("Player: " + playerName + "\nScore: " + Score.ToString());
            Vector2 scorePosition = new Vector2(
                (Game.GraphicsDevice.Viewport.Width - playerScoreTextSize.X) / 2,
                Game.GraphicsDevice.Viewport.Height / 3);

            float nextButtonY = scorePosition.Y + playerScoreTextSize.Y + ScorePositionOffset;

            foreach (var button in buttons)
            {
                button.Position = new Vector2(
                    (Game.GraphicsDevice.Viewport.Width - button.Rectangle.Width) / 2,
                    nextButtonY);
                button.Click += EventHandlerButton_Click;
                nextButtonY += button.Rectangle.Height + verticalSpacing;
            }
            
        }
        /// <summary>
        /// Handles button click events within the game over scene.
        /// </summary>
        /// <param name="sender">The source of the event, typically a button.</param>
        /// <param name="e">Event arguments (not used).</param>
        private void EventHandlerButton_Click(object sender, System.EventArgs e)
        {
            Button clickedButton = sender as Button;

            // Check if the cast was successful
            if (clickedButton != null)
            {
                // Access the Text property of the clicked button
                string buttonText = clickedButton.Text;

                // Implement logic to change the scene based on the button click
                switch (buttonText)
                {
                    case "Main Menu":
                        Game.hideAllScenes();
                        Game.startScene.show();
                        MediaPlayer.Play(BackgroundMusic);
                        break;
                    case "Exit Game":
                        Game.Exit();
                        break;
                    case "Next Level":
                        Game.hideAllScenes();
                        Game.level2ActionScene = new Level2ActionScene(Game,playerName);
                        Game.Components.Add(Game.level2ActionScene);
                        Game.level2ActionScene.show();
                        MediaPlayer.Play(BackgroundMusic);
                        break;
                }
            }
        }
        /// <summary>
        /// Updates the game over scene state. It is called once per frame.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            backgroundPosition1 -= speed;
            backgroundPosition2 -= speed;

            if (backgroundPosition1.X < -ScrollingBackgroundTexture.Width)
                backgroundPosition1.X = backgroundPosition2.X + ScrollingBackgroundTexture.Width;
            if (backgroundPosition2.X < -ScrollingBackgroundTexture.Width)
                backgroundPosition2.X = backgroundPosition1.X + ScrollingBackgroundTexture.Width;

            foreach (var button in buttons)
            {
                button.Update(gameTime);
            }

            base.Update(gameTime);
        }
        /// <summary>
        /// Draws the game over scene content. It is called once per frame after Update.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values, used for frame-based animation.</param>
        public override void Draw(GameTime gameTime)
        {
            Game.GraphicsDevice.Clear(Color.Black);
            Game._spriteBatch.Begin();

            Game._spriteBatch.Draw(ScrollingBackgroundTexture, backgroundPosition1, Color.White);
            Game._spriteBatch.Draw(ScrollingBackgroundTexture, backgroundPosition2, Color.White);

            string gameOverText = "Game Over\n\n";
            string playerScoreText = "Player: " + playerName + "\nScore: " + Score.ToString();
            string SurvivalTimeAndNumDestroy = $"Number of enemies destroyed: {NumberDestroyedEnemy}\nSurvival time: {SurvivalTime.ToString("0.00")} seconds";

            Vector2 gameOverTextSize = font.MeasureString(gameOverText);
            Vector2 playerScoreTextSize = font.MeasureString(playerScoreText);
            Vector2 SurvivalTimeAndNumDestroySize = font.MeasureString(SurvivalTimeAndNumDestroy);

            Vector2 gameOverPosition = new Vector2(
               (Game.GraphicsDevice.Viewport.Width - gameOverTextSize.X) / 2,
               Game.GraphicsDevice.Viewport.Height / 6 - gameOverTextSize.Y / 2);

            Vector2 scorePosition = new Vector2(
                (Game.GraphicsDevice.Viewport.Width - playerScoreTextSize.X) / 2,
                Game.GraphicsDevice.Viewport.Height / 4 - playerScoreTextSize.Y / 2);

            Vector2 SurvivalTimeAndNumDestroySizePosition = new Vector2(
                (Game.GraphicsDevice.Viewport.Width - SurvivalTimeAndNumDestroySize.X) / 2,
                Game.GraphicsDevice.Viewport.Height / 3 - SurvivalTimeAndNumDestroySize.Y / 2+40);


            Game._spriteBatch.DrawString(font, gameOverText, gameOverPosition, Color.Black);
            Game._spriteBatch.DrawString(font, playerScoreText, scorePosition, Color.Black);
            Game._spriteBatch.DrawString(font, SurvivalTimeAndNumDestroy, SurvivalTimeAndNumDestroySizePosition, Color.Black);

            foreach (var button in buttons)
            {
                button.Draw(gameTime, sb);
            }

            Game._spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
