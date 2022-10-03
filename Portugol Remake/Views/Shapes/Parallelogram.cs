using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Portugol_Remake.Views.Shapes
{
    public class Parallelogram : Shape
    {
        protected override Geometry DefiningGeometry
        {
            get
            {
                //Points = "0.5,34.5, 74.5,34.5, 99.5,0.5, 25,0.5"
                var offset = StrokeThickness / 2;
                Point p1 = new Point(offset, Height - offset);
                Point p2 = new Point(Width - (Width / 4) - offset, Height - offset);
                Point p3 = new Point(Width-offset, offset);
                Point p4 = new Point(Width / 4, offset);

                List<PathSegment> segments = new List<PathSegment>();
                segments.Add(new LineSegment(p1, true));
                segments.Add(new LineSegment(p2, true));
                segments.Add(new LineSegment(p3, true));
                segments.Add(new LineSegment(p4, true));

                List<PathFigure> figures = new List<PathFigure>(1);
                PathFigure pf = new PathFigure(p1, segments, true);
                figures.Add(pf);

                Geometry g = new PathGeometry(figures, FillRule.EvenOdd, null);
                return g;
            }
        }
    }
}
