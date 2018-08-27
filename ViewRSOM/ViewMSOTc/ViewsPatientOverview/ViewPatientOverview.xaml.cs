using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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



namespace ViewRSOM
{
    
    /// <summary>
    /// Interaction logic for ViewPatientOverview.xaml
    /// </summary>
    public partial class ViewPatientOverview : UserControl
    {
        
        public event EventHandler SwitchToAnalysis;
        protected virtual void OnSwitchToAnalysis()
        {
            SwitchToAnalysis.Invoke(this,EventArgs.Empty);
        }

        CollectionViewSource _src;
        PropertyGroupDescription _groupByType = new PropertyGroupDescription("VisualizationAndAnalysisType");
        PropertyGroupDescription _groupByDatePeriod = new PropertyGroupDescription("DatePeriod");
        SortDescription _sortByDate = new SortDescription("CreationTime", ListSortDirection.Descending);
        SortDescription _sortByType = new SortDescription("TypeFriendlyName", ListSortDirection.Ascending);
        GroupStyle _dateGroupStyle;
        GroupStyle _typeGroupStyle;

        Dictionary<TouchDevice, Point> _touchDevicesPoints;
        double _touchZoomDistance;

        public ViewPatientOverview()
        {
            _touchDevicesPoints = new Dictionary<TouchDevice, Point>();
            _touchZoomDistance = 0;
            InitializeComponent();
            _dateGroupStyle = new GroupStyle() { ContainerStyle = (Style)this.FindResource("DateGroupStyle") };
            _typeGroupStyle = new GroupStyle() { ContainerStyle = (Style)this.FindResource("TypeGroupStyle") };
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _src = this.Resources["groupableSuperSessionSource"] as CollectionViewSource;
            refreshSuperSessionViewGroups();

        }

        private void groupByTypeMenuItem_Checked(object sender, RoutedEventArgs e)
        {
           
            refreshSuperSessionViewGroups();
        }

        private void groupByDateMenuItem_Checked(object sender, RoutedEventArgs e)
        {
           
        }

        private void refreshSuperSessionViewGroups()
        {
            
        }

        private void mainListBox_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            refreshSuperSessionViewGroups();
        }

        private void filterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        void _src_Filter(object sender, FilterEventArgs e)
        {
           
        }

        private void clearFilterButton_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void mainListBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OnSwitchToAnalysis();
        }

        private void mainListBox_ManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            

        }

        private void mainListBox_TouchEnter(object sender, TouchEventArgs e)
        {
            
        }

        private void mainListBox_TouchLeave(object sender, TouchEventArgs e)
        {
            _touchDevicesPoints.Remove(e.TouchDevice);
        }

        private void mainListBox_TouchMove(object sender, TouchEventArgs e)
        {
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }

    }
}
