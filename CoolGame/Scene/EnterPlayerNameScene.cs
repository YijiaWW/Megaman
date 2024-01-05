using CoolGame.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace CoolGame.Scene
{
    /// <summary>
    /// Represents a scene for entering the player's name.
    /// </summary>
    public class EnterPlayerNameScene : GameScene
    {
        private Microsoft.Xna.Framework.Graphics.SpriteBatch sb;
        private SpriteFont font;
        private Button _Enterbutton;
        private string playerName = "";
        private string warningMessage = "";
        private KeyboardState oldState;
        private Game1 g;
        /// <summary>
        /// Initializes a new instance of the <see cref="EnterPlayerNameScene"/> class.
        /// </summary>
        /// <param name="game">The game this scene is associated with.</param>
        public EnterPlayerNameScene(Game game) : base(game)
        {
            g = (Game1)game;
            this.sb = g._spriteBatch;
            font = g.Content.Load<SpriteFont>("Fonts/Font");
            _Enterbutton = new Button(g.Content.Load<Texture2D>(("Controls/Button")), g.Content.Load<SpriteFont>("Fonts/Font"),
                g.Content.Load<SoundEffect>("Sounds/Clicksound"), g.Content.Load<SoundEffect>("Sounds/SelectSound"))
            {
                Position = new Vector2(100,150),
                Text = "Enter"
            };
            _Enterbutton.Click += _Enterbutton_Click;
         
        }
        /// <summary>
        /// Handles the Click event of the Enter button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An EventArgs that contains the event data.</param>
        private void _Enterbutton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(playerName))
            {
                warningMessage = "Please enter a valid name!";
            }
            else
            {
                g.actionScene = new ActionScene(g, playerName);
                g.Components.Add(g.actionScene);

                g.hideAllScenes();
                g.actionScene.show();
                playerName = "";
                warningMessage = "";  
            }
        }
        /// <summary>
        /// Updates the player name entry scene.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            KeyboardState newState = Keyboard.GetState();

            if (newState.GetPressedKeys().Length > 0)
            {
                foreach (var key in newState.GetPressedKeys())
                {
                    if (oldState.IsKeyUp(key))
                    {
                        if (key == Keys.Back && playerName.Length > 0)
                        {
                            playerName = playerName.Substring(0, playerName.Length - 1);
                            warningMessage = ""; 
                        }
                        else if (key == Keys.Space)
                        {
                            if (string.IsNullOrWhiteSpace(playerName))
                            {
                                warningMessage = "Name cannot be just a space!";
                            }
                            else
                            {
                                playerName += " ";
                                warningMessage = "";   
                            }
                        }
                        else
                        {
                            string newChar = key.ToString();
                            if (newChar.Length == 1)
                            {
                                playerName += newChar;
                                warningMessage = "";   
                            }
                        }
                    }
                }
            }

            oldState = newState;
            _Enterbutton.Update(gameTime);
            base.Update(gameTime);
        }
        /// <summary>
        /// Draws the player name entry scene.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values, used for frame-based animation.</param>
        public override void Draw(GameTime gameTime)
        {
            sb.Begin();
            sb.Draw(g.startScene.StartMenutex, Vector2.Zero, Color.White);
            _Enterbutton.Draw(gameTime, sb);
            sb.DrawString(font, "Enter Name: " + playerName, new Vector2(100, 100), Color.White);
            if (!string.IsNullOrEmpty(warningMessage))
            {
                sb.DrawString(font, warningMessage, new Vector2(100, 130), Color.Red);
            }

            sb.End();
            base.Draw(gameTime);
        }
    }
}
