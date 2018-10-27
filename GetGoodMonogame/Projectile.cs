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
    class Projectile
    {
        public float speed;
        public Texture2D projectileSprite;
        public Vector2 projectilePosition;

        public Projectile(float speed, Texture2D t2D, Vector2 pos)
        {
            this.speed = speed;
            projectileSprite = t2D;
            projectilePosition = pos;
        }

        public void projectileTranslation()
        {
            //On Y Axis  (-= because it goes from the bottom to the top)
            this.projectilePosition.Y -= this.projectileSprite.Height * this.speed;
        }
    }
}
