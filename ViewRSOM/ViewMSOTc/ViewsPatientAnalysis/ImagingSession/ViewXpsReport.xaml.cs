using System.Windows;
using System.Windows.Controls;

namespace ViewMSOTc
{
    /// <summary>
    /// Interaction logic for ViewXpsReport.xaml
    /// </summary>
    public partial class ViewXpsReport : UserControl
    {
        public ViewXpsReport()
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
            typeof(ViewXpsReport));

        private void OnDiscardButtonClick(object sender, RoutedEventArgs e)
        {
            CloseControl = true;
        }

        private void OnSaveButtonClick(object sender, RoutedEventArgs e)
        {
            CloseControl = true;
        }

        private void OnDocumentViewerContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            e.Handled = true;
        }
    }
}
