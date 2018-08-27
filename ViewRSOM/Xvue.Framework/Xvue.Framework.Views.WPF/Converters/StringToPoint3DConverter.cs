using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace Xvue.Framework.Views.WPF.Converters
{
    [ValueConversion(typeof(System.String), typeof(System.Windows.Media.Media3D.Point3D))]
    public class StringToPoint3DConverter : IValueConverter
    {
        private System.Windows.Media.Media3D.Point3D point3D = new System.Windows.Media.Media3D.Point3D();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            point3D = (System.Windows.Media.Media3D.Point3D)value;
            try
            {
                if (parameter.ToString() == "X") return point3D.X.ToString("F3", CultureInfo.InvariantCulture);
                else if (parameter.ToString() == "Y") return point3D.Y.ToString("F3", CultureInfo.InvariantCulture);
                else if (parameter.ToString() == "Z") return point3D.Z.ToString("F3", CultureInfo.InvariantCulture);
            }
            catch { }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value == null || value.GetType() != typeof(string) || targetType != typeof(System.Windows.Media.Media3D.Point3D)) return false;

                if (parameter.ToString() == "X") point3D.X = System.Convert.ToDouble(value, CultureInfo.InvariantCulture);
                else if (parameter.ToString() == "Y") point3D.Y = System.Convert.ToDouble(value, CultureInfo.InvariantCulture);
                else if (parameter.ToString() == "Z") point3D.Z = System.Convert.ToDouble(value, CultureInfo.InvariantCulture);
            }
            catch { }
            return point3D;
        }

    }
}
