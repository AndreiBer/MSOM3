using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Threading;
using System.IO;
using ViewRSOM.MSOT.DataModels.Plugins.Laser;
using ViewRSOM.MSOT.Hardware.ViewModels.Laser;
using Laser.Parameter;
using System.Diagnostics;
using System.Collections.ObjectModel;
using General.Tools.Communication;



namespace ViewRSOM.MSOT.Hardware.ViewModels.Laser
{
    public enum LaserState 
    { 
        Uninitialised, 
        Initialising,
        Initialised,
        LampWarminUp,
        Flashing,
        Firing, 
        FastScaning,
        //BurstScaning,
        Closed 
    }

    public enum LaserCommand 
    { 
        Init, 
        InitDone, 
        InitError, 
        Tune, 
        SetPower,
        LampOn,
        LampOff,
        LampReady,
        LampError,        
        FireOn, 
        FireOff,
        FastScan,
        FastScanDone,
        FastScanError,
        Close
    };

    public abstract class ViewModelLaserBase //: ViewModelMSOTHardwareDeviceStateMachinePluginBase<LaserState, LaserCommand>, ILaser
    {
       
        #region localvariables
        protected bool _abortWarmUp;
        protected double _minWaveLength, _maxWaveLength;
        protected int _spectrumLength;
        protected int _framesPerWavelength;
        protected int _sweepRepetitions;
        protected bool _continuousSweep;

        
        protected double _waveLength;
        protected int _flashLampCounter;
        protected int _pockelScellCounter;
        protected double _waterFlow;
        protected double _waterTemperature;
        protected double _power;
        protected Thread initThread;
        protected Thread wavingThread;
        protected Thread warmupThread;
        protected bool _triggerState;
        protected double _laserTrigerRate;
        protected DataModelLaserBase DMMS;
        private List<double> _wavelengths;
        internal static TraceSwitch laserTrace = new TraceSwitch("LaserTrace", "Laser Tracing");
        protected LaserCalibrationTable _laserCalibrationFile;
        protected ObservableCollection<double> _availableTriggerRates;        
        protected int _laserTriggerSelector;
        UInt16 _previousUndividedTriggerRate;
        
        #endregion  localvariables

        protected enum DeviceErrorCode : ulong
        {
            InitFailed,
            CalibrationFileError,
            Opo3SyncError,
            InnolasMessageBadArguments,
            InnolasErrorMessage,
            ReEnableSimmerError1,
            ReEnableSimmerError2
        }

        protected ViewModelLaserBase()
        {
            _minWaveLength = _maxWaveLength = 555.0;
            _waveLength = 0;
            _power = 0;
            _wavelengths = new List<double>();
            _availableTriggerRates = new ObservableCollection<double>();
/**/ /*
            AppendErrorCodeDictionary(new Dictionary<ulong, ErrorEventDescriptor>()
            {
                { (ulong) DeviceErrorCode.CalibrationFileError, 
                    new ErrorEventDescriptor() {  errorMessageParts = new string[] {"Calibration file error ",""} } }
            });

            AddTransition(LaserState.Uninitialised, LaserCommand.Init, LaserState.Initialising, compositeInit);
            AddTransition(LaserState.Uninitialised, LaserCommand.Close, LaserState.Closed, compositeClose);            

            AddTransition(LaserState.Initialising, LaserCommand.InitDone, LaserState.Initialised, null);
            AddTransition(LaserState.Initialising, LaserCommand.InitError, LaserState.Uninitialised, null);

            AddTransition(LaserState.Initialised, LaserCommand.LampOn, LaserState.LampWarminUp, lampWarmUp);
            AddTransition(LaserState.Initialised, LaserCommand.Close, LaserState.Closed, compositeClose);

            AddTransition(LaserState.LampWarminUp, LaserCommand.LampReady, LaserState.Flashing, null);
            AddTransition(LaserState.LampWarminUp, LaserCommand.LampError, LaserState.Initialised, null);

            AddTransition(LaserState.Flashing, LaserCommand.Tune, LaserState.Flashing, tuneChange);
            AddTransition(LaserState.Flashing, LaserCommand.SetPower, LaserState.Flashing, powerChange);
            AddTransition(LaserState.Flashing, LaserCommand.FireOn, LaserState.Firing, fireStateChange);
            AddTransition(LaserState.Flashing, LaserCommand.FastScan, LaserState.FastScaning, fastScan);            
            AddTransition(LaserState.Flashing, LaserCommand.LampOff, LaserState.Initialised, lampOff);
            AddTransition(LaserState.Flashing, LaserCommand.Close, LaserState.Closed, compositeClose);

            AddTransition(LaserState.Firing, LaserCommand.FireOff, LaserState.Flashing, fireStateChange);
            AddTransition(LaserState.Firing, LaserCommand.Tune, LaserState.Firing, tuneChange);
            AddTransition(LaserState.Firing, LaserCommand.SetPower, LaserState.Firing, powerChange);
            AddTransition(LaserState.Firing, LaserCommand.Close, LaserState.Closed, compositeClose);

            AddTransition(LaserState.FastScaning, LaserCommand.FastScanDone, LaserState.Flashing, null);
            AddTransition(LaserState.FastScaning, LaserCommand.FireOff, LaserState.Flashing, fireStateChange);
            AddTransition(LaserState.FastScaning, LaserCommand.FastScanError, LaserState.Flashing, null);          
 /**/ 
        }


