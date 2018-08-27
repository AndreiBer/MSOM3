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
using ViewMSOT.UIControls;
using Xvue.MSOT.ViewModels.ProjectManager.ImagingSession;

namespace ViewMSOTc
{
	/// <summary>
    /// Interaction logic for ViewImagePropertiesGridComparisonMenus.xaml
	/// </summary>
	public partial class ViewImagePropertiesGridComparisonMenus : UserControl
	{
        public ViewImagePropertiesGridComparisonMenus()
		{
			this.InitializeComponent();
		}

        private void roiSubMenuButton_Click(object sender, RoutedEventArgs e)
        {
            roiDesignMenuItem.SetCurrentValue(MenuItem.IsSubmenuOpenProperty, false);
        }

	}
}