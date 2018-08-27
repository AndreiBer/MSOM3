using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace ViewRSOM.Converters
{
    [ValueConversion(typeof(bool), typeof(System.Windows.Visibility))]
    public class InvertedBooleanToVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            System.Windows.Visibility result;
            result = System.Windows.Visibility.Hidden;
            try
            {
                bool invert = !(bool)value;
                if (invert)
                {
                    result = System.Windows.Visibility.Visible;
                }
                else
                {
                    if (parameter != null)
                    {
                        if( (string)parameter == "Collapsed")
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
                    result = false;
            }
            catch
            {
            }
            return result;
        }
    }
}
