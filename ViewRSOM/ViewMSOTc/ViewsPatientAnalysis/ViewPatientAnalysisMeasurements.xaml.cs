using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xvue.MSOT.Common;
using Xvue.MSOT.ViewModels.Imaging;
using Xvue.MSOT.ViewModels.ProjectManager.ImagingSession;

namespace ViewMSOTc
{
    /// <summary>
    /// Interaction logic for ViewPatientAnalysisMeasurements.xaml
    /// </summary>
    public partial class ViewPatientAnalysisMeasurements : UserControl
    {
        CollectionViewSource _src;
        PropertyGroupDescription _groupByRoiLayerName = new PropertyGroupDescription("RoiLayerName");
        SortDescription _sortByRoiLayerName = new SortDescription("RoiLayerName", ListSortDirection.Ascending);
        GroupStyle _roiLayerNameGroupStyle;

        public ViewPatientAnalysisMeasurements()
        {
            InitializeComponent();
            _roiLayerNameGroupStyle = new GroupStyle() { ContainerStyle = (Style)this.FindResource("RoiLayerNameGroupStyle") };
        }

        private void roiLayersItemsControl_ManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _src = this.Resources["groupableRoiLayersSource"] as CollectionViewSource;
            refreshRoisViewGroups();
        }

        private void refreshRoisViewGroups()
        {
            if (_src != null)
            {
                using (_src.DeferRefresh())
                {
                    _src.GroupDescriptions.Clear();
                    _src.SortDescriptions.Clear();
                }
                using (_src.DeferRefresh())
                {
                    _src.GroupDescriptions.Add(_groupByRoiLayerName);
                    _src.SortDescriptions.Add(_sortByRoiLayerName);
                }
            }

            if (roiLayersItemsControl != null)
            {
                roiLayersItemsControl.GroupStyle.Clear();
                roiLayersItemsControl.GroupStyle.Add(_roiLayerNameGroupStyle);
            }
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            refreshRoisViewGroups();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            regionPropertiesPopup.Child = null;

            ViewModelSuperSessionBase model = this.DataContext as ViewModelSuperSessionBase;

            ToggleButton roiLayerToggleButton = sender as ToggleButton;
            ViewModelRoiLayer roiLayerModel = roiLayerToggleButton.DataContext as ViewModelRoiLayer;
            roiLayerModel.LoadRegionProperties(model.RoiLayerViewingProperty);
            ViewRegionProperties regionPropertiesControl = new ViewRegionProperties() { DataContext = roiLayerModel.Parent };

            Binding newBinding = new Binding();
            newBinding.Path = new PropertyPath("IsChecked");
            newBinding.Source = sender;
            newBinding.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(regionPropertiesPopup, Popup.IsOpenProperty, newBinding);            
            regionPropertiesPopup.PlacementTarget = roiLayerToggleButton;
            regionPropertiesPopup.ControlToggleButton = roiLayerToggleButton;
            regionPropertiesPopup.Child = regionPropertiesControl;
            regionPropertiesPopup.SetCurrentValue(Popup.IsOpenProperty, true);
        }

    }
}
