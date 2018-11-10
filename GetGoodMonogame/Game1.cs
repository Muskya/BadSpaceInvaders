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
        //Test liaison nouvelle branche pour vagues de monstres
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
        private Texture2D fireModeIcon;
        private bool singleFireMode = true;

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
        List<Star> starsOnScreen; //Infinite-scrolling stars on the screen

        //Random variable to set random X and Y star positions
        Random randomStarPos;
        #endregion

        //ENVIRONMENT PROPERTIES
        #region TEXTURE
        private Texture2D projectileSprite;
        private Texture2D enemySprite;
        private Rectangle enemyCollibox;
        private float cooldownWaves;

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
            starsOnScreen = new List<Star>();
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
            //bgmInstance.IsLooped = true;
            //bgmInstance.Play();

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
            cooldownWaves = 5.0f;

            //FOR LOOP STARS - Creating all the screen's stars with random positions and speed
            for (int i = 0; i < 20; i++)
            {
                this._randomPosX = randomStarPos.Next(1, 500);
                this._randomPosY = randomStarPos.Next(1, 600);
                //RandomStars() method call that creates a new star and adds it to the list
                RandomStars(star1, this._randomPosX, this._randomPosY, this._randomStarSpeedY);
            }

            //Loading enemie sprites
            enemySprite = Content.Load<Texture2D>("enemy");
            
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
            fireModeIcon = Content.Load<Texture2D>("firemode");

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
                    if (Keyboard.GetState().IsKeyDown(Keys.Z))
                    { playerPosition.Y -= playerSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds; }
                    if (Keyboard.GetState().IsKeyDown(Keys.D))
                    { playerPosition.X += playerSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds; }
                    if (Keyboard.GetState().IsKeyDown(Keys.S))
                    { playerPosition.Y += playerSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds; }
                    if (Keyboard.GetState().IsKeyDown(Keys.Q))
                    { playerPosition.X -= playerSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds; }
                }

                //Screen bounds
                if (playerPosition.X < 0)
                    playerPosition.X = 1;
                else if (playerPosition.X > (500 - playerSprite.Width))
                    playerPosition.X = (500 - playerSprite.Width);
                else if (playerPosition.Y > (600 - playerSprite.Height))
                    playerPosition.Y = (600 - playerSprite.Height);
                else if (playerPosition.Y < 0)
                    playerPosition.Y = 1;

                
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
                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                    singleFireMode = false;
                else if (Keyboard.GetState().IsKeyDown(Keys.Left))
                    singleFireMode = true;

                if ((Keyboard.GetState().IsKeyDown(Keys.Space) && shootCooldown <= 0) && !playerIsDead)
                {
                    if (singleFireMode == true) {

                        //Creates a new rocket from a texture and a starting position.
                        ShootBullet(projectileSprite, new Vector2(playerPosition.X + 6.5f, playerPosition.Y));
                        //Shooting sound
                        shootSound.Play();
                        shootCooldown = 1.0f;

                    } else if (singleFireMode == false) {
                        //Créer les trois roquettes dans la liste
                        ShootBulletBurst(projectileSprite, new Vector2(playerPosition.X + 6.5f, playerPosition.Y));
                        shootSound.Play();
                        shootCooldown = 2.0f;
                    }                
                }
                    
                #endregion

                playerCollibox = new Rectangle((int)playerPosition.X, (int)playerPosition.Y, (int)playerSprite.Width, (int)playerSprite.Height);
                cooldownWaves -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                //Quand le cooldown de waves est à 0 (on créer une nouvelle vague)
                if (cooldownWaves <= 0)
                {
                    for (int i = 0; i < 12; i++)
                    {
                        SpawnMonsterWave(enemySprite, new Vector2(30 + (i * 37), 20));
                    }

                    cooldownWaves = 7.0f;
                }

                //Update() method call for any Rocket that is on the screen, so that they move.
                foreach (Projectile p in Projectile.projectilesOnScreen)
                {
                    //Console.WriteLine(p._position.Y);
                    //Moves the rocket as long as it's on the screen (after shooting and before hitting an enemy)
                    if (p._hasHitEnemy == false && p._isOnScreen == true)
                    {
                        p.Update(gameTime);
                    }
                    else {
                        p._collisionBox = Rectangle.Empty;
                    }

                    //ForEach used to manage collisions between Enemies and Projectiles
                    foreach (Enemy e in Enemy.enemiesOnScreen)
                    {
                        if (p._collisionBox.Intersects(e._collisionBox))
                        {
                            score += 15;
                            enemyDeathSoundInstance.Play();
                            enemyDeathSoundInstance.Pause();
                            p._hasHitEnemy = true;
                            e._isDead = true;
                        }
                    }
                }

                foreach (Projectile p in Projectile.projectilesBurst)
                {
                    if (p._hasHitEnemy == false && p._isOnScreen == true)
                    {
                        p.UpdateAlt(gameTime, p._angle);
                    }
                    else
                    {
                        p._collisionBox = Rectangle.Empty;
                    }

                    foreach (Enemy e in Enemy.enemiesOnScreen)
                    {
                        if (p._collisionBox.Intersects(e._collisionBox))
                        {
                            score += 15;
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
                    if (e._isDead == true)
                    {
                        //Basically sets the enemy collision box to "null". Can't do it explicitely because Rectangle is a struct.
                        e._collisionBox = Rectangle.Empty;
                    }
                    else
                    {
                        if (e._collisionBox.Intersects(playerCollibox))
                        {
                            playerDeathInstance.Play();
                            playerDeathInstance.Pause();
                            playerIsDead = true;
                            playerCollibox = Rectangle.Empty;
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
            { _gameState = GameState.MainMenu; }
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
                }
                #endregion

                #region SCREEN
                //Rocket icon
                spriteBatch.Draw(rocketIcon, rocketIconPos, Color.White);
                
                if (singleFireMode)
                {
                    spriteBatch.Draw(fireModeIcon, new Vector2(rocketIconPos.X + 7.5f, rocketIconPos.Y - 20), Color.White);
                    spriteBatch.DrawString(gameFont, "Semi-auto", new Vector2(rocketIconPos.X + 22, rocketIconPos.Y - 24), Color.Orange);
                } else
                {
                    spriteBatch.Draw(fireModeIcon, new Vector2(rocketIconPos.X + 7.5f, rocketIconPos.Y - 20), Color.White);
                    spriteBatch.Draw(fireModeIcon, new Vector2(rocketIconPos.X + 15, rocketIconPos.Y - 20), Color.White);
                    spriteBatch.Draw(fireModeIcon, new Vector2(rocketIconPos.X + 22.5f, rocketIconPos.Y - 20), Color.White);
                    spriteBatch.DrawString(gameFont, "Burst", new Vector2(rocketIconPos.X + 37, rocketIconPos.Y - 24), Color.Orange);
                }

                //Draws the cooldown string in green if player canShoot
                if (shootCooldown == 0.0f)
                    spriteBatch.DrawString(gameFont, "Cooldown: " + shootCooldown.ToString("F2") + " sec", new Vector2(rocketIconPos.X + 28, 578), Color.Green);
                else
                    spriteBatch.DrawString(gameFont, "Cooldown: " + shootCooldown.ToString("F2") + " sec", new Vector2(rocketIconPos.X + 28, 578), Color.White);

                spriteBatch.DrawString(gameFont, "Wave CD: " + cooldownWaves.ToString("F2") + " sec", new Vector2(345, 578), Color.White);
                spriteBatch.DrawString(gameFont, "Score: " + score.ToString(), new Vector2(0, 0), Color.White);
                #endregion

                #region PLAYER
                //Debug draw (player collision box in green)
                //spriteBatch.Draw(rectPlayer, playerPosition, Color.White);
                if (playerIsDead == true)  {

                } else {
                    spriteBatch.Draw(playerSprite, playerPosition, Color.White);
                }
                     
                //Draws the projectile if is shot (onScreen) and it hasn't hit an enemy yet (!hasHitEnemy)
                foreach (Projectile p in Projectile.projectilesOnScreen)
                {
                    if (p._hasHitEnemy == false && p._isOnScreen == true)
                    {
                       //Debug draw. Draws the projectile collision box in green.
                       //spriteBatch.Draw(rectProjectile, new Vector2(p._position.X + 12, p._position.Y), Color.White);
                       p.Draw(gameTime, spriteBatch);
                    }
                    else if (p._hasHitEnemy == true) { }
                    else if (!p._isOnScreen == false) { }       
                }

                foreach (Projectile p in Projectile.projectilesBurst)
                {
                        if (p._hasHitEnemy == false && p._isOnScreen == true)
                        {
                            //Debug draw. Draws the projectile collision box in green.
                            //spriteBatch.Draw(rectProjectile, new Vector2(p._position.X + 12, p._position.Y), Color.White);   
                            p.DrawAlt(gameTime, spriteBatch, p._angle);
                        }
                        else if (p._hasHitEnemy == true) { }
                        else if (!p._isOnScreen == false) { }  
                }
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
            Projectile.projectilesOnScreen.Add(projectile);
        }

        public void ShootBulletBurst(Texture2D texture, Vector2 startPos)
        {
            Projectile projectileLeft = new Projectile(texture, new Vector2(startPos.X - 15, startPos.Y + 17), -0.60f);
            Projectile projectileMiddle = new Projectile(texture, startPos, 0);
            Projectile projectileRight = new Projectile(texture, new Vector2(startPos.X + 20, startPos.Y), 0.60f);
            Projectile.projectilesBurst.Add(projectileRight);
            Projectile.projectilesBurst.Add(projectileLeft);
            Projectile.projectilesBurst.Add(projectileMiddle);
            
            
        }

        //SPAWNING STARS IN THE ENVIRONMENT (Called in Initialize() method, because nothing interacts with that)
        public void RandomStars(Texture2D texture, int rdmPosX, int rdmPosY, double rdmYSpeed)
        {
            Star star = new Star(texture, rdmPosX, rdmPosY, rdmYSpeed);
            starsOnScreen.Add(star);
        }

        //SPAWNING MONSTERS IN THE ENVIRONMENT
        //SpawnMonsterWave(enemySprite, new Vector2(30 + (i * 37), 20), enemyCollibox);
        public void SpawnMonsterWave(Texture2D texture, Vector2 pos)
        {
            enemyCollibox = new Rectangle(0, 0, (int)enemySprite.Width, (int)enemySprite.Height);
            Enemy enemy = new Enemy(texture, pos, enemyCollibox);
            Enemy.enemiesOnScreen.Add(enemy);
        }

        #endregion
    }
}
