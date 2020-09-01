using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using System;
using System.Diagnostics;

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

        public GameLoop()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferMultiSampling = true;
            graphics.SynchronizeWithVerticalRetrace = false;
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            IsFixedTimeStep = true;
            IsMouseVisible = true;
            TargetElapsedTime = TimeSpan.FromMilliseconds(1000.0f / 60);
        }

        protected override void Initialize()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            graphicsHandler = new GraphicsHandler(GraphicsDevice, spriteBatch);
            screenCoordinator = new ScreenCoordinator(Content);

            // TODO: Add your initialization logic here
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // TODO: use this.Content to load your game content here
            cat = Content.Load<Texture2D>("Images/Cat");
            font = Content.Load<SpriteFont>("Spritefonts/Arial25");
            testFont = Content.Load<BitmapFont>("BitmapFonts/Arial_Outline");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            // TODO: Add your update logic here
            screenCoordinator.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);

            /*graphicsHandler.DrawFilledRectangle(new Rectangle(0, 0, 10, 10), Color.Black);

            graphicsHandler.DrawFilledRectangleWithBorder(new Rectangle(80, 80, 60, 60), Color.Black, Color.White, 6);

            graphicsHandler.DrawRectangle(new Rectangle(200, 200, 20, 80), Color.Blue, 1);

            graphicsHandler.DrawImage(cat, new Vector2(200, 200), sourceRectangle: new Rectangle(0, 0, 23, 23), color: Color.White, scale: new Vector2(3f, 3f));

            graphicsHandler.DrawString(font, "Cat", new Vector2(300, 300), Color.Green);

            graphicsHandler.DrawStringWithOutline(font, "Cat", new Vector2(400, 300), Color.Green, outlineColor: Color.Blue);

            graphicsHandler.DrawImage(cat, new Vector2(500, 200), sourceRectangle: new Rectangle(0, 0, 23, 23), color: Color.White, scale: new Vector2(3f, 3f));

            spriteBatch.DrawString(testFont, "TEST", new Vector2(500, 100), Color.Green);*/
            screenCoordinator.Draw(graphicsHandler);


            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
