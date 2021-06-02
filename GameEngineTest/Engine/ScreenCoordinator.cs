using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using GameEngineTest.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace GameEngineTest.Engine
{
    public class ScreenCoordinator : Screen
    {
        private Screen currentScreen = new DefaultScreen();

        public GameState GameState { get; set; }
        protected GameState previousGameState;

        public ScreenCoordinator()
        {
            GameState = GameState.MENU;
            previousGameState = GameState;
            UpdateCurrentScreen();
        }

        public override void Initialize()
        {
            // start game off with Menu Screen
            GameState = GameState.SETTINGS;
        }

        public override void Update(GameTime gameTime)
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
                    currentScreen = new PlayLevelScreen(this);
                    break;
                case GameState.CREDITS:
                    currentScreen = new CreditsScreen(this);
                    break;
                case GameState.SETTINGS:
                    currentScreen = new SettingsScreen(this);
                    break;
            }
            currentScreen.Initialize();
            currentScreen.LoadContent();
        }

        public override void Draw(GraphicsHandler graphicsHandler)
        {
            currentScreen.Draw(graphicsHandler);
        }
    }
}
