using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xvue.MSOT.DataModels.Plugins.ProjectManager;
using Xvue.MSOT.ViewModels.ProjectManager.ImagingSession;

namespace ViewMSOTc
{
    /// <summary>
    /// Interaction logic for ViewThumbnailLength.xaml
    /// </summary>
    public partial class ViewThumbnailLength : UserControl
    {
        public ViewThumbnailLength()
        {
            InitializeComponent();
            this.Visibility = System.Windows.Visibility.Collapsed;
        }

        public double Seconds
        {
            get { return (double)GetValue(SecondsProperty); }
            set { SetValue(SecondsProperty, (double)value); }
        }

        public static readonly DependencyProperty SecondsProperty =
            DependencyProperty.Register(
             "Seconds",
             typeof(double),
             typeof(ViewThumbnailLength),
             new FrameworkPropertyMetadata(
                new PropertyChangedCallback(SecondsChanged)));

        private static void SecondsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ViewThumbnailLength control = d as ViewThumbnailLength;

            if (e.NewValue == null)
            {
                control.Visibility = Visibility.Collapsed;
            }
            else if ((double)e.NewValue == 0)
            {
                control.Visibility = Visibility.Collapsed;
            }
            else
            {
                control.Visibility = Visibility.Visible;
                int totalSeconds = (int)control.Seconds;
                int sec = totalSeconds % 60;
                int min = totalSeconds / 60;
                control.LabelText = "" + min.ToString() + ":" + sec.ToString("D2");
            }
                
        }

        public string LabelText
        {
            get { return (string)GetValue(LabelTextProperty); }
            set { SetValue(LabelTextProperty, (string)value); }
        }

        public static readonly DependencyProperty LabelTextProperty =
            DependencyProperty.Register(
             "LabelText",
             typeof(string),
             typeof(ViewThumbnailLength),
             new FrameworkPropertyMetadata());
    }
}
