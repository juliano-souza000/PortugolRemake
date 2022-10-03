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
    class Line : Shape
    {
        public double X1 { get; set; }
        public double Y1 { get; set; }
        public double X2 { get; set; }
        public double Y2 { get; set; }
        private List<Point> Points { get; set; }
        public Position StartPosition { get; set; }
        public Position EndPosition { get; set; }

        protected override Geometry DefiningGeometry
        {
            get
            {
                GeometryGroup geometryGroup = new GeometryGroup();
                Arrow lineEnd = new Arrow(EndPosition)
                {
                    VerticalAlignment = System.Windows.VerticalAlignment.Top,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                    Width = 10,
                    Height = 10,
                };

                Points = CalculatePoints();
                PathGeometry pathGeometry = new PathGeometry();
                PathFigure pathFigure = new PathFigure();
                PolyLineSegment polyLineSegments = new PolyLineSegment();
                PathSegmentCollection pathSegmentCollection = new PathSegmentCollection();
                PathFigureCollection pathFigureCollection = new PathFigureCollection();

                pathFigure.StartPoint = Points.FirstOrDefault();
                pathFigure.IsFilled = false;


                for (int i = 0; i < Points.Count; i++)
                {
                    polyLineSegments.Points.Add(Points[i]);
                }

                pathSegmentCollection.Add(polyLineSegments);
                pathFigure.Segments = pathSegmentCollection;
                pathFigureCollection.Add(pathFigure);
                pathGeometry.Figures = pathFigureCollection;

                geometryGroup.Children.Add(pathGeometry);

                switch (EndPosition)
                {
                    case Position.Top:
                        lineEnd.Margin = new Thickness(Points.Last().X - 5, Points.Last().Y, 0, 0);
                        break;
                    case Position.Bottom:
                        lineEnd.Margin = new Thickness(Points.Last().X - 5, Points.Last().Y + 5, 0, 0);
                        break;
                    case Position.Left:
                        lineEnd.Margin = new Thickness(Points.Last().X, Points.Last().Y - 5, 0, 0);
                        break;
                    case Position.Right:
                        lineEnd.Margin = new Thickness(Points.Last().X, Points.Last().Y - 5, 0, 0);
                        break;
                }

                var arrowGeometry = lineEnd.Geometry.Clone();
                arrowGeometry.Transform = new TranslateTransform(lineEnd.Margin.Left, lineEnd.Margin.Top);

                geometryGroup.Children.Add(arrowGeometry);
                return geometryGroup;
            }
        }

        private List<Point> CalculatePoints()
        {
            List<Point> points = new List<Point>();
            double offset = StrokeThickness / 2;
            double margin = 5;
            points.Add(new Point(X1, Y1));
            switch(StartPosition)
            {
                case Position.Top:
                    points.Add(new Point(X1, Y1 - margin));
                    break;
                case Position.Bottom:
                    points.Add(new Point(X1, Y1 + margin));
                    break;
                case Position.Left:
                    points.Add(new Point(X1 - margin, Y1));
                    break;
                case Position.Right:
                    points.Add(new Point(X1 + margin, Y1));
                    break;
            }

            switch (EndPosition)
            {
                case Position.Top:
                    if ((StartPosition != Position.Left && StartPosition != Position.Right))
                    {
                        if (Y2 - margin >= points.Last().Y)
                        {
                            points.Add(new Point(points.Last().X, Y2 - margin));
                            points.Add(new Point(X2, Y2 - margin));
                        }
                        else
                        {
                            points.Add(new Point(X2, points.Last().Y));
                            points.Add(new Point(X2, Y2 - margin));
                        }
                    }
                    else
                    {
                        if (Y2 - margin > points.Last().Y)
                        {
                            if ((X2 > points.Last().X && StartPosition == Position.Left) || (X2 < points.Last().X && StartPosition == Position.Right))
                            {
                                points.Add(new Point(points.Last().X, Y2 - margin));
                                points.Add(new Point(X2, Y2 - margin));
                            }
                            else
                            {
                                points.Add(new Point(X2, points.Last().Y));
                                points.Add(new Point(X2, Y2 - margin));
                            }
                        }
                        else
                        {
                            points.Add(new Point(points.Last().X, Y2 - margin));
                            points.Add(new Point(X2, Y2 - 2));
                        }
                    }
                    points.Add(new Point(X2, Y2 - 2));
                    break;
                case Position.Bottom:
                    if (Y2 - margin > Y1 )
                    {
                        points.Add(new Point(points.Last().X, Y2 + margin));
                        points.Add(new Point(X2, Y2 + margin));
                    }
                    else
                    {
                        points.Add(new Point(X2, points.Last().Y));
                        points.Add(new Point(X2, Y2 + margin));
                    }
                    break;
                case Position.Left:
                    if (Y2 - margin > Y1)
                    {
                        points.Add(new Point(points.Last().X, Y2));
                        points.Add(new Point(X2 - margin, Y2));
                    }
                    else
                    {
                        points.Add(new Point(X2 - margin, points.Last().Y));
                        points.Add(new Point(X2 - margin, Y2));
                    }
                    break;
                case Position.Right:
                    if (Y2 - margin > Y1)
                    {
                        points.Add(new Point(points.Last().X, Y2));
                        points.Add(new Point(X2, Y2));
                    }
                    else
                    {
                        points.Add(new Point(X2, points.Last().Y));
                        points.Add(new Point(X2, Y2));
                    }
                    break;
            }

            //points.Add(new Point(X2, Y2));

            //switch (StartPosition)
            //{
            //    case Position.Top:
            //        points.Add(new Point(X1, Y1 - 5));
            //        if (X2 - 7 > X1)
            //        {
            //            if (Y2 >= Y1 + 5)
            //            {
            //                switch (EndPosition)
            //                {
            //                    case Position.Left:
            //                        points.Add(new Point(X1, Y2));
            //                        points.Add(new Point(X1 - offset, Y2));
            //                        points.Add(new Point(X2 - 7, Y2));
            //                        points.Add(new Point(X2 - 4, Y2));
            //                        break;
            //                }
            //            }
            //            else
            //            {
            //                switch (EndPosition)
            //                {
            //                    case Position.Left:
            //                        points.Add(new Point(X1 - offset, Y1 - 5));
            //                        points.Add(new Point(X2 - 7, Y1 - 5));
            //                        points.Add(new Point(X2 - 7, Y1 - 5 + offset));
            //                        points.Add(new Point(X2 - 7, Y2));
            //                        points.Add(new Point(X2 - 7 - offset, Y2));
            //                        points.Add(new Point(X2 - 4, Y2));
            //                        break;
            //                }
            //            }
            //        }
            //        else
            //        {
            //            if (Y2 >= Y1 + 5)
            //            {
            //                switch (EndPosition)
            //                {
            //                    case Position.Left:
            //                        points.Add(new Point(X1, Y1 - 5));
            //                        points.Add(new Point(X1 + offset, Y1 - 5));
            //                        points.Add(new Point(X2 - 7, Y1 - 5));
            //                        points.Add(new Point(X2 - 7, Y1 - 5 - offset));
            //                        points.Add(new Point(X2 - 7, Y2));
            //                        points.Add(new Point(X2 - 7 - offset, Y2));
            //                        points.Add(new Point(X2 - 2, Y2));
            //                        break;
            //                }
            //            }
            //            else
            //            {
            //                switch (EndPosition)
            //                {
            //                    case Position.Left:
            //                        points.Add(new Point(X1, Y1 - 5));
            //                        points.Add(new Point(X1 + offset, Y1 - 5));
            //                        points.Add(new Point(X2 - 7, Y1 - 5));
            //                        points.Add(new Point(X2 - 7, Y1 - 5 + offset));
            //                        points.Add(new Point(X2 - 7, Y2));
            //                        points.Add(new Point(X2 - 7 - offset, Y2));
            //                        points.Add(new Point(X2 - 2, Y2));
            //                        break;
            //                }
            //            }
            //        }
            //        break;
            //    case Position.Bottom:
            //        points.Add(new Point(X1, Y1 + 5));
            //        if (X2 + 7 > X1)
            //        {
            //            if (Y2 >= Y1 + 5)
            //            {
            //                switch (EndPosition)
            //                {
            //                    case Position.Top:
            //                        break;
            //                    case Position.Left:
            //                        points.Add(new Point(X1, Y2));
            //                        points.Add(new Point(X1 - offset, Y2));
            //                        points.Add(new Point(X2 - 7, Y2));
            //                        points.Add(new Point(X2 - 4, Y2));
            //                        break;
            //                    case Position.Right:
            //                        points.Add(new Point(X1, Y1 + 5));
            //                        points.Add(new Point(X1 + offset, Y1 + 5));
            //                        points.Add(new Point(X2 + 7, Y1 + 5));
            //                        points.Add(new Point(X2 + 7, Y1 + 5 + offset));
            //                        points.Add(new Point(X2 + 7, Y2));
            //                        points.Add(new Point(X2 + 7 + offset, Y2));
            //                        points.Add(new Point(X2 - 1, Y2));
            //                        break;

            //                }
            //            }
            //            else
            //            {
            //                switch (EndPosition)
            //                {
            //                    case Position.Left:
            //                        points.Add(new Point(X1 - offset, Y1 + 5));
            //                        points.Add(new Point(X2 - 7, Y1 + 5));
            //                        points.Add(new Point(X2 - 7, Y1 + 5 + offset));
            //                        points.Add(new Point(X2 - 7, Y2));
            //                        points.Add(new Point(X2 - 7 - offset, Y2));
            //                        points.Add(new Point(X2 - 4, Y2));
            //                        break;
            //                    case Position.Right:
            //                        points.Add(new Point(X1, Y1 + 5));
            //                        points.Add(new Point(X1 + offset, Y1 + 5));
            //                        points.Add(new Point(X2 + 7, Y1 + 5));
            //                        points.Add(new Point(X2 + 7, Y1 + 5 + offset));
            //                        points.Add(new Point(X2 + 7, Y2));
            //                        points.Add(new Point(X2 + 7 + offset, Y2));
            //                        points.Add(new Point(X2 - 1, Y2));
            //                        break;
            //                }
            //            }
            //        }
            //        else
            //        {
            //            if (Y2 >= Y1 + 5)
            //            {
            //                switch (EndPosition)
            //                {
            //                    case Position.Left:
            //                        points.Add(new Point(X1, Y1 + 5));
            //                        points.Add(new Point(X1 + offset, Y1 + 5));
            //                        points.Add(new Point(X2 - 7, Y1 + 5));
            //                        points.Add(new Point(X2 - 7, Y1 + 5 - offset));
            //                        points.Add(new Point(X2 - 7, Y2));
            //                        points.Add(new Point(X2 - 7 - offset, Y2));
            //                        points.Add(new Point(X2 - 2, Y2));
            //                        break;
            //                    case Position.Right:
            //                        if (X2 + 7 > X1)
            //                        {
            //                            points.Add(new Point(X1, Y1 + 5));
            //                            points.Add(new Point(X1 + offset, Y1 + 5));
            //                            points.Add(new Point(X2 + 7, Y1 + 5));
            //                            points.Add(new Point(X2 + 7, Y1 + 5 + offset));
            //                            points.Add(new Point(X2 + 7, Y2));
            //                            points.Add(new Point(X2 + 7 + offset, Y2));
            //                            points.Add(new Point(X2 - 1, Y2));
            //                        }
            //                        else
            //                        {
            //                            points.Add(new Point(X1, Y2));
            //                            points.Add(new Point(X1 + offset, Y2));
            //                            points.Add(new Point(X2 + 7, Y2));
            //                            points.Add(new Point(X2 - 1, Y2));
            //                        }
            //                        break;
            //                }
            //            }
            //            else
            //            {
            //                switch (EndPosition)
            //                {
            //                    case Position.Left:
            //                        points.Add(new Point(X1, Y1 + 5));
            //                        points.Add(new Point(X1 + offset, Y1 + 5));
            //                        points.Add(new Point(X2 - 7, Y1 + 5));
            //                        points.Add(new Point(X2 - 7, Y1 + 5 + offset));
            //                        points.Add(new Point(X2 - 7, Y2));
            //                        points.Add(new Point(X2 - 7 - offset, Y2));
            //                        points.Add(new Point(X2 - 2, Y2));
            //                        break;
            //                    case Position.Right:
            //                        points.Add(new Point(X1, Y1 + 5));
            //                        points.Add(new Point(X1 + offset, Y1 + 5));
            //                        points.Add(new Point(X2 + 7, Y1 + 5));
            //                        points.Add(new Point(X2 + 7, Y1 + 5 + offset));
            //                        points.Add(new Point(X2 + 7, Y2));
            //                        points.Add(new Point(X2 + 7 + offset, Y2));
            //                        points.Add(new Point(X2 - 1, Y2));
            //                        break;
            //                }
            //            }
            //        }
            //        break;
            //    case Position.Left:
            //        points.Add(new Point(X1 - 5, Y1));
            //        if (X2 > X1 - 5)
            //        {
            //            if (Y2 - 6 >= Y1)
            //            {
            //                switch (EndPosition)
            //                {
            //                    case Position.Top:
            //                        points.Add(new Point(X1 - 5, Y1 - offset));
            //                        points.Add(new Point(X1 - 5, Y2 - 6));
            //                        points.Add(new Point(X1 - 5 - offset, Y2 - 6));
            //                        points.Add(new Point(X2, Y2 - 6));
            //                        points.Add(new Point(X2, Y2 - 6 - offset));
            //                        points.Add(new Point(X2, Y2 - 1));
            //                        break;
            //                }
            //            }
            //            else
            //            {
            //                switch (EndPosition)
            //                {
            //                    case Position.Top:
            //                        points.Add(new Point(X1 - 5, Y1 + offset));
            //                        points.Add(new Point(X1 - 5, Y2 - 6));
            //                        points.Add(new Point(X1 - 5 - offset, Y2 - 6));
            //                        points.Add(new Point(X2, Y2 - 6));
            //                        points.Add(new Point(X2, Y2 - 6 - offset));
            //                        points.Add(new Point(X2, Y2 - 1));
            //                        break;
            //                }
            //            }
            //        }
            //        else
            //        {
            //            if (Y2 - 6 >= Y1)
            //            {
            //                switch (EndPosition)
            //                {
            //                    case Position.Top:
            //                        points.Add(new Point(X1 - 5, Y1));
            //                        //points.Add(new Point(X1 - 5, Y1 - offset));
            //                        points.Add(new Point(X2, Y1));
            //                        points.Add(new Point(X2, Y1 - offset));
            //                        points.Add(new Point(X2, Y2 - 1));
            //                        break;
            //                }
            //            }
            //            else
            //            {
            //                switch (EndPosition)
            //                {
            //                    case Position.Top:
            //                        points.Add(new Point(X1 - 5, Y1 + offset));
            //                        points.Add(new Point(X1 - 5, Y2 - 6));
            //                        points.Add(new Point(X1 - 5 + offset, Y2 - 6));
            //                        points.Add(new Point(X2, Y2 - 6));
            //                        points.Add(new Point(X2, Y2 - 6 - offset));
            //                        points.Add(new Point(X2, Y2 - 1));
            //                        break;
            //                }
            //            }
            //        }
            //        break;
            //    case Position.Right:
            //        points.Add(new Point(X1 + 5, Y1));
            //        if (X2 > X1 + 5)
            //        {
            //            if (Y2 - 6 >= Y1)
            //            {
            //                switch (EndPosition)
            //                {
            //                    case Position.Top:
            //                        points.Add(new Point(X1 + 5, Y1));
            //                        //points.Add(new Point(X1 - 5, Y1 - offset));
            //                        points.Add(new Point(X2, Y1));
            //                        points.Add(new Point(X2, Y1 - offset));
            //                        points.Add(new Point(X2, Y2 - 1));
            //                        break;
            //                }
            //            }
            //            else
            //            {
            //                switch (EndPosition)
            //                {
            //                    case Position.Top:
            //                        points.Add(new Point(X1 + 5, Y1 + offset));
            //                        points.Add(new Point(X1 + 5, Y2 - 6));
            //                        points.Add(new Point(X1 + 5 - offset, Y2 - 6));
            //                        points.Add(new Point(X2, Y2 - 6));
            //                        points.Add(new Point(X2, Y2 - 6 - offset));
            //                        points.Add(new Point(X2, Y2 - 1));
            //                        break;
            //                }
            //            }
            //        }
            //        else
            //        {
            //            if (Y2 - 6 >= Y1)
            //            {
            //                switch (EndPosition)
            //                {
            //                    case Position.Top:
            //                        points.Add(new Point(X1 + 5, Y1 - offset));
            //                        points.Add(new Point(X1 + 5, Y2 - 6));
            //                        points.Add(new Point(X1 + 5 + offset, Y2 - 6));
            //                        points.Add(new Point(X2, Y2 - 6));
            //                        points.Add(new Point(X2, Y2 - 6 - offset));
            //                        points.Add(new Point(X2, Y2 - 1));
            //                        break;
            //                }
            //            }
            //            else
            //            {
            //                switch (EndPosition)
            //                {
            //                    case Position.Top:
            //                        points.Add(new Point(X1 + 5, Y1 + offset));
            //                        points.Add(new Point(X1 + 5, Y2 - 6));
            //                        points.Add(new Point(X1 + 5 + offset, Y2 - 6));
            //                        points.Add(new Point(X2, Y2 - 6));
            //                        points.Add(new Point(X2, Y2 - 6 - offset));
            //                        points.Add(new Point(X2, Y2 - 1));
            //                        break;
            //                }
            //            }
            //        }
            //        break;
            //}
            return points;
        }
    }
}
