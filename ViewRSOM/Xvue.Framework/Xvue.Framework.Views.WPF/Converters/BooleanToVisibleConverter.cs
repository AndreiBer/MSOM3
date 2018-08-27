using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace ViewRSOM.Converters
{
    /// <summary>
    /// Class BooleanToVisibleConverter. Converts a boolean parameter to System.Windows.Visibility. 
    /// If optional parameter is set to "Collapsed" false boolean values return System.Windows.Visibility.Collapsed
    /// else System.Windows.Visibility.Hidden;
    /// </summary>
    [ValueConversion(typeof(bool), typeof(System.Windows.Visibility))]
    public class BooleanToVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            System.Windows.Visibility result;
            result = System.Windows.Visibility.Hidden;            
            try
            {                
                if ((bool)value)
                {
                    result = System.Windows.Visibility.Visible;
                }
                else
                {
                    string param = parameter as string;
                    if ( !String.IsNullOrEmpty(param) )
                    {
                        if(param == "Collapsed")
                            result = System.Windows.Visibility.Collapsed;
                    }
                }
            }
            catch
            {
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool result = false;
            try
            {
                if (((System.Windows.Visibility)value) ==  System.Windows.Visibility.Visible)
                    result = true;
            }
            catch
            {
            }
            return result;
        }
    }
}
