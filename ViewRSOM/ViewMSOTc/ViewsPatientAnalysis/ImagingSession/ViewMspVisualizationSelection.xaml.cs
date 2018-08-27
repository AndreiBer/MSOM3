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

namespace ViewMSOTc
{
    /// <summary>
    /// Interaction logic for ViewMspVisualizationSelection.xaml
    /// </summary>
    public partial class ViewMspVisualizationSelection : UserControl
    {
        public ViewMspVisualizationSelection()
        {
            InitializeComponent();
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            SelectionResultOK = true;
            CloseControl = true;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            SelectionResultOK = false;
            CloseControl = true;
        }

        public bool CloseControl
        {
            get { return (bool)GetValue(CloseControlProperty); }
            set { SetValue(CloseControlProperty, value); }
        }

        public static readonly DependencyProperty CloseControlProperty =
            DependencyProperty.Register(
            "CloseControl",
            typeof(bool),
            typeof(ViewMspVisualizationSelection));

        public bool SelectionResultOK
        {
            get { return (bool)GetValue(SelectionResultOKProperty); }
            set { SetValue(SelectionResultOKProperty, value); }
        }

        public static readonly DependencyProperty SelectionResultOKProperty =
            DependencyProperty.Register(
            "SelectionResultOK",
            typeof(bool),
            typeof(ViewMspVisualizationSelection));

    }
}
