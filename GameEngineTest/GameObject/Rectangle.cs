using GameEngineTest.Engine;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameEngineTest.GameObject
{
    public class Rectangle : IntersectableRectangle
    {
        public float X { get; set; }
        public float Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public float Scale { get; set; }
        public Color Color { get; set; }
        public Color BorderColor { get; set; }
        public int BorderThickness { get; set; }

        public Rectangle(float x, float y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Scale = 1f;
            Color = Color.White;
            BorderColor = Color.Transparent;
            BorderThickness = 0;
        }

        public Rectangle(float x, float y, int width, int height, float scale)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Scale = scale;
            Color = Color.White;
            BorderColor = Color.Transparent;
            BorderThickness = 0;
        }

        public float GetX1()
        {
            return X;
        }

        public float GetX2()
        {
            return X + Width;
        }

        public float GetScaledX2()
        {
            return X + GetScaledWidth();
        }

        public void MoveX(float dx)
        {
            X += dx;
        }

        public void MoveRight(float dx)
        {
            X += dx;
        }

        public void MoveLeft(float dx)
        {
            X -= dx;
        }

        public float GetY1()
        {
            return Y;
        }

        public float GetY2()
        {
            return Y + Height;
        }

        public float GetScaledY2()
        {
            return Y + GetScaledHeight();
        }

        public void MoveY(float dy)
        {
            Y += dy;
        }

        public void MoveDown(float dy)
        {
            Y += dy;
        }

        public void MoveUp(float dy)
        {
            Y -= dy;
        }

        public void SetLocation(float x, float y)
        {
            X = x;
            Y = y;
        }

        public int GetScaledWidth()
        {
            return (int)Math.Round(Width * Scale);
        }

        public int GetScaledHeight()
        {
            return (int)Math.Round(Height * Scale);
        }

        public virtual void Update() { }

        public virtual void Draw(GraphicsHandler graphicsHandler)
        {
            graphicsHandler.DrawFilledRectangle((int)Math.Round(X), (int)Math.Round(X), GetScaledWidth(), GetScaledHeight(), Color);
            if (BorderColor != null && !BorderColor.Equals(Color))
            {
                graphicsHandler.DrawRectangle((int)Math.Round(X), (int)Math.Round(Y), GetScaledWidth(), GetScaledHeight(), BorderColor, BorderThickness);
            }
        }

        public virtual Rectangle GetIntersectRectangle()
        {
            return new Rectangle(X, Y, GetScaledWidth(), GetScaledHeight());
        }

        // check if this intersects with another rectangle
        public bool Intersects(IntersectableRectangle other)
        {
            Rectangle intersectRectangle = GetIntersectRectangle();
            Rectangle otherIntersectRectangle = other.GetIntersectRectangle();
            return Math.Round(intersectRectangle.GetX1()) < Math.Round(otherIntersectRectangle.GetX2()) && Math.Round(intersectRectangle.GetX2()) > Math.Round(otherIntersectRectangle.GetX1()) &&
                    Math.Round(intersectRectangle.GetY1()) < Math.Round(otherIntersectRectangle.GetY2()) && Math.Round(intersectRectangle.GetY2()) > Math.Round(otherIntersectRectangle.GetY1());
        }

        // check if this overlaps with another rectangle
        public bool Overlaps(IntersectableRectangle other)
        {
            Rectangle intersectRectangle = GetIntersectRectangle();
            Rectangle otherIntersectRectangle = other.GetIntersectRectangle();
            return Math.Round(intersectRectangle.GetX1()) <= Math.Round(otherIntersectRectangle.GetX2()) && Math.Round(intersectRectangle.GetX2()) >= Math.Round(otherIntersectRectangle.GetX1()) &&
                    Math.Round(intersectRectangle.GetY1()) <= Math.Round(otherIntersectRectangle.GetY2()) && Math.Round(intersectRectangle.GetY2()) >= Math.Round(otherIntersectRectangle.GetY1());
        }

        public override string ToString()
        {
            return string.Format("Rectangle: x=%s y=%s width=%s height=%s", X, Y, GetScaledWidth(), GetScaledHeight());
        }
    }
}
