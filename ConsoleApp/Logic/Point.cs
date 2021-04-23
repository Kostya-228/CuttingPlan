using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Logic
{
    public class Point : ICloneable
    {
        public int X;
        public int Y;

        public Point(int x = 0, int y = 0)
        {
            X = x;
            Y = y;
        }

        public Point Rotate(float degres, Point center)
        {
            if (degres == 0)
                return this;

            var x = X - center.X;
            var y = Y - center.Y;
            var radians = Math.PI * degres / 180.0;
            return new Point(
                X = (int)(x * Math.Cos(radians) - y * Math.Sin(radians)),
                Y = (int)(x * Math.Sin(radians) + y * Math.Cos(radians))
                );
        }

        public System.Drawing.Point GetDrawing()
        {
            return new System.Drawing.Point(X, Y);
        }

        public object Clone()
        {
            return new Point(X, Y);
        }

        public static Point operator *(Point a, int size)
        {
            return new Point(a.X * size, a.Y * size);
        }

        public static Point operator +(Point a, Point b)
        {
            return new Point(a.X + b.X, a.Y + b.Y);
        }

        public static Point operator -(Point a, Point b)
        {
            return new Point(a.X - b.X, a.Y - b.Y);
        }

        public bool Compare(Point other)
        {
            return other.X == X && other.Y == Y;
        }
    }
}
