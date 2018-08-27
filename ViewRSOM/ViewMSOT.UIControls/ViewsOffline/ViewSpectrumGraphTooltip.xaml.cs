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
	/// Interaction logic for ViewComponentGraphTooltip.xaml
	/// </summary>
	public partial class ViewSpectrumGraphTooltip : UserControl
	{
		public ViewSpectrumGraphTooltip()
		{
			this.InitializeComponent();
		}

        public List<OxyPlot.DataPoint> LineSeries
        {
            get { return (List<OxyPlot.DataPoint>)GetValue(LineSeriesProperty); }
            set { SetValue(LineSeriesProperty, (List<OxyPlot.DataPoint>)value); }
        }

        public static readonly DependencyProperty LineSeriesProperty =
            DependencyProperty.Register(
             "LineSeries",
             typeof(List<OxyPlot.DataPoint>),
             typeof(ViewSpectrumGraphTooltip),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(changedLineSeries)));

        private static void changedLineSeries(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ViewSpectrumGraphTooltip control = d as ViewSpectrumGraphTooltip;
            control.refreshPlot();
        }

        public Double AxisXMax
        {
            get { return (Double)GetValue(AxisXMaxProperty); }
            set { SetValue(AxisXMaxProperty, (Double)value); }
        }

        public static readonly DependencyProperty AxisXMaxProperty =
            DependencyProperty.Register(
             "AxisXMax",
             typeof(Double),
             typeof(ViewSpectrumGraphTooltip),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(changedLineSeries)));

        public Double AxisXMin
        {
            get { return (Double)GetValue(AxisXMinProperty); }
            set { SetValue(AxisXMinProperty, (Double)value); }
        }

        public static readonly DependencyProperty AxisXMinProperty =
            DependencyProperty.Register(
             "AxisXMin",
             typeof(Double),
             typeof(ViewSpectrumGraphTooltip),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(changedLineSeries)));

        private void refreshPlot()
        {
            try
            {
                if (this.DataContext != null && LineSeries != null && spectraPlotView.Axes != null)
                    if (spectraPlotView.Axes.Count > 1)
                    {
                        OxyPlot.Wpf.Axis xAxis = spectraPlotView.Axes[1];
                        if (xAxis != null)
                        {
                            int multiplier = (int)((AxisXMax - AxisXMin) / 300);
                            if (multiplier < 1)
                            {
                                xAxis.MajorStep = double.NaN;
                                xAxis.MinorStep = double.NaN;
                            }
                            else
                            {
                                xAxis.MajorStep = 100 * multiplier;
                                xAxis.MinorStep = 100 * multiplier;
                            }
                        }
                    }

                spectraPlotView.InvalidatePlot();
            }
            catch (Exception ex)
            {
                //Window parentWindow = Window.GetWindow(this);
                //ViewMSOTSystem.NotifyUserOnError("Error refreshing plot: " + ex.Message, parentWindow.Title, true, false, parentWindow);
            }
        }   
     
	}

}