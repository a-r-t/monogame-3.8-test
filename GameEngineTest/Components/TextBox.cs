using GameEngineTest.Engine;
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

        protected SpriteFont font;
        protected RectangleGraphic box;
        protected Stopwatch cursorBlinkTimer;
        protected bool showCursor = true;
        protected int spacingBetweenLetters;
        protected Stopwatch cursorChangeTimer;
        protected KeyLocker keyLocker = new KeyLocker();
        protected int scrollIndex;
        protected bool isMouseDrag;
        protected int previousMouseX;
        protected int highlightCursorIndex;
        protected int highlightStartIndex;
        protected int highlightEndIndex;
        protected bool disableCursor = false;
        
        private int cursorPosition = 0;
        protected int CursorPosition
        {
            get
            {
                return cursorPosition;
            }
            set
            {
                cursorPosition = value;
                if (cursorPosition > Text.Length)
                {
                    cursorPosition = Text.Length;
                }
                else if (cursorPosition < 0)
                {
                    cursorPosition = 0;
                }
            }
        }

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

        protected int StartLocationX
        {
            get
            {
                return (int)box.X + box.BorderThickness + 2;
            }
        }

        protected int EndLocationX
        {
            get
            {
                return (int)box.X + box.Width - box.BorderThickness - 2;
            }
        }

        protected int StartLocationY
        {
            get
            {
                return (int)box.Y + box.BorderThickness + 2;
            }
        }

        protected int EndLocationY
        {
            get
            {
                return (int)box.Y + box.Height - box.BorderThickness - 2;
            }
        }

        protected int CursorOffset
        {
            get
            {
                return CursorPosition * spacingBetweenLetters;
            }
        }


        public Color CursorColor { get; set; }
        public Color TextColor { get; set; }
        public Color HighlightColor { get; set; }
        public Color HighlightTextColor { get; set; }
        public Color BackColor
        {
            get
            {
                return box.Color;
            }
            set
            {
                box.Color = value;
            }
        }
        public Color BorderColor
        {
            get
            {
                return box.BorderColor;
            }
            set
            {
                box.BorderColor = value;
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

            CursorColor = Color.Black;
            BackColor = Color.White;
            BorderColor = Color.Black;
            TextColor = Color.Black;
            HighlightColor = new Color(50, 151, 253); // blue
            HighlightTextColor = Color.White;
            
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
            float mouseLocationOffset = (mouseLocation.X - StartLocationX + ScrollOffset) / spacingBetweenLetters;
            int calculatedCursorIndex = Math.Min(mouseLocationOffset.Round(), Text.Length);
            if (StartLocationX + (calculatedCursorIndex * spacingBetweenLetters) - ScrollOffset <= EndLocationX)
            {
                CursorPosition = Math.Min(mouseLocationOffset.Round(), Text.Length);
                cursorBlinkTimer.Reset();
                showCursor = true;

                cursorChangeTimer.SetWaitTime(100);

                isMouseDrag = true;
                previousMouseX = mouseState.X;

                highlightCursorIndex = CursorPosition;
                highlightStartIndex = CursorPosition;
                highlightEndIndex = CursorPosition;
                disableCursor = false;
            }
        }

        protected virtual void OnMouseDrag(MouseState mouseState, Vector2 mouseLocation)
        {
            int direction = mouseState.X - previousMouseX > 0 ? 1 : -1;

            if (direction == -1)
            {
                if (CursorPosition > 0)
                {

                    float mouseLocationOffset = (mouseLocation.X - StartLocationX + ScrollOffset) / spacingBetweenLetters;
                    CursorPosition = Math.Min(mouseLocationOffset.Round(), Text.Length);
                }
                if (CursorPosition < highlightCursorIndex)
                {
                    highlightStartIndex = CursorPosition;
                }
                else if (CursorPosition > highlightCursorIndex)
                {
                    highlightEndIndex = CursorPosition;
                }
                else
                {
                    highlightStartIndex = CursorPosition;
                    highlightEndIndex = CursorPosition;
                }
            }
            else if (direction == 1)
            {
                if (CursorPosition < Text.Length)
                {
                    float mouseLocationOffset = (mouseLocation.X - StartLocationX + ScrollOffset) / spacingBetweenLetters;
                    CursorPosition = Math.Min(mouseLocationOffset.Round(), Text.Length);
                }
                if (CursorPosition > highlightCursorIndex)
                {
                    highlightEndIndex = CursorPosition;
                }
                else if (CursorPosition < highlightCursorIndex)
                {
                    highlightStartIndex = CursorPosition;
                }
                else
                {
                    highlightStartIndex = CursorPosition;
                    highlightEndIndex = CursorPosition;
                }
            }

            disableCursor = highlightStartIndex != highlightEndIndex;
            if (!disableCursor)
            {
                cursorChangeTimer.SetWaitTime(100);
                showCursor = true;
            }

            previousMouseX = mouseState.X;
        }

        protected virtual void OnKeyPress(KeyboardState keyboardState)
        {
            if (keyboardState.IsKeyDown(Keys.Left) && CursorPosition > 0 && !keyLocker.IsKeyLocked(Keys.Left))
            {
                CursorPosition--;
                cursorBlinkTimer.Reset();
                showCursor = true;

                keyLocker.LockKey(Keys.Left);
                keyLocker.UnlockKey(Keys.Right);
                cursorChangeTimer.SetWaitTime(100);
            }
            else if (keyboardState.IsKeyDown(Keys.Right) && CursorPosition < Text.Length && !keyLocker.IsKeyLocked(Keys.Right))
            {
                CursorPosition++;
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
            if (StartLocationX + CursorOffset - ScrollOffset > EndLocationX)
            {
                scrollIndex++;
            }
            else if (StartLocationX + CursorOffset - ScrollOffset < StartLocationX)
            {
                scrollIndex--;
            }
        }

        public virtual void Draw(GraphicsHandler graphicsHandler)
        {
            box.Draw(graphicsHandler);

            graphicsHandler.SetScissorRectangle(Bounds);

            // highlighting
            graphicsHandler.DrawFilledRectangle(new Rectangle(StartLocationX + (highlightStartIndex * spacingBetweenLetters) - ScrollOffset, StartLocationY, ((highlightEndIndex - highlightStartIndex)  * spacingBetweenLetters), EndLocationY - StartLocationY), HighlightColor);

            // text
            graphicsHandler.DrawString(font, Text, new Vector2(StartLocationX - ScrollOffset, box.Y), color: TextColor);

            // cursor
            if (showCursor && !disableCursor)
            {
                graphicsHandler.DrawLine(new Vector2(StartLocationX + CursorOffset - ScrollOffset, StartLocationY), new Vector2(StartLocationX + CursorOffset - ScrollOffset, EndLocationY), CursorColor);
            }

            graphicsHandler.RemoveScissorRectangle();

            if (highlightStartIndex != highlightEndIndex)
            {
                int highlightX = StartLocationX + (highlightStartIndex * spacingBetweenLetters) - ScrollOffset;
                int highlightWidth = ((highlightEndIndex - highlightStartIndex) * spacingBetweenLetters);
                if (highlightX + highlightWidth > box.X + box.Width)
                {
                    highlightWidth = (int)box.X + box.Width - highlightX;
                }
                if (highlightX < box.X)
                {
                    int difference = (int)box.X - highlightX;
                    highlightX = (int)box.X;
                    highlightWidth -= difference;
                }
                graphicsHandler.SetScissorRectangle(new Rectangle(highlightX, StartLocationY, highlightWidth, EndLocationY - StartLocationY));

                graphicsHandler.DrawString(font, Text, new Vector2(StartLocationX - ScrollOffset, box.Y), color: HighlightTextColor);

                graphicsHandler.RemoveScissorRectangle();
            }
        }

        // event for handling keyboard input from OS
        private void Window_TextInput(object sender, TextInputEventArgs e)
        {
            if (e.Key == Keys.Back)
            {
                if (Text.Length > 0 && CursorPosition > 0)
                {
                    if (CursorPosition == Text.Length)
                    {
                        Text = Text.SubstringByIndexes(0, Text.Length - 1);
                    }
                    else
                    {
                        Text = Text.SubstringByIndexes(0, CursorPosition - 1) + Text.SubstringByIndexes(CursorPosition, Text.Length);
                    }
                    CursorPosition--;
                    cursorBlinkTimer.Reset();
                    showCursor = true;
                }
            }
            else if (CharacterLimit < 0 || Text.Length < CharacterLimit)
            {
                if (CursorPosition == Text.Length)
                {
                    Text += e.Character;
                }
                else if (CursorPosition == 0)
                {
                    Text = e.Character + Text;
                }
                else
                {
                    Text = Text.SubstringByIndexes(0, CursorPosition) + e.Character + Text.SubstringByIndexes(CursorPosition, Text.Length);
                }
                CursorPosition++;
                cursorBlinkTimer.Reset();
                showCursor = true;
            }
        }
    }
}
