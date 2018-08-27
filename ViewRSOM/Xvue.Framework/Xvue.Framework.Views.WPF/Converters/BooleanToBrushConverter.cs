using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace ViewRSOM.Converters
{
    /// <summary>
    /// Class BooleanToBrushConverter. Converts a boolean parameter to System.Windows.Media.Brush.
    /// Parameter expected is a System.Windows.Media.Brush array.
    /// First Brush returned if value is True, else Second Brush is returned.
    /// 
    /// Convert back is not supported.
    /// </summary>
    [ValueConversion(typeof(bool), typeof(System.Windows.Media.Brush))]
    public class BooleanToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            System.Windows.Media.Brush[] availableBrushes = (System.Windows.Media.Brush[])parameter;
            if (availableBrushes == null)
                return null;

            System.Windows.Media.Brush result;
            result = availableBrushes[1];
            try
            {                
                if ((bool)value)
                {
                    //var bc = new System.Windows.Media.BrushConverter();
                    result = availableBrushes[0];
                }
            }
            catch
            {
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("This method should never be called");
        }
    }
}
