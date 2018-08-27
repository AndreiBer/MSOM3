using System;
using System.Collections.Generic;

namespace ViewRSOM
{

    public delegate void ReconParameterChangedEventHandler(string argument);

    public static class reconstructionParameters
    {

        public static event ReconParameterChangedEventHandler ReconParameterChanged;

        private static void Notify(string argument)
        {
            if (ReconParameterChanged != null)
            {
                ReconParameterChanged(argument);
            }
        }

        // private static reconstructionParameters
        private static int _reflectionFilter;
        private static int _bandpassFilter;
        private static double _f_low;
        private static double _f_mid;
        private static double _f_high;
        private static int _coRegistration;
        private static int _focusModel;
        private static int _bpMode;
        private static double _bpWeight;
        private static double _v_s;
        private static double _zRecon_low;
        private static double _zRecon_high;
        private static int _blockSize;
        private static string _platform;
        private static string _deviceType;
        private static int _dz;
        private static int _ds;
        private static int _maxOffset;
        private static double _y_shiftInd;

        // parameters that can be changed in GUI
        public static List<double> zRecon_list = new List<double>();
        private static int _zRecon_listIndex;
        public static List<string> bandpassSeparation_list = new List<string>();
        private static int _bandpassSeparation;
        public static List<string> motionCorrection_list = new List<string>();
        private static int _motionCorrection;
        public static List<string> movingMAP_list = new List<string>();
        private static int _movingMAP;
        private static int _movingMAP_slabThickness;
        public static List<string> volume3D_list = new List<string>();
        private static int _volume3D;

        // total recon progress
        private static int[] _reconProgressTot;
        private static TimeSpan _remainingReconTime;


        // public static versions for static reconstructionParameters
        public static int reflectionFilter
        {
            get { return _reflectionFilter; }
            set
            {
                _reflectionFilter = value;
                Notify("reflectionFilter");
            }
        }

        public static int bandpassFilter
        {
            get { return _bandpassFilter; }
            set
            {
                _bandpassFilter = value;
                Notify("bandpassFilter");
            }
        }

        public static double f_low
        {
            get { return _f_low; }
            set
            {
                _f_low = value;
                Notify("f_low");
            }
        }

        public static double f_mid
        {
            get { return _f_mid; }
            set
            {
                _f_mid = value;
                Notify("f_mid");
            }
        }

        public static double f_high
        {
            get { return _f_high; }
            set
            {
                _f_high = value;
                Notify("f_high");
            }
        }

        public static int coRegistration
        {
            get { return _coRegistration; }
            set
            {
                _coRegistration = value;
                Notify("coRegistration");
            }
        }

        public static int focusModel
        {
            get { return _focusModel; }
            set
            {
                _focusModel = value;
                Notify("focusModel");
            }
        }

        public static int bpMode
        {
            get { return _bpMode; }
            set
            {
                _bpMode = value;
                Notify("bpMode");
            }
        }

        public static double bpWeight
        {
            get { return _bpWeight; }
            set
            {
                _bpWeight = value;
                Notify("bpWeight");
            }
        }

        public static double v_s
        {
            get { return _v_s; }
            set
            {
                _v_s = value;
                Notify("v_s");
            }
        }

        public static double zRecon_low
        {
            get { return _zRecon_low; }
            set
            {
                _zRecon_low = value;
                Notify("zRecon_low");
            }
        }

        public static double zRecon_high
        {
            get { return _zRecon_high; }
            set
            {
                _zRecon_high = value;
                Notify("zRecon_high");
            }
        }

        public static int blockSize
        {
            get { return _blockSize; }
            set
            {
                _blockSize = value;
                Notify("blockSize");
            }
        }

        public static string platform
        {
            get { return _platform; }
            set
            {
                _platform = value;
                Notify("platform");
            }
        }

        public static string deviceType
        {
            get { return _deviceType; }
            set
            {
                _deviceType = value;
                Notify("deviceType");
            }
        }

        public static int dz
        {
            get { return _dz; }
            set
            {
                _dz = value;
                Notify("dz");
            }
        }

        public static int ds
        {
            get { return _ds; }
            set
            {
                _ds = value;
                Notify("ds");
            }
        }

        public static int maxOffset
        {
            get { return _maxOffset; }
            set
            {
                _maxOffset = value;
                Notify("maxOffset");
            }
        }

        public static double y_shiftInd
        {
            get { return _y_shiftInd; }
            set
            {
                _y_shiftInd = value;
                Notify("y_shiftInd");
            }
        }


        // public static versions for static GUI parameters
        public static int zRecon_listIndex
        {
            get { return _zRecon_listIndex; }
            set
            {
                _zRecon_listIndex = value;
                Notify("zRecon_listIndex");
            }
        }

        public static int bandpassSeparation
        {
            get { return _bandpassSeparation; }
            set
            {
                _bandpassSeparation = value;
                Notify("bandpassSeparation");
            }
        }

        public static int motionCorrection
        {
            get { return _motionCorrection == null ? 0 : _motionCorrection; }
            // get { return _motionCorrection; }
            set
            {
                _motionCorrection = value;
                Notify("motionCorrection");
            }
        }

        public static int movingMAP
        {
            get { return _movingMAP; }
            set
            {
                _movingMAP = value;
                Notify("movingMAP");
            }
        }

        public static int movingMAP_slabThickness
        {
            get { return _movingMAP_slabThickness; }
            set
            {
                _movingMAP_slabThickness = value;
                Notify("movingMAP_slabThickness");
            }
        }

        public static int volume3D
        {
            get { return _volume3D; }
            set
            {
                _volume3D = value;
                Notify("volume3D");
            }
        }


        //
        public static int[] reconProgressTot
        {
            get { return _reconProgressTot; }
            set
            {
                _reconProgressTot = value;
                Notify("reconProgressTot");
            }
        }

        public static TimeSpan remainingReconTime
        {
            get { return _remainingReconTime; }
            set
            {
                _remainingReconTime = value;
                Notify("remainingReconTime");

            }
        }

    }
}