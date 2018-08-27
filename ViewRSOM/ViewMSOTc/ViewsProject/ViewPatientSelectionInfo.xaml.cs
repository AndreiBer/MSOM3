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

namespace ViewRSOM
{
    
    /// <summary>
    /// Interaction logic for ViewPatientSelectionInfo.xaml
    /// </summary>
    public partial class ViewPatientSelectionInfo : UserControl
    {
        public ViewPatientSelectionInfo()
        {
            InitializeComponent();
            IsPatientInfoVisible = System.Windows.Visibility.Collapsed;
        }

        public event EventHandler SelectPatient;
        protected virtual void OnSelectPatient()
        {
            //SelectPatient.Invoke(this,EventArgs.Empty);
            //System.Windows.MessageBox.Show("Bravo");
        }

        private void selectPatientButtonClick(object sender, RoutedEventArgs e)
        {
            OnSelectPatient();
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
            {
                IsPatientInfoVisible = System.Windows.Visibility.Collapsed;
            }
            else
            {
                IsPatientInfoVisible = System.Windows.Visibility.Visible;
            }
        }

        public Visibility IsPatientInfoVisible
        {
            get { return (Visibility)GetValue(IsPatientInfoVisibleProperty); }
            set { SetValue(IsPatientInfoVisibleProperty, (Visibility)value); }
        }
        public static readonly DependencyProperty IsPatientInfoVisibleProperty =
            DependencyProperty.Register(
             "IsPatientInfoVisible",
             typeof(Visibility),
             typeof(ViewPatientSelectionInfo),
             new FrameworkPropertyMetadata());
    }
}
