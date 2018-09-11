using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Globalization;
using System.Threading;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;



namespace ViewRSOM
{
    /// <summary>
    /// Interaction logic for ViewUnmixingTab.xaml
    /// </summary>
    public partial class ViewUnmixingTab : UserControl
    {
        // define culture 
        CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
        NumberStyles styles = NumberStyles.AllowExponent | NumberStyles.Number;
        // define recon list item
        private static List<Unmixing.UnmixItem> myUnmixItems = new List<Unmixing.UnmixItem>();
        private static List<Unmixing.CompItem> myCompItems = new List<Unmixing.CompItem>();
        private static List<string> unmixFolders_list = new List<string>();

        public ViewUnmixingTab()
        {
            InitializeComponent();
            // define IO-stream for wlchange
            ConsoleStream.IOEventHandler.myIOEvent  += new ConsoleStream.CharEventHandler(ErrorLog);
            //ConsoleStream.IOEventHandler.myStatusUnmix += new ConsoleStream.UnmixStatusEventHandler(UnmixProgressUpdate);

        }

        // First Panel with files to unmix
        #region Files_to_unmix
        public void studyList_Updated()
        {
            // list of reconstruction folders
            unmixFolders_list.Clear();

            for (int i_date = 0; i_date < studyParameters.myStudyDates_list.Count; i_date++)
            {
                string directoryEntry = studyParameters.myStudyDates_list[i_date].folderPath.Split('\\')[studyParameters.myStudyDates_list[i_date].folderPath.Split('\\').Length - 1];
                unmixFolders_list.Add(directoryEntry);
            }

            // Assign the ItemsSource to the List.
            studyDate_ComboBox.ItemsSource = unmixFolders_list;
            studyDate_ComboBox.SelectedIndex = studyParameters.myStudyDates_listIndex;
            studyDate_ComboBox.Items.Refresh();

            // load unmix files combo box
            load_unmixItems();
            // load unmix components 
            load_unmixCompItems();

        }

