using System.Windows;
using System.Windows.Controls;
using Xvue.Framework.Views.WPF.Behaviors;
using Xvue.MSOT.ViewModels.ProjectManager;

namespace ViewMSOTc
{
    /// <summary>
    /// Interaction logic for ViewPatientInformationExpander.xaml
    /// </summary>
    public partial class ViewPatientInformationExpander : UserControl
    {

        private MouseClickOutsideControlBehavior _mouseClickBehavior;

        public ViewPatientInformationExpander()
        {
            InitializeComponent();
            _mouseClickBehavior = new MouseClickOutsideControlBehavior(HandleClickOutsideOfControl, this);
        }

        // Called by MouseClickOutsideControlBehavior
        private void HandleClickOutsideOfControl()
        {
            ViewModelPatient vm = DataContext as ViewModelPatient;
            if (!vm.IsEditEnabled)
                infoExpander.SetCurrentValue(Expander.IsExpandedProperty, false);
        }

        // Attach MouseClickOutsideControlBehavior
        private void infoExpander_Expanded(object sender, RoutedEventArgs e)
        {
            _mouseClickBehavior.RegisterElement();
        }

        // Detach MouseClickOutsideControlBehavior
        private void infoExpander_Collapsed(object sender, RoutedEventArgs e)
        {
            _mouseClickBehavior.UnregisterElement();
        }
    }
}