        #region Threads
        public bool compositeInit()
        {
            initThread = new Thread(new ThreadStart(initTask));
            initThread.Name = "Laser Initialization Thread";
            initThread.Start();
            return true;
        }
        
        protected bool lampWarmUp(object e)
        {
            warmupThread = new Thread(new ThreadStart(warmUpTask));
            warmupThread.Name = "Lamp Warm up Task";
            warmupThread.Start();
            return true;
        }

        public bool fastScan()
        {
            wavingThread = new Thread(new ThreadStart(waveTask));
            wavingThread.Start();
            return true;        
        }

        public bool stopScan()
        {
            wavingThread.Abort();
            return true;
        }
        #endregion Threads

        public void SetSpeedDataModel(ViewRSOM.Framework.API.DataModels.Base.DataModelPluginConfiguration data)
        {
            DMMS = data as DataModelLaserBase;
        }

        

        public abstract void initTask();
        public abstract bool compositeClose();

        protected abstract void warmUpTask();        
        

        protected abstract void waveTask();
        protected abstract bool tuneChange(object e);
        protected abstract bool powerChange(object e);
        protected abstract bool fireStateChange(object e);
        protected abstract bool lampOff(object e);

        protected abstract bool setTriggerRate(double triggerRate);
        protected abstract bool switchChannel(DataModelLaserInnolas.InnolasChannelSwitch number);

        public abstract bool tune(List<double> wavelengths, int pulsesPerWl, int repeatCount);
        public abstract string getSweepStateAsString();
        public abstract bool lamp(bool state);
        public abstract void setWavelength(int wl);
        public abstract void illuminationON();
        public abstract void illuminationOFF();
        public abstract int[] retrieveWL(string comment);

        public bool AcceptTriggerChangeAndAttenuationCommands;
        public double Attenuation;
        public abstract bool switchToChannelTwo();       
        public abstract bool sendTriggerChangeCommand(int counter);
        public abstract bool setAttenuationViaPockelScell(int attenuation);

        public abstract void GetLaserState();
        public abstract bool q_switch(bool state);
        public abstract void AfterInitialize();
        public abstract bool StartChargerChange();
        public abstract bool setAttenuationViaAttenuator(double attenuation);
        public abstract void GetVersionInformation();
        public abstract string CheckShutterState();
        

