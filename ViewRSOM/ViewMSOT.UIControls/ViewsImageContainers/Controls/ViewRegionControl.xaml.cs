using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Xvue.MSOT.Common;
using Xvue.MSOT.DataModels.Plugins.Imaging.Session;
using Xvue.MSOT.Services.Imaging;

namespace ViewMSOT.UIControls
{
	/// <summary>
	/// Interaction logic for RoiControl.xaml
	/// </summary>
	public partial class ViewRegionControl : UserControl
    {
        #region localvariables
        IList<Point> _roiPoints;
        //double _touchAreaEllipseHalf;
        Path _pathPtr = null;
        Path _selectionPathPtr = null;
        Brush _penBrush;
        Style _thumbStyle;
        Style _anchorThumbStyle;
        double _thumbSize;
        #endregion localvariables

        public ViewRegionControl()
		{
			this.InitializeComponent();
            var doubleVar = Application.Current.FindResource("ControlThumbsTouchAreaLength");
            //_touchAreaEllipseHalf = (double)doubleVar / 2;
            _thumbSize = (double)doubleVar;
            _penBrush = new SolidColorBrush(Colors.Red);
            _thumbStyle = this.Resources["PolygonControlThumbStyle"] as Style;
            _anchorThumbStyle = this.Resources["AnchorControlThumbStyle"] as Style;
            _pointThumbMouseDownEventDelegate = new MouseButtonEventHandler(pointThumb_MouseDown);
        }

        #region properties

        public ColorType RoiColor
        {
            get { return (ColorType)GetValue(RoiColorProperty); }
            set { SetValue(RoiColorProperty, value); }
        }

        public static readonly DependencyProperty RoiColorProperty =
            DependencyProperty.Register(
            "RoiColor",
            typeof(ColorType),
            typeof(ViewRegionControl),
            new FrameworkPropertyMetadata(
                new PropertyChangedCallback(ChangeRoiColor)));

        public string RoiDetails
        {
            get { return (string)GetValue(RoiDetailsProperty); }
            set { SetValue(RoiDetailsProperty, value); }
        }

        public static readonly DependencyProperty RoiDetailsProperty =
            DependencyProperty.Register(
            "RoiDetails",
            typeof(string),
            typeof(ViewRegionControl),
            new FrameworkPropertyMetadata(
                new PropertyChangedCallback(ChangeRoiDetails)));

        public IEnumerable<Point> RoiPoints
        {
            get { return (IEnumerable<Point>)GetValue(RoiPointsProperty); }
            set { SetValue(RoiPointsProperty, (IEnumerable<Point>)value); }
        }

        public static readonly DependencyProperty RoiPointsProperty =
        DependencyProperty.Register(
         "RoiPoints",
         typeof(IEnumerable<Point>),
         typeof(ViewRegionControl),
         new FrameworkPropertyMetadata(new PropertyChangedCallback(RoiPointsChanged)));

        public double ZoomScale
        {
            get { return (double)GetValue(ZoomScaleProperty); }
            set { SetValue(ZoomScaleProperty, (double)value); }
        }

        public static readonly DependencyProperty ZoomScaleProperty =
            DependencyProperty.Register(
            "ZoomScale",
            typeof(double),
            typeof(ViewRegionControl),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(ZoomScaleChanged)));

        public bool IsDrawing
        {
            get { return (bool)GetValue(IsDrawingProperty); }
            set { SetValue(IsDrawingProperty, (bool)value); }
        }

