using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace Xvue.Framework.Views.WPF.Converters
{
    [ValueConversion(typeof(object[]), typeof(bool))]
    public class IntToBooleanConverter : IMultiValueConverter
    {
        public object lastParameter;

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool retValue = false;

            try
            {
                if (values == null)
                    return retValue;

                if (values.Length != 2)
                    return retValue;

                lastParameter = values[1];

                if (values[0].Equals(values[1]))
                    retValue = true;
            }
            catch { }

            return retValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            object[] retValue = new object[2]; 
            retValue[0] = retValue[1] = Binding.DoNothing;
            try
            {
                if (value.Equals(true))
                {
                    retValue[0] = lastParameter;
                    retValue[1] = Binding.DoNothing;
                }
            }
            catch { }
            return retValue;
        }
    }

}
