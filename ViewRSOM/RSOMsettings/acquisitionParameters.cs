using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace ViewRSOM
{
    public static class acquisitionParameters
    {

        public static event PropertyChangedEventHandler PropertyChanged;

        private static void Notify(string argument)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(null, new PropertyChangedEventArgs(argument));
            }
        }


        // private static acquisitionParameters

        // ROI
        private static double _y_0;
        private static double _x_0;
        private static double _z_low;
        private static double _z_target;
        private static double _z_high;
        private static double _l_y;
        private static double _l_x;
        // List of ROI for GUI
        public static List<double> z_list = new List<double>();
        private static int _z_listIndex;
        public static List<double> y_list = new List<double>();
        private static int _l_y_listIndex;
        public static List<double> x_list = new List<double>();
        private static int _l_x_listIndex;

        // scan parameters
        private static double _ds;
        private static double _v_x;
        private static double _acc;
        // private static string _laserUsed;
        private static string _laserPort;
        private static int _PRR;
        private static int _laserPower;
        // private static string _DAQUsed;
        private static int _f_s;
        // private static int _inputRange;
        private static int _acqDelay;
        private static double _d_acc;
        private static int _acqRes;
        private static string _acqMode;
        private static int _triggerMode;
        private static int _triggerLevel;
        private static int _numberOfWavelength;
        private static string _ArrayOfWavelength;
        private static int _BscanUpdate;
        private static string _controllerSerialNumber;
        // List of scan parameters for GUI
        public static List<int> inputRange_list = new List<int>();
        private static int _inputRange_listIndex;

        // laser source list
        private static int _laserSourceIndex;
        //public static List<string> laserSource_list = new List<string>();
        public static List<string> laserSource_list = new List<string> { "SW", "MW" };
        private static string _laserSource;

        // GUI plot QuickScan parameters
        private static int _NQuickScan;


        // public static versions (for DataBinding) - acquisitionParameters

        public static string ArrayOfWavelength
        {
            get { return _ArrayOfWavelength; }
            set
            {
                _ArrayOfWavelength = value;
                Notify("ArrayOfWavelength");
            }
        }

        // ROI
        public static double y_0
        {
            get { return _y_0; }
            set
            {
                _y_0 = value;
                Notify("y_0");
            }
        }

        public static double x_0
        {
            get { return _x_0; }
            set
            {
                _x_0 = value;
                Notify("x_0");
            }
        }

        public static double l_y
        {
            get { return _l_y; }
            set
            {
                _l_y = value;
                Notify("l_y");
            }
        }

        public static double l_x
        {
            get { return _l_x; }
            set
            {
                _l_x = value;
                Notify("l_x");
            }
        }

        public static double z_low
        {
            get { return _z_low; }
            set
            {
                _z_low = value;
                Notify("z_low");
            }
        }

        public static double z_target
        {
            get { return _z_target; }
            set
            {
                _z_target = value;
                Notify("z_target");
            }
        }

        public static double z_high
        {
            get { return _z_high; }
            set
            {
                _z_high = value;
                Notify("z_high");
            }
        }

        public static int z_listIndex
        {
            get { return _z_listIndex; }
            set
            {
                _z_listIndex = value;
                Notify("z_listIndex");
            }
        }

        public static int l_y_listIndex
        {
            get { return _l_y_listIndex; }
            set
            {
                _l_y_listIndex = value;
                Notify("l_y_listIndex");
            }
        }

        public static int l_x_listIndex
        {
            get { return _l_x_listIndex; }
            set
            {
                _l_x_listIndex = value;
                Notify("l_x_listIndex");
            }
        }


        // scan parameters
        public static double ds
        {
            get { return _ds; }
            set
            {
                _ds = value;
                Notify("ds");
            }
        }

        //public static string laserUsed
        //{
        //    get { return _laserUsed; }
        //    set
        //    {
        //        _laserUsed = value;
        //        Notify("laserUsed");
        //    }
        //}

        public static string laserPort
        {
            get { return _laserPort; }
            set
            {
                _laserPort = value;
                Notify("laserPort");
            }
        }

        public static int PRR
        {
            get { return _PRR; }
            set
            {
                _PRR = value;
                Notify("PRR");
            }
        }

        public static int laserPower
        {
            get { return _laserPower; }
            set
            {
                _laserPower = value;
                Notify("laserPower");
            }
        }        

        //public static string DAQUsed
        //{
        //    get { return _DAQUsed; }
        //    set
        //    {
        //        _DAQUsed = value;
        //        Notify("DAQUsed");
        //    }
        //}

        public static int f_s
        {
            get { return _f_s; }
            set
            {
                _f_s = value;
                Notify("f_s");
            }
        }

        //public static int inputRange
        //{
        //    get { return _inputRange; }
        //    set
        //    {
        //        _inputRange = value;
        //        Notify("inputRange");
        //    }
        //}

        public static double v_x
        {
            get { return _v_x; }
            set
            {
                _v_x = value;
                Notify("v_x");
            }
        }

        public static double acc
        {
            get { return _acc; }
            set
            {
                _acc = value;
                Notify("acc");
            }
        }

        public static int acqDelay
        {
            get { return _acqDelay; }
            set
            {
                _acqDelay = value;
                Notify("acqDelay");
            }
        }

        public static double d_acc
        {
            get { return _d_acc; }
            set
            {
                _d_acc = value;
                Notify("d_acc");
            }
        }

        public static int acqRes
        {
            get { return _acqRes; }
            set
            {
                _acqRes = value;
                Notify("acqRes");
            }
        }

        public static string acqMode
        {
            get { return _acqMode; }
            set
            {
                _acqMode = value;
                Notify("acqMode");
            }
        }

        public static int triggerLevel
        {
            get { return _triggerLevel; }
            set
            {
                _triggerLevel = value;
                Notify("triggerLevel");
            }
        }

        public static int triggerMode
        {
            get { return _triggerMode; }
            set
            {
                _triggerMode = value;
                Notify("triggerMode");
            }
        }

        public static int numberOfWavelength
        {
            get { return _numberOfWavelength; }
            set
            {
                _numberOfWavelength = value;
                Notify("numberOfWavelength");
            }
        }

        public static int BscanUpdate
        {
            get { return _BscanUpdate; }
            set
            {
                _BscanUpdate = value;
                Notify("BscanUpdate");
            }
        }

        public static string controllerSerialNumber
        {
            get { return _controllerSerialNumber; }
            set
            {
                _controllerSerialNumber = value;
                //_controllerSerialNumber = "118011008";
                _controllerSerialNumber = "117068821";
                Notify("controllerSerialNumber");
            }
        }

        public static int inputRange_listIndex
        {
            get { return _inputRange_listIndex; }
            set
            {
                _inputRange_listIndex = value;
                Notify("inputRange_listIndex");
            }
        }


        // GUI plot QuickScan parameters
        public static int NQuickScan
        {
            get { return _NQuickScan; }
            set
            {
                _NQuickScan = value;
                Notify("NQuickScan");
            }
        }


        // laser source parameters
        public static int laserSourceIndex
        {
            get { return _laserSourceIndex; }
            set
            {
                _laserSourceIndex = value;
                Notify("laserSource");
            }
        }

        public static string laserSource
        {
            get { return _laserSource; }
            set
            {
                _laserSource = value;
                Notify("laser source");
            }
        }
                

        
        



    }
}
