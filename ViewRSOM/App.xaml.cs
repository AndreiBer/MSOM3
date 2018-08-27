using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace ViewRSOM
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Window ActiveWindow
        {
            get
            {
                Window activeWindow = null;
                //find the active window that got the key focus
                foreach (Window wItem in Application.Current.Windows)
                {
                    if (wItem.IsActive)
                    {
                        activeWindow = wItem;
                        break;
                    }
                }
                return activeWindow;
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    TraversalRequest a = new TraversalRequest(FocusNavigationDirection.Next);
                    (e.Source as UIElement).MoveFocus(a);

                }
            }
            catch { }
        }

        private void TextBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                TextBox txt = sender as TextBox;
                txt.CaretIndex = 0;
            }
            catch { }
        }

        private void SelectCurrentItem(object sender, KeyboardFocusChangedEventArgs e)
        {
            ListBoxItem mysender = sender as ListBoxItem;
            if (mysender != null)
                mysender.IsSelected = true;
        }

        private void SelectCurrentTabItem(object sender, KeyboardFocusChangedEventArgs e)
        {
            TabItem mysender = sender as TabItem;
            if (mysender != null)
                mysender.IsSelected = true;
        }

        //
        private void SelectCurrentListViewItem(object sender, KeyboardFocusChangedEventArgs e)
        {
            ListViewItem mysender = sender as ListViewItem;
            if (mysender != null)
                mysender.IsSelected = true;
        }
    }
}
