using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ViewMSOT.UIControls;
using Xvue.MSOT.DataModels.Plugins.ProjectManager;
using Xvue.MSOT.Services.Core;
using Xvue.MSOT.ViewModels.ProjectManager.ImagingSession;

namespace ViewMSOTc
{
	/// <summary>
	/// Interaction logic for ViewSuperSessionsMenu.xaml
	/// </summary>
	public partial class ViewSuperSessionsMenu : UserControl
	{
		public ViewSuperSessionsMenu()
		{
			this.InitializeComponent();
		}

		private void MenuItem_Click(object sender, RoutedEventArgs e)
		{
            ViewModelImagingSuperSession session = this.DataContext as ViewModelImagingSuperSession;
            if (session == null)
                return;

            try
			{
				if (session.LinkedSuperSession != null)
					session.CommandSaveOpenedChanges.Execute(null);
			}
			catch (Exception ex)
			{
				session.MSOTService.NotifyUserOnAction("Error saving visualization." + ex.Message, "Error", UserNotificationType.Error, false);
			}
		}

		public ICommand CreateComparisonAnalysis
		{
			get { return (ICommand)GetValue(CreateComparisonAnalysisProperty); }
			set { SetValue(CreateComparisonAnalysisProperty, value); }
		}

		public static readonly DependencyProperty CreateComparisonAnalysisProperty =
			DependencyProperty.Register(
			"CreateComparisonAnalysis",
			typeof(ICommand),
			typeof(ViewSuperSessionsMenu),
			new FrameworkPropertyMetadata());

		private void ExportImages_Click(object sender, RoutedEventArgs e)
		{
            ViewModelImagingSuperSession ss = this.DataContext as ViewModelImagingSuperSession;
            if (ss == null)
                return;

            try
			{
				
				foreach (ViewModelImagingSessionBase dc in ss.ImagingSessions)
				{
					if ((ss.VisualizationAndAnalysisType == VisualizationAndAnalysis.Snapshot) || ((ss.VisualizationAndAnalysisType == VisualizationAndAnalysis.Visualization) && dc.IsVisible))
					{
						//if (!dc.Is3DImagingSession)
						//{
						//    dc.PrepareExportedSelectedImage();

						//    ModalChildWindow exportImagesPopupChildWindow;
						//    ViewMspExportImages exportImagesPopup = new ViewMspExportImages();
						//    exportImagesPopupChildWindow = new ModalChildWindow("Export Image(s)", exportImagesPopup, dc);
						//    var exportImagesPopupChildWindowControlStyle = Application.Current.FindResource("ChildWindowWiderCaptionStyle");
						//    exportImagesPopupChildWindow.CloseButtonVisibility = System.Windows.Visibility.Collapsed;
						//    exportImagesPopupChildWindow.Style = (Style)exportImagesPopupChildWindowControlStyle;
						//    Binding replyBinding = new Binding("UserReply");
						//    replyBinding.Source = exportImagesPopupChildWindow;
						//    replyBinding.Mode = BindingMode.TwoWay;
						//    exportImagesPopup.SetBinding(ViewMspExportImages.UserReplyProperty, replyBinding);
						//    bool retValue = exportImagesPopupChildWindow.GetReply();

						//    if (retValue)
						//    {
						//        dc.ExportVisibleStacks();
						//    }
						//}

						dc.IsExportTestImages = false;

						exportImagesAsync(dc);

						//}
						break;
					}
				}
			}
			catch (Exception ex)
			{
				Window parentWindow = Window.GetWindow(this);
				ss.MSOTService.NotifyUserOnAction("Error trying to open MSP visualization selector: " + ex.Message, parentWindow.Title, UserNotificationType.Error, false);
			}
		}

		private void ExportTestImages_Click(object sender, RoutedEventArgs e)
		{
            ViewModelImagingSuperSession ss = this.DataContext as ViewModelImagingSuperSession;
            if (ss == null)
                return;

            try
			{
				
				foreach (ViewModelImagingSessionBase dc in ss.ImagingSessions)
				{
					if ((ss.VisualizationAndAnalysisType == VisualizationAndAnalysis.Snapshot) || ((ss.VisualizationAndAnalysisType == VisualizationAndAnalysis.Visualization) && dc.IsVisible))
					{                        
						dc.IsExportTestImages = true;
						exportImagesAsync(dc);                        
						break;
					}
				}
			}
			catch (Exception ex)
			{
				Window parentWindow = Window.GetWindow(this);
				ss.MSOTService.NotifyUserOnAction("Error trying to open MSP visualization selector: " + ex.Message, parentWindow.Title, UserNotificationType.Error, false);
			}
		}

		static async void exportImagesAsync(ViewModelImagingSessionBase dc)
		{
			dc.MSOTService.SystemBusy(true);
			Task t = Task.Run(() =>
			{
				dc.MSOTService.UIStaticDispatcher.Invoke(new Action(() =>
				{
					dc.PrepareExportedSelectedImage();
					dc.PrepareSavingFormat();
				}));
			});
			await t;
			dc.MSOTService.SystemBusy(false);
			ModalChildWindow.ShowDialog(
				dc.IsExportTestImages ?
					(dc.Is3DImagingSession ?
						"Export Test 3D Image(s)"
						: "Export Test Image(s)")
					: "Export images",
				new ViewMspExportAllImages(),
				dc);
		}

		private void saveAsMenuItem_Click(object sender, RoutedEventArgs e)
		{
			ViewModelImagingSuperSession session = DataContext as ViewModelImagingSuperSession;
		   
			DateTime currentDateTime = DateTime.Now;
			string suggestedName = "Comparison_" + currentDateTime.ToShortDateString() + "_" + currentDateTime.ToShortTimeString();            
			ModalChildWindow.ShowDialog(
				"Save comparison as",
				new ViewSaveAs("Comparison name", session.CommandSaveAsOpenedChanges, suggestedName),
				DataContext);
		}

		private void createReportMenuItem_Click(object sender, RoutedEventArgs e)
		{
            ViewModelImagingSuperSession session = DataContext as ViewModelImagingSuperSession;

            session.CommandGenerateReport.Execute(null);
            ModalChildWindow.ShowDialog("Creating report ...", new ViewReportGenerationProgressDialog(), DataContext);
            if (session.ReportDocument != null)
            {
                double parentWindowHeight = Window.GetWindow(this).ActualHeight;
                ModalChildWindow.ShowDialog("Report preview", new ViewXpsReport(), DataContext, new Size(parentWindowHeight * 0.9 * 0.70, parentWindowHeight * 0.9));
            }
		}
	}
}