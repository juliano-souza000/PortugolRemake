using Portugol_Remake.Utils;
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
    class Arrow : Shape
    {
        public Position ToPosition { get; set; }

        public Arrow(Position toPosition)
        {
            ToPosition = toPosition;
        }

        public Geometry Geometry { get { return DefiningGeometry; } }

        protected override Geometry DefiningGeometry
        {
            get
            {
                Point p1 = new Point();
                Point p2 = new Point();
                Point p3 = new Point();
                Point p4 = new Point();
                var offset = StrokeThickness / 2;
                switch (ToPosition)
                {
                    case Position.Top:
                        p1 = new Point(0, 0);
                        p2 = new Point(Width, 0);
                        p3 = new Point(Width / 2, Height / 2);
                        p4 = new Point(-0.003, 0);
                        break;
                    case Position.Bottom:
                        p1 = new Point(0, Height);
                        p2 = new Point(0, 0);
                        p3 = new Point(0, Height / 2);
                        p4 = new Point(0, Height);
                        break;
                    case Position.Left:
                        p1 = new Point(0, 0);
                        p2 = new Point(0, Height);
                        p3 = new Point(Width / 2, Height / 2);
                        p4 = new Point(0, -0.003);
                        break;
                    case Position.Right:
                        p1 = new Point(0, Height / 2);
                        p2 = new Point(Width / 2, 0);
                        p3 = new Point(Width / 2, Height);
                        p4 = new Point(0, Height / 2);
                        break;
                }

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
