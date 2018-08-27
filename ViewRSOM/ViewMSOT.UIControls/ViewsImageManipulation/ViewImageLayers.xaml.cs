using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Xvue.Framework.Views.WPF.Behaviors;

namespace ViewMSOT.UIControls
{
	/// <summary>
    /// Interaction logic for ViewImageLayers.xaml
	/// </summary>
	public partial class ViewImageLayers : UserControl
	{

        private MouseClickOutsideControlBehavior _mouseClickBehavior;

        public ViewImageLayers()
		{
			this.InitializeComponent();
            _mouseClickBehavior = new MouseClickOutsideControlBehavior(HandleClickOutsideOfControl, this);
        }

        // Called by MouseClickOutsideControlBehavior
        private void HandleClickOutsideOfControl()
        {
            layerControlsPopup.SetCurrentValue(Grid.VisibilityProperty, Visibility.Collapsed);
        }

        private void layerControlsPopup_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                _mouseClickBehavior.RegisterElement();
            }
            else
            {
                _mouseClickBehavior.UnregisterElement();
            }
        }

        public bool AllowAutoScaling
        {
            get { return (bool)GetValue(AllowAutoScalingProperty); }
            set { SetValue(AllowAutoScalingProperty, (bool)value); }
        }

        public static readonly DependencyProperty AllowAutoScalingProperty =
        DependencyProperty.Register(
         "AllowAutoScaling",
         typeof(bool),
         typeof(ViewImageLayers),
         new FrameworkPropertyMetadata());

        public bool HideTabSelector
        {
            get { return (bool)GetValue(HideTabSelectorProperty); }
            set { SetValue(HideTabSelectorProperty, (bool)value); }
        }

        public static readonly DependencyProperty HideTabSelectorProperty =
        DependencyProperty.Register(
         "HideTabSelector",
         typeof(bool),
         typeof(ViewImageLayers),
         new FrameworkPropertyMetadata());

    }
}