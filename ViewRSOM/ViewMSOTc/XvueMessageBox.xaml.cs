using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xvue.MSOT.Services.Core;

namespace ViewMSOTc
{
    /// <summary>
    /// Interaction logic for XvueMessageBox.xaml
    /// </summary>
    public partial class XvueMessageBox : UserControl
    {
        public XvueMessageBox()
        {
            InitializeComponent();
            int minHeightOfScreens = Int32.MaxValue;
            foreach (System.Windows.Forms.Screen screen in System.Windows.Forms.Screen.AllScreens)
            {
                int h = screen.Bounds.Size.Height;
                if (h < minHeightOfScreens)
                    minHeightOfScreens = h;
            }
            MaxHeight = 0.9 * minHeightOfScreens;
        }

        public XvueMessageBox(UserNotificationType notificationType, string message)
            : this()
        {
            SetCurrentValue(XvueMessageBox.MessageTypeProperty, notificationType);
            SetCurrentValue(XvueMessageBox.MessageProperty, message);
        }

        public bool MessageType
        {
            get { return (bool)GetValue(MessageTypeProperty); }
            set { SetValue(MessageTypeProperty, value); }
        }

        public static readonly DependencyProperty MessageTypeProperty =
           DependencyProperty.Register(
              "MessageType",
              typeof(UserNotificationType),
              typeof(XvueMessageBox));

        public bool IsVerify
        {
            get { return (bool)GetValue(IsVerifyProperty); }
            set { SetValue(IsVerifyProperty, value); }
        }

        public static readonly DependencyProperty IsVerifyProperty =
            DependencyProperty.Register(
            "IsVerify",
            typeof(bool),
            typeof(XvueMessageBox));

        public bool? UserReply
        {
            get { return (bool?)GetValue(UserReplyProperty); }
            set { SetValue(UserReplyProperty, value); }
        }

        public static readonly DependencyProperty UserReplyProperty =
            DependencyProperty.Register(
            "UserReply",
            typeof(bool?),
            typeof(XvueMessageBox));

        public bool CloseControl
        {
            get { return (bool)GetValue(CloseControlProperty); }
            set { SetValue(CloseControlProperty, value); }
        }

        public static readonly DependencyProperty CloseControlProperty =
            DependencyProperty.Register(
            "CloseControl",
            typeof(bool),
            typeof(XvueMessageBox));

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register(
            "Message",
            typeof(string),
            typeof(XvueMessageBox));

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!IsVerify)
                okBtn.Focus();
        }

        private void yesBtn_Click(object sender, RoutedEventArgs e)
        {
            UserReply = true;
            CloseControl = true;
        }

        private void noBtn_Click(object sender, RoutedEventArgs e)
        {
            UserReply = false;
            CloseControl = true;
        }

        private void okBtn_Click(object sender, RoutedEventArgs e)
        {
            CloseControl = true;
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            UserReply = null;
            CloseControl = true;
        }

        private void userControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                CaptureMouse();
                ReleaseMouseCapture();
                focusMainElement();
            }
        }

        private void focusMainElement()
        {
            if (okBtn.Visibility == System.Windows.Visibility.Visible)
                giveElementFocus(okBtn);
            else if (yesBtn.Visibility == System.Windows.Visibility.Visible)
                giveElementFocus(yesBtn);
        }

        private void giveElementFocus(FrameworkElement element)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Input, new Action(delegate()
            {
                element.Focus();
                Keyboard.Focus(element);
            }));
        }

        public void Setup(string message, string yesChoiceCaption, string noChoiceMessage, bool hasCancel)
        {
            SetCurrentValue(XvueMessageBox.MessageProperty, message);

            if (yesChoiceCaption != null)
                yesBtn.SetCurrentValue(ContentControl.ContentProperty, yesChoiceCaption);
            else
                yesBtn.SetCurrentValue(ContentControl.ContentProperty, "Yes");

            if (noChoiceMessage != null)
                noBtn.SetCurrentValue(ContentControl.ContentProperty, noChoiceMessage);
            else
                noBtn.SetCurrentValue(ContentControl.ContentProperty, "No");
            
            if (hasCancel)
                cancelBtn.SetCurrentValue(UIElement.VisibilityProperty, System.Windows.Visibility.Visible);
            else
                cancelBtn.SetCurrentValue(UIElement.VisibilityProperty, System.Windows.Visibility.Collapsed);
        }

    }
}
