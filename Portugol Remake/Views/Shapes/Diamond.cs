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
    public class Diamond : Shape
    {
        protected override Geometry DefiningGeometry
        {
            get
            {
                Point p1 = new Point(0, Height / 2);
                Point p2 = new Point(Width / 2, Height);
                Point p3 = new Point(Width, Height / 2);
                Point p4 = new Point(Width / 2, 0);
                Point p5 = new Point(0, Height / 2);

                List<PathSegment> segments = new List<PathSegment>();
                segments.Add(new LineSegment(p1, true));
                segments.Add(new LineSegment(p2, true));
                segments.Add(new LineSegment(p3, true));
                segments.Add(new LineSegment(p4, true));
                segments.Add(new LineSegment(p5, true));


                List<PathFigure> figures = new List<PathFigure>(1);
                PathFigure pf = new PathFigure(p1, segments, true);
                figures.Add(pf);

                Geometry g = new PathGeometry(figures, FillRule.EvenOdd, null);
                
                return g;
            }
        }
    }
}
