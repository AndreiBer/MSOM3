using System;
using System.Windows;
using System.Threading;
using System.Xml;
using System.Globalization;
using System.IO;
using System.Collections.Generic;

namespace ViewRSOM
{
    /// <summary>
    /// Interaction logic for SplashStartup.xaml
    /// </summary>
    public partial class SplashStartup : Window
    {
        public SplashStartup()
        {
            this.InitializeComponent();
            this.Height = System.Windows.SystemParameters.VirtualScreenHeight;

            // Insert code required on object creation below this point.
        }

        System.Windows.Threading.DispatcherTimer _delay;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // check for instances of ViewRSOM
            int instances = 0;
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcesses();
            foreach (System.Diagnostics.Process process in processes)
            {
                if (process.ProcessName.Contains("RSOM") && process.MainModule.FileVersionInfo.CompanyName.Contains("iThera"))
                {
                    if (++instances > 1)
                    {
                        break;
                    }
                }
            }
            
            // load config file
            int loadSuccess;
            loadSuccess = loadConfigFile();

            // open ViewRSOM instance if config file has been loaded and no other instance of ViewRSOM is active
            if (loadSuccess > 0)
            {
                Hide();
                Close();
            }
            else
            {
                if (instances > 1)
                {
                    Hide();
                    //Cannot use Writeline Event trigger yet for error messages
                    MessageBox.Show("An RSOM application is still active.\nPlease close before proceeding.", this.Name, MessageBoxButton.OK, MessageBoxImage.Error);
                    ConsoleStream.IOEventHandler.triggerIOEvent("SplashScreen", "EventHandler", "An RSOM application is still active.\nPlease close before proceeding.");
                    Close();
                }
                else
                {

                    _delay = new System.Windows.Threading.DispatcherTimer();
                    _delay.Tick += new EventHandler(delay_Tick);
                    _delay.Interval = new TimeSpan(100);
                    _delay.Start();
                }
            }

        }

