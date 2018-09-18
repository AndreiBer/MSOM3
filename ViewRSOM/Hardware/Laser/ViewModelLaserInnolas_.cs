using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewRSOM.MSOT.DataModels.Plugins.Laser;
/**/ //using Xvue.Framework.API.Services.MonitorPlugins;
using System.Globalization;
using System.Threading;
/**/ //using Xvue.MSOT.Services.HardwareSystem_;
/**/ //using Xvue.MSOT.DataModels.Plugins.HardwareSystem;
using ViewRSOM.MSOT.Hardware.ViewModels.Laser.Innolas;
using ViewRSOM.MSOT.Hardware.ViewModels.Laser;
using System.Net;
using General.Tools.Communication;
using Laser.Parameter;
using Laser.OpoData;

namespace ViewRSOM.MSOT.Hardware.ViewModels.Laser
{
    public class ViewModelLaserInnolas : ViewModelLaserBase
    {
        #region localvariables
        //ProtocolWrapper<StandardCommandType> innolasModule;
        new DataModelLaserInnolas DMMS;
        bool _isIinterlockOpen;
        int lastOpo3Counter;
        int _pockelscellDelay;
        double _attenuation;
        int _amplifier;
        bool _acceptTriggerChangeAndAttenuationCommands;
        DataModelLaserVersionInfo _versionInfo;
        #endregion localvariables

        public ProgrammSettings myProgrammSettings = new ProgrammSettings();

        // Innolas laser object
        //Hardware.Laser.ViewModelLaer  innolasLaser;
        HandleOpoData myOpoDataHandler = new HandleOpoData();
        public ViewModelLaserInnolas my_laser = null;
        ProtocolWrapper<StandardCommandType> innolasModule = new ProtocolWrapper<StandardCommandType>(StandardCommandsDictionary.StandardCommands, StandardCommandsDictionary.StandardErrors);


        public ViewModelLaserInnolas()
        {

        }



        #region publicvariables
        // define laser EventHandler
        public RoutedEventHandler laserHandle;
        public bool AcceptTriggerChangeAndAttenuationCommands
        {
            get { return _acceptTriggerChangeAndAttenuationCommands; }
            set { _acceptTriggerChangeAndAttenuationCommands = value; }
        }
        public int PockelscellDelayMin { get { return LaserParameter.PockelscellDelayMin; } }
        public int PockelscellDelayMax { get { return LaserParameter.PockelscellDelayMax; } }

        public int PockelscellDelay
        {
            get { return _pockelscellDelay; }
            set
            {
                if ((value >= PockelscellDelayMin) && (value <= PockelscellDelayMax))
                {
                    if (_pockelscellDelay != value)
                    {
                        _pockelscellDelay = value;
                        setAttenuationViaPockelScell(_pockelscellDelay);
                        //OnPropertyChanged("PockelscellDelay");  // aus ViewModelBase
                    }
                }
            }
        }

        public double Attenuation
        {
            get { return _attenuation; }
            set
            {
                if ((value >= 0) && (value <= 100.0))
                {
                    if (_attenuation != value)
                    {
                        _attenuation = value;
                        setAttenuationViaAttenuator(_attenuation);
                        //  OnPropertyChanged("Attenuation");
                    }
                }
            }
        }

        public bool PockelscellControlMethodVisible { get { return (DMMS.PowerControlMethod == DataModelLaserInnolas.InnolasPowerControlMethod.PockelscellDelay); } }

        public int AmplifierMin { get { return LaserParameter.AmplifierMin; } }
        public int AmplifierMax { get { return LaserParameter.AmplifierMax; } }

        public int Amplifier
        {
            get { return _amplifier; }
            set
            {
                if ((value >= AmplifierMin) && (value <= AmplifierMax))
                {
                    if (_amplifier != value)
                    {
                        _amplifier = value;
                        setAttenuationViaAmplifier(_amplifier);
                        //  OnPropertyChanged("Amplifier");
                    }
                }
            }
        }

        public bool AmplifierControlMethodVisible { get { return (DMMS.PowerControlMethod == DataModelLaserInnolas.InnolasPowerControlMethod.Amplifier); } }
        public override bool IsInterlockOpen
        {
            get { return _isIinterlockOpen; }
        }

        public DataModelLaserVersionInfo VersionInfo
        {
            get { return _versionInfo; }
        }
        #endregion publicvariables

        #region importantpublicmethods


        public override void connectOPO(out bool status, out string strStatus)
        {
            int errorCode = 0;
            IPAddress proxyIP;
            int proxyPort;
            proxyIP = IPAddress.Parse(LaserParameter.proxyIP);
            proxyPort = LaserParameter.proxyPort;
            GUI_Communicator.sendStatus("Laser", "HardwareMonitor", "Parsing IpAdress: " + proxyIP);
            GUI_Communicator.sendStatus("Laser", "HardwareMonitor", "Trying to connect to " + proxyIP + " on Port " + proxyPort);
            errorCode = innolasModule.Connect(proxyIP, proxyPort);
            if (errorCode != 0)
            {
                status = false;
                strStatus = "No Connection to OPO";
            }
            else
            {
                status = true;
                strStatus = "OPO connected";
            }


        }

        public override void AfterInitialize()
        {
            //         base.AfterInitialize();  // aus basisklasse
            GUI_Communicator.sendStatus("Laser", "HardwareMonitor", "AfterInitialize ausgeführt");
            _isIinterlockOpen = false;
            _pockelscellDelay = int.MinValue;
            _attenuation = double.NaN;
            _amplifier = int.MinValue;
            innolasModule = new ProtocolWrapper<StandardCommandType>(StandardCommandsDictionary.StandardCommands, StandardCommandsDictionary.StandardErrors);
            _versionInfo = new DataModelLaserVersionInfo();
            AcceptTriggerChangeAndAttenuationCommands = true;
        }