        public static readonly DependencyProperty IsDrawingProperty =
        DependencyProperty.Register(
         "IsDrawing",
         typeof(bool),
         typeof(ViewRegionControl),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(IsDrawingChanged)));

        public bool IsDragging
        {
            get { return (bool)GetValue(IsDraggingProperty); }
            set { SetValue(IsDraggingProperty, value); }
        }

        public static readonly DependencyProperty IsDraggingProperty =
           DependencyProperty.Register(
              "IsDragging",
              typeof(bool),
              typeof(ViewRegionControl));

        public double ParentWidth
        {
            get { return (double)GetValue(ParentWidthProperty); }
            set { SetValue(ParentWidthProperty, (double)value); }
        }

        public static readonly DependencyProperty ParentWidthProperty =
        DependencyProperty.Register(
         "ParentWidth",
         typeof(double),
         typeof(ViewRegionControl));

        public double ParentHeight
        {
            get { return (double)GetValue(ParentHeightProperty); }
            set { SetValue(ParentHeightProperty, (double)value); }
        }

        public static readonly DependencyProperty ParentHeightProperty =
        DependencyProperty.Register(
         "ParentHeight",
         typeof(double),
         typeof(ViewRegionControl));

        public ROIsViewStyle RoisStyle
        {
            get { return (ROIsViewStyle)GetValue(RoisStyleProperty); }
            set { SetValue(RoisStyleProperty, (ROIsViewStyle)value); }
        }

        public static readonly DependencyProperty RoisStyleProperty =
            DependencyProperty.Register(
            "RoisStyle",
            typeof(ROIsViewStyle),
            typeof(ViewRegionControl),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(RoisStyleChanged)));

        public RoiShape CurrentShape
        {
            get { return (RoiShape)GetValue(CurrentShapeProperty); }
            set { SetValue(CurrentShapeProperty, value); }
        }

        public static readonly DependencyProperty CurrentShapeProperty =
            DependencyProperty.Register(
            "CurrentShape",
            typeof(RoiShape),
            typeof(ViewRegionControl),
            new FrameworkPropertyMetadata(
                new PropertyChangedCallback(ChangeCurrentShape)));

        public Point TrailPoint
        {
            get { return (Point)GetValue(TrailPointProperty); }
            set { SetValue(TrailPointProperty, (Point)value); }
        }

        public static readonly DependencyProperty TrailPointProperty =
            DependencyProperty.Register(
            "TrailPoint",
            typeof(Point),
            typeof(ViewRegionControl),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(TrailPointChanged)));

        public bool IsRoiSelected
        {
            get { return (bool)GetValue(IsRoiSelectedProperty); }
            set { SetValue(IsRoiSelectedProperty, (bool)value); }
        }

        public static readonly DependencyProperty IsRoiSelectedProperty =
        DependencyProperty.Register(
         "IsRoiSelected",
         typeof(bool),
         typeof(ViewRegionControl),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(IsRoiSelectedChanged)));

        #endregion properties

        private static void ChangeRoiColor(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                if (e.NewValue != null)
                {
                    ViewRegionControl control = (d as ViewRegionControl);
                    control.roiColorChanged();
                }
            }
            catch
            {
            }
        }

        private void roiColorChanged()
        {
            _penBrush = new SolidColorBrush(RoiColor.ColorProperty);
            updatePosition();
        }

        private static void ChangeRoiDetails(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                ViewRegionControl control = (source as ViewRegionControl);
                control.refreshDetails();
            }
            catch
            {
            }
        }

        void refreshDetails()
        {
            Thumb thumbPtr;
            foreach (UIElement child in drawingCanvas.Children)
            {
                if (child.GetType() == typeof(Thumb))
                {
                    thumbPtr = child as Thumb;
                    thumbPtr.ToolTip = RoiDetails;
                }
            }
        }

        private static void IsRoiSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                ViewRegionControl control = (d as ViewRegionControl);
                control.refreshIsSelected((bool)e.NewValue);
            }
            catch
            {
            }
        }

        private void refreshIsSelected(bool p)
        {
            System.Windows.Visibility newVisibility = p ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            foreach (UIElement child in drawingCanvas.Children)
            {
                if (child.GetType() == typeof(Thumb))
                {
                    child.SetCurrentValue(Thumb.VisibilityProperty, newVisibility);
                }
            }

            //if (p)
            //{
            //    this.CaptureMouse();
            //}
            //else
            //{
            //    this.ReleaseMouseCapture();
            //}
        }

        private static void IsDrawingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                ViewRegionControl control = (d as ViewRegionControl);
                if(control.CurrentShape == RoiShape.Polygon)
                {
                    if((bool)e.NewValue == false && (bool)e.OldValue == true)
                    {
                        control.updatePosition();
                        control.RemoveFirstThumbSpecialEvent();
                    }
                }
            }
            catch
            {
            }
        }

        private static void TrailPointChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ViewRegionControl control = (d as ViewRegionControl);
            control.updateDrawingTrail();
        }

        private static void ChangeCurrentShape(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                ViewRegionControl control = (source as ViewRegionControl);
                control.updatePosition();
            }
            catch
            {
            }
        }

        private static void RoisStyleChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ViewRegionControl control = (source as ViewRegionControl);
            control.updatePosition();
        }

        private static void RoiPointsChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                var control = (ViewRegionControl)source;
                var oldCollection = e.OldValue as INotifyCollectionChanged;
                var newCollection = e.NewValue as INotifyCollectionChanged;

                if (oldCollection != null)
                {
                    oldCollection.CollectionChanged -= control._roiPoints_CollectionChanged;
                }

                if (newCollection != null)
                {
                    newCollection.CollectionChanged += control._roiPoints_CollectionChanged;
                }

                control._roiPoints = newCollection as IList<Point>;
                control.updatePosition();
            }
            catch
            {
            }
        }

        void _roiPoints_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
            {
                updatePosition();
                if (CurrentShape == RoiShape.Polygon)
                {
                    // The following would need to be called just once if the updatePosition did not rebuild all the control points in every CollectionChanged event.
                    AddFirstThumbSpecialEvent();
                }
            }
        }



        private static void ZoomScaleChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                ViewRegionControl control = source as ViewRegionControl;
                control.updatePosition();
            }
            catch
            {
            }
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.DataContext != null)
            {
                updatePosition();
                refreshIsSelected(IsRoiSelected);
            }
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            updatePosition();
        }

        void updatePosition()
        {
            try
            {
                foreach (UIElement child in drawingCanvas.Children)
                {
                    if (child.GetType() == typeof(Thumb))
                        removeThumbEvents((child as Thumb));
                }

                if (_selectionPathPtr != null)
                {
                    _selectionPathPtr.MouseEnter -= _pathPtr_MouseEnter;
                    _selectionPathPtr.MouseLeave -= _pathPtr_MouseLeave;
                }

                drawingCanvas.Children.Clear();

                if (_roiPoints == null)
                    return;

                if (_roiPoints.Count == 0 || RoisStyle == ROIsViewStyle.None)
                    return;

                double scaleX, scaleY;
                scaleX = scaleY = ZoomScale;
                
                _pathPtr = new Path();
                _selectionPathPtr = new Path();
                _selectionPathPtr.MouseEnter += _pathPtr_MouseEnter;
                _selectionPathPtr.MouseLeave += _pathPtr_MouseLeave;

                if (RoisStyle == ROIsViewStyle.Fill)
                {
                    Color roiColor = RoiColor.ColorProperty;
                    _pathPtr.Fill = new SolidColorBrush(Color.FromArgb(128, roiColor.R, roiColor.G, roiColor.B));
                    _selectionPathPtr.Visibility = System.Windows.Visibility.Collapsed;
                }
                else if (RoisStyle == ROIsViewStyle.Outline)
                {
                    _pathPtr.Stroke = _penBrush;
                    _pathPtr.StrokeThickness = 1;
                    _selectionPathPtr.Visibility = System.Windows.Visibility.Visible;
                    _selectionPathPtr.Stroke = Brushes.Transparent;
                    _selectionPathPtr.StrokeThickness = 20;
                }

                List<Point> thumbPoints = new List<Point>();
                foreach (Point p in _roiPoints)
                    thumbPoints.Add(new Point(p.X * scaleX, p.Y * scaleY));

                _pathPtr.Data = (Geometry)drawPath();
                _selectionPathPtr.Data = (Geometry)drawPath();
                drawingCanvas.Children.Add(_pathPtr);
                drawingCanvas.Children.Add(_selectionPathPtr);
                int k = 0;
                foreach (Point thumb in thumbPoints)
                    BuildPointThumb(thumb, System.Windows.Input.Cursors.SizeAll, k++);
                refreshIsSelected(IsRoiSelected);
            }
            catch { }
        }

        void _pathPtr_MouseLeave(object sender, MouseEventArgs e)
        {
            decreasePathSize();
        }

        void _pathPtr_MouseEnter(object sender, MouseEventArgs e)
        {
            increasePathSize();
        }

        private void increasePathSize()
        {
            if (_pathPtr.StrokeThickness != 2)
                _pathPtr.StrokeThickness = 2;
        }

        private void decreasePathSize()
        {
            if (_pathPtr.StrokeThickness != 1)
                _pathPtr.StrokeThickness = 1;
        }

        void BuildPointThumb(Point t, Cursor customizedCursor, int i)
        {
            Thumb pointThumb = new Thumb();
            if (i == 0)
                pointThumb.Style = _anchorThumbStyle;
            else
                pointThumb.Style = _thumbStyle;
            Canvas.SetLeft(pointThumb, t.X - _thumbSize / 2);
            Canvas.SetTop(pointThumb, t.Y - _thumbSize / 2);
            pointThumb.Tag = i;
            // Set some arbitrary visual characteristics.
            pointThumb.Cursor = customizedCursor;
            pointThumb.Height = pointThumb.Width = _thumbSize;
            pointThumb.ToolTip = RoiDetails;
            addThumbEvents(pointThumb);
            drawingCanvas.Children.Add(pointThumb);
        }

        Delegate _pointThumbMouseDownEventDelegate;
        void addThumbEvents(Thumb thumb)
        {
            thumb.DragStarted += new DragStartedEventHandler(pointThumb_DragStarted);
            thumb.DragDelta += new DragDeltaEventHandler(pointThumb_DragDelta);
            thumb.DragCompleted += new DragCompletedEventHandler(pointThumb_DragCompleted);
        }

        public void AddFirstThumbSpecialEvent()
        {
            drawingCanvas.Children.OfType<Thumb>().Where(X => (int)X.Tag == 0).First().AddHandler(Mouse.PreviewMouseDownEvent, _pointThumbMouseDownEventDelegate);
        }

        public void RemoveFirstThumbSpecialEvent()
        {
            drawingCanvas.Children.OfType<Thumb>().Where(X => (int)X.Tag == 0).First().RemoveHandler(Mouse.PreviewMouseDownEvent, _pointThumbMouseDownEventDelegate);
        }

        void removeThumbEvents(Thumb thumb)
        {
            thumb.DragDelta -= pointThumb_DragDelta;
            thumb.DragStarted -= pointThumb_DragStarted;
            thumb.DragCompleted -= pointThumb_DragCompleted;
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
            Point p = _roiPoints[index];
            double x = (p.X * ZoomScale + dx) / ZoomScale;
            double y = (p.Y * ZoomScale + dy) / ZoomScale;

            if (index == 0)
            {
                if (y > ParentHeight / ZoomScale)
                {
                    y = ParentHeight / ZoomScale;
                    dy = ParentHeight - p.Y * ZoomScale;
                }
                else if (y < 0)
                {
                    y = 0;
                    dy = -p.Y * ZoomScale;
                }
                if (x > ParentWidth / ZoomScale)
                {
                    x = ParentWidth / ZoomScale;
                    dx = ParentWidth - p.X * ZoomScale;
                }
                else if (x < 0)
                {
                    x = 0;
                    dx = -p.X * ZoomScale;
                }
                for (int i = 1; i < _roiPoints.Count; i++)
                {
                    double locX = (_roiPoints[i].X * ZoomScale + dx) / ZoomScale;
                    double locY = (_roiPoints[i].Y * ZoomScale + dy) / ZoomScale;
                    _roiPoints[i] = new Point(locX, locY);
                    foreach (UIElement child in drawingCanvas.Children)
                    {
                        if (child.GetType() == typeof(Thumb))
                        {
                            Thumb tempThumb = child as Thumb;
                            if ((int)tempThumb.Tag == i)
                            {
                                double tempLeft = Canvas.GetLeft(tempThumb) + dx;
                                double tempTop = Canvas.GetTop(tempThumb) + dy;
                                Canvas.SetLeft(tempThumb, tempLeft);
                                Canvas.SetTop(tempThumb, tempTop);
                            }
                        }
                    }
                }
            }
            _roiPoints[index] = new Point(x, y);
            refreshPath();
            double left = Canvas.GetLeft(hitThumb) + dx;
            double top = Canvas.GetTop(hitThumb) + dy;
            Canvas.SetLeft(hitThumb, left);
            Canvas.SetTop(hitThumb, top);

        }

        // Create a custom routed event by first registering a RoutedEventID
        // This event uses the bubbling routing strategy
        public static readonly RoutedEvent ValidPolygonFirstControlPointClickEvent = EventManager.RegisterRoutedEvent(
            "ValidPolygonFirstControlPointClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ViewRegionControl));

        // Provide CLR accessors for the event
        public event RoutedEventHandler ValidPolygonFirstControlPointClick
        {
            add { AddHandler(ValidPolygonFirstControlPointClickEvent, value); }
            remove { RemoveHandler(ValidPolygonFirstControlPointClickEvent, value); }
        }

        // This method raises the ValidPolygonFirstControlPointClick event
        void RaiseValidPolygonFirstControlPointClickEvent()
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(ViewRegionControl.ValidPolygonFirstControlPointClickEvent);
            RaiseEvent(newEventArgs);
        }

        private void pointThumb_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                // Raise event just for polygon when the first thumb is clicked and the control points > 2
                if (drawingCanvas.Children.OfType<Thumb>().Count() > 2)
                    RaiseValidPolygonFirstControlPointClickEvent();
            }
        }

        void refreshPath()
        {
            if (_roiPoints.Count == 0)
                return;
            if (_pathPtr != null)
            {
                _pathPtr.Data = (Geometry)drawPath();
            }
            if (_selectionPathPtr != null)
            {
                _selectionPathPtr.Data = (Geometry)drawPath();
            }
        }

        object drawPath()
        {
            double scaleX, scaleY;
            scaleX = scaleY = ZoomScale;
            Geometry geo = null;

            switch (CurrentShape)
            {
                case RoiShape.Polygon:
                    geo = new StreamGeometry();
                    using (var ctx = ((StreamGeometry)geo).Open())
                    {
                        Point t = new Point(_roiPoints[0].X * scaleX, _roiPoints[0].Y * scaleY);
                        ctx.BeginFigure(t, true, true);
                        List<Point> data = new List<Point>();
                        for (int i = 1; i < _roiPoints.Count; i++)
                        {
                            t = new Point(_roiPoints[i].X * scaleX, _roiPoints[i].Y * scaleY);
                            data.Add(t);
                        }
                        ctx.PolyLineTo(data, true, false);
                    }
                    geo.Freeze();
                    break;
                case RoiShape.Rectangle:

                    if (_roiPoints.Count == 2)
                    {
                        Point t0 = new Point(_roiPoints[0].X * scaleX, _roiPoints[0].Y * scaleY);
                        Point t1 = new Point(_roiPoints[1].X * scaleX, _roiPoints[1].Y * scaleY);
                        geo = new RectangleGeometry(new Rect(t0, t1));
                    }
                    //else if (_roiPoints.Count == 3)
                    //{
                    //    Point t0 = new Point(_roiPoints[0].X * scaleX, _roiPoints[0].Y * scaleY);
                    //    Point t1 = new Point(_roiPoints[1].X * scaleX, _roiPoints[1].Y * scaleY);
                    //    Point t2 = new Point(_roiPoints[2].X * scaleX, _roiPoints[2].Y * scaleY);
                    //    Point center = new Point(Math.Abs(t0.X - t1.X), Math.Abs(t0.Y - t1.Y));
                    //    center = new Point(_roiPoints[0].X * scaleX, _roiPoints[0].Y * scaleY);
                    //    double angle = Math.Atan2(t2.Y - center.Y, t2.X - center.X) * 180 / Math.PI;
                    //    RotateTransform rotation = new RotateTransform(angle, t1.X, t1.Y);
                    //    Rect = new RectangleGeometry(new Rect(t0, t1), center.X, center.Y, rotation);                       
                    //}
                    break;
                case RoiShape.Circle:
                    if (_roiPoints.Count == 2)
                    {
                        Point t0 = new Point(_roiPoints[0].X * scaleX, _roiPoints[0].Y * scaleY);
                        Point t1 = new Point(_roiPoints[1].X * scaleX, _roiPoints[1].Y * scaleY);
                        Vector radius = new Vector();
                        radius = t0 - t1;
                        geo = new EllipseGeometry(t0, radius.Length, radius.Length);
                    }
                    break;
                case RoiShape.Ellipse:
                    if (_roiPoints.Count == 3)
                    {
                        Point t0 = new Point(_roiPoints[0].X * scaleX, _roiPoints[0].Y * scaleY);
                        Point t1 = new Point(_roiPoints[1].X * scaleX, _roiPoints[1].Y * scaleY);
                        Point t2 = new Point(_roiPoints[2].X * scaleX, _roiPoints[2].Y * scaleY);
                        Vector radiusX = new Vector();
                        radiusX = t0 - t1;
                        double angle = Math.Atan2(t1.Y - t0.Y, t1.X - t0.X) * 180 / Math.PI;
                        Vector radiusY = new Vector();
                        radiusY = t0 - t2;
                        RotateTransform Rotation = new RotateTransform(angle, t0.X, t0.Y);
                        geo = new EllipseGeometry(new Point(_roiPoints[0].X * scaleX, _roiPoints[0].Y * scaleY), radiusX.Length, radiusY.Length, Rotation);
                    }
                    break;
            }
            return geo;

        }

        object drawPath(Point p)
        {
            double scaleX, scaleY;
            scaleX = scaleY = ZoomScale;
            Geometry geo = null;

            switch (CurrentShape)
            {
                case RoiShape.Polygon:
                    {
                        geo = new StreamGeometry();
                        using (var ctx = ((StreamGeometry)geo).Open())
                        {
                            Point t = new Point(_roiPoints[0].X * scaleX, _roiPoints[0].Y * scaleY);
                            ctx.BeginFigure(t, true, false);
                            List<Point> data = new List<Point>();
                            for (int i = 1; i < _roiPoints.Count; i++)
                            {
                                t = new Point(_roiPoints[i].X * scaleX, _roiPoints[i].Y * scaleY);
                                data.Add(t);
                            }
                            t = new Point(p.X, p.Y);
                            data.Add(t);
                            ctx.PolyLineTo(data, true, false);
                        }
                        geo.Freeze();
                    }
                    break;
                case RoiShape.Rectangle:
                    if (_roiPoints.Count == 1)
                    {
                        Point t0 = new Point(_roiPoints[0].X * scaleX, _roiPoints[0].Y * scaleY);
                        Point t1 = new Point(p.X, p.Y);
                        geo = new RectangleGeometry(new Rect(t0, t1));
                    }
                    //else if (_roiPoints.Count == 3)
                    //{
                    //    Point t0 = new Point(_roiPoints[0].X * scaleX, _roiPoints[0].Y * scaleY);
                    //    Point t1 = new Point(_roiPoints[1].X * scaleX, _roiPoints[1].Y * scaleY);
                    //    Point t2 = new Point(_roiPoints[2].X * scaleX, _roiPoints[2].Y * scaleY);
                    //    Point center = new Point(Math.Abs(t0.X - t1.X), Math.Abs(t0.Y - t1.Y));
                    //    center = new Point(_roiPoints[0].X * scaleX, _roiPoints[0].Y * scaleY);
                    //    double angle = Math.Atan2(t2.Y - center.Y, t2.X - center.X) * 180 / Math.PI;
                    //    RotateTransform rotation = new RotateTransform(angle, t1.X, t1.Y);
                    //    Rect = new RectangleGeometry(new Rect(t0, t1), center.X, center.Y, rotation);                       
                    //}
                    break;
                case RoiShape.Circle:
                    {
                        Point t0 = new Point(_roiPoints[0].X * scaleX, _roiPoints[0].Y * scaleY);
                        Point t1 = new Point(p.X, p.Y);
                        Vector radius = new Vector();
                        radius = t0 - t1;
                        geo = new EllipseGeometry(t0, radius.Length, radius.Length);
                    }
                    break;
                case RoiShape.Ellipse:
                    {
                        Point t0; Point t1; Point t2;
                        Vector radiusX = new Vector();
                        Vector radiusY = new Vector();
                        double angle;
                        if (_roiPoints.Count == 1)
                        {
                            t0 = new Point(_roiPoints[0].X * scaleX, _roiPoints[0].Y * scaleY);
                            t1 = new Point(p.X, p.Y);
                        }
                        else //if (_roiPoints.Count == 2)
                        {
                            t0 = new Point(_roiPoints[0].X * scaleX, _roiPoints[0].Y * scaleY);
                            t1 = new Point(_roiPoints[1].X * scaleX, _roiPoints[1].Y * scaleY);
                            t2 = new Point(p.X, p.Y);
                            radiusY = t0 - t2;
                        }

                        radiusX = t0 - t1;
                        angle = Math.Atan2(t1.Y - t0.Y, t1.X - t0.X) * 180 / Math.PI;

                        RotateTransform Rotation = new RotateTransform(angle, t0.X, t0.Y);
                        geo = new EllipseGeometry(new Point(_roiPoints[0].X * scaleX, _roiPoints[0].Y * scaleY), radiusX.Length, radiusY.Length, Rotation);
                    }
                    break;
            }
            return geo;
        }

        private void updateDrawingTrail()
        {
            updateDrawingTrail(TrailPoint);
        }

        private void updateDrawingTrail(Point p)
        {
            if (_roiPoints.Count == 0)
                return;

            foreach (UIElement child in drawingCanvas.Children)
            {
                if (child.GetType() == typeof(Path))
                {
                    Path pathPtr = child as Path;

                    if (pathPtr != null)
                    {
                        pathPtr.Data = (Geometry)drawPath(p);
                        pathPtr.IsHitTestVisible = false;
                    }
                }
            }

        }

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsRoiSelected)
            {
                IsRoiSelected = false;
                e.Handled = true;
            }
        }

    }

}