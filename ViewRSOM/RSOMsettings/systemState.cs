using System.Windows;
using System.Windows.Media;

namespace ViewRSOM
{

    public delegate void SystemStateChangedEventHandler(string argument);

    public static class systemState
    {

        public static event SystemStateChangedEventHandler SystemStateChanged;

        private static void Notify(string argument)
        {
            if (SystemStateChanged != null) SystemStateChanged(argument);
            // SystemStateChanged?.Invoke(argument);
        }

        // private system state
        private static int _LASERconnected = 0;     // 0: no laser, 1: Wedge, 2: Onda, 3: Elforlight
        private static int _DAQconnected = 0;       // 0: no DAQ, 1: Gage (CompuScope 1520), 2: Alazar (9350)
        private static int _STAGESconnected = 0;    // 0: no stages, 1: PI stages
        private static int _GPUconnected = 0;       // 0: no GPU, 1: nVidia, 2: AMD

        // Active threads
        private static bool _reconThreadFree = true;
        private static bool _acqThreadFree = true;

        // define laser EventHandler
        private static RoutedEventHandler _reconHandle;

        // camera image
        private static ImageSource _currentCameraImage;

        // public versions (for DataBinding)
        public static int LASERconnected
        {
            get { return _LASERconnected; }
            set
            {
                _LASERconnected = value;
                Notify("LASERconnected");
            }
        }

        public static int DAQconnected
        {
            get { return _DAQconnected; }
            set
            {
                _DAQconnected = value;
                Notify("DAQconnected");
            }
        }

        public static int STAGESconnected
        {
            get { return _STAGESconnected; }
            set
            {
                _STAGESconnected = value;
                Notify("STAGESconnected");
            }
        }

        public static int GPUconnected
        {
            get { return _GPUconnected; }
            set
            {
                _GPUconnected = value;
                Notify("GPUconnected");
            }
        }


        public static bool reconThreadFree
        {
            get { return _reconThreadFree; }
            set
            {
                _reconThreadFree = value;
                Notify("reconThreadFree");
            }
        }

        public static bool acqThreadFree
        {
            get { return _acqThreadFree; }
            set
            {
                _acqThreadFree = value;
                Notify("acqThreadFree");
            }
        }
        
        public static RoutedEventHandler reconHandle
        {
            get { return _reconHandle; }
            set
            {
                _reconHandle = value;
                Notify("reconHandle");
            }
        }

        public static ImageSource currentCameraImage
        {
            get { return _currentCameraImage; }
            set
            {
                _currentCameraImage = value;
                Notify("currentCameraImage");
            }
        }

    }
}