        public override void GetLaserState(out List<string> receivedCommands, out string message)
        {
            //List<string> receivedCommands;
            //string message = "";
            innolasModule.ExchangeCommand(StandardCommandType.GetLaserState, "", out receivedCommands, out message);
        }
        public override void initTask()
        {
            string message = "";
            int errorCode = 0;

            // try
            {
                //_exitLogging = false;
                //_activePeriodPowerRegisters = new List<DataModelLaserPowerRegister>();

                GUI_Communicator.sendStatus("Laser", "GUI", "trying to execute init Task");//your code here...
                                                                                           //System.Windows.MessageBox.Show("we are here");

                _pendingPowerSamples = new Queue<PowerSample>();
                lastOpo3Counter = -1;
                //_loggingThread = new Thread(new ThreadStart(logTask));
                //_loggingThread.Name = "Innolas Laser Power Logging Thread";
                //_loggingThread.Start();                
                List<string> receivedCommands;
                IPAddress proxyIP;
                int proxyPort;
                //if (DMMS.ProxyIp != 0)
                {
                    // proxyIP = new IPAddress(DMMS.ProxyIp);
                }
                //else
                {
                    proxyIP = IPAddress.Parse(LaserParameter.proxyIP);
                    proxyPort = LaserParameter.proxyPort;
                    GUI_Communicator.sendStatus("Laser", "HardwareMonitor", "Parsing IpAdress: " + proxyIP);
                }
                GUI_Communicator.sendStatus("Laser", "HardwareMonitor", "Trying to connect to " + proxyIP + " on Port " + proxyPort);
                errorCode = innolasModule.Connect(proxyIP, proxyPort);

                errorCode = innolasModule.ExchangeCommand(StandardCommandType.StartupLaser, "", out receivedCommands, out message);

                if (errorCode == 0)
                {
                    GUI_Communicator.sendStatus("Laser", "GUI", "IPConnectionOK");
                    GUI_Communicator.sendStatus("Laser", "HardwareMonitor", "Starting Up Laser");
                    errorCode = innolasModule.ExchangeCommand(StandardCommandType.StartupLaser, "", out receivedCommands, out message);
                }
                else
                    GUI_Communicator.sendStatus("Laser", "GUI", "NoIPConnection");

                if (errorCode == 0)
                    GUI_Communicator.sendStatus("Laser", "GUI", "LaserStartUp");
                errorCode = innolasModule.ExchangeCommand(StandardCommandType.SetUserModeAdmin, "Innolas", out receivedCommands, out message);


                if (errorCode == 0)
                {
                    //Set User mode command above receives as replies several laser info commands
                    // which we do not want to mess with our next communications so we wait for their reception before we start
                    Thread.Sleep(500);
                    /**/ //                   errorCode = switchChannel(MSOTService.IMSOTHardware.SelectedDetector.LaserChannelNumber)?0:1;                    
                }
                if (errorCode == 0)
                {
                    GUI_Communicator.sendStatus("Laser", "HardwareMonitor", "Trying to get Version Information");
                    GetVersionInformation();

                }
                if (errorCode == 0)
                {
                    GUI_Communicator.sendStatus("Laser", "HardwareMonitor", "Trying to get min and max Wavelengths");
                    System.Globalization.NumberFormatInfo nfi = new System.Globalization.NumberFormatInfo();
                    nfi.NumberDecimalSeparator = ".";
                    errorCode = innolasModule.ExchangeCommand(StandardCommandType.GetMinWavelength, "", out receivedCommands, out message);

                    if ((errorCode == 0) && (receivedCommands.Count == 1))
                    {
                        GUI_Communicator.sendStatus("Laser", "HardwareMonitor", "im if");
                        LaserParameter.minWaveLength = Convert.ToDouble(receivedCommands[0], nfi);
                        errorCode = innolasModule.ExchangeCommand(StandardCommandType.GetMaxWavelength, "", out receivedCommands, out message);
                        if ((errorCode == 0) && (receivedCommands.Count == 1))
                            LaserParameter.maxWaveLength = Convert.ToDouble(receivedCommands[0], nfi);
                        else errorCode = -201;

                    }
                    else
                        errorCode = -200;
                    GUI_Communicator.sendStatus("Laser", "HardwareMonitor", "aktueller Errorcode " + errorCode);
                    /*!!!LÖSCHEN!!*/
                    errorCode = 0;
                }
                GUI_Communicator.sendStatus("Laser", "HardwareMonitor", "Hier");
                LaserTriggerRate = LaserParameter.LaserTriggerRate; //AvailableTriggerRates.Min();
                Amplifier = LaserParameter.Amplifier;
                PockelscellDelay = LaserParameter.PockelscellDelay;
                Attenuation = LaserParameter.Attenuator;
                if (errorCode == 0)//ask for active warnings so as to check (indirectly) for interlock start up state.
                {
                    GUI_Communicator.sendStatus("Laser", "HardwareMonitor", "Ask for warning list");
                    errorCode = innolasModule.ExchangeCommand(StandardCommandType.GetWarningList, "", out message);
                }

                if (errorCode == 0)
                {/*
                    checkWavelengthRange();
                    List<double> wavelengths = new List<double>();
                    wavelengths.Add(700);
                    GUI_Communicator.sendError("Laser", "ErrorDisplay", "Now fire first shot! WL: "+WaveLength);
                    if (tune(wavelengths, 1,1))
                    {

                     
                        GUI_Communicator.sendStatus("Laser", "HardwareMonitor", "Laser composite init complete");
                        return;
                    }
                    else
                    {
                        errorCode = 1;
                        GUI_Communicator.sendStatus("Laser", "ErrorDisplay", "Tune (Shooting) failed");
                    }    
                  */

                }
            }
            GUI_Communicator.sendError("Laser", "ErrorDisplay", "Composite init ended: ");

            if (errorCode != 1)
                GUI_Communicator.sendStatus("LASER_STATE", "GUI", "InitTaskNotCompleted");
            else if (errorCode == 0)
                GUI_Communicator.sendStatus("LASER_STATE", "GUI", "InitTaskComplete");
        }

        public override bool StartChargerChange()
        {
            return true;
        }

        public bool StopChargerChange()
        {
            return true;
        }

