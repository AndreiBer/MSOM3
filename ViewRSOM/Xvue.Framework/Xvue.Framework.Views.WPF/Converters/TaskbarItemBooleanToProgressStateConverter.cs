using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shell;

namespace ViewRSOM.Converters
{
    [ValueConversion(typeof(bool), typeof(TaskbarItemProgressState))]
    public class TaskbarItemBooleanToProgressStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                bool item = (bool)value;
                if (item)
                {
                    return TaskbarItemProgressState.Indeterminate;
                }
                else
                {
                    return TaskbarItemProgressState.None;
                }
            }
            catch
            {
                return TaskbarItemProgressState.None;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("This method should never be called");
        }
    }
}
