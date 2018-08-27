using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Xvue.Framework.Views.WPF.Controls;
using Xvue.MSOT.DataModels.Plugins.ProjectManager;
using Xvue.MSOT.ViewModels.ProjectManager;
using Xvue.MSOT.ViewModels.ProjectManager.ImagingSession;

namespace ViewMSOTc
{

    /// <summary>
    /// Interaction logic for ViewSnapshotsStrip.xaml
    /// </summary>
    public partial class ViewSnapshotsStrip : UserControl
    {
        CollectionViewSource _src;
        SortDescription _sortByDate = new SortDescription("CreationTime", ListSortDirection.Descending);

        public event EventHandler SnapshotDoubleClick;
        protected virtual void OnSnapshotDoubleClick()
        {
            SnapshotDoubleClick?.Invoke(this, EventArgs.Empty);
        }

        public ViewSnapshotsStrip()
        {
            InitializeComponent();
        }

        public bool IsAnalysis
        {
            get { return (bool)GetValue(IsAnalysisProperty); }
            set { SetValue(IsAnalysisProperty, (bool)value); }
        }
        public static readonly DependencyProperty IsAnalysisProperty =
            DependencyProperty.Register(
             "IsAnalysis",
             typeof(bool),
             typeof(ViewSnapshotsStrip),
             new FrameworkPropertyMetadata());

        public bool IsDragDropEnabled
        {
            get { return (bool)GetValue(IsDragDropEnabledProperty); }
            set { SetValue(IsDragDropEnabledProperty, (bool)value); }
        }
        public static readonly DependencyProperty IsDragDropEnabledProperty =
            DependencyProperty.Register(
             "IsDragDropEnabled",
             typeof(bool),
             typeof(ViewSnapshotsStrip),
             new FrameworkPropertyMetadata());

        public object SelectedSuperSession
        {
            get { return (object)GetValue(SelectedSuperSessionProperty); }
            set { SetValue(SelectedSuperSessionProperty, (object)value); }
        }
        public static readonly DependencyProperty SelectedSuperSessionProperty =
            DependencyProperty.Register(
             "SelectedSuperSession",
             typeof(object),
             typeof(ViewSnapshotsStrip),
             new FrameworkPropertyMetadata());

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.DataContext == null)
                return;

