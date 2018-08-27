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
using System.Collections.ObjectModel;
using Xvue.MSOT.DataModels.Plugins.ProjectManager.MsotProject;
using System.Windows.Media.Media3D;
using Xvue.MSOT.ViewModels.Imaging;
using Xvue.MSOT.ViewModels.ProjectManager.ImagingSession;
using Xvue.MSOT.DataModels.Plugins.Imaging.Session;
using Xvue.Imaging;

namespace ViewMSOT.UIControls
{
	/// <summary>
	/// Interaction logic for ViewTexture3d.xaml
	/// </summary>
	public partial class ViewTexture3D : UserControl
	{
        PointCollection texturePointsFront = new PointCollection() { new Point(0, 1), new Point(1, 1), new Point(0, 0), new Point(0, 0), new Point(1, 1), new Point(1, 0) };
        PointCollection texturePointsBack = new PointCollection() { new Point(0, 0), new Point(1, 0), new Point(1, 1), new Point(1, 1), new Point(0, 1), new Point(0, 0) };
        //PointCollection texturePointsBack = new PointCollection() { new Point(1, 0), new Point(0, 0), new Point(0, 1), new Point(0, 1), new Point(1, 1), new Point(1, 0) };            

		public ViewTexture3D()
		{
            matrixYZ = new Matrix();
            matrixYZ.ScaleAt(-1.0, 1.0, 0.5, 0.5);

            matrixXZ = new Matrix();
            matrixXZ.ScaleAt(-1.0, 1.0, 0.5, 0.5);
            matrixXZ.RotateAt(-90.0, 0.5, 0.5);
            matrixXZ.ScaleAt(-1.0, 1.0, 0.5, 0.5);

            _whiteAmbientLight = new AmbientLight(Colors.White);

            _yPlaneVisual3D = new ModelVisual3D();
            _xPlaneVisual3D = new ModelVisual3D();
            _zPlaneVisual3D = new ModelVisual3D();

            _yPlaneVisual3D.Content = new Model3DGroup();
            (_yPlaneVisual3D.Content as Model3DGroup).Children.Add(_whiteAmbientLight);
            _xPlaneVisual3D.Content = new Model3DGroup();
            (_xPlaneVisual3D.Content as Model3DGroup).Children.Add(_whiteAmbientLight);
            _zPlaneVisual3D.Content = new Model3DGroup();
            (_zPlaneVisual3D.Content as Model3DGroup).Children.Add(_whiteAmbientLight);

            _zRegionsSlicedModels = null;

            _zSelectedPlaneIndex = 0;
            //_zSelectedPlanePosition = 0;
            _zPrevSelectedPlaneIndex = -1;

            _slicedGrid = new Point3D[3, 4];
            _zRegionSlicesCoord = new Point3D[4];
            //BorderXYColor = BorderXZColor = BorderYZColor = Colors.Blue;
            BorderXYColor = Colors.Blue;
            BorderYZColor = Colors.Green;
            BorderXZColor = Colors.Yellow;
            _isRegionsIndexedModel3D = false;

            this.InitializeComponent();

            ResetScene();
        }

        public void ResetCamera()
        {
            volume3DControl.ResetTrackball();
        }

        #region localvariables
        Matrix matrixYZ;
        Matrix matrixXZ;

        Point3D[,] _slicedGrid;
        Point3D[] _zRegionSlicesCoord;
        Viewport3D _activeViewport;

        AmbientLight _whiteAmbientLight;

        ModelVisual3D _zPlaneVisual3D;
        ModelVisual3D _xPlaneVisual3D;
        ModelVisual3D _yPlaneVisual3D;

        int _zSelectedPlaneIndex;
        int _zPrevSelectedPlaneIndex;
        //double _zSelectedPlanePosition;

        GeometryModel3D _yzImageSlicedModel;
        GeometryModel3D _xzImageSlicedModel;
        GeometryModel3D _xyImageSlicedModel;
        GeometryModel3D[] _zRegionsSlicedModels;

        bool _isRegionsIndexedModel3D;

        BitmapSource _blankXY;
        BitmapSource _blankXZ;
        BitmapSource _blankYZ;

        #endregion localvariables

        #region properties

