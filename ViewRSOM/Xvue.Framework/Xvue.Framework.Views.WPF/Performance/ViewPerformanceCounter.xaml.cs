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

namespace Xvue.Framework.Views.WPF.Performance
{
	/// <summary>
	/// Interaction logic for ViewPerformanceCounter.xaml
	/// </summary>
	public partial class ViewPerformanceCounter : UserControl
	{
		public ViewPerformanceCounter()
		{
			this.InitializeComponent();
		}

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                this.Visibility = System.Windows.Visibility.Collapsed;
            else
                this.Visibility = System.Windows.Visibility.Visible;
        }
	}
}