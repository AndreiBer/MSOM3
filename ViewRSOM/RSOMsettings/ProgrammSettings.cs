using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Laser.Parameter;
using System.Windows;

namespace ViewRSOM
{
    public class ProgrammSettings : INotifyPropertyChanged
    {
        
        public event PropertyChangedEventHandler PropertyChanged;
       
        private void Notify(string argument)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(argument));
            }
        }
        
        // public Variables
        public const double minStepsize = 2;
        public const double mediumStepsize = 5;
        public const double maxStepsize = 10;
        public const double minFoV = 10;
        public const double mediumFoV = 25;
        public const double maxFoV = 60;
        public int maxAllowedPulseDivider = 10;
        public int minAllowedPulseDivider = 1;
        public double maxAllowedWavelength = 1350;
        public double minAllowedWavelength = 415;
        public int minAllowedAverages = 30;
        public int laserUndividedTrigger = 50;
        public int defaultOpoScaleMax = 2000;
        public string laserPowerControlMethod = "None"; //"SweepTableAttenuator", "GlobalAttenuator"
               
        

        // private variables
        private string _startingTime = "not defined";
        private double _actualFoVX = mediumFoV;
        private  double _actualFoVY = mediumFoV;
        private  double _actualStepsize = mediumStepsize;
        private  string _dataFileName = "EnergyMeasurement.emd";
        private string _attenuationFileName = "AttenuationValues.dat";
        private string _logFileName = "EnergyMeasurement.log";
        private string _laserSerial = "not defined";
        private  string _fluenceMapFileName = "FluenceMap.dat";
        private  string _folder = "d://";
        private int _laserPulseDivider = 1;
        private int _shotsPerPoint = 100;
        private double _wavelength = 525;
        private double _aperture = 3;
        private string _triggerSource = "ext";
        private string _laserControl = "automatic"; //"automatic"-> Laser is controlled by programm. "manual"-> Laser has to be controlled manually
        private bool _controlsEnabled = true;
        private int _energyMeterError = 1;
        private int _stageError = 1;
        private int _laserError = 1;
        private string _laserIP = LaserParameter.proxyIP;
        private int _laserPort = LaserParameter.proxyPort;
        private bool _laserReady = false;
        private double _minWavelength=415;
        private double _maxWavelength=680;
        private int _wlStepsize = 1;
        private bool _randomizeSweeptable = false;
        private bool _useWithoutEnergyMeter = true;
        private double _energyMeterScaleMax = 50; // specifies the scale of the Energy Graph
        private int _energyMeterRange = 50;
        private double _globalAttenuator = 0.0;
        private string _toolVersion = "Energy Measurement Tool v2.6";
        private int _channel = 1;
        private int _defaultPockelsCellDelay = 170;
        private string _laser_type = "preclinical"; //"clinical" or "preclinical"

        // public versions (for DataBinding)
        public string ToolVersion
        {
            get { return _toolVersion; }
            set
            {
                _toolVersion = value;
                Notify("ToolVersion");
            }
        }
        
        public string StartingTime
        {
            get { return _startingTime; }
            set
            {
                _startingTime = value;
                Notify("StartingTime");
            }
        }
        
        public double ActualFoVX
        {
            get { return _actualFoVX; }
            set
            {
                _actualFoVX = value;
                Notify("ActualFoVX");
            }
        }
        
        public  double ActualFoVY
        {
            get { return _actualFoVY; }
            set
            {
                _actualFoVY = value;
                Notify("ActualFoVY");
            }
        }

        public int WlStepsize
        {
            get { return _wlStepsize; }
            set
            {
                if (value > 300)
                {
                    MessageBox.Show("No Stepsize bigger than 300nm allowed");
                    _wlStepsize = 300;
                    Notify("WlStepsize");
                    //Notify("ScanningTime");
                }
                else
                {
                    _wlStepsize = value;
                    Notify("WlStepsize");
                    //Notify("ScanningTime");
                }

                
            }
        }
        public double ActualStepsize
        {
            get { return _actualStepsize; }
            set
            {
                _actualStepsize = value;
                Notify("ActualStepsize");
            }
        }

        public  string LogFileName
        {
            get { return _logFileName; }
            set
            {
                _logFileName = value;
                Notify("LogFileName");
            }
        }

        public string DataFileName
        {
            get { return _dataFileName; }
            set
            {
                _dataFileName = value;
                Notify("DataFileName");
            }
        }

        public string AttenuationFileName
        {
            get { return _attenuationFileName; }
            set
            {
                _attenuationFileName = value;
                Notify("AttenuationFileName");
            }
        }

        public string FluenceMapFileName
        {
            get { return _fluenceMapFileName; }
            set
            {
                _fluenceMapFileName = value;
                Notify("FluenceMapFileName");
            }
        }

        public string Folder
        {
            get { return _folder; }
            set
            {
                _folder = value;
                Notify("Folder");
            }
        }

        public string LaserSerial
        {
            get { return _laserSerial; }
            set
            {
                _laserSerial = value;
                Notify("LaserSerial");
            }
        }

        public double Wavelength
        {
            get { return _wavelength; }
            set
            {
                _wavelength = value;
                Notify("Wavelength");
            }
        }
        
        public double MinWavelength
        {
            get { return _minWavelength; }
            set
            {
                
                _minWavelength = value;
                
                Notify("MinWavelength");
               
            }
        }

        public double MaxWavelength
        {
            get { return _maxWavelength; }
            set
            {
                
                    _maxWavelength = value;
               
                    Notify("MaxWavelength");
               
            }
        }

        public int ShotsPerPoint
        {
            get { return _shotsPerPoint; }
            set
            {
                if (value > 100000)
                {
                    MessageBox.Show("Not more than 1000 Shots per point allowed");
                    _shotsPerPoint = 100000;
                    Notify("ShotsPerPoint");
                    //Notify("ScanningTime");
                }
                else
                {
                    _shotsPerPoint = value;
                    Notify("ShotsPerPoint");
                    //Notify("ScanningTime");
                }                                     
            }
        }

        

        public int LaserPulseDivider
        {
            get { return _laserPulseDivider; }
            set
            {
                if (value < minAllowedPulseDivider || value > maxAllowedPulseDivider)
                {
                    System.Windows.MessageBox.Show("Only Values " + minAllowedPulseDivider + " - " + maxAllowedPulseDivider + " - are allowed");
                }
                else
                {
                    _laserPulseDivider = value;
                    Notify("LaserPulseDivider");
                    Notify("RepetitionRate");
                }
            }
        }

        public double Aperture
        {
            get { return _aperture; }
            set
            {
                _aperture = value;
                Notify("Aperture");
            }
        }

        public string TriggerSource
        {
            get { return _triggerSource; }
            set
            {
                if (_energyMeterError == 0)
                {
                    _triggerSource = value;
                    Notify("TriggerSource");
                }
                else
                    MessageBox.Show("No EnergyMeter connected");
                
            }
        }

        public string LaserControl
        {
            get { return _laserControl; }
            set
            {
                _laserControl = value;
                Notify("LaserControl");
            }
        }

        public bool ControlsEnabled
        {
            get { return _controlsEnabled; }
            set
            {
                _controlsEnabled = value;
                Notify("ControlsEnabled");
            }
        }

        public int LaserError
        {
            get { return _laserError; }
            set
            {
                _laserError = value;
                Notify("LaserError");
            }
        }

        public int EnergyMeterError
        {
            get { return _energyMeterError; }
            set
            {
                _energyMeterError = value;
                Notify("EnergyMeterError");
            }
        }

        public int StageError
        {
            get { return _stageError; }
            set
            {
                _stageError = value;
                Notify("StageError");
            }
        }

        public int LaserPort
        {
            get { return _laserPort; }
            set
            {
                _laserPort = value;
                Notify("LaserPort");
                LaserParameter.proxyPort = _laserPort;
            }
        }

        public string LaserIP
        {
            get { return _laserIP; }
            set
            {
                _laserIP = value;
                Notify("LaserIP");
                LaserParameter.proxyIP = _laserIP;
            }
        }

        public bool LaserReady
        {
            get { return _laserReady; }
            set
            {
                _laserReady = value;
                Notify("LaserReady");
            }
        }

        public bool RandomizeSweeptable
        {
            get { return _randomizeSweeptable; }
            set
            {
                _randomizeSweeptable = value;
                Notify("RandomizeSweeptable");
            }
        }

        public bool UseWithoutEnergyMeter
        {
            get { return _useWithoutEnergyMeter; }
            set
            {
                _useWithoutEnergyMeter = value;
                Notify("UseWithoutEnergyMeter");
            }
        }

        public double EnergyMeterScaleMax
        {
            get { return _energyMeterScaleMax; }
            set
            {
                if (value < 0.001 && value > 200)
                    System.Windows.MessageBox.Show("Only values between 1 and 200 are allowed");
                else
                    _energyMeterScaleMax = value;
                Notify("EnergyMeterScaleMax");
                
            }
        }

        public int EnergyMeterRange
        {
            get { return _energyMeterRange; }
            set
            {
                if (value > 0 && value < 200)
                    _energyMeterRange = value;
                else
                    System.Windows.MessageBox.Show("Only values between 1 and 200 are allowed");
                Notify("EnergyMeterRange");        
                
            }
        }

        public string RepetitionRate
        {
            get { return ((double)laserUndividedTrigger/_laserPulseDivider).ToString("0.0"); }
            set
            {               
            }
        }

        public double GlobalAttenuator
        {
            get { return _globalAttenuator; }
            set
            {
                if (value > 0 && value < 100)
                    _globalAttenuator = value;
                else
                    System.Windows.MessageBox.Show("Only values between 1 and 100 are allowed"); 
                Notify("GlobalAttenuator");
                LaserParameter.GlobalAttenuator = value;

            }
        }

        public int Channel
        {
            get { return _channel; }
            set
            {
                _channel = value;
                Notify("Channel");

            }
        }

        public int DefaultPockelsCellDelay
        {
            get { return _defaultPockelsCellDelay; }
            set
            {
                _defaultPockelsCellDelay = value;
                Notify("DefaultPockelsCellDelay");
                Notify("ScanningTime");
                Notify("PockelsCellMeasurementMin");
                LaserParameter.PockelscellDelayMin = _defaultPockelsCellDelay;
            }
        }

        public string Laser_Type
        {
            get { return _laser_type; }
            set
            {
                _laser_type = value;
                Notify("Laser_Type");
            }
        }
    }
}