        private void load_unmixItems()
        {


            string[] dateFolderEntries = Directory.GetDirectories(fileParameters.studyFolder).ToArray();
            //myReconFolders_list
            for (int i_date = 0; i_date < dateFolderEntries.Length; i_date++)
            {
                // list of acquisition files
                string[] acqFileEntries = Directory.GetFiles(dateFolderEntries[i_date], "*.mat").Select(System.IO.Path.GetFileNameWithoutExtension).ToArray();


                // list of reconstruction folders
                string[] reconFolderEntries = Directory.GetDirectories(dateFolderEntries[i_date]).ToArray();

                // run through all reconstructed files found
                
                // initialize acq file list
                List<acqFileItem> myAcqFiles_list = new List<acqFileItem>();
                // initialize recon file list
                List<reconFileItem> myReconFiles_list = new List<reconFileItem>();
                List<unmixFileItem> myUnmixFiles_list = new List<unmixFileItem>();

                // run through all acquisition files found
                for (int i_acq = 0; i_acq < acqFileEntries.Length; i_acq++)
                {
                    // initialize recon list
                   // List<unmixFileItem> myUnmixFolders_list = new List<unmixFileItem>();

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
                                if (reconFolderWithoutPath.Contains("OPO"))
                                {
                                    string[] reconFileEntries = Directory.GetFiles(reconFolderEntries[i_recon], "*.mat").Select(System.IO.Path.GetFileNameWithoutExtension).ToArray();
                                    for (int i_recon_files = 0; i_recon_files < reconFileEntries.Length; i_recon_files++)
                                    {
                                        if (reconFileEntries[i_recon_files].Contains("OPO"))
                                        {
                                            //unmixFileItem myUnmixItems = new unmixFileItem(i_recon, reconFileEntries[i_recon_files], reconFolderEntries[i_recon], true);
                                            // path to reconstructed file
                                            myUnmixItems.Add(new Unmixing.UnmixItem(i_acq, reconFolderEntries[i_recon], reconFileEntries[i_recon_files], false));

                                            //myUnmixFiles_list.Add(myUnmixItems);
                                        }
                                       
                                    }

                                    UnmixFiles_ListBox.ItemsSource = myUnmixItems;
                                    UnmixFiles_ListBox.Items.Refresh();
                                    
                                }
                            }
                        }

                    }
                }
            }
            //if (studyParameters.myStudyDates_listIndex >= 0)
            //    loadReconThumbnails();

        }
        #endregion


        private void studyDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void UnmixFiles_CheckBox_Click(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            var item = cb.DataContext;
            UnmixFiles_ListBox.SelectedItem = item;
            //if (studyParameters.myStudyDates_listIndex >= 0)
            //    loadReconThumbnails();
        }

        private void export_Button_Click(object sender, RoutedEventArgs e)
        {            
        }

        private void unmix_Button_Click(object sender, RoutedEventArgs e)
        {
            // initialize and start recon thread
            
                Thread unmixThread = new Thread(new ThreadStart(initUnmixThread));
                unmixThread.IsBackground = true;
                unmixThread.Start();

                // allow thread to be cancelled
                systemState.unmixHandle = (innersender, args) => cancelUnmix_Button_Click(innersender, args, unmixThread);
                cancelUnmix_Button.Click += systemState.unmixHandle;
                unmix_MessageBox.Text = "Starting unmixing routine \n";
                //unmix_ProgressBar.Value = 1;
                //unmix_ProgressBar.Foreground = Brushes.LimeGreen;
                //unmix_ProgressBarTot.Value = 1;
                //unmix_ProgressBarTot.Foreground = Brushes.LimeGreen;
                systemState.unmixThreadFree = false;
        }

        private void initUnmixThread()
        {

            try
            {
                Unmixing.initUnmix newUnmix = new Unmixing.initUnmix();
                newUnmix.start(myUnmixItems, myCompItems); 
            }
            catch (ThreadAbortException abortException)
            {
                Console.WriteLine((string)abortException.ExceptionState);
                // show message when finished
                systemState.unmixThreadFree = true;
            }
        }

        private void load_unmixCompItems()
        {
            for (int i_comp = 0; i_comp < unmixingParameters.myUnmixComponents.Count; i_comp++)
            {
                myCompItems.Add(new Unmixing.CompItem(i_comp, unmixingParameters.myUnmixComponents[i_comp], false));
            }
            UnmixComponents_ListBox.ItemsSource = myCompItems;
            UnmixComponents_ListBox.Items.Refresh();
        }

        private void UnmixFiles_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void UnmixComponents_CheckBox_Click(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            var item = cb.DataContext;
            UnmixComponents_ListBox.SelectedItem = item;
        }


        private void cancelUnmix_Button_Click(object sender, RoutedEventArgs e, Thread unmixThread)
        {
            // Configure the message box to be displayed
            string messageBoxText = "You are about to cancel the current unmixing! Continue?";
            string caption = "Cancel unmixing";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Warning;

            // Display message box
            MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);

            // Process message box results
            switch (result)
            {
                case MessageBoxResult.Yes:

                    // Configure the message box to be displayed
                    messageBoxText = "Unmixing routine will stop after the current unmixing.";
                    caption = "Cancel reconstruction";
                    button = MessageBoxButton.OK;
                    icon = MessageBoxImage.Information;
                    MessageBox.Show(messageBoxText, caption, button, icon);

                    // Stop thread
                    unmixThread.Abort();
                    break;

                case MessageBoxResult.No:
                    break;
            }
        }

        private void ErrorLog(string sender, string receiver, string value)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(
                System.Windows.Threading.DispatcherPriority.Normal, (Action)delegate
                {
                    unmix_MessageBox.Text = value; 
                });
        }




        private void loadReconThumbnails()
        {
            // clear thumbnails
           // ReconThumbnailPanel.Children.Clear();

            //// run through all acquisition files in selected study date list
            //for (int i_acq = 0; i_acq < studyParameters.myStudyDates_list[studyParameters.myStudyDates_listIndex].myAcqFiles_list.Count; i_acq++)
            //{
            //    // check if the acquisition file is checked
            //    if (studyParameters.myStudyDates_list[studyParameters.myStudyDates_listIndex].myAcqFiles_list[i_acq].isChecked)
            //    {
            //        for (int i_recon = 0; i_recon < studyParameters.myStudyDates_list[studyParameters.myStudyDates_listIndex].myAcqFiles_list[i_acq].myReconFolders_list.Count; i_recon++)
            //        {
            //            string thumbnailPath = studyParameters.myStudyDates_list[studyParameters.myStudyDates_listIndex].myAcqFiles_list[i_acq].myReconFolders_list[i_recon].folderPath
            //                    + "\\Thumbnail\\R_thumb_" + studyParameters.myStudyDates_list[studyParameters.myStudyDates_listIndex].myAcqFiles_list[i_acq].fileName;
            //            for (int i_freq = 0; i_freq < 3; i_freq++)
            //            {
            //                string extension = "";
            //                if (i_freq == 1)
            //                    extension = "_LF";
            //                if (i_freq == 2)
            //                    extension = "_HF";
            //                try
            //                {
            //                    // get source image
            //                    BitmapImage src = new BitmapImage();
            //                    src.BeginInit();
            //                    src.CacheOption = BitmapCacheOption.OnLoad;
            //                    src.UriSource = new Uri(thumbnailPath + extension + ".png", UriKind.Absolute);
            //                    src.EndInit();

            //                    // create bitmap image
            //                    Image myThumb = new Image();
            //                    myThumb.Source = src;
            //                    myThumb.Height = 140;

            //                    //create border and add image inside
            //                    Border myImage = new Border();
            //                    myImage.BorderBrush = Brushes.Transparent;
            //                    myImage.BorderThickness = new Thickness(1.5);
            //                    string nameHelp = thumbnailPath + extension;
            //                    nameHelp = nameHelp.Split('\\')[nameHelp.Split('\\').Length - 3] + nameHelp.Split('\\')[nameHelp.Split('\\').Length - 1];
            //                    nameHelp = "_" + Regex.Replace(nameHelp, @"[^a-zA-Z0-9]", "_");
            //                    myImage.Name = nameHelp;
            //                    myImage.Child = myThumb;
            //                    Thickness margin = myThumb.Margin;
            //                    margin.Left = 5;
            //                    margin.Top = 5;
            //                    margin.Right = 5;
            //                    margin.Bottom = 5;
            //                    myImage.Margin = margin;

            //                    // add to Stack panel
            //                    ReconThumbnailPanel.Children.Add(myImage);

            //                    // setup event handler when thumbnail is clicked
            //                    int iHelp_date = studyParameters.myStudyDates_listIndex;
            //                    int iHelp_acq = i_acq;
            //                    int iHelp_recon = i_recon;
            //                    int iHelp_freq = i_freq;
            //                    myImage.MouseLeftButtonDown += (sender, args) => showRecon(sender, args, iHelp_date, iHelp_acq, iHelp_recon, iHelp_freq);
            //                }
            //                catch
            //                {
            //                    if (i_freq == 0)
            //                        recon_MessageBox.Text = "ERROR: Thumbnail " + thumbnailPath + " does not exist.";
            //                }

            //            }

            //        }

            //    }

            //}

        }
    }
}
