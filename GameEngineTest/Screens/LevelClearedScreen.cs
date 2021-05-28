using GameEngineTest.Engine;
using GameEngineTest.FontGraphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngineTest.Screens
{
    public class LevelClearedScreen : Screen
    {
        protected SpriteFontGraphic winMessage;

        public LevelClearedScreen()
        {
        }

        public override void Initialize()
        {
            SpriteFont comicSans30 = ContentManager.LoadSpriteFont("SpriteFonts/ComicSans30");
            winMessage = new SpriteFontGraphic("Level Cleared", comicSans30, new Vector2(320, 270), Color.White);
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(GraphicsHandler graphicsHandler)
        {
            // paint entire screen black and dislpay level cleared text
            graphicsHandler.DrawFilledRectangle(0, 0, ScreenManager.GetScreenWidth(), ScreenManager.GetScreenHeight(), Color.Black);
            winMessage.Draw(graphicsHandler);
        }
    }
}
