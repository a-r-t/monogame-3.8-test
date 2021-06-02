using GameEngineTest.Engine;
using GameEngineTest.Extensions;
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
        public string Text { get; set; }
        protected int cursorPosition = 0;
        private Stopwatch cursorBlinkTimer;
        private bool showCursor = true;
        private int spacingBetweenLetters;
        private Stopwatch cursorChangeTimer;
        private KeyLocker keyLocker = new KeyLocker();

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
            Text = defaultText;
            font = spriteFont;
            
            cursorBlinkTimer = new Stopwatch();
            cursorBlinkTimer.SetWaitTime(500);

            cursorChangeTimer = new Stopwatch();

            spacingBetweenLetters = (int)spriteFont.MeasureString("a").X;

            GameLoop.GameWindow.TextInput += Window_TextInput;
        }

        ~TextBox()
        {
            GameLoop.GameWindow.TextInput -= Window_TextInput;
        }

        public virtual void Update()
        {
            MouseState mouseState = Mouse.GetState();
            Vector2 mouseLocation = new Vector2(mouseState.X, mouseState.Y);
            if (mouseState.LeftButton == ButtonState.Pressed && box.ContainsPoint(mouseLocation))
            {
                float mouseLocationOffset = (mouseLocation.X - (box.X + box.BorderThickness + 2)) / spacingBetweenLetters;

                cursorPosition = Math.Min(mouseLocationOffset.Round(), Text.Length);
                cursorBlinkTimer.Reset();
                showCursor = true;
            }

            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Left) && cursorPosition > 0 && !keyLocker.IsKeyLocked(Keys.Left)) {
                cursorPosition--;
                cursorBlinkTimer.Reset();
                showCursor = true;
                keyLocker.LockKey(Keys.Left);
                keyLocker.UnlockKey(Keys.Right);
                cursorChangeTimer.SetWaitTime(150);
            }
            else if (keyboardState.IsKeyDown(Keys.Right) && cursorPosition < Text.Length && !keyLocker.IsKeyLocked(Keys.Right))
            {
                cursorPosition++;
                cursorBlinkTimer.Reset();
                showCursor = true;
                keyLocker.LockKey(Keys.Right);
                keyLocker.UnlockKey(Keys.Left);
                cursorChangeTimer.SetWaitTime(150);
            }

            if (cursorBlinkTimer.IsTimeUp())
            {
                showCursor = !showCursor;
                cursorBlinkTimer.Reset();
            }

            if (cursorChangeTimer.IsTimeUp())
            {
                keyLocker.UnlockKey(Keys.Left);
                keyLocker.UnlockKey(Keys.Right);
            }


        }


        public virtual void Draw(GraphicsHandler graphicsHandler)
        {
            box.Draw(graphicsHandler);

            graphicsHandler.SetScissorRectangle(Bounds);
            graphicsHandler.DrawString(font, Text, new Vector2(box.X + box.BorderThickness + 2, box.Y), color: Color.Black);

            if (showCursor)
            {
                graphicsHandler.DrawLine(new Vector2(box.X + box.BorderThickness + 2 + (cursorPosition * spacingBetweenLetters), box.Y + box.BorderThickness + 2), new Vector2(box.X + box.BorderThickness + 2 + (cursorPosition * spacingBetweenLetters), box.Y + box.Height - box.BorderThickness - 2), Color.Black);
            }

            graphicsHandler.RemoveScissorRectangle();

        }

        // event for handling keyboard input from OS
        private void Window_TextInput(object sender, TextInputEventArgs e)
        {
            if (e.Key == Keys.Back)
            {
                if (Text.Length > 0 && cursorPosition > 0)
                {
                    if (cursorPosition == Text.Length)
                    {
                        Text = Text.SubstringByIndexes(0, Text.Length - 1);
                    }
                    else
                    {
                        Text = Text.SubstringByIndexes(0, cursorPosition - 1) + Text.SubstringByIndexes(cursorPosition, Text.Length);
                    }
                    cursorPosition--;
                }
            }
            else
            {
                if (cursorPosition == Text.Length)
                {
                    Text += e.Character;
                }
                else if (cursorPosition == 0)
                {
                    Text = e.Character + Text;
                }
                else
                {
                    Text = Text.SubstringByIndexes(0, cursorPosition) + e.Character + Text.SubstringByIndexes(cursorPosition, Text.Length);
                }
                //Text += e.Character;
                cursorPosition++;
            }
        }
    }
}
