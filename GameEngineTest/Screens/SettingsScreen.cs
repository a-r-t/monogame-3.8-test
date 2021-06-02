using GameEngineTest.Components;
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
    public class SettingsScreen : Screen
    {
        protected ScreenCoordinator screenCoordinator;
        protected KeyLocker keyLocker = new KeyLocker();
        protected TextBox textBox;
        protected SpriteFont textBoxFont;

        public SettingsScreen(ScreenCoordinator screenCoordinator)
        {
            this.screenCoordinator = screenCoordinator;
        }

        public override void Initialize()
        {
            textBoxFont = ContentManager.LoadSpriteFont("SpriteFonts/Roboto20");
            textBox = new TextBox(10, 10, 80, textBoxFont, defaultText: "", characterLimit: -1);
        }

        public override void LoadContent()
        {

        }

        public override void Update(GameTime gameTime)
        {
            textBox.Update();
        }

        public override void Draw(GraphicsHandler graphicsHandler)
        {
            textBox.Draw(graphicsHandler);
        }
    }
}
