using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewRSOM.Hardware.Laser
{

    public enum LaserState
    {
        Uninitialised,
        Initialising,
        Initialised,
        Emission,
        Closed
    }


    public abstract class ViewModelLaserBase
    {

        #region localvariables
        protected double _waveLength;
        protected int _power;
        protected bool _externalTrigger;
        protected int _pulseRepetitionRate;
        // protected LaserCalibrationTable _laserCalibrationFile;
        #endregion  localvariables

        protected enum DeviceErrorCode //: ulong
        {
            InitFailed,
            Error2
        }

        protected ViewModelLaserBase()
        {
            _waveLength = 532;
            _power = 0;
            _externalTrigger = false;
            _pulseRepetitionRate = 0;
        }


        #region Threads
        //public bool compositeInit()
        //{
        //    initThread = new Thread(new ThreadStart(initTask));
        //    initThread.Name = "Laser Initialization Thread";
        //    initThread.Start();
        //    return true;
        //}
        #endregion Threads

        public abstract void initialize(string laserPort);
        public abstract bool closeConnection();
        public abstract bool setPower();
        public abstract bool setPRR();
        public abstract bool EmissionON();
        public abstract bool EmissionOFF();
        // public abstract void GetLaserState();
        
        public double Wavelength
        {
            get { return _waveLength; }
        }

        public int Power
        {
            get { return _power; }
            set
            {
                _power = value;
            }
        }

        public int PulseRepetitionRate
        {
            get { return _pulseRepetitionRate; }
            set
            {
                _pulseRepetitionRate = value;
            }
        }

        public bool ExternalTrigger
        {
            get { return _externalTrigger; }
            set
            {
                _externalTrigger = value;
            }
        }

    }
}
