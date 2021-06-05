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

        protected SpriteFont font;
        protected RectangleGraphic box;
        protected Stopwatch cursorBlinkTimer;
        protected bool showCursor = true;
        protected int spacingBetweenLetters;
        protected Stopwatch cursorChangeTimer;
        protected KeyLocker keyLocker = new KeyLocker();
        protected bool isMouseDrag;
        protected int previousMouseX;
        protected Vector2 previousMouseLocation;
        protected int highlightCursorIndex;
        protected int highlightStartIndex;
        protected int highlightEndIndex;
        protected bool disableCursor = false;
        protected Stopwatch clickTimer;
        private bool clickOnce = true;
        
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
                return ScrollIndex * spacingBetweenLetters;
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

        protected int MaxScrollIndex
        {
            get
            {
                int amountOfRoomBeforeScroll = (EndLocationX - StartLocationX) / spacingBetweenLetters;
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

            clickTimer = new Stopwatch();
            clickTimer.SetWaitTime(SystemInformation.DoubleClickTime);
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
                if (clickTimer.IsTimeUp())
                {
                    clickOnce = false;
                }
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

            if (keyboardState.IsKeyUp(Keys.C))
            {
                keyLocker.UnlockKey(Keys.C);
            }
            if (keyboardState.IsKeyUp(Keys.V))
            {
                keyLocker.UnlockKey(Keys.V);
            }

            UpdateTextScroll();

            previousMouseLocation = new Vector2(mouseState.X, mouseState.Y);
        }

        protected virtual void OnMouseClick(MouseState mouseState, Vector2 mouseLocation)
        {
            float mouseLocationOffset = (mouseLocation.X - StartLocationX + ScrollOffset) / spacingBetweenLetters;
            int calculatedCursorIndex = Math.Min(mouseLocationOffset.Round(), Text.Length);
            
            // if double click without moving mouse, select all text
            if (clickOnce && !clickTimer.IsTimeUp() && Math.Min(mouseLocationOffset.Round(), Text.Length) == CursorPosition)
            {
                HighlightAllText();
            }
            // if single click, move cursor
            else if (StartLocationX + (calculatedCursorIndex * spacingBetweenLetters) - ScrollOffset <= EndLocationX)
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
                clickTimer.Reset();
                clickOnce = true;
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
            // if left is pressed
            if (keyboardState.IsKeyDown(Keys.Left) && !keyLocker.IsKeyLocked(Keys.Left))
            {
                // if shift key is also held, highlight
                if (keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift))
                {
                    if (CursorPosition > 0)
                    {
                        CursorPosition--;
                        isMouseDrag = true;
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
                    disableCursor = highlightStartIndex != highlightEndIndex;
                    if (!disableCursor)
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
                    if (highlightStartIndex == highlightEndIndex)
                    {
                        CursorPosition--;
                    } 
                    else
                    {
                        CursorPosition = highlightStartIndex;
                    }
                    cursorBlinkTimer.Reset();
                    showCursor = true;

                    keyLocker.LockKey(Keys.Left);
                    keyLocker.UnlockKey(Keys.Right);
                    cursorChangeTimer.SetWaitTime(100);

                    highlightCursorIndex = CursorPosition;
                    highlightStartIndex = CursorPosition;
                    highlightEndIndex = CursorPosition;
                    disableCursor = false;
                    isMouseDrag = false;
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
                        isMouseDrag = true;
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
                    disableCursor = highlightStartIndex != highlightEndIndex;
                    if (!disableCursor)
                    {
                        cursorChangeTimer.SetWaitTime(100);
                        showCursor = true;
                    }

                    keyLocker.LockKey(Keys.Right);
                    keyLocker.UnlockKey(Keys.Left);
                    cursorChangeTimer.SetWaitTime(100);
                }
                else
                {
                    if (highlightStartIndex == highlightEndIndex)
                    {
                        CursorPosition++;
                    }
                    else
                    {
                        CursorPosition = highlightEndIndex;
                    }
                    cursorBlinkTimer.Reset();
                    showCursor = true;

                    keyLocker.LockKey(Keys.Right);
                    keyLocker.UnlockKey(Keys.Left);
                    cursorChangeTimer.SetWaitTime(100);

                    highlightCursorIndex = CursorPosition;
                    highlightStartIndex = CursorPosition;
                    highlightEndIndex = CursorPosition;
                    disableCursor = false;
                    isMouseDrag = false;
                }
            }

            // ctrl c, ctrl v
            if (keyboardState.IsKeyDown(Keys.LeftControl) || keyboardState.IsKeyDown(Keys.RightControl)) {
                if (keyboardState.IsKeyDown(Keys.C) && !keyLocker.IsKeyLocked(Keys.C))
                {
                    if (highlightStartIndex != highlightEndIndex)
                    {
                        string copiedText = Text.SubstringByIndexes(highlightStartIndex, highlightEndIndex);
                        Clipboard.SetText(copiedText);
                        keyLocker.LockKey(Keys.C);
                    }
                }
                else if (keyboardState.IsKeyDown(Keys.V) && !keyLocker.IsKeyLocked(Keys.V))
                {
                    keyLocker.LockKey(Keys.V);
                    string pastedText = Clipboard.GetText();
                    // append/insert (no highlighting)
                    if (highlightStartIndex == highlightEndIndex)
                    {
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
                        disableCursor = false;
                        cursorBlinkTimer.Reset();
                        showCursor = true;
                    }
                    // replace highlighted text
                    else
                    {
                        if (highlightStartIndex == 0 && highlightEndIndex == Text.Length)
                        {
                            Text = pastedText;
                        }
                        else if (highlightStartIndex == 0)
                        {
                            Text = pastedText + Text.Substring(pastedText.Length);
                        }
                        else if (highlightEndIndex == Text.Length)
                        {
                            Text = Text.SubstringByIndexes(0, highlightStartIndex) + pastedText;
                        }
                        else
                        {
                            Text = Text.SubstringByIndexes(0, highlightStartIndex) + pastedText + Text.SubstringByIndexes(highlightEndIndex, Text.Length);
                        }
                        CursorPosition = highlightEndIndex;
                        highlightCursorIndex = CursorPosition;
                        highlightStartIndex = CursorPosition;
                        highlightEndIndex = CursorPosition;
                        isMouseDrag = false;
                        disableCursor = false;
                        cursorBlinkTimer.Reset();
                        showCursor = true;
                    }
                }
                // ctrl + a (select all text)
                else if (keyboardState.IsKeyDown(Keys.A))
                {
                    HighlightAllText();
                }

            }
        }

        protected virtual void UpdateTextScroll()
        {
            // if cursor position is off screen
            while(StartLocationX + CursorOffset - ScrollOffset > EndLocationX)
            {
                ScrollIndex++;
            }

            while(StartLocationX + CursorOffset - ScrollOffset < StartLocationX)
            {
                ScrollIndex--;
            }
        }

        protected virtual void HighlightAllText()
        {
            disableCursor = true;
            highlightCursorIndex = 0;
            highlightStartIndex = 0;
            highlightEndIndex = Text.Length;
            CursorPosition = Text.Length;
            ScrollIndex = MaxScrollIndex;
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
            KeyboardState keyboardState = Keyboard.GetState();
            if (!keyboardState.IsKeyDown(Keys.LeftControl) && !keyboardState.IsKeyDown(Keys.RightControl)) // prevents ctrl + c and stuff from breaking since draw string cannot draw those weird characters
            {
                if (e.Key == Keys.Back)
                {
                    if (highlightStartIndex == highlightEndIndex)
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
                            if (ScrollIndex > 0)
                            {
                                ScrollIndex--;
                            }

                            highlightCursorIndex = CursorPosition;
                            highlightStartIndex = CursorPosition;
                            highlightEndIndex = CursorPosition;
                            disableCursor = false;
                            isMouseDrag = false;
                        }
                    }
                    else
                    {
                        if (highlightStartIndex == 0 && highlightEndIndex == Text.Length)
                        {
                            Text = "";
                        }
                        else if (highlightStartIndex == 0)
                        {
                            Text = Text.Substring(highlightEndIndex);
                        }
                        else if (highlightEndIndex == Text.Length)
                        {
                            Text = Text.Substring(0, highlightStartIndex);
                        }
                        else
                        {
                            Text = Text.SubstringByIndexes(0, highlightStartIndex) + Text.SubstringByIndexes(highlightEndIndex, Text.Length);
                        }

                        ScrollIndex -= highlightEndIndex - highlightStartIndex;
                        CursorPosition = highlightStartIndex;
                        highlightCursorIndex = CursorPosition;
                        highlightStartIndex = CursorPosition;
                        highlightEndIndex = CursorPosition;
                        isMouseDrag = false;
                        disableCursor = false;
                        cursorBlinkTimer.Reset();
                        showCursor = true;
                    }
                }
                else if (CharacterLimit < 0 || Text.Length < CharacterLimit)
                {
                    if (highlightStartIndex == highlightEndIndex)
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

                        highlightCursorIndex = CursorPosition;
                        highlightStartIndex = CursorPosition;
                        highlightEndIndex = CursorPosition;
                        disableCursor = false;
                        isMouseDrag = false;
                    }
                    else
                    {
                        if (highlightStartIndex == 0 && highlightEndIndex == Text.Length)
                        {
                            Text = e.Character.ToString();
                        }
                        else if (highlightStartIndex == 0)
                        {
                            Text = e.Character + Text.Substring(highlightEndIndex);
                        }
                        else if (highlightEndIndex == Text.Length)
                        {
                            Text = Text.Substring(0, highlightStartIndex) + e.Character;
                        }
                        else
                        {
                            Text = Text.SubstringByIndexes(0, highlightStartIndex) + e.Character + Text.SubstringByIndexes(highlightEndIndex, Text.Length);
                        }

                        ScrollIndex -= highlightEndIndex - highlightStartIndex;
                        CursorPosition = highlightEndIndex;
                        highlightCursorIndex = CursorPosition;
                        highlightStartIndex = CursorPosition;
                        highlightEndIndex = CursorPosition;
                        isMouseDrag = false;
                        disableCursor = false;
                        cursorBlinkTimer.Reset();
                        showCursor = true;
                    }
                }
            }
        }
    }
}
