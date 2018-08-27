using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows;

namespace Xvue.Framework.Views.WPF.Controls
{
    public sealed class StoryboardControlledClosingPopup : ClosingPopup, IDisposable
    {

        public readonly static int MaxOpenPeriodExtensions = 2;
        int _openPeriodsElapsed;
        int _openPeriodExtensiosTarget;
        bool _openForSecondsChanged;
        Timer _timer;
        

        public double OpenForSeconds
        {
            get { return (double)GetValue(OpenForSecondsProperty); }
            set { SetValue(OpenForSecondsProperty, value); }
        }

        public static readonly DependencyProperty OpenForSecondsProperty =
        DependencyProperty.Register(
        "OpenForSeconds",
        typeof(double),
        typeof(StoryboardControlledClosingPopup),
        new FrameworkPropertyMetadata(
        new PropertyChangedCallback(OpenForSecondsPropertyChanged)));

        private static void OpenForSecondsPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue != null && (double)e.NewValue > 0)
            {
                try
                {
                    StoryboardControlledClosingPopup control = source as StoryboardControlledClosingPopup;
                    control.OnOpenForSecondsChanged();
                }
                catch { }
            }
        }

        public bool CloseImmediately
        {
            get { return (bool)GetValue(CloseImmediatelyProperty); }
            set { SetValue(CloseImmediatelyProperty, value); }
        }

        public static readonly DependencyProperty CloseImmediatelyProperty =
        DependencyProperty.Register(
        "CloseImmediately",
        typeof(bool),
        typeof(StoryboardControlledClosingPopup),
        new FrameworkPropertyMetadata(
        new PropertyChangedCallback(CloseImmediatelyPropertyChanged)));

        private static void CloseImmediatelyPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null && (bool)e.NewValue)
            {
                try
                {
                    StoryboardControlledClosingPopup control = source as StoryboardControlledClosingPopup;
                    control.OnCloseImmediatelyChanged();
                }
                catch { }
            }
        }

        

        public void OnCloseImmediatelyChanged()
        {
            if (_openForSecondsChanged)
            {
                if(!IsOpen)
                {
                    setIsOpen();
                }
                else
                {
                    extendIsOpen();
                }
                _openForSecondsChanged = false;
            }
            else
            {
                SetCurrentValue(ClosingPopup.IsOpenProperty, false);
            }
        }

        public void OnOpenForSecondsChanged()
        {
            _openForSecondsChanged = true;
        }

        void setIsOpen()
        {
            _openPeriodExtensiosTarget = 1;
            _openPeriodsElapsed = 0;
            if(_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
            }
            _timer = new Timer(OpenForSeconds * 1000);
            _timer.Elapsed += (s, e) => {
                Dispatcher.Invoke(new Action(() => {
                    if (IsOpen)
                    {
                        if (++_openPeriodsElapsed == _openPeriodExtensiosTarget)
                        {
                            SetCurrentValue(ClosingPopup.IsOpenProperty, false);
                            killTimer();
                        }
                    }
                    else
                        killTimer();
                }));
            };
            _timer.AutoReset = true;
            SetCurrentValue(ClosingPopup.IsOpenProperty, true);
            _timer.Enabled = true;
        }

        void killTimer()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }
        }

        void extendIsOpen()
        {
            if (_openPeriodExtensiosTarget + 1 <= MaxOpenPeriodExtensions)
                _openPeriodExtensiosTarget++;
        }

        public void Dispose()
        {
            if (_timer != null)
                killTimer();
        }

    }
}
