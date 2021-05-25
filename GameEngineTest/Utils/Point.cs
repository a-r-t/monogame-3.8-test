using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngineTest.Utils
{
    public class Point
    {
        public float X { get; private set; }
        public float Y { get; private set; }

        public Point(float x, float y)
        {
            X = x;
            Y = y;
        }

        public Point Add(Point point)
        {
            return new Point(X + point.X, Y + point.Y);
        }

        public Point AddX(int dx)
        {
            return new Point(X + dx, Y);
        }

        public Point AddY(int dy)
        {
            return new Point(X, Y + dy);
        }

        public Point Subtract(Point point)
        {
            return new Point(X - point.X, Y - point.Y);
        }

        public Point SubtractX(int dx)
        {
            return new Point(X - dx, Y);
        }

        public Point SubtractY(int dy)
        {
            return new Point(X, Y - dy);
        }
    }
}
