using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace GetGoodMonogame
{
    public class Button
    {
        public enum State
        {
           None,
           Hover,
        }

        public Rectangle _rectangle;
        public State _state { get; set; }
        public Vector2 _position { get; set; }

        public Dictionary<State, Texture2D> _textures;

        public Button(Texture2D noneTexture, Texture2D hoverTexture, Vector2 position)
        {
            _textures = new Dictionary<State, Texture2D> {
                { State.None, noneTexture },
                { State.Hover, hoverTexture }
            };

            this._position = position;
        }

        public void SetRectangle(Rectangle rect)
        {
            _rectangle = rect;
        }

        public void Update(GameTime gameTime, string button)
        {
            //Si le bouton contient la souris
            if (_rectangle.Contains(Mouse.GetState().X, Mouse.GetState().Y))
            {
                _state = State.Hover;

                if (button == "play" && Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    Game1._gameState = Game1.GameState.Playing;
                    Game1.NewGame();
                }
                    
                else if (button == "quit" && Mouse.GetState().LeftButton == ButtonState.Pressed)
                    Game1._gameState = Game1.GameState.Quit;
                    
            } else {
                _state = State.None;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_textures[_state], this._position, Color.White);
        }
    }
}
