using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Logic
{
    public class Detail : ICloneable
    {
        public Point position = new Point();
        public int Size = 1;
        public float Angle;

        private Point[] points;
        private Point center;   

        public Detail(Point[] points, Point center = null)
        {
            this.points = points;
            if (center != null)
                this.center = center;
            else
                this.center = new Point();
        }

        public Detail(Point[] points, int size, Point center = null)
        {
            this.points = points;
            this.Size = size;
            if (center != null)
                this.center = center;
            else
                this.center = new Point();
        }

        public Detail(Point[] points, int size, Point position, Point center = null)
        {
            this.points = points;
            this.Size = size;
            this.position = position;
            if (center != null)
                this.center = center;
            else
                this.center = new Point();
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
                    point1 = ((points[i - 1] - center) * Size).Rotate(Angle, center) + position,
                    point2 = ((points[i] - center) * Size).Rotate(Angle, center) + position,
                    point3 = ((points[i + 1] - center) * Size).Rotate(Angle, center) + position
                });
            // соединяем начало и конец
            curves.Add(new Curve()
            {
                point1 = ((points[points.Length - 2] - center) * Size).Rotate(Angle, center) + position,
                point2 = ((points[points.Length - 1]- center) * Size).Rotate(Angle, center) + position,
                point3 = ((points[0] - center) * Size).Rotate(Angle, center) + position
            });
            return curves;
        }

        public object Clone()
        {
            return new Detail(
                points.Select(pnt => (Point)pnt.Clone()).ToArray(),
                Size,
                (Point)position.Clone(),
                (Point)center.Clone());
        }
    }
}
