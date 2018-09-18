using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Globalization;
using System.Threading;
using System.Text.RegularExpressions;

namespace ViewRSOM
{
    /// <summary>
    /// Interaction logic for ViewReconstructionTab.xaml
    /// </summary>
    public partial class ViewReconstructionTab : UserControl
    {
        
        // define culture 
        CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
        NumberStyles styles = NumberStyles.AllowExponent | NumberStyles.Number;

        // define recon list item
        private static List<Reconstruction.ReconItem> myReconItems = new List<Reconstruction.ReconItem>();
        private static List<string> reconFolders_list = new List<string>();

        public ViewReconstructionTab()
        {
            InitializeComponent();

            // define IO-stream for reconstruction status bar
            ConsoleStream.IOEventHandler.myStatusRecon += new ConsoleStream.ReconStatusEventHandler(reconProgressUpdate);
        }

        // event handlers for recon progress
        #region event_handlers
        private void reconProgressUpdate(string sender, string receiver, string value)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(
                System.Windows.Threading.DispatcherPriority.Normal, (Action)delegate
                {
                    bool canParse;
                    double doubleParse;
                    canParse = Double.TryParse(value.Substring(14), styles, culture, out doubleParse);
                    if (canParse)
                    {
                        // single scan progress bar
                        doubleParse = doubleParse * 100;
                        recon_ProgressBar.Value = doubleParse;

                        // total scan progress bar
                        if (reconstructionParameters.reconProgressTot[1] > 0)
                        {
                            double totProg = Convert.ToDouble(reconstructionParameters.reconProgressTot[0]) / Convert.ToDouble(reconstructionParameters.reconProgressTot[1]) * 100;
                            double totRatio = 1/Convert.ToDouble(reconstructionParameters.reconProgressTot[1]);

                            recon_ProgressBarTot.Value = Math.Min(totProg + doubleParse* totRatio, 100);
                        }
                    }
                });
        }
        #endregion

        // First Panel with files to reconstruct
        #region Files_to_reconstruct
        public void studyList_Updated()
        {
            // list of reconstruction folders
            reconFolders_list.Clear();

            for (int i_date = 0; i_date < studyParameters.myStudyDates_list.Count; i_date++)
            {
                string directoryEntry = studyParameters.myStudyDates_list[i_date].folderPath.Split('\\')[studyParameters.myStudyDates_list[i_date].folderPath.Split('\\').Length - 1];
                reconFolders_list.Add(directoryEntry);
            }

            // Assign the ItemsSource to the List.
            studyDate_ComboBox.ItemsSource = reconFolders_list;
            studyDate_ComboBox.SelectedIndex = studyParameters.myStudyDates_listIndex;
            studyDate_ComboBox.Items.Refresh();

            // load recon files combo box
            load_reconItems();

        }

        private void studyDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //// Update selected item
            //studyParameters.myStudyDates_listIndex = studyDate_ComboBox.SelectedIndex;

            //// load recon files combo box
            //load_reconItems();

