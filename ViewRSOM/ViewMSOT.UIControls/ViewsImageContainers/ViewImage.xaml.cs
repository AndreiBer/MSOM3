using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using ViewMSOT.UIControls.Adorners;
using Xvue.Framework.Views.WPF.Behaviors;
using Xvue.Framework.Views.WPF.Controls;
using Xvue.MSOT.Services.Imaging;
using Xvue.MSOT.ViewModels.Imaging;

namespace ViewMSOT.UIControls
{
    /// <summary>
    /// Interaction logic for ViewImage.xaml
    /// </summary>
    public partial class ViewImage : ViewImageBase
    {
        #region localvariables
        bool dragging = false;
        Point _p0;
        double _y0;
        double _x0;

        Point _imageAnchorPointBeforeZoom;
        Point _imageMouseOverPointBeforeZoom;
        Point _imageViewPortCenterBeforeZoom;
        double _lastZoom;
        bool _wasMouseOverImage;
        bool _isMouseWheelZoom;
        bool _isTouchZoom;
        List<Point> _zoomPoints;
        List<Point> _placementPoints;

        InfoAdorner _infoAdorner;
        GridAdorner _gridAdorner;
        AdornerLayer imageAdornerLayer;

        Dictionary<TouchDevice, Point> _touchDevicesPoints;
        double _touchZoomDistance;
        Point _touchZoomPointBeforeZoom;
        bool _touchEnabled;
        private MouseClickOutsideControlBehavior _mouseClickBehavior;
        private bool _contextMenuAllowedToOpen;
        #endregion localvariables

        public string GridPercentageWidthStepSize;

        public int ViewingPlaneIndex;

        #region properties
        public double RoiLowX
        {
            get { return (double)GetValue(RoiLowXProperty); }
            set { SetValue(RoiLowXProperty, value); }
        }

        public static readonly DependencyProperty RoiLowXProperty =
           DependencyProperty.Register(
              "RoiLowX",
              typeof(double),
              typeof(ViewImage),
              new FrameworkPropertyMetadata(RoiChanged));

        public double RoiHighX
        {
            get { return (double)GetValue(RoiHighXProperty); }
            set { SetValue(RoiHighXProperty, value); }
        }

        public static readonly DependencyProperty RoiHighXProperty =
           DependencyProperty.Register(
              "RoiHighX",
              typeof(double),
              typeof(ViewImage),
              new FrameworkPropertyMetadata(
                new PropertyChangedCallback(RoiChanged)));

        public double RoiLowY
        {
            get { return (double)GetValue(RoiLowYProperty); }
            set { SetValue(RoiLowYProperty, value); }
        }

        public static readonly DependencyProperty RoiLowYProperty =
           DependencyProperty.Register(
              "RoiLowY",
              typeof(double),
              typeof(ViewImage),
              new FrameworkPropertyMetadata(
                new PropertyChangedCallback(RoiChanged)));

        public double RoiHighY
        {
            get { return (double)GetValue(RoiHighYProperty); }
            set { SetValue(RoiHighYProperty, value); }
        }

        public static readonly DependencyProperty RoiHighYProperty =
           DependencyProperty.Register(
              "RoiHighY",
              typeof(double),
              typeof(ViewImage),
              new FrameworkPropertyMetadata(
                new PropertyChangedCallback(RoiChanged)));


        public double ImageHeight
        {
            get { return (double)GetValue(ImageHeightProperty); }
            set { SetValue(ImageHeightProperty, value); }
        }

        public static readonly DependencyProperty ImageHeightProperty =
            DependencyProperty.Register(
              "ImageHeight",
              typeof(double),
              typeof(ViewImage),
              new FrameworkPropertyMetadata(
                new PropertyChangedCallback(ImageSizeChanged)));

        public double RoiYOffset
        {
            get { return (double)GetValue(RoiYOffsetProperty); }
            set { SetValue(RoiYOffsetProperty, value); }
        }

        public static readonly DependencyProperty RoiYOffsetProperty =
           DependencyProperty.Register(
              "RoiYOffset",
              typeof(double),
              typeof(ViewImage),
              new FrameworkPropertyMetadata(
                new PropertyChangedCallback(RoiChanged)));

        public double RoiXOffset
        {
            get { return (double)GetValue(RoiXOffsetProperty); }
            set { SetValue(RoiXOffsetProperty, value); }
        }

