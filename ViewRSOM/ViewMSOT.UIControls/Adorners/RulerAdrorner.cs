using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Data;

namespace ViewMSOT.UIControls.Adorners
{
    public class RulerAdorner : Adorner
    {
        #region localvariables
        Color _penColor;
        //Color _textColor;
        Style _thumbStyle;
        Style _anchorThumbStyle;
        ObservableCollection<Point> _pathPoints;

        VisualCollection visualChildren;
        Canvas _raster;
        Path _pathPtr = null;
        double _horizontalLength;
        Label _pl;
        #endregion localvariables

        public RulerAdorner(UIElement adornedElement, Color penColor, Style thumbStyle, Style anchorThumbStyle)
            : base(adornedElement)
        {            
            visualChildren = new VisualCollection(this);
            _raster = new Canvas();
            visualChildren.Add(_raster);
            _penColor = penColor;
            //_textColor = textColor;
            _thumbStyle = thumbStyle;
            _anchorThumbStyle = anchorThumbStyle;
            IsClipEnabled = true;
            IsHitTestVisible = true;
            _pathPoints = new ObservableCollection<Point>();
            _pathPoints.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(_pathPoints_CollectionChanged);
            AdornedElement.IsVisibleChanged += new DependencyPropertyChangedEventHandler(AdornedElement_IsVisibleChanged);
            _horizontalLength = 1;
            _raster.SizeChanged += _rasterCanvas_SizeChanged;
            _pl = new Label() { Content = "Init", Foreground = new SolidColorBrush(Colors.White), Background = new SolidColorBrush(Color.FromArgb(30, 10, 10, 10)), IsHitTestVisible = false };
        }

        void AdornedElement_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
                _raster.Visibility = System.Windows.Visibility.Visible;
            else
                _raster.Visibility = System.Windows.Visibility.Hidden;
                    
        }

        void _rasterCanvas_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.AdornedElement.IsVisible)
                    drawElements();
            }
            catch (Exception ex)
            {
                Xvue.MSOT.ViewModels.Log.ViewModelLog.MsotTrace("_adornedGrid_SizeChanged exception: " + ex.Message, this.GetType().ToString());
            }
        }

#region properties

        public ObservableCollection<Point> PathPoints
        {
            get { return (ObservableCollection<Point>)GetValue(PathPointsProperty); }
        }

        public static readonly DependencyProperty PathPointsProperty =
           DependencyProperty.Register(
              "PathPoints",
              typeof(ObservableCollection<Point>),
              typeof(RulerAdorner),
              new FrameworkPropertyMetadata(new ObservableCollection<Point>(),
                new PropertyChangedCallback(ChangePathPoints)));

        private static void ChangePathPoints(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            RulerAdorner roi = null;
            try
            {
                roi = (source as RulerAdorner);
                roi._pathPoints.CollectionChanged -= roi._pathPoints_CollectionChanged;                
                roi._pathPoints = (ObservableCollection<Point>)e.NewValue;
                roi._pathPoints.CollectionChanged += roi._pathPoints_CollectionChanged;                
                roi.drawElements();
            }
            catch
            {
                if(roi != null)
                    roi._pathPoints = new ObservableCollection<Point>();
            }
        }
       
        public double Zoom
        {
            get { return (double)GetValue(ZoomProperty); }
            set { SetValue(ZoomProperty,value); }
        }

        public static readonly DependencyProperty ZoomProperty =
           DependencyProperty.Register(
              "Zoom",
              typeof(double),
              typeof(RulerAdorner),
              new FrameworkPropertyMetadata(
                new PropertyChangedCallback(ChangeZoom)));

        private static void ChangeZoom(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                (source as RulerAdorner).drawElements();
            }
            catch
            {
            }
        }
       
        public static readonly DependencyProperty IsDraggingProperty =
                   DependencyProperty.Register(
                      "IsDragging",
                      typeof(bool),
                      typeof(RulerAdorner));

        public bool IsDragging
        {
            get { return (bool)GetValue(IsDraggingProperty); }
            set { SetValue(IsDraggingProperty, value); }
        }

        public bool IsDrawing
        {
            get { return (bool)GetValue(IsDrawingProperty); }
            set { SetValue(IsDrawingProperty, value); }
        }

        public static readonly DependencyProperty IsDrawingProperty =
                   DependencyProperty.Register(
                      "IsDrawing",
                      typeof(bool),
                      typeof(RulerAdorner),
                      new FrameworkPropertyMetadata(
                        new PropertyChangedCallback(ChangeIsDrawing)));

        private static void ChangeIsDrawing(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                (source as RulerAdorner).drawElements();
            }
            catch
            {
            }
        }

        public string RoiDetails
        {
            get { return (string)GetValue(RoiDetailsProperty); }
            set { SetValue(RoiDetailsProperty, value); }
        }

        public static readonly DependencyProperty RoiDetailsProperty =
                   DependencyProperty.Register(
                      "RoiDetails",
                      typeof(string),
                      typeof(RulerAdorner),
                      new FrameworkPropertyMetadata(
                        new PropertyChangedCallback(ChangeRoiDetails)));

        private static void ChangeRoiDetails(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                RulerAdorner roi = (source as RulerAdorner);
                roi.refreshDetails();
            }
            catch
            {
            }
        }

        public double LowLimit
        {
            get { return (double)GetValue(LowLimitProperty); }
            set { SetValue(LowLimitProperty, value); }
        }

        public static readonly DependencyProperty LowLimitProperty =
           DependencyProperty.Register(
              "LowLimit",
              typeof(double),
              typeof(RulerAdorner),
              new FrameworkPropertyMetadata(
                Double.NaN, FrameworkPropertyMetadataOptions.AffectsRender,
                new PropertyChangedCallback(ChangeLowLimit)));

        private static void ChangeLowLimit(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                (source as RulerAdorner).refreshRoiDetails();
            }
            catch
            {
            }
        }

        public double HighLimit
        {
            get { return (double)GetValue(HighLimitProperty); }
            set { SetValue(HighLimitProperty, value); }
        }

        public static readonly DependencyProperty HighLimitProperty =
           DependencyProperty.Register(
              "HighLimit",
              typeof(double),
              typeof(RulerAdorner),
              new FrameworkPropertyMetadata(
                Double.NaN, FrameworkPropertyMetadataOptions.AffectsRender,
                new PropertyChangedCallback(ChangeHighLimit)));

        private static void ChangeHighLimit(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                (source as RulerAdorner).refreshRoiDetails();
            }
            catch
            {
            }
        }

        public MatrixTransform ImageElementTransform
        {
            get { return (MatrixTransform)GetValue(ImageElementTransformProperty); }
            set { SetValue(ImageElementTransformProperty, value); }
        }

        public static readonly DependencyProperty ImageElementTransformProperty =
           DependencyProperty.Register(
              "ImageElementTransform",
              typeof(MatrixTransform),
              typeof(RulerAdorner),
              new FrameworkPropertyMetadata(
                new PropertyChangedCallback(ChangeImageElementTransform)));

        private static void ChangeImageElementTransform(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                RulerAdorner adornerView = source as RulerAdorner;
                adornerView._pl.LayoutTransform = adornerView.ImageElementTransform.Inverse as Transform;
            }
            catch
            {
            }
        }


