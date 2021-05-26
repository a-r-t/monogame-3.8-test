using GameEngineTest.Engine;
using GameEngineTest.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngineTest.Screens
{
    public class PlayLevelScreen : Screen
    {
        protected ScreenCoordinator screenCoordinator;
        protected Map map;
        protected Player player;
        protected PlayLevelScreenState playLevelScreenState;
        protected Stopwatch screenTimer = new Stopwatch();
        protected LevelClearedScreen levelClearedScreen;
        protected LevelLoseScreen levelLoseScreen;

        public PlayLevelScreen(ScreenCoordinator screenCoordinator)
        {
            this.screenCoordinator = screenCoordinator;
        }

        public override void Initialize()
        {
            // define/setup map

            
            this.map = new TestMap();
            map.reset();

            // setup player
            this.player = new Cat(map.getPlayerStartPosition().x, map.getPlayerStartPosition().y);
            this.player.setMap(map);
            this.player.addListener(this);
            this.player.setLocation(map.getPlayerStartPosition().x, map.getPlayerStartPosition().y);
            this.playLevelScreenState = PlayLevelScreenState.RUNNING;        
        }

        public override void Update(GameTime gameTime)
        {
            // based on screen state, perform specific actions
            switch (playLevelScreenState)
            {
                // if level is "running" update player and map to keep game logic for the platformer level going
                case PlayLevelScreenState.RUNNING:
                    player.update();
                    map.update(player);
                    break;
                // if level has been completed, bring up level cleared screen
                case PlayLevelScreenState.LEVEL_COMPLETED:
                    levelClearedScreen = new LevelClearedScreen();
                    levelClearedScreen.initialize();
                    screenTimer.SetWaitTime(2500);
                    playLevelScreenState = PlayLevelScreenState.LEVEL_WIN_MESSAGE;
                    break;
                // if level cleared screen is up and the timer is up for how long it should stay out, go back to main menu
                case PlayLevelScreenState.LEVEL_WIN_MESSAGE:
                    if (screenTimer.IsTimeUp())
                    {
                        levelClearedScreen = null;
                        GoBackToMenu();
                    }
                    break;
                // if player died in level, bring up level lost screen
                case PlayLevelScreenState.PLAYER_DEAD:
                    levelLoseScreen = new LevelLoseScreen(this);
                    levelLoseScreen.initialize();
                    playLevelScreenState = PlayLevelScreenState.LEVEL_LOSE_MESSAGE;
                    break;
                // wait on level lose screen to make a decision (either resets level or sends player back to main menu)
                case PlayLevelScreenState.LEVEL_LOSE_MESSAGE:
                    levelLoseScreen.update();
                    break;
            }
        }

        public override void Draw(GraphicsHandler graphicsHandler)
        {
            // based on screen state, draw appropriate graphics
            switch (playLevelScreenState)
            {
                case PlayLevelScreenState.RUNNING:
                case PlayLevelScreenState.LEVEL_COMPLETED:
                case PlayLevelScreenState.PLAYER_DEAD:
                    map.draw(graphicsHandler);
                    player.draw(graphicsHandler);
                    break;
                case PlayLevelScreenState.LEVEL_WIN_MESSAGE:
                    levelClearedScreen.draw(graphicsHandler);
                    break;
                case PlayLevelScreenState.LEVEL_LOSE_MESSAGE:
                    levelLoseScreen.draw(graphicsHandler);
                    break;
            }
        }

        public void OnLevelCompleted()
        {
            playLevelScreenState = PlayLevelScreenState.LEVEL_COMPLETED;
        }

        public void OnDeath()
        {
            playLevelScreenState = PlayLevelScreenState.PLAYER_DEAD;
        }

        public void ResetLevel()
        {
            Initialize();
        }

        public void GoBackToMenu()
        {
            screenCoordinator.GameState = GameState.MENU;
        }
    }
}
