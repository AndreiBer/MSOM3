using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace ViewRSOM.Converters
{


    /// <summary>
    /// Class BooleanAndConverter. Multi-binding converter, performs binary AND on input parameters.
    /// If optional parameter is set to "Invert" it returns the Inverted result of the binary AND.
    /// </summary>
    [ValueConversion(typeof(bool[]), typeof(bool))]
    public class BooleanAndConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool result=false;
            try
            {
                if (values.Length > 0)
                {
                    result = true;
                    foreach (object input in values)
                    {
                        if (input.GetType() != typeof(bool))
                        {
                            result = false;
                            break;
                        }
                        result = result && (bool)input;
                    }
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
