﻿using GameEngineTest.Engine;
using GameEngineTest.Extensions;
using GameEngineTest.FontGraphics;
using GameEngineTest.GameObjects;
using GameEngineTest.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Stopwatch = GameEngineTest.Utils.Stopwatch;

namespace GameEngineTest.Components
{
    public class TextBox
    {
        public string Text { get; set; }
        public int CharacterLimit { get; set; }
        protected RectangleGraphic box;
        protected SpriteFont font;
        protected int cursorPosition = 0;
        protected Stopwatch cursorBlinkTimer;
        protected bool showCursor = true;
        protected int spacingBetweenLetters;
        protected Stopwatch cursorChangeTimer;
        protected KeyLocker keyLocker = new KeyLocker();
        protected int scrollIndex;
        protected bool isMouseDrag;
        protected int previousMouseX;

        protected int ScrollOffset
        {
            get
            {
                return scrollIndex * spacingBetweenLetters;
            }
        }

        protected Rectangle Bounds
        {
            get
            {
                return new Rectangle((int)box.X, (int)box.Y, box.Width, box.Height);
            }
        }

        protected int StartLocation
        {
            get
            {
                return (int)box.X + box.BorderThickness + 2;
            }
        }

        protected int EndLocation
        {
            get
            {
                return (int)box.X + box.Width - box.BorderThickness - 2;
            }
        }

        public TextBox(int x, int y, int width, SpriteFont spriteFont, string defaultText = "", int characterLimit = -1)
        {
            box = new RectangleGraphic(x, y, width, spriteFont.LineSpacing);
            box.Color = Color.White;
            box.BorderColor = Color.Black;
            box.BorderThickness = 2;
            Text = defaultText;
            font = spriteFont;
            
            cursorBlinkTimer = new Stopwatch();
            cursorBlinkTimer.SetWaitTime(500);

            cursorChangeTimer = new Stopwatch();

            spacingBetweenLetters = (int)spriteFont.MeasureString("a").X;

            CharacterLimit = characterLimit;

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

            if (mouseState.LeftButton == ButtonState.Pressed && box.ContainsPoint(mouseLocation) && cursorChangeTimer.IsTimeUp() && !isMouseDrag)
            {
                OnMouseClick(mouseState, mouseLocation);
            }
            else if (mouseState.LeftButton == ButtonState.Released)
            {
                isMouseDrag = false;
            }

            if (isMouseDrag && cursorChangeTimer.IsTimeUp())
            {
                if (mouseState.X != previousMouseX)
                {
                    OnMouseDrag(mouseState, mouseLocation);
                }
            }

            KeyboardState keyboardState = Keyboard.GetState();
            OnKeyPress(keyboardState);
            
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

            UpdateTextScroll();
        }

        protected virtual void OnMouseClick(MouseState mouseState, Vector2 mouseLocation)
        {
            float mouseLocationOffset = (mouseLocation.X - StartLocation + ScrollOffset) / spacingBetweenLetters;
            int calculatedCursorIndex = Math.Min(mouseLocationOffset.Round(), Text.Length);
            if (StartLocation + (calculatedCursorIndex * spacingBetweenLetters) - ScrollOffset <= EndLocation)
            {
                cursorPosition = Math.Min(mouseLocationOffset.Round(), Text.Length);
                cursorBlinkTimer.Reset();
                showCursor = true;

                cursorChangeTimer.SetWaitTime(100);

                isMouseDrag = true;
                previousMouseX = mouseState.X;
            }
        }

        protected virtual void OnMouseDrag(MouseState mouseState, Vector2 mouseLocation)
        {
            int direction = mouseState.X - previousMouseX > 0 ? 1 : -1;

            if (direction == -1 && cursorPosition > 0)
            {
                float mouseLocationOffset = (mouseLocation.X - StartLocation + ScrollOffset) / spacingBetweenLetters;
                cursorPosition = Math.Min(mouseLocationOffset.Round(), Text.Length);
                if (cursorPosition < 0)
                {
                    cursorPosition = 0;
                }
                else if (cursorPosition > Text.Length)
                {
                    cursorPosition = Text.Length;
                }
                cursorChangeTimer.SetWaitTime(100);
                showCursor = true;
            }
            else if (direction == 1 && cursorPosition < Text.Length)
            {
                float mouseLocationOffset = (mouseLocation.X - StartLocation + ScrollOffset) / spacingBetweenLetters;
                cursorPosition = Math.Min(mouseLocationOffset.Round(), Text.Length);
                if (cursorPosition > Text.Length)
                {
                    cursorPosition = Text.Length;
                }
                else if (cursorPosition < 0)
                {
                    cursorPosition = 0;
                }
                cursorChangeTimer.SetWaitTime(100);
                showCursor = true;
            }
            previousMouseX = mouseState.X;
        }

        protected virtual void OnKeyPress(KeyboardState keyboardState)
        {
            if (keyboardState.IsKeyDown(Keys.Left) && cursorPosition > 0 && !keyLocker.IsKeyLocked(Keys.Left))
            {
                cursorPosition--;
                cursorBlinkTimer.Reset();
                showCursor = true;

                keyLocker.LockKey(Keys.Left);
                keyLocker.UnlockKey(Keys.Right);
                cursorChangeTimer.SetWaitTime(100);
            }
            else if (keyboardState.IsKeyDown(Keys.Right) && cursorPosition < Text.Length && !keyLocker.IsKeyLocked(Keys.Right))
            {
                cursorPosition++;
                cursorBlinkTimer.Reset();
                showCursor = true;

                keyLocker.LockKey(Keys.Right);
                keyLocker.UnlockKey(Keys.Left);
                cursorChangeTimer.SetWaitTime(100);
            }
        }

        protected virtual void UpdateTextScroll()
        {
            // if cursor position is off screen
            if (StartLocation + (cursorPosition * spacingBetweenLetters) - ScrollOffset > EndLocation)
            {
                scrollIndex++;
            }
            else if (StartLocation + (cursorPosition * spacingBetweenLetters) - ScrollOffset < StartLocation)
            {
                scrollIndex--;
            }
        }

        public virtual void Draw(GraphicsHandler graphicsHandler)
        {
            box.Draw(graphicsHandler);

            graphicsHandler.SetScissorRectangle(Bounds);
            graphicsHandler.DrawString(font, Text, new Vector2(StartLocation - ScrollOffset, box.Y), color: Color.Black);

            if (showCursor)
            {
                graphicsHandler.DrawLine(new Vector2(StartLocation + (cursorPosition * spacingBetweenLetters) - ScrollOffset, box.Y + box.BorderThickness + 2), new Vector2(StartLocation + (cursorPosition * spacingBetweenLetters) - ScrollOffset, box.Y + box.Height - box.BorderThickness - 2), Color.Black);
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
                    cursorBlinkTimer.Reset();
                    showCursor = true;
                }
            }
            else if (CharacterLimit < 0 || Text.Length < CharacterLimit)
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
                cursorPosition++;
                cursorBlinkTimer.Reset();
                showCursor = true;
            }
        }
    }
}
