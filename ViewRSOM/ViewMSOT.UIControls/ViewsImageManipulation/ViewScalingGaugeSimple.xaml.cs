using System;
using System.Collections.Generic;
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

namespace ViewMSOT.UIControls
{

    /// <summary>
    /// Interaction logic for ViewScalingGaugeSimple.xaml
    /// </summary>
    public partial class ViewScalingGaugeSimple : UserControl
    {
        struct FivePointMarkers
        {
            public FivePointMarkers(double[] initialValues)
            {
                L = initialValues[0];
                LM = initialValues[1];
                M = initialValues[2];
                RM = initialValues[3];
                R = initialValues[4];
            }

            public readonly double L;
            public readonly double LM;
            public readonly double M;
            public readonly double RM;
            public readonly double R;
        }

        static FivePointMarkers LogarithmicMarkers = new FivePointMarkers(new double[] { 1.0, 1.0 - 0.94905713, 1.0 - 0.876403407, 1.0 - 0.752795977, 0.0 });
        static FivePointMarkers NonLogarithmicMarkers = new FivePointMarkers(new double[] { 0.0, 0.75, 0.5, 0.25, 1 });

        public ViewScalingGaugeSimple()
		{
			this.InitializeComponent();
            ScaleMax = 100;
            ScaleMin = 0;
            ImageMax = 100;
            ImageMin = 0;
		}

        public bool UseLogarithmic
        {
            get { return (bool)GetValue(UseLogarithmicProperty); }
            set { SetValue(UseLogarithmicProperty, (bool)value); }
        }

        public static readonly DependencyProperty UseLogarithmicProperty =
        DependencyProperty.Register(
         "UseLogarithmic",
         typeof(bool),
         typeof(ViewScalingGaugeSimple),
         new FrameworkPropertyMetadata(
                new PropertyChangedCallback(UseLogarithmicChanged)));

