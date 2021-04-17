using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Logic
{
    public class Curve
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

        public System.Drawing.Point[] GetDrawing()
        {
            return new System.Drawing.Point[] { point1.GetDrawing(), point2.GetDrawing(), point3.GetDrawing()};
        }
    }
}
