using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace Xvue.Framework.Views.WPF.Controls
{
    /// <summary>
    /// A modified popup control opened by a toggle button. Doesn't re-open when toggle button is pressed while popup is open.
    /// Instead, it is closing.
    /// It also when space keyboard button is pressed on the toggle button it is behaving properly and toggles open/close state.
    /// </summary>
    public class ClosingPopup : Popup
    {
        public FrameworkElement ControlToggleButton
        {
            get { return (FrameworkElement)GetValue(ControlToggleButtonProperty); }
            set { SetValue(ControlToggleButtonProperty, value); }
        }

        public static readonly DependencyProperty ControlToggleButtonProperty =
        DependencyProperty.Register(
         "ControlToggleButton",
         typeof(FrameworkElement),
         typeof(ClosingPopup),
         new FrameworkPropertyMetadata());

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            bool isOpen = this.IsOpen;

            base.OnPreviewMouseLeftButtonDown(e);

            if (ControlToggleButton != null && isOpen && !this.IsOpen)
            {
                if (ControlToggleButton.IsMouseOver)
                {
                    e.Handled = true;
                }
            }
        }

        protected override void OnOpened(EventArgs e)
        {
            if (ControlToggleButton != null)
                ControlToggleButton.PreviewKeyDown += controlToggleButton_PreviewKeyDown;
            base.OnOpened(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            if (ControlToggleButton != null)
                ControlToggleButton.PreviewKeyDown -= controlToggleButton_PreviewKeyDown;
            base.OnClosed(e);
        }

        private void controlToggleButton_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                SetCurrentValue(Popup.IsOpenProperty, false);
                e.Handled = true;
            }
        }

    }
}