        protected void checkWavelengthRange()
        {
            /**/            
            string presetRange = LaserParameter.SystemMinAllowedWavelength.ToString("F1") + "nm-" + LaserParameter.SystemMaxAllowedWavelength.ToString("F1") + "nm";
            string laserRange = LaserParameter.LaserMinAllowedWavelength.ToString("F1") + "nm-" + LaserParameter.LaserMaxAllowedWavelength.ToString("F1") + "nm";
            string diagnosticMessage = "System allowed wavelength range (" + presetRange + "). Laser capabilities (" + laserRange + ")";
            //if (laserTrace.TraceVerbose)
                /**/ //    Xvue.MSOT.ViewModels.Log.ViewModelLog.MsotTrace(diagnosticMessage, laserTrace);
                GUI_Communicator.sendStatus("Laser", "DiagnosticDisplay", diagnosticMessage);
            //if ((MSOTService.LaserMinAllowedWavelength < MinWaveLength) ||
            //    (MSOTService.LaserMaxAllowedWavelength > MaxWaveLength))
            //{
            //    MSOTService.NotifyUserOnAction("Preset wavelength range (" + presetRange + ") exceeds laser capabilities (" + laserRange + ").\n Presets may cause errors.", DisplayName, false, true);
            //}
            //else
            //{
            //    MSOTService.IMSOTLog.HandleError(Services.Log.EnumLogType.Info,DisplayName,"Preset wavelength range (" + presetRange + "). Laser capabilities (" + laserRange + ")");
            //}


        }


        //protected override bool CanAcceptInput(object input)
        //{
        //    bool retVal = base.CanAcceptInput(input);
        //    System.Diagnostics.Debug.WriteLine(DisplayName + ", CurrentState:" + CurrentState.ToString() + ", Input:" + input.ToString() + " - CanAccept: " + retVal.ToString());                        
        //    return retVal;
        //}

        protected bool Load()
        {
            
                _previousUndividedTriggerRate = 0;
                _flashLampCounter = 0;
                _waveLength = DMMS.Wavelength;
                _power = DMMS.Power;
/**/ //         DataModelOptoAcousticDetector selectedDetector = MSOTService.IMSOTHardware.SelectedDetector;
/**/ /*                
            if (SwitchDetector(selectedDetector.LaserChannelNumber, selectedDetector.LaserCalibrationFilePath))
                {
                    StartMachine(LaserState.Uninitialised);
                    CommandAcceptInput.Execute(LaserCommand.Init);
                    return true;
                }  
      * 
/**/
            
            return false;

        }

        public bool SwitchDetector(DataModelLaserInnolas.InnolasChannelSwitch channelNumber, string calibrationFilePath)
        {
            try
            {
                _laserCalibrationFile = new LaserCalibrationTable();
                string errorMessage;
                if (!_laserCalibrationFile.LoadFile(calibrationFilePath, out errorMessage))
                {
                    /**/ //  HardwareMonitorReport((ulong)DeviceErrorCode.CalibrationFileError, new string[] { errorMessage });
                    GUI_Communicator.sendStatus("Laser", "HardwareMonitor", errorMessage);   
                }
                else
                {
                    if(_previousUndividedTriggerRate != 0)
                    {
                        if( _previousUndividedTriggerRate != _laserCalibrationFile.LaserUndividedTriggerRate )
                        {
                            /**/ //       MSOTService.IMSOTLog.HandleError(EnumLogType.Warning, DisplayName, 
                            /**/ //           "Laser Calibration file defines a different undivided trigger rate than the previous file, new: " + _laserCalibrationFile.LaserUndividedTriggerRate +
                            /**/ //    " old: "+_previousUndividedTriggerRate);
                            GUI_Communicator.sendError("Laser", "ErrorDisplay","Laser Calibration file defines a different undivided trigger rate than the previous file, new: " + _laserCalibrationFile.LaserUndividedTriggerRate + " old: "+_previousUndividedTriggerRate);   
                        }
                    }
                    _previousUndividedTriggerRate = _laserCalibrationFile.LaserUndividedTriggerRate;
                    /**/ //     MSOTService.IMSOTLog.HandleError(EnumLogType.Info, DisplayName, "Laser Calibration file info: " + _laserCalibrationFile.Description);
                    GUI_Communicator.sendStatus("Laser", "ErrorDisplay", "Laser Calibration file info: " + _laserCalibrationFile.Description);   
                    string laserFileSummary;
                    AvailableTriggerRates = new ObservableCollection<double>(_laserCalibrationFile.GetTriggerRates(out laserFileSummary));
                    switchChannel(channelNumber);
                    /**/ //  MSOTService.IMSOTLog.HandleError(EnumLogType.Info, DisplayName, laserFileSummary);
                    GUI_Communicator.sendStatus("Laser", "ErrorDisplay", laserFileSummary);
                    return true;
                }
            }
            catch (Exception ex)
            {
                /**/ //  HardwareMonitorReport((ulong)DeviceErrorCode.CalibrationFileError, new string[] { ex.Message });
                GUI_Communicator.sendStatus("Laser","HardwareMonitor",ex.Message);
            }
            return false;
        }

