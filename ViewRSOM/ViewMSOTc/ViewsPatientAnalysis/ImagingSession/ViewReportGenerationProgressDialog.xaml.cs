using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xvue.MSOT.ViewModels.ProjectManager.ImagingSession;

namespace ViewMSOTc
{
    /// <summary>
    /// Interaction logic for ViewReportGenerationProgressDialog.xaml
    /// </summary>
    public partial class ViewReportGenerationProgressDialog : UserControl
    {

        public ViewReportGenerationProgressDialog()
        {
            InitializeComponent();
        }

        public bool CloseControl
        {
            get { return (bool)GetValue(CloseControlProperty); }
            set { SetValue(CloseControlProperty, value); }
        }

        public static readonly DependencyProperty CloseControlProperty =
            DependencyProperty.Register(
            "CloseControl",
            typeof(bool),
            typeof(ViewReportGenerationProgressDialog));

        private void userControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                focusMainElement();
            }
        }

        private void focusMainElement()
        {
            giveElementFocus(cancelButton);
        }

        private void giveElementFocus(FrameworkElement element)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Input, new Action(delegate ()
            {
                element.Focus();
                Keyboard.Focus(element);
            }));
        }

        private void userControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ( e.OldValue != null)
            {
                (e.OldValue as ViewModelImagingSuperSession).GenerateReportCompletedEvent -= Dc_GenerateReportCompletedEvent;
            }
            if (e.NewValue != null)
            {
                (e.NewValue as ViewModelImagingSuperSession).GenerateReportCompletedEvent += Dc_GenerateReportCompletedEvent;
            }
        }

        private void Dc_GenerateReportCompletedEvent(object sender, EventArgs e)
        {
            SetCurrentValue(ViewReportGenerationProgressDialog.CloseControlProperty, true);
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            cancelButton.SetCurrentValue(Button.IsEnabledProperty, false);
        }
    }
}
