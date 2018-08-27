/////////////////////////////////////////////////////////////
// "static class myShapes"
//
// provides shapes, that can be used to draw graphs
// - cross (size, pos X, pos Y)
// - phi (length, pos X, pos Y)
//////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Controls;

namespace General.Tools.MyShapes
{
    public static class myShapes
    {
    
        public static Canvas myCross(int size, int posX, int posY)
        {
            if(size%2==0)
                size+=1;
            Canvas myCanvas = new Canvas();
            myCanvas.Height = size;
            myCanvas.Width = size;
            myCanvas.Margin = new System.Windows.Thickness(posX, posY, 0, 0 );
            Line myLine = new Line();
            myLine.Stroke = System.Windows.Media.Brushes.Yellow;
            myLine.X1 = (size+1)/2;
            myLine.X2 = (size+1)/2;
            myLine.Y1 = 1;
            myLine.Y2 = size;
            myLine.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            myLine.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            myLine.StrokeThickness = 0.5;
            myCanvas.Children.Add(myLine);
            Line myLine2 = new Line();
            myLine2.Stroke = System.Windows.Media.Brushes.Yellow;
            myLine2.X1 = 1;
            myLine2.X2 = size;
            myLine2.Y1 = (size+1)/2;
            myLine2.Y2 = (size+1)/2;
            myLine2.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            myLine2.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            myLine2.StrokeThickness = 0.5;
            myCanvas.Children.Add(myLine2);
           
            return myCanvas;
        }

        public static Canvas myCross(int size, int posX, int posY, string color)
        {
            var converter = new System.Windows.Media.BrushConverter();
            var brush = (System.Windows.Media.Brush)converter.ConvertFromString(color);
            if (size % 2 == 0)
                size += 1;
            Canvas myCanvas = new Canvas();
            myCanvas.Height = size;
            myCanvas.Width = size;
            myCanvas.Margin = new System.Windows.Thickness(posX, posY, 0, 0);
            Line myLine = new Line();
            myLine.Stroke = brush;
            myLine.X1 = (size + 1) / 2;
            myLine.X2 = (size + 1) / 2;
            myLine.Y1 = 1;
            myLine.Y2 = size;
            myLine.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            myLine.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            myLine.StrokeThickness = 0.5;
            myCanvas.Children.Add(myLine);
            Line myLine2 = new Line();
            myLine2.Stroke = brush;
            myLine2.X1 = 1;
            myLine2.X2 = size;
            myLine2.Y1 = (size + 1) / 2;
            myLine2.Y2 = (size + 1) / 2;
            myLine2.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            myLine2.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            myLine2.StrokeThickness = 0.5;
            myCanvas.Children.Add(myLine2);

            return myCanvas;
        }

        public static Canvas myPhi(int length, int posX, int posY, string tooltip)
        {
            
            Canvas myCanvas = new Canvas();
            myCanvas.Height = (length*2)+1;
            myCanvas.Width = 5;
            myCanvas.Margin = new System.Windows.Thickness(posX, posY, 0, 0);
            myCanvas.ToolTip = tooltip;
            Ellipse myEllipse = new Ellipse();
            myEllipse.Stroke = System.Windows.Media.Brushes.Black;
            myEllipse.Fill = System.Windows.Media.Brushes.Blue;
            myEllipse.Width = 4;
            myEllipse.Height = 4;
            myEllipse.StrokeThickness = 0;
            myEllipse.Margin = new System.Windows.Thickness(1, length-1, 0, 0);
            
            myCanvas.Children.Add(myEllipse);
            
            Line myLine = new Line();
            myLine.Stroke = System.Windows.Media.Brushes.Blue;
            myLine.X1 = 1;
            myLine.X2 = 5;
            myLine.Y1 = 1;
            myLine.Y2 = 1;
            myLine.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            myLine.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            myLine.StrokeThickness = 1;
            myCanvas.Children.Add(myLine);
            Line myLine2 = new Line();
            myLine2.Stroke = System.Windows.Media.Brushes.Blue;
            myLine2.X1 = 1;
            myLine2.X2 = 5;
            myLine2.Y1 = length*2+1;
            myLine2.Y2 = length * 2 + 1;
            myLine2.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            myLine2.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            myLine2.StrokeThickness = 1;
            myCanvas.Children.Add(myLine2);
            Line myLine3 = new Line();
            myLine3.Stroke = System.Windows.Media.Brushes.Blue;
            myLine3.X1 = 3;
            myLine3.X2 = 3;
            myLine3.Y1 = length * 2 + 1;
            myLine3.Y2 = 1;
            myLine3.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            myLine3.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            myLine3.StrokeThickness = 1;
            myCanvas.Children.Add(myLine3);

            return myCanvas;
        }

        public static Canvas myPhi(int length, int posX, int posY, string color, string tooltip)
        {

            var converter = new System.Windows.Media.BrushConverter();
            var brush = (System.Windows.Media.Brush)converter.ConvertFromString(color);
            Canvas myCanvas = new Canvas();
            myCanvas.Height = (length * 2) + 1;
            myCanvas.Width = 5;
            myCanvas.Margin = new System.Windows.Thickness(posX, posY, 0, 0);
            myCanvas.ToolTip = tooltip;
            Ellipse myEllipse = new Ellipse();
            myEllipse.Stroke = System.Windows.Media.Brushes.Black;
            myEllipse.Fill = brush;
            myEllipse.Width = 4;
            myEllipse.Height = 4;
            myEllipse.StrokeThickness = 0;
            myEllipse.Margin = new System.Windows.Thickness(1, length - 1, 0, 0);

            myCanvas.Children.Add(myEllipse);

            Line myLine = new Line();
            myLine.Stroke = brush;
            myLine.X1 = 1;
            myLine.X2 = 5;
            myLine.Y1 = 1;
            myLine.Y2 = 1;
            myLine.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            myLine.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            myLine.StrokeThickness = 1;
            myCanvas.Children.Add(myLine);
            Line myLine2 = new Line();
            myLine2.Stroke = brush;
            myLine2.X1 = 1;
            myLine2.X2 = 5;
            myLine2.Y1 = length * 2 + 1;
            myLine2.Y2 = length * 2 + 1;
            myLine2.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            myLine2.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            myLine2.StrokeThickness = 1;
            myCanvas.Children.Add(myLine2);
            Line myLine3 = new Line();
            myLine3.Stroke = brush;
            myLine3.X1 = 3;
            myLine3.X2 = 3;
            myLine3.Y1 = length * 2 + 1;
            myLine3.Y2 = 1;
            myLine3.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            myLine3.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            myLine3.StrokeThickness = 1;
            myCanvas.Children.Add(myLine3);

            return myCanvas;
        }
    }
}
