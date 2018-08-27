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
using ViewMSOT.UIControls.Adorners;
using Xvue.Framework.Views.WPF.Controls;
using Xvue.MSOT.ViewModels;
using Xvue.MSOT.ViewModels.ProjectManager.ImagingSession;

namespace ViewMSOTc
{
    /// <summary>
    /// Interaction logic for ViewScanInformation.xaml
    /// </summary>
    public partial class ViewScanInformation : UserControl
    {
        public ViewScanInformation()
        {
            InitializeComponent();
        }

        public ClosingPopup ParentPopup
        {
            get { return (ClosingPopup)GetValue(ParentPopupProperty); }
            set { SetValue(ParentPopupProperty, value); }
        }

        public static readonly DependencyProperty ParentPopupProperty =
           DependencyProperty.Register(
              "ParentPopup",
              typeof(ClosingPopup),
              typeof(ViewScanInformation),
              new FrameworkPropertyMetadata());

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

        private void textBox_GotFocus(object sender, RoutedEventArgs e)
        {
            focusSelectTextBox(sender as TextBox);
        }

        private void focusSelectTextBox(TextBox textBoxElement)
        {
            if (textBoxElement == null)
                return;

            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Input, new Action(delegate()
            {
                textBoxElement.Focus();
                Keyboard.Focus(textBoxElement);
                textBoxElement.SelectAll();
            }));
        }

    }
}
