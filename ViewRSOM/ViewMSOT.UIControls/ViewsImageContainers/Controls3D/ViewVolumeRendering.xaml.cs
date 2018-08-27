using System;
using System.Collections.Generic;
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
using System.Collections.ObjectModel;
using Xvue.MSOT.DataModels.Plugins.ProjectManager.MsotProject;
using System.Windows.Media.Media3D;
using Xvue.MSOT.ViewModels.Imaging;
using Xvue.MSOT.ViewModels.ProjectManager.ImagingSession;

namespace ViewMSOT.UIControls
{
	/// <summary>
    /// Interaction logic for ViewVolumeRendering.xaml
	/// </summary>
    public partial class ViewVolumeRendering : UserControl
	{
        public ViewVolumeRendering()
		{
			this.InitializeComponent();
        }
        
        public void ResetCamera()
        {
            volume3DControl.ResetTrackball();
        }

        #region localvariables
        #endregion localvariables

        #region properties

        public ImageSource RenderedVolumeImage
        {
            get { return (ImageSource)GetValue(RenderedVolumeImageProperty); }
            set { SetValue(RenderedVolumeImageProperty, value); }
        }

        public static readonly DependencyProperty RenderedVolumeImageProperty =
            DependencyProperty.Register(
            "RenderedVolumeImage",
            typeof(ImageSource),
            typeof(ViewVolumeRendering));

        public bool IsCamera3DControlEnabled
        {
            get { return (bool)GetValue(IsCamera3DControlEnabledProperty); }
            set { SetValue(IsCamera3DControlEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsCamera3DControlEnabledProperty =
            DependencyProperty.Register(
            "IsCamera3DControlEnabled",
            typeof(bool),
            typeof(ViewVolumeRendering),
            new FrameworkPropertyMetadata((bool)true));

        #endregion properties

    }
}