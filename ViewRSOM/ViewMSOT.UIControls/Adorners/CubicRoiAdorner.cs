using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace ViewMSOT.UIControls.Adorners
{
    public class CubicRoiAdorner:FrameworkElement
    {
        #region localvariables
        RectangularRoiAdorner[] _rectAdorners;
        const int _xy = 0;
        const int _yz = 1;
        const int _xz = 2;
        #endregion localvariables
        public CubicRoiAdorner(UIElement[] adornedElements, Color penColor, Style thumbStyle, Style anchorThumbStyle)
        {
            _rectAdorners = new RectangularRoiAdorner[3];
            _rectAdorners[_xy] = new RectangularRoiAdorner(_xy,adornedElements[_xy], penColor, thumbStyle, anchorThumbStyle,this);
            _rectAdorners[_yz] = new RectangularRoiAdorner(_yz,adornedElements[_yz], penColor, thumbStyle, anchorThumbStyle, this);
            _rectAdorners[_xz] = new RectangularRoiAdorner(_xz,adornedElements[_xz], penColor, thumbStyle, anchorThumbStyle,this);

            _rectAdorners[_xy]._adornedGrid.SizeChanged += _adornedGrid_SizeChanged;
            _rectAdorners[_yz]._adornedGrid.SizeChanged += _adornedGrid_SizeChanged;
            _rectAdorners[_xz]._adornedGrid.SizeChanged += _adornedGrid_SizeChanged;
        }

        public void Remove()
        {
            for (int i = 0; i < _rectAdorners.Length; i++)
            {
                _rectAdorners[i]._adornedGrid.SizeChanged -= _adornedGrid_SizeChanged;
                _rectAdorners[i].Remove();
            }
        }

        void _adornedGrid_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                //updateParentPoints(true);
                setPoint3D(0, P0);
                setPoint3D(1, P1);
            }
            catch (Exception ex)
            {
                Xvue.MSOT.ViewModels.Log.ViewModelLog.MsotTrace("_adornedGrid_SizeChanged exception: " + ex.Message, this.GetType().ToString());
            }
        }

        public Point3D P0
        {
            get { return (Point3D)GetValue(P0Property); }
            set { SetValue(P0Property, value); }
        }

        public static readonly DependencyProperty P0Property =
           DependencyProperty.Register(
              "P0",
              typeof(Point3D),
              typeof(CubicRoiAdorner),
              new FrameworkPropertyMetadata(
                new PropertyChangedCallback(P0Changed)));

        private static void P0Changed(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                CubicRoiAdorner roi = (source as CubicRoiAdorner);
                roi.setPoint3D(0, roi.P0);
            }
            catch { }
        }

        public Point3D P1
        {
            get { return (Point3D)GetValue(P1Property); }
            set { SetValue(P1Property, value); }
        }

        public static readonly DependencyProperty P1Property =
           DependencyProperty.Register(
              "P1",
              typeof(Point3D),
              typeof(CubicRoiAdorner),
              new FrameworkPropertyMetadata(
                new PropertyChangedCallback(P1Changed)));

        private static void P1Changed(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                CubicRoiAdorner roi = (source as CubicRoiAdorner);
                roi.setPoint3D(1, roi.P1);
            }
            catch { }
        }

        void setPoint3D(int index, Point3D p)
        {                       
            setPointXY(index, p.X, p.Y);
            setPointYZ(index, p.Z, p.Y);
            setPointXZ(index, p.X, p.Z);
        }

        void setPointXY(int index, double x, double y)
        {
            _rectAdorners[_xy].setPoint(index, x, y);
        }

        void setPointYZ(int index, double x, double y)
        {
            _rectAdorners[_yz].setPoint(index, x, y);
        }

        void setPointXZ(int index, double x, double y)
        {
            _rectAdorners[_xz].setPoint(index, x,y);
        }

        public void RedrawPoints(int viewIndex, int pointIndex, Point point, bool dragFinished)
        {
            try
            {
                Point3D pi;
                Point3D pnew = new Point3D();
                if (pointIndex == 0) pi = P0;
                else pi = P1;
                switch (viewIndex)
                {
                    case _xy:
                        setPointYZ(pointIndex, pi.Z, point.Y);
                        setPointXZ(pointIndex, point.X, pi.Z);
                        pnew = new Point3D(point.X, point.Y, pi.Z);
                        break;
                    case _yz:
                        setPointXY(pointIndex, pi.X, point.Y);
                        setPointXZ(pointIndex, pi.X, point.X);
                        pnew = new Point3D(pi.X, point.Y, point.X);
                        break;
                    case _xz:
                        setPointXY(pointIndex, point.X, pi.Y);
                        setPointYZ(pointIndex, point.Y, pi.Y);
                        pnew = new Point3D(point.X, pi.Y, point.Y);
                        break;
                }
                if (dragFinished)
                {
                    if (pointIndex == 0) P0 = pnew;
                    else P1 = pnew;
                }
            }
            catch { }
        }

        protected override int VisualChildrenCount { get { return _rectAdorners.Length; } }  
        protected override Visual GetVisualChild(int index) { return _rectAdorners[index]; } 

        
    }
    public class RectangularRoiAdorner : Adorner
    {
        #region localvariables
        Color _penColor;
        Style _thumbStyle;
        Style _anchorThumbStyle;
        Point[] _pathPoints;
        CubicRoiAdorner _parent;
        VisualCollection visualChildren;
        Canvas _raster;
        Path _pathPtr = null;
        public Grid _adornedGrid;
        int _viewIndex;
        double _thumbSize;
        #endregion localvariables
        public RectangularRoiAdorner(int viewIndex, UIElement adornedElement, Color penColor, Style thumbStyle, Style anchorThumbStyle, CubicRoiAdorner parent)
            : base(adornedElement)
        {
            try
            {
                var doubleVar = Application.Current.FindResource("ControlThumbsTouchAreaLength");
                _thumbSize = (double)doubleVar;
                _parent = parent;
                _viewIndex = viewIndex;
                AdornerLayer imageAdornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
                _adornedGrid = adornedElement as Grid;
                visualChildren = new VisualCollection(this);
                _raster = new Canvas();
                visualChildren.Add(_raster);
                _penColor = penColor;
                _thumbStyle = thumbStyle;
                _anchorThumbStyle = anchorThumbStyle;
                this.IsClipEnabled = true;
                this.IsHitTestVisible = true;
                _pathPoints = new Point[2];
                AdornedElement.IsVisibleChanged += AdornedElement_IsVisibleChanged;
                imageAdornerLayer.Add(this);
            }
            catch (Exception ex)
            {
                Xvue.MSOT.ViewModels.Log.ViewModelLog.MsotTrace("RectangularRoiAdorner exception: " + ex.Message, this.GetType().ToString());
            }
        }

        public void Remove()
        {
            AdornedElement.IsVisibleChanged -= AdornedElement_IsVisibleChanged;
            AdornerLayer imageAdornerLayer = AdornerLayer.GetAdornerLayer(_adornedGrid);
            imageAdornerLayer.Remove(this);
        }
        void AdornedElement_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
                _raster.Visibility = System.Windows.Visibility.Visible;
            else
                _raster.Visibility = System.Windows.Visibility.Hidden;
        }

        protected override int VisualChildrenCount { get { return 1; } }  //visualChildren.Count; } }
        protected override Visual GetVisualChild(int index) { return _raster; } //visualChildren[index]; }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (Visual child in visualChildren)
            {
                (child as FrameworkElement).Arrange(new Rect(finalSize));
            }
            return finalSize;
        }
        void addThumbEvents(Thumb thumb)
        {
            thumb.DragDelta += new DragDeltaEventHandler(pointThumb_DragDelta);
            thumb.DragCompleted += new DragCompletedEventHandler(pointThumb_DragCompleted);
        }

        void removeThumbEvents(Thumb thumb)
        {
            thumb.DragDelta -= pointThumb_DragDelta;
            thumb.DragCompleted -= pointThumb_DragCompleted;
        }

        internal void setPoint(int index, double X, double Y)
        {
            try
            {
                _pathPoints[index] = new Point(X * _adornedGrid.ActualWidth, Y * _adornedGrid.ActualHeight);
                drawElements();
            }
            catch { }
        }

        void pointThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            updateParentPoints(true);
        }

        void updateParentPoints(bool dragFinished)
        {
            try
            {
                Point p0 = new Point(_pathPoints[0].X / _adornedGrid.ActualWidth, _pathPoints[0].Y / _adornedGrid.ActualHeight);
                Point p1 = new Point(_pathPoints[1].X / _adornedGrid.ActualWidth, _pathPoints[1].Y / _adornedGrid.ActualHeight);
                _parent.RedrawPoints(_viewIndex, 0, p0, dragFinished);
                _parent.RedrawPoints(_viewIndex, 1, p1, dragFinished);
            }
            catch { }
        }


        void trimLimits(ref Point pp)
        {
            double d = 0;
            if (pp.X < d) pp.X = d;
            if (pp.Y < d) pp.Y = d;
            if (pp.X > _adornedGrid.ActualWidth - d)
                pp.X = _adornedGrid.ActualWidth - d;
            if (pp.Y > _adornedGrid.ActualHeight - d) pp.Y = _adornedGrid.ActualHeight - d;
        }

        static Point trimOrder(Point p0, Point p1, int target)
        {
            Point newP = p0;
            Point oldP = p1;
            if (target == 1)
            {
                newP = p1;
                oldP = p0;
            }
            if (p0.X - p1.X > -2)
                newP.X = oldP.X + 2;
            if (p0.Y - p1.Y > -2)
                newP.Y = oldP.Y + 2;
            return newP;
        }

        void pointThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            try
            {
                Thumb hitThumb = sender as Thumb;
                int thumbIndex = (int)hitThumb.Tag;
                double dx = e.HorizontalChange, dy = e.VerticalChange;
                
                int index = (int)hitThumb.Tag;
                double x = (_pathPoints[index].X + dx);
                double y = (_pathPoints[index].Y + dy);
                Point pp = new Point(x, y);
                trimLimits(ref pp);
                Point pp0, pp1;
                if (thumbIndex == 0)
                {
                    pp0 = pp;
                    pp1 = _pathPoints[1];
                }
                else
                {
                    pp0 = _pathPoints[0];
                    pp1 = pp;
                }
                pp = trimOrder(pp0, pp1, thumbIndex);
                _pathPoints[index] = new Point(pp.X, pp.Y);
                refreshPath();
                Canvas.SetLeft(hitThumb, pp.X - _thumbSize / 2);
                Canvas.SetTop(hitThumb, pp.Y - _thumbSize / 2);
                updateParentPoints(false);
            }
            catch { }
        }

        private void drawElements()
        {
            _raster.Children.Clear();
            foreach (UIElement child in _raster.Children)
            {
                if (child.GetType() == typeof(Thumb))
                    removeThumbEvents((child as Thumb));
            }
            _pathPtr = new Path();
            _pathPtr.Stroke = new SolidColorBrush(_penColor);
            _pathPtr.StrokeThickness = 1;
            List<Point> thumbPoints = new List<Point>();
            foreach (Point p in _pathPoints)
                thumbPoints.Add(new Point(p.X, p.Y));

            _pathPtr.Data = (Geometry)drawPath();
            _raster.Children.Add(_pathPtr);
            //_raster.Children.Add(createDetails());
            int k = 0;
            foreach (Point thumb in thumbPoints)
                BuildPointThumb(thumb, System.Windows.Input.Cursors.SizeAll, k++);
        }

        object drawPath()
        {
            double scaleX, scaleY;
            scaleX = scaleY = 1;// Zoom;
            object geo = null;
            Point t0 = new Point(_pathPoints[0].X * scaleX, _pathPoints[0].Y * scaleY);
            Point t1 = new Point(_pathPoints[1].X * scaleX, _pathPoints[1].Y * scaleY);
            geo = new RectangleGeometry(new Rect(t0, t1));
            return geo;
        }

        void refreshPath()
        {
            if (_pathPtr != null)
            {
                _pathPtr.Data = (Geometry)drawPath();
                //_pathPtr.IsHitTestVisible = true;
            }
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
            addThumbEvents(pointThumb);
            _raster.Children.Add(pointThumb);
        }
    }
}
