using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace СuttingPlan
{
    class Point
    {
        public int X;
        public int Y;

        public Point(int x=0, int y=0)
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
    }

    class Line
    {
        public Point point1;
        public Point point2;

        private int A { get { return point1.Y - point2.Y; } }
        private int B { get { return point2.X - point1.X; } }
        private int C { get { return point1.X * point2.Y - point2.X*point1.Y; } }

        public float k { get { return -A/B; } }
        public float b { get { return -C/B; } }

        public Line(Point point1, Point point2)
        {
            this.point1 = point1;
            this.point2 = point2;
        }
        
        /// <summary>
        /// Имеет ли отрезок точку
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private bool HasAPoint(Point point)
        {
            // если сумма расстояний от точки до граничных точек отрезка равна длине отрезка
            return point.GetSqareDistance(point1) + point.GetSqareDistance(point2) == point1.GetSqareDistance(point2);
        }

        /// <summary>
        /// Пересекаются ли отрезки
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsCrossing(Line other)
        {
            // общая точка двух прямых
            var cross_point = new Point(
                (int)((other.b - this.b)/(this.k - other.k)), 
                (int)((this.k * other.b - other.k * this.b) / (this.k - other.k)));
            // если оба отрезка имеют точку пересечения прямых то и отрезки пересекаются
            return this.HasAPoint(cross_point) && other.HasAPoint(cross_point);
        }
    }

    class Curve
    {
        public Point point1;
        public Point point2;
        public Point point3;

        public bool IsCrossing(Curve other)
        {
            foreach (var line1 in this.GetLines())
            {
                foreach (var line2 in other.GetLines())
                {
                    if (line1.IsCrossing(line2))
                        return true;
                }
            }
            return false;
        }

        public List<Line> GetLines()
        {
            var lines = new List<Line>();
            lines.Add(new Line(point1, point2));
            lines.Add(new Line(point2, point3));
            lines.Add(new Line(point3, point1));
            return lines;
        }
    }


    class Detail
    {
        private Point[] points;

        private Point position = new Point();
        private float angle;

        //private List<Curve> curves;

        public Detail(Point[] points)
        {
            this.points = points;
        }


        public bool IsCrossing(Detail other)
        {
            foreach(var c1 in this.GetCurves())
            {
                foreach (var c2 in other.GetCurves())
                {
                    if (c1.IsCrossing(c2))
                        return true;
                }
            }
            return false;
        }

        public List<Curve> GetCurves()
        {
            var curves = new List<Curve>();
            for (int i = 1; i < points.Length - 1; i += 2)
                curves.Add(new Curve() { 
                    point1 = points[i-1] + position,
                    point2 = points[i] + position,
                    point3 = points[i+1] + position
                });
            curves.Add(new Curve()
            {
                point1 = points[points.Length - 2] + position,
                point2 = points[points.Length - 1] + position,
                point3 = points[0] + position
            });
            return curves;
        }
    }
}
