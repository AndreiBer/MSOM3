using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace ViewRSOM.Converters
{
    /// <summary>
    /// Class BooleanToWpfToolkitWindowState. Converts a boolean parameter to Xceed.Wpf.Toolkit.WindowState. 
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Xceed.Wpf.Toolkit.WindowState))]
    public class BooleanToWpfToolkitWindowStateConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Xceed.Wpf.Toolkit.WindowState result;
            result = Xceed.Wpf.Toolkit.WindowState.Closed;
            try
            {
                if ((bool)value)
                {
                    result = Xceed.Wpf.Toolkit.WindowState.Open;                    
                }
                else
                {
                    result = Xceed.Wpf.Toolkit.WindowState.Closed;                   
                }
            }
            catch
            {
            }
            return result;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool result = true;
            try
            {
                if (((Xceed.Wpf.Toolkit.WindowState)value) == Xceed.Wpf.Toolkit.WindowState.Open)
                    result = false;
            }
            catch
            {
            }
            return result;
        }


    }
}

