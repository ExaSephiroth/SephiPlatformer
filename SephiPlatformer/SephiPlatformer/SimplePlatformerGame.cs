using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Threading;

namespace SephiPlatformer
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class SimplePlatformerGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private Texture2D tileTexture, jumperTexture, aerisTexture, backgroundTexture;
    //    private Sprite tileSprite;
        private Jumper sephiSprite;
        private Aeris aerisSprite;
      //  private Tile blockedTile, unblockedTile;

       // private Sprite backgroundSprite;
        private Board board;
        private SpriteFont debugFont, scoreFont;
        private string isOnFirmGroundText, scoreString, videoPlayerState;
        private int score;

        private Video video;
        private VideoPlayer videoPlayer;
        private Texture2D videoTexture;
        private Texture2D startButton;
        private Texture2D exitButton;

        private Thread backgroundThread;
        private bool isLoading = false;
        private bool waitIntro = false;
        private bool alreadyFired = false;

        private List<Rectangle> startMenuRList = new List<Rectangle>();
        private List<Texture2D> startMenuBList = new List<Texture2D>();
        private int startMenuListindex;
        private KeyboardState previousState;

        enum GameState
        {
            Playing,
            StartMenu,
            Loading,
            Paused
        }

        private GameState gameState;
        

        public SimplePlatformerGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 960;
            graphics.PreferredBackBufferHeight = 640;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            gameState = GameState.Playing;
            startMenuListindex = 0;
            startMenuRList.Add(new Rectangle(250, 400, 200, 100));
            startMenuRList.Add(new Rectangle(250, 500, 200, 100));
            previousState = Keyboard.GetState();

            
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            tileTexture = Content.Load<Texture2D>("tile");
          //  jumperTexture = Content.Load<Texture2D>("Sephiroth");
            jumperTexture = Content.Load<Texture2D>("axlswag");
            aerisTexture = Content.Load<Texture2D>("swag");
            backgroundTexture = Content.Load<Texture2D>("background");
            startButton = Content.Load<Texture2D>("start");
            exitButton = Content.Load<Texture2D>("exit");

            startMenuBList.Add(startButton);
            startMenuBList.Add(exitButton);
            
            sephiSprite = new Jumper(jumperTexture, new Vector2(480, 160), spriteBatch);
          //  tileSprite = new Sprite(tileTexture, new Vector2(150, 160), spriteBatch);
         //   blockedTile = new Tile(tileTexture, new Vector2(200, 200), spriteBatch, true);
         //   unblockedTile = new Tile(tileTexture, new Vector2(0, 200), spriteBatch, false);
            board = new Board(tileTexture, spriteBatch, 10, 15);

            aerisSprite = new Aeris(aerisTexture, new Vector2(400, 400), spriteBatch, true);

            debugFont = Content.Load<SpriteFont>("debugFont");
            scoreFont = Content.Load<SpriteFont>("scoreFont");
            // TODO: use this.Content to load your game content here

            video = Content.Load<Video>("vid1");
            videoPlayer = new VideoPlayer();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here

           

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (gameState == GameState.StartMenu ) //isLoading bool is to prevent the LoadGame method from being called 60 times a seconds
            {

                CheckKeyboardAndReactMenu();


                if(!alreadyFired)
                {

                    backgroundThread = new Thread(StartGame);

                    //start backgroundthread
                    backgroundThread.Start();
                }
                
            
            }

            // TODO: Add your update logic here
           if (gameState == GameState.Loading && !isLoading) //isLoading bool is to prevent the LoadGame method from being called 60 times a seconds
           {
                    //set backgroundthread
                    backgroundThread = new Thread(LoadGame);
                    isLoading = true;

                    //start backgroundthread
                    backgroundThread.Start();
            }


            

            if(gameState == GameState.Playing)
            {
              //  CheckKeyboardAndReact();

            

                base.Update(gameTime);
                sephiSprite.Update(gameTime);

                checkIfKilled();

                if (videoPlayer.State == MediaState.Stopped)
                {
                    videoPlayer.Stop();
                    videoTexture = null;
                }
                CheckKeyboardAndReact();

            }
        }

        void StartGame()
        {
            alreadyFired = true;
            Thread.Sleep(1500);
            waitIntro = true;
        }

        void LoadGame()
        {
            Thread.Sleep(1000);

            //start playing
            gameState = GameState.Playing;
            isLoading = false;
        }

        private void checkIfKilled()
        {
            if (sephiSprite.Bounds.Intersects(aerisSprite.Bounds) && aerisSprite.IsAlive)
            {
                score++;
                aerisSprite.IsAlive = false;
                videoPlayer.Play(video);
     
            }
        }

        private void CheckKeyboardAndReact()
        {
            
            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.F5)) { RestartGame(); }
            if (state.IsKeyDown(Keys.Escape)) { Exit(); }
            if (state.IsKeyDown(Keys.F2))
            { loadSpritePosition(); }
            if (state.IsKeyDown(Keys.F3))
            { Serializer.Save(sephiSprite.Position); }

         
            

        }

        private void CheckKeyboardAndReactMenu()
        {

            KeyboardState state = Keyboard.GetState();
            
            if (state.IsKeyDown(Keys.Escape)) { Exit(); }

            if (state.IsKeyDown(Keys.Up) && !previousState.IsKeyDown(Keys.Up)) // of --> previousState.IsKeyUp(Keys.Up))
            {
                startMenuListindex--;
                previousState = state;
            }
                
            if (state.IsKeyDown(Keys.Down) && !previousState.IsKeyDown(Keys.Down))
            {
                startMenuListindex++;
                previousState = state;
            }

            if (state.IsKeyDown(Keys.Enter))
            {
                
                gameState = (GameState)startMenuListindex;
                System.Diagnostics.Debug.WriteLine((GameState)startMenuListindex);
            }


        }

        private void loadSpritePosition()
        {
            sephiSprite.Position = Serializer.Load<Vector2>();
        }

        private void RestartGame()
        {
            Board.CurrentBoard.CreateNewBoard();
            PutJumperInTopLeftCorner();
            MakeAerisLive();
            score = 0;
        }

        private void MakeAerisLive()
        {
            aerisSprite.IsAlive = true;
            aerisSprite.Position = Vector2.One * 160;
        }

        private void PutJumperInTopLeftCorner()
        {
            sephiSprite.Position = Vector2.One * 70;
            sephiSprite.Movement = Vector2.Zero;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.WhiteSmoke);

            spriteBatch.Begin();


            base.Draw(gameTime);

            if (gameState == GameState.StartMenu)
            {
                spriteBatch.Draw(backgroundTexture,new Rectangle(0, 0, 960, 640), Color.LightGray);

                if(waitIntro)
                {
                    sephiSprite.Draw();

                    for (var i = 0; i < startMenuRList.Count; i++)
                    {
                        if (i != startMenuListindex)
                            spriteBatch.Draw(startMenuBList[i], startMenuRList[i], Color.White);
                        else
                            spriteBatch.Draw(startMenuBList[i], startMenuRList[i], Color.Red);
                    }
                    /*
                    spriteBatch.Draw(startButton, new Rectangle(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2 , 100, 50), Color.LightGray);
                    spriteBatch.Draw(exitButton, new Rectangle(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2 + 50, 100, 50), Color.LightGray);
                    */
                }
                
            }


            if (gameState == GameState.Playing)
            {

                /* tileSprite.Draw();*/
                PutJumperInTopLeftCorner();
                board.Draw();

                WriteDebugInformation();
                WriteScore();

                sephiSprite.Draw();

                aerisSprite.Draw();

                
                if (videoPlayer.State != MediaState.Stopped)
                    videoTexture = videoPlayer.GetTexture();

                if (videoTexture != null)
                {
                    spriteBatch.Draw(videoTexture, new Rectangle(0, 0, 960, 640), Color.CornflowerBlue);
                }
                


                /*
                spriteBatch.Draw(sephiSprite.Texture, sephiSprite.Position, new Rectangle(0, 0, sephiSprite.Texture.Width, sephiSprite.Texture.Height), Color.White, sephiSprite.angle, 
                    new Vector2(sephiSprite.Texture.Width / 2, sephiSprite.Texture.Height) , 1.0f, SpriteEffects.None, 0);
                */
                /*
                spriteBatch.Draw(sephiSprite.Texture, sephiSprite.Position, new Rectangle(0, 0, sephiSprite.Texture.Width, sephiSprite.Texture.Height), Color.White, sephiSprite.angle,
                   new Vector2(sephiSprite.Texture.Width, sephiSprite.Texture.Height), 1.0f, SpriteEffects.None, 0);
                */
            }

            spriteBatch.End();

            
        }

        private void WriteScore()
        {
            scoreString = string.Format("{0}", score);
            spriteBatch.DrawString(scoreFont, scoreString, new Vector2(10, 20), Color.White);
        }

        private void WriteDebugInformation()
        {
 	        isOnFirmGroundText = string.Format("On firm ground? : {0}", sephiSprite.IsOnFirmGround());
            spriteBatch.DrawString(debugFont, isOnFirmGroundText, new Vector2(10, 40), Color.White);
            videoPlayerState = string.Format("videostate? : {0}", videoPlayer.State.ToString());
            spriteBatch.DrawString(debugFont, videoPlayerState, new Vector2(500, 40), Color.White);
        }
    }
}


 
