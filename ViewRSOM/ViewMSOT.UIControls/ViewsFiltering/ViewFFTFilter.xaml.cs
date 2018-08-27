using System;
using System.Collections.Generic;
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

namespace ViewMSOT.UIControls
{
	/// <summary>
	/// Interaction logic for ViewFFTFilter.xaml
	/// </summary>
	public partial class ViewFFTFilter : UserControl
	{
		public ViewFFTFilter()
		{
			this.InitializeComponent();
		}

        private void highCutOffTextBoxTextBox_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            try
            {
                double newValue;
                TextBox textBox = sender as TextBox;
                if (!Double.TryParse(textBox.Text, out newValue))
                    textBox.SetCurrentValue(TextBox.TextProperty, Convert.ToString(highCutOffSlider.Value));
            }
            catch { }
        }

        private void lowCutOffTextBoxTextBox_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            try
            {
                double newValue;
                TextBox textBox = sender as TextBox;
                if (!Double.TryParse(textBox.Text, out newValue))
                    textBox.SetCurrentValue(TextBox.TextProperty, Convert.ToString(lowCutOffSlider.Value));
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