        protected bool Unload()
        {
            try
            {                
/**/ //                if ( !CommandAcceptInput.CanExecute(LaserCommand.Close))
                {
                    int maxWait = 0;                    
/**/ //                    while (!CommandAcceptInput.CanExecute(LaserCommand.Close) )
                    {
                        _abortWarmUp = true;
                        Thread.Sleep(1000);
                        if (maxWait++ > 5)
                        {
                            /**/ //  MSOTService.IMSOTLog.HandleError(DisplayName, "Error unloading: timeout");
                            GUI_Communicator.sendStatus("Laser","ErrorDisplay","Error unloading: timeout");
 /**/ //                           break;
                        }
                    }
                }
 /**/ //               CommandAcceptInput.Execute(LaserCommand.Close);
                return true;
            }
            catch (Exception ex)
            {
                /**/ //        MSOTService.IMSOTLog.HandleError(DisplayName, "Error unloading: " + ex.Message);
                GUI_Communicator.sendError("Laser", "ErrorDisplay", "Error unloading: " + ex.Message);
            }
            return false;
        }

        public double MaxWaveLength
        {
            get { return _maxWaveLength; }
        }

        public double MinWaveLength
        {
            get { return _minWaveLength; }
        }

        public double LaserTriggerRate 
        { 
            get
            {
                if (_laserTrigerRate > 0)
                    return _laserTrigerRate;
                else return 1;
            }
            set
            {
                if (value != _laserTrigerRate)
                {
                    int triggerRateIndex;
                    bool sendLaserCommand = false;
                    if (validateNewTriggerRate(value))
                    {
                        triggerRateIndex = GetSafeTriggerRateIndex(value);
                        sendLaserCommand = true;                        
                    }
                    else //we should not change
                    {
                        triggerRateIndex = GetSafeTriggerRateIndex(_laserTrigerRate);
                        if (_laserTrigerRate != AvailableTriggerRates[triggerRateIndex])
                            sendLaserCommand = true;
                    }
                    if(sendLaserCommand)
                    {
                        double newValue = AvailableTriggerRates[triggerRateIndex];
                        /**/ //    MSOTService.IMSOTLog.HandleError(EnumLogType.Info, DisplayName, "Trigger rate changed to " + newValue + "Hz");
                        GUI_Communicator.sendError("Laser", "ErrorDisplay", "Trigger rate changed to " + newValue + "Hz");
                        setTriggerRate(newValue);
                    }
                    //set the double value reference to the ItemSource list                    
                   
                        _laserTrigerRate = AvailableTriggerRates[triggerRateIndex];
/**/ //                        OnPropertyChanged("LaserTriggerRate");
                    
                    
                }                
            }
        }

        int GetSafeTriggerRateIndex( double oldValue)
        {
            int triggerRateIndex;
            double newValue = oldValue;
            triggerRateIndex = AvailableTriggerRates.IndexOf(newValue);
            if (triggerRateIndex < 0) //we do not support that exact rate!!!, fall back to the minimum trigger rate
            {
                newValue = AvailableTriggerRates.Min();
                /**/ //    MSOTService.NotifyUserOnAction("The requested laser trigger rate (" + oldValue + "Hz) is not supported by the laser.\nTrigger rate set to " + newValue + "Hz", "Warning", false, true);
                GUI_Communicator.sendStatus("Laser", "UserWarning", "The requested laser trigger rate (" + oldValue + "Hz) is not supported by the laser.\nTrigger rate set to " + newValue + "Hz");
                triggerRateIndex = AvailableTriggerRates.IndexOf(newValue);
            }
            return triggerRateIndex;
        }

