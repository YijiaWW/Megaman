using CoolGame.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolGame.Scene
{
    /// <summary>
    /// Represents the level 2 action scene of the game, extending the base action scene functionalities.
    /// </summary>
    public class Level2ActionScene : ActionScene
    {
        private const float EnemySpawnIntervalLevel2 = 8f;
        private const float TotalSpawnIntervalLevel2 = 12f;
        /// <summary>
        /// Initializes a new instance of the Level2ActionScene class.
        /// </summary>
        /// <param name="game">The main game object this scene is associated with.</param>
        /// <param name="playerName">The name of the player, used for display and scoring purposes.</param>
        public Level2ActionScene(Game game, string playerName)
            : base(game, playerName)
        {
            enemyManager.CreateNewGameProp();
            enemyManager.UpdateEnemyManagerParams(EnemySpawnIntervalLevel2, TotalSpawnIntervalLevel2);
            collisionManager.isLevel2 = true;

        }
        /// <summary>
        /// Updates the scene's state. It is called once per frame.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime); 

        }
        /// <summary>
        /// Draws the scene's content. It is called once per frame after Update.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values, used for frame-based animation.</param>
        public override void Draw(GameTime gameTime)
        {
            sb.Begin();
            sb.End();
            base.Draw(gameTime); 

        }

    }
}
