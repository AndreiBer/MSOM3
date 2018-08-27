using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;

namespace ViewRSOM.Converters
{
    [ValueConversion(typeof(ListViewItem), typeof(System.Windows.Media.Brush))]
    public class BackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                ListViewItem item = value as ListViewItem;
                ListView listView = ItemsControl.ItemsControlFromItemContainer(item) as ListView;
                // Get the index of a ListViewItem
                int index = listView.ItemContainerGenerator.IndexFromContainer(item);

                if (index % 2 == 0)
                {
                    return Brushes.LightBlue;
                }
                else
                {
                    return Brushes.White;
                }
            }
            catch
            {
                return Brushes.Beige;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("This method should never be called");
        }
    }
}
