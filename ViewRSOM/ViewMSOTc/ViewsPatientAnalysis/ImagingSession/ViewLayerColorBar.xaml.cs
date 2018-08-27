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
using Xvue.MSOT.ViewModels.Imaging;

namespace ViewMSOTc
{
	/// <summary>
	/// Interaction logic for ViewLayerColorBar.xaml
	/// </summary>
	public partial class ViewLayerColorBar : UserControl
	{

        public ViewLayerColorBar()
        {
            this.InitializeComponent();
            this.Visibility = System.Windows.Visibility.Collapsed;
        }

		public double InfoFontSize
        {
            get { return (double)GetValue(InfoFontSizeProperty); }
            set { SetValue(InfoFontSizeProperty, value); }
        }

        public static readonly DependencyProperty InfoFontSizeProperty =
           DependencyProperty.Register(
              "InfoFontSize",
              typeof(double),
              typeof(ViewLayerColorBar),
              new FrameworkPropertyMetadata(
                new PropertyChangedCallback(InfoFontSizeChanged)));
        
		private static void InfoFontSizeChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                ViewLayerColorBar view = source as ViewLayerColorBar;
            	view.imageMaxLegendText.SetValue(FontSizeProperty, e.NewValue);
            	view.imageMiddleLegendText.SetValue(FontSizeProperty, e.NewValue);
            	view.imageMinLegendText.SetValue(FontSizeProperty, e.NewValue);
                view.layerNameText.SetValue(FontSizeProperty, e.NewValue);          
            }
            catch
            {
            }
        }
        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                this.Visibility = System.Windows.Visibility.Collapsed;
            else
            {
                ViewModelImagingLayer dc = this.DataContext as ViewModelImagingLayer;
                if (dc != null)
                {
                    if (dc.ImagingComponent.Visible) this.Visibility = System.Windows.Visibility.Visible;
                    else this.Visibility = System.Windows.Visibility.Collapsed;
                }
                else this.Visibility = System.Windows.Visibility.Collapsed;
            }
            refreshRows();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext == null)
                this.Visibility = System.Windows.Visibility.Collapsed;
            else
                refreshRows();
        }	

        private void refreshRows()
        {
            ViewModelImagingLayer dc = this.DataContext as ViewModelImagingLayer;
            if (dc != null)
            {
                double rest = LayoutRoot.ActualHeight - 150;
                if (rest < 0)
                    rest = 0;
                if (dc.ImagingComponent.LogarithmicScaling)
                {
                    double newH;

                    newH = 0.05094287 * rest - 4.5; 
                    if (newH < 0) 
                        newH = 0;
                    gridRow1.Height = new GridLength(newH, GridUnitType.Pixel);

                    gridRow2.Height = new GridLength(0.072653723 * rest + 4.5, GridUnitType.Pixel);

                    gridRow3.Height = new GridLength(0.12360743 * rest + 4.5, GridUnitType.Pixel);

                    newH = 0.752795977 * rest - 4.5; 
                    if (newH < 0) 
                        newH = 0;
                    gridRow4.Height = new GridLength(newH, GridUnitType.Pixel);
                }
                else
                {
                    double mean = rest / 4.0;
                    double corrected = (mean - 4.5) < 0 ? 0 : (mean - 4.5);
                    gridRow1.Height = new GridLength(corrected, GridUnitType.Pixel);
                    gridRow2.Height = new GridLength(mean + 4.5, GridUnitType.Pixel);
                    gridRow3.Height = new GridLength(mean + 4.5, GridUnitType.Pixel);
                    gridRow4.Height = new GridLength(corrected, GridUnitType.Pixel);
                }
            }
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            refreshRows();
        } 		
	}
}