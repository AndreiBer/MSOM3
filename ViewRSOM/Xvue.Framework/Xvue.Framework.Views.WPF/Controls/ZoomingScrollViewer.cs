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
    
    public class ZoomingScrollViewer : ScrollViewer
    {
        private bool _isRelativeSet;
        Point _selectedCenterPoint;

        public ZoomingScrollViewer()
        {
            _isRelativeSet = false;
            _selectedCenterPoint = new Point();
        }

        public bool KeepInCenter
        {
            get { return (bool)GetValue(KeepInCenterProperty); }
            set { SetValue(KeepInCenterProperty, value); }
        }

        public static readonly DependencyProperty KeepInCenterProperty =
            DependencyProperty.Register(
          "KeepInCenter",
          typeof(bool),
          typeof(ZoomingScrollViewer),
          new FrameworkPropertyMetadata(
            new PropertyChangedCallback(OnKeepInCenterChanged)));

        private static void OnKeepInCenterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ZoomingScrollViewer scroll = d as ZoomingScrollViewer;

            if (scroll != null)
            {
                if ((bool)e.NewValue)
                {
                    scroll.ScrollChanged += scroll_ScrollChanged;
                    scroll.ScrollChanged -= scrollSelectedZoom_ScrollChanged;
                    scroll._isRelativeSet = false;
                }
                else
                {
                    scroll.ScrollChanged -= scroll_ScrollChanged;
                    scroll.ScrollChanged += scrollSelectedZoom_ScrollChanged;
                }
            }
        }

        double relX;
        double relY;
        static void scroll_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            try
            {
                ZoomingScrollViewer scroll = sender as ZoomingScrollViewer;
                if (scroll._isRelativeSet)
                {
                    if (e.ExtentWidthChange != 0 || e.ExtentHeightChange != 0)
                    {
                        scroll.ScrollToHorizontalOffset(CalculateOffset(e.ExtentWidth, e.ViewportWidth, scroll.ScrollableWidth, scroll.relX));
                        scroll.ScrollToVerticalOffset(CalculateOffset(e.ExtentHeight, e.ViewportHeight, scroll.ScrollableHeight, scroll.relY));
                    }
                    else
                    {
                        scroll.relX = (e.HorizontalOffset + 0.5 * e.ViewportWidth) / e.ExtentWidth;
                        scroll.relY = (e.VerticalOffset + 0.5 * e.ViewportHeight) / e.ExtentHeight;
                    }
                }
                else
                {
                    scroll.relX = (e.HorizontalOffset + 0.5 * e.ViewportWidth) / e.ExtentWidth;
                    scroll.relY = (e.VerticalOffset + 0.5 * e.ViewportHeight) / e.ExtentHeight;
                    scroll._isRelativeSet = true;
                }
                scroll.OnScrollChanged();
            }
            catch { }
        }

        private static double CalculateOffset(double extent, double viewPort, double scrollWidth, double relBefore)
        {
            double offset = relBefore * extent - 0.5 * viewPort;
            if (offset < 0)
            {
                offset = 0.5 * scrollWidth;
            }
            return offset;
        }

        static void scrollSelectedZoom_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            try
            {
                ZoomingScrollViewer scroll = sender as ZoomingScrollViewer;
                if (e.ExtentWidthChange != 0 || e.ExtentHeightChange != 0)
                    scroll.ScrollToSelectedCenterPoint();
                scroll.OnScrollChanged();
            }
            catch { }
        }

        public void ScrollToSelectedCenterPoint()
        {
            ScrollToHorizontalOffset(_selectedCenterPoint.X * ScrollableWidth);
            ScrollToVerticalOffset(_selectedCenterPoint.Y * ScrollableHeight);
        }

        public void SetSelectedRelativePosition(Point point)
        {
            if (double.IsNaN(point.X) || double.IsNaN(point.Y))
                _selectedCenterPoint = new Point();
            else
                _selectedCenterPoint = point;
        }

        public event EventHandler ScrollChanged2;

        protected virtual void OnScrollChanged()
        {
            ScrollChanged2?.Invoke(this,EventArgs.Empty);
        }

     }
}
