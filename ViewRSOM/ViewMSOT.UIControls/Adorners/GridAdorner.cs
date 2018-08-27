using System;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Windows.Shapes;
using System.Windows.Controls;
using Xvue.MSOT.Services.Imaging;

namespace ViewMSOT.UIControls.Adorners
{
    public class GridAdorner : Adorner
    {
        #region localvariables
        VisualCollection _visualChildren;
        Canvas _raster;
        FrameworkElement _parentElement;
        int _parentDrawAreaMargin;

        Collection<Geometry> _horizontalGeometries;
        Collection<Geometry> _verticalGeometries;

        double _xOffset;
        double _yOffset;
        double _yLength;

        SolidColorBrush _pathStroke;
                    
        double _penWidth;

        DoubleCollection _dashes;
        #endregion localvariables

#region properties
        public GridLineType GridStyle
        {
            get { return (GridLineType)GetValue(GridStyleProperty); }
            set { SetValue(GridStyleProperty, value); }
        }

        public static readonly DependencyProperty GridStyleProperty =
           DependencyProperty.Register(
              "GridStyle",
              typeof(GridLineType),
              typeof(GridAdorner),
              new FrameworkPropertyMetadata(
                GridLineType.None, new PropertyChangedCallback(ChangeGridStyle)));

        private static void ChangeGridStyle(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                (source as GridAdorner).refreshGridStyle();
            }
            catch
            {
            }
        }

        public double StepSize
        {
            get { return (double)GetValue(StepSizeProperty); }
            set { SetValue(StepSizeProperty, value); }
        }

        public static readonly DependencyProperty StepSizeProperty =
           DependencyProperty.Register(
              "StepSize",
              typeof(double),
              typeof(GridAdorner),
              new FrameworkPropertyMetadata(
                (double)10, new PropertyChangedCallback(ChangeStepSize)));

