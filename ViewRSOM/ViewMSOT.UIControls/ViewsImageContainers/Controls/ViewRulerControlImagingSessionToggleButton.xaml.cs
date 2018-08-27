using System;
using System.Windows;
using System.Windows.Data;
using Xvue.MSOT.ViewModels.Imaging;

namespace ViewMSOT.UIControls
{
    /// <summary>
    /// Interaction logic for ViewRulerControlToggleButton.xaml
    /// </summary>
    public partial class ViewRulerControlImagingSessionToggleButton : Xvue.Framework.Views.WPF.Controls.ViewRulerControlToggleButtonBase
    {
        public ViewRulerControlImagingSessionToggleButton()
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
            return (DataContext as ViewModelRulers2D).WasLastRulerDrawingCanceled;
        }

        protected override void SetIsRulerDrawing(bool value)
        {
            (DataContext as ViewModelRulers2D).IsRulerDrawing = value;
        }
    }

}
