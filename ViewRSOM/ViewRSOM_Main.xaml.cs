using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using System.Threading;
using System.Xml;
using ViewRSOM.MSOT.Hardware.ViewModels.Laser;
using Laser.OpoData;


namespace ViewRSOM
{
    /// <summary>
    /// Interaction logic for ViewRSOM_Main.xaml
    /// </summary>
    public partial class ViewRSOM_Main : Window
    {

        // define redirected console stream
        public TextWriter _writer = null;
        //public bool OPO_connected = false;

        // camera object
        Hardware.BaslerCamera.BaslerCamera myUSBcamera;
        // OPO object
        HandleOpoData myOpoDataHandler = new HandleOpoData();
        public ViewModelLaserInnolas my_laser = null; 
        // define culture 
        CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
        NumberStyles styles = NumberStyles.AllowExponent | NumberStyles.Number;

        public ViewRSOM_Main()
        {
            my_laser = new ViewModelLaserInnolas();
            InitializeComponent();
            patientPopup.CancelPopupEvent += new EventHandler(MyEventHandlerFunction_CancelPopupEvent);
            patientPopup.SelectPopupEvent += new EventHandler(MyEventHandlerFunction_SelectPopupEvent);

            // define redirected console stream
            _writer = new ConsoleStream.IOstreamListener();
            Console.SetOut(_writer);

            // Error and warning stream
            ConsoleStream.IOEventHandler.myIOEvent += new ConsoleStream.CharEventHandler(writeInGUI);

            // GUI button control
            systemState.SystemStateChanged += new SystemStateChangedEventHandler(GUIbuttonOrCameraState);

            // acq finished
            ConsoleStream.IOEventHandler.myStatusAcq += new ConsoleStream.AcqStatusEventHandler(acqFinished);

            // recon finished
            ConsoleStream.IOEventHandler.myReconFinished += new ConsoleStream.ReconFinishedEventHandler(reconFinished);

            //unmix finished
            ConsoleStream.IOEventHandler.myUnmixFinished += new ConsoleStream.UnmixFinishedEventHandler(unmixFinished);

            // show patient popup on startup
            patientPopupChildWindow.Visibility = Visibility.Visible;
            
            //update DiskStatus
            updateDiskStatus();

            // initialize camera
            myUSBcamera = new Hardware.BaslerCamera.BaslerCamera();
            // bool OPO_connected = my_laser.connectOPO();

        }

        #region window_events
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            
        }

        private void window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
        #endregion window_events


        #region control_events
        private void overviewToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            analysisToggleButton.IsChecked = false;
            acquisitionToggleButton.IsChecked = false;
            UnmixingToggleButton.IsChecked = false;

            // update tab visibilities
            patientOverviewControl.Visibility = Visibility.Visible;
            ScanningControl.Visibility = Visibility.Collapsed;
            ReconstructionControl.Visibility = Visibility.Collapsed;
            UnmixingControl.Visibility = Visibility.Collapsed;

