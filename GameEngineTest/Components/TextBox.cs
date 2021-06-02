using GameEngineTest.Engine;
using GameEngineTest.FontGraphics;
using GameEngineTest.GameObject;
using GameEngineTest.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Rectangle = GameEngineTest.GameObject.Rectangle;
using Stopwatch = GameEngineTest.Utils.Stopwatch;

namespace GameEngineTest.Components
{
    public class TextBox
    {
        protected Rectangle box;
        protected SpriteFont font;
        protected string text;
        protected int cursorPosition = 0;
        private Stopwatch cursorBlinkTimer;
        private bool showCursor = true;
        private int spacingBetweenLetters;
        public Microsoft.Xna.Framework.Rectangle Bounds
        {
            get
            {
                return new Microsoft.Xna.Framework.Rectangle((int)box.X, (int)box.Y, box.Width, box.Height);
            }
        }

        public TextBox(int x, int y, int width, SpriteFont spriteFont, string defaultText = "")
        {
            box = new Rectangle(x, y, width, spriteFont.LineSpacing);
            box.Color = Color.White;
            box.BorderColor = Color.Black;
            box.BorderThickness = 2;
            text = defaultText;
            font = spriteFont;
            cursorBlinkTimer = new Stopwatch();
            cursorBlinkTimer.SetWaitTime(500);
            spacingBetweenLetters = (int)spriteFont.MeasureString("a").X;
            GameLoop.GameWindow.TextInput += Window_TextInput;
        }

        ~TextBox()
        {
            GameLoop.GameWindow.TextInput -= Window_TextInput;
        }

        public virtual void Update()
        {
            if (cursorBlinkTimer.IsTimeUp())
            {
                showCursor = !showCursor;
                cursorBlinkTimer.Reset();
            }
        }


        public virtual void Draw(GraphicsHandler graphicsHandler)
        {
            box.Draw(graphicsHandler);

            graphicsHandler.SetScissorRectangle(Bounds);
            graphicsHandler.DrawString(font, text, new Vector2(box.X + box.BorderThickness + 2, box.Y), color: Color.Black);

            if (showCursor)
            {
                Debug.WriteLine(font.Spacing);
                graphicsHandler.DrawLine(new Vector2(box.X + box.BorderThickness + 2 + (cursorPosition * spacingBetweenLetters), box.Y + box.BorderThickness + 2), new Vector2(box.X + box.BorderThickness + 2 + (cursorPosition * spacingBetweenLetters), box.Y + box.Height - box.BorderThickness - 2), Color.Black);
            }

            graphicsHandler.RemoveScissorRectangle();

        }

        // event for handling keyboard input from OS
        private void Window_TextInput(object sender, TextInputEventArgs e)
        {
            if (e.Key == Keys.Back)
            {
                if (text.Length > 0)
                {
                    text = text.Substring(0, text.Length - 1);
                    cursorPosition--;
                }
            }
            else
            {
                text += e.Character;
                cursorPosition++;
            }
        }
    }
}
