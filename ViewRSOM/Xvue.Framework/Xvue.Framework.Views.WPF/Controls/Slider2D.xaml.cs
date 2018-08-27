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

namespace Xvue.Framework.Views.WPF.Controls
{
    /// <summary>
    /// Interaction logic for Slider2D.xaml Slider 2D offers a 2-dimensional control pad to control 2 values
    /// in an interactive way by dragging the mouse
    /// </summary>
    public partial class Slider2D : UserControl
    {
        #region localVariables
        int _width;
        int _height;
        //bool _loaded = false;        

        //SolidColorBrush controlBrush = new SolidColorBrush(Colors.Yellow);
        #endregion localVariables

        public Slider2D()
        {
            InitializeComponent();
        }

        #region properties
        public double Y
        {
            get 
            { 
                return (double)GetValue(YProperty); 
            }
            set
            {               
                SetValue(YProperty, (double)value);
            }
        }

        public double X
        {
            get 
            {
                return (double)GetValue(XProperty); 
            }
            set
            {                      
                SetValue(XProperty, (double)value);
            }
        }

        public double MinY
        {
            get { return (double)GetValue(MinYProperty); }
            set
            {
                SetValue(MinYProperty, (double)value);
            }
        }

        public double MaxY
        {
            get { return (double)GetValue(MaxYProperty); }
            set
            {
                SetValue(MaxYProperty, (double)value);
            }
        }

        public double MinX
        {
            get { return (double)GetValue(MinXProperty); }
            set { SetValue(MinXProperty, (double)value); }
        }
        public double MaxX
        {
            get { return (double)GetValue(MaxXProperty); }
            set { SetValue(MaxXProperty, (double)value); }
        }

        public static readonly DependencyProperty YProperty =
        DependencyProperty.Register(
        "Y",
        typeof(double),
        typeof(Slider2D),
        new FrameworkPropertyMetadata(
        new PropertyChangedCallback(YChanged)));

        private static void YChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            Slider2D slider = null;
            try
            {
                slider = source as Slider2D;
                slider.Y = (double)e.NewValue;
            }
            catch
            {
                if(slider != null)
                    slider.Y = 0;
            }
        }

        public static readonly DependencyProperty XProperty =
        DependencyProperty.Register(
        "X",
        typeof(double),
        typeof(Slider2D),
        new FrameworkPropertyMetadata(
        new PropertyChangedCallback(XChanged)));

        private static void XChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            Slider2D slider = null;
            try
            {
                slider = source as Slider2D;
                slider.X = (double)e.NewValue;
            }
            catch
            {
                if (slider != null)
                    slider.X = 0;
            }
        }


        public static readonly DependencyProperty MinXProperty =
        DependencyProperty.Register(
         "MinX",
         typeof(double),
         typeof(Slider2D),
        new FrameworkPropertyMetadata(
                new PropertyChangedCallback(MinXChanged)));

        private static void MinXChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            Slider2D slider = null;
            try
            {
                slider = source as Slider2D;
                slider.MinX = (double)e.NewValue;
            }
            catch
            {
                if (slider != null)
                    slider.MinX = 0;
            }
        }

        public static readonly DependencyProperty MaxXProperty =
        DependencyProperty.Register(
         "MaxX",
         typeof(double),
         typeof(Slider2D),
         new FrameworkPropertyMetadata(
                new PropertyChangedCallback(MaxXChanged)));

        private static void MaxXChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            Slider2D slider = null;
            try
            {
                slider = source as Slider2D;
                slider.MaxX = (double)e.NewValue;
            }
            catch
            {
                if (slider != null)
                    slider.MaxX = 0;
            }
        }

        public static readonly DependencyProperty MinYProperty =
        DependencyProperty.Register(
         "MinY",
         typeof(double),
         typeof(Slider2D),
         new FrameworkPropertyMetadata(
                new PropertyChangedCallback(MinYChanged)));

        private static void MinYChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            Slider2D slider = null;
            try
            {
                slider = source as Slider2D;
                slider.MinY = (double)e.NewValue;
            }
            catch
            {
                if (slider != null)
                    slider.MinY = 0;
            }
        }

        public static readonly DependencyProperty MaxYProperty =
        DependencyProperty.Register(
         "MaxY",
         typeof(double),
         typeof(Slider2D),
         new FrameworkPropertyMetadata(
                new PropertyChangedCallback(MaxYChanged)));

        private static void MaxYChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            Slider2D slider = null;
            try
            {
                slider = source as Slider2D;
                slider.MaxY = (double)e.NewValue;
            }
            catch
            {
                if (slider != null)
                    slider.MaxY = 0;
            }
        }
        #endregion properties


        //private void initCurve()
        //{
        //    if (!_loaded)
        //    {
        //        _width = (int)slider2DCanvas.ActualWidth;
        //        _height = (int)slider2DCanvas.ActualHeight;

        //        _loaded = true;
        //    }
        //}

        void cpThumb_PreviewMouseMove(object sender, MouseEventArgs e)
        {
             UIElement a = (UIElement)sender;
            if (a.IsMouseCaptured)
            {
                Point lastP = e.GetPosition(slider2DCanvas);
                double cpX = lastP.X;
                double cpY = lastP.Y;
                if (cpX < 0) cpX = 0;
                if (cpX > _width) cpX = _width;
                if (cpY < 0) cpY = 0;
                if (cpY > _height) cpY = _height;
                updatePoint(cpX, cpY);
            }
            e.Handled = true;
        }

        void updateProperties(double cpX, double cpY)
        {
            if (_width > 0)
            {
                X = MinX + cpX / _width * (MaxX - MinX);
            }
            if (_height > 0)
            {
                Y = MinY + (_height - cpY) / _height * (MaxY - MinY);
            }
        }

        void updatePoint(double cpX, double cpY)
        {
            if (_width > 0)
            {
                Canvas.SetLeft(cp1Thumb, cpX - cp1Thumb.Width / 2);
            }
            if (_height > 0)
            {
                Canvas.SetTop(cp1Thumb, cpY - cp1Thumb.Height / 2);
            }
            //System.Diagnostics.Debug.WriteLine(X +" / "+ Y);
        }

        void refreshThumb()
        {
            if (MaxX > MinX)
            {
                double cpX = (X - MinX) * _width / (MaxX - MinX);
                Canvas.SetLeft(cp1Thumb, cpX - cp1Thumb.Width / 2);
            }
            if (MaxY > MinY)
            {
                double cpY = _height - (Y - MinY) * _height / (MaxY - MinY);
                Canvas.SetTop(cp1Thumb, cpY - cp1Thumb.Height / 2);
            }                        
        }
       
        void cpThumb_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point lastP = e.GetPosition(slider2DCanvas);
            double cpX = lastP.X;
            double cpY = lastP.Y;
            if (cpX < 0) cpX = 0;
            if (cpX > _width) cpX = _width;
            if (cpY < 0) cpY = 0;
            if (cpY > _height) cpY = _height;
            updateProperties(cpX, cpY);
            updatePoint(cpX, cpY);

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


        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _width = (int)slider2DCanvas.ActualWidth;
            _height = (int)slider2DCanvas.ActualHeight;
            refreshThumb();
        }

    }
}
