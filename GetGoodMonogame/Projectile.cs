using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetGoodMonogame
{
    public class Projectile
    {
        
        //FIELDS
        public Texture2D _texture; //Sprite of the projectile
        public Vector2 _position; //Position of the projectile

        //private Vector2 _velocity; //Velocity of the projectile
        //private float _speed; //Speed of the projectile

        public bool _isShot = false;
        public bool _isOnScreen = false;

        public Color _color;

        List<Projectile> projectilesOnScreen;

        //METHODS
        public Projectile(Texture2D texture, Vector2 startPos)
        {
           _color = Color.White;
            this._texture = texture;
            this._position = startPos;
           projectilesOnScreen = new List<Projectile>();

        }

        public void Update(GameTime gameTime)
        {
            this._position.Y -= 5;

            if (this._position.Y < -30)
            {
                projectilesOnScreen.Remove(this);
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this._texture, this._position, _color);
        }

    }
}
