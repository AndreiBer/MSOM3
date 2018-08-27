using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace ViewRSOM.Converters
{
    [ValueConversion(typeof(System.Double), typeof(System.String))]
    public class DoubleToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result = "-";
            try
            {
                if (System.Convert.ToDouble(value) == double.MaxValue)
                    result = "Max";
                else if (System.Convert.ToDouble(value) == double.MinValue)
                    result = "Min";
                else
                    result = (System.Convert.ToDouble(value)).ToString(((string)parameter), CultureInfo.CurrentCulture);
            }
            catch
            {
                result = "?";
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double result = double.NaN;
            try
            {
                string val = value as string;
                if (val.Equals("max", System.StringComparison.InvariantCultureIgnoreCase))
                    result = double.MaxValue;
                if (val.Equals("min", System.StringComparison.InvariantCultureIgnoreCase))
                    result = double.MinValue;
                else
                    result = System.Convert.ToDouble(value, CultureInfo.CurrentCulture);
            }
            catch
            {

            }
            return result;
        }
    }
}
