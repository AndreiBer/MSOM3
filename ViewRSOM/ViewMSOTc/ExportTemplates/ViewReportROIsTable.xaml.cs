using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Xvue.Framework.API.Converters;
using Xvue.MSOT.Services.Imaging;
using Xvue.MSOT.ViewModels.Imaging;

namespace ViewMSOTc
{
	/// <summary>
    /// Interaction logic for ViewReportROIsTable.xaml
	/// </summary>
	public partial class ViewReportROIsTable : UserControl
	{

        public ViewReportROIsTable()
        {
            this.InitializeComponent();
        }

        public RegionStatisticType RoiLayerViewingStatistic
        {
            get { return (RegionStatisticType)GetValue(RoiLayerViewingStatisticProperty); }
            set { SetValue(RoiLayerViewingStatisticProperty, value); }
        }

        public static readonly DependencyProperty RoiLayerViewingStatisticProperty =
           DependencyProperty.Register(
              "RoiLayerViewingStatistic",
              typeof(RegionStatisticType),
              typeof(ViewReportROIsTable),
              new FrameworkPropertyMetadata(
                new PropertyChangedCallback(RoiLayerViewingStatisticChanged)));

        private static void RoiLayerViewingStatisticChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ViewReportROIsTable control = d as ViewReportROIsTable;
            control.refreshMeasurementName();
        }

        private void refreshMeasurementName()
        {
            measurementNameTextBlock.Text = EnumDescriptionConverter.GetFriendlyName(RoiLayerViewingStatistic);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public IList<ViewModelRegion2DAllLayersDrawing> Regions
        {
            get { return (IList<ViewModelRegion2DAllLayersDrawing>)GetValue(RegionsProperty); }
            set { SetValue(RegionsProperty, (IList<ViewModelRegion2DAllLayersDrawing>)value); }
        }

        public static readonly DependencyProperty RegionsProperty =
           DependencyProperty.Register(
              "Regions",
              typeof(IList<ViewModelRegion2DAllLayersDrawing>),
              typeof(ViewReportROIsTable),
              new FrameworkPropertyMetadata(
                new PropertyChangedCallback(RegionsChanged)));

        private static void RegionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ViewReportROIsTable control = d as ViewReportROIsTable;
            control.refreshRegions();
        }

        private void refreshRegions()
        {
            bool show = false;
            if (Regions != null)
            {
                if (Regions.Count > 0)
                {
                    show = true;
                }
            }
            if (show)
            {

            }
            else
            {

            }
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            refreshMeasurementName();
        }

	}
}