using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace ViewRSOM.Converters
{
    /// <summary>
    /// Class BooleanToBitmapScalingModeConverter. Converts a boolean parameter to System.Windows.Media.BitmapScalingMode. 
    /// If optional string parameter is specified false boolean values return a smoothing System.Windows.Media.BitmapScalingMode
    /// else System.Windows.Media.BitmapScalingMode.NearestNeighbor
    /// </summary>
    [ValueConversion(typeof(bool), typeof(System.Windows.Media.BitmapScalingMode))]
    public class BooleanToBitmapScalingModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            System.Windows.Media.BitmapScalingMode result = System.Windows.Media.BitmapScalingMode.Linear;
            try
            {
                if ((bool)value)
                {
                    result = System.Windows.Media.BitmapScalingMode.NearestNeighbor;
                }
                else
                {
                    string param = parameter as string;
                    if (!String.IsNullOrEmpty(param))
                    {
                        if ( String.Equals(param, "HighQuality",StringComparison.InvariantCultureIgnoreCase) )
                            result = System.Windows.Media.BitmapScalingMode.HighQuality;
                        else if (String.Equals(param, "LowQuality", StringComparison.InvariantCultureIgnoreCase))
                            result = System.Windows.Media.BitmapScalingMode.LowQuality;
                        else if (String.Equals(param, "Fant", StringComparison.InvariantCultureIgnoreCase))
                            result = System.Windows.Media.BitmapScalingMode.Fant;
                    }
                    else result = System.Windows.Media.BitmapScalingMode.Linear;

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
                if (((System.Windows.Media.BitmapScalingMode)value) == System.Windows.Media.BitmapScalingMode.NearestNeighbor)
                    result = true;
            }
            catch
            {
            }
            return result;
        }
    }
}
