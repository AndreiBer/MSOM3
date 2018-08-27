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
using Xvue.Framework.API.Commands;
using Xvue.Framework.Views.WPF.Controls;

namespace ViewMSOT.UIControls
{
	/// <summary>
	/// Interaction logic for ViewControlCamera3D.xaml
	/// </summary>
	public partial class ViewControlCamera3D : UserControl
	{
        #region localvariables
        Trackball _trackBall;
        #endregion localvariables

        public ViewControlCamera3D()
		{
			this.InitializeComponent();
            _trackBall = new Trackball();
        }

        #region properties

        public Camera3DType CameraType
        {
            get { return (Camera3DType)GetValue(CameraTypeProperty); }
            set { SetValue(CameraTypeProperty, value); }
        }

        public static readonly DependencyProperty CameraTypeProperty =
            DependencyProperty.Register(
            "CameraType",
            typeof(Camera3DType),
            typeof(ViewControlCamera3D),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(changeCameraType)));

        private static void changeCameraType(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                if (e.NewValue == e.OldValue)
                    return;

                ViewControlCamera3D control = source as ViewControlCamera3D;
                control.correctScale();
            }
            catch
            {
            }
        }

        private void correctScale()
        {
            Transform3D tmpTranform = TrackballTransform;
            int sign = 1;
            int k = 50;
            if (CameraType == Camera3DType.Orthographic)
            { sign = -1; k += 7; }
            else if (CameraType == Camera3DType.Perspective)
            { sign = +1; k += 4; }

            while (k >= 0)
            {
                tmpTranform = _trackBall.Zoom(sign);
                k--;
            }
            TrackballTransform = tmpTranform;
        }

        public Transform3D TrackballTransform
        {
            get { return (Transform3D)GetValue(TrackballTransformProperty); }
            set { SetValue(TrackballTransformProperty, value); }
        }

        public static readonly DependencyProperty TrackballTransformProperty =
            DependencyProperty.Register(
            "TrackballTransform",
            typeof(Transform3D),
            typeof(ViewControlCamera3D),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(updateTrackballTransform)));

        private static void updateTrackballTransform(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                //if (e.NewValue == e.OldValue)
                //    return;

                bool update = false;

                if (e.OldValue == null && e.NewValue != null)
                    update = true;
                else if (e.NewValue != null && e.OldValue != null)
                {
                    Matrix3D newM = (e.NewValue as Transform3D).Value;
                    Matrix3D oldM = (e.OldValue as Transform3D).Value;

                    if (!newM.Equals(oldM))
                        update = true;
                }

                if (update)
                {
                    ViewControlCamera3D control = source as ViewControlCamera3D;
                    control.UpdateTrackball();
                }
            }
            catch
            {
            }
        }
        #endregion properties

        #region cameraMouseInteraction
        private void viewport3D_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            TrackballTransform = _trackBall.Zoom(e.Delta);
            e.Handled = true;
        }

        private void viewport3D_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_touchEnabled)
                return;

            Point p = e.GetPosition(LayoutRoot);           
            UIElement a = (UIElement)sender;
            _trackBall.StartDrag(p, LayoutRoot.ActualWidth, LayoutRoot.ActualHeight);
            a.CaptureMouse();
        }

        private void viewport3D_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_touchEnabled)
                return;

            UIElement a = (UIElement)sender;
            a.ReleaseMouseCapture();
        }

        private void viewport3D_MouseMove(object sender, MouseEventArgs e)
        {
            if (_touchEnabled)
                return;

            UIElement a = (UIElement)sender;
            if (a.IsMouseCaptured)
            {
                TrackballTransform = _trackBall.Track(e.GetPosition(LayoutRoot), LayoutRoot.ActualWidth, LayoutRoot.ActualHeight);
            }
        }
        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.ResetTrackball();
        }

        public void ResetTrackball()
        {
            TrackballTransform = _trackBall.ResetTrackball();
            if (CameraType == Camera3DType.Orthographic)
                correctScale();
        }

        public void UpdateTrackball()
        {
            _trackBall.UpdateTransform(TrackballTransform);
        }

        Dictionary<TouchDevice, Point> _touchDevicesPoints = new Dictionary<TouchDevice, Point>();
        double _touchZoomDistance = 0;
        bool _touchEnabled = false;

        private void LayoutRoot_TouchEnter(object sender, TouchEventArgs e)
        {
            _touchDevicesPoints[e.TouchDevice] = e.GetTouchPoint(LayoutRoot).Position;

            if (_touchDevicesPoints.Count == 2)
            {
                List<Point> points = new List<Point>(_touchDevicesPoints.Values);
                _touchZoomDistance = Point.Subtract(points[0], points[1]).Length;

                _touchEnabled = true;
                e.Handled = true;
            }
        }

        private void LayoutRoot_TouchLeave(object sender, TouchEventArgs e)
        {
            _touchDevicesPoints.Remove(e.TouchDevice);

            if (_touchDevicesPoints.Count != 2)
            {
                _touchEnabled = false;
            }
        }

        private void LayoutRoot_TouchMove(object sender, TouchEventArgs e)
        {
            if (_touchDevicesPoints.Count != 2)
            {
                return;
            }
            else
            {
                Point newPoint = e.GetTouchPoint(LayoutRoot).Position;
                Point previousPoint = _touchDevicesPoints[e.TouchDevice];

                if (!newPoint.Equals(previousPoint))
                {
                    _touchDevicesPoints[e.TouchDevice] = newPoint;

                    List<Point> points = new List<Point>(_touchDevicesPoints.Values);

                    double newTouchZoomDistance = Point.Subtract(points[0], points[1]).Length;

                    double diff = newTouchZoomDistance - _touchZoomDistance;

                    if (Math.Abs(diff) > 5)
                    {
                        TrackballTransform = _trackBall.Zoom(newTouchZoomDistance, _touchZoomDistance);
                        _touchZoomDistance = newTouchZoomDistance;
                    }
                }

                e.Handled = true;
            }
        }
    }

}