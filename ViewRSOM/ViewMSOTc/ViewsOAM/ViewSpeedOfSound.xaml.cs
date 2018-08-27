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
    /// Interaction logic for ViewSpeedOfSound.xaml
    /// </summary>
    public partial class ViewSpeedOfSound : UserControl
    {
        public ViewSpeedOfSound()
        {
            InitializeComponent();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    TraversalRequest a = new TraversalRequest(FocusNavigationDirection.Next);
                    (e.Source as UIElement).MoveFocus(a);

                }
            }
            catch { }
        }

        private void TextBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                TextBox txt = sender as TextBox;
                txt.CaretIndex = 0;
            }
            catch { }
        }

        private void TextBox_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            try
            {
                double newValue;
                TextBox textBox = sender as TextBox;
                if (!Double.TryParse(textBox.Text, out newValue))
                    textBox.SetCurrentValue(TextBox.TextProperty, Convert.ToString(SoSSlider.Value));
            }
            catch { }
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

    }
}