        public static readonly DependencyProperty RoiXOffsetProperty =
           DependencyProperty.Register(
              "RoiXOffset",
              typeof(double),
              typeof(ViewImage),
              new FrameworkPropertyMetadata(
                new PropertyChangedCallback(RoiChanged)));

        public bool IsROIDrawEnabled
        {
            get { return (bool)GetValue(IsROIDrawEnabledProperty); }
            set { SetValue(IsROIDrawEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsROIDrawEnabledProperty =
           DependencyProperty.Register(
              "IsROIDrawEnabled",
              typeof(bool),
              typeof(ViewImage),
              new FrameworkPropertyMetadata());

        public double ImageWidth
        {
            get { return (double)GetValue(ImageWidthProperty); }
            set { SetValue(ImageWidthProperty, value); }
        }

        public static readonly DependencyProperty ImageWidthProperty =
           DependencyProperty.Register(
              "ImageWidth",
              typeof(double),
              typeof(ViewImage),
              new FrameworkPropertyMetadata(
                new PropertyChangedCallback(ImageSizeChanged)));

        public double MinimumZoomAllowed
        {
            get { return (double)GetValue(MinimumZoomAllowedProperty); }
            set { SetValue(MinimumZoomAllowedProperty, value); }
        }

        public static readonly DependencyProperty MinimumZoomAllowedProperty =
           DependencyProperty.Register(
              "MinimumZoomAllowed",
              typeof(double),
              typeof(ViewImage),
              new FrameworkPropertyMetadata());

        public IEnumerable<ViewModelRegion2DAllLayersDrawing> Region2DAllLayers
        {
            get { return (IEnumerable<ViewModelRegion2DAllLayersDrawing>)GetValue(Region2DAllLayersProperty); }
            set { SetValue(Region2DAllLayersProperty, value); }
        }

        public static readonly DependencyProperty Region2DAllLayersProperty =
           DependencyProperty.Register(
              "Region2DAllLayers",
              typeof(IEnumerable<ViewModelRegion2DAllLayersDrawing>),
              typeof(ViewImage));

        public double ZoomInfo
        {
            get { return (double)GetValue(ZoomInfoProperty); }
            set { SetValue(ZoomInfoProperty, value); }
        }

        public static readonly DependencyProperty ZoomInfoProperty =
           DependencyProperty.Register(
              "ZoomInfo",
              typeof(double),
              typeof(ViewImage),
              new FrameworkPropertyMetadata());

        protected override Border ImageCanvasBorder
        {
            get
            {
                return imageCanvasBorder;
            }
        }

        protected override Canvas PlacementCanvas
        {
            get
            {
                return placementCanvas;
            }
        }

        protected override double ZoomHeight
        {
            get
            {
                return Math.Abs(_model.ZoomPathPoints[0].Y - _model.ZoomPathPoints[1].Y);
            }
        }

        protected override double ZoomWidth
        {
            get
            {
                return Math.Abs(_model.ZoomPathPoints[0].X - _model.ZoomPathPoints[1].X);
            }
        }

        protected override Rectangle ZpViewportRect
        {
            get
            {
                return zpViewportRect;
            }
        }

        protected override Viewbox ZpViewBox
        {
            get
            {
                return zpViewBox;
            }
        }

        protected override Border ZpImageBorder
        {
            get
            {
                return zpImageBorder;
            }
        }

        #endregion properties

        public ViewImage()
        {
            _touchDevicesPoints = new Dictionary<TouchDevice, Point>();
            _touchZoomDistance = 0;
            _touchZoomPointBeforeZoom = new Point();
            _touchEnabled = false;

            this.InitializeComponent();
        }

        public override void ChangeIsRulerDrawing(bool newValue)
        {
            try
            {
                if (newValue)
                {
                    if (_model.IsZoomDrawing && IsROIDrawEnabled)
                    {
                        _model.IsZoomDrawing = false;
                        _isRectangularZoom = false;
                        _zoomPoints.Clear();
                        _placementPoints.Clear();
                    }
                    _model.ImageProperties.DrawingRegions2D.FinishRoiDrawing();
                }
            }
            catch
            {
            }
        }

        protected override void RefreshInfoAdorner()
        {
            if (_infoAdorner != null)
                _infoAdorner.InvalidateVisual();
            if (_gridAdorner != null)
                _gridAdorner.RefreshPositionsAndDraw();
        }

        protected override void RefreshMinimumZoom()
        {
            if (ImageHeight > 0 && ImageWidth > 0 && borderGrid.ActualHeight > 0 && borderGrid.ActualWidth > 0)
            {
                double heightMinimumZoom = borderGrid.ActualHeight / (ImageHeight + 30);
                double widthMinimumZoom = borderGrid.ActualWidth / (ImageWidth + 30);

                if (heightMinimumZoom < widthMinimumZoom)
                    MinimumZoomAllowed = heightMinimumZoom;
                else
                    MinimumZoomAllowed = widthMinimumZoom;
            }
        }

        protected override void UpdateZoom(double newValue, double oldValue)
        {
            if (backgroundImage.Source != null)
            {
                _imageAnchorPointBeforeZoom = new Point(Canvas.GetLeft(imageCanvasBorder), Canvas.GetTop(imageCanvasBorder));

                if (_isTouchZoom)
                {
                    _touchZoomPointBeforeZoom = touchDevicesMidPoint(imageCanvasBorder);
                }
                else if (!_isRectangularZoom)
                {
                    _wasMouseOverImage = backgroundImage.IsMouseOver;
                    _imageMouseOverPointBeforeZoom = Mouse.GetPosition(imageCanvasBorder);
                    _imageViewPortCenterBeforeZoom = new Point(placementCanvas.ActualWidth / 2.0 - _imageAnchorPointBeforeZoom.X, placementCanvas.ActualHeight / 2.0 - _imageAnchorPointBeforeZoom.Y);
                }

                _lastZoom = oldValue;
                backgroundImage.Height = backgroundImage.Source.Height * newValue;
                backgroundImage.Width = backgroundImage.Source.Width * newValue;
            }
        }



        private static void ImageSizeChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                if (e.NewValue != null && e.NewValue != e.OldValue)
                    (source as ViewImage).RefreshMinimumZoom();
            }
            catch { }
        }

        static Point midPoint(Point a, Point b)
        {
            return new Point((a.X + b.X) / 2, (a.Y + b.Y) / 2);
        }

        private static void RoiChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null && e.NewValue != e.OldValue)
                (source as ViewImage).RefreshInfoAdorner();
        }
        private void backgroundImageTargetUpdated(object sender, DataTransferEventArgs e)
        {
            try
            {
                //System.Windows.Controls.Image image = e.Source as System.Windows.Controls.Image;
                //double imageZoom = backgroundImage.ActualHeight / backgroundImage.Source.Height;
                //Zoom = imageZoom;
                //if (imageZoom != Zoom)
                //  backgroundImage.Height = Zoom * backgroundImage.Source.Height;
                if (backgroundImage.Source != null)
                {
                    backgroundImage.Height = Zoom * backgroundImage.Source.Height;
                    backgroundImage.Width = Zoom * backgroundImage.Source.Width;
                }
            }
            catch
            {
                Zoom = 1;
                backgroundImage.Height = Zoom * backgroundImage.Source.Height;
                backgroundImage.Width = Zoom * backgroundImage.Source.Width;
            }
        }

