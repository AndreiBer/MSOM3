using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xvue.MSOT.ViewModels.Imaging;

namespace ViewMSOT.UIControls
{
	/// <summary>
    /// Interaction logic for ViewImageLayerControls.xaml
	/// </summary>
	public partial class ViewImageLayerControls : UserControl
	{
        public ViewImageLayerControls()
		{
			this.InitializeComponent();
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
         typeof(ViewImageLayerControls),
         new FrameworkPropertyMetadata());

        public double ImageMax
        {
            get { return (double)GetValue(ImageMaxProperty); }
            set { SetValue(ImageMaxProperty, (double)value); }
        }

        public static readonly DependencyProperty ImageMaxProperty =
        DependencyProperty.Register(
         "ImageMax",
         typeof(double),
         typeof(ViewImageLayerControls),
         new FrameworkPropertyMetadata());

        public double ImageMin
        {
            get { return (double)GetValue(ImageMinProperty); }
            set { SetValue(ImageMinProperty, (double)value); }
        }

        public static readonly DependencyProperty ImageMinProperty =
        DependencyProperty.Register(
         "ImageMin",
         typeof(double),
         typeof(ViewImageLayerControls),
         new FrameworkPropertyMetadata());

        public double ScaleMax
        {
            get { return (double)GetValue(ScaleMaxProperty); }
            set { SetValue(ScaleMaxProperty, (double)value); }
        }

        public static readonly DependencyProperty ScaleMaxProperty =
        DependencyProperty.Register(
         "ScaleMax",
         typeof(double),
         typeof(ViewImageLayerControls),
         new FrameworkPropertyMetadata());

        public double ScaleMin
        {
            get { return (double)GetValue(ScaleMinProperty); }
            set { SetValue(ScaleMinProperty, (double)value); }
        }

        public static readonly DependencyProperty ScaleMinProperty =
        DependencyProperty.Register(
         "ScaleMin",
         typeof(double),
         typeof(ViewImageLayerControls),
         new FrameworkPropertyMetadata());

        public static double StepPercent
        {
            //get { return stepPercentSlider.Value / 100.0; }
            get { return 2.5 / 100.0; }
        }

        private void scaleMaxIncBtn_Click(object sender, RoutedEventArgs e)
        {
            double newVal = ScaleMax + ((ScaleMax - ScaleMin) == 0 ? 1 : (ScaleMax - ScaleMin)) * StepPercent;
            if (newVal > ScaleMin)
                SetCurrentValue(ScaleMaxProperty, newVal);
        }

        private void scaleMaxDecBtn_Click(object sender, RoutedEventArgs e)
        {
            double newVal = ScaleMax - ((ScaleMax - ScaleMin) == 0 ? 1 : (ScaleMax - ScaleMin)) * StepPercent;
            if (newVal > ScaleMin)
                SetCurrentValue(ScaleMaxProperty, newVal);
        }

        private void scaleMinIncBtn_Click(object sender, RoutedEventArgs e)
        {
            double newVal = ScaleMin + ((ScaleMax - ScaleMin) == 0 ? 1 : (ScaleMax - ScaleMin)) * StepPercent;
            if (newVal < ScaleMax)
                SetCurrentValue(ScaleMinProperty, newVal);
        }

        private void scaleMinDecBtn_Click(object sender, RoutedEventArgs e)
        {

            double newVal = ScaleMin - ((ScaleMax - ScaleMin) == 0 ? 1 : (ScaleMax - ScaleMin)) * StepPercent;
            if (newVal < ScaleMax)
                SetCurrentValue(ScaleMinProperty, newVal);
        }

        private void scaleMaxEqBtn_Click(object sender, RoutedEventArgs e)
        {
            SetCurrentValue(ScaleMaxProperty, ImageMax);
        }

        private void scaleMinEqBtn_Click(object sender, RoutedEventArgs e)
        {
            SetCurrentValue(ScaleMinProperty, ImageMin);
        }

        private void scaleMinToZeroBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SetCurrentValue(ScaleMinProperty, (double)0);
            if (ScaleMin > ScaleMax)
                SetCurrentValue(ScaleMaxProperty, (double)0);
        }

        private void scaleMaxToZeroBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SetCurrentValue(ScaleMaxProperty, (double)0);
            if (ScaleMin > ScaleMax)
                SetCurrentValue(ScaleMinProperty, (double)0);
        }

        private void palettesPopup_Opened(object sender, EventArgs e)
        {
            fixMouseCapture(palletesListBox);
        }
        private void alphaControlPopup_Opened(object sender, EventArgs e)
        {
            fixMouseCapture(alphaControl);
        }
        private void filterLayerPopup_Opened(object sender, EventArgs e)
        {
            fixMouseCapture(filterLayerControl);
        }

        static void fixMouseCapture(FrameworkElement element)
        {
            element.CaptureMouse();
            element.ReleaseMouseCapture();
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            selectBgWavelength.SetCurrentValue(MenuItem.VisibilityProperty, System.Windows.Visibility.Collapsed);
            if (e.NewValue != null)
            {
                if (e.NewValue is ViewModelImagingBackground && selectBgWavelength.Items.Count > 0)
                {
                    selectBgWavelength.SetCurrentValue(MenuItem.VisibilityProperty, System.Windows.Visibility.Visible);
                }
            }
        }

        private void SelectedBgWlChecked(object sender, RoutedEventArgs e)
        {
            //MenuItem mi = sender as MenuItem;

            //if ((sender as MenuItem).Parent == null)
            //    return;

            //IEnumerable<Object> source = ((sender as MenuItem).Parent as MenuItem).ItemsSource as IEnumerable<Object>;
        }

        private void MenuItem_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                MenuItem mi = sender as MenuItem;
                mi.SetCurrentValue(MenuItem.IsCheckedProperty, true);
            }
        }

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

	}
}