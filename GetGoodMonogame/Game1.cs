using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

//Commentaire extra - GitHub Desktop utilisé pour ce repository

namespace GetGoodMonogame
{
    public class Game1 : Game
    {
        // SCREEN PROPERTIES
        GraphicsDeviceManager graphics; // L'écran en gros 
        SpriteBatch spriteBatch; // Layer d'affichage 
        //window's dimensions:
        private int screen_width = 500; 
        private int screen_height = 600;

        // GAME PROPERTIES
        private bool bgmPlayable = false;
        private bool projectileShot = false;
        private SpriteFont gameFont;

        // CLASSES INSTANCIATIONS
        Projectile projectile;

        // TEXTURES
        private Texture2D projectileSprite;
        private Texture2D playerSprite;
        private Texture2D brick;

        private Texture2D rocketIcon;
        private Vector2 rocketIconPos = new Vector2(-5, 570);

        // SOUNDS EFFECTS
        private SoundEffect hitSound;
        private SoundEffect afterHitSound;
        private SoundEffect shootSound;
        private SoundEffect bgm;
        private SoundEffect shootInCD;

        // PLAYER PROPERTIES
        private float playerSpeed = 150.0f;
        private Vector2 playerPosition;

        //shooting:
        private float shootCooldown;

        // Constructeur du jeu
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = screen_height; // 600px height
            graphics.PreferredBackBufferWidth = screen_width; // 500px width

            Content.RootDirectory = "Content";
        }

        // Initialize game logic
        protected override void Initialize()
        {
            playerPosition = new Vector2(screen_width/2, screen_height/2);
            //playerPosition.X - 15.5f, playerPosition.Y - 20 ||| Projectile position to actually be at the right position under the space ship
            projectile = new Projectile();
            projectile.ActivateProjectile(projectileSprite);

            bgmPlayable = true;
            shootCooldown = 2.5f;

            base.Initialize();
        }

        // Load all the needed content
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //player content:
            playerSprite = Content.Load<Texture2D>("ship");
            rocketIcon = Content.Load<Texture2D>("rocket");
            projectile.projectileSprite = Content.Load<Texture2D>("rocket");

            //sound content:
            bgm = Content.Load<SoundEffect>("BGM");
            afterHitSound = Content.Load<SoundEffect>("hitSound");
            shootSound = Content.Load<SoundEffect>("shootSound");
            shootInCD = Content.Load<SoundEffect>("shootInCD");

            //environment content:
            brick = Content.Load<Texture2D>("brick1");
            gameFont = Content.Load<SpriteFont>("gameFont");
        }

        protected override void UnloadContent(){}

        // Game's main loop
        protected override void Update(GameTime gameTime)
        {
            #region BGM
            // Playing BGM
            //if (bgmPlayable == true)
            //{
            //    bgmPlayable = false; //PlaysOnlyOnce
            //    bgm.Play();
            //}
            #endregion

            // Exits the game when pressing Esc
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            { Exit(); }

            // Shooting + cooldown system
            shootCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && shootCooldown <= 0)
            {
                projectile.SetPosition(new Vector2(playerPosition.X - 15.5f,
                    playerPosition.Y - 20));
                projectile.projectileIsShot = true;

                shootSound.Play();
                shootCooldown = 2.5f;
            }

            if (projectile.projectileIsActive && projectile.projectileIsShot)
            {
                projectile.projectilePosition.Y -= 5;
                //projectile.Update(gameTime);
            }

            //if (projectile.projectilePosition.Y < -30)
            //    projectile.Kill();
            //if (projectile.projectilePosition.X < -30 || projectile.projectilePosition.X > 530)
            //    projectile.Kill();

            //blocks the timing at 0sec:
            if (shootCooldown <= 0) {
                shootCooldown = 0; }
            
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

            base.Update(gameTime);
        }

        // Draw to screen EACH frame (60 fps by default)
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            spriteBatch.DrawString(gameFont, "Cooldown: " + shootCooldown.ToString("F0") + " sec", new Vector2(rocketIconPos.X + 28, 578), Color.White);
            spriteBatch.Draw(rocketIcon, rocketIconPos, Color.White);
            spriteBatch.Draw(playerSprite, playerPosition, null, Color.White, 0f, new Vector2(playerSprite.Width / 2, playerSprite.Height / 2),
                Vector2.One, SpriteEffects.None, 0.1f);
            //projectile.Draw(gameTime, spriteBatch, projectileSprite);

            if (projectile.projectileIsActive && projectile.projectileIsShot)
            {
                spriteBatch.Draw(projectile.projectileSprite, projectile.projectilePosition,
                Color.White);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
