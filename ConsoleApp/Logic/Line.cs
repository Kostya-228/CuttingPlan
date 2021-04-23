using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Logic
{
    public class Line
    {
        public Point point1;
        public Point point2;

        private int A { get { return point1.Y - point2.Y; } }
        private int B { get { return point2.X - point1.X; } }
        private int C { get { return point1.X * point2.Y - point2.X * point1.Y; } }

        public float k { get { return -A / (float)B; } }
        public float b { get { return -C / (float)B; } }

        public Line(Point point1, Point point2)
        {
            this.point1 = point1;
            this.point2 = point2;
        }

        /// <summary>
        /// Имеет ли отрезок точку, при условии, что точка точно находится на прямой
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private bool HasAPoint(Point point)
        {
            // чтобы два смежных отрезка давали только одно пересечение через смежную точку
            // считаем, что что пересечение через начало отрезка - не является пересечением
            if (point.Compare(point1)) return false;
            return Math.Min(point1.X, point2.X) <= point.X && Math.Max(point1.X, point2.X) >= point.X 
                && Math.Min(point1.Y, point2.Y) <= point.Y && Math.Max(point1.Y, point2.Y) >= point.Y;
        }

        /// <summary>
        /// Пересекаются ли отрезки
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsCrossing(Line other)
        {
            // общая точка двух прямых
            Point cross_point;
            // если это прямая параллельна оси Y
            if (this.B == 0)
            {
                // если другая прямая тоже параллельна оси Y - то прямые параллельны
                if (other.B == 0)
                {
                    // если они на одной прямой
                    if (this.point1.X == other.point1.X)
                    {
                        // проверка пересечения
                        return Math.Min(point1.Y, point2.Y) < Math.Max(other.point1.Y, other.point2.Y)
                            && Math.Min(other.point1.Y, other.point2.Y) < Math.Max(point1.Y, point2.Y);
                    }
                    return false;
                }
                else
                    cross_point = new Point(this.point1.X, (int)(other.k * this.point1.X + other.b));
            }
            // если только другая прямая параллельна оси Y
            else if (other.B == 0)
                cross_point = new Point(other.point1.X, (int)(this.k * other.point1.X + this.b));
            // если эта прямая параллельна оси X
            else if (this.A == 0)
            {
                // если другая прямая тоже параллельна оси X - то прямые параллельны
                if (other.A == 0) {
                    // если они на одной прямой
                    if (this.point1.Y == other.point1.Y)
                    {
                        // проверка пересечения
                        return Math.Min(point1.X, point2.X) < Math.Max(other.point1.X, other.point2.X)
                            && Math.Min(other.point1.X, other.point2.X) < Math.Max(point1.X, point2.X);
                    }
                    return false;
                }
                else
                    cross_point = new Point((int)((this.point1.Y - other.b) / other.k), this.point1.Y);
            }
            // если только другая параллельна оси X
            else if (other.A == 0)
                cross_point = new Point((int)((other.point1.Y - this.b) / this.k), other.point1.Y);
            else
                cross_point = new Point(
                    (int)((other.b - this.b) / (this.k - other.k)),
                    (int)((this.k * other.b - other.k * this.b) / (this.k - other.k)));
            // если оба отрезка имеют точку пересечения прямых то и отрезки пересекаются
            return this.HasAPoint(cross_point) && other.HasAPoint(cross_point);
        }
    }
}
