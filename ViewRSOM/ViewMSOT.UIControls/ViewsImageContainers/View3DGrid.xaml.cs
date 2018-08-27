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
using ViewMSOT.UIControls.Adorners;
using System.Windows.Controls.Primitives;
using Xvue.MSOT.Services.Imaging;

namespace ViewMSOT.UIControls
{
    /// <summary>
    /// Interaction logic for View3DGrid.xaml
    /// </summary>
    public partial class View3DGrid : UserControl
    {
        ViewModel3DPane model3DPane;

        RoiAdorner _currentRegion;
        CubicRoiAdorner _cropAdorner;
        AdornerLayer imageAdornerLayer;
        bool _cropVisible;
        bool _cropEnable;

        public View3DGrid()
        {
            this.InitializeComponent();
        }

        public CheckBox Show3DViewsCheckBox
        {
            get { return show3DViewsCheckBox; }
        }

        public Visualizations3DControl Show3DVRControl
        {
            get { return _3DRenderingControl; }
        }

        //public Button ExportCurrentImageBtn
        //{
        //    get { return exportCurrentImageBtn; }
        //}

		public bool EnableControls
        {
            get { return (bool)GetValue(EnableControlsProperty); }
            set
            {
                SetValue(EnableControlsProperty, (bool)value);
            }
        }
        public static readonly DependencyProperty EnableControlsProperty =
            DependencyProperty.Register(
             "EnableControls",
             typeof(bool),
             typeof(View3DGrid),
            new FrameworkPropertyMetadata(
                    new PropertyChangedCallback(EnableControlsChanged)));

