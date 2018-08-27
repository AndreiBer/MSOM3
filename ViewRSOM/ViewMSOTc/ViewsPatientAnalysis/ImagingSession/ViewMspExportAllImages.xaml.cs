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
using Xvue.MSOT.ViewModels.ProjectManager.ImagingSession;

namespace ViewMSOTc
{
    /// <summary>
    /// Interaction logic for ViewMspExportAllImages.xaml
    /// </summary>
    public partial class ViewMspExportAllImages : UserControl
    {
        bool _alreadyLoaded;
        public ViewMspExportAllImages()
        {
            InitializeComponent();
            _alreadyLoaded = false;
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModelImagingSessionBase dc = this.DataContext as ViewModelImagingSessionBase;
            ModalChildWindow.ShowDialog("File Name", new ViewMspExportName(), dc);
            CloseControl = true;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            CloseControl = true;
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
            typeof(ViewMspExportAllImages));

        private void userControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                if (!_alreadyLoaded)
                {
                    ViewModelImagingSessionBase dc = this.DataContext as ViewModelImagingSessionBase;
                    dc.ExportNewFileName = "";
                    DataTemplate usedTemplate;
                    if(dc.Is3DImagingSession)
                        usedTemplate = viewbox3DContentControl.ContentTemplate;
                    else
                        usedTemplate = viewboxContentControl.ContentTemplate;
                    
                    dc.ExportImageTemplate = usedTemplate;                    
                    dc.IsSelectedImageOnly = false;
                    dc.CheckImageStackExportFormat();
                    e.Handled = true;
                    _alreadyLoaded = true;
                }
            }
            catch { }
        }

    }
}