        public override bool compositeClose()
        {
            //SHUTDOWN_LASER
            int errorCode = 0;
            bool error = false;
            string message = "";
            List<string> receivedCommands;
            IPAddress proxyIP;
            int proxyPort;
            proxyIP = IPAddress.Parse(LaserParameter.proxyIP);
            proxyPort = LaserParameter.proxyPort;
            GUI_Communicator.sendStatus("Laser", "HardwareMonitor", "Parsing IpAdress: " + proxyIP);
            GUI_Communicator.sendStatus("Laser", "HardwareMonitor", "Trying to connect to " + proxyIP + " on Port " + proxyPort);
            errorCode = innolasModule.Connect(proxyIP, proxyPort);

            try
            {
                //_exitLogging = true;
                List<String> expectedCommands = new List<string>();
                if (errorCode == 0)
                {
                    q_switch(false);
                    lamp(false);
                    errorCode = innolasModule.ExchangeCommand(StandardCommandType.ShutdownLaser, "", out expectedCommands, out message);
                    if (errorCode == -3) //timeout, then try the workaround...
                    {
                        if (laserTrace.TraceVerbose)
                            /**/ //  Log MSOT     Xvue.MSOT.ViewModels.Log.ViewModelLog.MsotTrace("Shutdown failed, trying the workaround", laserTrace);
                            GUI_Communicator.sendError("Laser", "LogMSOT", "Shutdown failed, trying the workaround");
                        errorCode = innolasModule.ExchangeCommand(StandardCommandType.ShutdownWorkAround, "", out message);
                    }
                    if (errorCode == 0)
                    {
                        /**/ //  Log Error   MSOTService.IMSOTLog.HandleError(Services.Log.EnumLogType.Info, DisplayName, "Laser shutdown successful");
                        GUI_Communicator.sendError("Laser", "ErroDisplay", "Laser shutdown successful");
                    }
                    else
                    {
                        /**/ //   Log Error   MSOTService.IMSOTLog.HandleError(Services.Log.EnumLogType.Warning, DisplayName, "Laser shutdown failed");
                        GUI_Communicator.sendError("Laser", "ErroDisplay", "Laser shutdown failed");
                    }
                    if (innolasModule.Close(out message) < 0)
                    {
                        /**/  // log  Error  MSOTService.IMSOTLog.HandleError(DisplayName, "Close failed, reason: " + message);
                        GUI_Communicator.sendError("Laser", "ErroDisplay", "Close failed, reason: " + message);
                        error = true;
                    }
                }
                else error = true;

            }
            catch (Exception ex)
            {
                /**/  //  Log Error     MSOTService.IMSOTLog.HandleError(DisplayName, "Shutdown failed, reason: " +ex.Message);
                GUI_Communicator.sendError("Laser", "ErroDisplay", "Shutdown failed, reason: " + ex);
                error = true;
            }
            return !error;
        }

        public override bool lamp(bool state)
        {
            try
            {
                int errorCode = 0;
                string message = "";
                List<String> expectedCommands = new List<string>();
                if (state)
                {
                    //errorCode = reenableSimmer(); //in case the simmer is off (init ambiguity)
                    //GUI_Communicator.sendStatus("Laser", "GUI", "reenabled simmer, result: " + errorCode);
                    //if (errorCode == 0)
                    errorCode = innolasModule.ExchangeCommand(StandardCommandType.FlashLampOn, "", out expectedCommands, out message);
                    GUI_Communicator.sendStatus("Laser", "GUI", "Flashlamp on, result: " + errorCode);
                }
                else
                {
                    errorCode = innolasModule.ExchangeCommand(StandardCommandType.FlashLampOff, "", out message);
                }
                if (errorCode == 0)
                {
                    return true;
                }
                else
                {
                    /**/ // Log Error   MSOTService.IMSOTLog.HandleError(DisplayName, "Error on lamp control: " + message + "(" + errorCode + ")");
                    GUI_Communicator.sendError("Laser", "ErrorDisplay", "Error on lamp control: " + message + "(" + errorCode + ")");
                }
            }
            catch (Exception ex)
            {
                /**/ // Log  Error              MSOTService.IMSOTLog.HandleError(DisplayName, "Error on lamp control: " + ex.Message);
                GUI_Communicator.sendError("Laser", "ErrorDisplay", "Error on lamp control: " + ex.Message);
            }
            return false;
        }

        public int getSweepState()
        {
            string message = "";
            List<string> expectedCommands = null;
            int errorCode = innolasModule.ExchangeCommand(StandardCommandType.GetSweepState, "", out expectedCommands, out message);
            return errorCode;
        }

        public override string getSweepStateAsString()
        {
            string message = "";
            List<string> expectedCommands = null;
            int errorCode = innolasModule.ExchangeCommand(StandardCommandType.GetSweepState, "", out expectedCommands, out message);
            return message;
        }

        public override bool q_switch(bool state)
        {
            int errorCode;
            string message = "";
            List<string> expectedCommands = null;
            bool stateChangeFailed = false;
            try
            {
                if (state)
                {
                    lastOpo3Counter = -1;//reset the counter for the innolas counting issue
                    errorCode = reenablePockelSell(out message);
                    if (errorCode == 0)//check for pockelsell and reenable if needed        
                    {
                        //errorCode = innolasModule.ExchangeCommand(StandardCommandType.GetSweepState, "",out expectedCommands, out message);
                        if (errorCode == 0)
                        {
                            if (expectedCommands.Count > 0)
                            {
                                if (expectedCommands[0].Contains("SWEEP_POSSIBLE"))
                                {
                                    //we need to restart sweep
                                    //errorCode = innolasModule.ExchangeCommand(StandardCommandType.StartSweep, "", out message);
                                }
                            }
                        }
                    }
                    System.Threading.Thread.Sleep(200);
                    errorCode = innolasModule.ExchangeCommand(StandardCommandType.SetShutterState, "OPEN", out expectedCommands, out message);
                    if (errorCode == 0)
                        errorCode = innolasModule.ExchangeCommand(StandardCommandType.SetShutterState, "OPEN", out expectedCommands, out message);
                    if ((errorCode == 0) && (expectedCommands.Count > 0))
                    {
                        if (!expectedCommands[0].Contains("OPEN"))//"PcOnShutterOn"))
                        {
                            stateChangeFailed = true;
                        }
                    }
                }
                else
                {
                    ////errorCode = innolasModule.ExchangeCommand(StandardCommandType.StopSweep, "", out message);
                    errorCode = innolasModule.ExchangeCommand(StandardCommandType.SetShutterState, "CLOSE", out expectedCommands, out message);
                    if (errorCode != 0)
                    {
                        stateChangeFailed = true;
                    }
                }
                if ((errorCode == 0) && !stateChangeFailed)
                {
                    /**/ // Log Error    MSOTService.IMSOTLog.HandleError(Services.Log.EnumLogType.Info, DisplayName, "Q-Switch Control, New state: " + state);      
                    GUI_Communicator.sendError("Laser", "ErrorDisplay", "Q-Switch Control, New state: " + state);
                    return true;
                }
                else
                {
                    /**/ // Log Error  MSOTService.IMSOTLog.HandleError(DisplayName, "Error on Q-switch control: " + message + "(" + errorCode + "): stateChange:" + !stateChangeFailed);
                    GUI_Communicator.sendError("Laser", "ErrorDisplay", "Error on Q-switch control: " + message + "(" + errorCode + "): stateChange:" + !stateChangeFailed);
                }
            }
            catch (Exception ex)
            {
                /**/ // Log Error  MSOTService.IMSOTLog.HandleError(DisplayName, "Exception on Q-switch control: " + ex.Message);
                GUI_Communicator.sendError("Laser", "ErrorDisplay", "Exception on Q-switch control: " + ex.Message);
            }
            return false;
        }

