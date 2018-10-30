using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

//Commentaire extra - GitHub Desktop utilisé pour ce repository

namespace GetGoodMonogame
{
    public class Game1 : Game
    {
        #region SCREEN
        // SCREEN PROPERTIES
        GraphicsDeviceManager graphics; // L'écran en gros 
        SpriteBatch spriteBatch; // Layer d'affichage 
        
        //window's dimensions:
        private int screen_width = 500; 
        private int screen_height = 600;
        #endregion

        #region GAME
        // GAME PROPERTIES
        private bool bgmPlayable = false;
        private SpriteFont gameFont;
        #endregion

        #region CLASSES
        // CLASSES INSTANCIATIONS
        List<Projectile> projectilesOnScreen;
        List<Star> starsOnScreen;
        Random randomStarPos;

        #endregion

        #region TEXTURE
        // TEXTURES
        private Texture2D projectileSprite;
        private Texture2D playerSprite;

        private Texture2D star1;
        public int _randomPosX;
        public int _randomPosY;
        public double _randomStarSpeedY;

        private Texture2D rocketIcon;
        private Vector2 rocketIconPos = new Vector2(-5, 570);
        #endregion

        #region SOUND
        // SOUNDS EFFECTS
        private SoundEffect hitSound;
        private SoundEffect afterHitSound;
        private SoundEffect shootSound;
        private SoundEffect bgm;
        private SoundEffect shootInCD;
        #endregion

        #region PLAYER
        // PLAYER PROPERTIES
        private float playerSpeed = 150.0f;
        private Vector2 playerPosition;

        //shooting:
        private float shootCooldown;
        #endregion

        // Constructeur du jeu
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = screen_height; // 600px height
            graphics.PreferredBackBufferWidth = screen_width; // 500px width

            Content.RootDirectory = "Content";
            projectilesOnScreen = new List<Projectile>();
            starsOnScreen = new List<Star>();
        }

        // Initialize game logic
        protected override void Initialize()
        {
            #region PLAYER
            //PLAYER INITIALIZATION
            playerPosition = new Vector2(screen_width / 2, screen_height / 2);
            shootCooldown = 1.0f;
            #endregion

            #region ENVIRONMENT
            //ENVIRONMENT INITIALIZATION
            bgmPlayable = true;

            star1 = Content.Load<Texture2D>("star1");
            randomStarPos = new Random();
  
            for(int i = 0; i < 20; i++)
            {
                this._randomPosX = randomStarPos.Next(1, 500);
                this._randomPosY = randomStarPos.Next(1, 600);
                this._randomStarSpeedY = randomStarPos.NextDouble() * (1f - 0.15f) + 0.15f;
                RandomStars(star1, this._randomPosX, this._randomPosY, this._randomStarSpeedY);
            }
            #endregion

            #region GAMEPLAY
            //PROJECTILES INITIALIZATION
            projectileSprite = Content.Load<Texture2D>("rocket");
            #endregion

            base.Initialize();
        }

        // Load all the needed content
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //player content:
            playerSprite = Content.Load<Texture2D>("ship");
            rocketIcon = Content.Load<Texture2D>("rocket");

            //sound content:
            bgm = Content.Load<SoundEffect>("BGM");
            afterHitSound = Content.Load<SoundEffect>("hitSound");
            shootSound = Content.Load<SoundEffect>("shootSound");
            shootInCD = Content.Load<SoundEffect>("shootInCD");

            //environment content:
            gameFont = Content.Load<SpriteFont>("gameFont");
            
        }
        protected override void UnloadContent(){}

        // Game's main loop
        protected override void Update(GameTime gameTime)
        {
            #region BGM
            // Playing BGM
            if (bgmPlayable == true)
            {
                bgmPlayable = false; //PlaysOnlyOnce
                bgm.Play();
            }
            #endregion

            #region MAIN LOGIC
            // Exits the game when pressing Esc
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            { Exit(); }

            // Shooting + cooldown system
            shootCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            //blocks the timing at 0sec:
            if (shootCooldown <= 0)
            {
                shootCooldown = 0;
            }

            #endregion

            #region Déplacements
            if (Keyboard.GetState().IsKeyDown(Keys.Z))
            { playerPosition.Y -= playerSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds; }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            { playerPosition.X += playerSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds; }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            { playerPosition.Y += playerSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds; }
            if (Keyboard.GetState().IsKeyDown(Keys.Q))
            { playerPosition.X -= playerSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds; }
            #endregion

            if (Keyboard.GetState().IsKeyDown(Keys.Space) && shootCooldown <= 0)
            {
                ShootBullet(projectileSprite, new Vector2(playerPosition.X - 15.5f, 
                    playerPosition.Y - 20));

                foreach(Projectile p in projectilesOnScreen)
                {
                    //p.Update(gameTime);
                    p._isOnScreen = true;
                }

                shootSound.Play();
                shootCooldown = 1.0f;
            }

            foreach (Projectile p in projectilesOnScreen)
            {
                if (p._isOnScreen)
                {
                    p.Update(gameTime);
                }
            }

            foreach (Star s in starsOnScreen)
            {
                _randomStarSpeedY = randomStarPos.NextDouble() * (1f - 0.15f) + 0.15f;
                s.Update(gameTime, _randomStarSpeedY);
            }

            base.Update(gameTime);
        }

        // Draw to screen EACH frame (60 fps by default)
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            spriteBatch.DrawString(gameFont, "Cooldown: " + shootCooldown.ToString("F0") + " sec", new Vector2(rocketIconPos.X + 28, 578), Color.White);
            spriteBatch.Draw(rocketIcon, rocketIconPos, Color.White);
            spriteBatch.Draw(playerSprite, playerPosition, null, Color.White, 0f, new Vector2(playerSprite.Width / 2, playerSprite.Height / 2),
                Vector2.One, SpriteEffects.None, 0.1f);

            foreach(Projectile p in projectilesOnScreen)
            {
                if (p._isOnScreen)
                {
                    p.Draw(gameTime, spriteBatch);
                }
            }

            foreach(Star s in starsOnScreen)
            {
                s.Draw(gameTime, spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void ShootBullet(Texture2D texture, Vector2 startPos)
        {
            Projectile projectile = new Projectile(texture, startPos);
            projectilesOnScreen.Add(projectile);
        }

        public void RandomStars(Texture2D texture, int rdmPosX, int rdmPosY, double rdmYSpeed)
        {
            Star star = new Star(texture, rdmPosX, rdmPosY, rdmYSpeed);
            starsOnScreen.Add(star);
        }
    }
}
