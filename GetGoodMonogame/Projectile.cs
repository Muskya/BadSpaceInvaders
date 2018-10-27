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
        //projectile
        public Texture2D projectileSprite; //Sprite of the projectile
        private Vector2 projectileTarget; //Position of the target
        public Vector2 projectilePosition; //Position of the projectile
        private Vector2 projectileVelocity; //Velocity of the projectile
        public bool projectileIsActive; //Is the projectile Active ?
        public bool projectileIsShot = false;
        private float projectileSpeed; //Speed of the projectile
        public Rectangle projectileRectangle; //Rectangle of the projectile

        public Projectile()
        {
            projectileIsActive = false;
        }

        public void ActivateProjectile(Texture2D texture)
        {
            projectileTarget = new Vector2(projectilePosition.X, projectilePosition.Y - 100);
            //projectilePosition = pos;
            projectileSprite = texture;

            SetVelocity();

            projectileSpeed = 200;
            projectileIsActive = true;
        }

        public void SetPosition(Vector2 pos)
        {
            this.projectilePosition = pos;
        }

        private void SetVelocity()
        {
            projectileVelocity = -(projectilePosition - projectileTarget);
            projectileVelocity.Normalize();
        }


        public void Update(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (projectilePosition.Y < -30)
                Kill();
            if (projectilePosition.X < -30 || projectilePosition.X > 530)
                Kill();

            projectilePosition += (projectileVelocity * projectileSpeed * elapsedTime);
            projectileRectangle = new Rectangle((int)projectilePosition.X, (int)projectilePosition.Y, projectileSprite.Width, projectileSprite.Height);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Texture2D texture)
        {
            spriteBatch.Draw(texture, projectilePosition, Color.White);
        }
    
        public void Kill()
        {
            projectileIsActive = false;
            projectileIsShot = false;
        }

    }
}