        bool validateNewTriggerRate(double requestedLaserTriggerRate)
        {
            try
            {
 /*              
                if (MSOTService.IMSOTHardware.IManualScan.CurrentReconPresetMaxSupportedFrameRate > 0)
                {
                    if (MSOTService.IMSOTHardware.IManualScan.CurrentReconPresetMaxSupportedFrameRate < requestedLaserTriggerRate)
                    {
                        MSOTService.NotifyUserOnAction("Preset maximum frame rate (" + MSOTService.IMSOTHardware.IManualScan.CurrentReconPresetMaxSupportedFrameRate + "Hz) exceeds requested laser trigger rate (" + requestedLaserTriggerRate + "Hz)", "Warning", false, true);
                        return false;
                    }
                }
 */
            }
            catch (Exception ex)
            {
                /**/ //    MSOTService.IMSOTLog.HandleError(DisplayName, "Error validating trigger rate" + ex.Message);
                GUI_Communicator.sendError("Laser", "ErrorDisplay", "Error validating trigger rate" + ex.Message);
                return false;
            }
            return true;
        }

        public abstract bool IsInterlockOpen { get; }

        public bool IsLaserTriggerAdjustable { get { return AvailableTriggerRates.Count > 1; } }
        
        public virtual ObservableCollection<double> AvailableTriggerRates
        {
            get { return _availableTriggerRates; }
            protected set 
            { 
                _availableTriggerRates = value;
/**/ //                OnPropertyChanged("AvailableTriggerRates");
            }
        }
                
        #region ILaser Members

        public virtual string LaserSN { get { return DMMS.LaserSN; } }

        public int FlashLampCounter 
        {
            get { return _flashLampCounter; }
            protected set
            {
                if (_flashLampCounter != value)
                {
                    _flashLampCounter = value;
 /**/ //                   OnPropertyChanged("FlashLampCounter");
                }
            }
        }

        public int PockelScellCounter
        {
            get { return _pockelScellCounter; }
            protected set
            {
                if (_pockelScellCounter != value)
                {
                    _pockelScellCounter = value;
 /**/ //                   OnPropertyChanged("PockelScellCounter");
                }
            }
        }

        public double WaterFlow
        {
            get { return _waterFlow; }
            protected set
            {
                if (_waterFlow != value)
                {
                    _waterFlow = value;
/**/ //                    OnPropertyChanged("WaterFlow");
                }
            }
        }

        public double WaterTemperature
        {
            get { return _waterTemperature; }
            protected set
            {
                if (_waterTemperature != value)
                {
                    _waterTemperature = value;
/**/ //                    OnPropertyChanged("WaterTemperature");
                }
            }
        }

        public double WaveLength
        {
            get { return _waveLength; }
            protected set
            {
                try
                {
                    _waveLength = value;
/**/ //                    OnPropertyChanged("WaveLength");
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Value is not a valid " + ex.Message);
                }                
            }
        }

        /// <summary>
        /// The low power setting
        /// </summary>
        public double NominalPower { get { return DMMS.Power; } }

        /// <summary>
        /// The low power setting
        /// </summary>
        public virtual double LowPower { get { return DMMS.LowPower; } }

        /// <summary>
        /// The current power as %.
        /// </summary>
        public double Power
        {
            get { return _power; }
            private set
            {
                try
                {
                    _power = value;
/**/ //                    OnPropertyChanged("Power");
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Value is not a valid " + ex.Message);
                }
            }
        }

        public abstract int GetNextAvailableShotMetrics(out double powerValue, out double wavelength);

        protected int framesPerWavelength
        {
            get { return _framesPerWavelength; }
            set
            {
                if (value != _framesPerWavelength)
                {
                    _framesPerWavelength = value;
                }
            }
           
        }

        protected int sweepRepetitions
        {
            get { return _sweepRepetitions; }
            set
            {
                if (value != _sweepRepetitions)
                {
                    _sweepRepetitions = value;
                }
            }
        }

        protected List<double> wavelengths
        {
            get { return _wavelengths; }
        }

