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

namespace ViewMSOT.UIControls
{
	/// <summary>
    /// Interaction logic for ViewMeasurementToolsMenuControl.xaml
	/// </summary>
	public partial class ViewMeasurementToolsMenuControl : UserControl
	{
        public ViewMeasurementToolsMenuControl()
		{
			this.InitializeComponent();
		}

        public bool IsRulerDrawing
        {
            get { return (bool)GetValue(IsRulerDrawingProperty); }
            set { SetValue(IsRulerDrawingProperty, value); }
        }

        public static readonly DependencyProperty IsRulerDrawingProperty =
           DependencyProperty.Register(
              "IsRulerDrawing",
              typeof(bool),
              typeof(ViewMeasurementToolsMenuControl),
              new FrameworkPropertyMetadata());

        public bool IsMultipleRulerDrawing
        {
            get { return (bool)GetValue(IsMultipleRulerDrawingProperty); }
            set { SetValue(IsMultipleRulerDrawingProperty, value); }
        }

        public static readonly DependencyProperty IsMultipleRulerDrawingProperty =
           DependencyProperty.Register(
              "IsMultipleRulerDrawing",
              typeof(bool),
              typeof(ViewMeasurementToolsMenuControl),
              new FrameworkPropertyMetadata());

        public ICommand DeleteSelectedTools
        {
            get { return (ICommand)GetValue(DeleteSelectedToolsProperty); }
            set { SetValue(DeleteSelectedToolsProperty, value); }
        }

        public static readonly DependencyProperty DeleteSelectedToolsProperty =
           DependencyProperty.Register(
              "DeleteSelectedTools",
              typeof(ICommand),
              typeof(ViewMeasurementToolsMenuControl),
              new FrameworkPropertyMetadata());

        public ICommand DeleteAllTools
        {
            get { return (ICommand)GetValue(DeleteAllToolsProperty); }
            set { SetValue(DeleteAllToolsProperty, value); }
        }

        public static readonly DependencyProperty DeleteAllToolsProperty =
           DependencyProperty.Register(
              "DeleteAllTools",
              typeof(ICommand),
              typeof(ViewMeasurementToolsMenuControl),
              new FrameworkPropertyMetadata());

        public bool IsAnySelectedRulerAvailable
        {
            get { return (bool)GetValue(IsAnySelectedRulerAvailableProperty); }
            set { SetValue(IsAnySelectedRulerAvailableProperty, value); }
        }

        public static readonly DependencyProperty IsAnySelectedRulerAvailableProperty =
           DependencyProperty.Register(
              "IsAnySelectedRulerAvailable",
              typeof(bool),
              typeof(ViewMeasurementToolsMenuControl),
              new FrameworkPropertyMetadata());

	}
}