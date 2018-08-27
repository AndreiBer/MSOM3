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
    /// Interaction logic for ViewBackgroundControlHeader.xaml
	/// </summary>
	public partial class ViewBackgroundControlHeader : UserControl
	{
        public ViewBackgroundControlHeader()
		{
			this.InitializeComponent();
		}

        private void selectParentTabItem()
        {
            bool? state = visibilityToggleButton.IsChecked;
            if (state != true)
            {
                return;
            }

            DependencyObject parentTabItemDependencyObject = Xvue.Framework.Views.WPF.VisualTreeBrowser.GetAncestorByType(this, typeof(TabItem));
            if (parentTabItemDependencyObject != null)
            {
                (parentTabItemDependencyObject as TabItem).SetCurrentValue(TabItem.IsSelectedProperty, true);
            }
        }

        private void visibilityToggleButton_Click(object sender, RoutedEventArgs e)
        {
            selectParentTabItem();
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            selectParentTabItem();
        }

	}
}