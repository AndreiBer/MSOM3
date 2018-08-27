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
using Xvue.Framework.Views.WPF.Controls;
using Xvue.MSOT.ViewModels;

namespace ViewMSOTc
{
    /// <summary>
    /// Interaction logic for ViewOAMScanInformation.xaml
    /// </summary>
    public partial class ViewOAMScanInformation : UserControl
    {      

        public ViewOAMScanInformation()
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
              typeof(ViewOAMScanInformation),
              new FrameworkPropertyMetadata());

        public bool IsPresetChangeEnabled
        {
            get { return (bool)GetValue(IsPresetChangeEnabledProperty); }
            set { SetValue(IsPresetChangeEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsPresetChangeEnabledProperty =
           DependencyProperty.Register(
              "IsPresetChangeEnabled",
              typeof(bool),
              typeof(ViewOAMScanInformation),
              new FrameworkPropertyMetadata());

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ModalChildWindow.ShowDialog("Select preset", new ViewOAMPresetsPopup(), this.DataContext);
            if (ParentPopup != null)
                ParentPopup.SetCurrentValue(ClosingPopup.IsOpenProperty, false);
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
