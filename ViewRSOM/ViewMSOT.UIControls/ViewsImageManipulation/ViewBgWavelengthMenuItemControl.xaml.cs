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
    /// Interaction logic for ViewBgWavelengthMenuItemControl.xaml
	/// </summary>
    public partial class ViewBgWavelengthMenuItemControl : UserControl
	{
        public ViewBgWavelengthMenuItemControl()
		{
			this.InitializeComponent();
		}

        public bool IsParentChecked
        {
            get { return (bool)GetValue(IsParentCheckedProperty); }
            set { SetValue(IsParentCheckedProperty, (bool)value); }
        }

        public static readonly DependencyProperty IsParentCheckedProperty =
        DependencyProperty.Register(
         "IsParentChecked",
         typeof(bool),
         typeof(ViewBgWavelengthMenuItemControl),
         new FrameworkPropertyMetadata(new PropertyChangedCallback(ChangeIsParentChecked)));

        private static void ChangeIsParentChecked(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ViewBgWavelengthMenuItemControl control = d as ViewBgWavelengthMenuItemControl;
            control.SetCurrentValue(ViewBgWavelengthMenuItemControl.IsMenuItemCheckedProperty, (bool)e.NewValue);
        }

        public bool IsMenuItemChecked
        {
            get { return (bool)GetValue(IsMenuItemCheckedProperty); }
            set { SetValue(IsMenuItemCheckedProperty, (bool)value); }
        }

        public static readonly DependencyProperty IsMenuItemCheckedProperty =
        DependencyProperty.Register(
         "IsMenuItemChecked",
         typeof(bool),
         typeof(ViewBgWavelengthMenuItemControl),
         new FrameworkPropertyMetadata(new PropertyChangedCallback(ChangeIsMenuItemChecked)));

        private static void ChangeIsMenuItemChecked(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ViewBgWavelengthMenuItemControl control = d as ViewBgWavelengthMenuItemControl;
            control.SetCurrentValue(ViewBgWavelengthMenuItemControl.IsParentCheckedProperty, (bool)e.NewValue);
        }

	}
}