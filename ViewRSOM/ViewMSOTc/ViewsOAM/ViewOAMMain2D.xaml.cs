using System;
using System.Collections.Generic;
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
using Xvue.MSOT.Hardware.ViewModels.Experiment;

namespace ViewMSOTc
{
    /// <summary>
    /// Interaction logic for ViewOAMMain2D.xaml
    /// </summary>
    public partial class ViewOAMMain2D : UserControl
    {
        Brush _inactiveMainPanelBrush;

        public ViewOAMMain2D()
        {
            InitializeComponent();

            _inactiveMainPanelBrush = (Brush)Application.Current.FindResource("UidIconInvertedDisabledBrush");
        }

        public bool IsAnatomicalFocused
        {
            get { return (bool)GetValue(IsAnatomicalFocusedProperty); }
            set { SetValue(IsAnatomicalFocusedProperty, (bool)value); }
        }
        public static readonly DependencyProperty IsAnatomicalFocusedProperty =
            DependencyProperty.Register(
             "IsAnatomicalFocused",
             typeof(bool),
             typeof(ViewOAMMain2D),
            new FrameworkPropertyMetadata(
                    new PropertyChangedCallback(IsAnatomicalFocusedChanged)));

        private static void IsAnatomicalFocusedChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ViewOAMMain2D control = (source as ViewOAMMain2D);
            control.refreshBorders();
        }

        private void refreshBorders()
        {
            if (IsAnatomicalFocused)
            {
                anatomicalBorder.BorderBrush = Brushes.White;
                mspBorder.BorderBrush = _inactiveMainPanelBrush;
            }
            else
            {
                anatomicalBorder.BorderBrush = _inactiveMainPanelBrush;
                mspBorder.BorderBrush = Brushes.White;
            }
        }

        private void anatomicalBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ViewModelExperimentBase model = this.DataContext as ViewModelExperimentBase;
            if (model != null)
            {
                model.AnatomicalPreview.TryFocus();
            }
        }

        private void mspBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ViewModelExperimentBase model = this.DataContext as ViewModelExperimentBase;
            if (model != null)
            {
                model.MspPreview.TryFocus();
            }
        }

    }
}