        public bool IsCamera3DControlEnabled
        {
            get { return (bool)GetValue(IsCamera3DControlEnabledProperty); }
            set { SetValue(IsCamera3DControlEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsCamera3DControlEnabledProperty =
            DependencyProperty.Register(
            "IsCamera3DControlEnabled",
            typeof(bool),
            typeof(ViewTexture3D),
            new FrameworkPropertyMetadata((bool)true));

        public Color BorderXYColor;
        public Color BorderXZColor;
        public Color BorderYZColor;

        public ImageSource ImageXY
        {
            get { return (ImageSource)GetValue(ImageXYProperty); }
            set { SetValue(ImageXYProperty, value); }
        }

        public ImageSource ImageXZ
        {
            get { return (ImageSource)GetValue(ImageXZProperty); }
            set { SetValue(ImageXZProperty, value); }
        }

        public ImageSource ImageYZ
        {
            get { return (ImageSource)GetValue(ImageYZProperty); }
            set { SetValue(ImageYZProperty, value); }
        }

        public static readonly DependencyProperty ImageXYProperty =
            DependencyProperty.Register(
            "ImageXY",
            typeof(ImageSource),
            typeof(ViewTexture3D),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(changeImagesXY)));

        public static readonly DependencyProperty ImageXZProperty =
            DependencyProperty.Register(
            "ImageXZ",
            typeof(ImageSource),
            typeof(ViewTexture3D),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(changeImagesXZ)));

        public static readonly DependencyProperty ImageYZProperty =
            DependencyProperty.Register(
            "ImageYZ",
            typeof(ImageSource),
            typeof(ViewTexture3D),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(changeImagesYZ)));

        public double SelectedPlaneX
        {
            get { return (double)GetValue(SelectedPlaneXProperty); }
            set { SetValue(SelectedPlaneXProperty, value); }
        }

        public static readonly DependencyProperty SelectedPlaneXProperty =
            DependencyProperty.Register(
            "SelectedPlaneX",
            typeof(double),
            typeof(ViewTexture3D),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(ChangeDrawLocations)));

        public double SelectedPlaneY
        {
            get { return (double)GetValue(SelectedPlaneYProperty); }
            set { SetValue(SelectedPlaneYProperty, value); }
        }

