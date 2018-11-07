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

        private Texture2D mainMenuBackground;

        //window's dimensions:
        private int screen_width = 500; 
        private int screen_height = 600;

        Button quitButton, playButton;
        private Texture2D noneQuitButtonTexture, hoverQuitButtonTexture, nonePlayButtonTexture, hoverPlayButtonTexture;
        private Rectangle quitRectangle, playRectangle;

        private Texture2D rocketIcon;
        //The rocket icon position at the bottom of the screen
        private Vector2 rocketIconPos = new Vector2(-5, 570);

        private int score;


        #endregion

        //GAME PROPERTIES
        #region GAME
        private SpriteFont gameFont;

        public enum GameState {
            MainMenu = 0,
            Playing = 1,
            Quit = 2
        }
       
        public static GameState _gameState;
        private MouseState _mouseState;
        #endregion

        //CLASSES INSTANCIATIONS AND PROPERTIES
        #region CLASSES
        List<Projectile> projectilesOnScreen; //Rockets shot on the screen
        List<Star> starsOnScreen; //Infinite-scrolling stars on the screen
        List<Enemy> enemiesOnScreen; //Enemies on the screen

        //Random variable to set random X and Y star positions
        Random randomStarPos;
        #endregion

        //ENVIRONMENT PROPERTIES
        #region TEXTURE
        private Texture2D projectileSprite;
        private Texture2D enemySprite;
        private Rectangle enemyCollibox;

        //Debug-purpose textures. (Green collision boxes)
        private Texture2D rectEnemy;
        private Texture2D rectProjectile;
        private Texture2D rectPlayer;

        //Environment stars properties
        private Texture2D star1;
        public int _randomPosX;
        public int _randomPosY;
        public double _randomStarSpeedY;
        #endregion

        //SOUNDS PROPERTIES
        #region SOUND
        private SoundEffect hitSound; //When rocket hits an enemy
        private SoundEffect enemyDeadAfterHitSound; //Self-explanatory
        private SoundEffect enemyDeathSound;
        private SoundEffectInstance enemyDeathSoundInstance;
        private SoundEffect shootSound; 

        private SoundEffect bgm; //Main background music (same for all levels)
        private bool bgmPlayable = false;
        private SoundEffectInstance bgmInstance;

        private SoundEffect shootInCD; //Sound played if player tries to shoot while on CD
        #endregion

        //PLAYER PROPERTIES
        #region PLAYER
        private SoundEffect playerDeath;
        private SoundEffectInstance playerDeathInstance;
        private float playerSpeed = 250.0f; //Ship's moving value
        private Vector2 playerPosition; //Handles the player's position during the whole game
        private Texture2D playerSprite;
        private Rectangle playerCollibox;
        private float shootCooldown; //Cooldown value between each shots
        private bool playerIsDead;
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
            enemiesOnScreen = new List<Enemy>();
        }

        // ######## INITIALIZE METHOD ######## //
        protected override void Initialize()
        {
            //SOUND INITIALIZATION
            #region SOUND
            bgm = Content.Load<SoundEffect>("BGM");
            bgmPlayable = true;
            bgmInstance = bgm.CreateInstance();

            //Played() and IsLooped here because nothing interacts with the bgm to enable it
            bgmInstance.IsLooped = true;
            bgmInstance.Play();

            enemyDeathSound = Content.Load<SoundEffect>("enemyDeath");
            enemyDeathSoundInstance = enemyDeathSound.CreateInstance();
            #endregion

            //GAME INITIALIZATION
            #region GAME
            mainMenuBackground = Content.Load<Texture2D>("Mainmenu");
            _gameState = GameState.MainMenu;
            _mouseState = Mouse.GetState();

            //Loads the button textures for Nonestate and HoveredState
            noneQuitButtonTexture = Content.Load<Texture2D>("quitButtonNone");
            hoverQuitButtonTexture = Content.Load<Texture2D>("quitButtonHovered");

            nonePlayButtonTexture = Content.Load<Texture2D>("playButtonNone");
            hoverPlayButtonTexture = Content.Load<Texture2D>("playButtonHovered");

            //Creates the buttons
            quitButton = new Button(noneQuitButtonTexture, hoverQuitButtonTexture, new Vector2(275, 400));
            playButton = new Button(nonePlayButtonTexture, hoverPlayButtonTexture, new Vector2(75, 400));

            //Sets the button rectangles
            quitRectangle = new Rectangle((int)quitButton._position.X, (int)quitButton._position.Y, (int)noneQuitButtonTexture.Width, (int)noneQuitButtonTexture.Height);
            playRectangle = new Rectangle((int)playButton._position.X, (int)playButton._position.Y, (int)nonePlayButtonTexture.Width, (int)nonePlayButtonTexture.Height);
            quitButton.SetRectangle(quitRectangle);
            playButton.SetRectangle(playRectangle);

            #endregion

            //PLAYER INITIALIZATION
            #region PLAYER

            //Player position is at the middle of the screen by default
            playerPosition = new Vector2(screen_width / 2, screen_height / 2);
            shootCooldown = 1.5f; //Default shooting cooldown
            playerSprite = Content.Load<Texture2D>("ship");
            playerCollibox = new Rectangle((int)playerPosition.X, (int)playerPosition.Y, (int)playerSprite.Width, (int)playerSprite.Height);
            playerDeath = Content.Load<SoundEffect>("playerDeath");
            playerDeathInstance = playerDeath.CreateInstance();
            #endregion

            //ENVIRONMENT INITIALIZATION
            #region ENVIRONMENT
            star1 = Content.Load<Texture2D>("star1"); //Stars sprite loaded
            randomStarPos = new Random();

            //FOR LOOP STARS - Creating all the screen's stars with random positions and speed
            for (int i = 0; i < 20; i++)
            {
                this._randomPosX = randomStarPos.Next(1, 500);
                this._randomPosY = randomStarPos.Next(1, 600);
                //RandomStars() method call that creates a new star and adds it to the list
                RandomStars(star1, this._randomPosX, this._randomPosY, this._randomStarSpeedY);
            }

            //Loads the enemy sprite before creating some of them
            enemySprite = Content.Load<Texture2D>("enemy");
            for (int i = 0; i < 12; i++)
            {
                enemyCollibox = new Rectangle(0, 0, (int)enemySprite.Width, (int)enemySprite.Height);
                SpawnMonster(enemySprite, new Vector2(30 + (i * 37), 20), enemyCollibox);
            }
            #endregion

            //GAMEPLAY INITIALIZATION
            #region GAMEPLAY
            projectileSprite = Content.Load<Texture2D>("rocket");
            score = 0; //Initializes the score to 0 to allow calculations

            #endregion

            #region DEBUGGING
            //Debug purposes (Displays green rectangle around enemy and projectile sprites to show their collision box.)
            rectProjectile = new Texture2D(graphics.GraphicsDevice, projectileSprite.Width - 24, projectileSprite.Height);
            rectEnemy = new Texture2D(graphics.GraphicsDevice, enemySprite.Width, enemySprite.Height);
            rectPlayer = new Texture2D(graphics.GraphicsDevice, playerSprite.Width, playerSprite.Height);

            //Arrays de pixels vert pour créer les rectangles autour des sprites
            Color[] data = new Color[rectEnemy.Height * rectEnemy.Width];
            Color[] data1 = new Color[rectProjectile.Height * rectProjectile.Width];
            Color[] data2 = new Color[rectPlayer.Height * rectPlayer.Width];

            //Créer des rectangles autour du sprite en vert
            for (int i = 0; i < data.Length; ++i)
                data[i] = Color.Green;
            for (int i = 0; i < data1.Length; ++i)
                data1[i] = Color.Green;
            for (int i = 0; i < data2.Length; ++i)
                data2[i] = Color.Green;

            rectEnemy.SetData(data);
            rectProjectile.SetData(data1);
            rectPlayer.SetData(data2);
            #endregion

            base.Initialize();
        }

        // ######## LOAD CONTENT METHOD ######## //
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            //SOUND CONTENT LOAD:
            shootSound = Content.Load<SoundEffect>("shootSound");

            //ENVIRONMENT CONTENT LOAD:
            gameFont = Content.Load<SpriteFont>("gameFont");
            rocketIcon = Content.Load<Texture2D>("rocket");
        }
        // ######## UNLOAD CONTENT METHOD ######## //
        protected override void UnloadContent(){}

        // ######## UPDATE METHOD ######## //
        protected override void Update(GameTime gameTime)
        {

            if (_gameState == GameState.MainMenu)
            {
                IsMouseVisible = true;

                playButton.Update(gameTime, "play");
                quitButton.Update(gameTime, "quit");
            }

            if (_gameState == GameState.Playing)
            {
                IsMouseVisible = false;

                #region DIRECTIONAL MOVING
                if (!playerIsDead)
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.Z) || Keyboard.GetState().IsKeyDown(Keys.Up))
                    { playerPosition.Y -= playerSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds; }
                    if (Keyboard.GetState().IsKeyDown(Keys.D) || Keyboard.GetState().IsKeyDown(Keys.Right))
                    { playerPosition.X += playerSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds; }
                    if (Keyboard.GetState().IsKeyDown(Keys.S) || Keyboard.GetState().IsKeyDown(Keys.Down))
                    { playerPosition.Y += playerSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds; }
                    if (Keyboard.GetState().IsKeyDown(Keys.Q) || Keyboard.GetState().IsKeyDown(Keys.Left))
                    { playerPosition.X -= playerSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds; }
                }                
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
                //Pressing the spacebar
                if ((Keyboard.GetState().IsKeyDown(Keys.Space) && shootCooldown <= 0) && !playerIsDead)
                {
                    //Creates a new rocket from a texture and a starting position.
                    ShootBullet(projectileSprite, new Vector2(playerPosition.X + 6, playerPosition.Y));

                    //Sets every Projectile to "isOnScreen" after shooting (spacebar)
                    foreach (Projectile p in projectilesOnScreen)
                    {
                        p._isOnScreen = true;
                    }

                    //Shooting sound
                    shootSound.Play();
                    shootCooldown = 1.0f;
                }

                playerCollibox = new Rectangle((int)playerPosition.X, (int)playerPosition.Y, (int)playerSprite.Width, (int)playerSprite.Height);

                //Update() method call for any Rocket that is on the screen, so that they move.
                foreach (Projectile p in projectilesOnScreen)
                {

                    //Moves the rocket as long as it's on the screen (after shooting and before hitting an enemy)
                    if (p._isOnScreen)
                    {
                        p.Update(gameTime);
                    }
                    else { }

                    //ForEach used to manage collisions between Enemies and Projectiles
                    foreach (Enemy e in Enemy.enemiesOnScreen)
                    {
                        if (p._collisionBox.Intersects(e._collisionBox))
                        {
                            score += 10;
                            enemyDeathSoundInstance.Play();
                            enemyDeathSoundInstance.Pause();
                            p._hasHitEnemy = true;
                            e._isDead = true;
                        }
                    }
                }

                //Only updates Enemies movement if the Enemy isn't dead
                foreach (Enemy e in Enemy.enemiesOnScreen)
                {
                    if (e._isDead)
                    {
                        //Basically sets the enemy collision box to "null". Can't do it explicitely because Rectangle is a struct.
                        e._collisionBox = Rectangle.Empty;
                    }
                    else
                    {
                        if (e._collisionBox.Intersects(playerCollibox))
                        {
                            playerCollibox = Rectangle.Empty;
                            playerDeathInstance.Play();
                            playerDeathInstance.Pause();
                            playerIsDead = true;
                        }
                        e.Update(gameTime);
                    }
                }

                #endregion

                #region ENVIRONMENT
                //Makes the stars crolling with random speeds
                foreach (Star s in starsOnScreen)
                {
                    _randomStarSpeedY = randomStarPos.NextDouble() * (1f - 0.00001f) + 0.00001f;
                    s.Update(gameTime, _randomStarSpeedY);
                }
                #endregion
            }

            #region SYSTEM

            //Exits the game when pressing Esc
            if (_gameState == GameState.Quit)
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            { Exit(); }
            #endregion

            base.Update(gameTime);
        }

        // ######## DRAW METHOD ######### //
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            if (_gameState == GameState.MainMenu)
            {             
                spriteBatch.Draw(mainMenuBackground, Vector2.Zero, Color.White);
                playButton.Draw(spriteBatch);
                quitButton.Draw(spriteBatch); 
            }

            if (_gameState == GameState.Playing)
            {
                #region ENVIRONMENT
                //Draws each star of the collection initially
                foreach (Star s in starsOnScreen)
                {
                    s.Draw(gameTime, spriteBatch);
                }

                //Draws each Enemy of the collection only if he's not dead
                foreach (Enemy e in Enemy.enemiesOnScreen)
                {
                    //Si l'ennemi est mort, on ne draw rien
                    if (e._isDead) { }
                    //Sinon, l'ennemi n'est pas mort, donc là on le draw
                    else
                    {
                        //Debug draw. Draws the enemy collision box in Green.
                        //spriteBatch.Draw(rectEnemy, e._position, Color.White);
                        e.Draw(gameTime, spriteBatch);
                    }

                    //Si un ennemi rentre en collision avec le joueur
                    if (e._collisionBox.Intersects(playerCollibox))
                    {

                        //Gameover, life decrement, etc
                    }
                }
                #endregion

                #region SCREEN
                spriteBatch.Draw(rocketIcon, rocketIconPos, Color.White);

                //Draws the cooldown string in green if player canShoot
                if (shootCooldown == 0.0f)
                    spriteBatch.DrawString(gameFont, "Cooldown: " + shootCooldown.ToString("F2") + " sec", new Vector2(rocketIconPos.X + 28, 578), Color.Green);
                else
                    spriteBatch.DrawString(gameFont, "Cooldown: " + shootCooldown.ToString("F2") + " sec", new Vector2(rocketIconPos.X + 28, 578), Color.White);

                spriteBatch.DrawString(gameFont, "Score: " + score.ToString(), new Vector2(0, 0), Color.White);
                #endregion

                #region PLAYER
                //Debug draw (player collision box in green)
                //spriteBatch.Draw(rectPlayer, playerPosition, Color.White);
                if (playerIsDead)  {

                } else  {
                    spriteBatch.Draw(playerSprite, playerPosition, Color.White);
                }
                     
                //Draws the projectile if is shot (onScreen) and it hasn't hit an enemy yet (!hasHitEnemy)
                foreach (Projectile p in projectilesOnScreen)
                {
                    if (p._isOnScreen && !p._hasHitEnemy)
                    {
                        //Debug draw. Draws the projectile collision box in green.
                        //spriteBatch.Draw(rectProjectile, new Vector2(p._position.X + 12, p._position.Y), Color.White);
                        p.Draw(gameTime, spriteBatch);
                    }
                    else {

                    }
                }

                #endregion
                
            }

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

        //SPAWNING MONSTERS IN THE ENVIRONMENT
        public void SpawnMonster(Texture2D texture, Vector2 pos, Rectangle colliBox)
        {
            Enemy enemy = new Enemy(texture, pos, colliBox);
            Enemy.enemiesOnScreen.Add(enemy);
        }
        #endregion
    }
}
