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
        public float Size = 1;
        public float Angle;

        public Point[] points { get; private set; }
        public Point center { get; private set; }

        public Detail(Point[] points, Point center = null)
        {
            this.points = points;
            if (center != null)
                this.center = center;
            else
                this.center = new Point();
        }

        public Detail(Point[] points, float size, Point center = null)
        {
            this.points = points;
            this.Size = size;
            if (center != null)
                this.center = center;
            else
                this.center = new Point();
        }

        public Detail(Point[] points, float size, Point position, Point center = null)
        {
            this.points = points;
            this.Size = size;
            this.position = position;
            if (center != null)
                this.center = center;
            else
                this.center = new Point();
        }

        public bool IsCrossing(Detail[] others)
        {
            foreach (var det in others)
                if (IsCrossing(det))
                    return true;
            return false;
        }


        public bool IsCrossing(Detail other)
        {
            if (other.ContainsPoint(this.GetTranslatedPoints().First()))
                return true;
            if (this.ContainsPoint(other.GetTranslatedPoints().First()))
                return true;

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
        public Point[] GetInternalPolygon()
        {
            return GetTranslatedPoints().Where((x, i) => i % 2 == 0).ToArray();
        }

        private bool ContainsPoint(Point point)
        {
            return IsInPolygon(GetInternalPolygon(), Translate(center), point);
        }

        private static bool IsInPolygon(Point[] poly, Point center, Point point)
        {
            List<Line> lines = new List<Line>();
            for (int i = 1; i < poly.Length; i += 1)
                lines.Add(new Line(poly[i - 1], poly[i]));
            lines.Add(new Line(poly[poly.Length - 1], poly[0]));

            var line = new Line(center, point);
            var c = lines.Where(l => l.IsCrossing(line)).Count();

            return c % 2 == 0;
        }

        public Point Translate(Point p)
        {
            return ((p - center) * Size).Rotate(Angle, center) + position;
        }


        public IEnumerable<Point> GetTranslatedPoints()
        {
            return points.Select(p => Translate(p));
        }

        public List<Curve> GetCurves()
        {
            var curves = new List<Curve>();
            Point[] transaltared_points = GetTranslatedPoints().ToArray();
            for (int i = 1; i < transaltared_points.Length - 1; i += 2)
                curves.Add(new Curve()
                {
                    point1 = transaltared_points[i - 1],
                    point2 = transaltared_points[i],
                    point3 = transaltared_points[i + 1]
                });
            // соединяем начало и конец
            curves.Add(new Curve()
            {
                point1 = transaltared_points[transaltared_points.Length - 2],
                point2 = transaltared_points[transaltared_points.Length - 1],
                point3 = transaltared_points[0]
            });
            return curves;
        }

        public bool IsCrossBorder(int borderX = 0, int borderY = 0)
        {
            return borderX > 0 && GetTranslatedPoints().Select(p => p.X).Max() > borderX ||
                borderY > 0 && GetTranslatedPoints().Select(p => p.Y).Max() > borderY ||
                GetTranslatedPoints().Select(p => p.X).Min() < 0 ||
                GetTranslatedPoints().Select(p => p.Y).Min() < 0;
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
