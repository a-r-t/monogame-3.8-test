using GameEngineTest.Engine;
using GameEngineTest.FontGraphics;
using GameEngineTest.Level;
using GameEngineTest.Maps;
using GameEngineTest.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngineTest.Screens
{
    public class CreditsScreen : Screen
    {
        protected ScreenCoordinator screenCoordinator;
        protected Map background;
        protected KeyLocker keyLocker = new KeyLocker();
        protected SpriteFontGraphic creditsLabel;
        protected SpriteFontGraphic createdByLabel;
        protected SpriteFontGraphic contributorsLabel;
        protected SpriteFontGraphic returnInstructionsLabel;

        public CreditsScreen(ScreenCoordinator screenCoordinator)
        {
            this.screenCoordinator = screenCoordinator;
        }

        public override void Initialize()
        {
            // setup graphics on screen (background map, spritefont text)
            background = new TitleScreenMap();
            background.SetAdjustCamera(false);

            keyLocker.LockKey(Keys.Space);
        }

        public override void LoadContent()
        {
            SpriteFont timesNewRoman20 = ContentManager.LoadSpriteFont("SpriteFonts/TimesNewRoman20");
            SpriteFont timesNewRoman30 = ContentManager.LoadSpriteFont("SpriteFonts/TimesNewRoman30");
            creditsLabel = new SpriteFontGraphic("Credits", timesNewRoman30, new Vector2(15, 35), Color.White);
            createdByLabel = new SpriteFontGraphic("Created by Alex Thimineur for Quinnipiac's SER225 Course.", timesNewRoman20, new Vector2(130, 140), Color.White);
            contributorsLabel = new SpriteFontGraphic("Thank you to QU Alumni Brian Carducci, Joseph White,\nand Alex Hutman for their contributions.", timesNewRoman20, new Vector2(60, 220), Color.White);
            returnInstructionsLabel = new SpriteFontGraphic("Press Space to return to the menu", timesNewRoman30, new Vector2(20, 560), Color.White);
        }

        public override void Update(GameTime gameTime)
        {
            //background.update(null);

            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyUp(Keys.Space))
            {
                keyLocker.UnlockKey(Keys.Space);
            }

            // if space is pressed, go back to main menu
            if (!keyLocker.IsKeyLocked(Keys.Space) && keyboardState.IsKeyDown(Keys.Space))
            {
                screenCoordinator.GameState = GameState.MENU;
            }
        }

        public override void Draw(GraphicsHandler graphicsHandler)
        {
            background.Draw(graphicsHandler);
            creditsLabel.Draw(graphicsHandler);
            createdByLabel.Draw(graphicsHandler);
            contributorsLabel.Draw(graphicsHandler);
            returnInstructionsLabel.Draw(graphicsHandler);
        }
    }
}
