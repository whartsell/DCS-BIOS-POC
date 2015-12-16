using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using net.willshouse.dcs.dcsbios;
using net.willshouse.dcs.iastest;


namespace net.willshouse.dcs.gauges
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private Manager manager;
        private string address = "239.255.50.10";
        private int port = 5010;
        private Needle needle;
        private GaugeFace face;
        private bool test;
        private int testValue;
        private KeyboardState prevState;
        private Color background;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
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
            manager = new Manager(address, port);
            manager.Start();
            test = false;
            testValue = 0;
            background = Color.CornflowerBlue;
            
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
            int x = GraphicsDevice.Viewport.Width / 2;
            int y = GraphicsDevice.Viewport.Height / 2;
            face = new GaugeFace(Content.Load<Texture2D>("resources/faceplate"), new Vector2(0, 0));
            needle = new Needle(Content.Load<Texture2D>("resources/needle"), new Vector2(150, 150));
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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
            int value;
            KeyboardState state = Keyboard.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || state.IsKeyDown(Keys.Escape))
                Shutdown();

           if ((state.IsKeyDown(Keys.F1)) && (prevState.IsKeyUp(Keys.F1)))
            {
                if (!test)
                {
                    manager.Stop();
                    test = true;
                    background = Color.Red;
                    
                }
                else
                {
                    manager.Start();
                    test = false;
                    background = Color.CornflowerBlue;
                }

                
            }

            prevState = state;

            // TODO: Add your update logic here
            if (test)
            {
                if (state.IsKeyDown(Keys.Right))
                    testValue +=100;
                else if (state.IsKeyDown(Keys.Left))
                    testValue -=100;
                value = testValue;
            }
            else
            {
                value = manager.getDataAtAddress(0x107a);
            }
            needle.Update(value);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(background);
            spriteBatch.Begin();
            face.Draw(spriteBatch);
            needle.Draw(spriteBatch);
            spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        protected void Shutdown()
        {
            manager.Stop();
            Exit();
        }
    }
}
