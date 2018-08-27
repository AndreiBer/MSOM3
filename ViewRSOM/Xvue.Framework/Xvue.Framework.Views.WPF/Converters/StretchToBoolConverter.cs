using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace Xvue.Framework.Views.WPF.Converters
{
    [ValueConversion(typeof(System.Windows.Media.Stretch), typeof(bool))]
    public class StretchToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            System.Windows.Media.Stretch result;
            result = System.Windows.Media.Stretch.None;
            try
            {
                if ((bool)value)
                {
                    result = System.Windows.Media.Stretch.Uniform;
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
                if (((System.Windows.Media.Stretch)value) == System.Windows.Media.Stretch.Uniform)
                    result = true;
            }
            catch
            {
            }
            return result;
        }
    }


}
