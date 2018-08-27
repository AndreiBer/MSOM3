using System.Windows;
using System.Windows.Controls;

namespace ViewMSOT.UIControls
{
    /// <summary>
    /// Interaction logic for ViewReportProblem.xaml
    /// </summary>
    public partial class ViewIssueReportBase : UserControl
    {
        public ViewIssueReportBase()
        {
            InitializeComponent();
        }

        public bool IsStepsToReproduceEmpty
        {
            get { return (bool)GetValue(IsStepsToReproduceEmptyProperty); }
            set { SetValue(IsStepsToReproduceEmptyProperty, value); }
        }

        public static readonly DependencyProperty IsStepsToReproduceEmptyProperty =
            DependencyProperty.Register(
            "IsStepsToReproduceEmpty",
            typeof(bool),
            typeof(ViewIssueReportBase),
            new UIPropertyMetadata(true));

        private void OnStepsToReproduceTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
                SetCurrentValue(IsStepsToReproduceEmptyProperty, string.IsNullOrEmpty(textBox.Text));
        }

        private void OnStepsToReproduceTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
                SetCurrentValue(IsStepsToReproduceEmptyProperty, string.IsNullOrEmpty(textBox.Text));
        }

    }
}
