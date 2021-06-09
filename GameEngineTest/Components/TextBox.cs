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
using System.Windows.Forms;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using System.Text.RegularExpressions;

namespace GameEngineTest.Components
{
    public class TextBox
    {
        public string Text { get; set; }
        public int CharacterLimit { get; set; }

        protected KeyLocker keyLocker = new KeyLocker();
        protected SpriteFont font;
        protected RectangleGraphic box;
        protected Stopwatch cursorBlinkTimer;
        protected bool showCursor = true;
        protected int fontCharLength;
        protected Stopwatch cursorChangeTimer;
        protected bool highlightMode;
        protected Vector2 previousMouseLocation;
        protected int highlightCursorIndex;
        protected Stopwatch clickTimer;
        private bool singleClicked = false;
        private bool doubleClicked = false;

        public Vector2 Location
        {
            get
            {
                return new Vector2(box.X, box.Y);
            }
            set
            {
                box.SetLocation(value.X, value.Y);
            }
        }

        public int X
        {
            get
            {
                return (int)box.X;
            }
            set
            {
                box.X = value;
            }
        }

        public int Y
        {
            get
            {
                return (int)box.Y;
            }
            set
            {
                box.Y = value;
            }
        }

        public int Width
        {
            get
            {
                return box.Width;
            }
            set
            {
                box.Width = value;
            }
        }

        private int cursorPosition = 0;
        public int CursorPosition
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

        private int scrollIndex = 0;
        protected int ScrollIndex
        {
            get
            {
                return scrollIndex;
            }
            set
            {
                scrollIndex = value;
                if (scrollIndex < 0)
                {
                    scrollIndex = 0;
                }
            }
        }

        protected int ScrollOffset
        {
            get
            {
                return ScrollIndex * fontCharLength + ((int)font.Spacing * ScrollIndex);
            }
        }

