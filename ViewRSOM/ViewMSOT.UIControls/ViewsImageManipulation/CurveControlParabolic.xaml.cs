using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ViewMSOT.UIControls
{
    /// <summary>
    /// Interaction logic for CurveControlParabolic.xaml
    /// </summary>
    public partial class CurveControlParabolic : UserControl
    {
        public CurveControlParabolic()
        {
            InitializeComponent();
        }
#region localVariables
        int _width;
        int _height;
#endregion

#region properties

        public Point Cp1
        {
            get { return (Point)GetValue(Cp1Property); }
            set
            {
                SetValue(Cp1Property, (Point)value);
            }
        }

        public Point Cp2
        {
            get { return (Point)GetValue(Cp2Property); }
            set
            {
                SetValue(Cp2Property, (Point)value);
            }
        }

        public static readonly DependencyProperty Cp1Property =
            DependencyProperty.Register(
             "Cp1",
             typeof(Point),
             typeof(CurveControlParabolic),
            new FrameworkPropertyMetadata(
                    new PropertyChangedCallback(Cp1Changed)));

        private static void Cp1Changed(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            CurveControlParabolic control = null;
            try
            {
                control = source as CurveControlParabolic;
                control.updatePoint(control.cp1Thumb, control.Cp1.X, control.Cp1.Y);
            }
            catch
            {
                if(control != null)
                    control.Cp1 = new Point(0, 0);
            }
        }

        public static readonly DependencyProperty Cp2Property =
        DependencyProperty.Register(
         "Cp2",
         typeof(Point),
         typeof(CurveControlParabolic),
        new FrameworkPropertyMetadata(
                new PropertyChangedCallback(Cp2Changed)));

        private static void Cp2Changed(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            CurveControlParabolic control = null;
            try
            {
                control = source as CurveControlParabolic;
                control.updatePoint(control.cp2Thumb, control.Cp2.X, control.Cp2.Y);
                control.updateLine();
            }
            catch
            {
                if(control != null)
                    control.Cp2 = new Point(1, 1);
            }
        }

        public IEnumerable<int> CurvePoints
        {
            get { return (IEnumerable<int>)GetValue(CurvePointsProperty); }
            set { SetValue(CurvePointsProperty, (IEnumerable<int>)value); }
        }

        public static readonly DependencyProperty CurvePointsProperty =
        DependencyProperty.Register(
         "CurvePoints",
         typeof(IEnumerable<int>),
         typeof(CurveControlParabolic),
        new FrameworkPropertyMetadata(
                                new PropertyChangedCallback(CurvePointsPropertyChanged)));

        private static void CurvePointsPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                CurveControlParabolic gainControl = source as CurveControlParabolic;
                gainControl.updateCurve();                
            }
            catch
            {
                //(source as ViewChamberControl).ScanStart = 0;
            }
        }
#endregion proprties

        void updatePoint(Ellipse thumb,double cpX, double cpY)
        {
            try
            {
                //if (thumb == cp1Thumb)
                //    thumb.ToolTip = cpX.ToString("F2");
                //else
                //    thumb.ToolTip = cpX.ToString("F2") + " , " + cpY.ToString("F2");
                thumb.ToolTip = cpY.ToString("F2");
                cpX *= _width; cpY *= _height;
                if (_width > 0)
                    Canvas.SetLeft(thumb, cpX - thumb.Width / 2);
                if (_height > 0)
                    Canvas.SetTop(thumb, (_height - cpY) - thumb.Height / 2);
            }
            catch { }
        }


        Point scalePoint(int i, double dx, double y)
        {
            return new Point(i* dx, y * _height);
        }

        void updateCurve()
        {
            try
            {
                if (CurvePoints == null) return;
                int pixelbitrange = CurvePoints.Count() - 1;
                if (pixelbitrange > 0)
                {
                    float dx = (float)_width / (float)pixelbitrange;
                    PathFigure paths = new PathFigure();
                    float y0 = (float)CurvePoints.ElementAt(0) / (float)pixelbitrange * _height;
                    paths.StartPoint = new Point(0, _height - y0);
                    //float step = (float)1.0 / _width;
                    for (int i = 0; i <= pixelbitrange; i++)
                    {
                        float yi = (float)CurvePoints.ElementAt(i) / (float)pixelbitrange * _height;
                        paths.Segments.Add(new LineSegment(new Point((float)i * dx, _height - yi), true));
                    }
                    PathGeometry geom = new PathGeometry();
                    geom.Figures.Add(paths);
                    pathCurve.Data = geom;
                }
            }
            catch { }
        }

        void getValidatedPosition(object sender, MouseEventArgs e, out double cpX, out double cpY)
        {
            Point lastP = e.GetPosition(curveCanvas);
            cpX = lastP.X;
            cpY = lastP.Y;
            if (cpX < 0) cpX = 0;
            if (cpX > _width) cpX = _width;
            if (cpY < 0) cpY = 0;
            if (cpY > _height) cpY = _height;
            if (sender == cp1Thumb)
            {
                double cp2Y = _height - Cp2.Y * _height;
                if (cpY < cp2Y)
                    cpY = cp2Y;
                if (cpY == 0)
                    cpY = 0.01 * _height;
            }
            else // cp2Thumb
            {
                double cp1Y = _height - Cp1.Y * _height;
                if (cpY > cp1Y)
                    cpY = cp1Y;
            }
            cpX = _width / 2;
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _width = (int) curveCanvas.ActualWidth;
            _height = (int)curveCanvas.ActualHeight;
            updatePoint(cp1Thumb,Cp1.X,Cp1.Y);
            updatePoint(cp2Thumb,Cp2.X, Cp2.Y);
            updateCurve();
            updateLine();
        }

        void cpThumb_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                UIElement a = (UIElement)sender;
                if (a.IsMouseCaptured)
                {
                    double cpX, cpY;
                    Ellipse thumb = sender as Ellipse;
                    getValidatedPosition(sender, e, out cpX, out cpY);
                    double xVal = cpX / _width;
                    double yVal = (_height - cpY) / _height;
                    updatePoint(thumb, xVal, yVal);
                }
            }
            catch { }
            e.Handled = true;
        }

        void cpThumb_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                double cpX, cpY;
                getValidatedPosition(sender, e, out cpX, out cpY);
                Ellipse thumb = sender as Ellipse;
                //updatePoint(thumb,cpX,cpY);
                if (thumb == cp1Thumb)
                {
                    Cp1 = new Point(cpX / _width, (_height - cpY) / _height);
                }
                if (thumb == cp2Thumb)
                {
                    Cp2 = new Point(cpX / _width, (_height - cpY) / _height);
                }
            }
            catch { }
            UIElement a = (UIElement)sender;
            a.ReleaseMouseCapture();
            e.Handled = true;
        }

        void cpThumb_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UIElement a = (UIElement)sender;
            a.CaptureMouse();
            e.Handled = true;
        }

        private void updateLine()
        {
            topLine.Y1 = topLine.Y2 = (int)(_height - Cp2.Y * _height);
        }
    }
}