        private int loadConfigFile()
        {

            // Open RSOM-config-file
            XmlDocument xml = new XmlDocument();
            string configFilename = null;

            // If more than one cfg-file is present - let user choose the cfg-files he needs
            #region open cfg-file
            System.Windows.Forms.OpenFileDialog openDlg = null;
            try
            {
                string[] _pathsOfExistingSettings = System.IO.Directory.GetFiles(fileParameters.cfgFolder, "*.cfg");
                if (_pathsOfExistingSettings.Length > 1)
                {
                    openDlg = new System.Windows.Forms.OpenFileDialog();
                    openDlg.Title = "Select RSOMSettings File";
                    openDlg.CheckFileExists = true;
                    openDlg.CheckPathExists = true;
                    openDlg.InitialDirectory = fileParameters.cfgFolder;
                    openDlg.Filter =
                        "RSOMsettings" + "|" + "*.cfg";
                    if (openDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        configFilename = openDlg.FileName;
                    }
                }
                else
                {
                    configFilename = _pathsOfExistingSettings[0];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No RSOMsettings file found in CommonApplicationData folder.", this.Name, MessageBoxButton.OK, MessageBoxImage.Warning);
                ConsoleStream.IOEventHandler.triggerIOEvent("SplashScreen", "EventHandler", "WARNING: No RSOMsettings file found in CommonApplicationData folder.");
            }
            finally
            {
                if (openDlg != null)
                    openDlg.Dispose();
            }

            // try to open selected cfg-file. In debug-mode a config file within the debug folder is loaded as last resort
            try
            {
                xml.Load(@configFilename);
            }
            catch
            {
                configFilename = "RSOMsettings - developper.cfg";
                try
                {
                    xml.Load(@configFilename);
                    MessageBox.Show("Loading config file located in debug folder.", this.Name, MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch
                {
                    MessageBox.Show("Loading of RSOMsettings configuration file failed.", this.Name, MessageBoxButton.OK, MessageBoxImage.Error);
                    ConsoleStream.IOEventHandler.triggerIOEvent("SplashScreen", "EventHandler", "ERROR: Loading of RSOMsettings configuration file failed.");
                    return 1;
                }
            }
            #endregion


            // if loading of cfg-file was successfull - load parameters
            #region load cfg file
            try
            {
                // define culture and helping varibale to load the config file
                CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
                NumberStyles styles = NumberStyles.AllowExponent | NumberStyles.Number;
                string[] stringSeparators = new string[] { "," };
                string[] values;
                bool canParse;
                double doubleParse;
                int intParse;

                // load fileParameters
                #region fileParameters
                try
                {
                    fileParameters.RSOMdataFolder = xml.SelectSingleNode("RSOMsettings/fileParameters/RSOMdataFolder").InnerText;
                    if (!Directory.Exists(fileParameters.RSOMdataFolder))
                        Directory.CreateDirectory(fileParameters.RSOMdataFolder);
                }
                catch { throw new Exception("Cannot load fileParameters RSOMdataFolder."); }

                try
                {
                    fileParameters.errorLogFolder = xml.SelectSingleNode("RSOMsettings/fileParameters/RSOMsupportFolder").InnerText;
                    if (!Directory.Exists(fileParameters.errorLogFolder))
                        Directory.CreateDirectory(fileParameters.errorLogFolder);
                }
                catch { throw new Exception("Cannot load fileParameters errorLogFolder."); }

                try { fileParameters.SFFile = xml.SelectSingleNode("RSOMsettings/fileParameters/SFFile").InnerText; }
                catch { throw new Exception("Cannot load fileParameters SFFile."); }

                try { fileParameters.SFFileLF = xml.SelectSingleNode("RSOMsettings/fileParameters/SFFileLF").InnerText; }
                catch { throw new Exception("Cannot load fileParameters SFFileLF."); }

                try { fileParameters.SFFileHF = xml.SelectSingleNode("RSOMsettings/fileParameters/SFFileHF").InnerText; }
                catch { throw new Exception("Cannot load fileParameters SFFileHF."); }

                try { fileParameters.ImageJ = xml.SelectSingleNode("RSOMsettings/fileParameters/ImageJ").InnerText; }
                catch { throw new Exception("Cannot load fileParameters ImageJ."); }
                #endregion

                // load acquisitionParameters
                #region acquisitionParameters
                try { canParse = Double.TryParse(xml.SelectSingleNode("RSOMsettings/acquisitionParameters/y_0").InnerText, styles, culture, out doubleParse); }
                catch { throw new Exception("Cannot load acquisition parameter y_0."); }
                if (canParse) { acquisitionParameters.y_0 = doubleParse; }
                else { throw new Exception("Cannot parse acquisition parameter y_0."); }

                try { canParse = Double.TryParse(xml.SelectSingleNode("RSOMsettings/acquisitionParameters/x_0").InnerText, styles, culture, out doubleParse); }
                catch { throw new Exception("Cannot load acquisition parameter x_0."); }
                if (canParse) { acquisitionParameters.x_0 = doubleParse; }
                else { throw new Exception("Cannot parse acquisition parameter x_0."); }

                try { canParse = Double.TryParse(xml.SelectSingleNode("RSOMsettings/acquisitionParameters/z_low").InnerText, styles, culture, out doubleParse); }
                catch { throw new Exception("Cannot load acquisition parameter z_low."); }
                if (canParse) { acquisitionParameters.z_low = doubleParse; }
                else { throw new Exception("Cannot parse acquisition parameter z_low."); }

                try { canParse = Double.TryParse(xml.SelectSingleNode("RSOMsettings/acquisitionParameters/z_target").InnerText, styles, culture, out doubleParse); }
                catch { throw new Exception("Cannot load acquisition parameter z_target."); }
                if (canParse) { acquisitionParameters.z_target = doubleParse; }
                else { throw new Exception("Cannot parse acquisition parameter z_target."); }

                try
                {
                    string z_values_string = xml.SelectSingleNode("RSOMsettings/acquisitionParameters/z_list").InnerText;
                    values = z_values_string.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); // separate string in single values
                    for (int i = 0; i < values.Length; i++)
                    {
                        canParse = Double.TryParse(values[i], styles, culture, out doubleParse);
                        if (canParse)
                            acquisitionParameters.z_list.Add(doubleParse);
                        else
                            throw new Exception("Error loading acquisition parameter list z_list.");
                    }
                }
                catch
                {
                    throw new Exception("Error loading acquisition parameter list z_list.");
                }

                try { canParse = Int32.TryParse(xml.SelectSingleNode("RSOMsettings/acquisitionParameters/z_listIndex").InnerText, styles, culture, out intParse); }
                catch { throw new Exception("Cannot load acquisition parameter z_listIndex."); }
                if (canParse) { acquisitionParameters.z_listIndex = intParse; }
                else { throw new Exception("Cannot load acquisition parameter z_listIndex."); }

                try
                {
                    string y_values_string = xml.SelectSingleNode("RSOMsettings/acquisitionParameters/y_list").InnerText;
                    values = y_values_string.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); // separate string in single values
                    for (int i = 0; i < values.Length; i++)
                    {
                        canParse = Double.TryParse(values[i], styles, culture, out doubleParse);
                        if (canParse)
                            acquisitionParameters.y_list.Add(doubleParse);
                        else
                            throw new Exception("Error loading acquisition parameter list y_list.");
                    }
                }
                catch
                {
                    throw new Exception("Error loading acquisition parameter list y_list.");
                }

                try { canParse = Int32.TryParse(xml.SelectSingleNode("RSOMsettings/acquisitionParameters/l_y_listIndex").InnerText, styles, culture, out intParse); }
                catch { throw new Exception("Cannot load acquisition parameter l_y_listIndex."); }
                if (canParse) { acquisitionParameters.l_y_listIndex = intParse; }
                else { throw new Exception("Cannot parse acquisition parameter l_y_listIndex."); }


                try
                {
                    string x_values_string = xml.SelectSingleNode("RSOMsettings/acquisitionParameters/x_list").InnerText;
                    values = x_values_string.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); // separate string in single values
                    for (int i = 0; i < values.Length; i++)
                    {
                        canParse = Double.TryParse(values[i], styles, culture, out doubleParse);
                        if (canParse)
                            acquisitionParameters.x_list.Add(doubleParse);
                        else
                            throw new Exception("Error loading acquisition parameter list x_list.");
                    }
                }
                catch
                {
                    throw new Exception("Error loading acquisition parameter list y_list.");
                }

                try { canParse = Int32.TryParse(xml.SelectSingleNode("RSOMsettings/acquisitionParameters/l_x_listIndex").InnerText, styles, culture, out intParse); }
                catch { throw new Exception("Cannot load acquisition parameter l_x_listIndex."); }
                if (canParse) { acquisitionParameters.l_x_listIndex = intParse; }
                else { throw new Exception("Cannot parse acquisition parameter l_x_listIndex."); }

                try { canParse = Double.TryParse(xml.SelectSingleNode("RSOMsettings/acquisitionParameters/ds").InnerText, styles, culture, out doubleParse); }
                catch { throw new Exception("Cannot load acquisition parameter ds."); }
                if (canParse) { acquisitionParameters.ds = doubleParse; }
                else { throw new Exception("Cannot parse acquisition parameter ds."); }

                try { canParse = Double.TryParse(xml.SelectSingleNode("RSOMsettings/acquisitionParameters/v_x").InnerText, styles, culture, out doubleParse); }
                catch { throw new Exception("Cannot load acquisition parameter v_x."); }
                if (canParse) { acquisitionParameters.v_x = doubleParse; }
                else { throw new Exception("Cannot parse acquisition parameter v_x."); }

                try { canParse = Double.TryParse(xml.SelectSingleNode("RSOMsettings/acquisitionParameters/acc").InnerText, styles, culture, out doubleParse); }
                catch { throw new Exception("Cannot load acquisition parameter acc."); }
                if (canParse) { acquisitionParameters.acc = doubleParse; }
                else { throw new Exception("Cannot parse acquisition parameter acc."); }

                try { canParse = Int32.TryParse(xml.SelectSingleNode("RSOMsettings/acquisitionParameters/laserConnected").InnerText, styles, culture, out intParse); }
                catch { throw new Exception("Cannot load acquisition parameter laserConnected."); }
                if (canParse) { systemState.LASERconnected = intParse; }
                else { throw new Exception("Cannot parse acquisition parameter laserConnected."); }

                try { acquisitionParameters.laserPort = xml.SelectSingleNode("RSOMsettings/acquisitionParameters/laserPort").InnerText; }
                catch { throw new Exception("Cannot load acquisitionParameters laserPort."); }

                try { canParse = Int32.TryParse(xml.SelectSingleNode("RSOMsettings/acquisitionParameters/PRR").InnerText, styles, culture, out intParse); }
                catch { throw new Exception("Cannot load acquisition parameter PRR."); }
                if (canParse) { acquisitionParameters.PRR = intParse; }
                else { throw new Exception("Cannot parse acquisition parameter PRR."); }

                try { canParse = Int32.TryParse(xml.SelectSingleNode("RSOMsettings/acquisitionParameters/DAQConnected").InnerText, styles, culture, out intParse); }
                catch { throw new Exception("Cannot load acquisition parameter DAQConnected."); }
                if (canParse) { systemState.DAQconnected = intParse; }
                else { throw new Exception("Cannot parse acquisition parameter DAQConnected."); }

                try { canParse = Int32.TryParse(xml.SelectSingleNode("RSOMsettings/acquisitionParameters/f_s").InnerText, styles, culture, out intParse); }
                catch { throw new Exception("Cannot load acquisition parameter f_s."); }
                if (canParse) { acquisitionParameters.f_s = intParse; }
                else { throw new Exception("Cannot parse acquisition parameter f_s."); }

                try
                {
                    string inputRange_values_string = xml.SelectSingleNode("RSOMsettings/acquisitionParameters/inputRange_list").InnerText;
                    values = inputRange_values_string.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); // separate string in single values
                    for (int i = 0; i < values.Length; i++)
                    {
                        canParse = Int32.TryParse(values[i], styles, culture, out intParse);
                        if (canParse)
                            acquisitionParameters.inputRange_list.Add(intParse);
                        else
                            throw new Exception("Error loading acquisition parameter inputRange_list.");
                    }
                }
                catch { throw new Exception("Error loading acquisition parameter inputRange_list."); }

                try { canParse = Int32.TryParse(xml.SelectSingleNode("RSOMsettings/acquisitionParameters/inputRange_listIndex").InnerText, styles, culture, out intParse); }
                catch { throw new Exception("Cannot load acquisition parameter inputRange_listIndex."); }
                if (canParse) { acquisitionParameters.inputRange_listIndex = intParse; }
                else { throw new Exception("Cannot parse acquisition parameter inputRange_listIndex."); }

                try { canParse = Int32.TryParse(xml.SelectSingleNode("RSOMsettings/acquisitionParameters/acqDelay").InnerText, styles, culture, out intParse); }
                catch { throw new Exception("Cannot load acquisition parameter acqDelay."); }
                if (canParse) { acquisitionParameters.acqDelay = intParse; }
                else { throw new Exception("Cannot load acquisition parameter acqDelay."); }

                try { canParse = Double.TryParse(xml.SelectSingleNode("RSOMsettings/acquisitionParameters/d_acc").InnerText, styles, culture, out doubleParse); }
                catch { throw new Exception("Cannot load acquisition parameter d_acc."); }
                if (canParse) { acquisitionParameters.d_acc = doubleParse; }
                else { throw new Exception("Cannot parse acquisition parameter d_acc."); }

                try { canParse = Int32.TryParse(xml.SelectSingleNode("RSOMsettings/acquisitionParameters/acqRes").InnerText, styles, culture, out intParse); }
                catch { throw new Exception("Cannot load acquisition parameter acqRes."); }
                if (canParse) { acquisitionParameters.acqRes = intParse; }
                else { throw new Exception("Cannot parse acquisition parameter acqRes."); }

                try { acquisitionParameters.acqMode = xml.SelectSingleNode("RSOMsettings/acquisitionParameters/acqMode").InnerText; }
                catch { throw new Exception("Cannot load acquisition parameter acqMode."); }

                try { canParse = Int32.TryParse(xml.SelectSingleNode("RSOMsettings/acquisitionParameters/triggerMode").InnerText, styles, culture, out intParse); }
                catch { throw new Exception("Cannot load acquisition parameter triggerMode."); }
                if (canParse) { acquisitionParameters.triggerMode = intParse; }
                else { throw new Exception("Cannot load acquisition parameter triggerMode."); }

                try { canParse = Int32.TryParse(xml.SelectSingleNode("RSOMsettings/acquisitionParameters/triggerLevel").InnerText, styles, culture, out intParse); }
                catch { throw new Exception("Cannot load acquisition parameter triggerLevel."); }
                if (canParse) { acquisitionParameters.triggerLevel = intParse; }
                else { throw new Exception("Cannot load acquisition parameter triggerLevel."); }

                try { canParse = Int32.TryParse(xml.SelectSingleNode("RSOMsettings/acquisitionParameters/numberOfWavelength").InnerText, styles, culture, out intParse); }
                catch { throw new Exception("Cannot load acquisition parameter numberOfWavelength."); }
                if (canParse) { acquisitionParameters.numberOfWavelength = intParse; }
                else { throw new Exception("Cannot load acquisition parameter numberOfWavelength."); }

                try { canParse = Int32.TryParse(xml.SelectSingleNode("RSOMsettings/acquisitionParameters/BscanUpdate").InnerText, styles, culture, out intParse); }
                catch { throw new Exception("Cannot load acquisition parameter BscanUpdate."); }
                if (canParse) { acquisitionParameters.BscanUpdate = intParse; }
                else { throw new Exception("Cannot load acquisition parameter BscanUpdate."); }

                try {
                    acquisitionParameters.controllerSerialNumber = xml.SelectSingleNode("RSOMsettings/acquisitionParameters/controllerSerialNumber").InnerText;
                    if (acquisitionParameters.controllerSerialNumber.StartsWith("noStages"))
                        systemState.STAGESconnected = 0;
                    else
                        systemState.STAGESconnected = 1;
                }
                catch { throw new Exception("Cannot load acquisition parameter controllerSerialNumber."); }
                #endregion

                // load reconstructionParameters
                #region reconstructionParameters
                try { canParse = Int32.TryParse(xml.SelectSingleNode("RSOMsettings/reconstructionParameters/reflectionFilter").InnerText, styles, culture, out intParse); }
                catch { throw new Exception("Cannot load reconstruction parameter reflectionFilter."); }
                if (canParse) { reconstructionParameters.reflectionFilter = intParse; }
                else { throw new Exception("Cannot parse reconstruction parameter reflectionFilter."); }

                try { canParse = Int32.TryParse(xml.SelectSingleNode("RSOMsettings/reconstructionParameters/bandpassFilter").InnerText, styles, culture, out intParse); }
                catch { throw new Exception("Cannot load reconstruction parameter bandpassFilter."); }
                if (canParse) { reconstructionParameters.bandpassFilter = intParse; }
                else { throw new Exception("Cannot parse reconstruction parameter bandpassFilter."); }

                try { canParse = Double.TryParse(xml.SelectSingleNode("RSOMsettings/reconstructionParameters/f_low").InnerText, styles, culture, out doubleParse); }
                catch { throw new Exception("Cannot load reconstruction parameter f_low."); }
                if (canParse) { reconstructionParameters.f_low = doubleParse; }
                else { throw new Exception("Cannot parse reconstruction parameter f_low."); }

                try { canParse = Double.TryParse(xml.SelectSingleNode("RSOMsettings/reconstructionParameters/f_mid").InnerText, styles, culture, out doubleParse); }
                catch { throw new Exception("Cannot load reconstruction parameter f_mid."); }
                if (canParse) { reconstructionParameters.f_mid = doubleParse; }
                else { throw new Exception("Cannot parse reconstruction parameter f_mid."); }

                try { canParse = Double.TryParse(xml.SelectSingleNode("RSOMsettings/reconstructionParameters/f_high").InnerText, styles, culture, out doubleParse); }
                catch { throw new Exception("Cannot load reconstruction parameter f_high."); }
                if (canParse) { reconstructionParameters.f_high = doubleParse; }
                else { throw new Exception("Cannot parse reconstruction parameter f_high."); }

                try { canParse = Int32.TryParse(xml.SelectSingleNode("RSOMsettings/reconstructionParameters/coRegistration").InnerText, styles, culture, out intParse); }
                catch { throw new Exception("Cannot load reconstruction parameter coRegistration."); }
                if (canParse) { reconstructionParameters.coRegistration = intParse; }
                else { throw new Exception("Cannot parse reconstruction parameter coRegistration."); }

                try { canParse = Int32.TryParse(xml.SelectSingleNode("RSOMsettings/reconstructionParameters/focusModel").InnerText, styles, culture, out intParse); }
                catch { throw new Exception("Cannot load reconstruction parameter focusModel."); }
                if (canParse) { reconstructionParameters.focusModel = intParse; }
                else { throw new Exception("Cannot parse reconstruction parameter focusModel."); }

                try { canParse = Int32.TryParse(xml.SelectSingleNode("RSOMsettings/reconstructionParameters/bpMode").InnerText, styles, culture, out intParse); }
                catch { throw new Exception("Cannot load reconstruction parameter bpMode."); }
                if (canParse) { reconstructionParameters.bpMode = intParse; }
                else { throw new Exception("Cannot parse reconstruction parameter bpMode."); }

                try { canParse = Double.TryParse(xml.SelectSingleNode("RSOMsettings/reconstructionParameters/bpWeight").InnerText, styles, culture, out doubleParse); }
                catch { throw new Exception("Cannot load reconstruction parameter bpWeight."); }
                if (canParse) { reconstructionParameters.bpWeight = doubleParse; }
                else { throw new Exception("Cannot parse reconstruction parameter bpWeight."); }

                try { canParse = Double.TryParse(xml.SelectSingleNode("RSOMsettings/reconstructionParameters/v_s").InnerText, styles, culture, out doubleParse); }
                catch { throw new Exception("Cannot load reconstruction parameter v_s."); }
                if (canParse) { reconstructionParameters.v_s = doubleParse; }
                else { throw new Exception("Cannot parse reconstruction parameter v_s."); }

                try { canParse = Int32.TryParse(xml.SelectSingleNode("RSOMsettings/reconstructionParameters/blockSize").InnerText, styles, culture, out intParse); }
                catch { throw new Exception("Cannot load reconstruction parameter blockSize."); }
                if (canParse) { reconstructionParameters.blockSize = intParse; }
                else { throw new Exception("Cannot parse reconstruction parameter blockSize."); }

                try { reconstructionParameters.platform = xml.SelectSingleNode("RSOMsettings/reconstructionParameters/platform").InnerText; }
                catch { throw new Exception("Cannot load reconstructionParameters platform."); }

                try { reconstructionParameters.deviceType = xml.SelectSingleNode("RSOMsettings/reconstructionParameters/deviceType").InnerText; }
                catch { throw new Exception("Cannot load reconstructionParameters deviceType."); }

                try { canParse = Int32.TryParse(xml.SelectSingleNode("RSOMsettings/reconstructionParameters/dz").InnerText, styles, culture, out intParse); }
                catch { throw new Exception("Cannot load reconstruction parameter dz."); }
                if (canParse) { reconstructionParameters.dz = intParse; }
                else { throw new Exception("Cannot parse reconstruction parameter dz."); }

                try { canParse = Int32.TryParse(xml.SelectSingleNode("RSOMsettings/reconstructionParameters/ds").InnerText, styles, culture, out intParse); }
                catch { throw new Exception("Cannot load reconstruction parameter ds."); }
                if (canParse) { reconstructionParameters.ds = intParse; }
                else { throw new Exception("Cannot parse reconstruction parameter ds."); }

                try { canParse = Int32.TryParse(xml.SelectSingleNode("RSOMsettings/reconstructionParameters/maxOffset").InnerText, styles, culture, out intParse); }
                catch { throw new Exception("Cannot load reconstruction parameter maxOffset."); }
                if (canParse) { reconstructionParameters.maxOffset = intParse; }
                else { throw new Exception("Cannot parse reconstruction parameter maxOffset."); }

                try { canParse = Double.TryParse(xml.SelectSingleNode("RSOMsettings/reconstructionParameters/y_shiftInd").InnerText, styles, culture, out doubleParse); }
                catch { throw new Exception("Cannot load reconstruction parameter y_shiftInd."); }
                if (canParse) { reconstructionParameters.y_shiftInd = doubleParse; }
                else { throw new Exception("Cannot parse reconstruction parameter y_shiftInd."); }

                try { canParse = Double.TryParse(xml.SelectSingleNode("RSOMsettings/reconstructionParameters/zRecon_low").InnerText, styles, culture, out doubleParse); }
                catch { throw new Exception("Cannot load reconstruction parameter zRecon_low."); }
                if (canParse) { reconstructionParameters.zRecon_low = doubleParse; }
                else { throw new Exception("Cannot parse reconstruction parameter zRecon_low."); }

                try
                {
                    string zRecon_values_string = xml.SelectSingleNode("RSOMsettings/reconstructionParameters/zRecon_list").InnerText;
                    values = zRecon_values_string.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); // separate string in single values
                    for (int i = 0; i < values.Length; i++)
                    {
                        canParse = Double.TryParse(values[i], styles, culture, out doubleParse);
                        if (canParse)
                            reconstructionParameters.zRecon_list.Add(doubleParse);
                        else
                            throw new Exception("Cannot load reconstruction parameter zRecon_list.");
                    }
                }
                catch
                {
                    throw new Exception("Cannot load reconstruction parameter zRecon_list.");
                }

