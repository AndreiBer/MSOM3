using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace ViewRSOM.Converters
{
    /// <summary>
    /// This converter targets a column header,
    /// in order to take its width to zero when
    /// it should be hidden
    /// </summary>
    public class ColumnWidthConverter
        : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            System.Globalization.CultureInfo culture)
        {
            try
            {
                var isVisible = (bool)value;
                var width = double.Parse(parameter as string);
                return isVisible ? width : 0.0;
            }
            catch
            {
            }
            return 0;
        }


        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            System.Globalization.CultureInfo culture)
        {
            try
            {
                double columnWidth = (double)value;
                if(columnWidth < double.Epsilon)
                {
                    return false;
                }                
            }
            catch { }
            return true;
        }
    }
}
