using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace Xvue.Framework.Views.WPF.Converters
{
    [ValueConversion(typeof(System.TimeSpan), typeof(System.String))]
    public sealed class TimeSpanToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result = "-";
            try
            {
                TimeSpan a = (TimeSpan) value;
                //result = a.ToString(@"hh\:mm\:ss\.FFF");
                result = a.Hours.ToString("D2") + ":" + a.Minutes.ToString("D2") + ":" + a.Seconds.ToString("D2") + ":" + a.Milliseconds.ToString("D3");                
            }
            catch
            {
                result = "?";
            }
            return result;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
