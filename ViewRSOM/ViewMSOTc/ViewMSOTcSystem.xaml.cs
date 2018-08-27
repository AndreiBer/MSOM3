using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.Primitives;
using Xvue.MSOT.Services.Core;
using Xvue.MSOT.Services.Log;
using Xvue.MSOT.ViewModels;
using Xvue.MSOT.ViewModels.Base;

namespace ViewMSOTc
{

    internal static class NativeMethods
    {
        [DllImport("user32")]
        public static extern bool ExitWindowsEx(uint uFlags, uint dwReason);
    }

    static class ViewSystemHelper
    {
        public static Cursor LoadCursorResource(string cursorUri)
        {
            Cursor paintCursor = null;

            var streamResourceInfo = Application.GetResourceStream(new Uri(cursorUri));
            using (var stream = streamResourceInfo.Stream)
            {
                paintCursor = new Cursor(stream);
            }

            return paintCursor;
        }

        public static int AllScreensLength
        {
            get
            {
                return System.Windows.Forms.Screen.AllScreens.Length;
            }
        }

        public static string PrimaryScreenDeviceName
        {
            get
            {
                return System.Windows.Forms.Screen.PrimaryScreen.DeviceName;
            }
        }

        public static string ScreenDeviceName(Window window)
        {
            var parentArea = new System.Drawing.Rectangle((int)window.Left, (int)window.Top, (int)window.Width, (int)window.Height);
            System.Windows.Forms.Screen windowScreen = System.Windows.Forms.Screen.FromRectangle(parentArea);
            return windowScreen != null ? windowScreen.DeviceName : "";
        }

        public static void SetWindowToSecondaryScreen(Window window, WindowState windowState)
        {
            System.Windows.Forms.Screen secondary = null;
            foreach (System.Windows.Forms.Screen screenItem in System.Windows.Forms.Screen.AllScreens)
            {
                if (screenItem.DeviceName != System.Windows.Forms.Screen.PrimaryScreen.DeviceName)
                {
                    secondary = screenItem; // consider secondary the next screen after Primary
                    break;
                }
            }

            if (secondary != null)
            {
                window.WindowState = windowState;
                window.Left = secondary.WorkingArea.Left;
                window.Top = secondary.WorkingArea.Top;
                window.Width = secondary.WorkingArea.Width;
                window.Height = secondary.WorkingArea.Height;
            }
        }

        public static void TrySwitchToSecondaryScreen(Window window, WindowState windowState)
        {
            // check if we have multiple screens and uf we are currently on primary screen
            if (AllScreensLength > 0 && ScreenDeviceName(window) == PrimaryScreenDeviceName)
            {
                SetWindowToSecondaryScreen(window, windowState);
            }
            //else ..... define some custom logic in case multiple screens exist in the system
        }

        internal static Geometry GetClipCombine(Rect innerRect, Rect fullRect)
        {
            Geometry innerGeometry = new RectangleGeometry(innerRect);
            Geometry fullGeometry = new RectangleGeometry(fullRect);

            return Geometry.Combine(fullGeometry, innerGeometry, GeometryCombineMode.Exclude, null);
        }

        internal static Geometry GetClipCombine(FrameworkElement parentElement, FrameworkElement modalElement)
        {
            Point windowTopLeft = parentElement.PointToScreen(new Point(0, 0));
            Point elementTopLeft = modalElement.PointToScreen(new Point(0, 0));

            Point modalDialogTop = new Point(elementTopLeft.X - windowTopLeft.X, elementTopLeft.Y - windowTopLeft.Y);
            Point modalDialogBottom = modalDialogTop + new Vector(modalElement.ActualWidth, modalElement.ActualHeight);

            return GetClipCombine(new Rect(modalDialogTop, modalDialogBottom), new Rect(new Point(0, 0), new Point(parentElement.ActualWidth, parentElement.ActualHeight)));
        }

        internal static void CloseAllWindows(bool closeMain)
        {
            Window mainWindow = null;
            foreach (Window window in Application.Current.Windows)
            {
                if (window == Application.Current.MainWindow)
                    mainWindow = window;
                else
                    window.Close();
            }
            if (closeMain)
                mainWindow?.Close();
        }

        internal static void HandPlay()
        {
            System.Media.SystemSounds.Hand.Play();
        }

        internal static void BeepPlay()
        {
            System.Media.SystemSounds.Beep.Play();
        }
    }

