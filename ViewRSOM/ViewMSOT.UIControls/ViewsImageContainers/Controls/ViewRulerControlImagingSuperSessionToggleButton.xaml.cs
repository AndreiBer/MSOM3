using System;
using System.Windows;
using System.Windows.Data;
using Xvue.MSOT.ViewModels.ProjectManager.ImagingSession;

namespace ViewMSOT.UIControls
{
    /// <summary>
    /// Interaction logic for ViewRulerControlImagingSuperSessionToggleButton.xaml
    /// </summary>
    public partial class ViewRulerControlImagingSuperSessionToggleButton : Xvue.Framework.Views.WPF.Controls.ViewRulerControlToggleButtonBase
    {
        public ViewRulerControlImagingSuperSessionToggleButton()
        {
            InitializeComponent();
            SetBinding(ViewRulerControlImagingSessionToggleButton.IsCheckedProperty, new Binding
            {
                Source = toggleButton,
                Path = new PropertyPath("IsChecked"),
                Mode = BindingMode.OneWay
            });
        }

        protected override bool WasLastRulerDrawingCanceled()
        {
            return (DataContext as ViewModelImagingSuperSession).WasLastRulerDrawingCanceled;
        }

        protected override void SetIsRulerDrawing(bool value)
        {
            (DataContext as ViewModelImagingSuperSession).IsRulerDrawing = value;
        }


    }
}
