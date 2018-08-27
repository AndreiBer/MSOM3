using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Globalization;

namespace ViewMSOT.UIControls.Adorners
{
    public class InfoAdorner : Adorner
    {
        SolidColorBrush _penSolidColorBrush = new SolidColorBrush(Colors.Black);
        Typeface _textTypeface;
        int _parentDrawAreaMargin = 0;
        FrameworkElement _parentElement = null;

        public InfoAdorner(FrameworkElement adornedElement)
            : base(adornedElement)
        {
            adornedElement.IsVisibleChanged += new DependencyPropertyChangedEventHandler(adornedElement_IsVisibleChanged);
            adornedElement.SizeChanged += InfoAdorner_SizeChanged;
            _textTypeface = new Typeface(new FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Normal, new FontStretch());
        }

        public InfoAdorner(FrameworkElement adornedElement, FrameworkElement parentElement, Color color, int parentDrawAreaMargin)
            : this(adornedElement)
        {
            _penSolidColorBrush = new SolidColorBrush(color);
            _parentElement = parentElement;
            _parentElement.SizeChanged += InfoAdorner_SizeChanged;
            _parentDrawAreaMargin = parentDrawAreaMargin;
        }

        private void InfoAdorner_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            InvalidateVisual();
        }

        void adornedElement_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            InvalidateVisual();
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
              typeof(InfoAdorner),
              new FrameworkPropertyMetadata(
                  (double)1, FrameworkPropertyMetadataOptions.AffectsRender));

        public double LowLimit
        {
            get { return (double)GetValue(LowLimitProperty); }
            set { SetValue(LowLimitProperty, value); }
        }

        public static readonly DependencyProperty LowLimitProperty =
           DependencyProperty.Register(
              "LowLimit",
              typeof(double),
              typeof(InfoAdorner),
              new FrameworkPropertyMetadata(
                (double)0, FrameworkPropertyMetadataOptions.AffectsRender));

        public double HighLimit
        {
            get { return (double)GetValue(HighLimitProperty); }
            set { SetValue(HighLimitProperty, value); }
        }

        public static readonly DependencyProperty HighLimitProperty =
           DependencyProperty.Register(
              "HighLimit",
              typeof(double),
              typeof(InfoAdorner),
              new FrameworkPropertyMetadata(
                (double)0, FrameworkPropertyMetadataOptions.AffectsRender));

        public double LowVolumeZLimit
        {
            get { return (double)GetValue(LowVolumeZLimitProperty); }
            set { SetValue(LowVolumeZLimitProperty, value); }
        }

        public static readonly DependencyProperty LowVolumeZLimitProperty =
           DependencyProperty.Register(
              "LowVolumeZLimit",
              typeof(double),
              typeof(InfoAdorner),
              new FrameworkPropertyMetadata(
                (double)0, FrameworkPropertyMetadataOptions.AffectsRender));

        public double HighVolumeZLimit
        {
            get { return (double)GetValue(HighVolumeZLimitProperty); }
            set { SetValue(HighVolumeZLimitProperty, value); }
        }

        public static readonly DependencyProperty HighVolumeZLimitProperty =
           DependencyProperty.Register(
              "HighVolumeZLimit",
              typeof(double),
              typeof(InfoAdorner),
              new FrameworkPropertyMetadata(
                (double)0, FrameworkPropertyMetadataOptions.AffectsRender));

        public double LimitOffset
        {
            get { return (double)GetValue(LimitOffsetProperty); }
            set { SetValue(LimitOffsetProperty, value); }
        }

        public static readonly DependencyProperty LimitOffsetProperty =
           DependencyProperty.Register(
              "LimitOffset",
              typeof(double),
              typeof(InfoAdorner),
              new FrameworkPropertyMetadata(
                (double)0, FrameworkPropertyMetadataOptions.AffectsRender));

        public double VolumeOffset
        {
            get { return (double)GetValue(VolumeOffsetProperty); }
            set { SetValue(VolumeOffsetProperty, value); }
        }

        public static readonly DependencyProperty VolumeOffsetProperty =
           DependencyProperty.Register(
              "VolumeOffset",
              typeof(double),
              typeof(InfoAdorner),
              new FrameworkPropertyMetadata(
                (double)0, FrameworkPropertyMetadataOptions.AffectsRender));

        private FormattedText createInfoField(string text, string followingText = "")
        {
            return new FormattedText(text + followingText,
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    _textTypeface,
                    15,
                    _penSolidColorBrush);
        }

        private FormattedText createInfoField(double number, string followingText = "")
        {
            return createInfoField(number.ToString("F1"), followingText);
        }

        int _textMargin = 3;
        int _horizontalDirection = 1;
        int _verticalDirection = 1;

        double _xOffset = 0;
        double _yOffset = 0;
        double _xLength = 0;
        double _yLength = 0;

        double _xMM = 0;
        double _yMM = 0;

        double _xPixelSizeMM = 0;
        double _yPixelSizeMM = 0;

        double _topMeasure = double.NaN;
        double _bottomMeasure = double.NaN;
        double _leftMeasure = double.NaN;
        double _rightMeasure = double.NaN;

        //bool _visibleZeroY = false;
        //bool _visibleZeroX = false;
        
        private void refreshLengthsMeasures()
        {
            _xPixelSizeMM = Math.Abs(HighLimit - LowLimit) / AdornedElement.RenderSize.Width;
            _yPixelSizeMM = Math.Abs(HighVolumeZLimit - LowVolumeZLimit) / AdornedElement.RenderSize.Height;

            _verticalDirection = HighVolumeZLimit < LowVolumeZLimit ? 1 : -1;
            _horizontalDirection = LowLimit < HighLimit ? 1 : -1;

            Point relativeLocation = AdornedElement.TranslatePoint(new Point(0, 0), _parentElement);
            _yOffset = relativeLocation.Y < 0 ? -relativeLocation.Y : 0;
            _xOffset = relativeLocation.X < _parentDrawAreaMargin ? _parentDrawAreaMargin - relativeLocation.X : 0;

           _yLength = AdornedElement.RenderSize.Height;
            if (_yLength > _parentElement.ActualHeight - _parentDrawAreaMargin)
                _yLength = _parentElement.ActualHeight - _parentDrawAreaMargin;

            _xLength = AdornedElement.RenderSize.Width;
            if (_xLength > _parentElement.ActualWidth - _parentDrawAreaMargin)
                _xLength = _parentElement.ActualWidth - _parentDrawAreaMargin;

            _yMM = _yPixelSizeMM * _yLength;
            _xMM = _xPixelSizeMM * _xLength;

            _topMeasure = HighVolumeZLimit - VolumeOffset * 1000 + _verticalDirection * _yPixelSizeMM * _yOffset;
            _bottomMeasure = _topMeasure + _verticalDirection * _yMM;
            //_visibleZeroY = _verticalDirection * _topMeasure < 0 && _verticalDirection * _bottomMeasure > 0 ? true : false;

            _leftMeasure = LowLimit - LimitOffset * 1000 + _horizontalDirection * _xPixelSizeMM * _xOffset;
            _rightMeasure = _leftMeasure + _horizontalDirection * _xMM;
            //_visibleZeroX = _horizontalDirection * _leftMeasure < 0 && _horizontalDirection * _rightMeasure > 0 ? true : false;
        }


        protected override void OnRender(DrawingContext drawingContext)
        {
            if (this.AdornedElement.IsVisible )
            {
                refreshLengthsMeasures();

                ////Vertical, top to bottom
                //if (_visibleZeroY)
                //{
                //    double zeroYOffset = Math.Abs(_topMeasure) / _yPixelSizeMM;
                //    drawVerticalText(ref drawingContext, 0, zeroYOffset);

                //    double space = zeroYOffset / _yLength;
                //    if (space < 0.22)
                //    {
                //        drawVerticalText(ref drawingContext, verticalPointToMeasure(zeroYOffset + (_yLength - zeroYOffset) / 2), zeroYOffset + (_yLength - zeroYOffset) / 2);
                //        drawVerticalText(ref drawingContext, _topMeasure + _verticalDirection * _yMM, _yLength);
                //    }
                //    else if (space > 0.78)
                //    {
                //        drawVerticalText(ref drawingContext, _topMeasure, 0);
                //        drawVerticalText(ref drawingContext, verticalPointToMeasure(zeroYOffset / 2), zeroYOffset / 2);
                //    }
                //    else
                //    {
                //        drawVerticalText(ref drawingContext, _topMeasure, 0);
                //        drawVerticalText(ref drawingContext, _topMeasure + _verticalDirection * _yMM, _yLength);
                //    }
                //}
                //else
                //{
                //    drawVerticalText(ref drawingContext, _topMeasure, 0);
                //    drawVerticalText(ref drawingContext, _topMeasure + _verticalDirection * _yMM / 2, _yLength / 2);
                //    drawVerticalText(ref drawingContext, _topMeasure + _verticalDirection * _yMM, _yLength);
                //}



                ////Horizontal left to right
                //if (_visibleZeroX)
                //{
                //    double zeroXOffset = Math.Abs(_leftMeasure) / _xPixelSizeMM;

                //    double space = zeroXOffset / _xLength;
                //    if (space < 0.22)
                //    {
                //        drawHorizontalText(ref drawingContext, 0, zeroXOffset, true);
                //        drawHorizontalText(ref drawingContext, horizontalPointToMeasure(zeroXOffset + (_xLength - zeroXOffset) / 2), zeroXOffset + (_yLength - zeroXOffset) / 2);
                //        drawHorizontalText(ref drawingContext, _leftMeasure + _horizontalDirection * _xMM, _xLength);
                //    }
                //    else if (space > 0.78)
                //    {
                //        drawHorizontalText(ref drawingContext, _leftMeasure, 0, true);
                //        drawHorizontalText(ref drawingContext, horizontalPointToMeasure(zeroXOffset / 2), zeroXOffset / 2);
                //        drawHorizontalText(ref drawingContext, 0, zeroXOffset);
                //    }
                //    else
                //    {
                //        drawHorizontalText(ref drawingContext, _leftMeasure, 0, true);
                //        drawHorizontalText(ref drawingContext, _leftMeasure + _horizontalDirection * _xMM, _xLength);
                //        drawHorizontalText(ref drawingContext, 0, zeroXOffset);
                //    }
                //}
                //else
                //{
                //    drawHorizontalText(ref drawingContext, _leftMeasure, 0, true);
                //    drawHorizontalText(ref drawingContext, _leftMeasure + _horizontalDirection * _xMM / 2, _xLength / 2);
                //    drawHorizontalText(ref drawingContext, _leftMeasure + _horizontalDirection * _xMM, _xLength);
                //}

                {

                    if (double.IsNaN(_bottomMeasure) || double.IsNaN(_topMeasure) ||
                        double.IsNaN(_leftMeasure) || double.IsNaN(_rightMeasure))
                        return;

                    double min, max, m;
                    double correctedStep;

                    FormattedText testSizeText;
                    double testTextLength;
                    double diff;

                    ////Vertical, top to bottom
                    correctedStep = StepSize;
                    testSizeText = createInfoField(-StepSize);
                    testTextLength = testSizeText.Height;

                    diff = Math.Abs(verticalMeasureToPoint(0) - verticalMeasureToPoint(-correctedStep));
                    while (diff < 2 * testTextLength)
                    {
                        correctedStep += StepSize;
                        diff = Math.Abs(verticalMeasureToPoint(0) - verticalMeasureToPoint(-correctedStep));
                    }

                    if (_topMeasure > _bottomMeasure)
                    {
                        max = _topMeasure;
                        min = _bottomMeasure;
                    }
                    else
                    {
                        max = _bottomMeasure;
                        min = _topMeasure;
                    }

                    for (m = 0; m <= max; m += correctedStep)
                        drawVerticalText(ref drawingContext, m, verticalMeasureToPoint(m));
                    for (m = 0 - correctedStep; m >= min; m -= correctedStep)
                        drawVerticalText(ref drawingContext, m, verticalMeasureToPoint(m));
                    ////End Vertical


                    ////Horizontal left to right
                    correctedStep = StepSize;
                    testSizeText = createInfoField(-StepSize);
                    testTextLength = testSizeText.Width;

                    diff = Math.Abs(horizontalMeasureToPoint(0) - horizontalMeasureToPoint(-correctedStep));
                    while (diff < 2 * testTextLength)
                    {
                        correctedStep += StepSize;
                        diff = Math.Abs(horizontalMeasureToPoint(0) - horizontalMeasureToPoint(-correctedStep));
                    }

                    if (_leftMeasure > _rightMeasure)
                    {
                        max = _leftMeasure;
                        min = _rightMeasure;
                    }
                    else
                    {
                        max = _rightMeasure;
                        min = _leftMeasure;
                    }

                    for (m = 0; m <= max; m += correctedStep)
                        drawHorizontalText(ref drawingContext, m, horizontalMeasureToPoint(m));
                    for (m = 0 - correctedStep; m >= min; m -= correctedStep)
                        drawHorizontalText(ref drawingContext, m, horizontalMeasureToPoint(m));
                    ////End Horizontal

                }

                FormattedText t = createInfoField("(mm)");
                drawingContext.DrawText(t, new Point(_xOffset - t.Width - _textMargin, _yOffset + _yLength + _textMargin));
            }
        }

        private bool drawHorizontalText(ref DrawingContext drawingContext, double measure, double x, bool showUnit = false)
        {
            if (x < 0 || x > _xLength)
                return false;

            FormattedText t = createInfoField(measure, showUnit ? "mm" : "");
            double correctedX = x - t.Width / 2;
            if (correctedX < 0)
                correctedX = 0;
            else if (correctedX + t.Width > _xLength)
                correctedX = _xLength - t.Width;

            drawingContext.DrawText(t, new Point(_xOffset + correctedX, _yOffset + _yLength + _textMargin));
            return true;
        }

        private bool drawVerticalText(ref DrawingContext drawingContext, double measure, double y)
        {
            if (y < 0 || y > _yLength)
                return false;

            FormattedText t = createInfoField(measure);
            double correctedY = y - t.Height / 2;
            if (correctedY < 0)
                correctedY = 0;
            else if (correctedY + t.Height > _yLength)
                correctedY = _yLength - t.Height;
            drawingContext.DrawText(t, new Point(_xOffset - _textMargin - t.Width, _yOffset + correctedY));
            return true;
        }

        double verticalPointToMeasure(double y)
        {
            return _topMeasure + _verticalDirection * y * _yPixelSizeMM;
        }

        double verticalMeasureToPoint(double m)
        {
            return (m - _topMeasure) / (_verticalDirection * _yPixelSizeMM);
        }

        double horizontalPointToMeasure(double x)
        {
            return _leftMeasure + _horizontalDirection * x * _xPixelSizeMM;
        }

        double horizontalMeasureToPoint(double m)
        {
            return (m - _leftMeasure) / (_horizontalDirection * _xPixelSizeMM);
        }

    }
}

