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
using System.Windows.Shapes;

namespace ViewMSOT.UIControls
{
	/// <summary>
    /// Interaction logic for ViewSaveAs.xaml
	/// </summary>
	public partial class ViewSaveAs : UserControl
	{
		public ViewSaveAs()
		{
            this.InitializeComponent();
		}

        public ViewSaveAs(string label, ICommand okCommand, string name)
            : this()
        {
            mainLabel.Content = label;
            okBtn.Command = okCommand;
            textBox.Text = name;
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
            typeof(ViewSaveAs));

        public ICommand CheckInputLength
        {
            get { return (ICommand)GetValue(CheckInputLengthProperty); }
            set { SetValue(CheckInputLengthProperty, value); }
        }

        public static readonly DependencyProperty CheckInputLengthProperty =
           DependencyProperty.Register(
              "CheckInputLength",
              typeof(ICommand),
              typeof(ViewSaveAs),
              new FrameworkPropertyMetadata());

        public bool? SaveAsOK
        {
            get { return (bool)GetValue(SaveAsOKProperty); }
            set { SetValue(SaveAsOKProperty, value); }
        }

        public static readonly DependencyProperty SaveAsOKProperty =
            DependencyProperty.Register(
            "SaveAsOK",
            typeof(bool?),
            typeof(ViewSaveAs),
            new FrameworkPropertyMetadata(false));

        private void okBtn_Click(object sender, RoutedEventArgs e)
        {
            SetCurrentValue(SaveAsOKProperty, true);
            CloseControl = true;
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
                okBtn.IsEnabled = false;
            else
                okBtn.IsEnabled = true;

            if (CheckInputLength == null)
                return;

            if (CheckInputLength.CanExecute(textBox.Text))
            {
                okBtn.IsEnabled = true;
                textBox.Foreground = Brushes.Black;
                textBox.ToolTip = null;
            }
            else
            {
                okBtn.IsEnabled = false;
                textBox.Foreground = Brushes.Red;
                textBox.ToolTip = "Too long study name.";
            }
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            CloseControl = true;
        }

	}
}