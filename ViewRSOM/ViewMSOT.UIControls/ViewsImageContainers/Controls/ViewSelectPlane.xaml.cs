using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
using Xvue.Framework.Views.WPF;
using Xvue.MSOT.Common;
using Xvue.MSOT.Services.Imaging;

namespace ViewMSOT.UIControls
{
	/// <summary>
    /// Interaction logic for ViewSelectPlane.xaml
	/// </summary>
	public partial class ViewSelectPlane : UserControl
    {
        #region localvariables
        #endregion localvariables

        public ViewSelectPlane()
		{
			this.InitializeComponent();
		}

        #region properties

        public Visible3DGridPlanesType Visible3DGridPlanes
        {
            get { return (Visible3DGridPlanesType)GetValue(Visible3DGridPlanesProperty); }
            set { SetValue(Visible3DGridPlanesProperty, value); }
        }

        public static readonly DependencyProperty Visible3DGridPlanesProperty =
            DependencyProperty.Register(
            "Visible3DGridPlanes",
            typeof(Visible3DGridPlanesType),
            typeof(ViewSelectPlane),
            new FrameworkPropertyMetadata(
                new PropertyChangedCallback(Visible3DGridPlanesChanged)));

        public Visible3DGridPlanesType Visible3DGridPlaneSelection
        {
            get { return (Visible3DGridPlanesType)GetValue(Visible3DGridPlaneSelectionProperty); }
            set { SetValue(Visible3DGridPlaneSelectionProperty, value); }
        }

        public static readonly DependencyProperty Visible3DGridPlaneSelectionProperty =
            DependencyProperty.Register(
            "Visible3DGridPlaneSelection",
            typeof(Visible3DGridPlanesType),
            typeof(ViewSelectPlane),
            new FrameworkPropertyMetadata());

        #endregion properties

        private static void Visible3DGridPlanesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ViewSelectPlane control = d as ViewSelectPlane;
            control.refreshButtonStyle();
        }

        private void refreshButtonStyle()
        {
            if (Visible3DGridPlanes == Visible3DGridPlanesType.All)
            {
                controlButton.Content = Application.Current.FindResource("IconShowPlaneExpand");
            }
            else
            {
                controlButton.Content = Application.Current.FindResource("IconShowPlaneCollapse");
            }
        }

        private void controlButton_Click(object sender, RoutedEventArgs e)
        {
            if (Visible3DGridPlanes == Visible3DGridPlaneSelection)
            {
                SetCurrentValue(ViewSelectPlane.Visible3DGridPlanesProperty, Visible3DGridPlanesType.All);
            }
            else
            {
                SetCurrentValue(ViewSelectPlane.Visible3DGridPlanesProperty, Visible3DGridPlaneSelection);
            }
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.refreshButtonStyle();
        }

    }

}