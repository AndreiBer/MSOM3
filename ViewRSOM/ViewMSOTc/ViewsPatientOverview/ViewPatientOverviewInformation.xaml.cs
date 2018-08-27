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
using Xvue.MSOT.ViewModels.ProjectManager;

namespace ViewMSOTc
{
    /// <summary>
    /// Interaction logic for ViewPatientOverviewInformation.xaml
    /// </summary>
    public partial class ViewPatientOverviewInformation : UserControl
    {
        public ViewPatientOverviewInformation()
        {
            InitializeComponent();
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.DataContext != null)
            {
                SetCurrentValue(IsEnabledProperty, true);
            }
            else
            {
                SetCurrentValue(IsEnabledProperty, false);
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            ViewMSOTcSystem.ChangeModalAndEditState(false, this);
            KeyboardNavigation.SetTabNavigation(this, KeyboardNavigationMode.Continue);
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            ViewMSOTcSystem.ChangeModalAndEditState(false, this);
            KeyboardNavigation.SetTabNavigation(this, KeyboardNavigationMode.Continue);
        }

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            ViewMSOTcSystem.ChangeModalAndEditState(true, this);
            KeyboardNavigation.SetTabNavigation(this, KeyboardNavigationMode.Cycle);
        }

    }
}
