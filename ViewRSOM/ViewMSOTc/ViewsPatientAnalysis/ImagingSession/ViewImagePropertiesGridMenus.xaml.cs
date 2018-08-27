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
using Xvue.MSOT.Services.Core;
using Xvue.MSOT.ViewModels.ProjectManager.ImagingSession;

namespace ViewMSOTc
{
	/// <summary>
	/// Interaction logic for ViewImagePropertiesGridMenus.xaml
	/// </summary>
	public partial class ViewImagePropertiesGridMenus : UserControl
	{
		public ViewImagePropertiesGridMenus()
		{
			this.InitializeComponent();
		}

		private void roiSubMenuButton_Click(object sender, RoutedEventArgs e)
		{
			roiDesignMenuItem.SetCurrentValue(MenuItem.IsSubmenuOpenProperty, false);
		}

		private void menuItem_Click(object sender, RoutedEventArgs e)
		{
            ViewModelMspSelectionImagingSession dc = DataContext as ViewModelMspSelectionImagingSession;
            if (dc == null)
                return;

            try
			{                
				dc.InitSelectionLoad();               
				ModalChildWindow.ShowDialog(
					"Select images",
					 new ViewMspVisualizationSelection(),
					 DataContext, 
					 ViewMspVisualizationSelection.SelectionResultOKProperty);
			}
			catch (Exception ex)
			{
				Window parentWindow = Window.GetWindow(this);
				dc.MSOTService.NotifyUserOnAction("Error trying to open MSP visualization selector: " + ex.Message, parentWindow.Title, UserNotificationType.Error, false);
			}
		}

		
	}
}