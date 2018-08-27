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
using Xvue.MSOT.ViewModels.ProjectManager.ImagingSession;

namespace ViewMSOTc
{
	/// <summary>
	/// Interaction logic for ViewMspExportName.xaml
	/// </summary>
	public partial class ViewMspExportName : UserControl
	{
		public ViewMspExportName()
		{
			this.InitializeComponent();
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
            typeof(ViewMspExportName));
	
		private void okBtn_Click(object sender, RoutedEventArgs e)
        {		         
            CloseControl = true;
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            CloseControl = true;
        }

        private void textBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (textBox.SelectedText.Length == 0)
            {
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Input, new Action(() =>
                {
                    textBox.SelectAll();
                }));
            }
        }
    }
}