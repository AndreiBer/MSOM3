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

namespace ViewMSOT.UIControls
{
	/// <summary>
	/// Interaction logic for ViewForegroundLayerColorBar.xaml
	/// </summary>
	public partial class ViewLayerColorBar : UserControl
	{
		public ViewLayerColorBar()
		{
			this.InitializeComponent();
		}

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                this.Visibility = System.Windows.Visibility.Collapsed;
            else
            {
                if (DataContext is ViewModelImagingLayer || DataContext.GetType().IsSubclassOf(typeof(ViewModelImagingLayer)))
                {
					Binding myBinding1;
                    myBinding1 = new Binding("Parent.ExportInfo.ExportImageTextSize");
                    if (DataContext is ViewModelImagingBackground) // manual binding of ExportShowBackgroundColorBar (because we start with Visibility = Collapsed)
                    {
                        ViewModelImagingBackground dc = DataContext as ViewModelImagingBackground;
                        Binding myBinding0;
                        myBinding0 = new Binding("Parent.ExportInfo.ExportShowBackgroundColorBar");
                        myBinding0.Source = dc;
                        myBinding0.Converter = new Xvue.Framework.Views.WPF.Converters.BooleanToVisibleConverter();
                        myBinding0.ConverterParameter = "Collapsed";
                        this.SetBinding(ViewLayerColorBar.VisibilityProperty, myBinding0);

                        myBinding1.Source = dc;
                        imageMaxLegendText.SetBinding(FontSizeProperty, myBinding1);
                        imageMiddleLegendText.SetBinding(FontSizeProperty, myBinding1);
                        imageMinLegendText.SetBinding(FontSizeProperty, myBinding1);
                        layerNameText.SetBinding(FontSizeProperty, myBinding1);
                    }
                    else    //Foreground layers always visible
                    {
                        ViewModelImagingForeground dc = DataContext as ViewModelImagingForeground;
                        if (dc.ImagingComponent.Visible)
                            this.Visibility = System.Windows.Visibility.Visible;
                        else
                            this.Visibility = System.Windows.Visibility.Collapsed;

                        myBinding1.Source = dc as ViewModelImagingForeground;
                        imageMaxLegendText.SetBinding(FontSizeProperty, myBinding1);   
                        imageMiddleLegendText.SetBinding(FontSizeProperty, myBinding1);
                        imageMinLegendText.SetBinding(FontSizeProperty, myBinding1);
                        layerNameText.SetBinding(FontSizeProperty, myBinding1);
                    }
                }
                else
                    this.Visibility = System.Windows.Visibility.Visible;
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
                double rest = LayoutRoot.ActualHeight - 100;
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
                    layerNameText.SetCurrentValue(Grid.RowProperty, 7);
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