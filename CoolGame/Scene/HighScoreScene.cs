using CoolGame.Controls;
using CoolGame.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct2D1;
using System;
using System.Linq;

namespace CoolGame.Scene
{
    /// <summary>
    /// Represents the high score scene in the game, displaying the top scores across various categories.
    /// </summary>
    public class HighScoreScene: GameScene
	{
		private GraphicsDeviceManager graphics;
		private SpriteFont _font;
		private ScoreManager _scoreManager;
		private Microsoft.Xna.Framework.Graphics.SpriteBatch sb;
		private Game1 g;
        private const float StartPositionX = 80f;
        private const float StartPositionY = 100f;
        /// <summary>
        /// Initializes a new instance of the HighScoreScene class.
        /// </summary>
        /// <param name="game">The game object this scene is associated with.</param>
        public HighScoreScene(Game game) : base(game)
		{

			g = (Game1)game;
			this.sb = g._spriteBatch;
			this.graphics = g._graphics;
			_scoreManager = ScoreManager.Load();
			_font = g.Content.Load<SpriteFont>("Fonts/Font");
		}
        /// <summary>
        /// Updates the high score scene. It is called once per frame.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
		{   
			base.Update(gameTime);
            _scoreManager = ScoreManager.Load();
        }
        /// <summary>
        /// Draws the high score scene, including the top scores in various categories.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values, used for frame-based animation.</param>
        public override void Draw(GameTime gameTime)
		{
			sb.Begin();
            sb.Draw(g.startScene.StartMenutex, Vector2.Zero, Color.White);
            var topScoresByValue = _scoreManager.Highscores;
            var topScoresByEnemy = _scoreManager.GetTopFiveByNumberDestroyedEnemy();
            var topScoresByTime = _scoreManager.GetTopFiveBySurvivalTime();

            var sizeTopScores = _font.MeasureString("Top Scores:\n" + string.Join("\n", topScoresByValue.Select(c => c.PlayerName + " : " + c.Value)));
            var sizeTopEnemies = _font.MeasureString("Top Enemies Destroyed:\n" + string.Join("\n", topScoresByEnemy.Select(c => c.PlayerName + " : " + c.NumberDestroyedEnemy)));
            var sizeTopTimes = _font.MeasureString("Top Survival Times:\n" + string.Join("\n", topScoresByTime.Select(c => c.PlayerName + " : " + c.survivalTime.ToString("0.00") + " sec")));

            float verticalSpacing = 80;
            Vector2 startPosition = new Vector2(StartPositionX, StartPositionY);

            Vector2 positionTopScores = startPosition;
            Vector2 positionTopEnemies = new Vector2(startPosition.X + sizeTopScores.X + verticalSpacing, startPosition.Y );
            Vector2 positionTopTimes = new Vector2(startPosition.X + sizeTopScores.X + sizeTopEnemies.X + verticalSpacing, positionTopEnemies.Y );

            sb.DrawString(_font, "Top Scores:\n" + string.Join("\n", topScoresByValue.Select(c => c.PlayerName + " : " + c.Value)), positionTopScores, Microsoft.Xna.Framework.Color.Red);
            sb.DrawString(_font, "Top Enemies Destroyed:\n" + string.Join("\n", topScoresByEnemy.Select(c => c.PlayerName + " : " + c.NumberDestroyedEnemy)), positionTopEnemies, Microsoft.Xna.Framework.Color.Red);
            sb.DrawString(_font, "Top Survival Times:\n" + string.Join("\n", topScoresByTime.Select(c => c.PlayerName + " : " + c.survivalTime.ToString("0.00") + " sec")), positionTopTimes, Microsoft.Xna.Framework.Color.Red);
            sb.End();
			base.Draw(gameTime);
		}
	}
}
