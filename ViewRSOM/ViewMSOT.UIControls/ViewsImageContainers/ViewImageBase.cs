using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Shapes;
using Xvue.MSOT.ViewModels.Imaging;
using System.Windows.Data;
using System;

namespace ViewMSOT.UIControls
{
    public abstract class ViewImageBase : UserControl
    {
        #region localvariables
        protected bool _isRectangularZoom;
        protected ViewModelImagingContainerBase _model;
        #endregion localvariables

        #region properties
        public ImageSource ActiveImageSource
        {
            get { return (ImageSource)GetValue(ActiveImageSourceProperty); }
            set { SetValue(ActiveImageSourceProperty, value); }
        }

        public static readonly DependencyProperty ActiveImageSourceProperty =
                                                                           DependencyProperty.Register(
              "ActiveImageSource",
              typeof(ImageSource),
              typeof(ViewImageBase),
              new FrameworkPropertyMetadata());

        public MatrixTransform ActiveImageTransform
        {
            get { return (MatrixTransform)GetValue(ActiveImageTransformProperty); }
            set { SetValue(ActiveImageTransformProperty, value); }
        }

        public static readonly DependencyProperty ActiveImageTransformProperty =
           DependencyProperty.Register(
              "ActiveImageTransform",
              typeof(MatrixTransform),
              typeof(ViewImageBase),
              new FrameworkPropertyMetadata());

        public double HCenter
        {
            get { return (double)GetValue(HCenterProperty); }
            set { SetValue(HCenterProperty, value); }
        }

        public static readonly DependencyProperty HCenterProperty =
           DependencyProperty.Register(
              "HCenter",
              typeof(double),
              typeof(ViewImageBase),
              new FrameworkPropertyMetadata(
                new PropertyChangedCallback(ChangeHCenter)));

        public bool IsMultipleRulerDrawing
        {
            get { return (bool)GetValue(IsMultipleRulerDrawingProperty); }
            set { SetValue(IsMultipleRulerDrawingProperty, value); }
        }

        public static readonly DependencyProperty IsMultipleRulerDrawingProperty =
           DependencyProperty.Register(
              "IsMultipleRulerDrawing",
              typeof(bool),
              typeof(ViewImageBase),
              new FrameworkPropertyMetadata(
                new PropertyChangedCallback(ChangeIsRulerDrawing)));

        public bool IsRulerDrawing
        {
            get { return (bool)GetValue(IsRulerDrawingProperty); }
            set { SetValue(IsRulerDrawingProperty, value); }
        }

        public static readonly DependencyProperty IsRulerDrawingProperty =
           DependencyProperty.Register(
              "IsRulerDrawing",
              typeof(bool),
              typeof(ViewImageBase),
              new FrameworkPropertyMetadata(
                new PropertyChangedCallback(ChangeIsRulerDrawing)));

        public IEnumerable<ViewModelRulerTool2D> RulerMeasurementTools
        {
            get { return (IEnumerable<ViewModelRulerTool2D>)GetValue(RulerMeasurementToolsProperty); }
            set { SetValue(RulerMeasurementToolsProperty, value); }
        }

        public static readonly DependencyProperty RulerMeasurementToolsProperty =
           DependencyProperty.Register(
              "RulerMeasurementTools",
              typeof(IEnumerable<ViewModelRulerTool2D>),
              typeof(ViewImageBase));

        public double WCenter
        {
            get { return (double)GetValue(WCenterProperty); }
            set { SetValue(WCenterProperty, value); }
        }

        public static readonly DependencyProperty WCenterProperty =
           DependencyProperty.Register(
              "WCenter",
              typeof(double),
              typeof(ViewImageBase),
              new FrameworkPropertyMetadata(
                new PropertyChangedCallback(ChangeWCenter)));

        public double Zoom
        {
            get { return (double)GetValue(ZoomProperty); }
            set { SetValue(ZoomProperty, value); }
        }

        public static readonly DependencyProperty ZoomProperty =
           DependencyProperty.Register(
              "Zoom",
              typeof(double),
              typeof(ViewImageBase),
              new FrameworkPropertyMetadata(
                new PropertyChangedCallback(ChangeZoom)));