                try { canParse = Int32.TryParse(xml.SelectSingleNode("RSOMsettings/reconstructionParameters/zRecon_listIndex").InnerText, styles, culture, out intParse); }
                catch { throw new Exception("Cannot load reconstruction parameter zRecon_listIndex."); }
                if (canParse) { reconstructionParameters.zRecon_listIndex = intParse; }
                else { throw new Exception("Cannot parse reconstruction parameter zRecon_listIndex."); }

                try { canParse = Int32.TryParse(xml.SelectSingleNode("RSOMsettings/reconstructionParameters/bandpassSeparation").InnerText, styles, culture, out intParse); }
                catch { throw new Exception("Cannot load reconstruction parameter bandpassSeparation."); }
                if (canParse) { reconstructionParameters.bandpassSeparation = intParse; }
                else { throw new Exception("Cannot parse reconstruction parameter bandpassSeparation."); }

                try { canParse = Int32.TryParse(xml.SelectSingleNode("RSOMsettings/reconstructionParameters/movingMAP").InnerText, styles, culture, out intParse); }
                catch { throw new Exception("Cannot load reconstruction parameter movingMAP."); }
                if (canParse) { reconstructionParameters.movingMAP = intParse; }
                else { throw new Exception("Cannot parse reconstruction parameter movingMAP."); }

