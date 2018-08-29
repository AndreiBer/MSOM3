using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Globalization;
using System.IO;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Basler.Pylon;
using ViewRSOM.MSOT.Hardware.ViewModels.Laser;
using ViewRSOM.MSOT.Hardware.ViewModels.Laser.Innolas;
using Laser.Parameter;
using Laser.OpoData;

namespace ViewRSOM
{
    /// <summary>
    /// Interaction logic for ViewScanningTab.xaml
    /// </summary>
    public partial class ViewScanningTab : UserControl
    {
        // define preview list of quick scan
        public static List<BitmapImage> myQuickScanItems = new List<BitmapImage>();
        //public event PropertyChangedEventHandler PropertyChanged;
        //public bool recording = false;
        //public Camera camera;
        //private PixelDataConverter converter = new PixelDataConverter();
        public ProgrammSettings myProgrammSettings = new ProgrammSettings();

        // Innolas laser object
        //Hardware.Laser.ViewModelLaer  innolasLaser;
        HandleOpoData myOpoDataHandler = new HandleOpoData();
        public ViewModelLaserInnolas my_laser = null;
        ProtocolWrapper<StandardCommandType> innolasModule = new ProtocolWrapper<StandardCommandType>(StandardCommandsDictionary.StandardCommands, StandardCommandsDictionary.StandardErrors);
        bool GoOn;
        bool OpoStatus = false;
        string commentWL;
        

        // define culture 
        CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
        NumberStyles styles = NumberStyles.AllowExponent | NumberStyles.Number;

        public ViewScanningTab()
        {
            InitializeComponent();

            my_laser = new ViewModelLaserInnolas();
            this.DataContext = myProgrammSettings;
            //this.Loaded += new RoutedEventHandler(initialize);

            // define IO-stream for acquisition status bar
            ConsoleStream.IOEventHandler.myStatusAcq += new ConsoleStream.AcqStatusEventHandler(acqProgressUpdate);

            // define IO-stream for acq preview
            ConsoleStream.IOEventHandler.myPreviewAcq += new ConsoleStream.AcqPreviewEventHandler(loadPreviewImage);

            // define IO-stream for quick scan list
            ConsoleStream.IOEventHandler.myQuickScan += new ConsoleStream.QuickScanEventHandler(showQuickScan);

            // define IO-stream for time estimate
            ConsoleStream.IOEventHandler.myTimeEstimate += new ConsoleStream.AcqTimeEstimate(showTimeEstimate);

            // define IO-stream for wlchange
            ConsoleStream.IOEventHandler.myOPOWl += new ConsoleStream.OPOWlEventHandler(OPOWlChange);

            //// start camera
            // GrabCameraImages();
        }
        


