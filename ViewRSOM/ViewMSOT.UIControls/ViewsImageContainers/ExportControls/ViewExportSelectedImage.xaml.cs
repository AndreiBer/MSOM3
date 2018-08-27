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
using Xvue.MSOT.ViewModels.ProjectManager.ImagingSession;

namespace ViewMSOT.UIControls
{
	/// <summary>
	/// Interaction logic for ViewExportStack.xaml
	/// </summary>
	public partial class ViewExportSelectedImage : Window
	{
		public ViewExportSelectedImage()
		{
			InitializeComponent();
            //Owner = ViewMSOT.UIControls.App.GetActiveWindow();
            ShowInTaskbar = false;
			// Insert code required on object creation below this point.
            
		}

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
			
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModelImagingSessionBase dc = this.DataContext as ViewModelImagingSessionBase;
                DataTemplate usedTemplate = viewboxContentControl.ContentTemplate;
                dc.ExportImageTemplate = usedTemplate;
                dc.IsSelectedImageOnly = true;
                dc.CheckImageStackExportFormat();
            }
            catch { }
        }

        private void exportBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	ViewExportNameSingleImage dlg = new ViewExportNameSingleImage();
            Xvue.MSOT.ViewModels.ProjectManager.ImagingSession.ViewModelImagingSessionBase dc = this.DataContext as Xvue.MSOT.ViewModels.ProjectManager.ImagingSession.ViewModelImagingSessionBase;
            dlg.DataContext = dc;
            Nullable<bool> dialogResult = dlg.ShowDialog();
            if (dialogResult == true && !dc.UserCanceledImageStackExport)
                this.Close();
        }
	}
}