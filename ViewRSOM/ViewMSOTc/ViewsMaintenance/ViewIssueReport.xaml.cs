using System;
using System.Windows;
using System.Windows.Controls;
using Xvue.MSOT.ViewModels;

namespace ViewMSOTc
{
    /// <summary>
    /// Interaction logic for ViewIssueReport.xaml
    /// </summary>
    public partial class ViewIssueReport : UserControl
    {
        ViewModelIssueReporting _issueRepoter;

        public ViewIssueReport()
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
            typeof(ViewIssueReport));

        public bool? UserReply
        {
            get { return (bool)GetValue(UserReplyProperty); }
            set { SetValue(UserReplyProperty, value); }
        }

        public static readonly DependencyProperty UserReplyProperty =
            DependencyProperty.Register(
            "UserReply",
            typeof(bool?),
            typeof(ViewIssueReport));

        public void Close()
        {
            CloseControl = true;
        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnViewIssueReportBaseDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
                _issueRepoter.DataModelSaved -= OnDataModelSaved;

            _issueRepoter = e.NewValue as ViewModelIssueReporting;
            if (_issueRepoter != null)
                _issueRepoter.DataModelSaved += OnDataModelSaved;
        }

        private void OnDataModelSaved(object sender, EventArgs e)
        {
            Close();
        }
    }
}