#endregion properties

        private void refreshRoiDetails()
        {
            _horizontalLength = Math.Abs(HighLimit - LowLimit);
            if (_horizontalLength < double.Epsilon)
                _horizontalLength = 1;
            if (PathPoints.Count != 2)
                RoiDetails = "";
            else
            {
                double scale = Zoom;
                double a = (PathPoints[0].X - PathPoints[1].X) * scale * _horizontalLength / _raster.ActualWidth;
                double b = (PathPoints[0].Y - PathPoints[1].Y) * scale * _horizontalLength / _raster.ActualWidth;
                RoiDetails = Math.Sqrt(a * a + b * b).ToString("F3") + "mm";
            }
        }

        private void refreshRoiDetails(Point p)
        {
            _horizontalLength = Math.Abs(HighLimit - LowLimit);
            if (_horizontalLength < double.Epsilon)
                _horizontalLength = 1;
            if (PathPoints.Count != 1)
                RoiDetails = "";
            else
            {
                double scale = Zoom;
                double a = (PathPoints[0].X * scale - p.X) * _horizontalLength / _raster.ActualWidth;
                double b = (PathPoints[0].Y * scale - p.Y) * _horizontalLength / _raster.ActualWidth;
                RoiDetails = Math.Sqrt(a * a + b * b).ToString("F3") + "mm";
            }
        }

        void addThumbEvents(Thumb thumb)
        {
            thumb.DragStarted += new DragStartedEventHandler(pointThumb_DragStarted);
            thumb.DragDelta += new DragDeltaEventHandler(pointThumb_DragDelta);
            thumb.DragCompleted += new DragCompletedEventHandler(pointThumb_DragCompleted);            
        }

        void removeThumbEvents(Thumb thumb)
        {
            thumb.DragDelta -= pointThumb_DragDelta;
            thumb.DragStarted -= pointThumb_DragStarted;
        }

        private void drawElements()
        {
            double scaleX, scaleY;
            scaleX = scaleY = Zoom;
            _raster.Children.Clear();
            if (_pathPoints.Count == 0)
                return;
            foreach (UIElement child in _raster.Children)
            {
                if (child.GetType() == typeof(Thumb))
                    removeThumbEvents((child as Thumb));
            }
            _pathPtr = new Path();
            _pathPtr.Stroke = new SolidColorBrush(_penColor);
            _pathPtr.StrokeThickness = 1;            
            List<Point> thumbPoints = new List<Point>();
            foreach(Point p in _pathPoints)
                thumbPoints.Add(new Point(p.X * scaleX, p.Y * scaleY));

            _pathPtr.Data = (Geometry) drawPath();
            _raster.Children.Add(_pathPtr);
            //_raster.Children.Add(createDetails());
            int k = 0;
            foreach (Point thumb in thumbPoints)
                BuildPointThumb(thumb, System.Windows.Input.Cursors.SizeAll, k++);

            _raster.Children.Add(_pl);
        }

        object drawPath()
        {
            double scaleX, scaleY;
            scaleX = scaleY = Zoom;
            object geo = null;

            if (_pathPoints.Count == 2)
            {
                Point t0 = new Point(_pathPoints[0].X * scaleX, _pathPoints[0].Y * scaleY);
                Point t1 = new Point(_pathPoints[1].X * scaleX, _pathPoints[1].Y * scaleY);
                geo = new LineGeometry(t0, t1);

                Canvas.SetTop(_pl, (t0.Y + t1.Y) / 2 - _pl.ActualHeight - 4);
                Canvas.SetLeft(_pl, (t0.X + t1.X) / 2 - _pl.ActualWidth / 2);
            }
            refreshRoiDetails();
            return geo;
		}

        object drawPath(Point p)
        {
            double scaleX, scaleY;
            scaleX = scaleY = Zoom;
            object geo = null;
            Point t1 = new Point(p.X, p.Y);
            if (_pathPoints.Count == 1)
            {
                Point t0 = new Point(_pathPoints[0].X * scaleX, _pathPoints[0].Y * scaleY);
                geo = new LineGeometry(t0, t1);

                Canvas.SetTop(_pl, (t0.Y + t1.Y) / 2 - _pl.ActualHeight - 4);
                Canvas.SetLeft(_pl, (t0.X + t1.X) / 2 - _pl.ActualWidth / 2);

            }
            refreshRoiDetails(t1);
            return geo;
        }

        public void UpdateDrawingTrail(Point point)
        {
            if (_pathPoints.Count == 0)
                return;
            UIElement pathPtr = null;
            foreach (UIElement child in _raster.Children)
            {
                if (child.GetType() == typeof(Path))
                    pathPtr = child;
            }
            if (pathPtr != null)
            {
                Path path = pathPtr as Path;
                path.Data = (Geometry)drawPath(point);
                path.IsHitTestVisible = false;
            }
        }

        void refreshPath()
        {
            if (_pathPoints.Count == 0)
                return;            
            if (_pathPtr != null)
            {
                _pathPtr.Data = (Geometry) drawPath();
                //_pathPtr.IsHitTestVisible = true;
            }
        }

        void refreshDetails()
        {
            _pl.Content = RoiDetails;
        }

        void BuildPointThumb(Point t, Cursor customizedCursor,int i)
        {
            int size = 12;
            Thumb pointThumb = new Thumb();
            if (i == 0)
                pointThumb.Style = _anchorThumbStyle;
            else
                pointThumb.Style = _thumbStyle;
            Canvas.SetLeft(pointThumb, t.X-size/2);
            Canvas.SetTop(pointThumb, t.Y-size/2);
            pointThumb.Tag = i;
            // Set some arbitrary visual characteristics.
            pointThumb.Cursor = customizedCursor;
            pointThumb.Height = pointThumb.Width = size;
            addThumbEvents(pointThumb);
            _raster.Children.Add(pointThumb);
        }

        void _pathPoints_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
            {
                drawElements();
            }
        }

        void pointThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            IsDragging = true;
        }

        void pointThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            IsDragging = false;
        }

        void pointThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double dx = e.HorizontalChange, dy = e.VerticalChange;
            Thumb hitThumb = sender as Thumb;
            int index = (int)hitThumb.Tag;
            Point p = _pathPoints[index];
            double x = (p.X * Zoom + dx) / Zoom;
            double y = (p.Y * Zoom + dy) / Zoom;
            if (index == 0)
            {
                //for (int i = 1; i < _pathPoints.Count; i++)
                //{
                //    double locX = (_pathPoints[i].X * Zoom + dx) / Zoom;
                //    double locY = (_pathPoints[i].Y * Zoom + dy) / Zoom;
                //    _pathPoints[i] = new Point(locX, locY);
                //    foreach (UIElement child in _raster.Children)
                //    {
                //        if (child.GetType() == typeof(Thumb))
                //        {
                //            if ((int)((Thumb)child).Tag == i)
                //            {
                //                Thumb tempThumb = child as Thumb;
                //                double tempLeft = Canvas.GetLeft(tempThumb) + dx;
                //                double tempTop = Canvas.GetTop(tempThumb) + dy;
                //                Canvas.SetLeft(tempThumb, tempLeft);
                //                Canvas.SetTop(tempThumb, tempTop);
                //            }
                //        }
                //    }
                //}
            }
            _pathPoints[index] = new Point(x, y);
            refreshPath();
            double left = Canvas.GetLeft(hitThumb) + dx;
            double top = Canvas.GetTop(hitThumb) + dy;
            Canvas.SetLeft(hitThumb, left);
            Canvas.SetTop(hitThumb, top);

        }

        protected override int VisualChildrenCount { get { return 1; } }  //visualChildren.Count; } }
        protected override Visual GetVisualChild(int index) { return _raster; } //visualChildren[index]; }

        protected override Size ArrangeOverride( Size finalSize )
        {
            foreach (Visual child in visualChildren)
            {
                (child as FrameworkElement).Arrange(new Rect(finalSize));
            }
            return finalSize;
       }

    }
}

