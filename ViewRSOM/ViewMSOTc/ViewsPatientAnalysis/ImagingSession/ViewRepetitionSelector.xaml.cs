using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
	/// Interaction logic for ViewMultiSliceSelector.xaml
	/// </summary>
	public partial class ViewRepetitionSelector : UserControl
	{
		public ViewRepetitionSelector()
		{
			this.InitializeComponent();          
		}

        private void patternButton_Click(object sender, RoutedEventArgs e)
        {
            patternSelectWindow.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            patternSelectWindow.Show();
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                RepetitionsList.SelectAll();
            }
        }

	}
}