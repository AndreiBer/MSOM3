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
using System.Windows.Controls.Primitives;

namespace ViewMSOT.UIControls
{
    /// <summary>
    /// Interaction logic for View2DGrid.xaml
    /// </summary>
    public partial class View2DGrid : UserControl
    {
        Xvue.MSOT.ViewModels.Imaging.ViewModelPreview _model;

        public View2DGrid()
        {
            this.InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                imageViewXY.GridPercentageWidthStepSize = "GridStepSizePercentageXY";

                imageViewXY.zpImageBorder.BorderBrush = imageViewXY.imageCanvasBorder.BorderBrush = new SolidColorBrush(Colors.Blue);

                _model = base.DataContext as Xvue.MSOT.ViewModels.Imaging.ViewModelPreview;
            }
            catch
            {
            }
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _model = base.DataContext as Xvue.MSOT.ViewModels.Imaging.ViewModelPreview;
        }

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Delete || e.Key == Key.Back)
                {
                    _model.ImageProperties.RulersViewingPlanes.DeleteAllSelectedRulerToolsCommand.Execute(null);
                    _model.ImageProperties.DrawingRegions2D.DeletedSelectedRegions();
                }
                else if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                {
                    if (e.Key == Key.C)
                    {
                        _model.ImageProperties.DrawingRegions2D.CopySelectedRegion();
                    }
                    else if (e.Key==Key.V)
                    {
                        _model.ImageProperties.DrawingRegions2D.PasteSelectedRegion();
                    }
                }
            }
            catch { }
        }

    }
}
