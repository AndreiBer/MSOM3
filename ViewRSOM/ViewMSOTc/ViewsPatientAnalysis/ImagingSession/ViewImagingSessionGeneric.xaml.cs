using System.Windows.Controls;
using System.Windows.Input;

namespace ViewMSOTc
{
    /// <summary>
    /// Interaction logic for ViewImagingSessionGeneric.xaml
    /// </summary>
    public partial class ViewImagingSessionGeneric : UserControl
    {				
        public ViewImagingSessionGeneric()
        {
            InitializeComponent();
        }
		
		public Border SessionBorder
		{
			get {return sessionBorder;}
		}

        private void sessionBorder_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Xvue.MSOT.ViewModels.ProjectManager.ImagingSession.ViewModelImagingSessionBase _node = base.DataContext as Xvue.MSOT.ViewModels.ProjectManager.ImagingSession.ViewModelImagingSessionBase;
            if (!_node.IsEmptyImagingSession && _node.ClipboardPresetAvailable)
            {
                _node.CommandPasteImagingSettings.Execute(null);
                e.Handled = true;
            }
        }

    }
}
