using System.Windows;
using System.Windows.Controls;
using Xvue.Framework.Views.WPF;

namespace ViewMSOTc
{
    /// <summary>
    /// Interaction logic for ViewMaintenance.xaml
    /// </summary>
    public partial class ViewMaintenance : Window
	{
        ScrollViewer _logListScrollViewer=null;

        public ViewMaintenance()
		{
			this.InitializeComponent();
		}

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_logListScrollViewer == null)
                {
                    _logListScrollViewer = VisualTreeBrowser.GetDescendantByType(logList, typeof(ScrollViewer)) as ScrollViewer;
                    _logListScrollViewer.ScrollChanged += _logListScrollViewer_ScrollChanged;
                }
            }
            catch { }
        }

        private void _logListScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if(e.ExtentHeightChange >0)
                _logListScrollViewer.ScrollToEnd();
        }
    }
}