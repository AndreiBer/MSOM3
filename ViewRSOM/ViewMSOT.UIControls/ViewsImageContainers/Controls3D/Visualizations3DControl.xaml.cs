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
	/// Interaction logic for Visualizations3DControl.xaml
	/// </summary>
	public partial class Visualizations3DControl : UserControl
	{
		public Visualizations3DControl()
		{
			this.InitializeComponent();
		}

        public bool IsCamera3DControlEnabled
        {
            get { return (bool)GetValue(IsCamera3DControlEnabledProperty); }
            set { SetValue(IsCamera3DControlEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsCamera3DControlEnabledProperty =
            DependencyProperty.Register(
            "IsCamera3DControlEnabled",
            typeof(bool),
            typeof(Visualizations3DControl), 
            new FrameworkPropertyMetadata((bool)true));

        void refresh3dViewsGridRowsHeighVisibility()
        {
            try
            {
                bool show3dTexture = show3dTextureCheckBox.IsChecked.Value && show3dTextureCheckBox.IsEnabled;
                bool show3dVolRend = show3dVolRendCheckBox.IsChecked.Value;
                if (show3dTexture && show3dVolRend)
                {
                    volume3DSplitter.Visibility = Visibility.Visible;
                    text3DControl.Visibility = Visibility.Visible;
                    volume3DControl.Visibility = Visibility.Visible;
                    extra3DGrid.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Star);
                    extra3DGrid.RowDefinitions[3].Height = new GridLength(1, GridUnitType.Star);
                }
                else if (!show3dTexture && show3dVolRend)
                {
                    volume3DSplitter.Visibility = Visibility.Collapsed;
                    text3DControl.Visibility = Visibility.Collapsed;
                    volume3DControl.Visibility = Visibility.Visible;
                    extra3DGrid.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Star);
                    extra3DGrid.RowDefinitions[3].Height = new GridLength(0, GridUnitType.Star);
                }
                else if (show3dTexture && !show3dVolRend)
                {
                    volume3DSplitter.Visibility = Visibility.Collapsed;
                    text3DControl.Visibility = Visibility.Visible;
                    volume3DControl.Visibility = Visibility.Collapsed;
                    extra3DGrid.RowDefinitions[1].Height = new GridLength(0, GridUnitType.Star);
                    extra3DGrid.RowDefinitions[3].Height = new GridLength(1, GridUnitType.Star);
                }
                else if (!show3dTexture && !show3dVolRend)
                {
                    volume3DSplitter.Visibility = Visibility.Collapsed;
                    text3DControl.Visibility = Visibility.Collapsed;
                    volume3DControl.Visibility = Visibility.Collapsed;
                    extra3DGrid.RowDefinitions[1].Height = new GridLength(0, GridUnitType.Star);
                    extra3DGrid.RowDefinitions[3].Height = new GridLength(0, GridUnitType.Star);
                }
            }
            catch { }
        }

        private void show3dTextureCheckBox_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            refresh3dViewsGridRowsHeighVisibility();
        }

        private void show3dViewCheckBox_CheckedUnchecked(object sender, RoutedEventArgs e)
        {
            refresh3dViewsGridRowsHeighVisibility();
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            refresh3dViewsGridRowsHeighVisibility();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (text3DControl.Visibility == System.Windows.Visibility.Visible)
                text3DControl.ResetCamera();

            if (volume3DControl.Visibility == System.Windows.Visibility.Visible)
                volume3DControl.ResetCamera();
        }

	}
}