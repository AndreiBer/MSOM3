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
using Xvue.MSOT.Services.Imaging;

namespace ViewMSOT.UIControls
{
	/// <summary>
    /// Interaction logic for ViewRulerToolsControl.xaml
	/// </summary>
    public partial class ViewRulerToolsControl : UserControl
	{
        public ViewRulerToolsControl()
		{
			this.InitializeComponent();
		}

        public GridLineType GridStyle
        {
            get { return (GridLineType)GetValue(GridStyleProperty); }
            set { SetValue(GridStyleProperty, value); }
        }

        public static readonly DependencyProperty GridStyleProperty =
            DependencyProperty.Register(
            "GridStyle",
            typeof(GridLineType),
            typeof(ViewRulerToolsControl),
            new FrameworkPropertyMetadata());

        public Color GridColor
        {
            get { return (Color)GetValue(GridColorProperty); }
            set { SetValue(GridColorProperty, value); }
        }

        public static readonly DependencyProperty GridColorProperty =
            DependencyProperty.Register(
            "GridColor",
            typeof(Color),
            typeof(ViewRulerToolsControl),
            new FrameworkPropertyMetadata());

        public double GridSize
        {
            get { return (double)GetValue(GridSizeProperty); }
            set { SetValue(GridSizeProperty, value); }
        }

        public static readonly DependencyProperty GridSizeProperty =
            DependencyProperty.Register(
            "GridSize",
            typeof(double),
            typeof(ViewRulerToolsControl),
            new FrameworkPropertyMetadata());

        public bool IsImageGridLineOptionsAvailable
        {
            get { return (bool)GetValue(IsImageGridLineOptionsAvailableProperty); }
            set { SetValue(IsImageGridLineOptionsAvailableProperty, value); }
        }

        public static readonly DependencyProperty IsImageGridLineOptionsAvailableProperty =
            DependencyProperty.Register(
            "IsImageGridLineOptionsAvailable",
            typeof(bool),
            typeof(ViewRulerToolsControl),
            new FrameworkPropertyMetadata());

        private void textBox_GotFocus(object sender, RoutedEventArgs e)
        {
            selectAllTextBox(sender as TextBox);
        }

        private void selectAllTextBox(TextBox textBoxElement)
        {
            if (textBoxElement == null)
                return;

            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Input, new Action(delegate()
            {
                textBoxElement.SelectAll();
            }));
        }

        private void TextBox_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            e.Handled = true;
        }
    }

}