        public bool SetPower(double powerAsPercent)
        {
            try
            {
/**/ //                if (CommandAcceptInput.CanExecute(LaserCommand.SetPower))
                {
                    Power = powerAsPercent;                    
/**/ //                    CommandAcceptInput.Execute(LaserCommand.SetPower);
                    return true;
                }
  //              else
                    /**/ //     MSOTService.IMSOTLog.HandleError(DisplayName, "Unable to set power to: " + powerAsPercent.ToString("F2") + ", laser state: " + CurrentState.ToString());
                       }
            catch (Exception ex)
            {
                /**/ //                MSOTService.IMSOTLog.HandleError(DisplayName, "Error setting power: " + ex.Message);
                GUI_Communicator.sendError("Laser", "ErrorDisplay", "Error setting power: " + ex.Message);
            }
            return false;
        }

        public bool Tune(double waveLength)
        {
            try
            {
 /**/ //               if (CommandAcceptInput.CanExecute(LaserCommand.Tune))
                {
                    wavelengths.Clear();
                    WaveLength = waveLength;
                    _continuousSweep = true;
/**/ //                    CommandAcceptInput.Execute(LaserCommand.Tune);
                    return true;
                }
     //           else
/**/ //                    MSOTService.IMSOTLog.HandleError(DisplayName, "Unable to tune to: "+ waveLength.ToString("F2")+", laser state: " + CurrentState.ToString());
            }
            catch (Exception ex)
            {
                /**/ //   MSOTService.IMSOTLog.HandleError(DisplayName, "Error tuning: " + ex.Message);
                GUI_Communicator.sendError("Laser", "ErrorDisplay", "Error tuning: " + ex.Message);
            }
            return false;
        }

        public bool Tune(IList<double> newWavelengths, int pulsesPerWl, int repetitionCount)
        {
            try
            {
/**/ //                if (CommandAcceptInput.CanExecute(LaserCommand.Tune))
                {
                    framesPerWavelength = pulsesPerWl;
                    sweepRepetitions = repetitionCount;
                    wavelengths.Clear();                    
                    foreach (double val in newWavelengths)
                        wavelengths.Add(val);
                    _continuousSweep = false;
/**/ //                    CommandAcceptInput.Execute(LaserCommand.Tune);
                    return true;
                }
      //          else
/**/ //                    MSOTService.IMSOTLog.HandleError(DisplayName, "Unable to tune, laser state: " + CurrentState.ToString());
            }
            catch (Exception ex)
            {
                /**/ //     MSOTService.IMSOTLog.HandleError(DisplayName, "Error tuning: " + ex.Message);
                GUI_Communicator.sendError("Laser", "ErrorDisplay", "Error tuning: " + ex.Message);
            }
            return false;
        }

        public bool DoFastScan()
        {
            try
            {
/**/ //                if (CommandAcceptInput.CanExecute(LaserCommand.FastScan))
                {
/**/ //                    CommandAcceptInput.Execute(LaserCommand.FastScan);
                    return true;
                }
      //          else
/**/ //                    MSOTService.IMSOTLog.HandleError(DisplayName, "Unable to start waving scan, laser state: " + CurrentState.ToString());
            }
            catch (Exception ex)
            {
                /**/ //   MSOTService.IMSOTLog.HandleError(DisplayName, "Error starting waving mode: " + ex.Message);
                GUI_Communicator.sendError("Laser", "ErrorDisplay", "Error starting waving mode: " + ex.Message);
            }
            return false;
        }

