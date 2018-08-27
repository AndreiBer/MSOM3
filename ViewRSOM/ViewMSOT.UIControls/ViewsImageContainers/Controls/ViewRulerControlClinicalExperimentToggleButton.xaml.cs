using System.Windows;
using System.Windows.Data;
using Xvue.MSOT.Hardware.ViewModels.Experiment;

namespace ViewMSOT.UIControls
{
    /// <summary>
    /// Interaction logic for ViewRulerControlClinicalExperimentToggleButton.xaml
    /// </summary>
    public partial class ViewRulerControlClinicalExperimentToggleButton : Xvue.Framework.Views.WPF.Controls.ViewRulerControlToggleButtonBase
    {
        public ViewRulerControlClinicalExperimentToggleButton()
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
            return (DataContext as ViewModelExperiment).WasLastRulerDrawingCanceled;
        }

        protected override void SetIsRulerDrawing(bool value)
        {
            (DataContext as ViewModelExperiment).IsRulerDrawing = value;
        }
    }

}