        private void borderGridMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!_model.IsZoomDrawing)
            {
                _isMouseWheelZoom = true;
                DeltaZoom(e.Delta);
            }
            e.Handled = true;
        }

        private void borderGridTouchEnter(object sender, TouchEventArgs e)
        {
            _touchDevicesPoints[e.TouchDevice] = e.GetTouchPoint(borderGrid).Position;

            if (_touchDevicesPoints.Count == 2)
            {
                List<TouchDevice> devices = new List<TouchDevice>(_touchDevicesPoints.Keys);
                _touchDevicesPoints[devices[0]] = devices[0].GetTouchPoint(borderGrid).Position;
                _touchDevicesPoints[devices[1]] = devices[1].GetTouchPoint(borderGrid).Position;

                List<Point> points = new List<Point>(_touchDevicesPoints.Values);
                _touchZoomDistance = Point.Subtract(points[0], points[1]).Length;

                _touchEnabled = true;
                dragging = false;
                e.Handled = true;
            }
        }

        private void borderGridTouchLeave(object sender, TouchEventArgs e)
        {
            _touchDevicesPoints.Remove(e.TouchDevice);

            if (_touchDevicesPoints.Count != 2)
            {
                _touchEnabled = false;
            }
        }

        private void borderGridTouchMove(object sender, TouchEventArgs e)
        {
            if (_touchDevicesPoints.Count != 2)
            {
                return;
            }
            else
            {
                Point newPoint = e.GetTouchPoint(borderGrid).Position;

                if (!newPoint.Equals(_touchDevicesPoints[e.TouchDevice]))
                {
                    _touchDevicesPoints[e.TouchDevice] = newPoint;

                    List<Point> points = new List<Point>(_touchDevicesPoints.Values);

                    double newTouchZoomDistance = Point.Subtract(points[0], points[1]).Length;
                    double diff = newTouchZoomDistance - _touchZoomDistance;

                    if (Math.Abs(diff) > 5)
                    {
                        _isTouchZoom = true;

                        double a = newTouchZoomDistance / _touchZoomDistance;
                        _touchZoomDistance = newTouchZoomDistance;

                        deltaInfoZoom(a);
                    }
                }
                e.Handled = true;
            }
        }

        private void cancelAllDrawing()
        {
            if (_model.ImageProperties.DrawingRegions2D.IsRoiDrawing)
            {
                _model.ImageProperties.DrawingRegions2D.FinishRoiDrawing();
            }
            else if (_model.ImageProperties.RulersViewingPlanes.IsRulerDrawingInit())
            {
                _model.ImageProperties.RulersViewingPlanes.CancelRulerDrawing();
            }
            else if (_model.IsZoomDrawing)
            {
                _model.IsZoomDrawing = false;
                _isRectangularZoom = false;
                _zoomPoints.Clear();
                _placementPoints.Clear();
            }
        }

        private void ContextMenuOpened(object sender, RoutedEventArgs e)
        {
            if (ContextMenu.Visibility != System.Windows.Visibility.Visible)
                ContextMenu.SetCurrentValue(ContextMenu.IsOpenProperty, false);
        }

        private void UserControl_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            e.Handled = !_contextMenuAllowedToOpen;
            if (!_contextMenuAllowedToOpen && !IsCurrentROIDrawingCancellable())
            {
                // reset immediatelly for next time (click)
                _contextMenuAllowedToOpen = true;
            }
        }

        void deltaInfoZoom(double delta)
        {
            ZoomInfo *= delta;
        }

        private void HandleClickOutsideOfControl()
        {
            _mouseClickBehavior.UnregisterElement();
            cancelAllDrawing();
        }

        private void imageCanvasMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            bool monitorMouseClickBehaviour = false;

            if (_touchEnabled)
                return;

            if (_model.ImageProperties.DrawingRegions2D.IsRoiDrawing && IsROIDrawEnabled)
            {
                monitorMouseClickBehaviour = true;
                Point imagePoint = e.GetPosition(backgroundImage);
                _model.ImageProperties.DrawingRegions2D.AddRoiPoint(new Point(imagePoint.X / Zoom, imagePoint.Y / Zoom));
                if(! _model.ImageProperties.DrawingRegions2D.IsRoiDrawing)
                    monitorMouseClickBehaviour = false;
                if (_model.ImageProperties.RulersViewingPlanes.IsRulerDrawing || _model.ImageProperties.RulersViewingPlanes.IsMultipleRulerDrawing)
                {
                    _model.ImageProperties.RulersViewingPlanes.CancelRulerDrawing();
                    _model.ImageProperties.RulersViewingPlanes.CancelMultipleRulerDrawing();
                    monitorMouseClickBehaviour = false;
                }
            }
            else if (_model.IsZoomDrawing && IsROIDrawEnabled)
            {
                monitorMouseClickBehaviour = true;
                Point imagePoint = e.GetPosition(backgroundImage);
                _model.AddZoomPoint(imagePoint);
                _zoomPoints.Add(Mouse.GetPosition(imageCanvasBorder));
                _placementPoints.Add(Mouse.GetPosition(placementCanvas));
                if (_model.ZoomPathPoints.Count == 2)
                {
                    RectangularZoom();
                }
                else if (_model.ZoomPathPoints.Count > 2)
                {
                    _model.IsZoomDrawing = false;
                    _isRectangularZoom = false;
                    _zoomPoints.Clear();
                    _placementPoints.Clear();
                    monitorMouseClickBehaviour = false;
                }
                if (_model.ImageProperties.RulersViewingPlanes.IsRulerDrawing || _model.ImageProperties.RulersViewingPlanes.IsMultipleRulerDrawing)
                {
                    _model.ImageProperties.RulersViewingPlanes.CancelRulerDrawing();
                    _model.ImageProperties.RulersViewingPlanes.CancelMultipleRulerDrawing();
                    monitorMouseClickBehaviour = false;
                }
            }
            else if (
                (_model.ImageProperties.RulersViewingPlanes.IsRulerDrawing || _model.ImageProperties.RulersViewingPlanes.IsMultipleRulerDrawing) &&
                (_model.ImageProperties.RulersViewingPlanes.CanLockRulerDrawing() || _model.ImageProperties.RulersViewingPlanes.IsRulerDrawingIndex(ViewingPlaneIndex)))
            {
                monitorMouseClickBehaviour = true;
                if (!_model.ImageProperties.RulersViewingPlanes.IsRulerDrawingInit())
                    _model.ImageProperties.RulersViewingPlanes.InitRulerDrawing(ViewingPlaneIndex);

                Point imagePoint = e.GetPosition(backgroundImage);

                if (_model.ImageProperties.RulersViewingPlanes.DrawingRulerPathPoints.Count < 2)
                    _model.ImageProperties.RulersViewingPlanes.DrawingRulerPathPoints.Add(new Point(imagePoint.X / Zoom, imagePoint.Y / Zoom));
                else if (_model.ImageProperties.RulersViewingPlanes.DrawingRulerPathPoints.Count == 2)
                {
                    _model.ImageProperties.RulersViewingPlanes.FinishRulerDrawing();
                    monitorMouseClickBehaviour = false;
                }
                else if (_model.ImageProperties.RulersViewingPlanes.DrawingRulerPathPoints.Count > 2)
                {
                    _model.ImageProperties.RulersViewingPlanes.CancelRulerDrawing();
                    monitorMouseClickBehaviour = false;
                }
            }
            else if (!dragging)
            {
                dragging = true;
                _p0 = e.GetPosition(placementCanvas);
                _y0 = Canvas.GetTop(imageCanvasBorder);
                _x0 = Canvas.GetLeft(imageCanvasBorder);
            }

            if (IsCurrentROIDrawingCancellable())
                _contextMenuAllowedToOpen = false;

            if (monitorMouseClickBehaviour ^ _mouseClickBehavior.IsElementRegistered())
            {
                if (monitorMouseClickBehaviour)
                    _mouseClickBehavior.RegisterElement();
                else
                {
                    _mouseClickBehavior.UnregisterElement();
                    _contextMenuAllowedToOpen = true;
                }
            }
        }

        private void imageCanvasMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            dragging = false;
        }

        private void imageCanvasMouseMove(object sender, MouseEventArgs e)
        {
            if (_touchEnabled)
                return;
            try
            {
                //if (!this.IsFocused)
                //    this.Focus();
                if (dragging)
                {
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        Point p1 = e.GetPosition(placementCanvas);
                        double dx = p1.X - _p0.X;
                        double w0 = imageCanvasBorder.ActualWidth;
                        double w1 = placementCanvas.ActualWidth;
                        double dy = p1.Y - _p0.Y;
                        double h0 = imageCanvasBorder.ActualHeight;
                        double h1 = placementCanvas.ActualHeight;
                        if (h0 > h1)
                        {
                            if (dy <= 0)
                            {
                                if ((_y0 + dy) > (h1 - h0))
                                    SetImageCanvasTop(_y0 + dy);
                                else
                                    SetImageCanvasTop(h1 - h0);
                            }
                            else
                            {
                                if ((_y0 + dy) < 0)
                                    SetImageCanvasTop(_y0 + dy);
                                else
                                    SetImageCanvasTop(0);
                            }
                            UpdatePositionHProperty();
                        }
                        if (w0 > w1)
                        {

                            if (dx <= 0)
                            {
                                if ((_x0 + dx) > (w1 - w0))
                                    SetImageCanvasLeft(_x0 + dx);
                                else
                                    SetImageCanvasLeft(w1 - w0);
                            }
                            else
                            {
                                if ((_x0 + dx) < 0)
                                    SetImageCanvasLeft(_x0 + dx);
                                else
                                    SetImageCanvasLeft(0);
                            }
                            UpdatePositionWProperty();
                        }
                    }
                    else
                    {
                        dragging = false;
                    }
                }
                Point imagePoint = e.GetPosition(backgroundImage);//imageCanvas);
                _model.SetMouseOverPosition(imagePoint);
                if (_model.ImageProperties.DrawingRegions2D.IsRoiDrawing && IsROIDrawEnabled)
                {
                    _model.ImageProperties.DrawingRegions2D.CurrentDrawing.TrailPoint = imagePoint;
                }
                else if (
                    (_model.ImageProperties.RulersViewingPlanes.IsRulerDrawing || _model.ImageProperties.RulersViewingPlanes.IsMultipleRulerDrawing) &&
                    (_model.ImageProperties.RulersViewingPlanes.IsRulerDrawingIndex(ViewingPlaneIndex)))
                {
                    _model.ImageProperties.RulersViewingPlanes.UpdateCurrentDrawingRulerTool(new Point(imagePoint.X / Zoom, imagePoint.Y / Zoom));
                }
            }
            catch
            {
            }
        }

        private void imageCanvasSizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                bool verticallyCentered = false;
                double h0 = imageCanvasBorder.ActualHeight;
                double h1 = placementCanvas.ActualHeight;
                if (h0 <= h1)
                {
                    CenterImageVertical();
                    verticallyCentered = true;
                }

                bool horizontallyCentered = false;
                double w0 = imageCanvasBorder.ActualWidth;
                double w1 = placementCanvas.ActualWidth;
                if (w0 <= w1)
                {
                    CenterImageHorizontal();
                    horizontallyCentered = true;
                }

                if (_isRectangularZoom || _isTouchZoom || _isMouseWheelZoom)
                {
                    if (h0 > h1 || w0 > w1)
                    {
                        double a0 = _lastZoom;
                        double a1 = Zoom;

                        double x0 = _imageAnchorPointBeforeZoom.X;
                        double y0 = _imageAnchorPointBeforeZoom.Y;
                        double x1 = 0, y1 = 0; // new top-left anchor points for imageCanvasBorder 

                        if (_isRectangularZoom)
                        {
                            double _rectangularLeft = Math.Min(_zoomPoints[0].X, _zoomPoints[1].X) + Math.Abs(_zoomPoints[0].X - _zoomPoints[1].X) / 2;
                            double _rectangularTop = Math.Min(_zoomPoints[0].Y, _zoomPoints[1].Y) + Math.Abs(_zoomPoints[0].Y - _zoomPoints[1].Y) / 2;

                            double _rectPlacementLeft = Math.Min(_placementPoints[0].X, _placementPoints[1].X) + Math.Abs(_placementPoints[0].X - _placementPoints[1].X) / 2;
                            double _rectPlacementTop = Math.Min(_placementPoints[0].Y, _placementPoints[1].Y) + Math.Abs(_placementPoints[0].Y - _placementPoints[1].Y) / 2;

                            x1 = x0 + (1 - a1 / a0) * _rectangularLeft;
                            y1 = y0 + (1 - a1 / a0) * _rectangularTop;

                            double dx = (w1 / 2) - _rectPlacementLeft;
                            double dy = (h1 / 2) - _rectPlacementTop;

                            x1 = x1 + dx;
                            y1 = y1 + dy;
                        }
                        else if (_isMouseWheelZoom)
                        {
                            Point centerOfZoomedView;
                            if (!_wasMouseOverImage)    // mouse is outside the canvas, center on current view
                                centerOfZoomedView = _imageViewPortCenterBeforeZoom;
                            else // center around _mouseCursorBeforeZoom after zooming
                                centerOfZoomedView = _imageMouseOverPointBeforeZoom;

                            x1 = x0 + (1 - a1 / a0) * centerOfZoomedView.X;
                            y1 = y0 + (1 - a1 / a0) * centerOfZoomedView.Y;
                        }
                        else if (_isTouchZoom)
                        {
                            Point centerOfZoomedView;
                            centerOfZoomedView = _touchZoomPointBeforeZoom;

                            x1 = x0 + (1 - a1 / a0) * centerOfZoomedView.X;
                            y1 = y0 + (1 - a1 / a0) * centerOfZoomedView.Y;
                        }

                        SetImageCanvasTop(y1);
                        SetImageCanvasLeft(x1);
                    }

                    if (_isRectangularZoom)
                    {
                        _isRectangularZoom = false;
                        _model.IsZoomDrawing = false;
                        _zoomPoints.Clear();
                        _placementPoints.Clear();
                    }
                    else if (_isMouseWheelZoom)
                    {
                        _isMouseWheelZoom = false;
                    }
                    else if (_isTouchZoom)
                    {
                        _isTouchZoom = false;
                    }

                    UpdatePositionHProperty();
                    UpdatePositionWProperty();
                }
                else
                {
                    if(!verticallyCentered)
                        RefreshPositionH();
                    if(!horizontallyCentered)
                        RefreshPositionW();
                }
            }
            catch { }
            finally
            {
            }
        }

        private void imageGridMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
                {
                    crossHairControl.RefreshExternal(e.GetPosition((UIElement)sender));
                }
            }
            catch { }
        }

        void initInfoAdorner()
        {
            _infoAdorner = new InfoAdorner(imageCanvas, grid, Colors.Silver, 33);
            AdornerLayer.GetAdornerLayer(grid).Add(_infoAdorner);

            if (_infoAdorner != null)
            {
                _infoAdorner.SetBinding(InfoAdorner.LowLimitProperty, new Binding("RoiLowX") { Source = this });
                _infoAdorner.SetBinding(InfoAdorner.HighLimitProperty, new Binding("RoiHighX") { Source = this });
                _infoAdorner.SetBinding(InfoAdorner.LimitOffsetProperty, new Binding("RoiXOffset") { Source = this });

                _infoAdorner.SetBinding(InfoAdorner.LowVolumeZLimitProperty, new Binding("RoiHighY") { Source = this });
                _infoAdorner.SetBinding(InfoAdorner.HighVolumeZLimitProperty, new Binding("RoiLowY") { Source = this });
                _infoAdorner.SetBinding(InfoAdorner.VolumeOffsetProperty, new Binding("RoiYOffset") { Source = this });
            }
        }

        private bool IsCurrentROIDrawingCancellable()
        {
            return _model.ImageProperties.DrawingRegions2D.IsRoiDrawing && !_model.ImageProperties.DrawingRegions2D.CurrentDrawing.IsCurrentlyCancellable();
        }

        private void MenuItemClick(object sender, RoutedEventArgs e)
        {
            FrameworkElement menuItem = sender as FrameworkElement;
            if (menuItem.DataContext == null)
                return;

            IVisualization session = menuItem.DataContext as IVisualization;
            if (session == null)
                return;

            bool continueF = session.PrepareCreateComparisonFromSnapshot();

            if (continueF)
            {
                DateTime currentDateTime = DateTime.Now;
                string suggestedName = "Comparison_" + currentDateTime.ToShortDateString() + "_" + currentDateTime.ToShortTimeString();
                ViewSaveAs saveAsControl = new ViewSaveAs("Comparison name", session.CommandNewComparisonSuperSession, suggestedName);
                DllEntryPoint.ShowModalWindow("Save comparison as", saveAsControl, session, null);
            }
        }

        protected override void RebindInfoAdorners()
        {
            if (_infoAdorner != null)
            {
                _infoAdorner.SetBinding(
                    InfoAdorner.StepSizeProperty,
                    new Binding("ImageProperties.ImageGridLineStepSize") { Source = _model, Mode = BindingMode.OneWay });
            }

            if (_gridAdorner != null)
            {
                Binding mybBinding2 = new Binding("ImageProperties.ImageGridLineType");
                mybBinding2.Source = _model;
                _gridAdorner.SetBinding(GridAdorner.GridStyleProperty, mybBinding2);

                Binding mybBinding2_1 = new Binding("ImageProperties." + GridPercentageWidthStepSize);
                mybBinding2_1.Source = _model;
                _gridAdorner.SetBinding(GridAdorner.StepSizeProperty, mybBinding2_1);

                Binding mybBinding2_2 = new Binding("ImageProperties.ImageGridLineColor");
                mybBinding2_2.Source = _model;
                _gridAdorner.SetBinding(GridAdorner.PenColorProperty, mybBinding2_2);

                double verticalOffsetPercentage = RoiYOffset * 1000 / (RoiLowY - RoiHighY);
                _gridAdorner.SetCurrentValue(GridAdorner.VerticalOffsetProperty, verticalOffsetPercentage);

                double horizontalOffsetPercentage = RoiXOffset * 1000 / (RoiLowX - RoiHighX);
                _gridAdorner.SetCurrentValue(GridAdorner.HorizontalOffsetProperty, horizontalOffsetPercentage);
            }
        }

        private void roisListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
                rulersList.SetCurrentValue(ListBox.SelectedIndexProperty, -1);
        }

        private void rulersListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (!Keyboard.IsKeyDown(Key.LeftCtrl) &&
                    !Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    //model.ImageProperties.RulersViewingPlanes.DeselectRulersButLast();
                    if (e.AddedItems.Count > 0)
                    {
                        _model.ImageProperties.RulersViewingPlanes.DeselectRulersButLast(e.AddedItems);
                        e.Handled = true;
                    }
                }

                //if (e.AddedItems.Count > 0)
                //    roisList.SetCurrentValue(ListBox.SelectedIndexProperty, -1);
            }
            catch { }
        }

        private Point touchDevicesMidPoint(IInputElement relativeTo)
        {
            List<TouchDevice> devices = new List<TouchDevice>(_touchDevicesPoints.Keys);
            Point a = devices[0].GetTouchPoint(relativeTo).Position;
            Point b = devices[1].GetTouchPoint(relativeTo).Position;
            return midPoint(a, b);
        }


        private void UserControlLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _zoomPoints = new List<Point>();
                _placementPoints = new List<Point>();
                _isRectangularZoom = false;
                _isMouseWheelZoom = false;
                _isTouchZoom = false;

                imageAdornerLayer = AdornerLayer.GetAdornerLayer(imageGrid);
                _gridAdorner = new GridAdorner(imageGrid, grid, 33);
                imageAdornerLayer.Add(_gridAdorner);

                //Style thumbStyle = this.Resources["PolygonControlThumbStyle"] as Style;
                //Style anchorThumbStyle = this.Resources["AnchorControlThumbStyle"] as Style;

                _model = base.DataContext as ViewModelImagingContainerBase;
                initInfoAdorner();
                RebindInfoAdorners();
                RefreshPositionH();
                RefreshPositionW();
                _mouseClickBehavior = new MouseClickOutsideControlBehavior(HandleClickOutsideOfControl, this.placementCanvas);
                _contextMenuAllowedToOpen = true;
            }
            catch
            {
            }
        }
        // Bubbled event originating from Thumb objects
        private void ViewRegionControlValidPolygonFirstControlPointClick(object sender, RoutedEventArgs e)
        {
            if (_model.ImageProperties.DrawingRegions2D.IsRoiDrawing && IsROIDrawEnabled)
            {
                _model.ImageProperties.DrawingRegions2D.FinishRoiDrawing();
                _mouseClickBehavior.UnregisterElement();
                _contextMenuAllowedToOpen = true;
            }
        }

        protected override double SetImageCanvasLeftCheckLength(double length)
        {
            double w0 = imageCanvasBorder.ActualWidth;
            double w1 = placementCanvas.ActualWidth;
            if (w0 > w1)
            {
                if (length < -(w0 - w1))
                {
                    length = -(w0 - w1);
                }
                else if (length > 0)
                {
                    length = 0;
                }
            }
            return length;
        }

        protected override double SetImageCanvasTopCheckLength(double length)
        {
            double h0 = imageCanvasBorder.ActualHeight;
            double h1 = placementCanvas.ActualHeight;
            if (h0 > h1)
            {
                if (length < -(h0 - h1))
                {
                    length = -(h0 - h1);
                }
                else if (length > 0)
                {
                    length = 0;
                }
            }
            return length;
        }

        #region roiMethods
        private void imageCanvasMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_model.ImageProperties.DrawingRegions2D.IsRoiDrawing && IsROIDrawEnabled)
            {
                _model.ImageProperties.DrawingRegions2D.FinishRoiDrawing();
            }
            if (_model.IsZoomDrawing && IsROIDrawEnabled)
            {
                _model.IsZoomDrawing = false;
                _isRectangularZoom = false;
                _zoomPoints.Clear();
                _placementPoints.Clear();
            }
            if (_model.ImageProperties.RulersViewingPlanes.IsRulerDrawing || _model.ImageProperties.RulersViewingPlanes.IsMultipleRulerDrawing)
            {
                _model.ImageProperties.RulersViewingPlanes.CancelRulerDrawing();
            }
            _mouseClickBehavior.UnregisterElement();
        }

        #endregion roiMethods 
    }
}