        private static void UseLogarithmicChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                ViewScalingGaugeSimple control = source as ViewScalingGaugeSimple;
                control.updatePosition();
            }
            catch { }
        }


        public double ImageMax
        {
            get { return (double)GetValue(ImageMaxProperty); }
            set { SetValue(ImageMaxProperty, (double)value); }
        }

        public static readonly DependencyProperty ImageMaxProperty =
        DependencyProperty.Register(
         "ImageMax",
         typeof(double),
         typeof(ViewScalingGaugeSimple),
         new FrameworkPropertyMetadata(
                new PropertyChangedCallback(ImageMaxChanged)));

        private static void ImageMaxChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ViewScalingGaugeSimple control = null;
            try
            {
                control = source as ViewScalingGaugeSimple;
                control.updatePosition();                
            }
            catch
            {
                if(control != null)
                    control.ImageMax = 0;
            }
        }

        public double ImageMin
        {
            get { return (double)GetValue(ImageMinProperty); }
            set { SetValue(ImageMinProperty, (double)value); }
        }

        public static readonly DependencyProperty ImageMinProperty =
        DependencyProperty.Register(
         "ImageMin",
         typeof(double),
         typeof(ViewScalingGaugeSimple),
         new FrameworkPropertyMetadata(
                new PropertyChangedCallback(ImageMinChanged)));

        private static void ImageMinChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ViewScalingGaugeSimple control = null;
            try
            {

                control = source as ViewScalingGaugeSimple;
                control.updatePosition();                
            }
            catch
            {
                if(control != null)
                    control.ImageMin = 0;
            }
        }

        public double ScaleMax
        {
            get { return (double)GetValue(ScaleMaxProperty); }
            set { SetValue(ScaleMaxProperty, (double)value); }
        }

        public static readonly DependencyProperty ScaleMaxProperty =
        DependencyProperty.Register(
         "ScaleMax",
         typeof(double),
         typeof(ViewScalingGaugeSimple),
         new FrameworkPropertyMetadata(
                new PropertyChangedCallback(ScaleMaxChanged)));

        private static void ScaleMaxChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ViewScalingGaugeSimple control = null;
            try
            {
                control = source as ViewScalingGaugeSimple;
                control.updatePosition();                
            }
            catch
            {
                if(control != null)
                    control.ImageMax = 0;
            }
        }

        public double ScaleMin
        {
            get { return (double)GetValue(ScaleMinProperty); }
            set { SetValue(ScaleMinProperty, (double)value); }
        }

        public static readonly DependencyProperty ScaleMinProperty =
        DependencyProperty.Register(
         "ScaleMin",
         typeof(double),
         typeof(ViewScalingGaugeSimple),
         new FrameworkPropertyMetadata(
                new PropertyChangedCallback(ScaleMinChanged)));

        private static void ScaleMinChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ViewScalingGaugeSimple control = null;
            try
            {
                control = source as ViewScalingGaugeSimple;
                control.updatePosition();                
            }
            catch
            {
                if(control != null)
                    control.ImageMax = 0;
            }
        }

        private static void calculateMinMax(double[] allPoints, out double min, out double max)
        {
            min = double.MaxValue;
            max = double.MinValue;
            foreach (double val in allPoints)
            {
                if (val < min) min = val;
                if (val > max) max = val;
            }
        }

        double _layerMin;
        double _layerMax;
        double _scaleMin;
        double _scaleMax;
        double _min;
        double _max;
        double _h1, _h2;
        double _totalHeight;
        double _scalingFactor;
        FivePointMarkers _markers;


        void prepareMinMax()
        {
            //double minimumData = ImageMin;
            _layerMin = 0;
            _layerMax = ImageMax;
            _scaleMin = ScaleMin;
            _scaleMax = ScaleMax;

            //if (UseLogarithmic)
            //{
            //    if (minimumData < 0) minimumData = 0; else minimumData = Math.Log10(1 + minimumData);
            //    if (layerMax < 0) layerMax = 0; else layerMax = Math.Log10(1 + layerMax);
            //    if (scaleMin < 0) scaleMin = 0; else scaleMin = Math.Log10(1 + scaleMin);
            //    if (scaleMax < 0) scaleMax = 0; else scaleMax = Math.Log10(1 + scaleMax);
            //}

            if (!IsEnabled)
            {
                if (_layerMin == _layerMax)
                    _layerMax = _layerMax + 1;

                _scaleMin = _layerMin;
                _scaleMax = _layerMax;
            }

            if (DataContext == null)
                return;

            calculateMinMax(new double[] { _layerMin, _layerMax, _scaleMin, _scaleMax }, out _min, out _max);

            _totalHeight = _max - _min;
            _scalingFactor = 1;
            if (_totalHeight < 10) // if our image is less than 10 pixels, magnify to something reasonable
                _scalingFactor = 10.0 / _totalHeight;
            else if (_totalHeight > 1000)
            {
                _scalingFactor = 1000 / _totalHeight;
            }
        }

        void updateTopLine()
        {
            PART_gauge.Height = _totalHeight * _scalingFactor;

            _h1 = (_layerMax - _layerMin) * _scalingFactor;
            Canvas.SetTop(PART_image_values, (_max - _layerMax) * _scalingFactor);
            Canvas.SetTop(PART_image_values0, (_max - _layerMax) * _scalingFactor);

            _h2 = (_scaleMax - _scaleMin) * _scalingFactor;
            Canvas.SetTop(PART_scaling_values, (_max - _scaleMax) * _scalingFactor);

            if (_h1 < _h2 / 100.0) _h1 = _h2 / 100.0;
            if (_h2 < _h1 / 100.0) _h2 = _h1 / 100;
        }

        double prepareScalingLines()
        {
            double diff = 0;
            if (UseLogarithmic)
            {
                double logScaleD = 255.0 / Math.Log10(256.0);
                double scaleRange = _scaleMax - _scaleMin;
                if (_layerMax >= _scaleMax && _layerMin <= _scaleMin)
                {
                    //nothing special needs to be done
                }
                else if (_layerMax < _scaleMin || _layerMin > _scaleMax)
                {
                    //nothing special needs to be done
                }
                else if (_layerMax < _scaleMax && _layerMax > _scaleMin && _layerMin <= _scaleMin)
                {
                    //translate layerMax to log scaling palette
                    double oldD = _layerMax - _scaleMin;
                    double d256 = oldD * 255 / scaleRange;
                    double logd256 = Math.Log10(1.0 + d256) * logScaleD;
                    double newD = logd256 * scaleRange / 255;

                    diff = newD - oldD;
                    _h1 = _h1 + diff * _scalingFactor;
                }
                else if (_layerMax < _scaleMax && _layerMax > _scaleMin && _layerMin > _scaleMin)
                {
                    double oldD = _layerMax - _scaleMin;
                    double d256 = oldD * 255 / scaleRange;
                    double logd256 = Math.Log10(1.0 + d256) * logScaleD;
                    double newD = logd256 * scaleRange / 255;

                    diff = newD - oldD;
                    _h1 = _h1 + diff * _scalingFactor;

                    oldD = _layerMin - _scaleMin;
                    d256 = oldD * 255 / scaleRange;
                    logd256 = Math.Log10(1.0 + d256) * logScaleD;
                    newD = logd256 * scaleRange / 255;

                    double diff2 = newD - oldD;
                    _h1 = _h1 - diff2 * _scalingFactor;
                }
                else if (_layerMin > _scaleMin && _layerMin < _scaleMax && _layerMax >= _scaleMax)
                {
                    double oldD = _layerMin - _scaleMin;
                    double d256 = oldD * 255 / scaleRange;
                    double logd256 = Math.Log10(1.0 + d256) * logScaleD;
                    double newD = logd256 * scaleRange / 255;
                    double diff2 = newD - oldD;
                    _h1 = _h1 - diff2 * _scalingFactor;
                }
                Canvas.SetTop(PART_imageValuesLine, (_max - _layerMax - diff) * _scalingFactor);
                PART_imageValuesLine.Y2 = _h1;
            }
            else
            {
                Canvas.SetTop(PART_imageValuesLine, (_max - _layerMax) * _scalingFactor);
                PART_imageValuesLine.Y2 = _h1;
            }

            return diff;
        }

        void updateTrianglesPosition(double horizontalBorderWidth, double diff)
        {
            PART_image_values.Height = _h1;
            PART_image_values0.Height = _h1;
            PART_image_values.BorderThickness = new Thickness(1, horizontalBorderWidth, 1, 0);
            PART_image_values0.BorderThickness = new Thickness(0, 0, 0, horizontalBorderWidth);

            //Canvas.SetLeft(PART_imageValuesLine, 3);
            //Canvas.SetLeft(PART_imageValuesMaxTriangle, 3);
            //Canvas.SetLeft(PART_imageValuesMinTriangle, 3);

            double trianglesHeight = 3 * horizontalBorderWidth;
            double trianglesThickness = 2 * horizontalBorderWidth;
            if (trianglesThickness > 1.5)
                trianglesThickness = 1.5;
            if (!UseLogarithmic && trianglesThickness > 1.3)
                trianglesThickness = 1.3;
            Canvas.SetTop(PART_imageValuesMaxTriangle, (_max - _layerMax - diff) * _scalingFactor - horizontalBorderWidth * 2);
            PointCollection maxTrianglePoints = new PointCollection();
            maxTrianglePoints.Add(new Point(0, 0));
            maxTrianglePoints.Add(new Point(10, trianglesHeight / 2));
            maxTrianglePoints.Add(new Point(0, trianglesHeight));
            PART_imageValuesMaxTriangle.Points = maxTrianglePoints;
            PART_imageValuesMaxTriangle.StrokeThickness = trianglesThickness;

            Canvas.SetTop(PART_imageValuesMinTriangle, (_max - _layerMax - diff) * _scalingFactor + horizontalBorderWidth * 2);
            PointCollection minTrianglePoints = new PointCollection();
            minTrianglePoints.Add(new Point(0, _h1));
            minTrianglePoints.Add(new Point(10, _h1 - trianglesHeight / 2));
            minTrianglePoints.Add(new Point(0, _h1 - trianglesHeight));
            PART_imageValuesMinTriangle.Points = minTrianglePoints;
            PART_imageValuesMinTriangle.StrokeThickness = trianglesThickness;
        }

        void updateScalingLines(double horizontalBorderWidth)
        {
            DoubleCollection valueLinesDashArray = new DoubleCollection() { 10 / horizontalBorderWidth, 9 / horizontalBorderWidth };
            Canvas.SetTop(PART_imageLValuesLine, (_max - _scaleMax) * _scalingFactor);
            PART_imageLValuesLine.StrokeThickness = horizontalBorderWidth;
            PART_imageLValuesLine.Y1 = PART_imageLValuesLine.Y2 = _h2 * _markers.L;
            PART_imageLValuesLine.StrokeDashArray = valueLinesDashArray;

            Canvas.SetTop(PART_imageLMValuesLine, (_max - _scaleMax) * _scalingFactor);
            PART_imageLMValuesLine.StrokeThickness = horizontalBorderWidth;
            PART_imageLMValuesLine.Y1 = PART_imageLMValuesLine.Y2 = _h2 * _markers.LM;
            PART_imageLMValuesLine.StrokeDashArray = valueLinesDashArray;

            Canvas.SetTop(PART_imageMValuesLine, (_max - _scaleMax) * _scalingFactor);
            PART_imageMValuesLine.StrokeThickness = horizontalBorderWidth;
            PART_imageMValuesLine.Y1 = PART_imageMValuesLine.Y2 = _h2 * _markers.M;
            PART_imageMValuesLine.StrokeDashArray = valueLinesDashArray;

            Canvas.SetTop(PART_imageRMValuesLine, (_max - _scaleMax) * _scalingFactor);
            PART_imageRMValuesLine.StrokeThickness = horizontalBorderWidth;
            PART_imageRMValuesLine.Y1 = PART_imageRMValuesLine.Y2 = _h2 * _markers.RM;
            PART_imageRMValuesLine.StrokeDashArray = valueLinesDashArray;

            Canvas.SetTop(PART_imageRValuesLine, (_max - _scaleMax) * _scalingFactor);
            PART_imageRValuesLine.StrokeThickness = horizontalBorderWidth;
            PART_imageRValuesLine.Y1 = PART_imageRValuesLine.Y2 = _h2 * _markers.R;
            PART_imageRValuesLine.StrokeDashArray = valueLinesDashArray;
        }

        void updatePosition()
        {
            try
            {
                prepareMinMax();
                updateTopLine();
                
                if (!double.IsNaN(_h1) && !double.IsNaN(_h2))
                {
                    double diff = prepareScalingLines();

                    double horizontalBorderWidth = 1;

                    if (viewbox.ActualHeight == 0)
                    {
                        viewbox.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                        viewbox.Arrange(new Rect(0, 0, this.ActualWidth, this.ActualHeight / 2));
                    }

                    //calculate gauge zoom by viewbox control, so as to set border top/bottom width
                    if (viewbox.ActualHeight > 0)
                        horizontalBorderWidth = double.IsNaN(PART_gauge.Height / viewbox.ActualHeight) ? 1 : (PART_gauge.Height / viewbox.ActualHeight);

                    updateTrianglesPosition(horizontalBorderWidth, diff);
                    //border0Rect.StrokeDashArray = new DoubleCollection() { 1, 1 };
                    //border0Rect.StrokeThickness =  10;
                    //PART_image_values.BorderThickness = new Thickness(1, h1/100.0, 1, h1/100.0);

                    PART_scaling_values.Height = _h2;

                    if (UseLogarithmic)
                    {
                        _markers = LogarithmicMarkers;
                    }
                    else
                    {
                        _markers = NonLogarithmicMarkers;
                    }

                    updateScalingLines(horizontalBorderWidth);
                }
                //Scaling Range 
                //this.tooltipInfo.Content
                //if (IsEnabled)
                //    this.viewbox.ToolTip = "Image Data: [" + minimumData.ToString("E3") + "]/[" + layerMax.ToString("E3") + "]" +
                //                           "\nScaling : [" + scaleMin.ToString("E3") + "]/[" + scaleMax.ToString("E3") + "]";
                //else
                //    this.viewbox.ToolTip = "Image Data: [" + minimumData.ToString("E3") + "]/[" + layerMax.ToString("E3") + "]";

                refreshViewBoxToolTip();

                //Xvue.MSOT.ViewModels.Log.ViewModelLog.MsotTrace(
                //    "ImageMax: " + ImageMax +
                //    ", ImageMin: "+ ImageMin +
                //    ", ScaleMax: " + ScaleMax +
                //    ", ScaleMin: "+ ScaleMin +
                //    ", Viewbox : "+ viewbox.ActualHeight +
                //    ", Gauge: " + PART_gauge.ActualHeight
                //, this.GetType().ToString());
            }
            catch (Exception ex)
            {
                DllEntryPoint.LogMessage(Xvue.MSOT.Services.Log.EnumLogType.Warning, "ViewScalingGaugeSimple:updatePosition", ex.Message);
            }
        }

        private void refreshViewBoxToolTip()
        {
            string l1 = "Color bar information:";
            string l2 = "\r\nImage data range: [" + ImageMin.ToString("E3") + "]/[" + ImageMax.ToString("E3") + "]";
            string l3 = IsEnabled ? "\r\nColor bar range : [" + ScaleMin.ToString("E3") + "]/[" + ScaleMax.ToString("E3") + "]" : "";
            string l4 = "\r\n" + (UseLogarithmic ? "Type: Logarithmic color map" : "Type: Linear color map");
            string l5 = "\r\nNote: Left image data range indicator always at „0“. Color bar axis ticks at minimum, ¼, ½, ¾ and\r\nmaximum of the color bar range.";

            viewbox.ToolTip = l1 + l2 + l3 + l4 + l5;
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            updatePosition();
        }

        private void UserControl_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            updatePosition();
        }
	}
}