        private static void EnableControlsChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                View3DGrid control = (source as View3DGrid);
                control.UpdateCropManipulation(control.EnableControls);
            }
            catch
            {
               
            }
        }

        public Visible3DGridPlanesType Visible3DGridPlanes
        {
            get { return (Visible3DGridPlanesType)GetValue(Visible3DGridPlanesProperty); }
            set { SetValue(Visible3DGridPlanesProperty, (Visible3DGridPlanesType)value); }
        }
        public static readonly DependencyProperty Visible3DGridPlanesProperty =
            DependencyProperty.Register(
             "Visible3DGridPlanes",
             typeof(Visible3DGridPlanesType),
             typeof(View3DGrid),
            new FrameworkPropertyMetadata(
                    new PropertyChangedCallback(Visible3DGridPlanesChanged)));

        private static void Visible3DGridPlanesChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                View3DGrid control = (source as View3DGrid);
                control.refreshVisiblePlanes();
            }
            catch
            {

            }
        }

        private void refreshVisiblePlanes()
        {
            if (Visible3DGridPlanes == Visible3DGridPlanesType.All)
            {
                vertivalGridSplitter.Visibility = System.Windows.Visibility.Visible;
                horizontalGridSplitter.Visibility = System.Windows.Visibility.Visible;
                xyGrid.Visibility = System.Windows.Visibility.Visible;
                xzGrid.Visibility = System.Windows.Visibility.Visible;
                yzGrid.Visibility = System.Windows.Visibility.Visible;
                volume3DGrid.Visibility = System.Windows.Visibility.Visible;

                gridColumnLeft.Width = new GridLength(0.5, GridUnitType.Star);
                gridColumnRight.Width = new GridLength(0.5, GridUnitType.Star);
                gridRowTop.Height = new GridLength(0.5, GridUnitType.Star);
                gridRowBottom.Height = new GridLength(0.5, GridUnitType.Star);
            }
            else
            {
                vertivalGridSplitter.Visibility = System.Windows.Visibility.Collapsed;
                horizontalGridSplitter.Visibility = System.Windows.Visibility.Collapsed;
                xyGrid.Visibility = System.Windows.Visibility.Collapsed;
                xzGrid.Visibility = System.Windows.Visibility.Collapsed;
                yzGrid.Visibility = System.Windows.Visibility.Collapsed;
                volume3DGrid.Visibility = System.Windows.Visibility.Collapsed;

                if (Visible3DGridPlanes == Visible3DGridPlanesType.View3D)
                {
                    volume3DGrid.Visibility = System.Windows.Visibility.Visible;
                    gridColumnLeft.Width = new GridLength(0);
                    gridColumnRight.Width = new GridLength(1, GridUnitType.Star);
                    gridRowTop.Height = new GridLength(0);
                    gridRowBottom.Height = new GridLength(1, GridUnitType.Star);
                }
                else if (Visible3DGridPlanes == Visible3DGridPlanesType.XY)
                {
                    xyGrid.Visibility = System.Windows.Visibility.Visible;
                    gridColumnLeft.Width = new GridLength(1, GridUnitType.Star);
                    gridColumnRight.Width = new GridLength(0);
                    gridRowTop.Height = new GridLength(1, GridUnitType.Star);
                    gridRowBottom.Height = new GridLength(0);
                }
                else if (Visible3DGridPlanes == Visible3DGridPlanesType.XZ)
                {
                    xzGrid.Visibility = System.Windows.Visibility.Visible;
                    gridColumnLeft.Width = new GridLength(1, GridUnitType.Star);
                    gridColumnRight.Width = new GridLength(0);
                    gridRowTop.Height = new GridLength(0);
                    gridRowBottom.Height = new GridLength(1, GridUnitType.Star);
                }
                else if (Visible3DGridPlanes == Visible3DGridPlanesType.YZ)
                {
                    yzGrid.Visibility = System.Windows.Visibility.Visible;
                    gridColumnLeft.Width = new GridLength(0);
                    gridColumnRight.Width = new GridLength(1, GridUnitType.Star);
                    gridRowTop.Height = new GridLength(1, GridUnitType.Star);
                    gridRowBottom.Height = new GridLength(0);
                }
            }
        }

        public double Zoom
        {
            get { return (double)GetValue(ZoomProperty); }
            set { SetValue(ZoomProperty, value); }
        }

        public static readonly DependencyProperty ZoomProperty =
           DependencyProperty.Register(
              "Zoom",
              typeof(double),
              typeof(View3DGrid),
              new FrameworkPropertyMetadata());

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                imageViewXY.ViewingPlaneIndex = Xvue.MSOT.Services.Imaging.ImagingConstants.BufferIndexXY;
                imageViewXZ.ViewingPlaneIndex = Xvue.MSOT.Services.Imaging.ImagingConstants.BufferIndexXZ;
                imageViewYZ.ViewingPlaneIndex = Xvue.MSOT.Services.Imaging.ImagingConstants.BufferIndexYZ;

                imageViewXY.GridPercentageWidthStepSize = "GridStepSizePercentageXY";
                imageViewXZ.GridPercentageWidthStepSize = "GridStepSizePercentageXY";
                imageViewYZ.GridPercentageWidthStepSize = "GridStepSizePercentageZ";

                //Set different colors to Image borders that do not change.
                _3DRenderingControl.text3DControl.BorderXYColor = Colors.Blue;
                _3DRenderingControl.text3DControl.BorderYZColor = Colors.Green;
                _3DRenderingControl.text3DControl.BorderXZColor = Colors.Yellow;

                imageViewXY.zpImageBorder.BorderBrush = imageViewXY.imageCanvasBorder.BorderBrush = new SolidColorBrush(_3DRenderingControl.text3DControl.BorderXYColor);
                imageViewYZ.zpImageBorder.BorderBrush = imageViewYZ.imageCanvasBorder.BorderBrush = new SolidColorBrush(_3DRenderingControl.text3DControl.BorderYZColor);
                imageViewXZ.zpImageBorder.BorderBrush = imageViewXZ.imageCanvasBorder.BorderBrush = new SolidColorBrush(_3DRenderingControl.text3DControl.BorderXZColor);

                refreshCrossHairPARTColors();

                imageAdornerLayer = AdornerLayer.GetAdornerLayer(imageViewXY.backgroundImage);
                //AdornerLayer imageYZAdornerLayer = AdornerLayer.GetAdornerLayer(imageViewYZ.backgroundImage);
                //AdornerLayer imageXZAdornerLayer = AdornerLayer.GetAdornerLayer(imageViewXZ.backgroundImage);


                //imageAdornerLayer.Add(_roiAdorner);
                model3DPane = (base.DataContext as ViewModel3DPane);

                initUpdateCropLimitDetailsAdorner();
            }
            catch
            {
            }
        }

        private void refreshCrossHairPARTColors()
        {
            try
            {
                imageViewXY.crossHairControl.PART_W.Stroke = new SolidColorBrush(Colors.Yellow);
                imageViewXY.crossHairControl.PART_H.Stroke = new SolidColorBrush(Colors.Green);

                imageViewYZ.crossHairControl.PART_W.Stroke = new SolidColorBrush(Colors.Yellow);
                imageViewYZ.crossHairControl.PART_H.Stroke = new SolidColorBrush(Colors.Blue);

                imageViewXZ.crossHairControl.PART_W.Stroke = new SolidColorBrush(Colors.Blue);
                imageViewXZ.crossHairControl.PART_H.Stroke = new SolidColorBrush(Colors.Green);
            }
            catch { }
        }

        private void UserControl_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            try
            {
                ViewModel3DPane dc;
                dc = (base.DataContext as ViewModel3DPane);
                if (dc != null)
                {
                    //dc.ImageProperties.CurrentZoom = this.xyGrid.ActualWidth / dc.ImageProperties.WidthInPixels;
                    model3DPane = dc;
                    rebindCubicCropAdorners();

                    setImageViewCrossHairBindings(imageViewXY, "ImageProperties.SelectedPlaneY", "ImageProperties.SelectedPlaneX");
                    setImageViewCrossHairBindings(imageViewYZ, "ImageProperties.SelectedPlaneZ", "ImageProperties.SelectedPlaneX");
                    setImageViewCrossHairBindings(imageViewXZ, "ImageProperties.SelectedPlaneY", "ImageProperties.SelectedPlaneZ");
                }
            }
            catch { }
        }

        void setImageViewCrossHairBindings(ViewImage viewImage, string bindW, string bindH)
        {
            try
            {
                viewImage.crossHairControl.SetBinding(CrosshairControl.HorizontalPositionProperty,
                    new Binding(bindW) { Source = model3DPane, Mode = BindingMode.TwoWay });

                viewImage.crossHairControl.SetBinding(CrosshairControl.VerticalPositionProperty,
                    new Binding(bindH) { Source = model3DPane, Mode = BindingMode.TwoWay });
            }
            catch { }
        }

        public RegionShape CurrentShape
        {
            get { return (RegionShape)GetValue(CurrentShapeProperty); }
            set { SetValue(CurrentShapeProperty, value); }
        }

        public static readonly DependencyProperty CurrentShapeProperty =
           DependencyProperty.Register(
              "CurrentShape",
              typeof(RegionShape),
              typeof(View3DGrid),
              new FrameworkPropertyMetadata(
                new PropertyChangedCallback(ChangeCurrentShape)));

        private static void ChangeCurrentShape(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                (source as View3DGrid).UpdateCurrentShape((RegionShape)e.NewValue);
            }
            catch
            {
            }
        }

        void UpdateCurrentShape(RegionShape newValue)
        {
            try
            {
                if (newValue != null)
                {
                    if (_currentRegion != null)
                    {
                        imageAdornerLayer.Remove(_currentRegion);
                    }
                    Style thumbStyle = this.Resources["PolygonControlThumbStyle"] as Style;
                    Style anchorThumbStyle = this.Resources["AnchorControlThumbStyle"] as Style;
                    RegionShape region = newValue;

                    RoiAdorner newShape = new RoiAdorner(imageViewXY.backgroundImage, region.Pen, thumbStyle, anchorThumbStyle, true, model3DPane.ImageProperties.SelectedROIsStyle);
                    Binding myBinding = new Binding("ImageProperties.CurrentZoom");
                    myBinding.Source = model3DPane;
                    newShape.SetBinding(RoiAdorner.ZoomProperty, myBinding);
                    newShape.ChangePathPoints(region.PathPoints);
                    newShape.CurrentShape = region.Shape;
                    newShape.DrawStaticElements();
                    _currentRegion = newShape;
                    imageAdornerLayer.Add(newShape);
                }
            }
            catch { }
        }

        void showAndBindCropAdorners(string polygonThumb, string anchorThumb)
        {
            try
            {
                if (model3DPane != null)
                {
                    Style thumbStyle = this.Resources[polygonThumb] as Style;
                    Style anchorThumbStyle = this.Resources[anchorThumb] as Style;
                    //_cropAdorner = new CubicRoiAdorner(new UIElement[] { xyGrid, yzGrid, xzGrid }, Colors.Gold, thumbStyle, anchorThumbStyle);
                    _cropAdorner = new CubicRoiAdorner(new UIElement[] { imageViewXY.imageGrid, imageViewYZ.imageGrid, imageViewXZ.imageGrid }, Colors.Gold, thumbStyle, anchorThumbStyle);
                }
                rebindCubicCropAdorners();
            }
            catch { }
        }

        void rebindCubicCropAdorners()
        {
            try
            {
                if ((model3DPane != null) && (_cropAdorner != null))
                {
                    Binding myBinding1 = new Binding("ImageProperties.CroppingPoint0");
                    myBinding1.Source = model3DPane;
                    myBinding1.Mode = BindingMode.TwoWay;
                    _cropAdorner.SetBinding(CubicRoiAdorner.P0Property, myBinding1);

                    Binding myBinding2 = new Binding("ImageProperties.CroppingPoint1");
                    myBinding2.Source = model3DPane;
                    myBinding2.Mode = BindingMode.TwoWay;
                    _cropAdorner.SetBinding(CubicRoiAdorner.P1Property, myBinding2);
                }
            }
            catch { }
        }

        private void showMIPCropChecked()
        {
            _cropEnable = true;
            if (_cropVisible && _cropEnable)
            {
                showAndBindCropAdorners("PolygonControlThumbStyle", "AnchorControlThumbStyle");
                cropLimitsDetails.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void showMIPCropUnChecked()
        {
            _cropEnable = false;
            if (_cropAdorner != null)
            {
                _cropAdorner.Remove();
                _cropAdorner = null;
                cropLimitsDetails.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        public bool CropVisibility
        {
            get { return (bool)GetValue(CropVisibilityProperty); }
            set { SetValue(CropVisibilityProperty, value); }
        }

        public static readonly DependencyProperty CropVisibilityProperty =
           DependencyProperty.Register(
              "CropVisibility",
              typeof(bool),
              typeof(View3DGrid),
              new FrameworkPropertyMetadata(
                new PropertyChangedCallback(ChangeCropVisibility)));

        private static void ChangeCropVisibility(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                (source as View3DGrid).UpdateCropVisibility((bool)e.NewValue);
            }
            catch
            {
            }
        }

        public bool ShowMIPCrop
        {
            get { return (bool)GetValue(ShowMIPCropProperty); }
            set { SetValue(ShowMIPCropProperty, value); }
        }

        public static readonly DependencyProperty ShowMIPCropProperty =
           DependencyProperty.Register(
              "ShowMIPCrop",
              typeof(bool),
              typeof(View3DGrid),
              new FrameworkPropertyMetadata(
                new PropertyChangedCallback(ChangeShowMIPCrop)));

        private static void ChangeShowMIPCrop(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                View3DGrid control = source as View3DGrid;
                if ((bool)e.NewValue)
                {
                    control.showMIPCropChecked();
                }
                else
                {
                    control.showMIPCropUnChecked();
                }
            }
            catch
            {
            }
        }

        void UpdateCropVisibility(bool newValue)
        {
            _cropVisible = newValue;
            if (_cropVisible && _cropEnable)
            {
                showAndBindCropAdorners("PolygonControlThumbStyle", "AnchorControlThumbStyle");
                cropLimitsDetails.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                if (_cropAdorner != null)
                {
                    _cropAdorner.Remove();
                    _cropAdorner = null;
                    cropLimitsDetails.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }              

        private void initUpdateCropLimitDetailsAdorner()
        {
            if (_cropAdorner != null)
            {
                _cropAdorner.Remove();
                _cropAdorner = null;
            }

            bool collapse = true;

            if (/*cropToggleButton.IsChecked.Value &&*/ CropVisibility && model3DPane != null)
            {
                if (model3DPane.Image3DProperties.ShowMIPCrop)
                {
                    showAndBindCropAdorners("PolygonControlThumbStyle", "AnchorControlThumbStyle");
                    collapse = false;
                }
            }
            else
                collapse = true;

            if (collapse)
            {
                cropLimitsDetails.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                cropLimitsDetails.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void ComponentSelectionPopup_Closed(object sender, EventArgs e)
        {
            try
            {
                model3DPane.ImageProperties.ParentVisualization.GetComponentsUpdates();
            }
            catch { }
        }

        private void LayoutRoot_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Delete || e.Key == Key.Back)
                    model3DPane.ImageProperties.RulersViewingPlanes.DeleteAllSelectedRulerToolsCommand.Execute(null);
            }
            catch { }
        }
        
        void UpdateCropManipulation(bool newValue)
        {
            if (_cropVisible && _cropEnable)
            {
                _cropAdorner.Remove();
                _cropAdorner = null;
                if (newValue)
                    showAndBindCropAdorners("PolygonControlThumbStyle", "AnchorControlThumbStyle");
                else
                    showAndBindCropAdorners("PolygonThumbStyle", "AnchorThumbStyle");
            }            
        }

        private void ExportCurrentImageBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                ViewExportSelected3DImage exportDlg = new ViewExportSelected3DImage();
                if (model3DPane.ImageProperties.ParentVisualization != null)
                {
                    model3DPane.ImageProperties.ParentVisualization.PrepareExportedSelectedImage();
                    exportDlg.DataContext = model3DPane.ImageProperties.ParentVisualization;
                    exportDlg.ShowDialog();
                }
            }
            catch
            {
            }
        }

        public bool ViewSelectPlaneIsEnabledXY
        {
            get { return (bool)GetValue(ViewSelectPlaneIsEnabledXYProperty); }
            set { SetValue(ViewSelectPlaneIsEnabledXYProperty, (bool)value); }
        }
        public static readonly DependencyProperty ViewSelectPlaneIsEnabledXYProperty =
            DependencyProperty.Register(
             "ViewSelectPlaneIsEnabledXY",
             typeof(bool),
             typeof(View3DGrid),
             new FrameworkPropertyMetadata((bool)true)
            );

        public bool ViewSelectPlaneIsEnabledXZ
        {
            get { return (bool)GetValue(ViewSelectPlaneIsEnabledXZProperty); }
            set { SetValue(ViewSelectPlaneIsEnabledXZProperty, (bool)value); }
        }
        public static readonly DependencyProperty ViewSelectPlaneIsEnabledXZProperty =
            DependencyProperty.Register(
             "ViewSelectPlaneIsEnabledXZ",
             typeof(bool),
             typeof(View3DGrid),
             new FrameworkPropertyMetadata((bool)true)
            );

        public bool ViewSelectPlaneIsEnabledYZ
        {
            get { return (bool)GetValue(ViewSelectPlaneIsEnabledYZProperty); }
            set { SetValue(ViewSelectPlaneIsEnabledYZProperty, (bool)value); }
        }
        public static readonly DependencyProperty ViewSelectPlaneIsEnabledYZProperty =
            DependencyProperty.Register(
             "ViewSelectPlaneIsEnabledYZ",
             typeof(bool),
             typeof(View3DGrid),
             new FrameworkPropertyMetadata((bool)true)
            );

        public bool ViewSelectPlaneIsEnabledView3D
        {
            get { return (bool)GetValue(ViewSelectPlaneIsEnabledView3DProperty); }
            set { SetValue(ViewSelectPlaneIsEnabledView3DProperty, (bool)value); }
        }
        public static readonly DependencyProperty ViewSelectPlaneIsEnabledView3DProperty =
            DependencyProperty.Register(
             "ViewSelectPlaneIsEnabledView3D",
             typeof(bool),
             typeof(View3DGrid),
             new FrameworkPropertyMetadata((bool)true)
            );

    }
}