using System;
using System.Collections.Generic;
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
	/// Interaction logic for CrossHairControl.xaml
	/// </summary>
	public partial class CrosshairControl : UserControl
	{
        double _marginCorrection;
		public CrosshairControl()
		{
			this.InitializeComponent();
            var doubleVar = Application.Current.FindResource("ControlThumbsTouchAreaLength");
            _marginCorrection = (double)doubleVar / 2 - 1;
		}

        #region properties

        public double VerticalPosition
        {
            get { return (double)GetValue(VerticalPositionProperty); }
            set
            {
                SetValue(VerticalPositionProperty, (double)value);
            }
        }

        public static readonly DependencyProperty VerticalPositionProperty =
        DependencyProperty.Register(
         "VerticalPosition",
         typeof(double),
         typeof(CrosshairControl),
         new FrameworkPropertyMetadata(new PropertyChangedCallback(VerticalPositionChanged)));

        private static void VerticalPositionChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                CrosshairControl control = source as CrosshairControl;
                control.upadatePosition();
            }
            catch
            {                
            }
        }

        public double HorizontalPosition
        {
            get { return (double)GetValue(HorizontalPositionProperty); }
            set
            {
                SetValue(HorizontalPositionProperty, (double)value);
            }
        }

        public static readonly DependencyProperty HorizontalPositionProperty =
        DependencyProperty.Register(
         "HorizontalPosition",
         typeof(double),
         typeof(CrosshairControl),
         new FrameworkPropertyMetadata(new PropertyChangedCallback(HorizontalPositionChanged)));

        private static void HorizontalPositionChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                CrosshairControl control = source as CrosshairControl;
                control.upadatePosition();
            }
            catch
            {
            }
        }

        //-----------------------


        public double ParentWidth
        {
            get { return (double)GetValue(ParentWidthProperty); }
            set
            {
                SetValue(ParentWidthProperty, (double)value);
            }
        }

        public static readonly DependencyProperty ParentWidthProperty =
        DependencyProperty.Register(
         "ParentWidth",
         typeof(double),
         typeof(CrosshairControl),
         new FrameworkPropertyMetadata(new PropertyChangedCallback(ParentWidthChanged)));

        private static void ParentWidthChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                CrosshairControl control = source as CrosshairControl;
                control.upadatePosition();
            }
            catch
            {
            }
        }


        //--------------------------

        public double ParentHeight
        {
            get { return (double)GetValue(ParentHeightProperty); }
            set
            {
                SetValue(ParentHeightProperty, (double)value);
            }
        }

        public static readonly DependencyProperty ParentHeightProperty =
        DependencyProperty.Register(
         "ParentHeight",
         typeof(double),
         typeof(CrosshairControl),
         new FrameworkPropertyMetadata(new PropertyChangedCallback(ParentHeightChanged)));

        private static void ParentHeightChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                CrosshairControl control = source as CrosshairControl;
                control.upadatePosition();
            }
            catch
            {
            }
        }

        //-------------------
        void upadatePosition()
        {
            try
            {
                double left = this.HorizontalPosition * this.ActualWidth;
                double top = this.VerticalPosition * this.ActualHeight;
                PART_H.Margin = new Thickness(left, PART_H.Margin.Top, PART_H.Margin.Right, PART_H.Margin.Bottom);
                PART_THUMB_VISUAL.Margin = new Thickness(left - 4, top - 4, PART_THUMB_VISUAL.Margin.Right, PART_THUMB_VISUAL.Margin.Bottom);
                PART_THUMB.Margin = new Thickness(left - _marginCorrection, top - _marginCorrection, PART_THUMB.Margin.Right, PART_THUMB.Margin.Bottom);
                PART_W.Margin = new Thickness(PART_W.Margin.Left, top, PART_W.Margin.Right, PART_W.Margin.Bottom);

                //double scaleY = ActualHeight / ParentHeight;
                //double scaleX = ActualWidth / ParentWidth ;
                //if (scaleX < scaleY) scaleX = scaleY;

                //PART_H.StrokeThickness = scaleX * 1;
                //PART_W.StrokeThickness = scaleX * 1;

            }
            catch { }
        }
        #endregion properties

        private void PART_THUMB_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UIElement a = (UIElement)sender;            
            a.CaptureMouse();
        }

        private void PART_THUMB_MouseMove(object sender, MouseEventArgs e)
        {
            UIElement a = (UIElement)sender;            
            if (a.IsMouseCaptured)
            {
                Point lastP = e.GetPosition(this);
                double dw = lastP.X / ActualWidth;
                double dh = lastP.Y / ActualHeight;

                if (dw <= 0)
                    HorizontalPosition = 0.0001;
                else if (dw >= 1)
                    HorizontalPosition = 0.9999;
                else
                    HorizontalPosition = dw;

                if (dh < 0)
                    VerticalPosition = 0.0001;
                else if (dh >= 1)
                    VerticalPosition = 0.9999;
                else
                    VerticalPosition = dh;

                e.Handled = true;
            }
        }

        private void PART_THUMB_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            UIElement a = (UIElement)sender;
            a.ReleaseMouseCapture();
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            upadatePosition();
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            upadatePosition();
        }

        public void RefreshExternal(Point lastP)
        {
            if ((lastP.X > 0) && (lastP.X < ActualWidth) && (lastP.Y > 0) && (lastP.Y < ActualHeight))
            {
                double dw = lastP.X / ActualWidth;
                double dh = lastP.Y / ActualHeight;
                HorizontalPosition = dw;
                VerticalPosition = dh;
            }
        }

        //private void LayoutRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        //{
        //    try
        //    {
        //        double thickness = 1;
        //        if (LayoutRoot.ActualHeight > LayoutRoot.ActualWidth)
        //            thickness = 1.0 / LayoutRoot.ActualHeight;
        //        else
        //            thickness = 1.0 / LayoutRoot.ActualWidth;
        //        PART_H.Width = thickness;
        //        PART_W.Height = thickness;
        //    }
        //    catch { }
        //}
    }
}