            // update camera
            myUSBcamera.cameraRecord = false;

        }
        private void acquisitionToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            overviewToggleButton.IsChecked = false;
            analysisToggleButton.IsChecked = false;
            UnmixingToggleButton.IsChecked = false;

            // ScanningControl.acq_MessageBox.Text = "";
            ScanningControl.pulseEnergy_TextBox.Text = Convert.ToString(acquisitionParameters.laserPower);

            // update tab visibilities
            patientOverviewControl.Visibility = Visibility.Collapsed;
            ScanningControl.Visibility = Visibility.Visible;
            ReconstructionControl.Visibility = Visibility.Collapsed;
            UnmixingControl.Visibility = Visibility.Collapsed;

            // update camera
            myUSBcamera.GrabImages();
        }

        private void analysisToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            overviewToggleButton.IsChecked = false;
            acquisitionToggleButton.IsChecked = false;
            UnmixingToggleButton.IsChecked = false;

            // ReconstructionControl.recon_MessageBox.Text = "";

            // update tab visibilities
            patientOverviewControl.Visibility = Visibility.Collapsed;
            ScanningControl.Visibility = Visibility.Collapsed;
            ReconstructionControl.Visibility = Visibility.Visible;
            UnmixingControl.Visibility = Visibility.Collapsed;


            // update camera
            myUSBcamera.cameraRecord = false;
        }

        private void UnmixingToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            overviewToggleButton.IsChecked = false;
            acquisitionToggleButton.IsChecked = false;
            analysisToggleButton.IsChecked = false;

            //UnmixingToggleButton.IsChecked = true;
            // ReconstructionControl.recon_MessageBox.Text = "";

            // update tab visibilities
            patientOverviewControl.Visibility = Visibility.Collapsed;
            ScanningControl.Visibility = Visibility.Collapsed;
            ReconstructionControl.Visibility = Visibility.Collapsed;
            UnmixingControl.Visibility = Visibility.Visible;

            // update camera
            myUSBcamera.cameraRecord = false;
        }
        #endregion control_events


        #region button_events
        private void aboutButton_Click(object sender, RoutedEventArgs e)
        {
            aboutPopupChildWindow.Visibility = Visibility.Visible;           
        }

        private void maintenanceBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void selectPatientButtonClick(object sender, RoutedEventArgs e)
        {
            if (systemState.reconThreadFree && systemState.acqThreadFree)
            {
                patientPopupChildWindow.Visibility = Visibility.Visible;
                patientPopup.selectPatient(studyParameters.ActualPatientListIndex);
            }
            else
            {
                string messageBoxText = "Please wait untill acquisition or reconstruction is finished before you change the study.";
                string caption = "Change study";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBox.Show(messageBoxText, caption, button, icon);
            }
        }

        public void MyEventHandlerFunction_CancelPopupEvent(object sender, EventArgs e)
        {
            patientPopupChildWindow.Visibility = Visibility.Hidden;
            actualPatientfullNameLabel.Content = "" + studyParameters.ActualPatientLastName;
            //actualPatientbirthDateLabel.Content = "" + studyParameters.ActualPatientBirthDate.ToString("dd.MM.yyyy");           
            actualPatientidLabel.Content = "" + studyParameters.ActualPatientId;
            
        }

        public void MyEventHandlerFunction_SelectPopupEvent(object sender, EventArgs e)
        {
            patientPopupChildWindow.Visibility = Visibility.Hidden;
            actualPatientfullNameLabel.Content = "" + studyParameters.ActualPatientLastName;
            //actualPatientbirthDateLabel.Content = "" + studyParameters.ActualPatientBirthDate.ToString("dd.MM.yyyy");           
            actualPatientidLabel.Content = "" + studyParameters.ActualPatientId;
            
            if (fileParameters.studyFolder !=null)
                update_scanList();
        }
        #endregion


        #region event_handlers_IOstream
        // IOEventHandler functions 
        private void writeInGUI(string sender, string receiver, string value)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(
                System.Windows.Threading.DispatcherPriority.Normal, (Action)delegate
                {
                    if (!systemState.acqThreadFree)
                    {
                        ScanningControl.acq_MessageBox.AppendText(value);
                        ScanningControl.acq_MessageBox.ScrollToEnd();
                    }
                    if (!systemState.reconThreadFree)
                    {
                        ReconstructionControl.recon_MessageBox.AppendText(value);
                        ReconstructionControl.recon_MessageBox.ScrollToEnd();
                    }
                    if (!systemState.unmixThreadFree)
                    {
                        UnmixingControl.unmix_MessageBox.AppendText(value);
                        UnmixingControl.unmix_MessageBox.ScrollToEnd();
                    }
                });
        }

        private void GUIbuttonOrCameraState(string value)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(
                System.Windows.Threading.DispatcherPriority.Normal, (Action)delegate
                {
                    if (value.Equals("acqThreadFree"))
                    {
                        if (systemState.acqThreadFree)
                        {
                            // enable acquisition buttons
                            ScanningControl.quickScan_Button.IsEnabled = true;
                            ScanningControl.fullScan_Button.IsEnabled = true;
                            ScanningControl.fullScan_Button.Visibility = Visibility.Visible;
                            ScanningControl.cancelAcq_Button.Visibility = Visibility.Collapsed;
                            // enable recon buttons
                            ReconstructionControl.recon_Button.IsEnabled = true;
                            ReconstructionControl.export_Button.IsEnabled = true;

                            // enable unmix buttons
                            UnmixingControl.unmix_Button.IsEnabled = true;
                            UnmixingControl.export_Button.IsEnabled = true;

                            // notify user by sound
                            System.Media.SystemSounds.Hand.Play();
                        }
                        else
                        {
                            // disable acquisition buttons
                            ScanningControl.quickScan_Button.IsEnabled = false;
                            ScanningControl.fullScan_Button.IsEnabled = false;
                            ScanningControl.fullScan_Button.Visibility = Visibility.Collapsed;
                            ScanningControl.cancelAcq_Button.Visibility = Visibility.Visible;
                            // disable recon buttons
                            ReconstructionControl.recon_Button.IsEnabled = false;
                            ReconstructionControl.export_Button.IsEnabled = false;
                            // disable unmix buttons
                            UnmixingControl.unmix_Button.IsEnabled = false;
                            UnmixingControl.export_Button.IsEnabled = false;
                        }
                    }

                    if (value.Equals("reconThreadFree"))
                    {
                        if (systemState.reconThreadFree)
                        {
                            // enable acquisition buttons
                            ScanningControl.quickScan_Button.IsEnabled = true;
                            ScanningControl.fullScan_Button.IsEnabled = true;
                            // enable recon buttons
                            ReconstructionControl.recon_Button.IsEnabled = true;
                            ReconstructionControl.cancelRecon_Button.Visibility = Visibility.Collapsed;
                            ReconstructionControl.export_Button.IsEnabled = true;
                            // enable unmix buttons
                            UnmixingControl.unmix_Button.IsEnabled = true;
                            UnmixingControl.export_Button.IsEnabled = true;
                            UnmixingControl.cancelUnmix_Button.Visibility = Visibility.Collapsed;

                            // free cancel button event
                            if (systemState.reconHandle != null)
                            {
                                ReconstructionControl.cancelRecon_Button.Click -= systemState.reconHandle;
                            }

                            // update camera
                            if (acquisitionToggleButton.IsChecked == true)
                            {
                                //myUSBcamera.GrabImages();
                            }

                        }
                        else
                        {
                            // disable acquisition buttons
                            ScanningControl.quickScan_Button.IsEnabled = false;
                            ScanningControl.fullScan_Button.IsEnabled = false;
                            // disable recon buttons
                            ReconstructionControl.recon_Button.IsEnabled = false;
                            ReconstructionControl.cancelRecon_Button.Visibility = Visibility.Visible;
                            ReconstructionControl.export_Button.IsEnabled = false;
                            // disable unmix buttons
                            UnmixingControl.unmix_Button.IsEnabled = false;
                            UnmixingControl.export_Button.IsEnabled = false;
                            UnmixingControl.cancelUnmix_Button.Visibility = Visibility.Collapsed;
                        }
                    }

                    if (value.Equals("unmixThreadFree"))
                    {
                        if (systemState.unmixThreadFree)
                        {
                            // enable acquisition buttons
                            ScanningControl.quickScan_Button.IsEnabled = true;
                            ScanningControl.fullScan_Button.IsEnabled = true;
                            ScanningControl.fullScan_Button.Visibility = Visibility.Visible;
                            ScanningControl.cancelAcq_Button.Visibility = Visibility.Collapsed;
                            // enable recon buttons
                            ReconstructionControl.recon_Button.IsEnabled = true;
                            ReconstructionControl.export_Button.IsEnabled = true;
                            ReconstructionControl.cancelRecon_Button.Visibility = Visibility.Collapsed;

                            // enable unmix buttons
                            UnmixingControl.unmix_Button.IsEnabled = true;
                            UnmixingControl.cancelUnmix_Button.Visibility = Visibility.Collapsed;
                            UnmixingControl.export_Button.IsEnabled = true;

                            // free cancel button event
                            UnmixingControl.cancelUnmix_Button.Click -= systemState.unmixHandle;

                            // update camera
                            if (acquisitionToggleButton.IsChecked == true)
                            {
                                //myUSBcamera.GrabImages();
                            }

                        }
                        else
                        {
                            // disable acquisition buttons
                            ScanningControl.quickScan_Button.IsEnabled = false;
                            ScanningControl.fullScan_Button.IsEnabled = false;
                            // disable recon buttons
                            ReconstructionControl.recon_Button.IsEnabled = false;
                            ReconstructionControl.export_Button.IsEnabled = false;
                            // disable unmix buttons
                            UnmixingControl.unmix_Button.IsEnabled = false;
                            UnmixingControl.export_Button.IsEnabled = false;
                            UnmixingControl.cancelUnmix_Button.Visibility = Visibility.Visible;
                        }
                    }

                    if (value.Equals("currentCameraImage"))
                    {
                        ScanningControl.myCameraImage.Source = systemState.currentCameraImage;
                    }

                });
        }


        // acq finished
        private void acqFinished(string sender, string receiver, string value)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(
                System.Windows.Threading.DispatcherPriority.Normal, (Action)delegate
                {
                    bool canParse;
                    double doubleParse;
                    canParse = Double.TryParse(value.Substring(12), styles, culture, out doubleParse);
                    if (canParse)
                    {
                        doubleParse = doubleParse * 100;
                        if (doubleParse > 99)
                        {
                            // update study list
                            update_scanList();
                            // Notify user in acq
                            ScanningControl.acq_ProgressBar.Foreground = Brushes.MediumBlue;
                            ScanningControl.acq_MessageBox.AppendText("\n");
                            ScanningControl.acq_MessageBox.AppendText("Acquisition finished.");
                            // re-enable acquisition buttons
                            // ScanningControl.quickScan_Button.IsEnabled = true;
                            // ScanningControl.fullScan_Button.IsEnabled = true;
                            Thread.Sleep(500);
                            systemState.acqThreadFree = true;
                            // re-enable recon buttons
                            ReconstructionControl.recon_Button.IsEnabled = true;
                            ReconstructionControl.export_Button.IsEnabled = true;
                            // re-enable unmix buttons
                            UnmixingControl.unmix_Button.IsEnabled = true;
                            UnmixingControl.export_Button.IsEnabled = true;
                        }
                    }
                });
        }

        // recon finished
        private void reconFinished(string sender, string receiver, string value)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(
                System.Windows.Threading.DispatcherPriority.Normal, (Action)delegate
                {
                    if (value.Substring(16).Contains("reconstruction finished with errors"))
                    {
                        // show message when finished
                        ReconstructionControl.recon_MessageBox.AppendText(value);
                        ReconstructionControl.recon_MessageBox.ScrollToEnd();
                    }
                    else
                    {
                        if (value.Substring(16).Contains("All reconstructions finished") || value.Substring(16).Contains("Export of image stacks is finished"))
                        {
                            // notify user when finished
                            systemState.reconThreadFree = true;
                            ReconstructionControl.recon_ProgressBarTot.Foreground = Brushes.MediumBlue;
                            ReconstructionControl.recon_ProgressBar.Foreground = Brushes.MediumBlue;
                            UnmixingControl.studyList_Updated();
                        }
                    }

                });
        }

        private void unmixFinished(string sender, string receiver, string value)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(
                System.Windows.Threading.DispatcherPriority.Normal, (Action)delegate
                {
                    if (value.Substring(16).Contains("unmixing finished with errors"))
                    {
                        // show message when finished
                        UnmixingControl.unmix_MessageBox.AppendText(value);
                        UnmixingControl.unmix_MessageBox.ScrollToEnd();
                        systemState.unmixThreadFree = true;
                    }
                    else
                    {
                        if (value.Substring(16).Contains("All unmixing finished") || value.Substring(16).Contains("Export of image stacks is finished"))
                        {
                            // notify user when finished
                            systemState.unmixThreadFree = true;
                            //UnmixingControl.unmix_ProgressBar.Foreground = Brushes.MediumBlue;
                            UnmixingControl.studyList_Updated();
                        }
                        else
                        {
                            // show message when finished
                            UnmixingControl.unmix_MessageBox.AppendText("Finished unmixing " + value.Substring(16) + "\n");
                            //UnmixingControl.unmix_MessageBox.AppendText("\nExpected time to finish remaining reconstructions: " + reconstructionParameters.remainingReconTime.ToString() + "\n\n");
                            UnmixingControl.unmix_MessageBox.ScrollToEnd();
                            UnmixingControl.studyList_Updated();
                            // update study list
                            //update_scanList(value.Substring(16));
                        }
                    }

                });
        }
        #endregion


            // update scan list
            #region update_scanList
        private void update_scanList()
        {
            // Delete OverviewPanel
            patientOverviewControl.mainListStackPanel.Children.Clear();

            //unregister registered names
            unregisterRegisteredNames();

            // clear list so far
            studyParameters.myStudyDates_list.Clear();

            // list of acquisition dates
            string[] dateFolderEntries = Directory.GetDirectories(fileParameters.studyFolder).ToArray();

            // Make the first item selected.
            for (int i_date = 0; i_date < dateFolderEntries.Length; i_date++)
            {
                // list of acquisition files
                string[] acqFileEntries = Directory.GetFiles(dateFolderEntries[i_date], "*.mat").Select(System.IO.Path.GetFileNameWithoutExtension).ToArray();

                // list of reconstruction folders
                string[] reconFolderEntries = Directory.GetDirectories(dateFolderEntries[i_date]).ToArray();

                // initialize acq file list
                List<acqFileItem> myAcqFiles_list = new List<acqFileItem>();
                // initialize recon file list
                List<reconFileItem> myReconFiles_list = new List<reconFileItem>();

                // run through all acquisition files found
                for (int i_acq = 0; i_acq < acqFileEntries.Length; i_acq++)
                {
                    // initialize recon list
                    List<reconFileItem> myReconFolders_list = new List<reconFileItem>();

                    // run through all subfolders within the current study date
                    for (int i_recon = 0; i_recon < reconFolderEntries.Length; i_recon++)
                    {
                        string reconFolderWithoutPath = reconFolderEntries[i_recon].Split('\\')[reconFolderEntries[i_recon].Split('\\').Length - 1];

                        // add all reconstruction folders that correspond to the pre-defined naming convention
                        if (reconFolderWithoutPath.Length > 7 && acqFileEntries[i_acq].Length > 5)
                        {
                            // if (String.Equals(reconFolderWithoutPath.Substring(0, 8), "R_" + acqFileEntries[i_acq].Substring(0, 6)))
                            if (reconFolderWithoutPath.StartsWith("R_" + acqFileEntries[i_acq]))
                            {
                                // add recon folder to list
                                reconFileItem newReconItem = new reconFileItem(i_recon, acqFileEntries[i_acq], reconFolderEntries[i_recon], true);
                                myReconFolders_list.Add(newReconItem);
                                
                            }
                        }

                    }

                    // add acq file to list
                    acqFileItem myAcqItem = new acqFileItem(i_acq, acqFileEntries[i_acq], dateFolderEntries[i_date], true, myReconFolders_list, myReconFolders_list.Count - 1);
                    myAcqFiles_list.Add(myAcqItem);
                }

                // add study date list
                studyDateItem myStudyDateItem = new studyDateItem(i_date, dateFolderEntries[i_date], true, myAcqFiles_list, myAcqFiles_list.Count - 1);
                studyParameters.myStudyDates_list.Add(myStudyDateItem);

                // add current study dates to overview tab
                createOverviewTab(myStudyDateItem);
            }

            studyParameters.myStudyDates_listIndex = studyParameters.myStudyDates_list.Count - 1;

            ReconstructionControl.studyList_Updated();
            UnmixingControl.studyList_Updated();
            ReconstructionControl.xy_reconImage.Source = null;
            ReconstructionControl.yz_reconImage.Source = null;
            ReconstructionControl.xz_reconImage.Source = null;

            updateDiskStatus();

        }

        private void update_scanList(string reconFilePath)
        {
            // Delete OverviewPanel
            patientOverviewControl.mainListStackPanel.Children.Clear();

            //unregister registered names
            unregisterRegisteredNames();

            // clear list so far
            studyParameters.myStudyDates_list.Clear();

            // helping variables to jump to reconstructed image tab
            int iHelp_date = 0;
            int iHelp_acq = 0;
            int iHelp_recon = 0;
            int iHelp_freq = 0;

            // list of acquisition dates
            string[] dateFolderEntries = Directory.GetDirectories(fileParameters.studyFolder).ToArray();

            // Make the first item selected.
            for (int i_date = 0; i_date < dateFolderEntries.Length; i_date++)
            {
                // list of acquisition files
                string[] acqFileEntries = Directory.GetFiles(dateFolderEntries[i_date], "*.mat").Select(System.IO.Path.GetFileNameWithoutExtension).ToArray();

                // list of reconstruction folders
                string[] reconFolderEntries = Directory.GetDirectories(dateFolderEntries[i_date]).ToArray();

                // initialize acq file list
                List<acqFileItem> myAcqFiles_list = new List<acqFileItem>();

                // run through all acquisition files found
                for (int i_acq = 0; i_acq < acqFileEntries.Length; i_acq++)
                {
                    // initialize recon list
                    List<reconFileItem> myReconFolders_list = new List<reconFileItem>();

                    // set counter to zero
                    int counter = 0;

                    // run through all subfolders within the current study date
                    for (int i_recon = 0; i_recon < reconFolderEntries.Length; i_recon++)
                    {
                        string reconFolderWithoutPath = reconFolderEntries[i_recon].Split('\\')[reconFolderEntries[i_recon].Split('\\').Length - 1];

                        // add all reconstruction folders that correspond to the pre-defined naming convention
                        if (reconFolderWithoutPath.Length > 7 && acqFileEntries[i_acq].Length > 5)
                        {
                            // if (String.Equals(reconFolderWithoutPath.Substring(0, 8), "R_" + acqFileEntries[i_acq].Substring(0, 6)))
                            if (reconFolderWithoutPath.StartsWith("R_" + acqFileEntries[i_acq]))
                            {
                                reconFileItem newReconItem = new reconFileItem(i_recon, acqFileEntries[i_acq], reconFolderEntries[i_recon], false);
                                myReconFolders_list.Add(newReconItem);

                                if (reconFilePath.StartsWith(reconFolderEntries[i_recon]))
                                {
                                    iHelp_date = i_date;
                                    iHelp_acq = i_acq;
                                    iHelp_recon = counter;
                                }

                                counter++;
                            }
                        }
                    }

                    // add acq file to list
                    acqFileItem myAcqItem = new acqFileItem(i_acq, acqFileEntries[i_acq], dateFolderEntries[i_date], false, myReconFolders_list, iHelp_recon);
                    myAcqFiles_list.Add(myAcqItem);
                }

                // add study date list
                studyDateItem myStudyDateItem = new studyDateItem(i_date, dateFolderEntries[i_date], true, myAcqFiles_list, iHelp_acq);
                studyParameters.myStudyDates_list.Add(myStudyDateItem);

                // add current study dates to overview tab
                createOverviewTab(myStudyDateItem);
            }


            // update study list and show new image
            ReconstructionControl.load_reconFinished(iHelp_date, iHelp_acq);
            Thread.Sleep(500);
            if (reconFilePath.EndsWith("_LF"))
                iHelp_freq = 1;
            if (reconFilePath.EndsWith("_HF"))
                iHelp_freq = 2;
            ReconstructionControl.showRecon(iHelp_date, iHelp_acq, iHelp_recon, iHelp_freq);

            updateDiskStatus();

        }
        #endregion


        // Creates a tab of acquired and reconstructed data for one scan date
        #region scan_overview
        private void createOverviewTab(studyDateItem actualStudyDateItem)
        {
            // 0: Create a Group Box as Container with the scan date as label
            GroupBox myOverviewTab = new GroupBox();
            myOverviewTab.VerticalAlignment = VerticalAlignment.Stretch;
            myOverviewTab.Margin = new System.Windows.Thickness(5, 10, 5, 10);
            myOverviewTab.Height = 320;                 
            string dateLabel = actualStudyDateItem.folderPath.Split('\\')[actualStudyDateItem.folderPath.Split('\\').Length - 1];
            myOverviewTab.Header = " "+dateLabel+" ";            

            // 0.1: add a StackPanel, that is containing all information: Scan list, scan Thumb, scan info/edit | Recon list, Recon thumb, recon edit
            StackPanel mainOverviewStackPanel = new StackPanel();
            mainOverviewStackPanel.Orientation = Orientation.Horizontal;

            #region acquisition_1
            // 1: Acquired scan info
            // 1.1: A Dock Panel for the scan list 
            DockPanel scanList_Panel = new DockPanel();
            scanList_Panel.Width = 320;
            scanList_Panel.Margin = new Thickness(10, 10, 10, 10);

            // list of scan names
            List<string> scanName_list = new List<string>();
            for (int i = 0; i < actualStudyDateItem.myAcqFiles_list.Count; i++)
            {
                scanName_list.Add(actualStudyDateItem.myAcqFiles_list[i].fileName);
            }
            
            // label of panel
            Label scanLabel = new Label();
            scanLabel.Content="Acquired Scans";
            scanLabel.FontWeight = FontWeights.Bold;
            scanLabel.Foreground = Brushes.White;
            scanLabel.BorderThickness = new Thickness(0, 0, 0, 3);
            scanLabel.BorderBrush = Brushes.White;
            scanLabel.Height = 38;

            // scrollviewer of panel
            ScrollViewer scrollViewer = new ScrollViewer();
            scrollViewer.HorizontalAlignment = HorizontalAlignment.Stretch;
            scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

            // list view of acquired scans
            ListView scanListView = new ListView();
            scanListView.ItemsSource = scanName_list;
            scanListView.Background = Brushes.Transparent;
            scanListView.Foreground = Brushes.White;
            // scanListView.FontWeight = FontWeights.Bold;
            scanListView.BorderThickness = new Thickness(0, 0, 0, 0);
            scanListView.SelectionChanged += (sender, args) => showScanDetails(sender, args, actualStudyDateItem.id, scanListView.SelectedIndex, actualStudyDateItem.myAcqFiles_list[scanListView.SelectedIndex]);

            // add controls to panel
            DockPanel.SetDock(scanLabel, Dock.Top);
            DockPanel.SetDock(scrollViewer, Dock.Top);
            scrollViewer.Content = scanListView;
            scanList_Panel.Children.Add(scanLabel);
            scanList_Panel.Children.Add(scrollViewer);
            mainOverviewStackPanel.Children.Add(scanList_Panel);

            // 1.2: a placeholder for scan thumbnails
            Grid ScanThumbs = new Grid();
            ScanThumbs.Width = 300;
            ScanThumbs.Margin = new Thickness(10, 45, 10, 10);
            string gridName = "scanThumbs_Grid_" + actualStudyDateItem.id;
            ScanThumbs.Name = gridName;
            // register the name, so it is possibvle to adress the elemnet via name later
            RegisterName(gridName, ScanThumbs);
            mainOverviewStackPanel.Children.Add(ScanThumbs);

            // 1.3: a DockPanel for scan meta info and edit buttons 
            DockPanel ScanInfo = new DockPanel();
            ScanInfo.Width = 320;
            ScanInfo.Margin = new Thickness(10, 45, 30, 10);
            gridName = "scanInfo_Panel_" + actualStudyDateItem.id;
            ScanInfo.Name = gridName;
            // register the name, so it is possibvle to adress the elemnet via name later
            RegisterName(gridName, ScanInfo);
            mainOverviewStackPanel.Children.Add(ScanInfo);
            #endregion

            // 2: a vertical line seperating acquisition and reconstruction
            System.Windows.Shapes.Rectangle mySeparator = new System.Windows.Shapes.Rectangle();
            mySeparator.Width = 3;
            mySeparator.Height = 270;
            mySeparator.Fill = Brushes.White;
            mainOverviewStackPanel.Children.Add(mySeparator);

            #region reconstruction_3
            // 3: Reconstruction info
            // 3.1: ListView of all recons
            DockPanel reconList_Panel = new DockPanel();
            reconList_Panel.Width = 320;
            reconList_Panel.Margin = new Thickness(10, 10, 10, 10);
            gridName = "reconList_Panel_" + actualStudyDateItem.id;
            reconList_Panel.Name = gridName;
            // register the name, so it is possibvle to adress the elemnet via name later
            RegisterName(gridName, reconList_Panel);
            mainOverviewStackPanel.Children.Add(reconList_Panel);

            // 3.2: a placeholder for recon thumbs
            Grid ReconThumbs = new Grid();
            ReconThumbs.Width = 300;
            ReconThumbs.Margin = new Thickness(10, 45, 10, 10);
            gridName = "reconThumbs_Grid_" + actualStudyDateItem.id;
            ReconThumbs.Name = gridName;
            // register the name, so it is possible to adress the element via name later
            RegisterName(gridName, ReconThumbs);
            mainOverviewStackPanel.Children.Add(ReconThumbs);

            // 3.3: a DockPanel for scan meta info and edit buttons 
            StackPanel ReconInfo = new StackPanel();
            ReconInfo.Width = 100;
            ReconInfo.Margin = new Thickness(10, 45, 10, 10);
            ReconInfo.HorizontalAlignment = HorizontalAlignment.Left;
            gridName = "reconInfo_Panel_" + actualStudyDateItem.id;
            ReconInfo.Name = gridName;
            // register the name, so it is possibvle to adress the elemnet via name later
            RegisterName(gridName, ReconInfo);
            mainOverviewStackPanel.Children.Add(ReconInfo);
            #endregion

            // 0: add the Tab to the PatientInformationScreen
            myOverviewTab.Content = mainOverviewStackPanel;
            patientOverviewControl.mainListStackPanel.Children.Add(myOverviewTab);

        }

        private void showScanDetails(object sender, RoutedEventArgs e, int i_date, int i_acq, acqFileItem actualAcqFileItem)
        {
            try
            {
                #region scanThumb1.2
                // 1.2: Add scanThumb
                Image scanThumb = new Image();
                BitmapImage src = new BitmapImage();
                src.BeginInit();
                src.CacheOption = BitmapCacheOption.OnLoad;
                src.UriSource = new Uri(actualAcqFileItem.folderPath + "\\Thumbnail\\" + actualAcqFileItem.fileName + ".png", UriKind.Absolute);
                src.EndInit();
                scanThumb.Source = src;
                scanThumb.Stretch = Stretch.Fill;
                scanThumb.MouseEnter += (innersender, args) => changeMouse(innersender, args, true);
                scanThumb.MouseLeave += (innersender, args) => changeMouse(innersender, args, false);
                scanThumb.MouseLeftButtonDown += (innersender, args) => openSelectedScan(innersender, args, i_date, i_acq);

                // Add acq file thumbnail and info
                string gridNamethumbs = "scanThumbs_Grid_" + i_date;
                object j = this.FindName(gridNamethumbs);
                ((Grid)j).Children.Clear();
                ((Grid)j).Children.Add(scanThumb);
                #endregion

                #region scanInfo1.3
                // 1.3.1: Create edit and rename panel
                Button renameButton_acq = new Button();
                renameButton_acq.Content = FindResource("IconRenameText");
                renameButton_acq.Style = (Style)FindResource("UidEditButton");

                Button deleteButton_acq = new Button();
                deleteButton_acq.Content = FindResource("IconTrashCan");
                deleteButton_acq.Style = (Style)FindResource("UidEditButton");

                Button rename_acq_yes = new Button();
                rename_acq_yes.Content = FindResource("IconCheckMark");
                rename_acq_yes.Visibility = Visibility.Collapsed;
                rename_acq_yes.Style = (Style)FindResource("UidEditButton");

                Button rename_acq_no = new Button();
                rename_acq_no.Content = FindResource("IconDelete");
                rename_acq_no.Visibility = Visibility.Collapsed;
                rename_acq_no.Style = (Style)FindResource("UidEditButton");

                TextBox renameText_acq = new TextBox();
                renameText_acq.Visibility = Visibility.Collapsed;

                // Create action when clicked
                deleteButton_acq.Click += (innersender3, args) => deleteScan(innersender3, args, i_date, i_acq);
                renameButton_acq.Click += (innersender2, args) => renameScan(innersender2, args, i_date, i_acq, renameButton_acq, deleteButton_acq, renameText_acq, rename_acq_yes, rename_acq_no);
                rename_acq_yes.Click += (innersender4, args) => renameScan_acq_yes(innersender4, args, i_date, i_acq, renameButton_acq, deleteButton_acq, renameText_acq, rename_acq_yes, rename_acq_no);
                rename_acq_no.Click += (innersender5, args) => renameScan_acq_no(innersender5, args, i_date, i_acq, renameButton_acq, deleteButton_acq, renameText_acq, rename_acq_yes, rename_acq_no);
                
                // Add rename button list to Panel
                DockPanel editButtons_acq = new DockPanel();
                DockPanel.SetDock(renameButton_acq, Dock.Left);
                DockPanel.SetDock(deleteButton_acq, Dock.Left);
                DockPanel.SetDock(rename_acq_yes, Dock.Left);
                DockPanel.SetDock(rename_acq_no, Dock.Left);
                DockPanel.SetDock(renameText_acq, Dock.Left);
                editButtons_acq.Children.Add(renameButton_acq);
                editButtons_acq.Children.Add(deleteButton_acq);
                editButtons_acq.Children.Add(rename_acq_yes);
                editButtons_acq.Children.Add(rename_acq_no);
                editButtons_acq.Children.Add(renameText_acq);

                // 1.3.2: Show scan info TextBlock with scroll

                // scroll viewer
                ScrollViewer scrollViewer_scanInfo = new ScrollViewer();
                scrollViewer_scanInfo.HorizontalAlignment = HorizontalAlignment.Left;
                scrollViewer_scanInfo.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                scrollViewer_scanInfo.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

                // text block with info
                TextBlock myScanInfo = new TextBlock();
                myScanInfo.Width = 320;
                myScanInfo.HorizontalAlignment = HorizontalAlignment.Stretch;
                myScanInfo.TextWrapping = TextWrapping.Wrap;
                myScanInfo.Foreground = Brushes.White;
                myScanInfo.Margin = new Thickness(10, 10, 10, 10);

                // populate textblock with meta data --> load xml file
                XmlDocument xml = new XmlDocument();
                string scanMetaData_filename = actualAcqFileItem.folderPath + "\\" + actualAcqFileItem.fileName + ".xml";

                try
                {
                    xml.Load(@scanMetaData_filename);
                }
                catch
                {
                    myScanInfo.Text = "no aditional Information available";
                }

                try
                {
                    string l_x = xml.SelectSingleNode("acquisitionParameters/l_x").InnerText;
                    string l_y = xml.SelectSingleNode("acquisitionParameters/l_y").InnerText;
                    string z_high = xml.SelectSingleNode("acquisitionParameters/z_high").InnerText;
                    string comment = xml.SelectSingleNode("acquisitionParameters/comment").InnerText;
                    string input_range = xml.SelectSingleNode("acquisitionParameters/inputRange").InnerText;

                    myScanInfo.Text = "ROI           : " + l_x + "mm x " + l_y + " mm\n";
                    myScanInfo.Text += "Depth       : " + z_high + " mm\n";
                    myScanInfo.Text += "Input        : " + input_range + " mV\n";
                    myScanInfo.Text += "Comment :\n" + comment;

                }
                catch
                {
                    myScanInfo.Text = "no aditional Information available";
                }
                
                // Add to DockPanel
                scrollViewer_scanInfo.Content = myScanInfo;
                string gridNameInfo = "scanInfo_Panel_" + i_date;
                object k = this.FindName(gridNameInfo);
                DockPanel.SetDock(editButtons_acq, Dock.Top);
                DockPanel.SetDock(scrollViewer_scanInfo, Dock.Top);
                ((DockPanel)k).Children.Clear();
                ((DockPanel)k).Children.Add(editButtons_acq);
                ((DockPanel)k).Children.Add(scrollViewer_scanInfo);
                #endregion

                #region reconList_3.1
                // 3.1: Add ReconList to Panel

                // label of panel
                Label reconLabel = new Label();
                reconLabel.Content = "Reconstructions";
                reconLabel.FontWeight = FontWeights.Bold;
                reconLabel.Foreground = Brushes.White;
                reconLabel.BorderThickness = new Thickness(0, 0, 0, 3);
                reconLabel.BorderBrush = Brushes.White;
                reconLabel.Height = 35;

                // create Lis of all recon names
                List<string> reconName_list = new List<string>();
                for (int i = 0; i < actualAcqFileItem.myReconFolders_list.Count; i++)
                {
                    string nameHelp = actualAcqFileItem.myReconFolders_list[i].folderPath;
                    nameHelp = nameHelp.Split('\\')[nameHelp.Split('\\').Length - 1];
                    reconName_list.Add(nameHelp);
                }

                // scroll viewer
                ScrollViewer scrollViewer_recon = new ScrollViewer();
                scrollViewer_recon.HorizontalAlignment = HorizontalAlignment.Left;
                scrollViewer_recon.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                scrollViewer_recon.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

                // list view of all recons
                ListView reconListView = new ListView();
                reconListView.ItemsSource = reconName_list;
                reconListView.Background = Brushes.Transparent;
                reconListView.Foreground = Brushes.White;
                // reconListView.FontWeight = FontWeights.Bold;
                reconListView.BorderThickness = new Thickness(0, 0, 0, 0);
                reconListView.SelectionChanged += (sender1, args1) => showReconDetails(sender1, args1, i_date, i_acq, reconListView.SelectedIndex, actualAcqFileItem.myReconFolders_list[reconListView.SelectedIndex]);
                
                // add controls to panel
                scrollViewer_recon.Content = reconListView;
                DockPanel.SetDock(reconLabel, Dock.Top);
                DockPanel.SetDock(scrollViewer_recon, Dock.Top);

                string gridNameReconList = "reconList_Panel_" + i_date;
                object l = this.FindName(gridNameReconList);
                ((DockPanel)l).Children.Clear();
                ((DockPanel)l).Children.Add(reconLabel);
                ((DockPanel)l).Children.Add(scrollViewer_recon);
                #endregion

                string gridNameReconThumbs = "reconThumbs_Grid_" + i_date;
                object m = this.FindName(gridNameReconThumbs);
                ((Grid)m).Children.Clear();

                string gridNameReconInfo = "reconInfo_Panel_" + i_date;
                object n = this.FindName(gridNameReconInfo);
                ((StackPanel)n).Children.Clear();

            }
            catch
            {
                TextBlock myDefaultTextBlock = new TextBlock();
                myDefaultTextBlock.Text = "Couldn't find Scan Information";
                myDefaultTextBlock.HorizontalAlignment = HorizontalAlignment.Stretch;
                myDefaultTextBlock.TextWrapping = TextWrapping.Wrap;

                string gridName = "scanThumbs_Grid_" + i_date;
                object j = this.FindName(gridName);
                ((Grid)j).Children.Clear();
                ((Grid)j).Children.Add(myDefaultTextBlock);

                string gridNameInfo = "scanInfo_Panel_" + i_date;
                object k = this.FindName(gridNameInfo);
                ((DockPanel)k).Children.Clear();

                string gridNamereconList = "reconList_Panel_" + i_date;
                object l = this.FindName(gridNamereconList);
                ((DockPanel)l).Children.Clear();

                string gridNamethumbs = "reconThumbs_Grid_" + i_date;
                object m = this.FindName(gridNamethumbs);
                ((Grid)m).Children.Clear();

                string reconInfo_Panel = "reconInfo_Panel_" + i_date;
                object n = this.FindName(reconInfo_Panel);
                ((StackPanel)n).Children.Clear();
                
            }
        }

        private void showReconDetails(object sender, RoutedEventArgs e, int i_date, int i_acq, int i_recon, reconFileItem actualReconItem)
        {
            try
            {
                // 3.2 Add reconThumb
                string thumbnailPath = actualReconItem.folderPath + "\\Thumbnail\\R_thumb_" + actualReconItem.fileName;
                Image reconThumb = new Image();
                BitmapImage src = new BitmapImage();
                src.BeginInit();
                src.CacheOption = BitmapCacheOption.OnLoad;
                src.UriSource = new Uri(thumbnailPath + ".png", UriKind.Absolute);
                src.EndInit();
                reconThumb.Source = src;
                reconThumb.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                reconThumb.MouseEnter += (innersender, args) => changeMouse(innersender, args, true);
                reconThumb.MouseLeave += (innersender, args) => changeMouse(innersender, args, false);
                reconThumb.MouseLeftButtonDown += (innersender, args) => openSelectedRecon(innersender, args, i_date, i_acq, i_recon);

                // Add to Grid
                string gridNameReconthumbs = "reconThumbs_Grid_" + i_date;
                object j = this.FindName(gridNameReconthumbs);
                ((Grid)j).Children.Clear();
                ((Grid)j).Children.Add(reconThumb);


                // 3.3: Create delete panel
                Button deleteButton_recon = new Button();
                deleteButton_recon.Content = FindResource("IconTrashCan");
                deleteButton_recon.Style = (Style)FindResource("UidEditButton");
                deleteButton_recon.HorizontalAlignment = HorizontalAlignment.Left;

                // Create action when clicked
                deleteButton_recon.Click += (sender1, args) => deleteRecon(sender1, args, i_date, i_acq, i_recon);

                // Add to Panel
                string gridNameReconInfo = "reconInfo_Panel_" + i_date;
                object k = this.FindName(gridNameReconInfo);
                // DockPanel.SetDock(deleteButton_recon, Dock.Top);
                ((StackPanel)k).Children.Clear();
                ((StackPanel)k).Children.Add(deleteButton_recon);

            }
            catch
            {
                // Configure the message box to be displayed
                string deleteFolder = studyParameters.myStudyDates_list[i_date].myAcqFiles_list[i_acq].myReconFolders_list[i_recon].folderPath;
                string messageBoxText = "The reconstruction folder " + deleteFolder + " is corrupted and should be deleted. Continue?";
                string caption = "Delete reconstruction";
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxImage icon = MessageBoxImage.Warning;

                // Display message box
                MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);

                // Process message box results
                switch (result)
                {
                    case MessageBoxResult.Yes:

                        // delete recon folder                   
                        Directory.Delete(@deleteFolder, true);

                        // update the list of acq files to reconstruct in recon tab - Show no recon image
                        update_scanList();
                        ReconstructionControl.studyList_Updated();
                        UnmixingControl.studyList_Updated();

                        break;

                    case MessageBoxResult.No:
                        break;
                }


            }
        }

        private void changeMouse(object sender, RoutedEventArgs e, bool change)
        {
            if(change)
                Mouse.OverrideCursor = Cursors.Hand;
            else
                Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void openSelectedScan(object sender, RoutedEventArgs e, int date, int acq)
        {
            // helping variables to jump to reconstructed image tab
            int iHelp_date = date;
            int iHelp_acq = acq;

            // update selected list index for current study date and selected acquisition file
            studyParameters.myStudyDates_listIndex = date;
            studyParameters.myStudyDates_list[studyParameters.myStudyDates_listIndex].myAcqFiles_listIndex = acq;

            // place check mark on selected acq file - all other acquisitions are unchecked
            for (int i_acq = 0; i_acq < studyParameters.myStudyDates_list[studyParameters.myStudyDates_listIndex].myAcqFiles_list.Count; i_acq++)
            {
                if (i_acq == acq)
                {
                    studyParameters.myStudyDates_list[studyParameters.myStudyDates_listIndex].myAcqFiles_list[i_acq].isChecked = true;
                }
                else
                {
                    studyParameters.myStudyDates_list[studyParameters.myStudyDates_listIndex].myAcqFiles_list[i_acq].isChecked = false;
                }
            }

            // update the list of acq files to reconstruct in recon tab - Show no recon image
            ReconstructionControl.studyList_Updated();
            UnmixingControl.studyList_Updated();
            ReconstructionControl.xy_reconImage.Source = null;
            ReconstructionControl.yz_reconImage.Source = null;
            ReconstructionControl.xz_reconImage.Source = null;

            // make recon tab visible
            overviewToggleButton.IsChecked = false;
            acquisitionToggleButton.IsChecked = false;
            analysisToggleButton.IsChecked = true;
            UnmixingToggleButton.IsChecked = false;
            patientOverviewControl.Visibility = Visibility.Collapsed;
            ScanningControl.Visibility = Visibility.Collapsed;
            ReconstructionControl.Visibility = Visibility.Visible;
            UnmixingControl.Visibility = Visibility.Collapsed;
        }

        private void renameScan(object sender, RoutedEventArgs e, int date, int acq, Button renameButton_acq, Button deleteButton_acq, TextBox renameText_acq, Button rename_acq_yes, Button rename_acq_no)
        {
            renameButton_acq.Visibility = Visibility.Collapsed;
            deleteButton_acq.Visibility = Visibility.Collapsed;
            renameText_acq.Visibility = Visibility.Visible;
            rename_acq_yes.Visibility = Visibility.Visible;
            rename_acq_no.Visibility = Visibility.Visible;
        }

        private void renameScan_acq_yes(object sender, RoutedEventArgs e, int date, int acq, Button renameButton_acq, Button deleteButton_acq, TextBox renameText_acq, Button rename_acq_yes, Button rename_acq_no)
        {
            // Change visibility
            renameButton_acq.Visibility = Visibility.Visible;
            deleteButton_acq.Visibility = Visibility.Visible;
            renameText_acq.Visibility = Visibility.Collapsed;
            rename_acq_yes.Visibility = Visibility.Collapsed;
            rename_acq_no.Visibility = Visibility.Collapsed;

            // check for illelagl characters and replace
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                renameText_acq.Text = renameText_acq.Text.Replace(c, '_');
            }
            renameText_acq.Text = renameText_acq.Text.Replace('.', '_');

            // old and new scan name
            string oldName = studyParameters.myStudyDates_list[date].myAcqFiles_list[acq].fileName;
            string newName = studyParameters.myStudyDates_list[date].myAcqFiles_list[acq].fileName.Substring(0, 6) + "_" + renameText_acq.Text;

            // Configure the message box to be displayed
            string messageBoxText = "Do you want to rename " + oldName + " by new name " + newName + "?";
            string caption = "Rename scan";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Warning;

            // Display message box
            MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);

            // Process message box results
            switch (result)
            {
                case MessageBoxResult.Yes:

                    // create list of files/folders to be renamed
                    List<string> renameFile_old = new List<string>();
                    renameFile_old.Clear();
                    List<string> renameFile_new = new List<string>();
                    renameFile_new.Clear();
                    List<string> renameFolder_old = new List<string>();
                    renameFolder_old.Clear();
                    List<string> renameFolder_new = new List<string>();
                    renameFolder_new.Clear();

                    // fill rename lists with files and folder paths
                    try
                    {
                        DirectoryInfo parentDir = new DirectoryInfo(@studyParameters.myStudyDates_list[date].folderPath);
                        int Place = -1;

                        // search through all Acq folders: Log, QuickScan, Recons, Thumbnail
                        string newFullname;
                        foreach (DirectoryInfo childDir in parentDir.GetDirectories())
                        {
                            // rename Recon folders - replace the last occurence of the found string only
                            Place = childDir.FullName.LastIndexOf(oldName);
                            if (Place >= 0)
                            {
                                newFullname = childDir.FullName.Remove(Place, oldName.Length).Insert(Place, newName);
                                // childDir.MoveTo(@newFullname);
                                renameFolder_old.Add(childDir.FullName);
                                renameFolder_new.Add(newFullname);
                            }

                            // search through all Recon folders: Export, Images, LogFile, Thumbnail
                            DirectoryInfo parentDir_2gen = new DirectoryInfo(@childDir.FullName);
                            foreach (DirectoryInfo childDir_2gen in parentDir_2gen.GetDirectories())
                            {
                                // replace file names within Recon subfolders
                                DirectoryInfo parentDir_3gen = new DirectoryInfo(@childDir_2gen.FullName);
                                foreach (FileInfo childFile_3gen in parentDir_3gen.GetFiles())
                                {
                                    // replace the last occurence of the found string
                                    Place = childFile_3gen.FullName.LastIndexOf(oldName);
                                    if (Place >= 0)
                                    {
                                        newFullname = childFile_3gen.FullName.Remove(Place, oldName.Length).Insert(Place, newName);
                                        // childFile_3gen.MoveTo(@newFullname);
                                        renameFile_old.Add(childFile_3gen.FullName);
                                        renameFile_new.Add(newFullname);
                                    }
                                }
                            }

                            // replace file names within Acq folders
                            foreach (FileInfo childFile_2gen in parentDir_2gen.GetFiles())
                            {
                                // replace the last occurence of the found string
                                Place = childFile_2gen.FullName.LastIndexOf(oldName);
                                if (Place >= 0)
                                {
                                    newFullname = childFile_2gen.FullName.Remove(Place, oldName.Length).Insert(Place, newName);
                                    // childFile_2gen.MoveTo(@newFullname);
                                    renameFile_old.Add(childFile_2gen.FullName);
                                    renameFile_new.Add(newFullname);
                                }
                            }
                        }

                        // replace file names within study date folder
                        foreach (FileInfo childFile in parentDir.GetFiles())
                        {
                            // replace the last occurence of the found string
                            Place = childFile.FullName.LastIndexOf(oldName);
                            if (Place >= 0)
                            {
                                newFullname = childFile.FullName.Remove(Place, oldName.Length).Insert(Place, newName);
                                // childFile.MoveTo(@newFullname);
                                renameFile_old.Add(childFile.FullName);
                                renameFile_new.Add(newFullname);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Console.WriteLine("ERROR: renaming error occured. Check folder and file structure.");
                        Console.WriteLine("ERROR: Unable to rename scan. Rename list could not be created. " + ex.Message);
                        break;
                    }

                    // try to rename files
                    int i_ren = 0;
                    try
                    {
                        FileInfo temp = null;
                        for (i_ren = 0; i_ren < renameFile_old.Count(); i_ren++)
                        {
                            temp = new FileInfo(renameFile_old[i_ren]);
                            temp.MoveTo(@renameFile_new[i_ren]);
                        }
                    }
                    catch (Exception ex)
                    {
                        // show message that error has occured
                        Console.WriteLine("WARNING: Files cannot be renamed. " + ex.Message);
                        messageBoxText = "Files cannot be renamed. Access was denied. Close all open files and folders, restart the software, and try again. Previous file name will be restored.";
                        caption = "Rename scan - Warning";
                        button = MessageBoxButton.OK;
                        icon = MessageBoxImage.Warning;
                        MessageBox.Show(messageBoxText, caption, button, icon);

                        // restore original file name and break
                        try
                        {
                            FileInfo temp = null;
                            for (int i_res = 0; i_res < i_ren; i_res++)
                            {
                                temp = new FileInfo(renameFile_new[i_res]);
                                temp.MoveTo(@renameFile_old[i_res]);
                            }
                        }
                        catch
                        {
                            Console.WriteLine("ERROR: error occured, when trying to restore old file names. File and folder names might be corrupted.");
                            messageBoxText = "ERROR: error occured, when trying to restore old file names. File and folder names might be corrupted.";
                            caption = "Rename scan - Error";
                            button = MessageBoxButton.OK;
                            icon = MessageBoxImage.Error;
                            MessageBox.Show(messageBoxText, caption, button, icon);
                        }
                        break;
                    }

                    // try to rename folders
                    try
                    {
                        DirectoryInfo temp = null;
                        for (i_ren = 0; i_ren < renameFolder_old.Count(); i_ren++)
                        {
                            temp = new DirectoryInfo(renameFolder_old[i_ren]);
                            temp.MoveTo(@renameFolder_new[i_ren]);
                        }
                    }
                    catch (Exception ex)
                    {
                        // show message that error has occured
                        Console.WriteLine("WARNING: Folders cannot be renamed. " + ex.Message);
                        messageBoxText = "Folders cannot be renamed. Access was denied. Close all open files and folders, restart the software, and try again. Previous file name will be restored.";
                        caption = "Rename scan - Warning";
                        button = MessageBoxButton.OK;
                        icon = MessageBoxImage.Warning;
                        MessageBox.Show(messageBoxText, caption, button, icon);

                        // restore original file name and break
                        try
                        {
                            DirectoryInfo temp = null;
                            for (int i_res = 0; i_res < i_ren; i_res++)
                            {
                                temp = new DirectoryInfo(renameFolder_new[i_res]);
                                temp.MoveTo(@renameFolder_old[i_res]);
                            }

                            // revert file names
                            FileInfo temp2 = null;
                            for (int i_res = 0; i_res < renameFile_old.Count(); i_res++)
                            {
                                temp2 = new FileInfo(renameFile_new[i_res]);
                                temp2.MoveTo(@renameFile_old[i_res]);
                            }
                        }
                        catch
                        {
                            Console.WriteLine("ERROR: error occured, when trying to restore old file/folder names. File and folder names might be corrupted.");
                            messageBoxText = "ERROR: error occured, when trying to restore old file/folder names. File and folder names might be corrupted.";
                            caption = "Rename scan - Error";
                            button = MessageBoxButton.OK;
                            icon = MessageBoxImage.Error;
                            MessageBox.Show(messageBoxText, caption, button, icon);
                        }
                        break;
                    }

                    // update the list of acq files to reconstruct in recon tab - Show no recon image
                    update_scanList();
                    ReconstructionControl.studyList_Updated();
                    UnmixingControl.studyList_Updated();
                    ReconstructionControl.xy_reconImage.Source = null;
                    ReconstructionControl.yz_reconImage.Source = null;
                    ReconstructionControl.xz_reconImage.Source = null;

                    break;

                case MessageBoxResult.No:
                    break;
            }
        }

        private void renameScan_acq_no(object sender, RoutedEventArgs e, int date, int acq, Button renameButton_acq, Button deleteButton_acq, TextBox renameText_acq, Button rename_acq_yes, Button rename_acq_no)
        {
            // Change visibility
            renameButton_acq.Visibility = Visibility.Visible;
            deleteButton_acq.Visibility = Visibility.Visible;
            renameText_acq.Visibility = Visibility.Collapsed;
            rename_acq_yes.Visibility = Visibility.Collapsed;
            rename_acq_no.Visibility = Visibility.Collapsed;
        }

        private void deleteScan(object sender, RoutedEventArgs e, int date, int acq)
        {
            // Configure the message box to be displayed
            string messageBoxText = "Do you want to delete the scan "
                + studyParameters.myStudyDates_list[date].myAcqFiles_list[acq].folderPath + "\\"
                + studyParameters.myStudyDates_list[date].myAcqFiles_list[acq].fileName + " and all its reconstructions?";
            string caption = "Delete scan";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Warning;

            // Display message box
            MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);

            // Process message box results
            switch (result)
            {
                case MessageBoxResult.Yes:

                    // delete recon folder
                    for (int i_recon = 0; i_recon < studyParameters.myStudyDates_list[date].myAcqFiles_list[acq].myReconFolders_list.Count; i_recon++)
                    {
                        string deleteFolder = studyParameters.myStudyDates_list[date].myAcqFiles_list[acq].myReconFolders_list[i_recon].folderPath;
                        Directory.Delete(@deleteFolder, true);
                    }

                    // delete acq matlab file, meta info, and thumbnail
                    string matFile = studyParameters.myStudyDates_list[date].myAcqFiles_list[acq].folderPath + "\\" + studyParameters.myStudyDates_list[date].myAcqFiles_list[acq].fileName + ".mat";
                    string metaFile = studyParameters.myStudyDates_list[date].myAcqFiles_list[acq].folderPath + "\\" + studyParameters.myStudyDates_list[date].myAcqFiles_list[acq].fileName + ".xml";
                    string thumbnailFile = studyParameters.myStudyDates_list[date].myAcqFiles_list[acq].folderPath + "\\Thumbnail\\" + studyParameters.myStudyDates_list[date].myAcqFiles_list[acq].fileName + ".png";
                    File.Delete(@matFile);
                    File.Delete(@metaFile);
                    File.Delete(@thumbnailFile);

                    // update the list of acq files to reconstruct in recon tab - Show no recon image
                    update_scanList();
                    ReconstructionControl.studyList_Updated();
                    UnmixingControl.studyList_Updated();

                    break;

                case MessageBoxResult.No:
                    break;
            }
        }

        private void deleteRecon(object sender, RoutedEventArgs e, int date, int acq, int recon)
        {
            // Configure the message box to be displayed
            string deleteFolder = studyParameters.myStudyDates_list[date].myAcqFiles_list[acq].myReconFolders_list[recon].folderPath;
            string messageBoxText = "Do you want to delete" + deleteFolder + " ?";
            string caption = "Delete reconstruction";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Warning;

            // Display message box
            MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);

            // Process message box results
            switch (result)
            {
                case MessageBoxResult.Yes:

                    // delete recon folder                   
                    Directory.Delete(@deleteFolder, true);

                    // update the list of acq files to reconstruct in recon tab - Show no recon image
                    update_scanList();
                    ReconstructionControl.studyList_Updated();
                    UnmixingControl.studyList_Updated();

                    break;

                case MessageBoxResult.No:
                    break;
            }
        }

        private void openSelectedRecon(object sender, RoutedEventArgs e, int date, int acq, int recon)
        {
            // helping variables to jump to reconstructed image tab
            int iHelp_date = date;
            int iHelp_acq = acq;
            int iHelp_recon = recon;
            int iHelp_freq = 0;

            // update selected list index for current study date and selected acquisition file
            studyParameters.myStudyDates_listIndex = date;
            studyParameters.myStudyDates_list[studyParameters.myStudyDates_listIndex].myAcqFiles_listIndex = acq;

            // place check mark on selected acq file - all other acquisitions are unchecked
            for (int i_acq = 0; i_acq < studyParameters.myStudyDates_list[studyParameters.myStudyDates_listIndex].myAcqFiles_list.Count; i_acq++)
            {
                if (i_acq == acq)
                {
                    studyParameters.myStudyDates_list[studyParameters.myStudyDates_listIndex].myAcqFiles_list[i_acq].isChecked = true;
                }
                else
                {
                    studyParameters.myStudyDates_list[studyParameters.myStudyDates_listIndex].myAcqFiles_list[i_acq].isChecked = false;
                }
            }

            // update the list of acq files to reconstruct in recon tab - Show recon image
            ReconstructionControl.studyList_Updated();
            UnmixingControl.studyList_Updated();
            ReconstructionControl.showRecon(iHelp_date, iHelp_acq, iHelp_recon, iHelp_freq);

            // make recon tab visible
            overviewToggleButton.IsChecked = false;
            acquisitionToggleButton.IsChecked = false;
            analysisToggleButton.IsChecked = true;
            UnmixingToggleButton.IsChecked = false;
            patientOverviewControl.Visibility = Visibility.Collapsed;
            ScanningControl.Visibility = Visibility.Collapsed;
            ReconstructionControl.Visibility = Visibility.Visible;
            UnmixingControl.Visibility = Visibility.Collapsed;
            
        }

        private void unregisterRegisteredNames()
        {
            int numberOfChildren = 0;
            bool search_more = true;

            while (search_more)
            {
                object j = FindName("scanThumbs_Grid_" + numberOfChildren);
                if (j == null)
                    search_more = false;
                else
                    numberOfChildren++;
            }

            for (int i = 0; i < numberOfChildren; i++)
            {
                try
                {
                    UnregisterName("scanThumbs_Grid_" + i);
                    UnregisterName("scanInfo_Panel_" + i);
                    UnregisterName("reconList_Panel_" + i);
                    UnregisterName("reconThumbs_Grid_" + i);
                    UnregisterName("reconInfo_Panel_" + i);
                }
                catch
                {

                }

            }
        }
        #endregion

        #region diskStatus
        private void updateDiskStatus()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            string usedHardDrive = fileParameters.RSOMdataFolder.Substring(0, 3);

            foreach (DriveInfo d in allDrives)
            {
                if (d.Name == usedHardDrive)
                {
                    double usedDiskSpaceInPercent = (1 - (double)d.AvailableFreeSpace / (double)d.TotalSize) * 100;
                    DiskStatusProgressBar.Value = usedDiskSpaceInPercent;
                    DiskStatusProgressBar.ToolTip = usedDiskSpaceInPercent.ToString("0") + "%";
                }
            }
        }
        #endregion

        // commands for closing ViewRSOM
        #region close_ViewRSOM
        private void exitMenuItem_Click(object sender, RoutedEventArgs e)
        {

            string messageBoxText, caption;
            MessageBoxResult result;

            if (systemState.acqThreadFree && systemState.reconThreadFree)
            {
                // Configure the message box to be displayed
                messageBoxText = "Exit ViewRSOM?";
                caption = "Exit View RSOM";
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxImage icon = MessageBoxImage.Information;
                // Display message box
                result = MessageBox.Show(messageBoxText, caption, button, icon);
            }
            else
            {
                // Configure the message box to be displayed
                messageBoxText = "Acquisition or reconstruction is still running. Please wait till the process has finished. If you continue, all processes will be interrupted and might result in corrupted data folders. Continue to exit?";
                caption = "Exit View RSOM";
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxImage icon = MessageBoxImage.Warning;
                // Display message box
                result = MessageBox.Show(messageBoxText, caption, button, icon);
            }


            // Process message box results
            switch (result)
            {
                case MessageBoxResult.Yes:

                    // Stop camera
                    //myUSBcamera.cameraRecord = false;
                    Thread.Sleep(100);
                    //myUSBcamera.StopCamera();
                    try
                    {
                        //my_laser = new ViewModelLaserInnolas();                        
                        my_laser.compositeClose();                        
                    }
                    catch { System.Windows.MessageBox.Show("Not possible"); }

                    this.Close();
                    break;

                case MessageBoxResult.No:

                    break;
            }

        }

        private void logOffItem_Click(object sender, RoutedEventArgs e)
        {
            // Configure the message box to be displayed
            string messageBoxText = "You are about to log off from this PC! Continue?";
            string caption = "Log off!";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Warning;
            // Display message box
            MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);

            // Process message box results
            switch (result)
            {
                case MessageBoxResult.Yes:

                    // Stop camera 
                    myUSBcamera.StopCamera();

                    LogOff();
                    break;

                case MessageBoxResult.No:

                    break;
            }
        }

        private void shutDownItem_Click(object sender, RoutedEventArgs e)
        {
            // Configure the message box to be displayed
            string messageBoxText = "You are about to shut down the system! Continue?";
            string caption = "Shut Down";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Warning;
            // Display message box
            MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);

            // Process message box results
            switch (result)
            {
                case MessageBoxResult.Yes:

                    // Stop camera 
                    myUSBcamera.StopCamera();
                    Shut();
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }

        public static void Restart()
        {
            StartShutDown("-f -r -t 5");
        }

        /// <summary>
        /// Log off.
        /// </summary>
        public static void LogOff()
        {
            StartShutDown("-l");
        }

        /// <summary>
        ///  Shutting Down Windows 
        /// </summary>
        public static void Shut()
        {
            StartShutDown("-f -s -t 5");
        }

        private static void StartShutDown(string param)
        {
            ProcessStartInfo proc = new ProcessStartInfo();
            proc.FileName = "cmd";
            proc.WindowStyle = ProcessWindowStyle.Hidden;
            proc.Arguments = "/C shutdown " + param;
            Process.Start(proc);
        }
        #endregion

    }
}

