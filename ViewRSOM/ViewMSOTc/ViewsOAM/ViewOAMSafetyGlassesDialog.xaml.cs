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
    /// Interaction logic for ViewOAMSafetyGlassesDialog.xaml
    /// </summary>
    public partial class ViewOAMSafetyGlassesDialog : UserControl
    {
        public ViewOAMSafetyGlassesDialog()
        {
            SafetyOK = null;
            InitializeComponent();
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
            typeof(ViewOAMSafetyGlassesDialog));

        public bool? SafetyOK
        {
            get { return (bool)GetValue(SafetyOKProperty); }
            set { SetValue(SafetyOKProperty, value); }
        }

        public static readonly DependencyProperty SafetyOKProperty =
            DependencyProperty.Register(
            "SafetyOK",
            typeof(bool?),
            typeof(ViewOAMSafetyGlassesDialog));

        private void continueButton_Click(object sender, RoutedEventArgs e)
        {
            SafetyOK = true;
            CloseControl = true;
        }

        private void userControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                focusMainElement();
            }
        }

        private void focusMainElement()
        {
            giveElementFocus(continueButton);
        }

        private void giveElementFocus(FrameworkElement element)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Input, new Action(delegate()
            {
                element.Focus();
                Keyboard.Focus(element);
            }));
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            SafetyOK = null;
            CloseControl = true;
        }

    }
}
