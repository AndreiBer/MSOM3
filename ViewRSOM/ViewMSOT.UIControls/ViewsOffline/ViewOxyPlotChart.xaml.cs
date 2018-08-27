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
using System.Collections.ObjectModel;
//using Xvue.MSOT.ViewModels.Imaging;
using Xvue.MSOT.Common;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Linq;
using System.Collections.Specialized;

namespace ViewMSOT.UIControls
{
    /// <summary>
    /// OxyPlot using the MultiFunctionalChartData object for general use
    /// </summary>
    public partial class ViewOxyPlotChart : UserControl
    {

        public ViewOxyPlotChart()
        {
            InitializeComponent();
            _oxyPlotViewAnnotations = new ObservableCollection<OxyPlot.Wpf.Annotation>();
            _oxyPlotViewAxes = new ObservableCollection<OxyPlot.Wpf.Axis>();
            OxyPlotViewAnnotations.CollectionChanged += OxyPlotViewAnnotations_CollectionChanged;
            _oxyPlotViewAxes.CollectionChanged += _oxyPlotViewAxes_CollectionChanged;
            //System.Diagnostics.Debug.WriteLine($"{Name} created (OxyPlotViewAxes Count = {OxyPlotViewAxes.Count}).");
        }

        private void _oxyPlotViewAxes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine($"{Name} Axes modified (Count = {_oxyPlotViewAxes.Count}).");
            ConstructPlot();
        }

        ObservableCollection<OxyPlot.Wpf.Axis> _oxyPlotViewAxes;

        public ObservableCollection<OxyPlot.Wpf.Axis> OxyPlotViewAxes
        {
            get { return _oxyPlotViewAxes; }
        }

