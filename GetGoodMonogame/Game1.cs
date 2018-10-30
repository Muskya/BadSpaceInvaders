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

namespace GetGoodMonogame
{
    public class Game1 : Game
    {
        #region PROPERTIES
        //SCREEN PROPERTIES
        #region SCREEN
        GraphicsDeviceManager graphics; // L'écran en gros 
        SpriteBatch spriteBatch; // Layer d'affichage 
        
        //window's dimensions:
        private int screen_width = 500; 
        private int screen_height = 600;
        #endregion

        //GAME PROPERTIES
        #region GAME
        private bool bgmPlayable = false;
        private SpriteFont gameFont;
        #endregion

        //CLASSES INSTANCIATIONS AND PROPERTIES
        #region CLASSES
        List<Projectile> projectilesOnScreen; //Rockets shot on the screen
        List<Star> starsOnScreen; //Infinite-scrolling stars on the screen
        //List<Enemy> enemiesOnScreen; //Enemies on the screen

        //Random variable to set random X and Y star positions
        Random randomStarPos;
        #endregion

        //TEXTURES PROPERTIES
        #region TEXTURE
        private Texture2D projectileSprite;
        private Texture2D playerSprite;

        //Environment stars properties
        private Texture2D star1;
        public int _randomPosX;
        public int _randomPosY;
        public double _randomStarSpeedY;

        private Texture2D rocketIcon;
        //The rocket icon position at the bottom of the screen
        private Vector2 rocketIconPos = new Vector2(-5, 570);
        #endregion

        //SOUNDS PROPERTIES
        #region SOUND
        private SoundEffect hitSound; //When rocket hits an enemy
        private SoundEffect enemyDeadAfterHitSound; //Self-explanatory
        private SoundEffect shootSound; 
        private SoundEffect bgm; //Main background music (same for all levels)
        private SoundEffect shootInCD; //Sound played if player tries to shoot while on CD
        #endregion

        //PLAYER PROPERTIES
        #region PLAYER
        private float playerSpeed = 150.0f; //Ship's moving value
        private Vector2 playerPosition; //Handles the player's position during the whole game

        private float shootCooldown; //Cooldown value between each shots
        #endregion

        #endregion

        #region MAIN METHODS (6)

        //GAME CONSTRUCTOR METHOD
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = screen_height; //600px HEIGHT SCREEN
            graphics.PreferredBackBufferWidth = screen_width; //500px WIDTH SCREEN

            //Indicates the main content folder's name.
            Content.RootDirectory = "Content";

            //Initialize shooting and environment lists for display.
            projectilesOnScreen = new List<Projectile>();
            starsOnScreen = new List<Star>();
            //enemisOnScreen = new List<Enemy>();
        }

        //INITIALIZE METHOD
        protected override void Initialize()
        {
            //PLAYER INITIALIZATION
            #region PLAYER

            //Player position is at the middle of the screen by default
            playerPosition = new Vector2(screen_width / 2, screen_height / 2);
            shootCooldown = 1.0f; //Default shooting cooldown

            #endregion

            //ENVIRONMENT INITIALIZATION
            #region ENVIRONMENT
            bgmPlayable = true;

 
            star1 = Content.Load<Texture2D>("star1"); //Stars sprite loaded
            randomStarPos = new Random();
            //FOR LOOP - Creating all the screen's stars with random positions and speed
            for(int i = 0; i < 20; i++)
            {
                this._randomPosX = randomStarPos.Next(1, 500);
                this._randomPosY = randomStarPos.Next(1, 600);
                this._randomStarSpeedY = randomStarPos.NextDouble() * (1f - 0.15f) + 0.15f;
                RandomStars(star1, this._randomPosX, this._randomPosY, this._randomStarSpeedY);
            }
            #endregion

            //GAMEPLAY INITIALIZATION
            #region GAMEPLAY
            projectileSprite = Content.Load<Texture2D>("rocket");
            #endregion

            base.Initialize();
        }
        
        //LOADING CONTENT METHOD
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //PLAYER CONTENT LOAD:
            playerSprite = Content.Load<Texture2D>("ship");

            //SOUND CONTENT LOAD:
            bgm = Content.Load<SoundEffect>("BGM");
            enemyDeadAfterHitSound = Content.Load<SoundEffect>("hitSound");
            shootSound = Content.Load<SoundEffect>("shootSound");
            shootInCD = Content.Load<SoundEffect>("shootInCD");

