using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace ViewRSOM.Converters
{
    /// <summary>
    /// Class BooleanOrConverter. Multi-binding converter, performs binary OR on input parameters.
    /// If optional parameter is set to "Invert" it returns the Inverted result of the binary OR.
    /// </summary>
    [ValueConversion(typeof(bool[]), typeof(bool))]
    public class BooleanOrConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool result = false;
            try
            {
                if (values.Length > 0)
                {
                    result = false;
                    foreach (object input in values)
                        result = result || (bool)input;
                }
            }
            catch
            {
                result = false;
            }
            string param = parameter as string;
            if (!String.IsNullOrEmpty(param))
            {
                if (param == "Invert")
                    result = !result;
            }
            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
