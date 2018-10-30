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
    public class Star
    {
        public Texture2D _texture;
        public Vector2 _position;
        public double _randomSpeedY;


        //Also initializes background white stars
        public Star(Texture2D texture, int rdmPosX, int rdmPosY, double rdmYSpeed)
        {
            this._texture = texture;
            this._position.X = rdmPosX;
            this._position.Y = rdmPosY;
            this._randomSpeedY = rdmYSpeed;
        }

        public void Update(GameTime gameTime, double randomStarSpeedY)
        {
            this._position.Y += (float) randomStarSpeedY;

            if (this._position.Y > 630)
            {
                this._position.Y = (float)_randomSpeedY;
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this._texture, this._position, Color.White);
        }
    }
}