            //ENVIRONMENT CONTENT LOAD:
            gameFont = Content.Load<SpriteFont>("gameFont");
            rocketIcon = Content.Load<Texture2D>("rocket");
        }

        //UNLOADING CONTENT METHOD
        protected override void UnloadContent(){}

        //GAME'S MAIN LOOP - UPDATE
        protected override void Update(GameTime gameTime)
        {
            #region SOUNDS & MUSICS
            //PLAYING BGM
            if (bgmPlayable == true)
            {
                bgmPlayable = false; //PlaysOnlyOnce
                bgm.Play();
            }
            #endregion

            #region DIRECTIONAL MOVING
            if (Keyboard.GetState().IsKeyDown(Keys.Z))
            { playerPosition.Y -= playerSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds; }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            { playerPosition.X += playerSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds; }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            { playerPosition.Y += playerSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds; }
            if (Keyboard.GetState().IsKeyDown(Keys.Q))
            { playerPosition.X -= playerSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds; }
            #endregion

            #region COOLDOWN
            shootCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            //blocks the timing at 0sec:
            if (shootCooldown <= 0)
            {
                shootCooldown = 0;
            }
            #endregion

            #region SHOOTING
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && shootCooldown <= 0)
            {
                //Creates a new rocket from a texture and a starting position.
                ShootBullet(projectileSprite, new Vector2(playerPosition.X - 15.5f, 
                    playerPosition.Y - 20));

                //If-projectile-is-shot checker
                foreach(Projectile p in projectilesOnScreen)
                {
                    p._isOnScreen = true;
                }

                shootSound.Play();
                shootCooldown = 1.0f;
            }

            //Update() method call for any Rocket that is on the screen, so that they move.
            foreach (Projectile p in projectilesOnScreen)
            {
                if (p._isOnScreen)
                {
                    p.Update(gameTime);
                }
            }
            #endregion

            #region ENVIRONMENT
            //Makes the stars crolling with random speeds
            foreach (Star s in starsOnScreen)
            {
                _randomStarSpeedY = randomStarPos.NextDouble() * (1f - 0.15f) + 0.15f;
                s.Update(gameTime, _randomStarSpeedY);
            }
            #endregion

            #region SYSTEM
            //Exits the game when pressing Esc
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            { Exit(); }
            #endregion

            base.Update(gameTime);
        }

        //GAME'S MAIN LOOP - DRAW
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);

            //Anything drawn on the screen between .Begin() and .End()
            spriteBatch.Begin();

            #region SCREEN
            spriteBatch.DrawString(gameFont, "Cooldown: " + shootCooldown.ToString("F0") + " sec", new Vector2(rocketIconPos.X + 28, 578), Color.White);
            spriteBatch.Draw(rocketIcon, rocketIconPos, Color.White);
            #endregion

            #region PLAYER
            spriteBatch.Draw(playerSprite, playerPosition, null, Color.White, 0f, new Vector2(playerSprite.Width / 2, playerSprite.Height / 2),
                Vector2.One, SpriteEffects.None, 0.1f);

            foreach(Projectile p in projectilesOnScreen)
            {
                if (p._isOnScreen)
                {
                    p.Draw(gameTime, spriteBatch);
                }
            }
            #endregion

            #region ENVIRONMENT
            foreach(Star s in starsOnScreen)
            {
                s.Draw(gameTime, spriteBatch);
            }
            #endregion

            spriteBatch.End();

            base.Draw(gameTime);
        }
        #endregion

        #region OTHER METHODS
        
        //WHEN SHOOTING A BULLET (Called in Update() when pressing the Shooting Button)
        public void ShootBullet(Texture2D texture, Vector2 startPos)
        {
            Projectile projectile = new Projectile(texture, startPos);
            projectilesOnScreen.Add(projectile);
        }

        //SPAWNING STARS IN THE ENVIRONMENT (Called in Initialize() method, because nothing interacts with that)
        public void RandomStars(Texture2D texture, int rdmPosX, int rdmPosY, double rdmYSpeed)
        {
            Star star = new Star(texture, rdmPosX, rdmPosY, rdmYSpeed);
            starsOnScreen.Add(star);
        }
        #endregion
    }
}