        public override void setWavelength(int wl)
        {
            string message = "";
            innolasModule.ExchangeCommand(StandardCommandType.SetWavelength, wl.ToString(), out message);
        }

        public override void illuminationON()
        {
            string StatusMessage;
            StatusMessage = CheckShutterState();
            if (StatusMessage == "OPEN")
            {
                q_switch(false);
                lamp(false);
                Thread.Sleep(1000);
            }
            int wl = Convert.ToInt32(LaserParameter.LaserDefaultWavelength);
            setWavelength(wl);
            Thread.Sleep(50);
            lamp(true);
            Thread.Sleep(500);
            AcceptTriggerChangeAndAttenuationCommands = true;
            LaserParameter.PowerControlMethod = "None";
            setAttenuationViaPockelScell(LaserParameter.PockelscellDelay);
            q_switch(true);
            //Thread.Sleep(1000);

            StatusMessage = CheckShutterState();
            while (StatusMessage == "CLOSE")
            {
                // errorCode = innolasModule.ExchangeCommand(StandardCommandType.GetShutterState, "", out receivedCommands, out message);
                StatusMessage = CheckShutterState(); ;
                Thread.Sleep(100);
            }
        }

        public override void illuminationOFF()
        {
            q_switch(false);
            lamp(false);

        }

        public override int[] retrieveWL(string comment)
        {


            char[] delimiterChars = { ']', '[', '}', '{', '(', ')', ' ', ',', '.', '\t' };
            string[] words = comment.Split(delimiterChars);
            words = words.Where(x => !string.IsNullOrEmpty(x)).ToArray(); //delete empty array elements
            List<int> numArrList = new List<int>();
            int[] numArrStep = new int[0];
            string[] tempStr = new string[] { };
            int firstVal = new int { };
            int step = new int { };
            int secondVal = new int { };
            int count = new int { };
            // parse the elements
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Contains(":") == true)
                {
                    tempStr = words[i].Split(':');
                    firstVal = int.Parse(tempStr[0]);
                    step = int.Parse(tempStr[1]);
                    secondVal = int.Parse(tempStr[2]);
                    for (int val = firstVal; val <= secondVal; val = (val + step))
                    {
                        numArrList.Add(val);
                    }
                }
                else
                {
                    numArrList.Add(int.Parse(words[i]));
                }
            }
            int[] numArr = numArrList.ToArray();

            Array.Sort(numArr);
            //string wlArrStr = numArr.ToString(); 