                try { canParse = Int32.TryParse(xml.SelectSingleNode("RSOMsettings/reconstructionParameters/movingMAP_slabThickness").InnerText, styles, culture, out intParse); }
                catch { throw new Exception("Cannot load reconstruction parameter movingMAP_slabThickness."); }
                if (canParse) { reconstructionParameters.movingMAP_slabThickness = intParse; }
                else { throw new Exception("Cannot parse reconstruction parameter movingMAP_slabThickness."); }

                try { canParse = Int32.TryParse(xml.SelectSingleNode("RSOMsettings/reconstructionParameters/volume3D").InnerText, styles, culture, out intParse); }
                catch { throw new Exception("Cannot load reconstruction parameter volume3D."); }
                if (canParse) { reconstructionParameters.volume3D = intParse; }
                else { throw new Exception("Cannot parse reconstruction parameter volume3D."); }
                #endregion

                // load detectorParameters
                #region detectorParameters
                try { canParse = Double.TryParse(xml.SelectSingleNode("RSOMsettings/detectorParameters/t_focus").InnerText, styles, culture, out doubleParse); }
                catch { throw new Exception("Cannot load detector parameter t_focus."); }
                if (canParse) { detectorParameters.t_focus = doubleParse; }
                else { throw new Exception("Cannot parse detector parameter t_focus."); }