            if ((bool)e.NewValue)
            {
                if (SelectedSuperSession == null)
                    return;

                ViewModelImagingSuperSession selectedSession = SelectedSuperSession as ViewModelImagingSuperSession;

                if (!IsAnalysis)
                {
                    if (selectedSession.VisualizationAndAnalysisType != VisualizationAndAnalysis.Snapshot ||
                        !selectedSession.WasCreatedAfterLastStudyLoad)
                        SelectedSuperSession = null;

                    if (SelectedSuperSession == null && snapshotsListBox.HasItems)
                    {
                        foreach (ViewModelImagingSuperSession session in snapshotsListBox.Items)
                        {
                            if (session.VisualizationAndAnalysisType == VisualizationAndAnalysis.Snapshot &&
                                    session.WasCreatedAfterLastStudyLoad)
                            {
                                snapshotsListBox.SetCurrentValue(ListBox.SelectedItemProperty, session);
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _src = this.Resources["groupableSuperSessionSource"] as CollectionViewSource;
            refreshSuperSessionSource();
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            refreshSuperSessionSource();
        }

        private void refreshSuperSessionSource()
        {
            if (_src == null)
                return;

            using (_src.DeferRefresh())
            {
                _src.SortDescriptions.Clear();
            }
            using (_src.DeferRefresh())
            {
                _src.SortDescriptions.Add(_sortByDate);
            }
        }

        private void mainListBox_ManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }

        private void previewImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!_openPopupOnMouseUp)
                return;

            ListBoxItem item = sender as ListBoxItem;
            ViewModelImagingSuperSession ss = item.DataContext as ViewModelImagingSuperSession;

            if (ss.VisualizationAndAnalysisType == VisualizationAndAnalysis.Visualization)
                return;

            ViewModelImagingSessionBase dc = ss.ImagingSessions[0];

            openScanInformation(sender, dc);

            Task.Run(() =>
            {
                Task.Delay(System.Windows.Forms.SystemInformation.DoubleClickTime).Wait();
                Dispatcher.Invoke(new Action(() =>
                {
                    if (OAMScanInformationClosingPopup.IsOpen)
                    {
                        oamScanInformation.CaptureMouse();
                        oamScanInformation.ReleaseMouseCapture();
                    }
                }));
            });

            _openPopupOnMouseUp = false;
        }

        private void previewImage_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (_blockDoDragDrop)
                    return;

                ListBoxItem item = sender as ListBoxItem;

                if (e.LeftButton == MouseButtonState.Pressed && item.IsSelected && IsDragDropEnabled)
                {
                    if (OAMScanInformationClosingPopup.IsOpen == true)
                    {
                        closeScanInformation();
                        e.Handled = true;
                        return;
                    }

                    ViewModelImagingSuperSession ss = item.DataContext as ViewModelImagingSuperSession;

                    if (ss.VisualizationAndAnalysisType == VisualizationAndAnalysis.Visualization)
                        return;

                    DataObject data = new DataObject();
                    data.SetData("snapshotDragItem", ss);
                    DragDrop.DoDragDrop(item, data, DragDropEffects.Move);
                }
            }
            catch { }
        }

        private void previewImage_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            try
            {
                if (e.Effects.HasFlag(DragDropEffects.Move))
                    Mouse.SetCursor(Cursors.Hand);
                else
                    Mouse.SetCursor(Cursors.No);
            }
            catch { }

            e.Handled = true;
        }

        private void previewComparisonImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!_openPopupOnMouseUp)
                return;

            ListBoxItem item = sender as ListBoxItem;
            ViewModelImagingSessionBase dc = item.DataContext as ViewModelImagingSessionBase;
            if (dc.IsEmptyImagingSession)
            {
                _openPopupOnMouseUp = false;
                return;
            }

            openScanInformation(sender, dc);

            oamScanInformation.CaptureMouse();
            oamScanInformation.ReleaseMouseCapture();

            _openPopupOnMouseUp = false;
        }

        private void previewComparisonImage_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem item = sender as ListBoxItem;

            if (e.ChangedButton == MouseButton.Left)
            {
                try
                {
                    ViewModelImagingSessionBase _node = item.DataContext as ViewModelImagingSessionBase;
                    if (!_node.IsEmptyImagingSession)
                    {
                        if (_node.ClipboardPresetAvailable)
                        {
                            _node.CommandPasteImagingSettings.Execute(null);
                        }
                        else if (e.ClickCount == 1 && item.IsSelected)
                        {
                            if (OAMScanInformationClosingPopup.IsOpen)
                            {
                                closeScanInformation();
                            }
                            else
                            {
                                _openPopupOnMouseUp = true;
                            }
                        }
                    }
                }
                catch { }

            }
        }

        private void closeScanInformation()
        {
            setOAMScanInformationClosingPopupProperties(null, null, null);
            OAMScanInformationClosingPopup.SetCurrentValue(System.Windows.Controls.Primitives.Popup.IsOpenProperty, false);
        }

        private void setOAMScanInformationClosingPopupProperties(object controlToggleButton, object placementTarget, object dataContext)
        {
            OAMScanInformationClosingPopup.SetCurrentValue(ClosingPopup.ControlToggleButtonProperty, controlToggleButton);
            OAMScanInformationClosingPopup.SetCurrentValue(System.Windows.Controls.Primitives.Popup.PlacementTargetProperty, placementTarget);
            OAMScanInformationClosingPopup.SetCurrentValue(System.Windows.Controls.Primitives.Popup.DataContextProperty, dataContext);
        }

        private void openScanInformation(object sender, object dc)
        {
            setOAMScanInformationClosingPopupProperties(sender, sender, dc);
            OAMScanInformationClosingPopup.SetCurrentValue(System.Windows.Controls.Primitives.Popup.IsOpenProperty, true);

            Binding editBinding = new Binding("IsEditEnabled");
            editBinding.Source = dc;
            OAMScanInformationClosingPopup.SetBinding(ClosingPopup.StaysOpenProperty, editBinding);
        }

        private void OAMScanInformationClosingPopup_Closed(object sender, EventArgs e)
        {
            setOAMScanInformationClosingPopupProperties(null, null, null);
        }

        bool _openPopupOnMouseUp = false;
        bool _blockDoDragDrop = false;
        private void ListBoxItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem item = sender as ListBoxItem;

            if (e.ClickCount == 1 && item.IsSelected)
            {
                if (OAMScanInformationClosingPopup.IsOpen)
                {
                    closeScanInformation();
                }
                else
                {
                    _openPopupOnMouseUp = true;
                }
            }
            else if (e.ClickCount == 2)
            {
                _blockDoDragDrop = true;
                DispatcherOperation result = Dispatcher.BeginInvoke(new Action(() =>
                {
                    closeScanInformation();
                    OnSnapshotDoubleClick();
                }));
                result.Wait();
                Thread.Sleep(200);
                _blockDoDragDrop = false;
            }
        }

        private void ListBoxItem_TouchDown(object sender, TouchEventArgs e)
        {
            ListBoxItem item = sender as ListBoxItem;

            if (item.IsSelected)
            {
                if (OAMScanInformationClosingPopup.IsOpen)
                {
                    closeScanInformation();
                }
                else
                {
                    ViewModelImagingSuperSession ss = item.DataContext as ViewModelImagingSuperSession;

                    if (ss.VisualizationAndAnalysisType == VisualizationAndAnalysis.Visualization)
                        return;

                    ViewModelImagingSessionBase dc = ss.ImagingSessions[0];

                    openScanInformation(sender, dc);

                    oamScanInformation.CaptureMouse();
                    oamScanInformation.ReleaseMouseCapture();
                }
                e.Handled = true;
            }
        }

    }
}
