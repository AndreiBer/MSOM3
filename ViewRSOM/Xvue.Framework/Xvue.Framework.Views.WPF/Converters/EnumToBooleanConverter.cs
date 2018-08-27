using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace Xvue.Framework.Views.WPF.Converters
{
    [ValueConversion(typeof(System.Enum), typeof(bool))]
    public class EnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool retValue = false;
            try
            {
                if (value.Equals(parameter))
                    retValue = true;
            }
            catch { }
            return retValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object retValue = Binding.DoNothing;
            try
            {
                if (value.Equals(true))
                    retValue = parameter;
            }
            catch { }
            return retValue;
        }
    }

}