            // make sure that wavelength values are within the range  
            switch (LaserParameter.Channel)
            {
                case 0:
                    if (numArr.Min() < MinWaveLength || numArr.Max() > MaxWaveLength)
                        numArr = null;                     
                    break;
                case 1:
                    if(numArr.Min() < MinWaveLength || numArr.Max() > MaxWaveLength)
                        numArr = null;
                    break;
                case 2:
                    if(numArr.Min() < MinWaveLength*2 || numArr.Max() > MaxWaveLength*2)
                        numArr = null;
                    break;
            }            
            return numArr;
        }

        public override bool tune(List<double> wavelengths, int pulsesPerWl, int repeatCount)
        {
            int errorCode = 0;
            string message = "";
            int sweepCount;
            if (errorCode == 0)
                errorCode = innolasModule.ExchangeCommand(StandardCommandType.ResetSweepTable, "", out message);
            if (errorCode == 0)
                errorCode = innolasModule.ExchangeCommand(StandardCommandType.SetNumberOfPulsesPerWavelength, pulsesPerWl.ToString(), out message);
            if (!_continuousSweep) //decide if we will be running on continuous sweeping
                sweepCount = repeatCount;
            else
                sweepCount = 0;
            if (errorCode == 0)
                errorCode = innolasModule.ExchangeCommand(StandardCommandType.SetNumberOfSweeps, sweepCount.ToString(), out message);

            if (errorCode == 0)
                errorCode = innolasModule.ExchangeCommand(StandardCommandType.SetNumberOfSweepTableLines, wavelengths.Count.ToString(), out message);
            for (int i = 0; i < wavelengths.Count; i++)
            {
                if (errorCode == 0)
                {
                    NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
                    nfi.NumberGroupSeparator = "";
                    nfi.NumberDecimalSeparator = ".";
                    string valueAsString = wavelengths[i].ToString("F1", nfi);
                    if (LaserParameter.PowerControlMethod == "SweepTableAttenuator")
                    {
                        // wavelength dependant attenuation - untested
                        double attenuation = 100.0 * GetAttenuation((int)Math.Round(wavelengths[i]), LaserParameter.ActualPulseDivider);
                        valueAsString += ("," + attenuation.ToString("F1", nfi));
                    }
                    if (LaserParameter.PowerControlMethod == "GlobalAttenuator")
                    {
                        // wavelength dependant attenuation - untested
                        double attenuation = LaserParameter.GlobalAttenuator;
                        valueAsString += ("," + attenuation.ToString("F1", nfi));
                        //  System.Windows.MessageBox.Show(valueAsString);
                    }
                    errorCode = innolasModule.ExchangeCommand(StandardCommandType.AddSweepTableLine, i + "," + valueAsString, out message);
                    GUI_Communicator.sendError("Laser", "ErrorDisplay", "Added SweepTable Line: " + valueAsString);
                }

            }
            List<string> receivedCommands;
            if (errorCode == 0)
            {
                GUI_Communicator.sendError("Laser", "ErrorDisplay", "Request CHecksum");
                for (int i = 0; i < 3; i++)
                    errorCode = innolasModule.ExchangeCommand(StandardCommandType.IsChecksumValid, "", out receivedCommands, out message);
            }
            if (errorCode == 0)
            {
                GUI_Communicator.sendError("Laser", "ErrorDisplay", "Start Sweep");
                errorCode = innolasModule.ExchangeCommand(StandardCommandType.StartSweep, "", out message);
            }

            if (errorCode == 0) return true;
            else
            {
                /**/  // Log Error  MSOTService.IMSOTLog.HandleError(DisplayName, "Error tuning: " + message + "(" + errorCode + ")");
                GUI_Communicator.sendError("Laser", "ErrorDisplay", "Error tuning: " + message + "(" + errorCode + ")");
                return false;
            }
        }
        #endregion importantpublicmethods

        #region notSoImportantPublicMethods
        public override int GetNextAvailableShotMetrics(out double powerValue, out double wavelength)
        {
            powerValue = 0;
            wavelength = 0;
            PowerSample sample;
            int itemsInFifo;
            lock (_pendingPowerSamples)
            {
                itemsInFifo = _pendingPowerSamples.Count;
                if (itemsInFifo > 0)
                {
                    sample = _pendingPowerSamples.Dequeue();
                    powerValue = sample.Reading;
                    wavelength = sample.Wl;
                }
            }
            //return base.GetNextAvailableShotMetrics(out powerValue, out wavelength);
            return itemsInFifo;
        }
        #endregion notsoimportantpublic methods





        #region privateandprotectedmethods
        bool checkAndUpdateSyncCounter(int newOpo3Counter)
        {
            bool syncErrorDetected = false;
            if (lastOpo3Counter != -1)//uninitialized
            {
                if (newOpo3Counter == 0)
                {
                    if (lastOpo3Counter != UInt16.MaxValue)
                        syncErrorDetected = true;
                }
                else if (newOpo3Counter - lastOpo3Counter != 1)
                {
                    syncErrorDetected = true;
                }
                // Alternative code for Innolas past behavior of increasing the counter undivided.....
                //if (newOpo3Counter < lastOpo3Counter)
                //{
                //    long sum1 = (long)UInt16.MaxValue + (long)newOpo3Counter ;
                //    long sum2 = (long)lastOpo3Counter + (long)_laserTrigerSelector -1;
                //    if( sum1 != sum2)                    
                //        syncErrorDetected = true;
                //}
                //else if (newOpo3Counter - lastOpo3Counter != _laserTrigerSelector)
                //{
                //    syncErrorDetected = true;
                //}
                if (syncErrorDetected)
                {
                    /**/ // HW Monitor                  HardwareMonitorReport((ulong)DeviceErrorCode.Opo3SyncError, new string[] { newOpo3Counter + "/" + lastOpo3Counter });
                    GUI_Communicator.sendError("Laser", "SyncErrorDisplay", newOpo3Counter + "/" + lastOpo3Counter);
                    lastOpo3Counter = -1; //set to uninitialized - until we exit HW
                }
            }
            lastOpo3Counter = newOpo3Counter;
            return !syncErrorDetected;
        }
        public override void GetVersionInformation()
        {
            string message;
            List<string> receivedCommands;
            if (innolasModule.ExchangeCommand(StandardCommandType.GetGuiVersion, "", out receivedCommands, out message) == 0)
            {
                VersionInfo.GUIVersion = receivedCommands[0];
                /**/    // Log Error               MSOTService.IMSOTLog.HandleError(Services.Log.EnumLogType.Info, DisplayName, "SpitLight GUI Version: " + VersionInfo.GUIVersion);
                GUI_Communicator.sendError("Laser", "ErrorDisplay", "SpitLight GUI Version: " + VersionInfo.GUIVersion);
            }
            if (innolasModule.ExchangeCommand(StandardCommandType.GetControllerVersion, "", out receivedCommands, out message) == 0)
            {
                VersionInfo.ControllerFWVersion = receivedCommands[0];
                /**/    //Log Error               MSOTService.IMSOTLog.HandleError(Services.Log.EnumLogType.Info, DisplayName, "Controller FW Version: " + VersionInfo.ControllerFWVersion);
                GUI_Communicator.sendError("Laser", "ErrorDisplay", "Controller FW Version: " + VersionInfo.ControllerFWVersion);
            }
            if (innolasModule.ExchangeCommand(StandardCommandType.GetFPGAVersion, "", out receivedCommands, out message) == 0)
            {
                VersionInfo.FPGAVersion = receivedCommands[0];

                /**/   // Log Error                  MSOTService.IMSOTLog.HandleError(Services.Log.EnumLogType.Info, DisplayName, "FPGA Version: " + VersionInfo.FPGAVersion);
                GUI_Communicator.sendError("Laser", "ErrorDisplay", "FPGA Version: " + VersionInfo.FPGAVersion);
            }
            if (innolasModule.ExchangeCommand(StandardCommandType.GetOPOVersion, "", out receivedCommands, out message) == 0)
            {
                VersionInfo.OPOVersion = receivedCommands[0];
                /**/ //Log Error     MSOTService.IMSOTLog.HandleError(Services.Log.EnumLogType.Info, DisplayName, "OPO Version: " + VersionInfo.OPOVersion);
                GUI_Communicator.sendError("Laser", "ErrorDisplay", "OPO Version: " + VersionInfo.OPOVersion);
            }
            if (innolasModule.ExchangeCommand(StandardCommandType.GetEMONVersion, "", out receivedCommands, out message) == 0)
            {
                VersionInfo.EMONversion = receivedCommands[0];
                /**/  //   Log Error             MSOTService.IMSOTLog.HandleError(Services.Log.EnumLogType.Info, DisplayName, "Energy Monitor Version: " + VersionInfo.EMONversion);
                GUI_Communicator.sendError("Laser", "ErrorDisplay", "Energy Monitor Version: " + VersionInfo.EMONversion);
            }
        }
        protected override void warmUpTask()
        {
            try
            {
                if (lamp(true))
                {
                    int i = 0;
                    _abortWarmUp = false;
                    do
                    {
                        System.Threading.Thread.Sleep(100);
                    } while ((i++ < DMMS.WarmUpInSecs * 10) && (_abortWarmUp == false));
                    /**/ // Status  CommandAcceptInput.Execute(LaserCommand.LampReady);
                    return;
                }
            }
            catch (Exception ex)
            {
                /**/  // Log Error   MSOTService.IMSOTLog.HandleError(DisplayName, "Warm up exception: " + ex.Message);
                GUI_Communicator.sendError("Laser", "ErroDisplay", "Warm up exception: " + ex.Message);
            }

            /**/ //  Log Error          MSOTService.IMSOTLog.HandleError(DisplayName, "Warm up failed.");
            GUI_Communicator.sendError("Laser", "ErroDisplay", "Warm up failed");
            /**/ //Status           CommandAcceptInput.Execute(LaserCommand.LampError);
        }

        public override string CheckShutterState()
        {
            string message = "";
            List<string> expectedCommands = null;
            try
            {
                int errorCode = innolasModule.ExchangeCommand(StandardCommandType.GetShutterState, "", out expectedCommands, out message);
                string Status = expectedCommands[0];
                return Status;
            }
            catch
            {
                string Status = "OPEN";
                return Status;
            }

        }


        protected override void waveTask()
        {
            int errorCode = 0;
            string message;
            List<string> receivedCommands;
            try
            {
                _triggerState = true;
                /**/ // Status                OnPropertyChanged("TriggerState");
                q_switch(true);
                do
                {
                    errorCode = innolasModule.ExchangeCommand(StandardCommandType.GetShutterState, "", out receivedCommands, out message);
                    //GUI_Communicator.sendError("Laser", "ErrorDisplay", "shutter state: "+receivedCommands[0]);
                    if (errorCode == 0)
                    {
                        if (receivedCommands.Count > 0)
                        {
                            //contains PcXXXShutterOff ?
                            if (receivedCommands[0].Contains("CLOSE"))//"ShutterOff"))
                                break; // we are done - exit
                            //else we need to wait until it sweeping is over                              
                        }
                        else //communication error? 
                            errorCode = -100;
                    }
                    System.Threading.Thread.Sleep(20);
                    //GUI_Communicator.sendError("Laser", "ErrorDisplay", "Error Code: " + errorCode);
                } while ((errorCode == 0) && _triggerState);
                if (_triggerState)
                {
                    _triggerState = false;
                    /**/ // Status                   OnPropertyChanged("TriggerState");
                }
                else
                {
                    /**/  // Log Error   MSOTService.IMSOTLog.HandleError(Services.Log.EnumLogType.Info,DisplayName, "Waving has been stopped by trigger off");
                    GUI_Communicator.sendError("Laser", "ErrorDisplay", "Waving has been stopped by trigger off");
                    return;// firing was  already stopped
                }
                if (errorCode == 0)
                {
                    GUI_Communicator.sendStatus("WAVE_TASK", "GUI", "DONE");
                    /**/ //Status                  CommandAcceptInput.Execute(LaserCommand.FastScanDone);
                    return;
                }
                /**/  //  Log Error    MSOTService.IMSOTLog.HandleError(DisplayName, "Error on waving function: (" + errorCode +") " + message );
                GUI_Communicator.sendError("Laser", "ErrorDisplay", "Error on waving function: (" + errorCode + ") " + message);
                //else let it exit from error
            }
            catch (Exception ex)
            {
                /**/   //  Log Error  MSOTService.IMSOTLog.HandleError(DisplayName, "Error on waving function: " + ex.Message);
                GUI_Communicator.sendError("Laser", "ErrorDisplay", "Error on waving function: " + ex.Message);
            }
            /**/ // Status            CommandAcceptInput.Execute(LaserCommand.FastScanError);
        }

        protected override bool tuneChange(object e)
        {
            try
            {
                if (!_continuousSweep) //(wavelengths.Count > 0)
                {
                    if (tune(wavelengths, framesPerWavelength, sweepRepetitions))
                    {
                        return true;
                    }
                }
                else
                {
                    List<double> tempWavelengths = new List<double>();
                    tempWavelengths.Add(WaveLength);
                    if (tune(tempWavelengths, 1, 1))
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                /**/      //   Log Error      MSOTService.IMSOTLog.HandleError(DisplayName, "Tune failed with exception: " + ex.Message);
                GUI_Communicator.sendError("Laser", "ErrorDisplay", "Tune failed with exception: " + ex.Message);
            }
            return false;
        }

        protected override bool powerChange(object e)
        {
            return true;
        }

        protected override bool fireStateChange(object e)
        {
            try
            {
                bool state;
                LaserCommand command = (LaserCommand)Enum.Parse(typeof(LaserCommand), (string)e);
                if (command == LaserCommand.FireOn)
                    state = true;
                else
                    state = false;
                if (q_switch(state))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                /**/ //  Log Error    MSOTService.IMSOTLog.HandleError(DisplayName, "Fire change failed with exception: " + ex.Message);
                GUI_Communicator.sendError("Laser", "ErrorDisplay", "Fire change failed with exception: " + ex.Message);
            }
            return false;
        }

        protected override bool lampOff(object e)
        {
            try
            {
                return lamp(false);
            }
            catch (Exception ex)
            {
                /**/ //    Log Error            MSOTService.IMSOTLog.HandleError(DisplayName, "Lamp off exception: " + ex.Message);
                GUI_Communicator.sendError("Laser", "ErrorDisplay", "Lamp off exception: " + ex.Message);
                return false;
            }
        }




        protected override bool setTriggerRate(double triggerRate)
        {
            try
            {
                int counter = (int)Math.Round(_laserCalibrationFile.LaserUndividedTriggerRate / triggerRate);
                if (counter == 0) counter = 1;
                if (AcceptTriggerChangeAndAttenuationCommands)
                {
                    return sendTriggerChangeCommand(counter);
                }
                else
                {
                    _laserTriggerSelector = counter;
                    return false;
                }
            }
            catch (Exception ex)
            {
                /**/     // Log Error                MSOTService.IMSOTLog.HandleError(DisplayName, "Error setting laser trigger rate: " +ex.Message);
                GUI_Communicator.sendError("Laser", "ErrorDisplay", "Error setting laser trigger rate: " + ex.Message);
            }
            return false;
        }

        protected override bool switchChannel(DataModelLaserInnolas.InnolasChannelSwitch number)
        {
            List<string> receivedCommands;
            string message = "";
            int errorcode = -1;
            try
            {
                if (number != DataModelLaserInnolas.InnolasChannelSwitch.None)
                {
                    string channelPrefix = "CHANNEL";
                    string channelName = channelPrefix + "_" + (int)number;
                    errorcode = innolasModule.ExchangeCommand(StandardCommandType.GetChannelSwitch, "", out receivedCommands, out message);
                    if (errorcode == 0)
                    {
                        if (receivedCommands[0].Contains(channelPrefix))
                        {
                            //check if we already have the correct channel
                            if (!receivedCommands[0].Contains(channelName))
                            {
                                if (laserTrace.TraceVerbose)
                                    /**/ // Log  MSOT   Xvue.MSOT.ViewModels.Log.ViewModelLog.MsotTrace("We need to change fiber channel", laserTrace);
                                    GUI_Communicator.sendError("Laser", "LogMSOT", "We need to change fiber channel");
                                errorcode = innolasModule.ExchangeCommand(StandardCommandType.SetChannelSwitch, channelName, out receivedCommands, out message);
                                if (errorcode == 0)
                                {
                                    if (receivedCommands[0].Contains(channelName))
                                    {
                                        if (laserTrace.TraceVerbose)
                                            /**/ // Log MSOT     Xvue.MSOT.ViewModels.Log.ViewModelLog.MsotTrace("Successfully changed fiber channel to #" + (int)number, laserTrace);
                                            GUI_Communicator.sendError("Laser", "LogMSOT", "Successfully changed fiber channel to #" + (int)number);
                                    }
                                    else errorcode = -102;
                                }
                                else errorcode = -101;
                            }
                        }
                        else errorcode = -100;
                    }
                    if (errorcode != 0)
                    {
                        if (laserTrace.TraceVerbose)
                            /**/ // Log MSOT    Xvue.MSOT.ViewModels.Log.ViewModelLog.MsotTrace("Laser Set Channel Switch failed, Error: " + errorcode + ", " + message, laserTrace);
                            GUI_Communicator.sendError("Laser", "LogMSOT", "Laser Set Channel Switch failed, Error: " + errorcode + ", " + message);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                /**/ // Log Error   MSOTService.IMSOTLog.HandleError(DisplayName, "Laser Set Channel Switch failed, exception: " + ex.Message);
                GUI_Communicator.sendError("Laser", "ErrorDisplay", "Laser Set Channel Switch failed, exception: " + ex.Message);
                return false;
            }
            return true;
        }

        public override bool switchToChannelTwo()
        {
            bool result = switchChannel(DataModelLaserInnolas.InnolasChannelSwitch.Channel2);
            return result;
        }

        public override int getChannel()
        {
            List<string> receivedCommands;
            string message = "";
            int errorcode = -1;
            int Channel = 0;
            try
            {
                {
                    string channelPrefix = "CHANNEL";
                    errorcode = innolasModule.ExchangeCommand(StandardCommandType.GetChannelSwitch, "", out receivedCommands, out message);
                    if (errorcode == 0)
                    {
                        if (receivedCommands[0].Contains("CHANNEL_1") || receivedCommands[0].Contains("CHANNEL_2"))
                        {
                            Channel = int.Parse(receivedCommands[0].Substring(8));
                        }
                        else errorcode = -100;
                    }
                    if (errorcode != 0)
                    {
                        if (laserTrace.TraceVerbose)
                            GUI_Communicator.sendError("Laser", "LogMSOT", "Laser Get Channel Switch failed, Error: " + errorcode + ", " + message);
                        return 0;
                    }
                }
            }
            catch (Exception ex)
            {
                GUI_Communicator.sendError("Laser", "ErrorDisplay", "Laser Set Channel Switch failed, exception: " + ex.Message);
                return 0;
            }
            return Channel;
        }

        public override bool switchbtwChannels(int Channel)
        {
            if (Channel == 2)
            {
                bool result = switchChannel(DataModelLaserInnolas.InnolasChannelSwitch.Channel2);
                return result;
            }
            else
            {
                bool result = switchChannel(DataModelLaserInnolas.InnolasChannelSwitch.Channel1);
                return result;
            }
        }


        public override bool sendTriggerChangeCommand(int counter)
        {
            int errorCode = 0;
            string message = "";
            //return true;
            List<string> receivedCommands;
            errorCode = innolasModule.ExchangeCommand(StandardCommandType.SetPulseDivider, counter.ToString(), out message);
            if (errorCode == 0)
            {
                errorCode = innolasModule.ExchangeCommand(StandardCommandType.GetPulseDivider, "", out receivedCommands, out message);
                if (receivedCommands.Count == 1)
                {
                    if (receivedCommands[0] == counter.ToString())
                    {
                        _laserTriggerSelector = counter;
                    }
                    return true;
                }
            }

            /**/ // log Error  else MSOTService.IMSOTLog.HandleError(DisplayName, "Error (" + errorCode + ") in SetPulseDivider command: " + message);
            else
                GUI_Communicator.sendError("Laser", "ErrorDisplay", "Error (" + errorCode + ") in SetPulseDivider command: " + message);
            return false;
        }

        public override bool setAttenuationViaAttenuator(double attenuation)
        {
            try
            {
                if (AcceptTriggerChangeAndAttenuationCommands)
                {
                    return sendAttenuatorCommand(attenuation);
                }
            }
            catch (Exception ex)
            {
                /**/ //Log Error   MSOTService.IMSOTLog.HandleError(DisplayName, "Error setting attenuation: " + ex.Message);
                GUI_Communicator.sendError("Laser", "ErrorDisplay", "Error setting attenuation: " + ex.Message);
            }
            return false;

        }

        bool sendAttenuatorCommand(double attenuation)
        {
            if (LaserParameter.PowerControlMethod == "GlobalAttenuator")
            {
                int errorCode = 0;
                string message = "";
                errorCode = innolasModule.ExchangeCommand(StandardCommandType.SetAttenuation, attenuation.ToString("F1"), out message);
                if (errorCode == 0)
                {
                    GUI_Communicator.sendStatus("ATTENUATOR_STATE", "GUI", attenuation.ToString());
                    return true;

                }
                /**/ // Log Error  else MSOTService.IMSOTLog.HandleError(DisplayName, "Error (" + errorCode + ") in SetAttenuation command: " + message);
                else
                    GUI_Communicator.sendError("Laser", "ErrorDisplay", "Error (" + errorCode + ") in SetAttenuation command: " + message);
            }
            return false;
        }

        public override bool setAttenuationViaPockelScell(int attenuation)
        {
            try
            {
                if (AcceptTriggerChangeAndAttenuationCommands)
                {
                    return sendPockelScellAttenuateCommand(attenuation);
                }
            }
            catch (Exception ex)
            {
                /**/ // Log Error  MSOTService.IMSOTLog.HandleError(DisplayName, "Error setting pockels cell delay 1: " + ex.Message);
                GUI_Communicator.sendError("Laser", "ErrorDisplay", "Error setting pockels cell delay 1: " + ex.Message);
            }
            return false;
        }
        bool sendPockelScellAttenuateCommand(int attenuation)
        {
            //if (LaserParameter.PowerControlMethod == "PockelsCellAttenuation")
            {
                int errorCode = 0;
                string message = "";
                errorCode = innolasModule.ExchangeCommand(StandardCommandType.SetPockelScellDelay1, attenuation.ToString(), out message);
                if (errorCode == 0)
                    return true;
                /**/ // Log Error  else MSOTService.IMSOTLog.HandleError(DisplayName, "Error (" + errorCode + ") in SetPockelScellDelay1 command: " + message);
                else
                    GUI_Communicator.sendError("Laser", "ErrorDisplay", "Error (" + errorCode + ") in SetPockelScellDelay1 command: " + message);
            }
            return false;
        }
        bool setAttenuationViaAmplifier(int attenuation)
        {
            try
            {
                if (AcceptTriggerChangeAndAttenuationCommands)
                {
                    return sendAmplifierAttenuateCommand(attenuation);
                }
            }
            catch (Exception ex)
            {
                /**/ // Log Error     MSOTService.IMSOTLog.HandleError(DisplayName, "Error setting amplifier delay: " + ex.Message);
                GUI_Communicator.sendError("Laser", "ErrorDisplay", "Error setting amplifier delay: " + ex.Message);
            }
            return false;
        }

        bool sendAmplifierAttenuateCommand(int attenuation)
        {
            if (LaserParameter.PowerControlMethod == "AmplifierAttenuator")
            {
                int errorCode = 0;
                string message = "";
                errorCode = innolasModule.ExchangeCommand(StandardCommandType.SetAmplifierDelay, attenuation.ToString(), out message);
                if (errorCode == 0)
                    return true;
                /**/ // Log  Error   MSOTService.IMSOTLog.HandleError(DisplayName, "Error (" + errorCode + ") in SetAmplifierDelay command: " + message);
                else
                    GUI_Communicator.sendError("Laser", "ErrorDisplay", "Error (" + errorCode + ") in SetAmplifierDelay command: " + message);
            }
            return false;
        }



        void _reenableSimmer(object e)
        {
            Thread.Sleep(100); //wait for Innolas controller simmer state to settle
            reenableSimmer();
        }

        int reenableSimmer()
        {
            int errorcode = 0;
            try
            {
                string message;
                List<string> expectedCommands;
                errorcode = innolasModule.ExchangeCommand(StandardCommandType.GetSimmertState, "", out expectedCommands, out message);
                if (errorcode == 0)
                {
                    if (expectedCommands.Count > 0)
                        if (expectedCommands[0].Contains("OFF"))
                        {
                            DateTime warmUpPfcCheckStart = DateTime.Now;
                            TimeSpan timeoutCheck;
                            do
                            {
                                errorcode = innolasModule.ExchangeCommand(StandardCommandType.CheckPowerCorrectionFactorStatus, "", out message);
                                if (errorcode == 0)
                                {
                                    errorcode = innolasModule.ExchangeCommand(StandardCommandType.SimmerOn, "", out message);
                                    /**/ // Log Error     MSOTService.IMSOTLog.HandleError(Services.Log.EnumLogType.Info, DisplayName, "Simmer On command sent");
                                    GUI_Communicator.sendError("Laser", "ErrorDisplay", "Simmer On command sent");
                                    break;
                                }
                                else if (errorcode == -3)//timeout, laser state cannot accept SimmerOn
                                {
                                    /**/ // Log Error    MSOTService.IMSOTLog.HandleError(Services.Log.EnumLogType.Info, DisplayName, "Laser cannot accept Simmer On command yet");
                                    GUI_Communicator.sendError("Laser", "ErrorDisplay", "Laser cannot accept Simmer On command yet");
                                    errorcode = 0; //so as not to exit the loop
                                }
                                else
                                {
                                    /**/ // Ausgabe  HardwareMonitorReport((ulong)DeviceErrorCode.ReEnableSimmerError2, new string[] { "Unable to get laser status: " + errorcode });

                                    GUI_Communicator.sendStatus("Laser", "HardwareMonitor", "Unable to get laser status: " + errorcode);
                                    break;
                                }
                                timeoutCheck = DateTime.Now - warmUpPfcCheckStart;
                                if (timeoutCheck.TotalSeconds > 6)
                                {
                                    message = "Timeout expecting Pfc state";
                                    errorcode = -101;
                                }
                            } while (errorcode == 0);
                        }
                        else
                        {
                            /**/ // Log Error   MSOTService.IMSOTLog.HandleError(Services.Log.EnumLogType.Info, DisplayName, "No need to set the simmer state");
                            GUI_Communicator.sendError("Laser", "ErrorDisplay", "No need to set the simmer state");
                        }
                    else errorcode = -100;
                }
                if (errorcode < 0)
                {
                    /**/ // Ausgabe   HardwareMonitorReport((ulong)DeviceErrorCode.ReEnableSimmerError1, new string[] { message + ": "+ errorcode.ToString() });
                    GUI_Communicator.sendStatus("Laser", "HardwareMonitor", message + ": " + errorcode.ToString());
                }
            }
            catch (Exception ex)
            {
                /**/  // Ausgabe  HardwareMonitorReport((ulong)DeviceErrorCode.ReEnableSimmerError2,new string[]{ex.Message});
                GUI_Communicator.sendStatus("Laser", "HardwareMonitor", ex.Message);
            }
            return errorcode;
        }

        int reenablePockelSell(out string message)
        {
            int errorCode = 0;
            List<string> expectedCommands;
            //check for pockelsell and reenable if needed
            errorCode = innolasModule.ExchangeCommand(StandardCommandType.GetPockelscellTriggerState, "", out expectedCommands, out message);
            if (errorCode == 0)
            {
                if (expectedCommands.Count == 1)
                {
                    if (expectedCommands[0].Equals("DISABLED", StringComparison.InvariantCultureIgnoreCase))
                    {
                        errorCode = innolasModule.ExchangeCommand(StandardCommandType.SetPockelscellTriggerState, "ENABLED", out expectedCommands, out message);
                    }
                }
            }
            return errorCode;
        }



        /// <summary>
        /// Gets the appropriate laser attenuation based on the wavelength used. The implementation uses the 
        /// trigger rate also if this is a variable trigger rate system
        /// </summary>
        /// <param name="wavelength">The wavelength.</param>
        /// <returns>System.Double.</returns>     
        double GetAttenuation(int wavelength, int pulseDivider)
        {
            try
            {
                return LaserParameter.GetAttenuation(wavelength, pulseDivider);
            }
            catch (Exception ex)
            {
                //MSOTService.IMSOTLog.HandleError(DisplayName, "Error reading Laser attenuation value: " + ex.Message);
                GUI_Communicator.sendError("Laser", "ErrorDisplay", "Error reading Laser attenuation value: " + ex.Message);
            }
            return 0;
        }


        #endregion privateandprotectedmethods

        #region laserPowerLog
        Queue<PowerSample> _pendingPowerSamples;
        struct PowerSample
        {
            internal PowerSample(double reading, double wl)
            {
                Reading = reading;
                Wl = wl;
            }
            internal readonly double Wl;
            internal readonly double Reading;
        }
        #endregion laserPowerLog
    }
}
