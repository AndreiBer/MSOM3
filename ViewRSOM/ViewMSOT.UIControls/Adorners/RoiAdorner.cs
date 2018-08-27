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
using Xvue.MSOT.Services.Imaging;
using Xvue.MSOT.DataModels.Plugins.Imaging.Session;

namespace ViewMSOT.UIControls.Adorners
{
    public class RoiAdorner : Adorner
    {
        #region localvariables
        Color _penColor;
        //Color _textColor;
        Style _thumbStyle;
        Style _anchorThumbStyle;

        VisualCollection visualChildren;
        Canvas _raster;
        Path _pathPtr = null;
        bool _isStaticDrawing;

        ROIsViewStyle _roisStyle;

        double _thumbSize;
        #endregion localvariables

        public RoiAdorner(UIElement adornedElement, Color penColor, Style thumbStyle, Style anchorThumbStyle, bool staticDrawing, ROIsViewStyle roisStyle)
            : base(adornedElement)
        {
            var doubleVar = Application.Current.FindResource("ControlThumbsTouchAreaLength");
            _thumbSize = (double)doubleVar;
            visualChildren = new VisualCollection(this);
            _raster = new Canvas();
            visualChildren.Add(_raster);
            _penColor = penColor;
            //_textColor = textColor;
            _thumbStyle = thumbStyle;
            _anchorThumbStyle = anchorThumbStyle;
            _isStaticDrawing = staticDrawing;
            this.IsClipEnabled = true;
            this.IsHitTestVisible = true;
            AdornedElement.IsVisibleChanged += new DependencyPropertyChangedEventHandler(AdornedElement_IsVisibleChanged);
            _roisStyle = roisStyle;
        }

        void AdornedElement_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
                _raster.Visibility = System.Windows.Visibility.Visible;
            else
                _raster.Visibility = System.Windows.Visibility.Hidden;
                    
        }

#region properties

        public List<Point> PathPoints
        {
            get { return (List<Point>)GetValue(PathPointsProperty); }
        }

        public static readonly DependencyProperty PathPointsProperty =
           DependencyProperty.Register(
              "PathPoints",
              typeof(List<Point>),
              typeof(RoiAdorner),
              new FrameworkPropertyMetadata(new List<Point>(),
                new PropertyChangedCallback(ChangePathPoints)));

        private static void ChangePathPoints(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            (source as RoiAdorner)?.ChangePathPoints((IEnumerable<Point>)e.NewValue);
        }

        public void ChangePathPoints(IEnumerable<Point> newPoints)
        {
            try
            {
                PathPoints.Clear();
                foreach (Point p in newPoints)
                    PathPoints.Add(p);
                drawElements();
            }
            catch(Exception ex)
            {
                DllEntryPoint.LogMessage(Xvue.MSOT.Services.Log.EnumLogType.Warning, "Exception", "RoiAdorner.ChangePathPoints(): " + ex.Message);
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
              typeof(RoiAdorner),
              new FrameworkPropertyMetadata(
                new PropertyChangedCallback(ChangeZoom)));

        private static void ChangeZoom(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                (source as RoiAdorner).drawElements();
            }
            catch
            {
            }
        }
        

        
        ///////////////////////////////////////////////////////////////////
        

        public double PenColor
        {
            get { return (double)GetValue(PenColorProperty); }
            set { SetValue(PenColorProperty, value); }
        }

        public static readonly DependencyProperty PenColorProperty =
           DependencyProperty.Register(
              "PenColor",
              typeof(Color),
              typeof(RoiAdorner),
              new FrameworkPropertyMetadata(
                new PropertyChangedCallback(ChangePenColor)));

        private static void ChangePenColor(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                RoiAdorner roi = (source as RoiAdorner);
                roi._penColor = (Color)e.NewValue;
                roi.DrawStaticElements();
            }
            catch
            {
            }
        }

        
        /////////////////////////////////////////////////////////////
       



        public static readonly DependencyProperty IsDraggingProperty =
                   DependencyProperty.Register(
                      "IsDragging",
                      typeof(bool),
                      typeof(RoiAdorner));

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
                      typeof(RoiAdorner),
                      new FrameworkPropertyMetadata(
                        new PropertyChangedCallback(ChangeIsDrawing)));

        private static void ChangeIsDrawing(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                (source as RoiAdorner).drawElements();
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
                      typeof(RoiAdorner),
                      new FrameworkPropertyMetadata(
                        new PropertyChangedCallback(ChangeRoiDetails)));

        private static void ChangeRoiDetails(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                RoiAdorner roi = (source as RoiAdorner);
                roi.refreshDetails();
            }
            catch
            {
            }
        }

        public RoiShape CurrentShape
        {
            get { return (RoiShape)GetValue(CurrentShapeProperty); }
            set { SetValue(CurrentShapeProperty, value); }
        }

        public static readonly DependencyProperty CurrentShapeProperty =
                   DependencyProperty.Register(
                      "CurrentShape",
                      typeof(RoiShape),
                      typeof(RoiAdorner),
                      new FrameworkPropertyMetadata(
                        new PropertyChangedCallback(ChangeCurrentShape)));

        private static void ChangeCurrentShape(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                (source as RoiAdorner).drawElements();
            }
            catch
            {
            }
        }