        #region event_handlers
        private void acqProgressUpdate(string sender, string receiver, string value)
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
                        acq_ProgressBar.Value = doubleParse;
                    }
                });
        }

        private void loadPreviewImage(string sender, string receiver, string value)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(
                System.Windows.Threading.DispatcherPriority.Normal, (Action)delegate
                {
                    String PreviewFile = value.Substring(13);
                    try
                    {
                        BitmapImage src = new BitmapImage();
                        src.BeginInit();
                        //src.CacheOption = BitmapCacheOption.None;
                        //src.UriCachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
                        //src.CacheOption = BitmapCacheOption.None;
                        src.CacheOption = BitmapCacheOption.OnLoad;
                        src.UriSource = new Uri(PreviewFile, UriKind.Absolute);
                        src.EndInit();
                        acq_PreviewImage.Source = src;
                        acq_PreviewImage.Stretch = Stretch.Fill;

                        // remove quick scan file
                        if (PreviewFile.EndsWith(".png"))
                            File.Delete(PreviewFile);
                    }
                    catch
                    {
                        Console.Write("ERROR: Could not load preview file " + PreviewFile + ".\n");
                    }
                });
        }

        private void showQuickScan(string sender, string receiver, string value)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(
                System.Windows.Threading.DispatcherPriority.Normal, (Action)delegate
                {
                    string QuickScanFile = value.Substring(19);
                    try
                    {
                        BitmapImage src = new BitmapImage();
                        src.BeginInit();
                        //src.CacheOption = BitmapCacheOption.None;
                        //src.UriCachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
                        //src.CacheOption = BitmapCacheOption.None;
                        src.CacheOption = BitmapCacheOption.OnLoad;
                        src.UriSource = new Uri(QuickScanFile, UriKind.Absolute);
                        src.EndInit();
                        myQuickScanItems.Add(src);
                        acquisitionParameters.NQuickScan = myQuickScanItems.Count - 1;
                        quickScanFrameNumber_Label.Content = Convert.ToString(acquisitionParameters.NQuickScan);
                        acq_PreviewImage.Source = myQuickScanItems[acquisitionParameters.NQuickScan];
                        //acq_PreviewImage.Source = src;
                        if (myQuickScanItems.Count == 1)
                        {
                            quickScanFrameNumber_Label.Visibility = Visibility.Visible;
                            quickScanNextFrame_Button.Visibility = Visibility.Visible;
                        }
                        
                        // remove quick scan file
                        if (QuickScanFile.EndsWith(".png"))
                            File.Delete(QuickScanFile);
                    }
                    catch
                    {
                        Console.Write("ERROR: Could not load quick scan file " + QuickScanFile + ".\n");
                    }
                });
        }

        private void showTimeEstimate(string sender, string receiver, string value)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(
                System.Windows.Threading.DispatcherPriority.Normal, (Action)delegate
                {
                    acq_MessageBox.Text = value;
                });
        }
        
       private void OPOWlChange(string sender, string receiver, string value)
        {
            
            //get number of wavelength from comment box
            int[] convertedItems = { Convert.ToInt32(LaserParameter.LaserDefaultWavelength) };
            if (!string.IsNullOrEmpty(commentWL))
            {
                char[] delimiterChars = { ' ', ',', '.', ':', ';', '\t' };
                string[] tokens = commentWL.Split(delimiterChars);
                convertedItems = Array.ConvertAll<string, int>(tokens, int.Parse);                
            }
            else
            {                
                my_laser.setWavelength(convertedItems[0]);
            }

            System.Windows.Application.Current.Dispatcher.Invoke(
                System.Windows.Threading.DispatcherPriority.Normal, (Action)delegate
                {
                    acq_MessageBox.Text = value;
                    if (!string.IsNullOrEmpty(commentWL))
                    {
                        
                        bool canParse2;
                        Int16 doubleParse2;
                        canParse2 = Int16.TryParse(value.Substring(16,1), styles, culture, out doubleParse2);
                        if (canParse2)
                        {
                            //doubleParse2 = doubleParse2 * 100;
                            //acq_ProgressBar.Value = doubleParse2;
                            my_laser.setWavelength(convertedItems[doubleParse2-1]);
                        }
                    }

                });
               
    
        }
        

        #endregion

        // GUI functionality for input parameters
        #region input_parameters
        private void l_x_ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            // Assign the ItemsSource to the List.
            l_x_ComboBox.ItemsSource = acquisitionParameters.x_list;

            // Make the first item selected.
            l_x_ComboBox.SelectedIndex = acquisitionParameters.l_x_listIndex;
            acquisitionParameters.l_x = acquisitionParameters.x_list[acquisitionParameters.l_x_listIndex];            
        }

        private void l_x_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // change l_x
            acquisitionParameters.l_x_listIndex = l_x_ComboBox.SelectedIndex;
            acquisitionParameters.l_x = acquisitionParameters.x_list[acquisitionParameters.l_x_listIndex];
        }

        private void l_y_ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            // Assign the ItemsSource to the List.
            l_y_ComboBox.ItemsSource = acquisitionParameters.y_list;

            // Make the first item selected.
            l_y_ComboBox.SelectedIndex = acquisitionParameters.l_y_listIndex;
            acquisitionParameters.l_y = acquisitionParameters.y_list[acquisitionParameters.l_y_listIndex];
        }

        private void l_y_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // change l_y
            acquisitionParameters.l_y_listIndex = l_y_ComboBox.SelectedIndex;
            acquisitionParameters.l_y = acquisitionParameters.y_list[acquisitionParameters.l_y_listIndex];
        }

        private void z_highValue_Loaded(object sender, RoutedEventArgs e)
        {
            // Assign the ItemsSource to the List.
            z_high_ComboBox.ItemsSource = acquisitionParameters.z_list;

            // Make the first item selected.
            z_high_ComboBox.SelectedIndex = acquisitionParameters.z_listIndex;
            acquisitionParameters.z_high = acquisitionParameters.z_list[acquisitionParameters.z_listIndex];
        }

        private void z_highValue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // change z_depth
            acquisitionParameters.z_listIndex = z_high_ComboBox.SelectedIndex;
            acquisitionParameters.z_high = acquisitionParameters.z_list[acquisitionParameters.z_listIndex];
        }
        #endregion

        private void scanName_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (scanName_TextBox.Text.Length > 0)
            {
                foreach (char c in System.IO.Path.GetInvalidFileNameChars())
                {
                    scanName_TextBox.Text = scanName_TextBox.Text.Replace(c, '_');
                }
                scanName_TextBox.Text = scanName_TextBox.Text.Replace('.', '_');
                fileParameters.dataName = scanName_TextBox.Text;
            }
            else
                fileParameters.dataName = "";
        }

        private void pulseEnergy_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool canParse;
            int intParse;
            try
            {
                canParse = Int32.TryParse(pulseEnergy_TextBox.Text, styles, culture, out intParse);
                if (!canParse || intParse<0 || intParse>100)
                {
                    throw new Exception("Input of laser power invalid.");
                }
                else
                {
                    acquisitionParameters.laserPower = intParse;
                }
            }
            catch
            {
                acquisitionParameters.laserPower = 0;
                pulseEnergy_TextBox.Text = "0";
                MessageBox.Show("Power setting must be an integer between 0-100. " + pulseEnergy_TextBox.Text + " is not valid. Setting to 0.", this.Name, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Camera frame andscroll through Quick scan
        #region camera_and_quickScan
        private void QuickScanNextFrame_Click(object sender, RoutedEventArgs e)
        {
            if (myQuickScanItems.Count > 0)
            {
                acquisitionParameters.NQuickScan = (acquisitionParameters.NQuickScan + 1) % (myQuickScanItems.Count);
                quickScanFrameNumber_Label.Content = Convert.ToString(acquisitionParameters.NQuickScan);
                acq_PreviewImage.Source = myQuickScanItems[acquisitionParameters.NQuickScan];
                //acqPreviewImage.Stretch = Stretch.UniformToFill;
            }
        }

        //void GrabCameraImages()
        //{
        //    recording = true;

        //    System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(delegate (object o)
        //    {
        //        try
        //        {
        //            camera = new Camera();
        //            // Create a camera object that selects the first camera device found.
        //            // More constructors are available for selecting a specific camera device.
        //            //using (Camera camera = new Camera())
        //            {
        //                // Print the model name of the camera.
        //                Console.WriteLine("Using camera {0}.", camera.CameraInfo[CameraInfoKey.ModelName]);

        //                // Set the acquisition mode to free running continuous acquisition when the camera is opened.
        //                camera.CameraOpened += Configuration.AcquireContinuous;

        //                // Open the connection to the camera device.
        //                camera.Open();

        //                // The parameter MaxNumBuffer can be used to control the amount of buffers
        //                // allocated for grabbing. The default value of this parameter is 10.
        //                camera.Parameters[PLCameraInstance.MaxNumBuffer].SetValue(5);

        //                // Start grabbing.
        //                camera.StreamGrabber.Start();

        //                BitmapSource bmpSource;
        //                // int counter = 0;

        //                // Grab a number of images.
        //                while (recording)
        //                {
                            
        //                    //if (counter % 100 == 0)
        //                    //{
        //                    //    System.GC.Collect();
        //                    //    System.GC.WaitForPendingFinalizers();
        //                    //}

        //                    // Wait for an image and then retrieve it. A timeout of 5000 ms is used.
        //                    IGrabResult grabResult = camera.StreamGrabber.RetrieveResult(5000, TimeoutHandling.ThrowException);
        //                    using (grabResult)
        //                    {

        //                        // counter++;

        //                        // Image grabbed successfully?
        //                        if (grabResult.GrabSucceeded)
        //                        {
        //                            // Access the image data.
        //                            int stride = (int)grabResult.ComputeStride();
        //                            byte[] buffer = grabResult.PixelData as byte[];

        //                            // new buffer for format conversion
        //                            byte[] new_buffer = new byte[grabResult.Width * grabResult.Height * 3];
        //                            int new_stride = 3 * stride;

        //                            // pixel conversion from Bayer to rgb 
        //                            converter.OutputPixelFormat = PixelType.RGB8packed;
        //                            converter.Convert<byte>(new_buffer, grabResult);//converter.Convert(buffer, grabResult);

        //                            // create Bitmap
        //                            bmpSource = BitmapSource.Create(grabResult.Width, grabResult.Height, 0, 0,
        //                            PixelFormats.Rgb24, null, new_buffer, new_stride);
        //                            bmpSource.Freeze();
                                    
        //                            // show
        //                            System.Windows.Application.Current.Dispatcher.Invoke(
        //                             System.Windows.Threading.DispatcherPriority.DataBind, (Action)delegate
        //                             {
        //                                 myCameraImage.Source = bmpSource;
        //                             });

        //                            // System.GC.Collect();
        //                            // System.GC.WaitForPendingFinalizers();
                                    
        //                        }
        //                        else
        //                        {
        //                            Console.WriteLine("Camera Error: {0} {1}", grabResult.ErrorCode, grabResult.ErrorDescription);
        //                        }
        //                    }

        //                }

        //                // Stop grabbing.
        //                camera.StreamGrabber.Stop();

        //                // Close the connection to the camera device.
        //                camera.Close();


        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            Console.Error.WriteLine("Exception: {0}", e.Message);

        //            // show laser warning sign --> no camera means clinical version
        //            BitmapImage src = new BitmapImage();
        //            src.BeginInit();
        //            src.CacheOption = BitmapCacheOption.OnLoad;
        //            src.UriSource = new Uri("pack://application:,,,/ViewMSOTc/ViewsOAM/LaserSafetyImageWearGoggles.png");
        //            // src.UriSource = new Uri("H:\\!Repositories\\com.itheramedical.RSOM\\!v2_ViewRSOM\\ViewRSOM\\ViewRSOM\\ViewMSOTc\\ViewsOAM\\LaserSafetyImageWearGoggles.png", UriKind.Absolute);
        //            src.EndInit();
        //            src.Freeze();

        //            // show
        //            System.Windows.Application.Current.Dispatcher.Invoke(
        //            System.Windows.Threading.DispatcherPriority.DataBind, (Action)delegate
        //            {
        //                myCameraImage.Source = src;
        //                myCameraImage.VerticalAlignment = VerticalAlignment.Center;
        //                // myCameraImage.Stretch = Stretch.Fill;
        //             });             

        //        }
        //        finally
        //        {
        //            // Comment the following two lines to disable waiting on exit.
        //            //Console.Error.WriteLine("\nPress enter to exit.");
        //            //Console.ReadLine();
        //        }

        //    }));

        //    //Environment.Exit(exitCode);
        //}

        //public void StopCamera()
        //{
        //    try
        //    {
        //        // Stop grabbing.
        //        camera.StreamGrabber.Stop();

        //        // Close the connection to the camera device.
        //        camera.Close();
        //    }
        //    catch
        //    {

        //    }
        //}
        #endregion

        // GUI functionality for quick scan and full scan
        private void quickScan_Button_Click(object sender, RoutedEventArgs e)
        {
            if (fileParameters.studyFolder == null)
            {
                acq_MessageBox.Text = "Choose study first! \n";
            }
            else
            {

                string comment = comment_TextBox.Text;
                commentWL = comment;
                // Prepare GUI
                acq_MessageBox.Text = "Starting quick scan \n";
                acq_PreviewImage.Source = null;
                myQuickScanItems.Clear();
                quickScanFrameNumber_Label.Visibility = Visibility.Collapsed;
                quickScanNextFrame_Button.Visibility = Visibility.Collapsed;
                
                // update GUI buttons
                systemState.acqThreadFree = false;
                acq_ProgressBar.Value = 0;
                acq_ProgressBar.Foreground = Brushes.LimeGreen;

                // Start laser

                if (acquisitionParameters.laserSourceIndex == 0)
                {
                    Hardware.LaserSW.ViewModelBrightSolutions ondaLaser = new Hardware.LaserSW.ViewModelBrightSolutions();

                    try
                    {
                        // control laser
                        if (systemState.LASERconnected == 1 || systemState.LASERconnected == 2)
                        {
                            // setup laser -> initialize and switch on
                            ondaLaser.setup();

                            // allow acq to be cancelled
                            ondaLaser.laserHandle = (innersender, args) => cancelAcq_Button_Click(innersender, args, ondaLaser);
                            cancelAcq_Button.Click += ondaLaser.laserHandle;

                            // start acquisition
                            Acquisition.initQuickScan newQuickScan = new Acquisition.initQuickScan();
                            System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(delegate(object o)
                            {
                                newQuickScan.start(ondaLaser, cancelAcq_Button);
                            }));

                        }
                    }
                    catch (Exception err)
                    {
                        // check if laser emission can be stopped
                        if (ondaLaser != null)
                        {
                            try
                            {
                                ondaLaser.EmissionOFF();
                                ondaLaser.closeConnection();
                                cancelAcq_Button.Click -= ondaLaser.laserHandle;
                            }
                            catch
                            {
                                Console.WriteLine("WARNING: switch off laser EMISSION if necessary.");
                            };
                        }

                        if (!err.Message.Contains("ERROR:"))
                        {
                            Console.WriteLine("ERROR:" + err.Message);
                        }
                        Console.WriteLine("\n");
                        Console.WriteLine("Status-Acq: 1.00");
                    }
                }
                else
                {
                    if (OpoStatus == false)
                    {
                        acq_MessageBox.Text = "Turn on OPO! \n";
                    }
                    else
                    {

                        //ViewRSOM.MSOT.Hardware.ViewModels.Laser.ViewModelLaserInnolas multiLaser = new ViewRSOM.MSOT.Hardware.ViewModels.Laser.ViewModelLaserInnolas();
                        //my_laser = new ViewModelLaserInnolas();
                        //my_laser.AfterInitialize();
                        myOpoDataHandler.ClearData();
                        //myCoherent.ClearData();

                        myOpoDataHandler.myOpoMapData.Clear();
                        //myCoherent.myCohMapData.Clear();
                        //my_wavelengths.Clear();
                        //wl = Int32.Parse(wlBox.Text);
                        string StatusMessage;
                        StatusMessage = my_laser.CheckShutterState();
                        int wl = 550;
                        my_laser.setWavelength(wl);
                        Thread.Sleep(50);
                        my_laser.lamp(true);
                        Thread.Sleep(500);
                        my_laser.AcceptTriggerChangeAndAttenuationCommands = true;
                        LaserParameter.PowerControlMethod = "None";
                        my_laser.setAttenuationViaPockelScell(myProgrammSettings.DefaultPockelsCellDelay);
                        my_laser.q_switch(true);
                        Thread.Sleep(3000);
                        //my_laser.q_switch(false);
                        // wavingThread.Abort stop
                        // my_laser.fastScan();

                         
                       StatusMessage= my_laser.CheckShutterState();
                       while (StatusMessage == "CLOSE")
                       {
                          // errorCode = innolasModule.ExchangeCommand(StandardCommandType.GetShutterState, "", out receivedCommands, out message);
                           StatusMessage = my_laser.CheckShutterState(); ;
                           Thread.Sleep(100);
                       }
                         

                        
                       // allow acq to be cancelled
                       //myOpoDataHandler = (innersender, args) => cancelAcq_Button_Click(innersender, args, multiLaser);
                       //cancelAcq_Button.Click += ondaLaser.laserHandle;
                       

                       // start acquisition
                       Acquisition.initQuickScan newQuickScan = new Acquisition.initQuickScan();
                       if (comment == null)
                           comment = "no comment";
                       System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(delegate(object o)
                       {
                           newQuickScan.startMulti(comment, my_laser, cancelAcq_Button);
                       }));
                         
                    }
                }
            }
        }

        private void fullScan_Button_Click(object sender, RoutedEventArgs e)
        {
            if (fileParameters.studyFolder == null)
            {
                acq_MessageBox.Text = "Choose study first! \n";
            }
            else
            {
                acq_MessageBox.Text = "Starting full scan \n";
                quickScanFrameNumber_Label.Visibility = Visibility.Collapsed;
                quickScanNextFrame_Button.Visibility = Visibility.Collapsed;
                quickScan_Button.IsEnabled = false;
                fullScan_Button.IsEnabled = false;
                systemState.acqThreadFree = false;
                acq_ProgressBar.Value = 0;
                acq_ProgressBar.Foreground = Brushes.LimeGreen;
                string comment = comment_TextBox.Text;
                commentWL = comment;

                // Start laser

                if (acquisitionParameters.laserSourceIndex == 0)
                {
                    Hardware.LaserSW.ViewModelBrightSolutions ondaLaser = new Hardware.LaserSW.ViewModelBrightSolutions();

                    try
                    {
                        // control laser
                        if (systemState.LASERconnected == 1 || systemState.LASERconnected == 2)
                        {
                            // setup laser -> initialize and switch on
                            ondaLaser.setup();

                            // allow acq to be cancelled
                            ondaLaser.laserHandle = (innersender, args) => cancelAcq_Button_Click(innersender, args, ondaLaser);
                            cancelAcq_Button.Click += ondaLaser.laserHandle;

                            // define comment
                            Acquisition.initFullScan newFullScan = new Acquisition.initFullScan();
                            //string comment = comment_TextBox.Text;
                            if (comment == null)
                                comment = "no comment";

                            // start acquisition
                            System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(delegate(object o)
                            {
                                myQuickScanItems.Clear();
                                newFullScan.start(comment, ondaLaser, cancelAcq_Button);
                            }));

                        }
                    }
                    catch (Exception err)
                    {
                        // check if laser emission can be stopped
                        if (ondaLaser != null)
                        {
                            try
                            {
                                ondaLaser.EmissionOFF();
                                ondaLaser.closeConnection();
                                cancelAcq_Button.Click -= ondaLaser.laserHandle;
                            }
                            catch
                            {
                                Console.WriteLine("WARNING: switch off laser EMISSION if necessary.");
                            };
                        }

                        if (!err.Message.Contains("ERROR:"))
                        {
                            Console.WriteLine("ERROR:" + err.Message);
                        }
                        Console.WriteLine("\n");
                        Console.WriteLine("Status-Acq: 1.00");
                    }
                }
                else
                { 
                    if (OpoStatus == false)
                    {
                        acq_MessageBox.Text = "Turn on OPO! \n";
                    }
                    else
                    {

                        //ViewRSOM.MSOT.Hardware.ViewModels.Laser.ViewModelLaserInnolas multiLaser = new ViewRSOM.MSOT.Hardware.ViewModels.Laser.ViewModelLaserInnolas();
                        //my_laser = new ViewModelLaserInnolas();
                        //my_laser.AfterInitialize();
                        myOpoDataHandler.ClearData();
                        //myCoherent.ClearData();

                        myOpoDataHandler.myOpoMapData.Clear();
                        //myCoherent.myCohMapData.Clear();
                        //my_wavelengths.Clear();
                        //wl = Int32.Parse(wlBox.Text);
                        string StatusMessage;
                        StatusMessage = my_laser.CheckShutterState();
                        //int wl = 550;

                        my_laser.setWavelength(Convert.ToInt32(LaserParameter.LaserDefaultWavelength));
                        Thread.Sleep(50);
                        my_laser.lamp(true);
                        Thread.Sleep(500);
                        my_laser.AcceptTriggerChangeAndAttenuationCommands = true;
                        LaserParameter.PowerControlMethod = "None";
                        my_laser.setAttenuationViaPockelScell(myProgrammSettings.DefaultPockelsCellDelay);
                        my_laser.q_switch(true);
                        Thread.Sleep(3000);
                        //my_laser.q_switch(false);
                        // wavingThread.Abort stop
                        // my_laser.fastScan();

                         
                       StatusMessage= my_laser.CheckShutterState();
                       while (StatusMessage == "CLOSE")
                       {
                          // errorCode = innolasModule.ExchangeCommand(StandardCommandType.GetShutterState, "", out receivedCommands, out message);
                           StatusMessage = my_laser.CheckShutterState(); ;
                           Thread.Sleep(100);
                       }
                         

                        
                       // allow acq to be cancelled
                       //myOpoDataHandler = (innersender, args) => cancelAcq_Button_Click(innersender, args, multiLaser);
                       //cancelAcq_Button.Click += ondaLaser.laserHandle;                      

                       // define comment
                       Acquisition.initFullScan newFullScan = new Acquisition.initFullScan();
                       //string comment = comment_TextBox.Text;
                       if (comment == null)
                           comment = "no comment";

                       // start acquisition
                       System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(delegate(object o)
                       {
                           myQuickScanItems.Clear();
                           newFullScan.startMulti(comment, my_laser, cancelAcq_Button);
                       }));
                         
                    }

                }
            }            
        }


        private void cancelAcq_Button_Click(object sender, RoutedEventArgs e, Hardware.LaserSW.ViewModelBrightSolutions ondaLaser)
        {
            // Configure the message box to be displayed
            // string messageBoxText = "Please press the EMISSION button on the laser control box OR release the foot pedal.";
            // MessageBoxButton button = MessageBoxButton.OK;
            string messageBoxText = "Do you want to cancael the acqusition? Continue?";
            string caption = "Cancel acquisition";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Warning;

            // Display message box
            MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);

            // Process message box results
            switch (result)
            {
                case MessageBoxResult.Yes:
                    
                    try
                    {
                        // control laser
                        if (systemState.LASERconnected == 1 || systemState.LASERconnected == 2)
                        {
                            // switch laser off and close connection
                            ondaLaser.EmissionOFF();
                        } 
                        // turn off OPO
                        if (OpoStatus != false)
                        {
                            offOPOMethod();
                            OpoStatus = false;
                        }

                    }
                    catch (Exception err)
                    {
                        Console.WriteLine("WARNING: switch off laser EMISSION manually OR release foot pedal.");
                    }

                    break;

                case MessageBoxResult.No:
                    break;
            }
        }

        private void laserSource_ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            // list of laser sources
            laserSource_ComboBox.ItemsSource = acquisitionParameters.laserSource_list;
                //= acquisitionParameters.laserSource_list.Add("Single Wavelength");

            // Make the first item selected
            laserSource_ComboBox.SelectedIndex = acquisitionParameters.laserSourceIndex;
            acquisitionParameters.laserSource = acquisitionParameters.laserSource_list[acquisitionParameters.laserSourceIndex];
            
        }

        private void laserSource_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // choose a laser
            acquisitionParameters.laserSourceIndex = laserSource_ComboBox.SelectedIndex;
            acquisitionParameters.laserSource = acquisitionParameters.laserSource_list[acquisitionParameters.laserSourceIndex];
              if (acquisitionParameters.laserSourceIndex == 1)
                {
                    // Start OPO laser
                    if (OpoStatus == false)
                    {
                        StartOPOMethod();
                        OpoStatus = true;
                    }
                }
        }

        private void StartOPO_Click(object sender, RoutedEventArgs e)
        {
            if (OpoStatus == false)
            { 
                StartOPOMethod();
                OpoStatus = true;
                
            }
            else
            {
                offOPOMethod();
                OpoStatus = false;
            }
        }


        public void StartOPOMethod()
        {
            
                //StartOPO.Text == "00OPO on";
                
                my_laser = new ViewModelLaserInnolas();
                my_laser.AfterInitialize();
                GoOn = false;
                my_laser.compositeInit();

                System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(delegate(object o)
                {
                    while (!GoOn)
                    {
                        Thread.Sleep(5000);
                        my_laser.GetLaserState();
                    }
                }));

                if (myProgrammSettings.LaserError == 7) // if Laser in State CBANK.OFF
                    my_laser.GetLaserState();
                Thread.Sleep(1000);
                StartOPO.Content = "ON";
            }        
            

            public void offOPOMethod()
            {
                        try
                        {
                            my_laser.compositeClose();
                            StartOPO.Content = "OFF";
                        }
                        catch { System.Windows.MessageBox.Show("Not possible"); }
            }


        #region eventhandler

          

        private void handleLaserErrorMessage(string sender, string receiver, string message)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(delegate(object o)
            {
                try
                {
                    string log_filename = myProgrammSettings.Folder + "\\" + myProgrammSettings.LogFileName;
                    //myFilewriter.writeStringToFile(log_filename, DateTime.Now + ":  LASER ERROR MESSAGE -> " + message);
                }
                catch { }
            }));

        }


        private void handleLaserStatusMessage(string sender, string receiver, string message)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(
            System.Windows.Threading.DispatcherPriority.Normal, (Action)delegate
            {
                if (receiver == "GUI")
                {
                    if (sender == "LASER_STATE")
                    {
                        switch (message)
                        {
                            case "IPConnectionOK":
                                myProgrammSettings.LaserError = 2; // connected but not initialized
                                

                                //ConnectLaserButton.IsEnabled = false;
                                break;
                            case "NoIPConnection":
                                myProgrammSettings.LaserError = 1; // not connected, off
                               
                                System.Windows.MessageBox.Show("Could not connect to Laser. Check Ip and Port");
                                
                                break;
                            case "InitTaskException":
                                myProgrammSettings.LaserError = 8; // init Task failed;
                                
                                GoOn = true;
                                break;
                            case "InitTaskComplete":
                                myProgrammSettings.LaserError = 3; // initialized but not ready (warm up)                                
                                GoOn = true;
                                break;
                            case "INITDONE":
                                myProgrammSettings.LaserError = 3; // initialized but not ready (warm up)                               
                                GoOn = true;
                                break;
                            case "LaserWarmingUp":
                                myProgrammSettings.LaserError = 3; // initialized but not ready (warm up)
                                
                                break;
                            case "InitTaskNotCompleted":
                                myProgrammSettings.LaserError = 2; // connected but not initialized
                                
                                my_laser.GetLaserState();
                                //System.Windows.MessageBox.Show("Could not complete Laser Initialization.");
                                break;
                            case "LaserReady":
                                myProgrammSettings.LaserError = 0; // Laser Ready
                                myProgrammSettings.LaserReady = true;
                                
                                GoOn = true;
                                
                                break;
                            case "LaserSwitchingOff":
                                myProgrammSettings.LaserError = 1; // not connected,off
                                myProgrammSettings.LaserReady = false;
                                
                                break;
                            case "LaserOff":
                                myProgrammSettings.LaserError = 5; // not connected,off
                                myProgrammSettings.LaserReady = false;
                                
                                break;
                            case "LaserCharged":
                                myProgrammSettings.LaserError = 6; // CBANK.Wait
                                myProgrammSettings.LaserReady = false;
                                
                                GoOn = true;
                                
                                if (LaserParameter.HW_Version == "not defined")
                                {
                                    System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(delegate(object o)
                                    {
                                        try
                                        {
                                            my_laser.GetVersionInformation();
                                        }
                                        catch { }
                                    }));
                                }

                                break;
                            case "LaserConnectedNotCharged":
                                myProgrammSettings.LaserError = 7; // CBANK.Off
                                myProgrammSettings.LaserReady = false;
                                
                                GoOn = true;
                                
                                my_laser.StartChargerChange();
                                break;


                            //LaserSatusMessage
                            //OPO3data
                            //LaserAbbruch
                        }

                    }
                }
                if (receiver == "VERSION_NUMBERS")
                {
                    switch (sender)
                    {
                        case "CONTROLLER_SW_VERSION":
                            LaserParameter.Controller_SW_Version = message;
                            break;
                        case "FPGA SW VERSION":
                            LaserParameter.FPGA_SW_Version = message;
                            break;
                        case "HW VERSION":
                            LaserParameter.HW_Version = message;
                            break;
                        case "GUI VERSION":
                            LaserParameter.GUI_Version = message;
                            break;
                        case "PROXY VERSION":
                            LaserParameter.Proxy_Version = message;
                            break;
                        case "OPO VERSION":
                            LaserParameter.Opo_Version = message;
                            break;
                        case "ENERGY MONITOR VERSION":
                            LaserParameter.Energy_Monitor_Version = message;
                            break;

                    }
                    
                }


                if (sender == "SWEEP_STATE")
                {
                    switch (message)
                    {
                        case "SETUP INCOMPLETE":

                            break;
                        case "CHECKSUM COMPUTING -ONGOING":
                            GoOn = false;
                            break;
                        case "CHECKSUM INVALID":

                            break;
                        case "SWEEP_POSSIBLE":
                            GoOn = true;

                            break;
                        case "SWEEP PAUSED":

                            break;


                    }
                    
                }


                if (receiver == "COHERENT" && sender == "OPO_DATA_HANDLER")
                {
                    string wl_changed = "NEW_WL_INDEX=";
                    if (message.StartsWith(wl_changed))
                    {
                        // Ermittle Wellenlänge

                        string[] stringSeparators = new string[] { "," };
                        string[] values = message.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
                        string msgBody = values[0].Replace(wl_changed, "");
                        int real_shotcount = Convert.ToInt32(values[1]);                        

                        //setzte neue Wellenlänge im energy meter


                        string Coh_answer = "";
                        
                        DateTime coh_time = DateTime.Now;


                        // alles weitere in neuen Thread auslagern
                        System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(delegate(object o)
                        {
                            // Initialisiere Variablen                            
                            double actualAttenuation = 0;
                            double mean_coh = 0;
                            double mean_opo = 0;
                            double opo_stdv = 0;

                            // erzeuge neue OpoMap Daten
                            List<OPOData> actualOPOData = myOpoDataHandler.showRecordedData(myOpoDataHandler.showRecordedData().Count - myProgrammSettings.ShotsPerPoint - 1, myOpoDataHandler.showRecordedData().Count - 1);

                            



                            // erzeuge neue OpoMap Daten

                            //myOpoDataHandler.processOpoData(myOpoDataHandler.showRecordedData(myOpoDataHandler.showRecordedData().Count - myProgrammSettings.ShotsPerPoint - 1, myOpoDataHandler.showRecordedData().Count - 1), my_wavelengths[last_WL_index]);
                            // erzeuge neue CohMap Daten
                            // !! DER COUNTER wird von den OPO-DATEn übernommen, da evtl. schon wesentlich mehr Energy-Meter-Daten aufgelaufen sind, und somit die Zuordnung nicht mehr stimmen würde 
                            // da evtl ein energy meter value weniger aufgenommen wurde (timedelay) wird auch der index hier um eins niedriger angesetzt(-2)

                            //ent.processCohData(actualWlData, my_wavelengths[last_WL_index]);

                            // save mean value and StDv of collected Coherent Data to Coherent Map 
                            double meanCoh = 0;//ent.myCohMapData[myCoherent.myCohMapData.Count - 1].MeanEnergy;
                            double stdvCoh = 0;// myCoherent.myCohMapData[myOpoDataHandler.myOpoMapData.Count() - 1].Stdv;
                           

                            // check max Energy Value                  
                            double actualMaxEnergy = 0;
                            double actualMinEnergy = 1000;
                            try
                            {
                                
                            }
                            catch
                            { }

                            
                            if (myProgrammSettings.UseWithoutEnergyMeter)
                            {
                                meanCoh = 0;
                                stdvCoh = 1;
                            }

                            System.Windows.Application.Current.Dispatcher.Invoke(
                            System.Windows.Threading.DispatcherPriority.Normal, (Action)delegate
                            {
                                //OutputTextBox.Text += my_wavelengths[last_WL_index] + "nm; " + Math.Round(meanCoh, 2) + "(+/- " + Math.Round(stdvCoh, 2) + ")mJ\n";
                                //OutputTextBox.ScrollToEnd();

                                

                                //display coh value
                                //int posX = Convert.ToInt32(((double)(my_wavelengths[last_WL_index] - myProgrammSettings.MinWavelength) / (myProgrammSettings.MaxWavelength - myProgrammSettings.MinWavelength)) * 600);
                                int posY = 0;
                                if (!myProgrammSettings.UseWithoutEnergyMeter)
                                {
                                    try
                                    {
                                        posY = Convert.ToInt32((200 - (meanCoh / myProgrammSettings.EnergyMeterScaleMax) * 200));
                                        //EnergyCanvas.Children.Add(myShapes.myPhi(Convert.ToInt32(stdvCoh / myProgrammSettings.EnergyMeterScaleMax * 200), posX + 9, posY + 10, "WL: " + my_wavelengths[last_WL_index] + "nm; E:" + Math.Round(meanCoh, 2) + "mJ; Stdv: " + Math.Round(stdvCoh, 2) + "mJ"));
                                    }
                                    catch
                                    {
                                        posY = Convert.ToInt32((200 - (0 / myProgrammSettings.EnergyMeterScaleMax) * 200));
                                        //EnergyCanvas.Children.Add(myShapes.myPhi(Convert.ToInt32(1 / myProgrammSettings.EnergyMeterScaleMax * 200), posX + 9, posY + 10, "WL: " + my_wavelengths[last_WL_index] + "nm; E:Measurement Failure"));
                                    }

                                }

                                else if (myProgrammSettings.UseWithoutEnergyMeter)
                                {
                                    posY = 1;
                                    //EnergyCanvas.Children.Add(myShapes.myPhi(Convert.ToInt32(stdvCoh / myProgrammSettings.EnergyMeterScaleMax * 200), posX + 9, posY, "WL: " + my_wavelengths[last_WL_index] + "nm; (Measurement Mode without Energy Meter)"));
                                }

                                //display opo value
                                //int OpoPosX = Convert.ToInt32(((double)(my_wavelengths[last_WL_index] - myProgrammSettings.MinWavelength) / (myProgrammSettings.MaxWavelength - myProgrammSettings.MinWavelength)) * 600);
                                //int OpoPosY = Convert.ToInt32((200 - (meanOpo / myProgrammSettings.defaultOpoScaleMax) * 200));
                                //EnergyMonitorCanvas.Children.Add(myShapes.myPhi(Convert.ToInt32(stdvOpo / myProgrammSettings.defaultOpoScaleMax * 200), OpoPosX + 9, OpoPosY + 10, "WL: " + my_wavelengths[last_WL_index] + "nm; Value:" + Math.Round(meanOpo, 2) + "; Stdv: " + Math.Round(stdvOpo, 2) + ""));

                                ////

                                
                            });

                            // create and fill Data file for actual wavelength
                            

                            
                            

                                //myCoherent.stopMeasuring();
                                try
                                {
                                    my_laser.q_switch(false);
                                    my_laser.lamp(false);
                                }
                                catch { }
                                System.Windows.Application.Current.Dispatcher.Invoke(
                                System.Windows.Threading.DispatcherPriority.Normal, (Action)delegate
                                {
                                    System.Windows.MessageBox.Show("Done!");
                                    /*
                                    ProgressLabel.Content = "Measurement Done!";
                                    LogTextBox.Text += DateTime.Now.ToString("o") + " : SCAN ENDED\n";
                                    myProgressbar.Value = 100;
                                    ShowResult();
                                    enableAllGuiControls();
                                     */
                                });

                                

                            


                        }));


                    }



                }
                //LogTextBox.Text += data.Timestamp + ": " + data.Energy + "\n";

            });

        }

        #endregion eventhandler

        #region initialise and close

        void initialize(object sender, RoutedEventArgs e)
        {

            int error;

            //XmlDocument xml = new XmlDocument();
            // Read settings File(xml)
            
            try
            {                

                my_laser = new ViewModelLaserInnolas();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Not able to read data from Settings file:" + ex);
            }

          



        }

        

        #endregion initialise and close

        private void comment_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        

    }
}
