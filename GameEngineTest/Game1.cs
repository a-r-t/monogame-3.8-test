using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace GameEngineTest
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private GraphicsHandler graphicsHandler;
        private Texture2D cat;
        private SpriteFont font;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            IsFixedTimeStep = true;
        }

        protected override void Initialize()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            graphicsHandler = new GraphicsHandler(GraphicsDevice, spriteBatch);

            // TODO: Add your initialization logic here
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // TODO: use this.Content to load your game content here
            cat = Content.Load<Texture2D>("Images/Cat");
            font = Content.Load<SpriteFont>("Spritefonts/Arial12");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) 
            {
                Exit();
            }

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);

            graphicsHandler.DrawFilledRectangle(new Rectangle(0, 0, 10, 10), Color.Black);

            graphicsHandler.DrawFilledRectangleWithBorder(new Rectangle(80, 80, 60, 60), Color.Black, Color.White, 6);

            graphicsHandler.DrawRectangle(new Rectangle(200, 200, 20, 80), Color.Blue, 1);

            graphicsHandler.DrawImage(cat, new Vector2(200, 200), sourceRectangle: new Rectangle(0, 0, 23, 23), color: Color.White, scale: new Vector2(3f, 3f));

            graphicsHandler.DrawString(font, "Cat", new Vector2(300, 300), Color.Green);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
