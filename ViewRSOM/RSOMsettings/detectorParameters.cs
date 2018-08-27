using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace ViewRSOM
{
    public static class detectorParameters
    {

        public static event PropertyChangedEventHandler PropertyChanged;

        private static void Notify(string argument)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(null, new PropertyChangedEventArgs(argument));
            }
        }

        // private static detectorParameters
        private static double _t_focus;
        private static double _focalLength;
        private static double _detectorDiameter;
        private static int _dataSign;

        // derived private static detectorParameters


        // public static versions (for DataBinding) - detectorParameters
        public static double t_focus
        {
            get { return _t_focus; }
            set
            {
                _t_focus = value;
                Notify("t_focus");
            }
        }

        public static double focalLength
        {
            get { return _focalLength; }
            set
            {
                _focalLength = value;
                Notify("focalLength");
            }
        }

        public static double detectorDiameter
        {
            get { return _detectorDiameter; }
            set
            {
                _detectorDiameter = value;
                Notify("detectorDiameter");
            }
        }

        public static int dataSign
        {
            get { return _dataSign; }
            set
            {
                _dataSign = value;
                Notify("dataSign");
            }
        }
        

    }
}
