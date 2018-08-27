using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Xvue.Framework.Views.WPF.Controls
{
    public abstract class ViewRulerControlToggleButtonBase : System.Windows.Controls.UserControl
    {
        bool _distanceStateChangeOriginatedByView;
        bool _distanceDrawingCancelledByClickingOnSelf;
        bool _previewMouseDownFired;

        public static readonly DependencyProperty IsCheckedProperty =
        DependencyProperty.Register(
        "IsChecked",
        typeof(bool),
        typeof(ViewRulerControlToggleButtonBase),
        new FrameworkPropertyMetadata());

        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        protected void DistanceToggleButtonOnLoaded(object sender, RoutedEventArgs e)
        {
            AddHandler(Mouse.PreviewMouseDownEvent, new MouseButtonEventHandler(DistanceToggleButtonOnPreviewMouseDown), true);
            // The following is required because PopUp controls prevent the previewMouseDownEvent from firing.
            AddHandler(Mouse.MouseDownEvent, new MouseButtonEventHandler(DistanceToggleButtonOnMouseDown), true);
            _previewMouseDownFired = false;
        }

        protected void DistanceToggleButtonOnPreviewMouseDown(object sender, RoutedEventArgs e)
        {
            _previewMouseDownFired = true;
            _distanceStateChangeOriginatedByView = true;
            if (_distanceDrawingCancelledByClickingOnSelf)
            {
                // Landing here means that the ruler drawing operation was cancelled because the user pressed the distance toggle button while he was drawing the ruler ! So we mark the event as handled in order to avoid triggering the call of the DistanceToggleButton_Checked routine below, resulting in starting ruler drawing again.
                _distanceDrawingCancelledByClickingOnSelf = false;
                e.Handled = true;
                _distanceStateChangeOriginatedByView = false;
            }
        }

        protected void DistanceToggleButtonOnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!_previewMouseDownFired)
            {
                DistanceToggleButtonOnPreviewMouseDown(null, null);
            }
            _previewMouseDownFired = false;
        }

        protected void DistanceToggleButtonOnChecked(object sender, RoutedEventArgs e)
        {
            if (_distanceStateChangeOriginatedByView)
                DistanceToggleButtonOnChanged(true);
        }

        protected void DistanceToggleButtonOnUnchecked(object sender, RoutedEventArgs e)
        {
            DistanceToggleButtonOnChanged(false);
        }

        protected abstract bool WasLastRulerDrawingCanceled();

        protected abstract void SetIsRulerDrawing(bool value);

        public void CancelRulerDrawing()
        {
            SetIsRulerDrawing(false);
        }

        void DistanceToggleButtonOnChanged(bool isChecked)
        {
            try
            {
                if (!_distanceStateChangeOriginatedByView)
                {
                    // Landing here can only mean that the view model notified the IsChecked property which was bound below (line 129). It also means that the IsChecked is false.
                    if (WasLastRulerDrawingCanceled() && this.IsMouseOver)
                        _distanceDrawingCancelledByClickingOnSelf = true;
                }
                else if (isChecked)
                {
                    // Start ruler drawing. Note that there is a DataTrigger in XAML which will bind 'OneWay' the ToggleButton IsChecked property to IsRulerDrawing after that, in order to be able for this view to be notified when the user finishes with (or cancels) the drawing.
                    SetIsRulerDrawing(true);
                }
            }
            catch
            {

            }
            finally
            {
                _distanceStateChangeOriginatedByView = false;
            }
        }
    }
}
