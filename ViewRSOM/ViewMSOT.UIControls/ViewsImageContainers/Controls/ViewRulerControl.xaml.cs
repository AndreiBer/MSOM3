using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xvue.MSOT.ViewModels.Imaging;

namespace ViewMSOT.UIControls
{
	/// <summary>
	/// Interaction logic for RulerControl.xaml
	/// </summary>
	public partial class ViewRulerControl : UserControl
    {
        #region localvariables
        IList<Point> _rulerPoints;
        double _touchAreaEllipseHalf;
        #endregion localvariables

        public ViewRulerControl()
		{
			this.InitializeComponent();
            var doubleVar = Application.Current.FindResource("ControlThumbsTouchAreaLength");
            _touchAreaEllipseHalf = (double)doubleVar / 2;
		}

        #region properties

        public IEnumerable<Point> RulerPoints
        {
            get { return (IEnumerable<Point>)GetValue(RulerPointsProperty); }
            set { SetValue(RulerPointsProperty, (IEnumerable<Point>)value); }
        }

        public static readonly DependencyProperty RulerPointsProperty =
        DependencyProperty.Register(
         "RulerPoints",
         typeof(IEnumerable<Point>),
         typeof(ViewRulerControl),
         new FrameworkPropertyMetadata(new PropertyChangedCallback(RulerPointsChanged)));

        public bool IsRulerSelected
        {
            get { return (bool)GetValue(IsRulerSelectedProperty); }
            set { SetValue(IsRulerSelectedProperty, (bool)value); }
        }

        public static readonly DependencyProperty IsRulerSelectedProperty =
        DependencyProperty.Register(
         "IsRulerSelected",
         typeof(bool),
         typeof(ViewRulerControl),
         new FrameworkPropertyMetadata(new PropertyChangedCallback(IsRulerSelectedChanged)));

        public double ZoomScale
        {
            get { return (double)GetValue(ZoomScaleProperty); }
            set { SetValue(ZoomScaleProperty, (double)value); }
        }

        public static readonly DependencyProperty ZoomScaleProperty =
            DependencyProperty.Register(
            "ZoomScale",
            typeof(double),
            typeof(ViewRulerControl),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(ZoomScaleChanged)));
        #endregion properties

        public Point PathPoint0
        {
            get { return (Point)GetValue(PathPoint0Property); }
            set { SetValue(PathPoint0Property, (Point)value); }
        }

        public static readonly DependencyProperty PathPoint0Property =
            DependencyProperty.Register(
            "PathPoint0",
            typeof(Point),
            typeof(ViewRulerControl));

        public Point PathPoint1
        {
            get { return (Point)GetValue(PathPoint1Property); }
            set { SetValue(PathPoint1Property, (Point)value); }
        }

        public static readonly DependencyProperty PathPoint1Property =
            DependencyProperty.Register(
            "PathPoint1",
            typeof(Point),
            typeof(ViewRulerControl));

        public bool IsDrawing
        {
            get { return (bool)GetValue(IsDrawingProperty); }
            set { SetValue(IsDrawingProperty, (bool)value); }
        }

        public static readonly DependencyProperty IsDrawingProperty =
        DependencyProperty.Register(
         "IsDrawing",
         typeof(bool),
         typeof(ViewRulerControl));

        public MatrixTransform ImageElementTransform
        {
            get { return (MatrixTransform)GetValue(ImageElementTransformProperty); }
            set { SetValue(ImageElementTransformProperty, value); }
        }

        public static readonly DependencyProperty ImageElementTransformProperty =
           DependencyProperty.Register(
              "ImageElementTransform",
              typeof(MatrixTransform),
              typeof(ViewRulerControl),
              new FrameworkPropertyMetadata(
                new PropertyChangedCallback(ChangeImageElementTransform)));

