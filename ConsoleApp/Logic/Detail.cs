using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Logic
{
    public class Detail : ICloneable
    {
        public int Size = 1;
        public Point position = new Point();

        private Point[] points;
        private float angle;

        public Detail(Point[] points)
        {
            this.points = points;
        }

        public Detail(Point[] points, int size)
        {
            this.points = points;
            this.Size = size;
        }

        public Detail(Point[] points, int size, Point position)
        {
            this.points = points;
            this.Size = size;
            this.position = position;
        }


        public bool IsCrossing(Detail other)
        {
            foreach (var c1 in this.GetCurves())
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
                curves.Add(new Curve()
                {
                    point1 = (points[i - 1] + position) * Size,
                    point2 = (points[i] + position) * Size,
                    point3 = (points[i + 1] + position) * Size
                });
            // соединяем начало и конец
            curves.Add(new Curve()
            {
                point1 = (points[points.Length - 2] + position) * Size,
                point2 = (points[points.Length - 1] + position) * Size,
                point3 = (points[0] + position) * Size
            });
            return curves;
        }

        public object Clone()
        {
            return new Detail(
                points.Select(pnt => (Point)pnt.Clone()).ToArray(),
                Size,
                (Point)position.Clone());
        }
    }
}
