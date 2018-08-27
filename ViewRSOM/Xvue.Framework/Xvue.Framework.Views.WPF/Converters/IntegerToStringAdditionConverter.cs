using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Xvue.Framework.Views.WPF.Converters
{

    [ValueConversion(typeof(System.Int32), typeof(System.String))]
    public sealed class IntegerToStringAdditionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result;
            try
            {
                string[] arguments;
                string argumentLine = (string)parameter;
                bool appplyPositive = false;
                Int32 addend = 0;
                Int32 baseAddend = (Int32)value;
                if (!String.IsNullOrEmpty(argumentLine))
                {
                    arguments = argumentLine.Split(' ');
                    foreach(string command in arguments)
                    {
                        if (string.Compare(command, "positive", true) == 0) appplyPositive = true;
                        else if (command.Contains("addend=") )
                        {
                            string[] addendParts = command.Split('=');
                            if(addendParts.Length==2)
                            {
                                addend = System.Convert.ToInt32(addendParts[1]);
                            }
                        }


                    }
                }
                Int32 intResult = baseAddend + addend;
                if (appplyPositive)
                    if (intResult < 0) intResult = 0;
                result = intResult.ToString();
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