#endregion properties

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

        public void DrawElements(bool isWorkable)
        {
            _isStaticDrawing = !isWorkable;
            drawElements();
        }

        private void drawElements()
        {
            if (_isStaticDrawing)
            {
                DrawStaticElements();
                return;
            }
            double scaleX, scaleY;
            scaleX = scaleY = Zoom;
            _raster.Children.Clear();
            if (PathPoints.Count == 0)
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
            foreach(Point p in PathPoints)
                thumbPoints.Add(new Point(p.X * scaleX, p.Y * scaleY));

            _pathPtr.Data = (Geometry) drawPath();
            _raster.Children.Add(_pathPtr);
            //_raster.Children.Add(createDetails());
            int k = 0;
            foreach (Point thumb in thumbPoints)
                BuildPointThumb(thumb, System.Windows.Input.Cursors.SizeAll, k++);
        }

        public void DrawStaticElements()
        {            
            //double scaleX, scaleY;
            //scaleX = scaleY = Zoom;
            _raster.Children.Clear();
            if (PathPoints.Count == 0)
                return;
            foreach (UIElement child in _raster.Children)
            {
                if (child.GetType() == typeof(Thumb))
                    removeThumbEvents((child as Thumb));
            }
            _pathPtr = new Path();
            if (_roisStyle == ROIsViewStyle.Fill)
            {
                _pathPtr.Fill = new SolidColorBrush(Color.FromArgb(128, _penColor.R, _penColor.G, _penColor.B));
                _pathPtr.IsHitTestVisible = false;
            }
            else
            {
                _pathPtr.Stroke = new SolidColorBrush(_penColor);
                _pathPtr.StrokeThickness = 1;
                _pathPtr.IsHitTestVisible = false;
            }
            
            _pathPtr.Data = (Geometry)drawPath();
            _raster.Children.Add(_pathPtr);
            //_raster.Children.Add(createDetails());
            
        }
        public void DrawStaticZoomElements()
        {
            //double scaleX, scaleY;
            //scaleX = scaleY = Zoom;
            _raster.Children.Clear();
            if (ZoomPathPoints.Count == 0)
                return;
            foreach (UIElement child in _raster.Children)
            {
                if (child.GetType() == typeof(Thumb))
                    removeThumbEvents((child as Thumb));
            }
            _pathPtr = new Path();
            _pathPtr.Stroke = new SolidColorBrush(_penColor);
            _pathPtr.StrokeThickness = 1;

            _pathPtr.Data = (Geometry)drawZoomPath();
            _raster.Children.Add(_pathPtr);
            //_raster.Children.Add(createDetails());

        }
        public void UpdateDrawingTrail(Point point)
        {
            if (PathPoints.Count == 0)
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
        public void UpdateZoomDrawingTrail(Point point)
        {
            if (ZoomPathPoints.Count == 0)
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
                path.Data = (Geometry)drawZoomPath(point);
                path.IsHitTestVisible = false;
            }
        }

        object drawPath()
        {
            double scaleX, scaleY;
            scaleX = scaleY = Zoom;
            Geometry geo = null;

            switch (CurrentShape)
            {
                case RoiShape.Polygon:
                    geo = new StreamGeometry();
                    using (var ctx = ((StreamGeometry)geo).Open())
                    {
                        Point t = new Point(PathPoints[0].X * scaleX, PathPoints[0].Y * scaleY);
                        ctx.BeginFigure(t, true, true);
                        List<Point> data = new List<Point>();
                        for (int i = 1; i < PathPoints.Count; i++)
                        {
                            t = new Point(PathPoints[i].X * scaleX, PathPoints[i].Y * scaleY);
                            data.Add(t);
                        }
                        ctx.PolyLineTo(data, true, false);
                    }
                    geo.Freeze();
                    break;
                case RoiShape.Rectangle:
                    
                    if (PathPoints.Count == 2)
                    {
                        Point t0 = new Point(PathPoints[0].X * scaleX, PathPoints[0].Y * scaleY);
                        Point t1 = new Point(PathPoints[1].X * scaleX, PathPoints[1].Y * scaleY);
                        geo = new RectangleGeometry(new Rect(t0, t1));
                    }
                    //else if (PathPoints.Count == 3)
                    //{
                    //    Point t0 = new Point(PathPoints[0].X * scaleX, PathPoints[0].Y * scaleY);
                    //    Point t1 = new Point(PathPoints[1].X * scaleX, PathPoints[1].Y * scaleY);
                    //    Point t2 = new Point(PathPoints[2].X * scaleX, PathPoints[2].Y * scaleY);
                    //    Point center = new Point(Math.Abs(t0.X - t1.X), Math.Abs(t0.Y - t1.Y));
                    //    center = new Point(PathPoints[0].X * scaleX, PathPoints[0].Y * scaleY);
                    //    double angle = Math.Atan2(t2.Y - center.Y, t2.X - center.X) * 180 / Math.PI;
                    //    RotateTransform rotation = new RotateTransform(angle, t1.X, t1.Y);
                    //    Rect = new RectangleGeometry(new Rect(t0, t1), center.X, center.Y, rotation);                       
                    //}
                    break;
                case RoiShape.Circle:
                    if (PathPoints.Count == 2)
                    {
                        Point t0 = new Point(PathPoints[0].X * scaleX, PathPoints[0].Y * scaleY);
                        Point t1 = new Point(PathPoints[1].X * scaleX, PathPoints[1].Y * scaleY);
                        Vector radius = new Vector();
                        radius = t0 - t1;
                        geo = new EllipseGeometry(t0, radius.Length, radius.Length);
                    }
                    break;
                case RoiShape.Ellipse:
                    if (PathPoints.Count == 3)
                    {
                        Point t0 = new Point(PathPoints[0].X * scaleX, PathPoints[0].Y * scaleY);
                        Point t1 = new Point(PathPoints[1].X * scaleX, PathPoints[1].Y * scaleY);
                        Point t2 = new Point(PathPoints[2].X * scaleX, PathPoints[2].Y * scaleY);
                        Vector radiusX = new Vector();
                        radiusX = t0 - t1;
                        double angle = Math.Atan2(t1.Y - t0.Y, t1.X - t0.X) * 180 / Math.PI;
                        Vector radiusY = new Vector();
                        radiusY = t0 - t2;
                        RotateTransform Rotation = new RotateTransform(angle, t0.X, t0.Y);
                        geo = new EllipseGeometry(new Point(PathPoints[0].X * scaleX, PathPoints[0].Y * scaleY), radiusX.Length, radiusY.Length, Rotation);                        
                    }
                    break;                    
            }
            return geo;

		}

        object drawPath(Point p)
        {
            double scaleX, scaleY;
            scaleX = scaleY = Zoom;
            Geometry geo = null;

            switch (CurrentShape)
            {
                case RoiShape.Polygon:
                    {
                        geo = new StreamGeometry();
                        using (var ctx = ((StreamGeometry)geo).Open())
                        {
                            Point t = new Point(PathPoints[0].X * scaleX, PathPoints[0].Y * scaleY);
                            ctx.BeginFigure(t, true, false);
                            List<Point> data = new List<Point>();
                            for (int i = 1; i < PathPoints.Count; i++)
                            {
                                t = new Point(PathPoints[i].X * scaleX, PathPoints[i].Y * scaleY);
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
                    if (PathPoints.Count == 1)
                    {
                        Point t0 = new Point(PathPoints[0].X * scaleX, PathPoints[0].Y * scaleY);
                        Point t1 = new Point(p.X, p.Y);
                        geo = new RectangleGeometry(new Rect(t0, t1));
                    }
                    //else if (PathPoints.Count == 3)
                    //{
                    //    Point t0 = new Point(PathPoints[0].X * scaleX, PathPoints[0].Y * scaleY);
                    //    Point t1 = new Point(PathPoints[1].X * scaleX, PathPoints[1].Y * scaleY);
                    //    Point t2 = new Point(PathPoints[2].X * scaleX, PathPoints[2].Y * scaleY);
                    //    Point center = new Point(Math.Abs(t0.X - t1.X), Math.Abs(t0.Y - t1.Y));
                    //    center = new Point(PathPoints[0].X * scaleX, PathPoints[0].Y * scaleY);
                    //    double angle = Math.Atan2(t2.Y - center.Y, t2.X - center.X) * 180 / Math.PI;
                    //    RotateTransform rotation = new RotateTransform(angle, t1.X, t1.Y);
                    //    Rect = new RectangleGeometry(new Rect(t0, t1), center.X, center.Y, rotation);                       
                    //}
                    break;
                case RoiShape.Circle:
                    {
                        Point t0 = new Point(PathPoints[0].X * scaleX, PathPoints[0].Y * scaleY);
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
                        if (PathPoints.Count == 1)
                        {
                            t0 = new Point(PathPoints[0].X * scaleX, PathPoints[0].Y * scaleY);
                            t1 = new Point(p.X, p.Y);
                        }
                        else //if (PathPoints.Count == 2)
                        {
                            t0 = new Point(PathPoints[0].X * scaleX, PathPoints[0].Y * scaleY);
                            t1 = new Point(PathPoints[1].X * scaleX, PathPoints[1].Y * scaleY);
                            t2 = new Point(p.X, p.Y);
                            radiusY = t0 - t2;
                        }

                        radiusX = t0 - t1;
                        angle = Math.Atan2(t1.Y - t0.Y, t1.X - t0.X) * 180 / Math.PI;

                        RotateTransform Rotation = new RotateTransform(angle, t0.X, t0.Y);
                        geo = new EllipseGeometry(new Point(PathPoints[0].X * scaleX, PathPoints[0].Y * scaleY), radiusX.Length, radiusY.Length, Rotation);
                    }
                    break;
            }
            return geo;
        }

        object drawZoomPath(Point p)
        {
            double scaleX, scaleY;
            scaleX = scaleY = Zoom;
            object geo = null;

            if (ZoomPathPoints.Count == 1)
            {
                Point t0 = new Point(ZoomPathPoints[0].X * scaleX, ZoomPathPoints[0].Y * scaleY);
                Point t1 = new Point(p.X, p.Y);
                geo = new RectangleGeometry(new Rect(t0, t1));
            }
            return geo;
        }

        object drawZoomPath()
        {
            double scaleX, scaleY;
            scaleX = scaleY = Zoom;
            object geo = null;

            geo = new StreamGeometry();
            if (ZoomPathPoints.Count == 2)
            {
                Point t0 = new Point(ZoomPathPoints[0].X * scaleX, ZoomPathPoints[0].Y * scaleY);
                Point t1 = new Point(ZoomPathPoints[1].X * scaleX, ZoomPathPoints[1].Y * scaleY);
                geo = new RectangleGeometry(new Rect(t0, t1));
            }
            return geo;
        }

        void refreshPath()
        {
            if (PathPoints.Count == 0)
                return;            
            if (_pathPtr != null)
            {
                _pathPtr.Data = (Geometry) drawPath();
                //_pathPtr.IsHitTestVisible = true;
            }
        }

        void refreshDetails()
        {
            Thumb thumbPtr;
            foreach (UIElement child in _raster.Children)
            {
                if (child.GetType() == typeof(Thumb))
                {
                    thumbPtr = child as Thumb;
                    thumbPtr.ToolTip = RoiDetails;
                }
            }
        }

        void BuildPointThumb(Point t, Cursor customizedCursor,int i)
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
            _raster.Children.Add(pointThumb);
        }

        void _pathPoints_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
                drawElements();
        }

        void _zoomPathPoints_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
                DrawStaticZoomElements();
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
            Point p = PathPoints[index];
            double x = (p.X * Zoom + dx) / Zoom;
            double y = (p.Y * Zoom + dy) / Zoom;
            if (index == 0)
            {
                for (int i = 1; i < PathPoints.Count; i++)
                {
                    double locX = (PathPoints[i].X * Zoom + dx) / Zoom;
                    double locY = (PathPoints[i].Y * Zoom + dy) / Zoom;
                    PathPoints[i] = new Point(locX, locY);
                    foreach (UIElement child in _raster.Children)
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
            PathPoints[index] = new Point(x, y);
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

        public bool IsZoomDrawing
        {
            get { return (bool)GetValue(IsZoomDrawingProperty); }
            set { SetValue(IsZoomDrawingProperty, value); }
        }

        public static readonly DependencyProperty IsZoomDrawingProperty =
                   DependencyProperty.Register(
                      "IsZoomDrawing",
                      typeof(bool),
                      typeof(RoiAdorner),
                      new FrameworkPropertyMetadata(
                        new PropertyChangedCallback(ChangeIsZoomDrawing)));

        private static void ChangeIsZoomDrawing(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                (source as RoiAdorner).drawElements();
            }
            catch
            {
            }
        }

        public List<Point> ZoomPathPoints
        {
            get { return (List<Point>)GetValue(ZoomPathPointsProperty); }
        }

        public static readonly DependencyProperty ZoomPathPointsProperty =
           DependencyProperty.Register(
              "ZoomPathPoints",
              typeof(List<Point>),
              typeof(RoiAdorner),
              new FrameworkPropertyMetadata(new List<Point>(),
                new PropertyChangedCallback(ChangeZoomPathPoints)));

        private static void ChangeZoomPathPoints(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                RoiAdorner roi = (source as RoiAdorner);
                roi.ZoomPathPoints.Clear();
                foreach (Point p in (IEnumerable<Point>)e.NewValue)
                    roi.ZoomPathPoints.Add(p);
                roi.DrawStaticZoomElements();
            }
            catch(Exception ex)
            {
                DllEntryPoint.LogMessage(Xvue.MSOT.Services.Log.EnumLogType.Warning, "Exception", "RoiAdorner.ChangeZoomPathPoints(): " + ex.Message);
            }
        }
    }
}
