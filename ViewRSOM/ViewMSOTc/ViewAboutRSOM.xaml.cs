using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;


namespace ViewRSOM
{
    /// <summary>
    /// Interaction logic for ViewAboutMSOTc.xaml
    /// </summary>
    public partial class ViewAboutRSOM : UserControl
	{
        Storyboard _remoteAccessToggleButtonStoryboard;
        public ViewAboutRSOM()
		{
			InitializeComponent();
            _remoteAccessToggleButtonStoryboard = (Storyboard)TryFindResource("RemoteAccessBorderStoryboard");
        }

       
       

       
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void userControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
           
        }

        private void focusMainElement()
        {
            
        }

        private void giveElementFocus(FrameworkElement element)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Input, new Action(delegate()
            {
                element.Focus();
                Keyboard.Focus(element);
            }));
        }

        private static void RemoteAccessInitiationInProgressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }

        //private void OnReportButtonClick(object sender, RoutedEventArgs e)
        //{
        //    MSOTSystemBase dc = DataContext as MSOTSystemBase;
        //    if (dc == null)
        //        return;

        //    CloseControl = true;
        //    dc.NotifyUserOnAction("", "Your feedback", UserNotificationType.ReportIssue, false);
        //}
    }
}