        public bool LampState(bool state)
        {
            try
            {
                LaserCommand command;
                if (state)
                    command = LaserCommand.LampOn;
                else
                    command = LaserCommand.LampOff;
/**/ //                if (CommandAcceptInput.CanExecute(command))
                {
/**/ //                    CommandAcceptInput.Execute(command);
                    return true;
                }
            }
            catch (Exception ex)
            {
                /**/ //   MSOTService.IMSOTLog.HandleError(DisplayName, "Turning flash lamp on failed with exception:" + ex.Message);
                GUI_Communicator.sendError("Laser", "ErrorDisplay", "Turning flash lamp on failed with exception:" + ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Enable or Disable the Q switch of the Laser. This effectively also enables or disables 
        /// the trigger pulse created by the Laser.
        /// </summary>
        /// <param name="state">If true Q-Switch is set to On, else to Off </param>
        /// <returns>Depending on current Laser State the command may not be acceptable. If command is accepted function
        /// returns true, else false</returns>
        bool Firing(bool state)
        {
            try
            {       
                LaserCommand command;
                if(state )
                    command = LaserCommand.FireOn;
                else
                    command = LaserCommand.FireOff;
 /**/ //               if (CommandAcceptInput.CanExecute(command))
                {
/**/ //                    CommandAcceptInput.Execute(command);
                    if (state)// try to switch on
                    {
                        if (LaserParameter.CurrentState == LaserState.Firing.ToString()) return true;
                    }
                    else //// try to switch off
                    {
                        if (LaserParameter.CurrentState == LaserState.Flashing.ToString()) return true;
                    }
                }
            }
            catch (Exception ex)
            {
                /**/ //    MSOTService.IMSOTLog.HandleError(DisplayName, "Fire change attempt failed with exception:" + ex.Message);
                GUI_Communicator.sendError("Laser", "ErrorDisplay", "Fire change attempt failed with exception:" + ex.Message);
            }
            return false;
        }

        public bool LampReady 
        {
            get
            {
                try
                {
                    return ( LaserParameter.CurrentState == LaserState.Flashing.ToString() );
                }                               
                catch (Exception ex)
                {
                    /**/ //    MSOTService.IMSOTLog.HandleError(DisplayName, "Exception enabling trigger: " + ex.Message);
                    GUI_Communicator.sendError("Laser", "ErrorDisplay", "Exception enabling trigger: " + ex.Message);
                    return false;
                }

            }
        }
        public bool TriggerState
        {
            get
            {
                return _triggerState;
            }
            set
            {
                //send command to hardware
                try
                {
                    if (Firing(value))
                    {
                        //change trigger state only if Firing change succeeds
                        _triggerState = value;
                    }
     //               else
/**/ //                        MSOTService.IMSOTLog.HandleError(Services.Log.EnumLogType.Warning,DisplayName, "Unable to change trigger state, laser state: " + CurrentState.ToString());                        
/**/ //                    OnPropertyChanged("TriggerState");
                }
                catch (Exception ex)
                {
                    /**/ //    MSOTService.IMSOTLog.HandleError(DisplayName, "Exception enabling trigger: " + ex.Message);
                    GUI_Communicator.sendError("Laser", "ErrorDisplay", "Exception enabling trigger: " + ex.Message);
                    
                }

            }
        }        

        public double GetAveragePower(int wavelength)
        {            
            try
            {
                return _laserCalibrationFile.GetAveragePower(wavelength, _laserTriggerSelector);
            }
            catch (Exception ex)
            {
                /**/ //  MSOTService.IMSOTLog.HandleError(DisplayName, "Error reading average power value: " + ex.Message);
                GUI_Communicator.sendError("Laser", "ErrorDisplay", "Error reading average power value: " + ex.Message);
            }
            return 0;
        }
        public double GetPowerEstimation(int wavelength, double x)
        {            
            try
            {
                return _laserCalibrationFile.GetPowerEstimation(wavelength, _laserTriggerSelector, x);
            }
            catch (Exception ex)
            {
                /**/ //  MSOTService.IMSOTLog.HandleError(DisplayName, "Error reading power estimation value: " + ex.Message);
                GUI_Communicator.sendError("Laser", "ErrorDisplay", "Error reading power estimation value: " + ex.Message);
            }
            return 0;
        }        
        #endregion


        //private  DelegateCommand tuneCommand;
        //public ICommand TuneCommand
        //{
        //    get
        //    {
        //        if (tuneCommand == null)
        //        {
        //            tuneCommand = new DelegateCommand(TuneHelper);
        //        }
        //        return tuneCommand;
        //    }
        //}

        //void TuneHelper()
        //{
        //    CommandAcceptInput.Execute(LaserCommand.Tune);
        //}
    }
}