                try { canParse = Double.TryParse(xml.SelectSingleNode("RSOMsettings/detectorParameters/focalLength").InnerText, styles, culture, out doubleParse); }
                catch { throw new Exception("Cannot load detector parameter focalLength."); }
                if (canParse) { detectorParameters.focalLength = doubleParse; }
                else { throw new Exception("Cannot parse detector parameter focalLength."); }

                try { canParse = Double.TryParse(xml.SelectSingleNode("RSOMsettings/detectorParameters/detectorDiameter").InnerText, styles, culture, out doubleParse); }
                catch { throw new Exception("Cannot load detector parameter detectorDiameter."); }
                if (canParse) { detectorParameters.detectorDiameter = doubleParse; }
                else { throw new Exception("Cannot parse detector parameter detectorDiameter."); }

                try { canParse = Int32.TryParse(xml.SelectSingleNode("RSOMsettings/detectorParameters/dataSign").InnerText, styles, culture, out intParse); }
                catch { throw new Exception("Cannot load detector parameter dataSign."); }
                if (canParse) { detectorParameters.dataSign = intParse; }
                else { throw new Exception("Cannot parse detector parameter dataSign."); }
                #endregion

                #region unmixing
                try
                {
                    string unmixingComponents_string = xml.SelectSingleNode("RSOMsettings/unmixingParameters/Components").InnerText;
                    values = unmixingComponents_string.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); // separate string in single values
                    for (int i = 0; i < values.Length; i++)
                    {
                        unmixingParameters.myUnmixComponents.Add(values[i]);                        
                    }
                }
                catch
                {
                    throw new Exception("Cannot load unmixing parameter Components.");
                }

                try
                {
                    unmixingParameters.Spectra = xml.SelectSingleNode("RSOMsettings/unmixingParameters/Spectra").InnerText;
                    
                }
                catch { throw new Exception("Cannot load unmixingParameters Spectra."); }

                try
                {
                    unmixingParameters.LaserEnergy = xml.SelectSingleNode("RSOMsettings/unmixingParameters/LaserEnergy").InnerText;
                }
                catch { throw new Exception("Cannot load unmixingParameters LaserEnergy for calibration."); }

                try { canParse = Double.TryParse(xml.SelectSingleNode("RSOMsettings/unmixingParameters/Unmixing3D").InnerText, styles, culture, out doubleParse); }
                catch { throw new Exception("Cannot load unmixingParameters Unmixing3D."); }

                try { canParse = Double.TryParse(xml.SelectSingleNode("RSOMsettings/unmixingParameters/RegImages").InnerText, styles, culture, out doubleParse); }
                catch { throw new Exception("Cannot load unmixingParameters RegImages."); }

                try { canParse = Double.TryParse(xml.SelectSingleNode("RSOMsettings/unmixingParameters/RegImage_interp").InnerText, styles, culture, out doubleParse); }
                catch { throw new Exception("Cannot load unmixingParameters RegImage_interp."); }


                #endregion



            }
            catch (Exception ex)
            {
                //Cannot use Writeline Event trigger yet for error messages
                MessageBox.Show("ERROR: Loading config parameters failed.\n" + ex.Message, this.Name, MessageBoxButton.OK, MessageBoxImage.Error);
                ConsoleStream.IOEventHandler.triggerIOEvent("SplashScreen", "EventHandler", "ERROR: Loading config parameters failed.\n" + ex.Message);
                return 1;
            }
            #endregion

            // open RSOMpreset file
            #region open preset
            XmlDocument xml_preset = new XmlDocument();
            string presetFilename = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\iThera\\ViewRSOM\\RSOMpreset.xml";
            try
            {
                // load RSOMpreset
                xml_preset.Load(@presetFilename);

                // define culture and helping varibale to load the config file
                CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
                NumberStyles styles = NumberStyles.AllowExponent | NumberStyles.Number;
                bool canParse;
                int intParse;

                // load fileParameters
                try { canParse = Int32.TryParse(xml_preset.SelectSingleNode("RSOMpreset/laserSettings/energy").InnerText, styles, culture, out intParse); }
                catch { throw new Exception("Cannot load laser energy preset."); }
                if (canParse)
                {
                    acquisitionParameters.laserPower = intParse;
                }
                else
                {
                    MessageBox.Show("no RSOMpreset file was found. Using default value 0 for laser energy. Adapt as needed.", this.Name, MessageBoxButton.OK, MessageBoxImage.Information);
                    acquisitionParameters.laserPower = 0;
                }
            }
            catch
            {
                MessageBox.Show("no RSOMpreset file was found. Using default value 0 for laser energy. Adapt as needed.", this.Name, MessageBoxButton.OK, MessageBoxImage.Information);
                acquisitionParameters.laserPower = 0;
            }
            #endregion

            return 0;

        }

        void delay_Tick(object sender, EventArgs e)
        {
            
            ViewRSOM_Main mainView = null;
            
            try
            {
                Thread.Sleep(1000);
                _delay.Stop();
                mainView = new ViewRSOM_Main();
                //mainView.StartMSOT(childWindowContainer);
                mainView.Show();
                Close();
                Application.Current.MainWindow = mainView;
                //mainView.ShowPendingPushedMessages();
                mainView = null;
                
            }
            finally
            {
                //mainView.Dispose();
            }
              
        }

    }

}
