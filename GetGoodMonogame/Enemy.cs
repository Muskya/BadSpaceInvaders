using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace GetGoodMonogame
{
    class Enemy
    {
        public Texture2D _texture;
        public Vector2 _position;
        public Vector2 _startPos; //Checker for default starting direction (right or left)
        public bool _isDead = false;

        public Rectangle _collisionBox;

        public Rectangle _debugRectangle;
        public Texture2D _debugTexture;

        public static List<Enemy> enemiesOnScreen = new List<Enemy>();

        public Enemy(Texture2D texture, Vector2 pos, Rectangle colliBox)
        {
            this._texture = texture;
            this._position = pos;
            this._startPos = pos;

            //this._collisionBox = new Rectangle((int)this._position.X, (int)this._position.Y, (int)this._texture.Width, (int)this._texture.Height);
            this._collisionBox = colliBox;
            this._collisionBox.X = (int)this._position.X;
            this._collisionBox.Y = (int)this._position.Y;
        }

        public void Update(GameTime gameTime)
        {
            //Mouvement vertical vers le bas des ennemis.
            this._position.Y += 0.5f;

            //La collisionBox suit le sprite de l'ennemi. Placé à la fin après quelconque modification de position
            this._collisionBox = new Rectangle((int)this._position.X, (int)this._position.Y, (int)this._texture.Width, (int)this._texture.Height);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this._texture, this._position, Color.White);
        }
    }
}
