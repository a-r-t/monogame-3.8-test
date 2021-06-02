using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using System;
using System.Diagnostics;
using System.Text;

namespace GameEngineTest.Engine
{
    public class GameLoop : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private GraphicsHandler graphicsHandler;
        private Texture2D cat;
        private SpriteFont font;
        private BitmapFont testFont;
        private ScreenCoordinator screenCoordinator;
        private ScreenManager screenManager;
        public static GraphicsDeviceManager GraphicsDeviceManager { get; private set; }
        public static ContentManager ContentManager { get; private set; }
        public static GameServiceContainer GameServiceContainer { get; private set; }
        public static int ViewportWidth { get; private set; }
        public static int ViewportHeight { get; private set; }
        public static GameWindow GameWindow { get; private set; }
        RenderTarget2D renderTarget;

        public GameLoop()
        {
            graphics = new GraphicsDeviceManager(this);
            GraphicsDeviceManager = graphics;
            
            // holding on to these just in case but they make the game choppy
            //graphics.PreferMultiSampling = true;
            //graphics.SynchronizeWithVerticalRetrace = false;

            graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            IsFixedTimeStep = true;
            IsMouseVisible = true;
            TargetElapsedTime = TimeSpan.FromMilliseconds(1000.0f / 60);
            ContentManager = Content;
            GameServiceContainer = Services;
            graphics.PreferredBackBufferWidth = 800;  // set this value to the desired width of your window
            graphics.PreferredBackBufferHeight = 605;   // set this value to the desired height of your window
            graphics.ApplyChanges();
            ViewportWidth = GraphicsDevice.Viewport.Width;
            ViewportHeight = GraphicsDevice.Viewport.Height;

            /*
            this.Window.AllowUserResizing = true;
            this.Window.ClientSizeChanged += new EventHandler<EventArgs>(Window_ClientSizeChanged);
            */
            GameWindow = this.Window;
            //this.Window.TextInput += Window_TextInput;
        }

        /*
        private void Window_TextInput(object sender, TextInputEventArgs e)
        {
            StringBuilder builder = new StringBuilder();
            if (e.Key == Keys.Back)
            {
                if (builder.Length > 0)
                {
                    builder.Remove(builder.Length - 1, 1);
                }
            }
            else
            {
                builder.Append(e.Character);
            }
            this.Window.Title = builder.ToString();
        }
        */

        /*
        void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
            graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
            graphics.ApplyChanges();
        }
        */

        protected override void Initialize()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            graphicsHandler = new GraphicsHandler(GraphicsDevice, spriteBatch);
            screenManager = new ScreenManager();
            screenManager.Initialize(new Rectangle(0, 0, 800, 605));
            screenCoordinator = new ScreenCoordinator();
            screenManager.SetCurrentScreen(screenCoordinator);

            renderTarget = new RenderTarget2D(
                GraphicsDevice,
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);

            // TODO: Add your initialization logic here
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // TODO: use this.Content to load your game content here
            cat = Content.Load<Texture2D>("Images/Cat");
            font = Content.Load<SpriteFont>("TrueTypeFonts/cat_letters");
            testFont = Content.Load<BitmapFont>("BitmapFonts/Arial_Outline");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            // TODO: Add your update logic here
            screenManager.Update(gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// Draws the entire scene in the given render target.
        /// </summary>
        /// <returns>A texture2D with the scene drawn in it.</returns>
        protected void DrawSceneToTexture(RenderTarget2D renderTarget)
        {
            // Set the render target
            GraphicsDevice.SetRenderTarget(renderTarget);

            GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };

            // Draw the scene
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);

            screenManager.Draw(graphicsHandler);
            spriteBatch.End();

            // Drop the render target
            GraphicsDevice.SetRenderTarget(null);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            DrawSceneToTexture(renderTarget);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);

            spriteBatch.Draw(renderTarget, new Rectangle(0, 0, 800, 605), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

/*
            graphicsHandler.DrawFilledRectangle(new Rectangle(0, 0, 10, 10), Color.Black);

            graphicsHandler.DrawFilledRectangleWithBorder(new Rectangle(80, 80, 60, 60), Color.Black, Color.White, 6);

            graphicsHandler.DrawRectangle(new Rectangle(200, 200, 20, 80), Color.Blue, 1);

            graphicsHandler.DrawImage(cat, new Vector2(200, 200), sourceRectangle: new Rectangle(0, 0, 23, 23), color: Color.White, scale: new Vector2(3f, 3f));

            graphicsHandler.DrawString(font, "Cat", new Vector2(300, 300), Color.Black);

            graphicsHandler.DrawString(font, "Cat", new Vector2(301, 299), Color.Black);

            graphicsHandler.DrawString(font, "Cat", new Vector2(302, 298), Color.Black);

            graphicsHandler.DrawString(font, "Cat", new Vector2(303, 297), Color.Green);

            graphicsHandler.DrawImage(cat, new Vector2(500, 200), sourceRectangle: new Rectangle(0, 0, 23, 23), color: Color.White, scale: new Vector2(3f, 3f));

            spriteBatch.DrawString(testFont, "TEST", new Vector2(500, 100), Color.Green);*/