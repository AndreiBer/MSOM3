using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace ViewRSOM.Converterss
{
    public class BooleanOrToVisibleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            System.Windows.Visibility result;
            result = System.Windows.Visibility.Hidden;
            try
            {
                bool visible = false;
                if (values.Length > 0)
                {
                    visible = false;
                    foreach (object input in values)
                        visible = visible || (input == null || input == DependencyProperty.UnsetValue || !(input is bool) ? false : (bool)input);
                }

                if (visible)
                {
                    result = System.Windows.Visibility.Visible;
                }
                else
                {
                    if (parameter != null)
                    {
                        if ((string)parameter == "Collapsed")
                            result = System.Windows.Visibility.Collapsed;
                    }
                }
            }
            catch { }
            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("This method should never be called");
        }
    }
}