        private static void ChangeStepSize(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                (source as GridAdorner).drawElements();
            }
            catch
            {
            }
        }

        public Color PenColor
        {
            get { return (Color)GetValue(PenColorProperty); }
            set { SetValue(PenColorProperty, value); }
        }

        public static readonly DependencyProperty PenColorProperty =
           DependencyProperty.Register(
              "PenColor",
              typeof(Color),
              typeof(GridAdorner),
              new FrameworkPropertyMetadata(
                Colors.Blue, new PropertyChangedCallback(ChangePenColor)));

        private static void ChangePenColor(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                (source as GridAdorner).refreshPenColor();
            }
            catch
            {
            }
        }

        public double HorizontalOffset
        {
            get { return (double)GetValue(HorizontalOffsetProperty); }
            set { SetValue(HorizontalOffsetProperty, value); }
        }

        public static readonly DependencyProperty HorizontalOffsetProperty =
           DependencyProperty.Register(
              "HorizontalOffset",
              typeof(double),
              typeof(GridAdorner),
              new FrameworkPropertyMetadata(
                (double)0, new PropertyChangedCallback(ChangedOffset)));

        public double VerticalOffset
        {
            get { return (double)GetValue(VerticalOffsetProperty); }
            set { SetValue(VerticalOffsetProperty, value); }
        }

        public static readonly DependencyProperty VerticalOffsetProperty =
           DependencyProperty.Register(
              "VerticalOffset",
              typeof(double),
              typeof(GridAdorner),
              new FrameworkPropertyMetadata(
                (double)0, new PropertyChangedCallback(ChangedOffset)));

        private static void ChangedOffset(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            (source as GridAdorner).drawElements();
        }
        #endregion properties

        public GridAdorner(UIElement adornedElement, FrameworkElement parentElement, int parentDrawAreaMargin)
            : base(adornedElement)
        {
            _dashes = new DoubleCollection(2);
            _penWidth = 0.8;
            _pathStroke = new SolidColorBrush(PenColor);
            _visualChildren = new VisualCollection(this);
            _raster = new Canvas();
            _visualChildren.Add(_raster);
            IsClipEnabled = true;
            IsHitTestVisible = false;
            AdornedElement.IsVisibleChanged += new DependencyPropertyChangedEventHandler(AdornedElement_IsVisibleChanged);
            _raster.SizeChanged += _rasterCanvas_SizeChanged;
            _parentElement = parentElement;
            _parentElement.SizeChanged += _parentElement_SizeChanged;
            _parentDrawAreaMargin = parentDrawAreaMargin;
            _horizontalGeometries = new Collection<Geometry>();
            _verticalGeometries = new Collection<Geometry>();
        }

        private void _parentElement_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            refreshAfterSizesChanged();
        }

        void AdornedElement_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                _raster.Visibility = Visibility.Visible;
                drawElements();
            }
            else
                _raster.Visibility = Visibility.Hidden;
        }

        void _rasterCanvas_SizeChanged(object sender, EventArgs e)
        {
            refreshAfterSizesChanged();
        }

        private void refreshAfterSizesChanged()
        {
            try
            {
                refreshRelativePositions();
                if (AdornedElement.IsVisible)
                {
                    drawElements();
                }
            }
            catch (Exception ex)
            {
                Xvue.MSOT.ViewModels.Log.ViewModelLog.MsotTrace("_adornedGrid_SizeChanged exception: " + ex.Message, this.GetType().ToString());
            }
        }

        public void RefreshPositionsAndDraw()
        {
            refreshRelativePositions();

            if (GridStyle == GridLineType.Ruler)
                drawElements();
        }

        private void refreshRelativePositions()
        {
            Point relativeLocation = AdornedElement.TranslatePoint(new Point(0, 0), _parentElement);
            _yOffset = relativeLocation.Y < 0 ? -relativeLocation.Y : 0;
            _xOffset = relativeLocation.X < _parentDrawAreaMargin ? _parentDrawAreaMargin - relativeLocation.X : 0;

            _yLength = AdornedElement.RenderSize.Height;
            if (_yLength > _parentElement.ActualHeight - _parentDrawAreaMargin)
                _yLength = _parentElement.ActualHeight - _parentDrawAreaMargin;
        }

        private void refreshGridStyle()
        {
            _penWidth = GridStyle == GridLineType.Ruler ? 1.0 : 0.8;

            _dashes = new DoubleCollection(2);
            switch (GridStyle)
            {
                case GridLineType.Dot:
                    _dashes.Add(3);
                    _dashes.Add(10);
                    break;
                case GridLineType.Solid:
                default:
                    break;
            }

            RefreshPositionsAndDraw();
            drawElements();
            }

        private void refreshPenColor()
        {
            _pathStroke = new SolidColorBrush(PenColor);
            drawElements();
        }

        private void drawElements()
        {
            _raster.Children.Clear();

            _horizontalGeometries = new Collection<Geometry>();
            _verticalGeometries = new Collection<Geometry>();
            
            if (_raster.ActualWidth == 0 || 
                _raster.ActualHeight == 0 ||
                GridStyle == GridLineType.None)
                return;

            double step = StepSize * _raster.ActualWidth;

            doVerticalGrid(_raster.ActualWidth, _raster.ActualHeight, step);
            doHorizontalGrid(_raster.ActualWidth, _raster.ActualHeight, step);

            foreach (Geometry gm in _verticalGeometries)
            {
                _raster.Children.Add(getPathFromGeometry(gm));
            }
            foreach (Geometry gm in _horizontalGeometries)
            {
                _raster.Children.Add(getPathFromGeometry(gm));
            }
        }

        private Path getPathFromGeometry(Geometry gm)
        {
            return new Path()
            {
                Stroke = _pathStroke,
                Data = gm,
                StrokeDashArray = _dashes,
                StrokeThickness = _penWidth
            };
        }

        private void doVerticalGrid(double width, double height, double step)
        {
            List<double> pointsCP = new List<double>();
            List<double> pointsM = new List<double>();

            double marker;

            List<Point> pointsY = new List<Point>();
            marker = height / 2 - VerticalOffset * height;
            while (marker <= height)
            {
                pointsCP.Add(marker);
                marker += step;
            }
            marker = height / 2 - step - VerticalOffset * height;
            while (marker >= 0)
            {
                pointsM.Add(marker);
                marker -= step;
            }
            for (int i = pointsM.Count - 1; i >= 0; i--)
                pointsY.Add(new Point(0, pointsM[i]));
            for (int i = 0; i < pointsCP.Count; i++)
                pointsY.Add(new Point(0, pointsCP[i]));

            if (GridStyle == GridLineType.Ruler)
            {
                double rulerHeight;
                rulerHeight = _xOffset + 10;
                for (int i = 0; i < pointsY.Count; i++)
                {
                    _verticalGeometries.Add(new LineGeometry(pointsY[i], new Point(rulerHeight, pointsY[i].Y)));
                }
            }
            else
            {
                for (int i = 0; i < pointsY.Count; i++)
                {
                    _verticalGeometries.Add(new LineGeometry(pointsY[i], new Point(width, pointsY[i].Y)));
                }
            }
        }

        private void doHorizontalGrid(double width, double height, double step)
        {
            List<double> pointsCP = new List<double>();
            List<double> pointsM = new List<double>();

            double marker;

            List<Point> pointsX = new List<Point>();
            marker = width / 2 - HorizontalOffset * width;
            while (marker <= width)
            {
                pointsCP.Add(marker);
                marker += step;
            }
            marker = width / 2 - HorizontalOffset * width - step;
            while (marker >= 0)
            {
                pointsM.Add(marker);
                marker -= step;
            }
            for (int i = pointsM.Count - 1; i >= 0; i--)
                pointsX.Add(new Point(pointsM[i], 0));
            for (int i = 0; i < pointsCP.Count; i++)
                pointsX.Add(new Point(pointsCP[i], 0));

            if (GridStyle == GridLineType.Ruler)
            {
                double rulerHeight;
                rulerHeight = 10;
                for (int i = 0; i < pointsX.Count; i++)
                {
                    _horizontalGeometries.Add(new LineGeometry(new Point(pointsX[i].X, _yOffset + _yLength), new Point(pointsX[i].X, _yOffset + _yLength - rulerHeight)));
                }
            }
            else
            {
                for (int i = 0; i < pointsX.Count; i++)
                {
                    _horizontalGeometries.Add(new LineGeometry(pointsX[i], new Point(pointsX[i].X, height)));
                }
            }
        }

        protected override int VisualChildrenCount { get { return 1; } }  //visualChildren.Count; } }
        protected override Visual GetVisualChild(int index) { return _raster; } //visualChildren[index]; }

        protected override Size ArrangeOverride( Size finalSize )
        {
            foreach (Visual child in _visualChildren)
            {
                (child as FrameworkElement).Arrange(new Rect(finalSize));
            }
            return finalSize;
       }

    }
}