    /// <summary>
    /// Interaction logic for ViewMSOTc.xaml
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = ViewModelMSOTPluginBase.ComplexityJustificationMessage)]
    public partial class ViewMSOTcSystem : Window, IDisposable
    {
        #region localvariable
        MSOTSystem _system; 
        ViewMaintenance _maintenanceDlg;
        List<ToggleButton> _tabHeaders;

        bool _debuggerAttached;
        Cursor _paintCursor;
        FrameworkElement _modalElement;
        List<NotifyUserEventArgs> _notificationsStack = new List<NotifyUserEventArgs>();
        static IMSOTLog _log;
        private bool _pendingPushedMessagesHandled;
        #endregion localvariable

        internal static WindowContainer ChildWindowContainer;

        public ViewMSOTcSystem()
        {
            InitializeComponent();
            _tabHeaders = new List<ToggleButton>();
            _tabHeaders.Add(overviewToggleButton);
            _tabHeaders.Add(acquisitionToggleButton);
            _tabHeaders.Add(analysisToggleButton);            
            UpdateTabButtons(overviewToggleButton, true);
            
            refreshViewEvents();

            if (System.Diagnostics.Debugger.IsAttached)
            {
                _debuggerAttached = true;
                WindowStyle = System.Windows.WindowStyle.SingleBorderWindow;
            }
            ViewMSOT.UIControls.DllEntryPoint.ModalWindow = new ViewMSOT.UIControls.ModalWindowCallback(ModalChildWindow.ShowDialog);
            ViewMSOT.UIControls.DllEntryPoint.MessageLog = new ViewMSOT.UIControls.MessageLogCallback(LogMessage);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!_system.IsSystemExited)
            {
                e.Cancel = true;
                _system.TryExitSystem(SystemExitReason.PowerOff);
            }
        }

        private void OnSystemExited(object sender, EventArgs e)
        {
            ViewSystemHelper.CloseAllWindows(true);
            Dispose();
        }

        private void maintenanceBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_maintenanceDlg == null)
            {
                _maintenanceDlg = new ViewMaintenance();
                _maintenanceDlg.DataContext = base.DataContext;
            }
            else
            {
                if (!_maintenanceDlg.IsLoaded)
                {
                    _maintenanceDlg = new ViewMaintenance();
                    _maintenanceDlg.DataContext = base.DataContext;
                }
            }
            //bsar: maintenance is a Dialog or not?
            _maintenanceDlg.Show();
            _maintenanceDlg.Activate();            
        }

        /// <summary>
        /// Start System and load all basic modules.
        /// </summary>        
        public void StartMSOT(WindowContainer tempWindowContainer)
        {
            try
            {
                ChildWindowContainer = tempWindowContainer;

                _paintCursor = ViewSystemHelper.LoadCursorResource("pack://application:,,,/ViewMSOT.UIControls;component/GFX/Paint.cur");
                _system = new MSOTSystem();
                _system.PaintCursor = _paintCursor;
                _system.Notify += systemPushNotifications;
                _system.Verify += system_Verify;

                _system.InitializePlugin("MSOT System", null);
                if (_system.HasSystemConfigWriteAccess)
                    _system.ExtractAll("");
                _system.CommandLoad.Execute(null);

                _system.AddEmptyImagingSessionAtAnalysis = true;
                _system.EnablePreviewVisibleImagesRepeatMode = true;
                _system.AddSnapshotImagingSuperSessionAtVisualization = true;

                _log = _system.IMSOTLog;       

                string cfgFile = "";
                if (_system.IsUsingAdvancedGui)
                    cfgFile = " - " + System.IO.Path.GetFileNameWithoutExtension(_system.SettingsFilePath);
                this.Title = _system.MsotApplicationInfo.ProductName;
                this.TaskbarItemInfo.Description = this.Title + cfgFile;
                this.DataContext = _system;

                if (_system.IsSystemSuccessfullyLoaded())
                {
                    _pendingPushedMessagesHandled = true;
                    loadSystemOnStartUp();
                }

                ChildWindowContainer = childWindowContainer;
            }
            catch (Exception ex)
            {
                NotifyUserOnError("Unable to load " + this.Title + "\nException: " + ex.Message, Title, UserNotificationType.Error, false);
                //System.Windows.MessageBox.Show(this,"Unable to load "+this.Title + "\nException: "+ex.Message);
            }
            finally
            {
                _system.SystemExit += OnSystemExited;
                _system.Notify -= systemPushNotifications;
                _system.Notify += system_Notify;
            }            
        }

        /// <summary>
        /// Shows the pending pushed messages if system has not gone online
        /// in which case pending pushed notifications need to be shown after 
        /// the patients popup.
        /// </summary>
        internal void ShowPendingPushedMessages()
        {
            if (!_pendingPushedMessagesHandled)
                showPushedNotifications();
        }

        void system_Notify(object sender, NotifyUserEventArgs notifyMessage)
        {
            NotifyUserOnError(notifyMessage.Message, notifyMessage.Caption, notifyMessage.NotificationType, notifyMessage.DoNotBlock);
        }

        public void NotifyUserOnError(string message, string caption, UserNotificationType notificationType, bool doNotBlock)
        {
            if(notificationType == UserNotificationType.ReportIssue)
            {
                ViewSystemHelper.HandPlay();
                if (!string.IsNullOrEmpty(message))
                    _system.IMSOTLog.HandleError(caption, message);
                _system.CommandCreateIssueReporter.Execute(message);
                mainIssueReportChildWindow.Caption = caption;
                mainIssueReportChildWindow.IsModal = !doNotBlock;
                mainIssueReportChildWindow.WindowState = Xceed.Wpf.Toolkit.WindowState.Open;
            }
            else
            {
                MessageBoxChildWindow messageBoxNotify = new MessageBoxChildWindow(message, caption, notificationType, doNotBlock);
                ViewSystemHelper.BeepPlay();

                messageBoxNotify.ShowMessage();
            }
        }

        bool? system_Verify(object sender, VerifyUserEventArgs verifyMessage)
        {
            verifyXvueMessageBox.Setup(verifyMessage.Message, verifyMessage.YesChoiceCaption, verifyMessage.NoChoiceCaption, verifyMessage.HasCancel);
            return verifyWindow.ShowMessage(verifyMessage.Caption);
        }

        /// <summary>
        /// Logs the message.
        /// </summary>
        /// <param name="logType">Type of the log.</param>
        /// <param name="messageHeader">The message header.</param>
        /// <param name="messageReason">The message reason.</param>
        public static void LogMessage(EnumLogType logType, string messageHeader, string messageReason)
        {
            if (_log != null)
            {
                _log.HandleError(logType, messageHeader, messageReason);
            }
            else
            {
                DateTime now = new DateTime(DateTime.Now.Ticks);
                string message = now.ToString("o") + "; [" + messageHeader + "]; " + messageReason;
                switch (logType)
                {
                    case EnumLogType.Critical:
                        System.Diagnostics.Trace.TraceError(message);
                        break;
                    case EnumLogType.Warning:
                        System.Diagnostics.Trace.TraceWarning(message);
                        break;
                    case EnumLogType.Info:
                    default:
                        System.Diagnostics.Trace.TraceInformation(message);
                        break;
                }
            }
        }

        void showPushedNotifications()
        {
            try
            {
                foreach (NotifyUserEventArgs notify in _notificationsStack)
                {
                    system_Notify(null, notify);
                }
                _notificationsStack.Clear();
            }
            catch (Exception ex)
            {
                LogMessage(EnumLogType.Warning, "ViewMSOTcSystem:showPushedNotifications()", "Exception showing notifications during system loading" + ex.Message);
            }
        }

        void systemPushNotifications(object sender, NotifyUserEventArgs notifyMessage)
        {
            _notificationsStack.Add(notifyMessage);
        }

        void loadSystemOnStartUp()
        {
            System.Threading.Thread _goOnlineThread = new System.Threading.Thread(goOnline);
            _goOnlineThread.Name = "Hardware go online thread";
            _goOnlineThread.Start();
        }

        void goOnline()
        {
            try
            {
                if (_system.TryGetHardwareOnline())
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        openPatientPopup(null, EventArgs.Empty);
                        _system.ReportLastSystemCrash();
                    }));
                }
                Dispatcher.Invoke(new Action(() =>
                {
                    showPushedNotifications();
                }));
            }
            catch (Exception ex)
            {
                LogMessage(EnumLogType.Critical, "Exception going online", ex.Message);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (_debuggerAttached)
            {
                //try to make application full screen in secondary monitor
                ViewSystemHelper.TrySwitchToSecondaryScreen(this, WindowState.Normal);
            }
            patientOverviewControl.SwitchToAnalysis += patientOverviewControl_SwitchToAnalysis;
            acquisitionControl.snapshotsStrip.SnapshotDoubleClick += patientOverviewControl_SwitchToAnalysis;
        }

        void patientOverviewControl_SwitchToAnalysis(object sender,EventArgs e)
        {
            if (analysisToggleButton.Command.CanExecute(null) && analysisToggleButton.IsEnabled)
            {
                analysisToggleButton.SetCurrentValue(ToggleButton.IsCheckedProperty, true);
            }
        }

        private void openPatientPopup(object sender,EventArgs e)
        {
            patientPopupChildWindow.WindowState = Xceed.Wpf.Toolkit.WindowState.Open;
        }

        private void showAcquisition()
        {
            patientOverviewControl.Visibility = System.Windows.Visibility.Collapsed;
            patientAnalysisControl.Visibility = System.Windows.Visibility.Collapsed;
            acquisitionControl.Visibility = System.Windows.Visibility.Visible;
        }

        private void showOverview()
        {
            patientOverviewControl.Visibility = System.Windows.Visibility.Visible;
            patientAnalysisControl.Visibility = System.Windows.Visibility.Collapsed;
            acquisitionControl.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void showOverviewAndClosePopup(object sender, SelectOverviewEventArgs e)
        {
            e.Canceled = !verifySaveAnalysisSession(true);
            if (!e.Canceled)
                UpdateTabButtons(overviewToggleButton, true);
        }

        private void showAnalysis()
        {
            patientOverviewControl.Visibility = System.Windows.Visibility.Collapsed;
            patientAnalysisControl.Visibility = System.Windows.Visibility.Visible;
            acquisitionControl.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void refreshViewEvents()
        {
            patientSelectionInfoControl.SelectPatient += openPatientPopup;
            patientPopup.SelectOverview += showOverviewAndClosePopup;
        }

        private void initiateNewAcquisition()
        {
            try
            {
                _system.ProjectManager.InitiateActiveStudyNewScan();
                _system.MSOTHardware.ExperimentManualScan.CommandScanSessionSwitchover.Execute(null);
            }
            catch { }

            showAcquisition();
        }

        bool verifySaveAnalysisSession(bool closeSession)
        {
            bool continueF = false;
            try
            {
                if (analysisToggleButton.IsChecked ?? false)
                {
                    continueF = _system.ProjectManager.VerifySaveOpenedSelectedSuperSession(closeSession);
                }
                else
                {
                    continueF = true;
                }
            }
            catch (Exception ex)
            {
                LogMessage(EnumLogType.Warning, "checkCloseAnalysisSession exception: ", ex.Message);
            }

            return continueF;
        }

        void UpdateTabButtons(ToggleButton selected, bool checkTabButton=false )
        {
            foreach(ToggleButton button in _tabHeaders)
            {
                if (button == selected)
                {
                    if (checkTabButton)
                    {
                        button.IsChecked = true;
                    }
                }
                else
                {
                    button.IsChecked = false;
                }
            }
        }

        private void overviewToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            bool canceled = !verifySaveAnalysisSession(true);
            if (canceled)
            {
                overviewToggleButton.IsChecked = false;
                return;
            }

            _system?.MSOTHardware.ExperimentManualScan.CommandUnload.Execute(null);

            showOverview();
            UpdateTabButtons(overviewToggleButton);
        }

        private void acquisitionToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            bool continueF = false;
            try
            {
                continueF = verifySaveAnalysisSession(false);   // Don't close the session yet.
                if (continueF)
                {
                    bool? userReply = ModalChildWindow.ShowDialog("Select preset", new ViewOAMPresetsPopup(), DataContext, ViewOAMPresetsPopup.UserReplyProperty);
                    continueF = userReply == true ? true : false;
                }
                if (continueF)
                {
                    bool? userReply = ModalChildWindow.ShowDialog("LASER SAFETY WARNING", new ViewOAMSafetyGlassesDialog(), _system, ViewOAMSafetyGlassesDialog.SafetyOKProperty);
                    continueF = userReply == true ? true : false;
                }
                if (continueF)
                {
                    if (_system.MSOTHardware.HardwareSelfTest.TestRequired)
                    {
                        if (_system.MSOTHardware.HardwareSelfTest.AcceptInitialize())
                        {
                            ModalChildWindow.ShowDialog("Self-test", new ViewSelfTest(), _system.MSOTHardware.HardwareSelfTest);
                            continueF = !_system.MSOTHardware.HardwareSelfTest.UserAborted;
                        }
                        else
                        {
                            continueF = false;
                        }
                        if (!continueF)
                            _system.MSOTHardware.Laser.AcceptStopCharger();
                    }
                }
                if (continueF)
                {
                    _system.ProjectManager.CloseOpenedSelectedSuperSession();    // Close session here as it cannot be determined before, i.e. in the call to verifySaveAnalysisSession above.
                    initiateNewAcquisition();
                    UpdateTabButtons(acquisitionToggleButton);
                }
                else
                {
                    acquisitionToggleButton.IsChecked = false;
                }
            }
            catch
            {
                acquisitionToggleButton.IsChecked = false;
            }
        }

        private void analysisToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            int timeoutCounter = 0;
            if (_system.MSOTHardware.ExperimentManualScan.IsLoaded)
            {
                while (!_system.MSOTHardware.ExperimentManualScan.CanAcceptClose())
                {
                    System.Threading.Thread.Sleep(100);
                    timeoutCounter += 100;
                    if (timeoutCounter > 3000)
                    {
                        LogMessage(EnumLogType.Critical, "ViewMSOTcSystem", "Timeout closing experiment, State: " + _system.MSOTHardware.ExperimentManualScan.CurrentState);
                        return;
                    }
                    else
                    {
                        LogMessage(EnumLogType.Warning, "ViewMSOTcSystem", "Unable to close experiment after " + timeoutCounter + "ms, State: " + _system.MSOTHardware.ExperimentManualScan.CurrentState);
                    }
                }
                _system.MSOTHardware.ExperimentManualScan.CommandUnload.Execute(null);
            }

            showAnalysis();
            UpdateTabButtons(analysisToggleButton);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (_system.SystemExitTypeRequested == SystemExitReason.LogOff)
            {
                if (_debuggerAttached)
                {
                    LogMessage(EnumLogType.Info, "Logging off", "*******************************************************");
                    LogMessage(EnumLogType.Info, "Logging off", "I could have logged off Windows, but will not, for now.");
                    LogMessage(EnumLogType.Info, "Logging off", "*******************************************************");
                }
                else
                {
                    NativeMethods.ExitWindowsEx(0, 0);
                }
            }
            else if (_system.SystemExitTypeRequested == SystemExitReason.PowerOff)
            {
                if (_debuggerAttached)
                {
                    LogMessage(EnumLogType.Info, "Shutting down", "*******************************************************");
                    LogMessage(EnumLogType.Info, "Shutting down", "I could have shut down system, but will not, for now.");
                    LogMessage(EnumLogType.Info, "Shutting down", "*******************************************************");
                }
                else
                {
                    // starts the shutdown application 
                    // the argument /s is to shut down the computer
                    // the argument /t 0 is to tell the process that 
                    // the specified operation needs to be completed 
                    // after 0 seconds
                    System.Diagnostics.Process.Start("shutdown", "/s /t 5");
                }
            }
        }

        private void aboutButton_Click(object sender, RoutedEventArgs e)
        {
            aboutPopupChildWindow.WindowState = Xceed.Wpf.Toolkit.WindowState.Open;
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (acquisitionControl.IsVisible)
            {
                acquisitionControl.HandlePreviewKeyDown(sender, e);
            }
            else if (patientAnalysisControl.IsVisible)
            {
                patientAnalysisControl.HandlePreviewKeyDown(sender, e);
            }
        }

        public void ToggleModalLayer(FrameworkElement element)
        {
            if (element != null)
            {
                _modalElement = element;
                modalDialogBorder.Visibility = Visibility.Visible;
                reCalculateModalOverlayLayer();
            }
            else
            {
                modalDialogBorder.Visibility = Visibility.Hidden;
            }            
        }

        void reCalculateModalOverlayLayer()
        {
            if (modalDialogBorder.Visibility == Visibility.Visible)
            {
                modalDialogBorder.Clip = ViewSystemHelper.GetClipCombine(this, _modalElement);
            }
        }

        private void window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            reCalculateModalOverlayLayer();
        }

        public static void ChangeModalAndEditState(bool isEditable, FrameworkElement element)
        {
            if (isEditable)
            {
                Window parentWindow = Window.GetWindow(element);
                (parentWindow as ViewMSOTcSystem).ToggleModalLayer(element);
            }
            else
            {
                Window parentWindow = Window.GetWindow(element);
                (parentWindow as ViewMSOTcSystem).ToggleModalLayer(null);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ViewMSOTcSystem()
        {
            Dispose(false);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "_system", Justification = "CA cannot recognize the recommended Dispose pattern")]
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _paintCursor?.Dispose();
                _system?.Dispose();
            }
            // free native resources if there are any here
        }
    }
}