        private static void OxyPlotViewAxesChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ViewOxyPlotChart control = null;
            try
            {
                if (e.NewValue != null && e.NewValue != e.OldValue)
                {
                    control = source as ViewOxyPlotChart;
                    control.OxyPlotView.Axes.Clear();
                    control.ConstructPlot();
                }
            }
            catch (Exception ex)
            {
                DllEntryPoint.LogMessage(Xvue.MSOT.Services.Log.EnumLogType.Warning, "Exception", $"{control.Name}: Exception updating axes: '{ex.Message}'.");
            }
        }

        ObservableCollection<OxyPlot.Wpf.Annotation> _oxyPlotViewAnnotations;

        public ObservableCollection<OxyPlot.Wpf.Annotation> OxyPlotViewAnnotations
        {
            get { return _oxyPlotViewAnnotations; }
        }

        void OxyPlotViewAnnotations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            try
            {
                OxyPlotView.Annotations.Clear();
                foreach (OxyPlot.Wpf.Annotation annotation in OxyPlotViewAnnotations)
                    OxyPlotView.Annotations.Add(annotation);
                //System.Diagnostics.Debug.WriteLine($"{Name}: Annotations updated.");
            }
            catch (Exception ex)
            {
                DllEntryPoint.LogMessage(Xvue.MSOT.Services.Log.EnumLogType.Warning, "Exception", $"{Name}: Exception updating annotations: '{ex.Message}'.");
            }
        }


        public static readonly DependencyProperty LineSeriesProperty =
        DependencyProperty.Register(
         "LineSeries",
         typeof(IEnumerable<DataPoint>),
         typeof(ViewOxyPlotChart),
         new FrameworkPropertyMetadata(new PropertyChangedCallback(LineSeriesChanged))
         );

        public IEnumerable<DataPoint> LineSeries
        {
            get { return (IEnumerable<DataPoint>)GetValue(LineSeriesProperty); }
            set { SetValue(LineSeriesProperty, value); }
        }

        private static void LineSeriesChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ViewOxyPlotChart control = null;
            try
            {
                control = source as ViewOxyPlotChart;
                control.ConstructPlot();
            }
            catch (Exception ex)
            {
                DllEntryPoint.LogMessage(Xvue.MSOT.Services.Log.EnumLogType.Warning, "Exception", $"{control.Name}: Exception updating line series: '{ex.Message}'.");
            }
        }

        public static readonly DependencyProperty XAxisStyleProperty =
        DependencyProperty.Register(
         "XAxisStyle",
         typeof(Style),
         typeof(ViewOxyPlotChart));

        public Style XAxisStyle
        {
            get { return (Style)GetValue(XAxisStyleProperty); }
            set { SetValue(XAxisStyleProperty, value); }
        }

        public static readonly DependencyProperty YAxisStyleProperty =
        DependencyProperty.Register(
         "YAxisStyle",
         typeof(Style),
         typeof(ViewOxyPlotChart));

        public Style YAxisStyle
        {
            get { return (Style)GetValue(YAxisStyleProperty); }
            set { SetValue(YAxisStyleProperty, value); }
        }

        public Style PlotViewStyle
        {
            get { return (Style)GetValue(PlotViewStyleProperty); }
            set { SetValue(PlotViewStyleProperty, value); }
        }

        public static readonly DependencyProperty PlotViewStyleProperty =
        DependencyProperty.Register(
         "PlotViewStyle",
         typeof(Style),
         typeof(ViewOxyPlotChart),
         new FrameworkPropertyMetadata(new PropertyChangedCallback(PlotViewStyleChanged))
         );

        private static void PlotViewStyleChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ViewOxyPlotChart control = null;
            try
            {
                if (e.NewValue != e.OldValue)
                {
                    control = source as ViewOxyPlotChart;
                    control.OxyPlotView.Style = e.NewValue as Style;
                    //System.Diagnostics.Debug.WriteLine($"{control.Name}: PlotView Style updated.");
                }
            }
            catch (Exception ex)
            {
                DllEntryPoint.LogMessage(Xvue.MSOT.Services.Log.EnumLogType.Warning, "Exception", $"{control.Name}: Exception PlotView style: '{ex.Message}'.");
            }
        }

        public Style LineSeriesStyle
        {
            get { return (Style)GetValue(LineSeriesStyleProperty); }
            set { SetValue(LineSeriesStyleProperty, value); }
        }

        public static readonly DependencyProperty LineSeriesStyleProperty =
        DependencyProperty.Register(
         "LineSeriesStyle",
         typeof(Style),
         typeof(ViewOxyPlotChart));

        public static readonly DependencyProperty ChartDataProperty =
        DependencyProperty.Register(
         "ChartData",
         typeof(MultifunctionalChartData),
         typeof(ViewOxyPlotChart),
         new FrameworkPropertyMetadata(new PropertyChangedCallback(ChartDataChanged)));

        public MultifunctionalChartData ChartData
        {
            get { return (MultifunctionalChartData)GetValue(ChartDataProperty); }
            set { SetValue(ChartDataProperty, value); }
        }

        private static void ChartDataChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ViewOxyPlotChart control = null;
            try
            {
                if (e.NewValue != e.OldValue)
                {
                    control = source as ViewOxyPlotChart;
                    control.OxyPlotView.Axes.Clear();
                    control.ConstructPlot();
                }
            }
            catch (Exception ex)
            {
                DllEntryPoint.LogMessage(Xvue.MSOT.Services.Log.EnumLogType.Warning, "Exception", $"{control.Name}: Exception ChartData: '{ex.Message}'.");
            }
        }

        public static readonly DependencyProperty ShowGridLinesProperty =
        DependencyProperty.Register(
         "ShowGridLines",
         typeof(bool),
         typeof(ViewOxyPlotChart));
        public bool ShowGridLines
        {
            get { return (bool)GetValue(ShowGridLinesProperty); }
            set { SetValue(ShowGridLinesProperty, value); }
        }

        public static readonly DependencyProperty ShowHelpButtonProperty =
        DependencyProperty.Register(
         "ShowHelpButton",
         typeof(bool),
         typeof(ViewOxyPlotChart));

        public bool ShowHelpButton
        {
            get { return (bool)GetValue(ShowHelpButtonProperty); }
            set { SetValue(ShowHelpButtonProperty, value); }
        }

        public static readonly DependencyProperty GridWidthProperty =
        DependencyProperty.Register(
         "GridWidth",
         typeof(double),
         typeof(ViewOxyPlotChart));

        public double GridWidth
        {
            get { return (double)GetValue(GridWidthProperty); }
            set { SetValue(GridWidthProperty, value); }
        }

        public static readonly DependencyProperty GridHeightProperty =
        DependencyProperty.Register(
         "GridHeight",
         typeof(double),
         typeof(ViewOxyPlotChart));

        public double GridHeight
        {
            get { return (double)GetValue(GridHeightProperty); }
            set { SetValue(GridHeightProperty, value); }
        }

        public double AbsoluteMinimumX { get; private set; }
        public double AbsoluteMaximumX { get; private set; }
        public double AbsoluteMinimumY { get; private set; }
        public double AbsoluteMaximumY { get; private set; }

        private bool constructAxes()
        {
            // Setup Axes defined in XAML
            if (LineSeries != null && LineSeries.Count() > 0 && OxyPlotViewAxes.Count > 1)
            {
                IEnumerator<OxyPlot.Wpf.Axis> it = OxyPlotViewAxes.GetEnumerator();
                while (it.MoveNext())
                {
                    OxyPlotView.Axes.Add(it.Current);
                }
            }
            // Setup Axes defined in the ViewModel
            else if (ChartData != null && ChartData.LinesData.Count > 0 && ChartData.AxesConfigurations.Count > 1)
            {
                int i = 0;

                foreach (ChartAxisConf axisConf in ChartData.AxesConfigurations)
                {
                    OxyPlot.Wpf.Axis _axis = (OxyPlot.Wpf.Axis)Activator.CreateInstance(Type.GetType("OxyPlot.Wpf." + axisConf.AxisTypeName + ",OxyPlot.Wpf"));
                    _axis.SetCurrentValue(OxyPlot.Wpf.Axis.PositionProperty, MultifunctionalChartData.AxesPositions[i]);
                    // Set axis style
                    Style _axis_style = null;
                    switch (MultifunctionalChartData.AxesPositions[i])
                    {
                        case AxisPosition.Left:
                            _axis.Title = (ChartData.TitleY == null ? "" : ChartData.TitleY);
                            _axis_style = YAxisStyle;
                            break;
                        case AxisPosition.Bottom:
                            _axis.Title = (ChartData.TitleX == null ? "" : ChartData.TitleX);
                            _axis_style = XAxisStyle;
                            break;
                        // implement more cases if required
                        default:
                            DllEntryPoint.LogMessage(Xvue.MSOT.Services.Log.EnumLogType.Info, "Warning", $"{Name}.constructAxes: Axis # {(i + 1)}: Unhandled AxisPosition: {Convert.ToString(MultifunctionalChartData.AxesPositions[i])}");
                            break;
                    }
                    OxyPlotView.Axes.Add(_axis);
                    // Style DependencyProperty has to be defined always after adding the axis to the view
                    if (_axis_style == null) { _axis_style = TryFindResource("OxyPlotDefaultAxisStyle") as Style; }
                    if (_axis_style != null) { _axis.SetCurrentValue(StyleProperty, _axis_style); }

                    i++;
                }
            }
            
            return OxyPlotView.Axes.Count() > 1;
        }

        private void contructSingleLineSeries(Style _series_style)
        {
            OxyPlot.Wpf.LineSeries _series = null;
            _series = new OxyPlot.Wpf.LineSeries();
            LineSeries _seriesModel = (LineSeries)_series.CreateModel();
            _seriesModel.Points.AddRange(LineSeries);
            OxyPlotView.Series.Add(_series);
            // Style DependencyProperty has to be defined always after adding the series to the view
            if (_series_style != null) { _series.SetCurrentValue(StyleProperty, _series_style); }
            AbsoluteMinimumX = LineSeries.Min(element => element.X);
            AbsoluteMaximumX = LineSeries.Max(element => element.X);
            AbsoluteMinimumY = LineSeries.Min(element => element.Y);
            AbsoluteMaximumY = LineSeries.Max(element => element.Y);
        }

        private void constructMultiSetAxes()
        {
            for (int k = 0; k < OxyPlotView.Axes.Count; k++)
            {
                Axis _axisModel = (Axis)Activator.CreateInstance((OxyPlotView.Axes[k].CreateModel()).GetType());
                System.Reflection.MethodInfo mi = _axisModel.GetType().GetMethod("ToDouble");
                switch (MultifunctionalChartData.AxesPositions[k])
                {
                    case AxisPosition.Left:
                        if (ChartData.MaximumY != null)
                            AbsoluteMaximumY = Convert.ToDouble(mi == null ? ChartData.MaximumY : mi.Invoke(_axisModel, new object[] { ChartData.MaximumY }));
                        if (ChartData.MinimumY != null)
                            AbsoluteMinimumY = Convert.ToDouble(mi == null ? ChartData.MinimumY : mi.Invoke(_axisModel, new object[] { ChartData.MinimumY }));
                        break;
                    case AxisPosition.Bottom:
                        if (ChartData.MaximumX != null)
                            AbsoluteMaximumX = Convert.ToDouble(mi == null ? ChartData.MaximumX : mi.Invoke(_axisModel, new object[] { ChartData.MaximumX }));
                        if (ChartData.MinimumX != null)
                            AbsoluteMinimumX = Convert.ToDouble(mi == null ? ChartData.MinimumX : mi.Invoke(_axisModel, new object[] { ChartData.MinimumX }));
                        break;
                    default:
                        DllEntryPoint.LogMessage(Xvue.MSOT.Services.Log.EnumLogType.Info, "Warning", $"{Name}.contructMultipleLineSeries: Axis # {(k + 1)}: Unhandled AxisPosition: {Convert.ToString(MultifunctionalChartData.AxesPositions[k])}");
                        break;
                }
            }
        }

        private void contructMultipleLineSeries(Style _series_style)
        {
            constructMultiSetAxes();

            bool initExtremes = true;
            // For each line series in the ViewModel
            foreach (MultifunctionalChartLineData line in ChartData.LinesData)
            {
                OxyPlot.Wpf.LineSeries _wpf_series = new OxyPlot.Wpf.LineSeries();
                LineSeries _series = (LineSeries)_wpf_series.CreateModel();

                foreach (ChartPoint cp in line.LinePoints)
                {
                    if (cp.X == null || cp.Y == null)
                    {
                        _series.Points.Add(DataPoint.Undefined);
                    }
                    else
                    {
                        DataPoint dp = new DataPoint();
                        for (int k = 0; k < OxyPlotView.Axes.Count; k++)
                        {
                            Axis _axisModel = (Axis)Activator.CreateInstance((OxyPlotView.Axes[k].CreateModel()).GetType());
                            System.Reflection.MethodInfo mi = _axisModel.GetType().GetMethod("ToDouble");
                            switch (MultifunctionalChartData.AxesPositions[k])
                            {
                                case AxisPosition.Left:
                                    dp.Y = Convert.ToDouble(mi == null ? cp.Y : mi.Invoke(_axisModel, new object[] { cp.Y }));
                                    break;
                                case AxisPosition.Bottom:
                                    dp.X = Convert.ToDouble(mi == null ? cp.X : mi.Invoke(_axisModel, new object[] { cp.X }));
                                    break;
                                default:
                                    DllEntryPoint.LogMessage(Xvue.MSOT.Services.Log.EnumLogType.Info, "Warning", $"{Name}.contructMultipleLineSeries: Axis # {(k + 1)}: Unhandled AxisPosition: {Convert.ToString(MultifunctionalChartData.AxesPositions[k])}");
                                    break;
                            }
                        }
                        _series.Points.Add(dp);
                        if (initExtremes)
                        {
                            if (ChartData.MinimumX == null) AbsoluteMinimumX = dp.X;
                            if (ChartData.MaximumX == null) AbsoluteMaximumX = dp.X;
                            if (ChartData.MinimumY == null) AbsoluteMinimumY = dp.Y;
                            if (ChartData.MaximumY == null) AbsoluteMaximumY = dp.Y;
                            /* Currently not implemented
                            AbsoluteMinimumZ = dp.Z;
                            AbsoluteMaximumZ = dp.Z;
                            */
                            initExtremes = false;
                        }
                        else
                        {
                            if (dp.X < AbsoluteMinimumX) { AbsoluteMinimumX = dp.X; }
                            if (dp.X > AbsoluteMaximumX) { AbsoluteMaximumX = dp.X; }
                            if (dp.Y < AbsoluteMinimumY) { AbsoluteMinimumY = dp.Y; }
                            if (dp.Y > AbsoluteMaximumY) { AbsoluteMaximumY = dp.Y; }
                            /* Currently not implemented
                            if (_Z < AbsoluteMinimumZ) { AbsoluteMinimumY = _Z; }
                            if (_Z > AbsoluteMaximumZ) { AbsoluteMaximumY = _Z; }                             
                            */
                        }
                    }
                }
                OxyPlotView.Series.Add(_wpf_series);
                // Style DependencyProperty has to be defined always after adding the series to the view
                if (_series_style != null) { _wpf_series.SetCurrentValue(StyleProperty, _series_style); }
                _wpf_series.SetCurrentValue(OxyPlot.Wpf.LineSeries.MarkerStrokeProperty, line.Color);
                _wpf_series.SetCurrentValue(OxyPlot.Wpf.Series.ColorProperty, line.Color);
                _wpf_series.SetCurrentValue(OxyPlot.Wpf.LineSeries.MarkerFillProperty, line.Color);
                _wpf_series.SetCurrentValue(OxyPlot.Wpf.Series.TitleProperty, line.Name);
            }
        }

        private void setAxesLimits()
        {
            // Set Axes limits if not defined via styling
            for (int i = 0; i < OxyPlotView.Axes.Count; i++)
            {
                bool isMajorStepStyled = false, isAbsoluteMinimumStyled = false, isAbsoluteMaximumStyled = false;
                double currentAbsoluteMaximum = 0, currentAbsoluteMinimum = 0, extremumOffset = 0;
                IEnumerable<Setter> axisStyleSetters = null;

                if (ShowGridLines)
                {
                    OxyPlotView.Axes[i].SetCurrentValue(OxyPlot.Wpf.Axis.MajorGridlineStyleProperty, LineStyle.Solid);
                    OxyPlotView.Axes[i].SetCurrentValue(OxyPlot.Wpf.Axis.MinorGridlineStyleProperty, LineStyle.Dot);
                }

                if (OxyPlotView.Axes[i].Style != null)
                    axisStyleSetters = OxyPlotView.Axes[i].Style.Setters.OfType<Setter>();

                if (axisStyleSetters != null)
                {
                    isMajorStepStyled = axisStyleSetters.Where(X => X.Property.Name == "MajorStep").Any();
                    isAbsoluteMinimumStyled = axisStyleSetters.Where(X => X.Property.Name == "AbsoluteMinimum").Any();
                    isAbsoluteMaximumStyled = axisStyleSetters.Where(X => X.Property.Name == "AbsoluteMaximum").Any();
                }
                
                switch (OxyPlotView.Axes[i].Position)
                {
                    case AxisPosition.Bottom:
                        currentAbsoluteMaximum = AbsoluteMaximumX;
                        currentAbsoluteMinimum = AbsoluteMinimumX;

                        if (currentAbsoluteMaximum == currentAbsoluteMinimum)
                        {
                            extremumOffset = 1;                          
                        }
                        else
                            extremumOffset = (currentAbsoluteMaximum - currentAbsoluteMinimum) * 0.05;
                        break;
                    case AxisPosition.Left:
                        currentAbsoluteMaximum = AbsoluteMaximumY;
                        currentAbsoluteMinimum = AbsoluteMinimumY;

                        if (currentAbsoluteMaximum == currentAbsoluteMinimum)
                        {
                            extremumOffset = (currentAbsoluteMaximum == 0 ? 1 : currentAbsoluteMaximum * 0.05);
                            break;
                        }

                        if (!isMajorStepStyled) // Y-Axis major step calculation based on the number of steps (nSteps defined below)
                        {
                            int nSteps = 10;
                            double step = (currentAbsoluteMaximum - currentAbsoluteMinimum) / nSteps;
                            double logStep = Math.Log10(step);
                            int precision = (int)(logStep < 0 ? -logStep + 1 : 0); // For rounding to first non-zero decimal ...
                            OxyPlotView.Axes[i].SetCurrentValue(OxyPlot.Wpf.Axis.MajorStepProperty, Math.Round(step, precision));
                        }

                        extremumOffset = OxyPlotView.Axes[i].MajorStep / 5;
                        break;
                    default:
                        DllEntryPoint.LogMessage(Xvue.MSOT.Services.Log.EnumLogType.Info, "Warning", $"{Name}.setAxesLimits: Axis # {(i + 1)}: Unhandled AxisPosition: {Convert.ToString(MultifunctionalChartData.AxesPositions[i])}");
                        return;
                }
                
                if (!isAbsoluteMinimumStyled)
                    OxyPlotView.Axes[i].SetCurrentValue(OxyPlot.Wpf.Axis.AbsoluteMinimumProperty, currentAbsoluteMinimum - extremumOffset);

                OxyPlotView.Axes[i].SetCurrentValue(OxyPlot.Wpf.Axis.MinimumProperty, OxyPlotView.Axes[i].AbsoluteMinimum);

                if (!isAbsoluteMaximumStyled)
                    OxyPlotView.Axes[i].SetCurrentValue(OxyPlot.Wpf.Axis.AbsoluteMaximumProperty, currentAbsoluteMaximum + extremumOffset);
                
                OxyPlotView.Axes[i].SetCurrentValue(OxyPlot.Wpf.Axis.MaximumProperty, OxyPlotView.Axes[i].AbsoluteMaximum);
            }
        }

        public void ConstructPlot()
        {
            // Set Grid size if defined
            if (GridWidth != 0 && GridHeight != 0) {
                ViewOxyPlotChartLayoutRoot.SetCurrentValue(WidthProperty, GridWidth);
                ViewOxyPlotChartLayoutRoot.SetCurrentValue(HeightProperty, GridHeight);
            }
            // Show Help button if enabled
            HelpButton.SetCurrentValue(VisibilityProperty, (ShowHelpButton ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden));

            if (OxyPlotView.Axes.Count == 0 && !constructAxes())
                return;

            //System.Diagnostics.Debug.WriteLine($"{Name}: Axes updated.");

            AbsoluteMinimumX = 0;
            AbsoluteMaximumX = 0;
            AbsoluteMinimumY = 0;
            AbsoluteMaximumY = 0;
            /* Currently not implemented
            double AbsoluteMinimumZ = 0;
            double AbsoluteMaximumZ = 0;
            */

            // Get the common style properties for each line series that will be added to the plot
            Style _series_style = null;
            _series_style = LineSeriesStyle;
            if (_series_style == null)
            {
                _series_style = TryFindResource("OxyPlotDefaultLineSeriesStyle") as Style;
            }

            OxyPlotView.Series.Clear();
            if (LineSeries != null)
            {
                // Data bound to the LineSeries property, we have a single line series
                contructSingleLineSeries(_series_style);
                //System.Diagnostics.Debug.WriteLine($"{Name}: Line series updated.");
            }
            else if (ChartData != null)
            {
                // Data bound to the Chartdata property, we have potentially many line series.
                contructMultipleLineSeries(_series_style);
                //System.Diagnostics.Debug.WriteLine($"{Name}: ChartData updated.");
            }
            if (OxyPlotView.Series.Count > 0)
                setAxesLimits();
        }

    }
}