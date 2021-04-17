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

        public static Point operator +(Point a, Point b)
        {
            return new Point(a.X + b.X, a.Y + b.Y);
        }

        /// <summary>
        /// расстояние от точки до точки в квадрате
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public float GetSqareDistance(Point point)
        {
            return (int)Math.Pow(point.X - X, 2) + (int)Math.Pow(point.Y - Y, 2);
        }

        public System.Drawing.Point GetDrawing()
        {
            return new System.Drawing.Point(X, Y);
        }

        public static Point operator *(Point a, int size)
        {
            return new Point(a.X * size, a.Y * size);
        }

        public object Clone()
        {
            return new Point(X, Y);
        }
    }
}