            if (studyParameters.myStudyDates_listIndex == -1)
                studyParameters.myStudyDates_listIndex = studyDate_ComboBox.SelectedIndex;
            else
            {
                if (studyDate_ComboBox.SelectedIndex != -1)
                { studyParameters.myStudyDates_listIndex = studyDate_ComboBox.SelectedIndex; }
            }

        }

        private void load_reconItems()
        {
            // Reconstruction.ReconItem myReconItem = new Reconstruction.ReconItem();
            myReconItems.Clear();
            if (studyParameters.myStudyDates_listIndex >= 0)
            {
                for (int i_acq = 0; i_acq < studyParameters.myStudyDates_list[studyParameters.myStudyDates_listIndex].myAcqFiles_list.Count; i_acq++)
                {
                    bool isChecked = studyParameters.myStudyDates_list[studyParameters.myStudyDates_listIndex].myAcqFiles_list[i_acq].isChecked;
                    myReconItems.Add(new Reconstruction.ReconItem(i_acq, studyParameters.myStudyDates_list[studyParameters.myStudyDates_listIndex].myAcqFiles_list[i_acq].fileName, isChecked));
                }

                reconFiles_ListBox.ItemsSource = myReconItems;
                reconFiles_ListBox.Items.Refresh();
            }

            if (studyParameters.myStudyDates_listIndex >= 0)
                loadReconThumbnails();

        }

        public void load_reconFinished(int iHelp_date, int iHelp_acq)
        {
            // update checked data
            try
            {
                if (studyParameters.myStudyDates_listIndex == iHelp_date)
                {
                    for (int i_acq = 0; i_acq < studyParameters.myStudyDates_list[studyParameters.myStudyDates_listIndex].myAcqFiles_list.Count; i_acq++)
                    {
                        studyParameters.myStudyDates_list[studyParameters.myStudyDates_listIndex].myAcqFiles_list[i_acq].isChecked = myReconItems[i_acq].isChecked;
                    }
                    // mark reconstructed file as checked
                    myReconItems[iHelp_acq].isChecked = true;
                    reconFiles_ListBox.ItemsSource = myReconItems;
                    reconFiles_ListBox.Items.Refresh();
                    studyParameters.myStudyDates_list[studyParameters.myStudyDates_listIndex].myAcqFiles_list[iHelp_acq].isChecked = true;
                   
                    loadReconThumbnails();
                }
                else
                {
                    studyParameters.myStudyDates_listIndex = iHelp_date;
                    // Reconstruction.ReconItem myReconItem = new Reconstruction.ReconItem();
                    myReconItems.Clear();
                    for (int i_acq = 0; i_acq < studyParameters.myStudyDates_list[studyParameters.myStudyDates_listIndex].myAcqFiles_list.Count; i_acq++)
                    {
                        bool isChecked = false;
                        if (iHelp_acq == i_acq)
                            isChecked = true;
                        myReconItems.Add(new Reconstruction.ReconItem(i_acq, studyParameters.myStudyDates_list[studyParameters.myStudyDates_listIndex].myAcqFiles_list[i_acq].fileName, isChecked));
                        studyParameters.myStudyDates_list[studyParameters.myStudyDates_listIndex].myAcqFiles_list[i_acq].isChecked = isChecked;
                    }

                    reconFiles_ListBox.ItemsSource = myReconItems;
                    reconFiles_ListBox.Items.Refresh();

                    loadReconThumbnails();
                }
            }
            catch{}
        }

        private void reconFiles_CheckBox_Click(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            var item = cb.DataContext;
            reconFiles_ListBox.SelectedItem = item;

            // update check mark of study list
            for (int i_acq = 0; i_acq < studyParameters.myStudyDates_list[studyParameters.myStudyDates_listIndex].myAcqFiles_list.Count; i_acq++)
                studyParameters.myStudyDates_list[studyParameters.myStudyDates_listIndex].myAcqFiles_list[i_acq].isChecked = myReconItems[i_acq].isChecked;

            if (studyParameters.myStudyDates_listIndex >= 0)
                loadReconThumbnails();
        }
        #endregion

        // reconstructionparameters panel
        #region recon_params
        private void zRecon_highValue_Loaded(object sender, RoutedEventArgs e)
        {
            // Assign the ItemsSource to the List.
            zRecon_high_ComboBox.ItemsSource = reconstructionParameters.zRecon_list;

            // Make the first item selected.
            zRecon_high_ComboBox.SelectedIndex = reconstructionParameters.zRecon_listIndex;
            reconstructionParameters.zRecon_high = reconstructionParameters.zRecon_list[reconstructionParameters.zRecon_listIndex];
        }

        private void zRecon_highValue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // change z_depth
            reconstructionParameters.zRecon_listIndex = zRecon_high_ComboBox.SelectedIndex;
            reconstructionParameters.zRecon_high = reconstructionParameters.zRecon_list[reconstructionParameters.zRecon_listIndex];
        }

        private void bandwidth_ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            // list of reconstruction folders
            reconstructionParameters.bandpassSeparation_list.Clear();
            reconstructionParameters.bandpassSeparation_list.Add("no");
            reconstructionParameters.bandpassSeparation_list.Add("yes");
            
            // Assign the ItemsSource to the List.
            bandwidth_ComboBox.ItemsSource = reconstructionParameters.bandpassSeparation_list;

            // Make the first item selected.
            bandwidth_ComboBox.SelectedIndex = reconstructionParameters.bandpassSeparation;
        }

        private void bandwidth_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // change bandpassSeperation
            reconstructionParameters.bandpassSeparation = bandwidth_ComboBox.SelectedIndex;
        }

        private void motionCorrection_ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            // list of reconstruction folders
            reconstructionParameters.motionCorrection_list.Clear();
            reconstructionParameters.motionCorrection_list.Add("no");
            reconstructionParameters.motionCorrection_list.Add("yes");

            // Assign the ItemsSource to the List.
            motionCorrection_ComboBox.ItemsSource = reconstructionParameters.motionCorrection_list;

            // Make the first item selected.
            motionCorrection_ComboBox.SelectedIndex = reconstructionParameters.motionCorrection;
        }

        private void motionCorrection_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // change bandpassSeperation
            reconstructionParameters.motionCorrection = motionCorrection_ComboBox.SelectedIndex;
        }

        private void movingMAP_Loaded(object sender, RoutedEventArgs e)
        {
            // list of reconstruction folders
            reconstructionParameters.movingMAP_list.Clear();
            reconstructionParameters.movingMAP_list.Add("no");
            reconstructionParameters.movingMAP_list.Add("yes");

            // Assign the ItemsSource to the List.
            movingMAP_ComboBox.ItemsSource = reconstructionParameters.movingMAP_list;

            // Make the first item selected.
            movingMAP_ComboBox.SelectedIndex = reconstructionParameters.movingMAP;

        }

        private void movingMAP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // change bandpassSeperation
            reconstructionParameters.movingMAP = movingMAP_ComboBox.SelectedIndex;
        }

        private void volume_ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            // list of reconstruction folders
            reconstructionParameters.volume3D_list.Clear();
            reconstructionParameters.volume3D_list.Add("no");
            reconstructionParameters.volume3D_list.Add("yes");

            // Assign the ItemsSource to the List.
            volume_ComboBox.ItemsSource = reconstructionParameters.volume3D_list;

            // Make the first item selected.
            volume_ComboBox.SelectedIndex = reconstructionParameters.volume3D;
        }

        private void volume_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // change bandpassSeperation
            reconstructionParameters.volume3D = volume_ComboBox.SelectedIndex;

        }
        #endregion

        private void recon_Button_Click(object sender, RoutedEventArgs e)
        {
            // initialize and start recon thread
            Thread reconThread = new Thread(new ThreadStart(initReconThread));
            reconThread.IsBackground = true;
            reconThread.Start();

            // allow thread to be cancelled
            systemState.reconHandle = (innersender, args) => cancelRecon_Button_Click(innersender, args, reconThread);
            cancelRecon_Button.Click += systemState.reconHandle;

            recon_MessageBox.Text = "Starting reconstruction routine \n";
            recon_ProgressBar.Value = 1;
            recon_ProgressBar.Foreground = Brushes.LimeGreen;
            recon_ProgressBarTot.Value = 1;
            recon_ProgressBarTot.Foreground = Brushes.LimeGreen;
            systemState.reconThreadFree = false;

        }

        private void initReconThread()
        {
            try
            {

                Reconstruction.initRecon newRecon = new Reconstruction.initRecon();
                newRecon.start(myReconItems);
            }
            catch (ThreadAbortException abortException)
            {
                Console.WriteLine((string)abortException.ExceptionState);

                // show message when finished
                systemState.reconThreadFree = true;
            }
        } 

        private void cancelRecon_Button_Click(object sender, RoutedEventArgs e, Thread reconThread)
        {
            // Configure the message box to be displayed
            string messageBoxText = "You are about to cancel the current reconstruction! Continue?";
            string caption = "Cancel reconstruction";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Warning;

            // Display message box
            MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);

            // Process message box results
            switch (result)
            {
                case MessageBoxResult.Yes:

                    // Configure the message box to be displayed
                    messageBoxText = "Reconstruction routine will stop after the current reconstruction.";
                    caption = "Cancel reconstruction";
                    button = MessageBoxButton.OK;
                    icon = MessageBoxImage.Information;
                    MessageBox.Show(messageBoxText, caption, button, icon);

                    // Stop thread
                    reconThread.Abort();
                    break;

                case MessageBoxResult.No:
                    break;
            }
        }

        private void export_Button_Click(object sender, RoutedEventArgs e)
        {
            // initialize and start recon thread
            Thread exportThread = new Thread(new ThreadStart(initExportThread));
            exportThread.IsBackground = true;
            exportThread.Start();

            // allow thread to be cancelled
            systemState.reconHandle = (innersender, args) => cancelRecon_Button_Click(innersender, args, exportThread);
            cancelRecon_Button.Click += systemState.reconHandle;

            recon_MessageBox.Text = "Starting export routine \n";
            recon_ProgressBar.Value = 1;
            recon_ProgressBar.Foreground = Brushes.LimeGreen;
            recon_ProgressBarTot.Value = 1;
            recon_ProgressBarTot.Foreground = Brushes.LimeGreen;
            systemState.reconThreadFree = false;
            //System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(delegate (object o)
            //{
                
            //}));
        }

        private void initExportThread()
        {
            try
            {
                Reconstruction.initExport newExport = new Reconstruction.initExport();
                // get number of recons to export
                int N_tot = 0;
                System.Windows.Application.Current.Dispatcher.Invoke(
                System.Windows.Threading.DispatcherPriority.Normal, (Action)delegate
                {
                    N_tot = ReconThumbnailPanel.Children.Count;
                });
                newExport.start(myReconItems, N_tot);
            }
            catch (ThreadAbortException abortException)
            {
                Console.WriteLine((string)abortException.ExceptionState);

                // show message when finished
                systemState.reconThreadFree = true;
            }
        }

        // show recon thumbnails and link them to show reconstructions
        #region thumbnails
        private void loadReconThumbnails()
        {
           // clear thumbnails
            ReconThumbnailPanel.Children.Clear();
            
            // run through all acquisition files in selected study date list
            for (int i_acq = 0; i_acq < studyParameters.myStudyDates_list[studyParameters.myStudyDates_listIndex].myAcqFiles_list.Count; i_acq++)
            {
                // check if the acquisition file is checked
                if (studyParameters.myStudyDates_list[studyParameters.myStudyDates_listIndex].myAcqFiles_list[i_acq].isChecked)
                {
                    for (int i_recon = 0; i_recon < studyParameters.myStudyDates_list[studyParameters.myStudyDates_listIndex].myAcqFiles_list[i_acq].myReconFolders_list.Count; i_recon++)
                    {
                        string thumbnailPath = studyParameters.myStudyDates_list[studyParameters.myStudyDates_listIndex].myAcqFiles_list[i_acq].myReconFolders_list[i_recon].folderPath
                                + "\\Thumbnail\\R_thumb_" + studyParameters.myStudyDates_list[studyParameters.myStudyDates_listIndex].myAcqFiles_list[i_acq].fileName;
                        for (int i_freq = 0; i_freq < 3; i_freq++)
                        {
                            string extension = "";
                            if (i_freq == 1)
                                extension = "_LF";
                            if (i_freq == 2)
                                extension = "_HF";
                            try
                            {
                                // get source image
                                BitmapImage src = new BitmapImage();
                                src.BeginInit();
                                src.CacheOption = BitmapCacheOption.OnLoad;
                                src.UriSource = new Uri(thumbnailPath + extension + ".png", UriKind.Absolute);
                                src.EndInit();
                                
                                // create bitmap image
                                Image myThumb = new Image();
                                myThumb.Source = src;
                                myThumb.Height = 140;

                                //create border and add image inside
                                Border myImage = new Border();
                                myImage.BorderBrush = Brushes.Transparent;
                                myImage.BorderThickness = new Thickness(1.5);
                                string nameHelp = thumbnailPath + extension;
                                nameHelp = nameHelp.Split('\\')[nameHelp.Split('\\').Length - 3] + nameHelp.Split('\\')[nameHelp.Split('\\').Length - 1];
                                nameHelp = "_" + Regex.Replace(nameHelp, @"[^a-zA-Z0-9]", "_");
                                myImage.Name = nameHelp;
                                myImage.Child = myThumb;
                                Thickness margin = myThumb.Margin;
                                margin.Left = 5;
                                margin.Top = 5;
                                margin.Right = 5;
                                margin.Bottom = 5;
                                myImage.Margin = margin;

                                // add to Stack panel
                                ReconThumbnailPanel.Children.Add(myImage);

                                // setup event handler when thumbnail is clicked
                                int iHelp_date = studyParameters.myStudyDates_listIndex;
                                int iHelp_acq = i_acq;
                                int iHelp_recon = i_recon;
                                int iHelp_freq = i_freq;
                                myImage.MouseLeftButtonDown += (sender, args) => showRecon(sender, args, iHelp_date, iHelp_acq, iHelp_recon, iHelp_freq);
                            }
                            catch
                            {
                                if (i_freq == 0)
                                    recon_MessageBox.Text = "ERROR: Thumbnail " + thumbnailPath + " does not exist.";
                            }

                        }

                    }

                }

            }

        }

        // show reconstructions
        private void showRecon(object sender, RoutedEventArgs e, int i_date, int i_acq, int i_recon, int i_freq)
        {
            // get file name extension
            string extension = "";
            if (i_freq == 1)
                extension = "_LF";
            if (i_freq == 2)
                extension = "_HF";

            //lade Reconstruction-Images in die Tabs
            try
            {
                string xyImagePath = studyParameters.myStudyDates_list[i_date].myAcqFiles_list[i_acq].myReconFolders_list[i_recon].folderPath
                    + "\\Images\\R_xy_" + studyParameters.myStudyDates_list[i_date].myAcqFiles_list[i_acq].fileName + extension + ".png";
                BitmapImage src1 = new BitmapImage();
                src1.BeginInit();
                src1.CacheOption = BitmapCacheOption.OnLoad;
                src1.UriSource = new Uri(xyImagePath, UriKind.Absolute);
                src1.EndInit();
                xy_reconImage.Source = src1;
            }
            catch
            {

            }


            try
            {
                string yzImagePath = studyParameters.myStudyDates_list[i_date].myAcqFiles_list[i_acq].myReconFolders_list[i_recon].folderPath
                + "\\Images\\R_yz_" + studyParameters.myStudyDates_list[i_date].myAcqFiles_list[i_acq].fileName + extension + ".png";
                BitmapImage src2 = new BitmapImage();
                src2.BeginInit();
                src2.CacheOption = BitmapCacheOption.OnLoad;
                src2.UriSource = new Uri(yzImagePath, UriKind.Absolute);
                src2.EndInit();
                yz_reconImage.Source = src2;
            }
            catch
            {

            }


            try
            {
                string xzImagePath = studyParameters.myStudyDates_list[i_date].myAcqFiles_list[i_acq].myReconFolders_list[i_recon].folderPath
                + "\\Images\\R_xz_" + studyParameters.myStudyDates_list[i_date].myAcqFiles_list[i_acq].fileName + extension + ".png";
                BitmapImage src3 = new BitmapImage();
                src3.BeginInit();
                src3.CacheOption = BitmapCacheOption.OnLoad;
                src3.UriSource = new Uri(xzImagePath, UriKind.Absolute);
                src3.EndInit();
                xz_reconImage.Source = src3;
            }
            catch
            {

            }

            // update Recon LOG Message
            recon_MessageBox.Text = "Loading reconstruction file: " + studyParameters.myStudyDates_list[i_date].myAcqFiles_list[i_acq].myReconFolders_list[i_recon].folderPath
                    + "\\Images\\R_xy_" + studyParameters.myStudyDates_list[i_date].myAcqFiles_list[i_acq].fileName + extension + ".png\n";

            // highlight selected iamge
            foreach (object child in ReconThumbnailPanel.Children)
            {
                Border iThumb = (Border)child;
                Border iSend = (Border)sender;
                if (iThumb.Name.Equals(iSend.Name))
                    iThumb.BorderBrush = Brushes.LightSkyBlue;
                else
                    iThumb.BorderBrush = Brushes.Transparent;
            }

        }

        // show reconstructions - accessible from other tabs
        public void showRecon(int i_date, int i_acq, int i_recon, int i_freq)
        {
            // get file name extension
            string extension = "";
            if (i_freq == 1)
                extension = "_LF";
            if (i_freq == 2)
                extension = "_HF";

            //lade Reconstruction-Images in die Tabs
            try
            {
                string xyImagePath = studyParameters.myStudyDates_list[i_date].myAcqFiles_list[i_acq].myReconFolders_list[i_recon].folderPath
                    + "\\Images\\R_xy_" + studyParameters.myStudyDates_list[i_date].myAcqFiles_list[i_acq].fileName + extension + ".png";
                BitmapImage src1 = new BitmapImage();
                src1.BeginInit();
                src1.CacheOption = BitmapCacheOption.OnLoad;
                src1.UriSource = new Uri(xyImagePath, UriKind.Absolute);
                src1.EndInit();
                xy_reconImage.Source = src1;
            }
            catch
            {

            }


            try
            {
                string yzImagePath = studyParameters.myStudyDates_list[i_date].myAcqFiles_list[i_acq].myReconFolders_list[i_recon].folderPath
                + "\\Images\\R_yz_" + studyParameters.myStudyDates_list[i_date].myAcqFiles_list[i_acq].fileName + extension + ".png";
                BitmapImage src2 = new BitmapImage();
                src2.BeginInit();
                src2.CacheOption = BitmapCacheOption.OnLoad;
                src2.UriSource = new Uri(yzImagePath, UriKind.Absolute);
                src2.EndInit();
                yz_reconImage.Source = src2;
            }
            catch
            {

            }


            try
            {
                string xzImagePath = studyParameters.myStudyDates_list[i_date].myAcqFiles_list[i_acq].myReconFolders_list[i_recon].folderPath
                + "\\Images\\R_xz_" + studyParameters.myStudyDates_list[i_date].myAcqFiles_list[i_acq].fileName + extension + ".png";
                BitmapImage src3 = new BitmapImage();
                src3.BeginInit();
                src3.CacheOption = BitmapCacheOption.OnLoad;
                src3.UriSource = new Uri(xzImagePath, UriKind.Absolute);
                src3.EndInit();
                xz_reconImage.Source = src3;
            }
            catch
            {

            }

            // update Recon LOG Message
            try
            {
                recon_MessageBox.AppendText("Loading reconstruction file: " + studyParameters.myStudyDates_list[i_date].myAcqFiles_list[i_acq].myReconFolders_list[i_recon].folderPath
                    + "\\Images\\R_xy_" + studyParameters.myStudyDates_list[i_date].myAcqFiles_list[i_acq].fileName + extension + ".png\n");
                recon_MessageBox.ScrollToEnd();
            }
            catch
            {

            }

            // highlight selected iamge
            foreach (object child in ReconThumbnailPanel.Children)
            {
                Border iThumb = (Border)child;
                string thumbnailPath = studyParameters.myStudyDates_list[i_date].myAcqFiles_list[i_acq].myReconFolders_list[i_recon].folderPath
                                + "\\Thumbnail\\R_thumb_" + studyParameters.myStudyDates_list[i_date].myAcqFiles_list[i_acq].fileName + extension;
                string nameHelp = thumbnailPath.Split('\\')[thumbnailPath.Split('\\').Length - 3] + thumbnailPath.Split('\\')[thumbnailPath.Split('\\').Length - 1];
                nameHelp = "_" + Regex.Replace(nameHelp, @"[^a-zA-Z0-9]", "_");
                if (iThumb.Name.Equals(nameHelp))
                    iThumb.BorderBrush = Brushes.LightSkyBlue;
                else
                    iThumb.BorderBrush = Brushes.Transparent;
            }

        }
        #endregion
    }
}
