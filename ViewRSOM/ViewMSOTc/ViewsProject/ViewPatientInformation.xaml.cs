using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;


namespace ViewRSOM
{
    /// <summary>
    /// Interaction logic for ViewPatientInformation.xaml
    /// </summary>
    public partial class ViewPatientInformation : UserControl
    {
        //ViewModelPatient _model;

        public ViewPatientInformation()
        {
            InitializeComponent();
        }

        public bool IsDuplicateId
        {
            get { return (bool)GetValue(IsDuplicateIdProperty); }
            set { SetValue(IsDuplicateIdProperty, (bool)value); }
        }
        public static readonly DependencyProperty IsDuplicateIdProperty =
            DependencyProperty.Register(
             "IsDuplicateId",
             typeof(bool),
             typeof(ViewPatientInformation),
             new FrameworkPropertyMetadata(
                 new PropertyChangedCallback(isDuplicateIdChanged)));

        private static void isDuplicateIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
           
        }

        FrameworkElement _lastFocusedFrameworkElement = null;
        private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        {
            _lastFocusedFrameworkElement = e.OriginalSource as FrameworkElement;
        }

        public void FocusLastInputElement()
        {
            if (_lastFocusedFrameworkElement != null)
            {
                cancelSafetyDialog(_lastFocusedFrameworkElement);
            }
        }

        private void cancelSafetyDialog(FrameworkElement element)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Input, new Action(delegate()
            {
                element.Focus();
                Keyboard.Focus(element);
            }));
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.DataContext != null)
            {
                //_model = this.DataContext as ViewModelPatient;
                SetCurrentValue(IsEnabledProperty, true);
            }
            else
            {
                //_model = null;
                SetCurrentValue(IsEnabledProperty, false);
            }
        }

        private void ComboBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            ComboBox cb = sender as ComboBox;

            if (cb.IsDropDownOpen)
            {
                return;
            }
            else
            {
                if (e.Key == Key.Down || e.Key == Key.Up || e.Key == Key.Right || e.Key == Key.Left)
                {
                    cb.SetCurrentValue(ComboBox.IsDropDownOpenProperty, true);
                    e.Handled = true;
                }
            }
        }

        private void textBox_GotFocus(object sender, RoutedEventArgs e)
        {
            selectAllTextBox(sender as TextBox);
        }

        private void selectAllTextBox(TextBox textBoxElement)
        {
            if (textBoxElement == null)
                return;

            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Input, new Action(delegate()
            {
                textBoxElement.SelectAll();
            }));
        }

        private void textBox_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (sender is TextBox)
            {
                BindingExpression binding = BindingOperations.GetBindingExpression(sender as UIElement, TextBox.TextProperty);
                if (binding != null)
                    binding.UpdateSource();
            }
        }

    }
}
