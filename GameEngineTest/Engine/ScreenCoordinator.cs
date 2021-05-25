using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using GameEngineTest.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace GameEngineTest.Engine
{
    public class ScreenCoordinator
    {
        private Screen currentScreen;

        public GameState GameState;
        private GameState previousGameState;

        public ScreenCoordinator()
        {
            GameState = GameState.MENU;
            previousGameState = GameState;
            UpdateCurrentScreen();
        }

        public void Update(GameTime gameTime)
        {
            do
            {
                if (previousGameState != GameState)
                {
                    currentScreen.UnloadContent();
                    UpdateCurrentScreen();
                }
                previousGameState = GameState;

                currentScreen.Update(gameTime);
            } while (previousGameState != GameState);
        }

        private void UpdateCurrentScreen()
        {
            switch (GameState)
            {
                case GameState.MENU:
                    currentScreen = new MenuScreen(this);
                    break;
                case GameState.LEVEL:
                    //currentScreen = new PlayLevelScreen(this);
                    break;
                case GameState.CREDITS:
                    currentScreen = new CreditsScreen(this);
                    break;
            }
            currentScreen.Initialize();
            currentScreen.LoadContent();
        }

        public void Draw(GraphicsHandler graphicsHandler)
        {
            currentScreen.Draw(graphicsHandler);
        }
    }
}
