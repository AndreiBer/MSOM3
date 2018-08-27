using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace Xvue.Framework.Views.WPF.Converters
{
    [ValueConversion(typeof(System.String), typeof(System.Windows.Media.Media3D.Vector3D))]
    public class StringToVector3DConverter : IValueConverter
    {
        private System.Windows.Media.Media3D.Vector3D vector3D = new System.Windows.Media.Media3D.Vector3D();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            vector3D = (System.Windows.Media.Media3D.Vector3D)value;
            try
            {
                if (parameter.ToString() == "X") return vector3D.X.ToString("F3", CultureInfo.InvariantCulture);
                else if (parameter.ToString() == "Y") return vector3D.Y.ToString("F3", CultureInfo.InvariantCulture);
                else if (parameter.ToString() == "Z") return vector3D.Z.ToString("F3", CultureInfo.InvariantCulture);
            }
            catch { }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value == null || value.GetType() != typeof(string) || targetType != typeof(System.Windows.Media.Media3D.Vector3D)) return false;

                if (parameter.ToString() == "X") vector3D.X = System.Convert.ToDouble(value, CultureInfo.InvariantCulture);
                else if (parameter.ToString() == "Y") vector3D.Y = System.Convert.ToDouble(value, CultureInfo.InvariantCulture);
                else if (parameter.ToString() == "Z") vector3D.Z = System.Convert.ToDouble(value, CultureInfo.InvariantCulture);
            }
            catch { }
            return vector3D;
        }

    }
}