        public Visibility CrosshairVisibility
        {
            get { return (Visibility)GetValue(CrosshairVisibilityProperty); }
            set { SetValue(CrosshairVisibilityProperty, value); }
        }

        public static readonly DependencyProperty CrosshairVisibilityProperty =
           DependencyProperty.Register(
              "CrosshairVisibility",
              typeof(Visibility),
              typeof(ViewImageBase),
              new FrameworkPropertyMetadata(Visibility.Collapsed));

        #endregion properties

        protected abstract void RebindInfoAdorners();

        protected abstract Border ImageCanvasBorder { get; }

        protected abstract Canvas PlacementCanvas { get; }

        protected abstract double ZoomHeight { get; }

        protected abstract Border ZpImageBorder { get; }

        protected abstract Viewbox ZpViewBox { get; }

        protected abstract Rectangle ZpViewportRect { get; }

        protected abstract double ZoomWidth { get; }

        protected void UserControlDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _model = base.DataContext as ViewModelImagingContainerBase;
            RebindInfoAdorners();
        }

        protected static void ChangeHCenter(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                if (e.NewValue != null && e.NewValue != e.OldValue)
                {
                    //System.Diagnostics.Debug.WriteLine("HCenter changed.");
                    (source as ViewImageBase).RefreshPositionH();
                }
            }
            catch
            {
            }
        }

        protected static void ChangeIsRulerDrawing(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                (source as ViewImageBase).ChangeIsRulerDrawing((bool)e.NewValue);
            }
            catch { }
        }

        protected static void ChangeWCenter(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                if (e.NewValue != null && e.NewValue != e.OldValue)
                {
                    //System.Diagnostics.Debug.WriteLine("WCenter changed.");
                    (source as ViewImageBase).RefreshPositionW();
                }
            }
            catch
            {
            }
        }

        protected static void ChangeZoom(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                if (e.NewValue != null && e.NewValue != e.OldValue)
                {
                    //System.Diagnostics.Debug.WriteLine("Zoom changed.");
                    (source as ViewImageBase).UpdateZoom((double)e.NewValue, (double)e.OldValue);
                }
            }
            catch
            {
            }
        }

        protected void BorderGridSizeChanged(object sender, SizeChangedEventArgs e)
        {
            RefreshPositionH();
            RefreshPositionW();
            RefreshMinimumZoom();
        }

        public abstract void ChangeIsRulerDrawing(bool newValue);

        protected abstract void RefreshInfoAdorner();

        protected abstract void RefreshMinimumZoom();

        protected abstract void UpdateZoom(double newValue, double oldValue);

        protected void CenterImage()
        {
            CenterImageHorizontal();
            CenterImageVertical();
        }

        protected void CenterImageHorizontal()
        {
            SetImageCanvasLeft((PlacementCanvas.ActualWidth - ImageCanvasBorder.ActualWidth) / 2.0);
        }

        protected void CenterImageVertical()
        {
            SetImageCanvasTop((PlacementCanvas.ActualHeight - ImageCanvasBorder.ActualHeight) / 2.0);
        }

        protected void DeltaZoom(double delta)
        {
            double percent = (double)delta / 500.0;
            Zoom += percent * Zoom;
        }

        protected void RectangularZoom()
        {
            Canvas canvas = PlacementCanvas;
            double rWidth = canvas.ActualWidth / ZoomWidth;
            double rHeight = canvas.ActualHeight / ZoomHeight;
            double rZoom = 1.0;

            if (rWidth < rHeight)
                rZoom = rWidth;
            else
                rZoom = rHeight;

            _isRectangularZoom = true;
            Zoom = rZoom;
        }

        protected void UpdatePositionHProperty()
        {
            try
            {
                Border border = ImageCanvasBorder;
                double OL = border.ActualHeight;
                double VL = PlacementCanvas.ActualHeight;

                if (OL <= VL)
                {
                    SetCurrentValue(HCenterProperty, 0.0);
                    return;
                }

                double LL = Canvas.GetTop(border);
                SetCurrentValue(HCenterProperty, (VL / 2 - LL - OL / 2) / OL);
            }
            catch { }
        }

        protected void RefreshPositionH()
        {
            try
            {
                double OL = ImageCanvasBorder.ActualHeight;
                double VL = PlacementCanvas.ActualHeight;
                SetImageCanvasTop(VL / 2 - OL / 2 - HCenter * OL);
            }
            catch { }
        }

        protected void UpdatePositionWProperty()
        {
            try
            {
                Border border = ImageCanvasBorder;
                double OL = border.ActualWidth;
                double VL = PlacementCanvas.ActualWidth;

                if (OL <= VL)
                {
                    SetCurrentValue(WCenterProperty, 0.0);
                    return;
                }

                double LL = Canvas.GetLeft(border);
                SetCurrentValue(WCenterProperty, (VL / 2 - LL - OL / 2) / OL);
            }
            catch { }
        }

        protected void RefreshPositionW()
        {
            try
            {
                double OL = ImageCanvasBorder.ActualWidth;
                double VL = PlacementCanvas.ActualWidth;
                SetImageCanvasLeft(VL / 2 - OL / 2 - WCenter * OL);
            }
            catch { }
        }

        protected void SetImageCanvasLeft(double length)
        {
            length = SetImageCanvasLeftCheckLength(length);
            
            Canvas canvas = PlacementCanvas;
            Border border = ImageCanvasBorder;
            Rectangle viewportRect = ZpViewportRect;
            Viewbox viewBox = ZpViewBox;

            if(length == Canvas.GetLeft(ImageCanvasBorder)) return;
            //System.Diagnostics.Debug.WriteLine($"setImageCanvasLeft: ImageCanvasBorder left { Canvas.GetLeft(ImageCanvasBorder)} -> {length})");

            Canvas.SetLeft(border, length);
            Canvas.SetLeft(viewportRect, -length);
            viewportRect.StrokeThickness = Zoom * 3;
            ZpImageBorder.BorderThickness = new Thickness(Zoom * 3);
            if (length < 0 || (length + border.ActualWidth > canvas.ActualWidth))
            {
                viewBox.Visibility = Visibility.Visible;
            }
            else
            {
                double topLength = Canvas.GetTop(border);
                if (topLength < 0 || (topLength + border.ActualHeight > canvas.ActualHeight))
                    viewBox.Visibility = Visibility.Visible;
                else
                    viewBox.Visibility = Visibility.Collapsed;
            }
            RefreshInfoAdorner();
        }

        protected abstract double SetImageCanvasLeftCheckLength(double length);

        protected void SetImageCanvasTop(double length)
        {
            
            length = SetImageCanvasTopCheckLength(length);
            Canvas canvas = PlacementCanvas;
            Border border = ImageCanvasBorder;
            Rectangle viewPortRect = ZpViewportRect;
            Viewbox viewBox = ZpViewBox;

            if (length == Canvas.GetTop(ImageCanvasBorder)) return;
            //System.Diagnostics.Debug.WriteLine($"setImageCanvasTop: ImageCanvasBorder Top { Canvas.GetTop(ImageCanvasBorder)} -> {length})");

            Canvas.SetTop(border, length);
            Canvas.SetTop(viewPortRect, -length);
            viewPortRect.StrokeThickness = Zoom * 3;
            ZpImageBorder.BorderThickness = new Thickness(Zoom * 3);
            if (length < 0 || (length + border.ActualHeight > canvas.ActualHeight))
            {
                viewBox.Visibility = Visibility.Visible;
            }
            else
            {
                double leftLength = Canvas.GetLeft(border);
                if (leftLength < 0 || (leftLength + border.ActualWidth > canvas.ActualWidth))
                    viewBox.Visibility = Visibility.Visible;
                else
                    viewBox.Visibility = Visibility.Collapsed;
            }
            RefreshInfoAdorner();
        }

        protected abstract double SetImageCanvasTopCheckLength(double length);

        protected void UserControlKeyDown(object sender, KeyEventArgs e)
        {
            double delta = double.NaN;
            if (e.Key == Key.F2)
                delta = 125;
            if (e.Key == Key.F3)
                delta = -125;

            if (!double.IsNaN(delta))
                DeltaZoom(delta);
        }
    }
}
