using GameEngineTest.Engine;
using GameEngineTest.FontGraphics;
using GameEngineTest.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

// This is the class for the level lose screen
namespace GameEngineTest.Screens
{
    public class LevelLoseScreen : Screen
    {
        protected SpriteFontGraphic loseMessage;
        protected SpriteFontGraphic instructions;
        protected KeyLocker keyLocker = new KeyLocker();
        protected PlayLevelScreen playLevelScreen;

        public LevelLoseScreen(PlayLevelScreen playLevelScreen)
        {
            this.playLevelScreen = playLevelScreen;
        }

        public override void Initialize()
        {
            SpriteFont comicSans20 = ContentManager.LoadSpriteFont("SpriteFonts/ComicSans20");
            SpriteFont comicSans30 = ContentManager.LoadSpriteFont("SpriteFonts/ComicSans30");
            loseMessage = new SpriteFontGraphic("You lose!", comicSans30, new Vector2(350, 270), Color.White);
            instructions = new SpriteFontGraphic("Press Space to try again or Escape to go back to the main menu", comicSans20, new Vector2(120, 300), Color.White);
            keyLocker.LockKey(Keys.Space);
            keyLocker.LockKey(Keys.Escape);
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyUp(Keys.Space))
            {
                keyLocker.UnlockKey(Keys.Space);
            }
            if (keyboardState.IsKeyUp(Keys.Escape))
            {
                keyLocker.UnlockKey(Keys.Escape);
            }

            // if space is pressed, reset level. if escape is pressed, go back to main menu
            if (keyboardState.IsKeyDown(Keys.Space))
            {
                playLevelScreen.ResetLevel();
            }
            else if (keyboardState.IsKeyDown(Keys.Escape))
            {
                playLevelScreen.GoBackToMenu();
            }
        }

        public override void Draw(GraphicsHandler graphicsHandler)
        {
            graphicsHandler.DrawFilledRectangle(0, 0, ScreenManager.GetScreenWidth(), ScreenManager.GetScreenHeight(), Color.Black);
            loseMessage.Draw(graphicsHandler);
            instructions.Draw(graphicsHandler);
        }
    }
}