        public int HighlightStartIndex { get; set; }
        public int HighlightEndIndex { get; set; }

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
                return (CursorPosition * fontCharLength) + ((int)font.Spacing * CursorPosition);
            }
        }

        protected int MaxScrollIndex
        {
            get
            {
                int amountOfRoomBeforeScroll = (EndLocationX - StartLocationX) / fontCharLength;
                if (Text.Length <= amountOfRoomBeforeScroll)
                {
                    return 0;
                }
                else
                {
                    return Text.Length - amountOfRoomBeforeScroll;
                }
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

        public int BorderThickness
        {
            get
            {
                return box.BorderThickness;
            }
            set
            {
                box.BorderThickness = value;
            }
        }

        // space between letters in font
        public int FontSpacing
        {
            get
            {
                return (int)font.Spacing;
            }
            set
            {
                font.Spacing = value;
                if (font.Spacing < 1)
                {
                    font.Spacing = 1;
                }
            }
        }

        public bool IsTextHighlighted
        {
            get
            {
                return HighlightStartIndex != HighlightEndIndex;
            }
        }

        public string HighlightedText
        {
            get
            {
                return Text.SubstringByIndexes(HighlightStartIndex, HighlightEndIndex);
            }
        }

        protected bool IsCharacterLimitSet
        {
            get
            {
                return CharacterLimit > -1;
            }
        }

        protected bool IsAtCharacterLimit
        {
            get
            {
                return IsCharacterLimitSet && Text.Length >= CharacterLimit;
            }
        }

        public TextBox(int x, int y, int width, SpriteFont spriteFont, string defaultText = "", int characterLimit = -1, int borderThickness = 2)
        {
            box = new RectangleGraphic(x, y, width, spriteFont.LineSpacing + (borderThickness * 2));
            box.Color = Color.White;
            box.BorderColor = Color.Black;
            box.BorderThickness = borderThickness;
            Text = defaultText;
            font = spriteFont;

            // make sure spacing between each letter is at least 1 so the cursor can fit inbetween
            if (spriteFont.Spacing < 1)
            {
                spriteFont.Spacing = 1;
            }

            if (spriteFont.DefaultCharacter == null)
            {
                spriteFont.DefaultCharacter = '?';
            }

            CursorColor = Color.Black;
            BackColor = Color.White;
            BorderColor = Color.Black;
            TextColor = Color.Black;
            HighlightColor = new Color(50, 151, 253); // blue
            HighlightTextColor = Color.White;
            
            cursorBlinkTimer = new Stopwatch();
            cursorBlinkTimer.SetWaitTime(500);

            cursorChangeTimer = new Stopwatch();

            fontCharLength = (int)spriteFont.MeasureString("a").X;

            CharacterLimit = characterLimit;

            GameLoop.GameWindow.TextInput += Window_TextInput;

            clickTimer = new Stopwatch();
            clickTimer.SetWaitTime(SystemInformation.DoubleClickTime);
        }

        // destructor to prevent memory leak
        ~TextBox()
        {
            GameLoop.GameWindow.TextInput -= Window_TextInput;
        }

        public virtual void Update()
        {
            MouseState mouseState = Mouse.GetState();
            Vector2 mouseLocation = new Vector2(mouseState.X, mouseState.Y);

            // mouse events
            if (mouseState.LeftButton == ButtonState.Pressed && box.ContainsPoint(mouseLocation) && !highlightMode)
            {
                OnMouseClick(mouseState, mouseLocation);
            }
            else if (mouseState.LeftButton == ButtonState.Released)
            {
                highlightMode = false;
                doubleClicked = false;
            }
            if (singleClicked && clickTimer.IsTimeUp())
            {
                singleClicked = false;
            }
            if (highlightMode && cursorChangeTimer.IsTimeUp())
            {
                if (mouseState.X != (int)previousMouseLocation.X)
                {
                    OnMouseDrag(mouseState, mouseLocation);
                }
            }

            // keyboard events
            KeyboardState keyboardState = Keyboard.GetState();
            OnKeyPress(keyboardState);
            
            // state changes
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
            if (keyboardState.IsKeyUp(Keys.C))
            {
                keyLocker.UnlockKey(Keys.C);
            }
            if (keyboardState.IsKeyUp(Keys.V))
            {
                keyLocker.UnlockKey(Keys.V);
            }

            // text scrolling (if cursor is moved out of view, text needs to scroll)
            UpdateTextScroll();

            previousMouseLocation = new Vector2(mouseState.X, mouseState.Y);
        }

        protected virtual void OnMouseClick(MouseState mouseState, Vector2 mouseLocation)
        {
            float mouseLocationOffset = (mouseLocation.X - StartLocationX + ScrollOffset) / fontCharLength;
            int calculatedCursorIndex = Math.Min(mouseLocationOffset.Round(), Text.Length);
            
            // if double click without moving mouse, select all text
            if (singleClicked && !clickTimer.IsTimeUp() && Math.Min(mouseLocationOffset.Round(), Text.Length) == CursorPosition)
            {
                doubleClicked = true;
                HighlightAllText();
            }
            // if single click, move cursor
            else if (StartLocationX + (calculatedCursorIndex * fontCharLength) - ScrollOffset <= EndLocationX && !doubleClicked)
            {
                CursorPosition = Math.Min(mouseLocationOffset.Round(), Text.Length);
                ResetCursorBlinkTimer();

                cursorChangeTimer.SetWaitTime(100);

                highlightMode = true;
                ResetHighlightIndexes();

                clickTimer.Reset();
                singleClicked = true;
            }
        }

        protected virtual void OnMouseDrag(MouseState mouseState, Vector2 mouseLocation)
        {
            int direction = mouseState.X - (int)previousMouseLocation.X > 0 ? 1 : -1;

            if (direction == -1)
            {
                if (CursorPosition > 0)
                {
                    float mouseLocationOffset = (mouseLocation.X - StartLocationX + ScrollOffset) / fontCharLength;
                    CursorPosition = Math.Min(mouseLocationOffset.Round(), Text.Length);
                }
                if (CursorPosition < highlightCursorIndex)
                {
                    HighlightStartIndex = CursorPosition;
                }
                else if (CursorPosition > highlightCursorIndex)
                {
                    HighlightEndIndex = CursorPosition;
                }
                else
                {
                    HighlightStartIndex = CursorPosition;
                    HighlightEndIndex = CursorPosition;
                }
            }
            else if (direction == 1)
            {
                if (CursorPosition < Text.Length)
                {
                    float mouseLocationOffset = (mouseLocation.X - StartLocationX + ScrollOffset) / fontCharLength;
                    CursorPosition = Math.Min(mouseLocationOffset.Round(), Text.Length);
                }
                if (CursorPosition > highlightCursorIndex)
                {
                    HighlightEndIndex = CursorPosition;
                }
                else if (CursorPosition < highlightCursorIndex)
                {
                    HighlightStartIndex = CursorPosition;
                }
                else
                {
                    HighlightStartIndex = CursorPosition;
                    HighlightEndIndex = CursorPosition;
                }
            }

            if (!IsTextHighlighted)
            {
                cursorChangeTimer.SetWaitTime(100);
                showCursor = true;
            }
        }

        protected virtual void OnKeyPress(KeyboardState keyboardState)
        {
            // if left is pressed
            if (keyboardState.IsKeyDown(Keys.Left) && !keyLocker.IsKeyLocked(Keys.Left))
            {
                // if shift key is also held, highlight
                if (keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift))
                {
                    if (CursorPosition > 0)
                    {
                        CursorPosition--;
                        highlightMode = true;
                    }
                    if (CursorPosition < highlightCursorIndex)
                    {
                        HighlightStartIndex = CursorPosition;
                    }
                    else if (CursorPosition > highlightCursorIndex)
                    {
                        HighlightEndIndex = CursorPosition;
                    }
                    else
                    {
                        HighlightStartIndex = CursorPosition;
                        HighlightEndIndex = CursorPosition;
                    }
                    if (!IsTextHighlighted)
                    {
                        cursorChangeTimer.SetWaitTime(100);
                        showCursor = true;
                    }

                    keyLocker.LockKey(Keys.Left);
                    keyLocker.UnlockKey(Keys.Right);
                    cursorChangeTimer.SetWaitTime(100);
                }
                // move cursor to left
                else
                {
                    if (!IsTextHighlighted)
                    {
                        CursorPosition--;
                    } 
                    else
                    {
                        CursorPosition = HighlightStartIndex;
                    }
                    ResetCursorBlinkTimer();

                    keyLocker.LockKey(Keys.Left);
                    keyLocker.UnlockKey(Keys.Right);
                    cursorChangeTimer.SetWaitTime(100);

                    ResetHighlightIndexes();
                    highlightMode = false;
                }
            }
            // if right is pressed
            else if (keyboardState.IsKeyDown(Keys.Right) && !keyLocker.IsKeyLocked(Keys.Right))
            {
                // if shift key is also held, highlight
                if (keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift))
                {
                    if (CursorPosition < Text.Length)
                    {
                        CursorPosition++;
                        highlightMode = true;
                    }
                    if (CursorPosition > highlightCursorIndex)
                    {
                        HighlightEndIndex = CursorPosition;
                    }
                    else if (CursorPosition < highlightCursorIndex)
                    {
                        HighlightStartIndex = CursorPosition;
                    }
                    else
                    {
                        HighlightStartIndex = CursorPosition;
                        HighlightEndIndex = CursorPosition;
                    }
                    if (!IsTextHighlighted)
                    {
                        cursorChangeTimer.SetWaitTime(100);
                        showCursor = true;
                    }

                    keyLocker.LockKey(Keys.Right);
                    keyLocker.UnlockKey(Keys.Left);
                    cursorChangeTimer.SetWaitTime(100);
                }
                // move cursor to right
                else
                {
                    if (!IsTextHighlighted)
                    {
                        CursorPosition++;
                    }
                    else
                    {
                        CursorPosition = HighlightEndIndex;
                    }
                    ResetCursorBlinkTimer();

                    keyLocker.LockKey(Keys.Right);
                    keyLocker.UnlockKey(Keys.Left);
                    cursorChangeTimer.SetWaitTime(100);

                    ResetHighlightIndexes();
                    highlightMode = false;
                }
            }

            // ctrl c, ctrl v
            if (keyboardState.IsKeyDown(Keys.LeftControl) || keyboardState.IsKeyDown(Keys.RightControl)) {
                if (keyboardState.IsKeyDown(Keys.C) && !keyLocker.IsKeyLocked(Keys.C))
                {
                    if (IsTextHighlighted)
                    {
                        string copiedText = Text.SubstringByIndexes(HighlightStartIndex, HighlightEndIndex);
                        Clipboard.SetText(copiedText);
                        keyLocker.LockKey(Keys.C);
                    }
                }
                else if (keyboardState.IsKeyDown(Keys.V) && !keyLocker.IsKeyLocked(Keys.V))
                {
                    keyLocker.LockKey(Keys.V);
                    string pastedText = Clipboard.GetText();

                    // append/insert (no highlighting)
                    if (!IsTextHighlighted && !IsAtCharacterLimit)
                    {
                        // if pasted string would make text go past character limit, cut off a piece from pasted text to stay at limit
                        if (IsCharacterLimitSet && Text.Length + pastedText.Length > CharacterLimit)
                        {
                            int difference = Text.Length + pastedText.Length - CharacterLimit;
                            pastedText = pastedText.Substring(0, difference);
                        }

                        if (CursorPosition == Text.Length)
                        {
                            Text += pastedText;
                        }
                        else if (CursorPosition == 0)
                        {
                            Text = pastedText + Text;
                        }
                        else
                        {
                            Text = Text.SubstringByIndexes(0, CursorPosition) + pastedText + Text.SubstringByIndexes(CursorPosition, Text.Length);
                        }
                        CursorPosition += pastedText.Length;
                    }
                    // replace highlighted text
                    else
                    {
                        // if pasted string would make text go past character limit, cut off a piece from pasted text to stay at limit
                        if (IsCharacterLimitSet && Text.Length - HighlightedText.Length + pastedText.Length > CharacterLimit)
                        {
                            int difference = Text.Length - HighlightedText.Length + pastedText.Length - CharacterLimit;
                            pastedText = pastedText.Substring(0, difference);
                        }

                        if (HighlightStartIndex == 0 && HighlightEndIndex == Text.Length)
                        {
                            Text = pastedText;
                        }
                        else if (HighlightStartIndex == 0)
                        {
                            Text = pastedText + Text.Substring(pastedText.Length);
                        }
                        else if (HighlightEndIndex == Text.Length)
                        {
                            Text = Text.SubstringByIndexes(0, HighlightStartIndex) + pastedText;
                        }
                        else
                        {
                            Text = Text.SubstringByIndexes(0, HighlightStartIndex) + pastedText + Text.SubstringByIndexes(HighlightEndIndex, Text.Length);
                        }
                        CursorPosition = HighlightEndIndex;
                        ResetHighlightIndexes();
                        highlightMode = false;
                    }
                    ResetCursorBlinkTimer();
                }
                // ctrl + a (select all text)
                else if (keyboardState.IsKeyDown(Keys.A))
                {
                    HighlightAllText();
                }

            }
        }

        protected void ResetCursorBlinkTimer()
        {
            cursorBlinkTimer.Reset();
            showCursor = true;
        }

        protected virtual void UpdateTextScroll()
        {
            // if cursor position is off screen
            if (StartLocationX + CursorOffset - ScrollOffset > EndLocationX) {
                do
                {
                    ScrollIndex++;
                } while (StartLocationX + CursorOffset - ScrollOffset > EndLocationX);
            }
            else if (StartLocationX + CursorOffset - ScrollOffset < StartLocationX)
            {
                do
                {
                    ScrollIndex--;
                } while (StartLocationX + CursorOffset - ScrollOffset < StartLocationX);
            }
        }

        protected void ResetHighlightIndexes()
        {
            highlightCursorIndex = CursorPosition;
            HighlightStartIndex = CursorPosition;
            HighlightEndIndex = CursorPosition;
        }

        protected void HighlightAllText()
        {
            highlightCursorIndex = 0;
            HighlightStartIndex = 0;
            HighlightEndIndex = Text.Length;
            CursorPosition = Text.Length;
            ScrollIndex = MaxScrollIndex;
        }

        public virtual void Draw(GraphicsHandler graphicsHandler)
        {
            // textbox itself (includes border)
            box.Draw(graphicsHandler);

            // setting scissor rectangle to the size of the whitespace in textbox
            // anything attempting to be drawn outside of scissor rectangle will be cut off
            // necessary to cut off text and such, since otherwise there's no way to only draw a partial letter of a spritefont
            graphicsHandler.SetScissorRectangle(new Rectangle(
                (int)box.X + box.BorderThickness,
                (int)box.Y + box.BorderThickness,
                box.Width - box.BorderThickness,
                box.Height - box.BorderThickness
            ));

            // highlighting
            int highlightX = StartLocationX + (HighlightStartIndex * fontCharLength) - ScrollOffset + (HighlightStartIndex * (int)font.Spacing);
            int highlightWidth = ((HighlightEndIndex - HighlightStartIndex) * fontCharLength) + ((HighlightEndIndex - HighlightStartIndex) * (int)font.Spacing);
            graphicsHandler.DrawFilledRectangle(new Rectangle(highlightX, StartLocationY, highlightWidth, EndLocationY - StartLocationY), HighlightColor);

            // text
            graphicsHandler.DrawString(font, Text, new Vector2(StartLocationX - ScrollOffset, box.Y), color: TextColor);

            // cursor
            if (showCursor && !IsTextHighlighted)
            {
                graphicsHandler.DrawLine(new Vector2(StartLocationX + CursorOffset - ScrollOffset, StartLocationY), new Vector2(StartLocationX + CursorOffset - ScrollOffset, EndLocationY), CursorColor);
            }

            graphicsHandler.RemoveScissorRectangle();

            // if text is highlighted, the text needs to be redrawn to be the highlight color
            if (IsTextHighlighted)
            {
                int highlightTextX = StartLocationX + (HighlightStartIndex * fontCharLength) - ScrollOffset + (HighlightStartIndex * (int)font.Spacing);
                int highlightTextWidth = ((HighlightEndIndex - HighlightStartIndex) * fontCharLength) + ((HighlightEndIndex - HighlightStartIndex) * (int)font.Spacing);
                if (highlightTextX + highlightTextWidth > box.X + box.Width - box.BorderThickness)
                {
                    highlightTextWidth = (int)box.X + box.Width - highlightTextX;
                }
                if (highlightTextX < (int)box.X + box.BorderThickness)
                {
                    int difference = (int)box.X + box.BorderThickness - highlightTextX;
                    highlightTextX = (int)box.X + box.BorderThickness;
                    highlightTextWidth -= difference;
                }

                // redraw the spritefont text only where the text is highlighted
                graphicsHandler.SetScissorRectangle(new Rectangle(highlightTextX, StartLocationY, highlightTextWidth, EndLocationY - StartLocationY));

                // draw text in highlighted color
                graphicsHandler.DrawString(font, Text, new Vector2(StartLocationX - ScrollOffset, box.Y), color: HighlightTextColor);

                graphicsHandler.RemoveScissorRectangle();
            }
        }

        private bool IsControlKeyPressed()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            return keyboardState.IsKeyDown(Keys.LeftControl) || keyboardState.IsKeyDown(Keys.RightControl);
        }

        // event for handling keyboard input from OS
        private void Window_TextInput(object sender, TextInputEventArgs e)
        {
            if (!IsControlKeyPressed()) // prevents ctrl + c and stuff from breaking since draw string cannot draw those weird characters
            {
                // if there is room for more text or backspace input is received, process input
                if (!IsAtCharacterLimit || e.Key == Keys.Back)
                {
                    HandleTextInput(e);
                    ResetCursorBlinkTimer();
                    ResetHighlightIndexes();
                    highlightMode = false;
                }
            }
        }

        private void HandleTextInput(TextInputEventArgs e)
        {
            if (e.Key == Keys.Back)
            {
                HandleBackSpaceInput();
            }
            else
            {
                if (!IsTextHighlighted)
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
                }
                else
                {
                    if (HighlightStartIndex == 0 && HighlightEndIndex == Text.Length)
                    {
                        Text = e.Character.ToString();
                    }
                    else if (HighlightStartIndex == 0)
                    {
                        Text = e.Character + Text.Substring(HighlightEndIndex);
                    }
                    else if (HighlightEndIndex == Text.Length)
                    {
                        Text = Text.Substring(0, HighlightStartIndex) + e.Character;
                    }
                    else
                    {
                        Text = Text.SubstringByIndexes(0, HighlightStartIndex) + e.Character + Text.SubstringByIndexes(HighlightEndIndex, Text.Length);
                    }
                    CursorPosition = HighlightEndIndex;
                }
            }
        }

        private void HandleBackSpaceInput()
        {
            if (!IsTextHighlighted)
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
                    ScrollIndex--;
                }
            }
            else
            {
                if (HighlightStartIndex == 0 && HighlightEndIndex == Text.Length)
                {
                    Text = "";
                }
                else if (HighlightStartIndex == 0)
                {
                    Text = Text.Substring(HighlightEndIndex);
                }
                else if (HighlightEndIndex == Text.Length)
                {
                    Text = Text.Substring(0, HighlightStartIndex);
                }
                else
                {
                    Text = Text.SubstringByIndexes(0, HighlightStartIndex) + Text.SubstringByIndexes(HighlightEndIndex, Text.Length);
                }

                CursorPosition = HighlightStartIndex;
                ScrollIndex -= HighlightEndIndex - HighlightStartIndex;
            }
        }
    }
}