        public static readonly DependencyProperty SelectedPlaneYProperty =
            DependencyProperty.Register(
            "SelectedPlaneY",
            typeof(double),
            typeof(ViewTexture3D),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(ChangeDrawLocations)));

        public double SelectedPlaneZ
        {
            get { return (double)GetValue(SelectedPlaneZProperty); }
            set { SetValue(SelectedPlaneZProperty, value); }
        }

        public static readonly DependencyProperty SelectedPlaneZProperty =
            DependencyProperty.Register(
            "SelectedPlaneZ",
            typeof(double),
            typeof(ViewTexture3D),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(ChangeDrawLocationsZ)));

        public double VolumeSizeX
        {
            get { return (double)GetValue(VolumeSizeXProperty); }
            set { SetValue(VolumeSizeXProperty, value); }
        }

        public static readonly DependencyProperty VolumeSizeXProperty =
            DependencyProperty.Register(
            "VolumeSizeX",
            typeof(double),
            typeof(ViewTexture3D),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(ChangeDrawLocations)));

        public double VolumeSizeY
        {
            get { return (double)GetValue(VolumeSizeYProperty); }
            set { SetValue(VolumeSizeYProperty, value); }
        }

        public static readonly DependencyProperty VolumeSizeYProperty =
            DependencyProperty.Register(
            "VolumeSizeY",
            typeof(double),
            typeof(ViewTexture3D),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(ChangeDrawLocations)));

        public double VolumeSizeZ
        {
            get { return (double)GetValue(VolumeSizeZProperty); }
            set { SetValue(VolumeSizeZProperty, value); }
        }

        public static readonly DependencyProperty VolumeSizeZProperty =
            DependencyProperty.Register(
            "VolumeSizeZ",
            typeof(double),
            typeof(ViewTexture3D),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(ChangeDrawLocationsZ)));

        public IEnumerable<ViewModel2DRegions> RegionsShapes
        {
            get { return (IEnumerable<ViewModel2DRegions>)GetValue(RegionsShapesProperty); }
            set { SetValue(RegionsShapesProperty, value); }
        }

        public static readonly DependencyProperty RegionsShapesProperty =
           DependencyProperty.Register(
              "RegionsShapes",
              typeof(IEnumerable<ViewModel2DRegions>),
              typeof(ViewTexture3D),
              new FrameworkPropertyMetadata(new List<ViewModel2DRegions>(),
                new PropertyChangedCallback(ChangeRegionsShapes)));

        public bool ShowRegions
        {
            get { return (bool)GetValue(ShowRegionsProperty); }
            set { SetValue(ShowRegionsProperty, value); }
        }

        public static readonly DependencyProperty ShowRegionsProperty =
            DependencyProperty.Register(
            "ShowRegions",
            typeof(bool),
            typeof(ViewTexture3D),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(ChangeShowRegions)));

        public bool IsRegionsDrawEnabled
        {
            get { return (bool)GetValue(IsRegionsDrawEnabledProperty); }
            set { SetValue(IsRegionsDrawEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsRegionsDrawEnabledProperty =
            DependencyProperty.Register(
            "IsRegionsDrawEnabled",
            typeof(bool),
            typeof(ViewTexture3D));

        public RegionShape CurrentShape
        {
            get { return (RegionShape)GetValue(CurrentShapeProperty); }
            set { SetValue(CurrentShapeProperty, value); }
        }

        public static readonly DependencyProperty CurrentShapeProperty =
            DependencyProperty.Register(
            "CurrentShape",
            typeof(RegionShape),
            typeof(ViewTexture3D),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(ChangeCurrentShape)));

        private static void ChangeRegionsShapes(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                if (e.NewValue == e.OldValue)
                    return;
                ViewTexture3D view = source as ViewTexture3D;

                if (!view.IsRegionsDrawEnabled)
                    return;

                view.refreshZPositions();
                view.refreshRegionShapes();
            }
            catch
            {
            }
        }

        private static void ChangeShowRegions(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                if (e.NewValue == e.OldValue)
                    return;
                ViewTexture3D view = source as ViewTexture3D;

                if (!view.IsVisible)
                    return;

                if (!view.IsRegionsDrawEnabled)
                    return;

                if (view._isRegionsIndexedModel3D)
                    view.initViewPortImagesXY();
                else
                    view.refreshViewPortImagesXY();
            }
            catch
            {
            }
        }

        private static void ChangeDrawLocations(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                ViewTexture3D view = source as ViewTexture3D;
                if (!view.IsVisible)
                    return;
                view.refreshSlicedGrid();
            }
            catch
            {
            }
        }

        private static void ChangeDrawLocationsZ(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                ViewTexture3D view = source as ViewTexture3D;
                if (!view.IsVisible)
                    return;
                view.refreshSlicedGrid();
                view.refreshZPositions();
            }
            catch
            {
            }
        }

        private static void changeImagesXY(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                if (e.NewValue == null || e.NewValue == e.OldValue)
                    return;
                ViewTexture3D view = source as ViewTexture3D;
                if (!view.IsVisible)
                    return;
                ViewTexture3D.refreshBlankImage(view.ImageXY, ref view._blankXY);
                if (view._isRegionsIndexedModel3D)
                    //view.refreshViewPortImageXY(view._zSelectedPlaneIndex);
                    view.refreshViewPortImageXY();
                else
                    view.refreshViewPortImagesXY();                
            }
            catch
            {
            }
        }

        private static void changeImagesXZ(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                if (e.NewValue == null || e.NewValue == e.OldValue)
                    return;
                ViewTexture3D view = source as ViewTexture3D;
                if (!view.IsVisible)
                    return;
                ViewTexture3D.refreshBlankImage(view.ImageXZ, ref view._blankXZ);
                view.refreshViewPortImagesXZ();
            }
            catch
            {
            }
        }

        private static void changeImagesYZ(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                if (e.NewValue == null || e.NewValue == e.OldValue)
                    return;
                ViewTexture3D view = source as ViewTexture3D;
                if (!view.IsVisible)
                    return;
                ViewTexture3D.refreshBlankImage(view.ImageYZ, ref view._blankYZ);
                view.refreshViewPortImagesYZ();
            }
            catch
            {
            }
        }

        private static void ChangeCurrentShape(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                ViewTexture3D view = source as ViewTexture3D;
                if (!view.IsVisible)
                    return;
                if (view._isRegionsIndexedModel3D)
                    //view.refreshViewPortImageXY(view._zSelectedPlaneIndex);
                    view.refreshViewPortImageXY();
                else
                    view.refreshViewPortImagesXY();
            }
            catch
            {
            }
        }
        #endregion properties

        static void refreshBlankImage(ImageSource templateImage, ref BitmapSource blankImage)
        {
            if (blankImage != null)
            {
                if (blankImage.Height == templateImage.Height &&
                    blankImage.Width == templateImage.Width)
                    return;
            }

            int stride = BitmapUtilities.CalculateStride(PixelFormats.Bgra32, (int)templateImage.Width);
            byte[] pixelValues = new byte[(int)templateImage.Height * stride];
            for (int i = 0; i < pixelValues.Length; i = i + 4)
            {
                pixelValues[i] = 0;
                pixelValues[i + 1] = 0;
                pixelValues[i + 2] = 0;
                pixelValues[i + 3] = 255;
            }
            blankImage = BitmapSource.Create((int)templateImage.Width, (int)templateImage.Height, 96, 96, PixelFormats.Bgra32, null, pixelValues, stride);
        }

        private void refreshActiveViewport()
        {
            try
            {
                slicedViewport.Children.Clear();
                //slicedViewportOrtho.Children.Clear();
                //if (orthoRadioButton.IsChecked.Value)
                //{
                //    slicedViewport.Visibility = System.Windows.Visibility.Collapsed;
                //    slicedViewportOrtho.Visibility = System.Windows.Visibility.Visible;
                //    _activeViewport = slicedViewportOrtho;
                //}
                //else
                //{
                    slicedViewport.Visibility = System.Windows.Visibility.Visible;
                    //slicedViewportOrtho.Visibility = System.Windows.Visibility.Collapsed;
                    _activeViewport = slicedViewport;
                //}
                _activeViewport.Children.Add(_xPlaneVisual3D);
                _activeViewport.Children.Add(_yPlaneVisual3D);
                _activeViewport.Children.Add(_zPlaneVisual3D);
            }
            catch { }
        }

        void refreshSlicedGrid()
        {
            double xySizeD2High = 1000 * VolumeSizeX / 2;
            double xySizeD2Low = -1000 * VolumeSizeX / 2;
            double zSizeD2High = 1000 * VolumeSizeZ / 2;
            double zSizeD2Low = -1000 * VolumeSizeZ / 2;

            double aZ = zSizeD2High - 1000 * VolumeSizeZ * (1 - SelectedPlaneZ);
            double aY = xySizeD2High - 1000 * VolumeSizeX * SelectedPlaneY;
            double aX = xySizeD2High - 1000 * VolumeSizeX * (1 - SelectedPlaneX);
            _slicedGrid = new Point3D[3, 4];
            _slicedGrid[0, 0] = new Point3D(xySizeD2Low, xySizeD2Low, aZ);
            _slicedGrid[0, 1] = new Point3D(xySizeD2High, xySizeD2Low, aZ);
            _slicedGrid[0, 2] = new Point3D(xySizeD2High, xySizeD2High, aZ);
            _slicedGrid[0, 3] = new Point3D(xySizeD2Low, xySizeD2High, aZ);

            _slicedGrid[1, 0] = new Point3D(aX, xySizeD2Low, zSizeD2High);
            _slicedGrid[1, 1] = new Point3D(aX, xySizeD2Low, zSizeD2Low);
            _slicedGrid[1, 2] = new Point3D(aX, xySizeD2High, zSizeD2Low);
            _slicedGrid[1, 3] = new Point3D(aX, xySizeD2High, zSizeD2High);

            _slicedGrid[2, 0] = new Point3D(xySizeD2High, aY, zSizeD2High);
            _slicedGrid[2, 1] = new Point3D(xySizeD2High, aY, zSizeD2Low);
            _slicedGrid[2, 2] = new Point3D(xySizeD2Low, aY, zSizeD2Low);
            _slicedGrid[2, 3] = new Point3D(xySizeD2Low, aY, zSizeD2High);

            _zRegionSlicesCoord[0] = new Point3D(xySizeD2Low, xySizeD2Low, 0);
            _zRegionSlicesCoord[1] = new Point3D(xySizeD2High, xySizeD2Low, 0);
            _zRegionSlicesCoord[2] = new Point3D(xySizeD2High, xySizeD2High, 0);
            _zRegionSlicesCoord[3] = new Point3D(xySizeD2Low, xySizeD2High, 0);
        }

        private void refreshZPositions()
        {
            try
            {
                List<ViewModel2DRegions> _regionsShapes = RegionsShapes as List<ViewModel2DRegions>;
                if (_regionsShapes == null)
                {
                    _zSelectedPlaneIndex = -1;
                    //_zSelectedPlanePosition = 1000 * VolumeSizeZ * (1 / 2 - SelectedPlaneZ);
                    return;
                }

                if (_regionsShapes.Count > 0 && IsRegionsDrawEnabled)
                {
                    _zPrevSelectedPlaneIndex = _zSelectedPlaneIndex;
                    _zSelectedPlaneIndex = (int)Math.Floor(((double)_regionsShapes.Count) * SelectedPlaneZ);
                    if (_zPrevSelectedPlaneIndex != _zSelectedPlaneIndex)
                    {
                        //refreshViewPortImageXY(_zPrevSelectedPlaneIndex);
                        refreshViewPortImageXY();
                        //refreshViewPortImageXY(_zSelectedPlaneIndex);
                    }
                    //_zSelectedPlanePosition = 1000.0 * VolumeSizeZ * (0.5 - ((double)_zSelectedPlaneIndex / (double)_regionsShapes.Count));
                    return;
                }
                else
                {
                    _zSelectedPlaneIndex = 0;
                    //_zSelectedPlanePosition = 1000 * VolumeSizeZ * (1 / 2 - SelectedPlaneZ);
                }
            }
            catch
            {

            }
        }

        private void refreshRegionShapes()
        {
            try
            {
                List<ViewModel2DRegions> _regionsShapes = RegionsShapes as List<ViewModel2DRegions>;
                if (_regionsShapes == null || !IsRegionsDrawEnabled)
                {
                    _zRegionsSlicedModels = null;
                    _isRegionsIndexedModel3D = false;
                    initViewPortSingleImageXY();
                    return;
                }

                foreach (ViewModel2DRegions region in _regionsShapes)
                {
                    region.ItemChanged += region_ItemChanged;
                }

                if (_regionsShapes.Count > 0)
                {
                    _isRegionsIndexedModel3D = true;
                    _zRegionsSlicedModels = new GeometryModel3D[_regionsShapes.Count * 2];
                    initViewPortImagesXY();
                }
            }
            catch
            {
            }
        }

        void region_ItemChanged(object sender,int index)
        {
            //refreshViewPortImageXY(index);
            refreshViewPortImageXY();
        }

        private void initViewPortImagesXY()
        {
            try
            {
                List<ViewModel2DRegions> _regionsShapes = RegionsShapes as List<ViewModel2DRegions>;
                Model3DGroup model3DGroupXY = _zPlaneVisual3D.Content as Model3DGroup;
                _zRegionsSlicedModels = new GeometryModel3D[_regionsShapes.Count * 2];
                model3DGroupXY.Children.Clear();
                model3DGroupXY.Children.Add(_whiteAmbientLight);
                for (int i = 0; i < _regionsShapes.Count; i++)
                {
                    _zRegionsSlicedModels[_regionsShapes.Count - 1 - i] = createSingleImageMeshFirst(_zRegionSlicesCoord, new Matrix(), BorderXYColor, ImageXY, i, _blankXY);
                    _zRegionsSlicedModels[_regionsShapes.Count + i] = createSingleImageMeshSecond(_zRegionSlicesCoord, new Matrix(), BorderXYColor, ImageXY, i, _blankXY);
                }
                for (int i = 0; i < _zRegionsSlicedModels.Length; i++)
                    if (_zRegionsSlicedModels[i] != null)
                        model3DGroupXY.Children.Add(_zRegionsSlicedModels[i]);
            }
            catch
            {
            }
        }

        private void initViewPortSingleImageXY()
        {
            try
            {
                Model3DGroup model3DGroupXY = _zPlaneVisual3D.Content as Model3DGroup;
                model3DGroupXY.Children.Clear();
                model3DGroupXY.Children.Add(_whiteAmbientLight);
                refreshViewPortImagesXY();
            }
            catch
            {
            }
        }

        //void refreshViewPortImageXY(int index)
        void refreshViewPortImageXY()
        {
            try
            {
                //Model3DGroup model3DGroupXY = _zPlaneVisual3D.Content as Model3DGroup;
                //int secondIndex = _zRegionsSlicedModels.Length - 1 - index;
                //int front = model3DGroupXY.Children.IndexOf(_zRegionsSlicedModels[index]);
                //int back = model3DGroupXY.Children.IndexOf(_zRegionsSlicedModels[secondIndex]);
                //if (front < 0 || back < 0)
                //{
                    initViewPortImagesXY();
                    //return;
                //}

                //Do not delete these commented code
                //_zRegionsSlicedModels[index] = createSingleImageMeshFirst(_zRegionSlicesCoord, new Matrix(), BorderXYColor, ImageXY, index);
                //_zRegionsSlicedModels[secondIndex] = createSingleImageMeshSecond(_zRegionSlicesCoord, new Matrix(), BorderXYColor, ImageXY, index);
                //if (_zRegionsSlicedModels[index] != null && front >= 0)
                //    model3DGroupXY.Children[front] = _zRegionsSlicedModels[index];
                //else
                //    model3DGroupXY.Children.RemoveAt(front);
                //if (_zRegionsSlicedModels[secondIndex] != null && back >= 0)
                //    model3DGroupXY.Children[back] = _zRegionsSlicedModels[secondIndex];
                //else
                //    model3DGroupXY.Children.RemoveAt(back);
            }
            catch
            {
            }
        }

        void refreshViewPortImagesXY()
        {
            try
            {
                Model3DGroup model3DGroupXY = _zPlaneVisual3D.Content as Model3DGroup;
                model3DGroupXY.Children.Remove(_xyImageSlicedModel);
                if (ImageXY != null)
                {
                    _xyImageSlicedModel = createSingleImageMesh(_slicedGrid, 0, new Matrix(), BorderXYColor, ImageXY, _blankXY);
                    if (_xyImageSlicedModel != null)
                        model3DGroupXY.Children.Add(_xyImageSlicedModel);
                }
            }
            catch
            {
            }
        }

        void refreshViewPortImagesXZ()
        {
            try
            {
                Model3DGroup model3DGroupXZ = _yPlaneVisual3D.Content as Model3DGroup;
                model3DGroupXZ.Children.Remove(_xzImageSlicedModel);
                if (ImageXZ != null)
                {
                    _xzImageSlicedModel = createSingleImageMesh(_slicedGrid, 2, matrixXZ, BorderXZColor, ImageXZ, _blankXZ);
                    if (_xzImageSlicedModel != null)
                        model3DGroupXZ.Children.Add(_xzImageSlicedModel);
                }
            }
            catch
            {
            }
        }

        void refreshViewPortImagesYZ()
        {
            try
            {
                Model3DGroup model3DGroupYZ = _xPlaneVisual3D.Content as Model3DGroup;
                model3DGroupYZ.Children.Remove(_yzImageSlicedModel);
                if (ImageYZ != null)
                {
                    _yzImageSlicedModel = createSingleImageMesh(_slicedGrid, 1, matrixYZ, BorderYZColor, ImageYZ, _blankYZ);
                    if (_yzImageSlicedModel != null)
                        model3DGroupYZ.Children.Add(_yzImageSlicedModel);
                }
            }
            catch
            {
            }
        }

        private GeometryModel3D createSingleImageMesh(Point3D[,] slicedGrid, int dim, Matrix genMatrix, Color borderColor, ImageSource image, BitmapSource blankImage)
        {
            try
            {
                ImageSource sliceImage = image;
                Rect sliceImageRect = new Rect(0, 0, sliceImage.Width, sliceImage.Height);
                DrawingGroup dg = new DrawingGroup();

                if (blankImage != null)
                    dg.Children.Add(new ImageDrawing(blankImage, sliceImageRect));

                dg.Children.Add(new ImageDrawing(sliceImage, sliceImageRect));
                RectangleGeometry rGeometry = new RectangleGeometry(sliceImageRect);
                dg.Children.Add(new GeometryDrawing(null, new Pen(new SolidColorBrush(borderColor), 1), rGeometry));
                dg.ClipGeometry = new RectangleGeometry(sliceImageRect);
                if (dim == 0)
                {
                    if (CurrentShape != null && CurrentShape.PathPoints != null)
                    {
                        GeometryDrawing gd;
                        if (CurrentShape.PathPoints.Count > 0)
                            gd = DrawRegion(CurrentShape);
                        else
                            gd = null;
                        if (gd != null)
                            dg.Children.Add(gd);
                    }
                }
                sliceImage = new DrawingImage(dg);

                Point3D p0 = slicedGrid[dim, 0];
                Point3D p1 = slicedGrid[dim, 1];
                Point3D p2 = slicedGrid[dim, 2];
                Point3D p3 = slicedGrid[dim, 3];
                Point3DCollection positionsFront = new Point3DCollection() { p0, p1, p3, p3, p1, p2 };

                MeshGeometry3D meshGeometry3DFront = new MeshGeometry3D();
                meshGeometry3DFront.Positions = positionsFront;
                GeometryModel3D sliceGeometryModelFront = new GeometryModel3D();
                sliceGeometryModelFront.Geometry = meshGeometry3DFront;
                meshGeometry3DFront.TextureCoordinates = texturePointsFront;
                ImageBrush imageBrushTransformed = new ImageBrush(sliceImage);
                imageBrushTransformed.RelativeTransform = new MatrixTransform(genMatrix);
                DiffuseMaterial imageMaterial = new DiffuseMaterial(imageBrushTransformed);
                imageMaterial.Freeze();
                sliceGeometryModelFront.Material = imageMaterial;
                sliceGeometryModelFront.BackMaterial = imageMaterial;
                meshGeometry3DFront.Freeze();
                sliceGeometryModelFront.Freeze();
                return sliceGeometryModelFront;
            }
            catch
            {
                return null;
            }
        }

        bool addImageToDrawingGroup(DrawingGroup dg, ImageSource sliceImage, Color borderColor, int index, BitmapSource blankImage)
        {
            bool add = false;
            Rect imageRect = new Rect(0, 0, sliceImage.Width, sliceImage.Height);
            if (index == _zSelectedPlaneIndex)
            {
                if (blankImage != null)
                    dg.Children.Add(new ImageDrawing(blankImage, imageRect));

                dg.Children.Add(new ImageDrawing(sliceImage, imageRect));
                RectangleGeometry rGeometry = new RectangleGeometry(imageRect);
                dg.Children.Add(new GeometryDrawing(null, new Pen(new SolidColorBrush(borderColor), 1), rGeometry));
                dg.ClipGeometry = new RectangleGeometry(imageRect);
                add = true;
            }
            else
            {
                dg.Children.Add(new GeometryDrawing(Brushes.Transparent, new Pen(), new RectangleGeometry(imageRect)));
                dg.ClipGeometry = new RectangleGeometry(imageRect);
            }
            return add;
        }

        bool addRoisToDrawingGroup(DrawingGroup dg, int index, List<ViewModel2DRegions> _regionsShapes)
        {
            bool add = false;
            GeometryDrawing gd;
            if (CurrentShape != null)
                if (CurrentShape.PathPoints.Count > 0 && index == _zSelectedPlaneIndex)
                {
                    gd = DrawRegion(CurrentShape);
                    if (gd != null)
                    {
                        dg.Children.Add(gd);
                        add = true;
                    }
                }

            if (ShowRegions)
                foreach (RegionShape region in _regionsShapes[index].AvailableRegionsShapes)
                    if (region.PathPoints.Count > 0)
                    {
                        gd = DrawRegion(region);
                        if (gd != null)
                        {
                            dg.Children.Add(gd);
                            add = true;
                        }
                    }

            if (_regionsShapes[index].CurrentShape.PathPoints.Count > 0)
            {
                gd = DrawRegion(_regionsShapes[index].CurrentShape);
                if (gd != null)
                {
                    dg.Children.Add(gd);
                    add = true;
                }
            }
            return add;
        }

        static GeometryModel3D createGeometryModel3D(Point3DCollection meshPositions, ImageSource sliceImage, Matrix genMatrix, PointCollection textureCoordinates)
        {
            MeshGeometry3D meshGeometry = new MeshGeometry3D();
            meshGeometry.Positions = meshPositions;
            GeometryModel3D sliceGeometryModel = new GeometryModel3D();
            sliceGeometryModel.Geometry = meshGeometry;
            meshGeometry.TextureCoordinates = textureCoordinates;
            ImageBrush imageBrushTransformed = new ImageBrush(sliceImage);
            imageBrushTransformed.RelativeTransform = new MatrixTransform(genMatrix);
            DiffuseMaterial imageMaterial = new DiffuseMaterial(imageBrushTransformed);
            imageMaterial.Freeze();
            sliceGeometryModel.BackMaterial = imageMaterial;
            meshGeometry.Freeze();
            sliceGeometryModel.Freeze();
            return sliceGeometryModel;
        }

        GeometryModel3D createSingleImageMeshFirst(Point3D[] zRegionSlicesCoord, Matrix genMatrix, Color borderColor, ImageSource image, int index, BitmapSource blankImage)
        {
            try
            {
                List<ViewModel2DRegions> _regionsShapes = RegionsShapes as List<ViewModel2DRegions>;
                bool add = false;

                if (ImageXY == null)
                    return null;

                DrawingGroup dg = new DrawingGroup();
                add = addImageToDrawingGroup(dg, image, borderColor, index, blankImage);
                add |= addRoisToDrawingGroup(dg, index, _regionsShapes);

                if (!add)
                    return null;


                Point3D p0 = zRegionSlicesCoord[0];
                Point3D p1 = zRegionSlicesCoord[1];
                Point3D p2 = zRegionSlicesCoord[2];
                Point3D p3 = zRegionSlicesCoord[3];
                p0.Z = p1.Z = p2.Z = p3.Z = 1000.0 * VolumeSizeZ * (0.5 - (1 - (double)index / (double)_regionsShapes.Count));

                return createGeometryModel3D(new Point3DCollection() { p0, p1, p3, p3, p1, p2 }, new DrawingImage(dg), genMatrix, texturePointsFront);
            }
            catch
            {
                return null;
            }
        }

        GeometryModel3D createSingleImageMeshSecond(Point3D[] zRegionSlicesCoord, Matrix genMatrix, Color borderColor, ImageSource image, int index, BitmapSource blankImage)
        {
            try
            {
                List<ViewModel2DRegions> _regionsShapes = RegionsShapes as List<ViewModel2DRegions>;
                bool add = false;

                if (ImageXY == null)
                    return null;

                DrawingGroup dg = new DrawingGroup();
                add = addImageToDrawingGroup(dg, image, borderColor, index, blankImage);
                if(add)
                    _zPrevSelectedPlaneIndex = index;

                add |= addRoisToDrawingGroup(dg, index, _regionsShapes);

                if (!add)
                    return null;

                Point3D p0 = zRegionSlicesCoord[0];
                Point3D p1 = zRegionSlicesCoord[1];
                Point3D p2 = zRegionSlicesCoord[2];
                Point3D p3 = zRegionSlicesCoord[3];
                p0.Z = p1.Z = p2.Z = p3.Z = 1000.0 * VolumeSizeZ * (0.5 - (1- (double)index / (double)_regionsShapes.Count));

                return createGeometryModel3D(new Point3DCollection() { p3, p2, p1, p1, p0, p3 }, new DrawingImage(dg), genMatrix, texturePointsBack);
            }
            catch
            {
                return null;
            }
        }

        public void ResetScene()
        {
            try
            {
                List<ViewModel2DRegions> _regionsShapes = RegionsShapes as List<ViewModel2DRegions>;
                refreshActiveViewport();

                refreshSlicedGrid();
                refreshZPositions();

                if (_regionsShapes == null || !IsRegionsDrawEnabled)
                {
                    _zRegionsSlicedModels = null;
                    _isRegionsIndexedModel3D = false;
                }
                else
                {
                    foreach (ViewModel2DRegions region in _regionsShapes)
                    {
                        region.ItemChanged -= region_ItemChanged;
                    }
                    if (_regionsShapes.Count > 0)
                    {
                        _isRegionsIndexedModel3D = true;
                        _zRegionsSlicedModels = new GeometryModel3D[_regionsShapes.Count * 2];
                        foreach (ViewModel2DRegions region in _regionsShapes)
                        {
                            region.ItemChanged += region_ItemChanged;
                        }
                    }
                }

                if (_isRegionsIndexedModel3D)
                    initViewPortImagesXY();
                else
                    refreshViewPortImagesXY();

                refreshViewPortImagesYZ();

                refreshViewPortImagesXZ();
            }
            catch (Exception ex)
            {
                Xvue.MSOT.ViewModels.Log.ViewModelLog.MsotTrace("ResetScene exception: " + ex.Message, this.GetType().ToString());
            }
        }

        private void radioButton_Checked(object sender, RoutedEventArgs e)
        {
            ResetScene();
        }

        protected static GeometryDrawing DrawRegion(RegionShape shape)
        {
            try
            {
                GeometryGroup roiDrawing = new GeometryGroup();

                roiDrawing.Children.Add((Geometry)drawPath(shape.Shape, shape.PathPoints));
                Pen myPen = new Pen();
                myPen.Thickness = 1;// ReconNode.Resolution > 200 ? 1 : 2;
                myPen.LineJoin = PenLineJoin.Round;
                myPen.EndLineCap = PenLineCap.Round;
                LinearGradientBrush myLinearGradientBrush = new LinearGradientBrush();
                myLinearGradientBrush.GradientStops.Add(new GradientStop(shape.Pen, 0));
                myLinearGradientBrush.GradientStops.Add(new GradientStop(shape.Pen, 1));
                myPen.Brush = myLinearGradientBrush;
                return (new GeometryDrawing(
                         Brushes.Transparent,
                         myPen,
                         roiDrawing
                         ));
            }
            catch
            {
                return new GeometryDrawing();
            }
        }

        static Geometry drawPath(RoiShape shape, ObservableCollection<Point> _pathPoints)
        {
            try
            {
                double scaleX, scaleY;
                scaleX = scaleY = 1;
                Geometry geo = null;

                switch (shape)
                {
                    case RoiShape.Polygon:
                        geo = new StreamGeometry();
                        using (var ctx = ((StreamGeometry)geo).Open())
                        {
                            Point t = new Point(_pathPoints[0].X * scaleX, _pathPoints[0].Y * scaleY);
                            ctx.BeginFigure(t, false, true);
                            List<Point> data = new List<Point>();
                            for (int i = 1; i < _pathPoints.Count; i++)
                            {
                                t = new Point(_pathPoints[i].X * scaleX, _pathPoints[i].Y * scaleY);
                                data.Add(t);
                            }
                            ctx.PolyLineTo(data, true, false);
                        }
                        geo.Freeze();
                        break;
                    case RoiShape.Rectangle:

                        if (_pathPoints.Count == 2)
                        {
                            Point t0 = new Point(_pathPoints[0].X * scaleX, _pathPoints[0].Y * scaleY);
                            Point t1 = new Point(_pathPoints[1].X * scaleX, _pathPoints[1].Y * scaleY);
                            geo = new RectangleGeometry(new Rect(t0, t1));
                        }
                        //else if (_pathPoints.Count == 3)
                        //{
                        //    Point t0 = new Point(_pathPoints[0].X * scaleX, _pathPoints[0].Y * scaleY);
                        //    Point t1 = new Point(_pathPoints[1].X * scaleX, _pathPoints[1].Y * scaleY);
                        //    Point t2 = new Point(_pathPoints[2].X * scaleX, _pathPoints[2].Y * scaleY);
                        //    Point center = new Point(Math.Abs(t0.X - t1.X), Math.Abs(t0.Y - t1.Y));
                        //    center = new Point(_pathPoints[0].X * scaleX, _pathPoints[0].Y * scaleY);
                        //    double angle = Math.Atan2(t2.Y - center.Y, t2.X - center.X) * 180 / Math.PI;
                        //    RotateTransform rotation = new RotateTransform(angle, t1.X, t1.Y);
                        //    Rect = new RectangleGeometry(new Rect(t0, t1), center.X, center.Y, rotation);                       
                        //}
                        break;
                    case RoiShape.Circle:
                        if (_pathPoints.Count == 2)
                        {
                            Point t0 = new Point(_pathPoints[0].X * scaleX, _pathPoints[0].Y * scaleY);
                            Point t1 = new Point(_pathPoints[1].X * scaleX, _pathPoints[1].Y * scaleY);
                            Vector radius = new Vector();
                            radius = t0 - t1;
                            geo = new EllipseGeometry(t0, radius.Length, radius.Length);
                        }
                        break;
                    case RoiShape.Ellipse:
                        if (_pathPoints.Count == 3)
                        {
                            Point t0 = new Point(_pathPoints[0].X * scaleX, _pathPoints[0].Y * scaleY);
                            Point t1 = new Point(_pathPoints[1].X * scaleX, _pathPoints[1].Y * scaleY);
                            Point t2 = new Point(_pathPoints[2].X * scaleX, _pathPoints[2].Y * scaleY);
                            Vector radiusX = new Vector();
                            radiusX = t0 - t1;
                            double angle = Math.Atan2(t1.Y - t0.Y, t1.X - t0.X) * 180 / Math.PI;
                            Vector radiusY = new Vector();
                            radiusY = t0 - t2;
                            RotateTransform Rotation = new RotateTransform(angle, t0.X, t0.Y);
                            geo = new EllipseGeometry(new Point(_pathPoints[0].X * scaleX, _pathPoints[0].Y * scaleY), radiusX.Length, radiusY.Length, Rotation);
                        }
                        break;
                }
                return geo;
            }
            catch
            {
                return null;
            }
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == true)
            {
                ResetScene();
            }
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            ResetScene();
            return base.ArrangeOverride(arrangeBounds);
        }

    }
}