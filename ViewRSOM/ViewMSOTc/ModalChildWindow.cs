using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using Xceed.Wpf.Toolkit;
using Xvue.MSOT.Services.Core;

namespace ViewMSOTc
{
    class ChildWindowBase : ChildWindow
    {
        public bool? UserReply
        {
            get { return (bool?)GetValue(UserReplyProperty); }
            set { SetValue(UserReplyProperty, value); }
        }

        public static readonly DependencyProperty UserReplyProperty =
            DependencyProperty.Register(
            "UserReply",
            typeof(bool?),
            typeof(ChildWindowBase));


        // Blocking wait implementation in the UI without freezing it: http://stackoverflow.com/questions/33002966/wpf-dispatcherframe-magic-how-and-why-this-works
        protected void wait()
        {
            DispatcherFrame f = new DispatcherFrame(); //At application shutdown, all frames are requested to exit.
            ThreadPool.QueueUserWorkItem(State =>
            {
                Xceed.Wpf.Toolkit.WindowState s = Xceed.Wpf.Toolkit.WindowState.Open;
                while (s == Xceed.Wpf.Toolkit.WindowState.Open)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        s = WindowState;
                    }));
                }
                f.Continue = false;
            });
            Dispatcher.PushFrame(f);
        }
    }

    class ModalChildWindow : ChildWindowBase
    {
        ModalChildWindow(string caption="", UserControl content=null, object dataContext=null)
        {
            SetCurrentValue(WindowStateProperty, Xceed.Wpf.Toolkit.WindowState.Closed);
            IsModal = true;
            Caption = caption;
            WindowStartupLocation = Xceed.Wpf.Toolkit.WindowStartupLocation.Center;
            HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            Content = content;
            DataContext = dataContext;
            if (ViewMSOTcSystem.ChildWindowContainer != null)
            {
                ViewMSOTcSystem.ChildWindowContainer.Children.Add(this);
            }     
            Unloaded += ModalChildWindow_Unloaded;
        }

        private void ModalChildWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            ViewMSOTcSystem.ChildWindowContainer.Children.Remove(this);
        }

        //default void constructor is needed for XAML
        public ModalChildWindow() : this("") { }

        bool? ShowDialog()
        {
            SetCurrentValue(WindowStateProperty, Xceed.Wpf.Toolkit.WindowState.Open);
            wait();
            return UserReply;
        }

        static ModalChildWindow CreateModal(string caption, UserControl content, object dataContext)
        {
            ModalChildWindow newChildWindow = new ModalChildWindow(caption, content, dataContext);
            var presetPopupChildWindowControlStyle = Application.Current.FindResource("ChildWindowWiderCaptionStyle");
            newChildWindow.CloseButtonVisibility = System.Windows.Visibility.Collapsed;
            newChildWindow.Style = (Style)presetPopupChildWindowControlStyle;
            return newChildWindow;
        }

        internal static void ShowDialog(string caption, UserControl content, object dataContext)
        {
            ModalChildWindow newChildWindow = CreateModal(caption, content, dataContext);
            newChildWindow.ShowDialog();
        }

        internal static void ShowDialog(string caption, UserControl content, object dataContext, Size dialogSize)
        {
            ModalChildWindow newChildWindow = CreateModal(caption, content, dataContext);
            newChildWindow.Width = dialogSize.Width;
            newChildWindow.Height = dialogSize.Height;
            newChildWindow.ShowDialog();
        }

        internal static bool? ShowDialog(string caption, UserControl content, object dataContext, DependencyProperty controlProperty)
        {
            ModalChildWindow newChildWindow = CreateModal(caption, content, dataContext);
            if (controlProperty != null)
            {
                Binding replyBinding = new Binding("UserReply");
                replyBinding.Source = newChildWindow;
                replyBinding.Mode = BindingMode.TwoWay;
                content.SetBinding(controlProperty, replyBinding);
                return newChildWindow.ShowDialog();
            }
            else
            {
                newChildWindow.ShowDialog();
                return null;
            }
            
        }
    }

    class MessageBoxChildWindow : ChildWindowBase
    {

        bool _autoRemove;

        MessageBoxChildWindow(bool autoRemove)
        {
            WindowStartupLocation = Xceed.Wpf.Toolkit.WindowStartupLocation.Center;
            HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            SetCurrentValue(WindowStateProperty, Xceed.Wpf.Toolkit.WindowState.Closed);
            IsModal = true;
            CloseButtonVisibility = System.Windows.Visibility.Collapsed;
            var presetPopupChildWindowControlStyle = Application.Current.FindResource("ChildWindowWiderCaptionStyle");
            Style = (Style)presetPopupChildWindowControlStyle;
            _autoRemove = autoRemove;
        }

        public MessageBoxChildWindow(string message, string caption, UserNotificationType notificationType, bool doNotBlock): this(true)
        {
            XvueMessageBox contentMessageBox = new XvueMessageBox(notificationType, message);
            Caption = caption;
            IsModal = !doNotBlock;
            Content = contentMessageBox;
            ViewMSOTcSystem.ChildWindowContainer.Children.Add(this);

            Unloaded += MessageBoxChildWindow_Unloaded;
        }

        private void MessageBoxChildWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_autoRemove)
                ViewMSOTcSystem.ChildWindowContainer.Children.Remove(this);
        }

        //default void constructor is needed for XAML
        public MessageBoxChildWindow():this(false) { }

        public void ShowMessage()
        {
            SetCurrentValue(WindowStateProperty, Xceed.Wpf.Toolkit.WindowState.Open);
            if (IsModal)
            {
                wait();
            }
        }

        public bool? ShowMessage(string caption)
        {
            Caption = caption;
            SetCurrentValue(WindowStateProperty, Xceed.Wpf.Toolkit.WindowState.Open);
            wait();
            return UserReply;
        }
    }

}
