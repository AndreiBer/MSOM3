using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Xvue.Framework.Views.WPF.Converters
{
    /// <summary>
    /// Class BooleanToBitmapScalingModeConverter. Converts a boolean parameter to System.Windows.Media.BitmapScalingMode. 
    /// If optional string parameter is specified false boolean values return a smoothing System.Windows.Media.BitmapScalingMode
    /// else System.Windows.Media.BitmapScalingMode.NearestNeighbor
    /// </summary>
    [ValueConversion(typeof(GridLength), typeof(double))]
    public class GridLengthToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            GridLength result = new GridLength();
            try
            {
                if (value.GetType() == typeof(double))
                {
                    result = new GridLength((double)value);
                }
            }
            catch
            {
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double result = 0;
            try
            {
                GridLength gridLength = (GridLength)value;
                result = gridLength.Value;
            }
            catch
            {
            }
            return result;
        }
    }
}
