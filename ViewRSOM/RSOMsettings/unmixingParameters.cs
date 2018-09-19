using System;
using System.Collections.Generic;

namespace ViewRSOM
{
    public delegate void UnmixParameterChangedEventHandler(string argument);
    public static class unmixingParameters
    {
        public static event UnmixParameterChangedEventHandler UnmixParameterChanged;

        private static void Notify(string argument)
        {
            if (UnmixParameterChanged != null)
            {
                UnmixParameterChanged(argument);
            }
        }

            // total recon progress
        private static int[] _unmixProgressTot;
        private static TimeSpan _remainingUnmixTime;

        // parameters that can be changed in GUI
        public static List<string> _myUnmixComponents_List = new List<string>();
        public static List<string> _myUnmixComponents = new List<string>();
        public static string[] _Components;
        public static string _Spectra;
        public static string _LaserEnergy;
        public static int _Unmixing3D;
        public static int _RegImages;
        public static int _RegImage_interp;



        // public static versions for static GUI parameters
        public static List<string> myUnmixComponents
        {
            get { return _myUnmixComponents; }
            set
            {
                _myUnmixComponents = value;
                Notify("UnmixedComponents");
            }
        }

        public static string[] Components
        {
            get { return _Components; }
            set
            {
                _Components = value;
                Notify("UnmixedComponents");
            }
        }

        //
        public static int[] unmixProgressTot
        {
            get { return unmixProgressTot; }
            set
            {
                unmixProgressTot = value;
                Notify("unmixProgressTot");
            }
        }

        public static TimeSpan remainingUnmixTime
        {
            get { return _remainingUnmixTime; }
            set
            {
                _remainingUnmixTime = value;
                Notify("remainingUnmixTime");

            }
        }

        public static  string Spectra
        {
            get { return _Spectra; }
            set
            {
                _Spectra = value;
                Notify("UnmixingSpectra");
            }
        }

        public static string LaserEnergy
        {
            get { return _LaserEnergy; }
            set
            {
                _LaserEnergy = value;
                Notify("LaserEnergy");
            }
        }

        public static int Unmixing3D
        {
            get { return _Unmixing3D; }
            set
            {
                _Unmixing3D = value;
                Notify("Unmixing3D");
            }
        }

        public static int RegImages
        {
            get { return _RegImages; }
            set
            {
                _RegImages = value;
                Notify("RegisterImages");
            }
        }

        public static int RegImage_interp
        {
            get { return _RegImage_interp; }
            set
            {
                _RegImage_interp = value;
                Notify("InterpolateRegisterImages");
            }
        }



    }
}