        private static void ChangeImageElementTransform(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                ViewRulerControl control = source as ViewRulerControl;
                control.detailsLabel.LayoutTransform = control.ImageElementTransform.Inverse as Transform;
            }
            catch
            {
            }
        }

        private static void RulerPointsChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                var control = (ViewRulerControl)source;
                var oldCollection = e.OldValue as INotifyCollectionChanged;
                var newCollection = e.NewValue as INotifyCollectionChanged;

                if (oldCollection != null)
                {
                    oldCollection.CollectionChanged -= control._rulerPoints_CollectionChanged;
                }

                if (newCollection != null)
                {
                    newCollection.CollectionChanged += control._rulerPoints_CollectionChanged;
                }

                control._rulerPoints = newCollection as IList<Point>;
                control.updatePosition();
            }
            catch
            {
            }
        }

        void _rulerPoints_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //if (e.Action != System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
            //{
                updatePosition();
            //}
        }

        public double ParentWidth
        {
            get { return (double)GetValue(ParentWidthProperty); }
            set { SetValue(ParentWidthProperty, (double)value); }
        }

        public static readonly DependencyProperty ParentWidthProperty =
        DependencyProperty.Register(
         "ParentWidth",
         typeof(double),
         typeof(ViewRulerControl));

        public double ParentHeight
        {
            get { return (double)GetValue(ParentHeightProperty); }
            set { SetValue(ParentHeightProperty, (double)value); }
        }

        public static readonly DependencyProperty ParentHeightProperty =
        DependencyProperty.Register(
         "ParentHeight",
         typeof(double),
         typeof(ViewRulerControl));

        private static void IsRulerSelectedChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                ViewRulerControl control = source as ViewRulerControl;
                control.refreshViewIsRulerSelected();
            }
            catch
            {
            }
        }

        private static void ZoomScaleChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                ViewRulerControl control = source as ViewRulerControl;
                control.updatePosition();
            }
            catch
            {
            }
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            updatePosition();
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            updatePosition();
        }

        void updatePosition()
        {
            try
            {
                if (_rulerPoints == null)
                {
                    hideElements();
                    return;
                }

                if (_rulerPoints.Count != 2)
                {
                    hideElements();
                    return;
                }

                Point t0 = new Point(_rulerPoints[0].X * ZoomScale, _rulerPoints[0].Y * ZoomScale);
                Point t1 = new Point(_rulerPoints[1].X * ZoomScale, _rulerPoints[1].Y * ZoomScale);
                PathPoint0 = t0;
                PathPoint1 = t1;

                double top = (t0.Y + t1.Y) / 2 - detailsLabel.ActualHeight - 4;
                if (top < 0)
                    top = (t0.Y + t1.Y) / 2 - 4;
                Canvas.SetTop(detailsLabel, top);
                Canvas.SetLeft(detailsLabel, (t0.X + t1.X) / 2 - detailsLabel.ActualWidth / 2);

                Canvas.SetLeft(point0Thumb, _rulerPoints[0].X * ZoomScale - _touchAreaEllipseHalf);
                Canvas.SetLeft(point1Thumb, _rulerPoints[1].X * ZoomScale - _touchAreaEllipseHalf);
                Canvas.SetTop(point0Thumb, _rulerPoints[0].Y * ZoomScale - _touchAreaEllipseHalf);
                Canvas.SetTop(point1Thumb, _rulerPoints[1].Y * ZoomScale - _touchAreaEllipseHalf);

                showElements();
            }
            catch { }
        }

        private void pointThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            double dx = e.HorizontalChange, dy = e.VerticalChange;
            Thumb hitThumb = sender as Thumb;
            this.ParentHeight.ToString();
            this.ParentWidth.ToString();
            int index;
            if (hitThumb == point0Thumb)
                index = 0;
            else
                index = 1;
            Point p = _rulerPoints[index];
            this.ToString();
            double x = (p.X * ZoomScale + dx) / ZoomScale;
            double y = (p.Y * ZoomScale + dy) / ZoomScale;
            if (y > ParentHeight / ZoomScale)
                y = ParentHeight / ZoomScale;
            else if (y < 0)
                y = 0;
            if (x > ParentWidth / ZoomScale)
                x = ParentWidth / ZoomScale;
            else if (x < 0)
                x = 0;
            _rulerPoints[index] = new Point(x, y);
            updatePosition();
        }

        private void hideElements()
        {
            point0Thumb.Visibility = System.Windows.Visibility.Collapsed;
            point1Thumb.Visibility = System.Windows.Visibility.Collapsed;
            rulerPath.Visibility = System.Windows.Visibility.Collapsed;
            detailsLabel.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void showElements()
        {
            point0Thumb.Visibility = System.Windows.Visibility.Visible;
            if (IsDrawing)
            {
                point1Thumb.Visibility = System.Windows.Visibility.Collapsed;
                rulerPath.IsHitTestVisible = false;
                detailsLabel.IsHitTestVisible = false;
            }
            else
            {
                point1Thumb.Visibility = System.Windows.Visibility.Visible;
                rulerPath.IsHitTestVisible = true;
                detailsLabel.IsHitTestVisible = true;
            }

            rulerPath.Visibility = System.Windows.Visibility.Visible;
            detailsLabel.Visibility = System.Windows.Visibility.Visible;
        }

        private void detailsLabel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            updatePosition();
        }

        private void refreshViewIsRulerSelected()
        {
            if (IsRulerSelected)
            {
                rulerPath.Stroke = ViewModelRulerTool2D.SelectedRulerPathStroke;
                detailsLabel.Foreground = ViewModelRulerTool2D.SelectedRulerLabelForeground;
            }
            else
            {
                rulerPath.Stroke = ViewModelRulerTool2D.RulerPathStroke;
                detailsLabel.Foreground = ViewModelRulerTool2D.RulerLabelForeground;
            }
        }


        private void detailsLabel_MouseEnter(object sender, MouseEventArgs e)
        {
            increasePathLabelSize();
        }

        private void detailsLabel_MouseLeave(object sender, MouseEventArgs e)
        {
            decreasePathLabelSize();
        }

        private void increasePathLabelSize()
        {
            if (detailsLabel.FontSize != 18)
                detailsLabel.FontSize = 18;
            if (rulerPath.StrokeThickness != 2)
                rulerPath.StrokeThickness = 2;
        }

        private void decreasePathLabelSize()
        {
            if (detailsLabel.FontSize != 12)
                detailsLabel.FontSize = 12;
            if (rulerPath.StrokeThickness != 1)
                rulerPath.StrokeThickness = 1;
        }

        private void UserControl_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            IsRulerSelected = true;
        }
    }

}