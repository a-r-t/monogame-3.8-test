using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

/*
 * The game engine uses this class to start off the cascading Screen updating/drawing
 * The idea is an external class should be allowed to set its own Screen to this class's currentScreen variable,
 * and then that class can handle coordinating which Screen to show.
 */
namespace GameEngineTest.Engine
{
    public class ScreenManager
    {
        private Screen currentScreen;
        private static Rectangle screenBounds = new Rectangle(0, 0, 0, 0);

        public void Initialize(Rectangle screenBounds)
        {
            ScreenManager.screenBounds = screenBounds;
            SetCurrentScreen(new DefaultScreen());
        }

        // attach an external Screen class here for the ScreenManager to start calling its update/draw cycles
        public void SetCurrentScreen(Screen screen)
        {
            screen.Initialize();
            this.currentScreen = screen;
        }

        public void Update(GameTime gameTime)
        {
            currentScreen.Update(gameTime);
        }

        public void Draw(GraphicsHandler graphicsHandler)
        {
            currentScreen.Draw(graphicsHandler);
        }

        // gets width of currentScreen -- can be called from anywhere in an application
        public static int GetScreenWidth()
        {
            return screenBounds.Width;
        }

        // gets height of currentScreen -- can be called from anywhere in an application
        public static int GetScreenHeight()
        {
            return screenBounds.Height;
        }

        // gets bounds of currentScreen -- can be called from anywhere in an application
        public static Rectangle GetScreenBounds()
        {
            return screenBounds;
        }
    }
}
