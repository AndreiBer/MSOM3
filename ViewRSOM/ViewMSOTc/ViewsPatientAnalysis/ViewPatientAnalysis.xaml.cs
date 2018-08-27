using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xvue.MSOT.Services.Core;
using Xvue.MSOT.ViewModels.ProjectManager;
using Xvue.MSOT.ViewModels.ProjectManager.ImagingSession;

namespace ViewMSOTc
{
    /// <summary>
    /// Interaction logic for ViewPatientAnalysis.xaml
    /// </summary>
    public partial class ViewPatientAnalysis : UserControl
    {
        public ViewPatientAnalysis()
        {
            InitializeComponent();
        }

        public bool ShowSelectedSuperSession
        {
            get { return (bool)GetValue(ShowSelectedSuperSessionProperty); }
            set { SetValue(ShowSelectedSuperSessionProperty, value); }
        }

        public static readonly DependencyProperty ShowSelectedSuperSessionProperty =
            DependencyProperty.Register(
            "ShowSelectedSuperSession",
            typeof(bool),
            typeof(ViewPatientAnalysis),
            new FrameworkPropertyMetadata(
                new PropertyChangedCallback(ShowSelectedSuperSessionChanged)));

        static void ShowSelectedSuperSessionChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ViewPatientAnalysis control = source as ViewPatientAnalysis;
            ViewModelProjectManager dc = control.DataContext as ViewModelProjectManager;
            try
            {
                if ((bool)e.NewValue && dc != null && (dc?.ActiveStudy?.SelectedSuperSession.IsLoaded ?? false))
                    dc.ActiveStudy.OpenedSelectedSuperSession = dc.ActiveStudy.SelectedSuperSession;
            }
            finally
            {
                control.ShowSelectedSuperSession = false;
            }
        }

        void onSnapshotDoubleClick(object sender,EventArgs e)
        {
            loadSuperSession();
        }

        void onLoadSuperSessionButtonClick(object sender, RoutedEventArgs e)
        {
            loadSuperSession();
        }

        void loadSuperSession()
        {
            ViewModelProjectManager dc = DataContext as ViewModelProjectManager;
            if (dc == null || dc.ActiveStudy == null || superSessionStrip.SelectedSuperSession == null)
                return;

            if (!dc.ActiveStudy.IsSelectedSuperSessionOpened())
            {
                Dispatcher.BeginInvoke(new Action(() => {
                    try
                    {
                        ViewModelImagingSuperSession selected = superSessionStrip.SelectedSuperSession as ViewModelImagingSuperSession;
                        if ((dc.ActiveStudy.OpenedSelectedSuperSession as ViewModelImagingSuperSession)?.VerifySavingOfDataModelChanges(true) ?? true)
                        {
                            dc.ActiveStudy.SelectedSuperSession = selected;
                            dc.ActiveStudy.LoadSelectedSuperSession();
                        }
                    }
                    catch(Exception ex)
                    {
                        ViewMSOTcSystem.LogMessage(Xvue.MSOT.Services.Log.EnumLogType.Warning, "Selected SuperSession load", ex.Message);
                    }
                }));
            }
            else if(dc.ActiveStudy.SelectedSuperSession == null)
            {
                Window parentWindow = Window.GetWindow(this);
                dc.MSOTService.NotifyUserOnAction("Select a visualization to load", parentWindow.Title, UserNotificationType.Warning, false);
            }
        }

        void onNewComparisonSuperSessionButtonClick(object sender, RoutedEventArgs e)
        {
            ViewModelProjectManager dc = this.DataContext as ViewModelProjectManager;

            if (dc != null && dc.ActiveStudy != null && superSessionStrip.SelectedSuperSession != null)
            {
                dc.ActiveStudy.CreateNewAnalysisFromSelectedSnapshot();
                loadSuperSession();
            }
        }

        void userControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext != null && (bool)e.NewValue)
            {
                loadSuperSession();
            }
        }

        void userControl_Loaded(object sender, RoutedEventArgs e)
        {
            superSessionStrip.SnapshotDoubleClick += onSnapshotDoubleClick;
        }

        internal void HandlePreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (superSessionGridControl.IsVisible)
                superSessionGridControl.HandlePreviewKeyDown(sender, e);
        }
    }
}
