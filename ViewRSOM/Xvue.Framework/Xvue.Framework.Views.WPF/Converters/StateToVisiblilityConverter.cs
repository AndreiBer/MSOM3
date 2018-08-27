using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows;

namespace Xvue.Framework.Views.WPF.Converters
{
    [ValueConversion(typeof(System.String), typeof(Visibility))]
    public class StateToVisibilityConverter : IValueConverter
    {       

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visibility= Visibility.Hidden;
            try
            {
                if ( value != null )
                {
                    if( value.ToString() == parameter.ToString())
                    {
                        return Visibility.Visible;
                    }
                }
            }
            catch { }
            return visibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

    }
}
