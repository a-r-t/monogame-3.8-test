using GameEngineTest.Engine;
using GameEngineTest.FontGraphics;
using GameEngineTest.Level;
using GameEngineTest.Maps;
using GameEngineTest.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngineTest.Screens
{
    public class MenuScreen : Screen
    {
        protected ScreenCoordinator screenCoordinator;
        protected int currentMenuItemHovered = 0;
        protected int menuItemSelected = -1;
        protected BitmapFontGraphic playGameText;
        protected BitmapFontGraphic creditsText;
        protected Map background;
        protected Stopwatch keyTimer = new Stopwatch();
        protected int pointerLocationX, pointerLocationY;
        protected KeyLocker keyLocker = new KeyLocker();

        public MenuScreen(ScreenCoordinator screenCoordinator)
        {
            this.screenCoordinator = screenCoordinator;
        }

        public override void Initialize()
        {
            background = new TitleScreenMap();
            background.SetAdjustCamera(false);
            keyTimer.SetWaitTime(200);
            menuItemSelected = -1;
            keyLocker.LockKey(Keys.Space);
        }

        public override void LoadContent()
        {
            BitmapFont arialOutline = ContentManager.LoadBitmapFont("BitmapFonts/Arial_Outline");
            playGameText = new BitmapFontGraphic("PLAY GAME", arialOutline, new Vector2(200, 150), new Color(49, 207, 240));
            creditsText = new BitmapFontGraphic("CREDITS", arialOutline, new Vector2(200, 250), new Color(49, 207, 240));
        }

        public override void Update(GameTime gameTime)
        {
            keyTimer.Tick(gameTime.ElapsedGameTime.TotalMilliseconds);

            // update background map (to play tile animations)
            background.Update(null);

            KeyboardState keyboardState = Keyboard.GetState();

            // if down or up is pressed, change menu item "hovered" over (blue square in front of text will move along with currentMenuItemHovered changing)
            if (keyboardState.IsKeyDown(Keys.Down) && keyTimer.IsTimeUp())
            {
                keyTimer.Reset();
                currentMenuItemHovered++;
            }
            else if (keyboardState.IsKeyDown(Keys.Up) && keyTimer.IsTimeUp())
            {
                keyTimer.Reset();
                currentMenuItemHovered--;
            }

            // if down is pressed on last menu item or up is pressed on first menu item, "loop" the selection back around to the beginning/end
            if (currentMenuItemHovered > 1)
            {
                currentMenuItemHovered = 0;
            }
            else if (currentMenuItemHovered < 0)
            {
                currentMenuItemHovered = 1;
            }

            // sets location for blue square in front of text (pointerLocation) and also sets color of spritefont text based on which menu item is being hovered
            if (currentMenuItemHovered == 0)
            {
                playGameText.Color = new Color(255, 215, 0);
                creditsText.Color = new Color(49, 207, 240);
                pointerLocationX = 170;
                pointerLocationY = 155;
            }
            else if (currentMenuItemHovered == 1)
            {
                playGameText.Color = new Color(49, 207, 240);
                creditsText.Color = new Color(255, 215, 0);
                pointerLocationX = 170;
                pointerLocationY = 255;
            }

            // if space is pressed on menu item, change to appropriate screen based on which menu item was chosen
            if (keyboardState.IsKeyUp(Keys.Space))
            {
                keyLocker.UnlockKey(Keys.Space);
            }
            if (!keyLocker.IsKeyLocked(Keys.Space) && keyboardState.IsKeyDown(Keys.Space))
            {
                menuItemSelected = currentMenuItemHovered;
                if (menuItemSelected == 0)
                {
                    screenCoordinator.GameState = GameState.LEVEL;
                }
                else if (menuItemSelected == 1)
                {
                    screenCoordinator.GameState = GameState.CREDITS;
                }
            }
        }

        public override void Draw(GraphicsHandler graphicsHandler)
        {
            background.Draw(graphicsHandler);
            playGameText.Draw(graphicsHandler);
            creditsText.Draw(graphicsHandler);
            graphicsHandler.DrawFilledRectangleWithBorder(new Rectangle(pointerLocationX, pointerLocationY, 20, 20), new Color(49, 207, 240), Color.Black, 2);
        }
    }
}
