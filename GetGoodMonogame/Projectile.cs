using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

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
        public Texture2D projectileSprite;
        public Texture2D _texture; //Sprite of the projectile
        public Vector2 _position; //Position of the projectile

        public Rectangle _collisionBox;

        public bool _isOnScreen = false;

        //POUR LE MOMENT, LES PROJECTILES NE SONT PAS "DETRUITS" LORSQU'ILS SORTENT DE L'ECRAN
        //CELA PEUT CREER DES PROBLEMES DE MEMOIRE A L'AVENIR. SI JAMAIS, IL FAUT SIMPLEMENT TROUVER UN MOYEN
        //DE SUPPRIMER DE LA LISTE DES PROJECTILES LE PROJECTILE COURANT SI JAMAIS SA POSITION.Y EST HORS DE L'ECRAN

        //METHODS
        public Projectile(Texture2D texture, Vector2 startPos)
        {
            this._texture = texture;
            this._position = startPos;
            this._collisionBox = new Rectangle((int)this._position.X, (int)this._position.Y, (int)this._texture.Width, (int)this._texture.Height);
        }

        //Method() sample to load a content from within the class
        public void LoadContent(ContentManager cm, string assetName)
        {
            projectileSprite = cm.Load<Texture2D>(assetName);
        }

        public void Update(GameTime gameTime)
        {
            this._position.Y -= 5;

            this._collisionBox = new Rectangle((int)this._position.X, (int)this._position.Y, (int)this._texture.Width, (int)this._texture.Height);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this._texture, this._position, Color.White);
        }

    }
}
