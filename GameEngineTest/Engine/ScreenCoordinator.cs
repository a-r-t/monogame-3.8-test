using System;
using System.Collections.Generic;
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
        private ContentManager contentManager;

        public ScreenCoordinator(ContentManager contentManager)
        {
            this.contentManager = contentManager;
            GameState = GameState.MENU;
        }

        public void Update(GameTime gameTime)
        {
            do
            {
                if (previousGameState != GameState || currentScreen == null)
                {
                    switch (GameState)
                    {
                        case GameState.MENU:
                            currentScreen = new MenuScreen(contentManager, this);
                            break;
                        case GameState.LEVEL:
                            //currentScreen = new PlayLevelScreen(this);
                            break;
                        case GameState.CREDITS:
                            //currentScreen = new CreditsScreen(this);
                            break;
                    }
                    currentScreen.Initialize();
                }
                previousGameState = GameState;

                currentScreen.Update(gameTime);
            } while (previousGameState != GameState);
        }

        public void Draw(GraphicsHandler graphicsHandler)
        {
            currentScreen.Draw(graphicsHandler);
        }
    }
}
