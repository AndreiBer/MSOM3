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
	/// Interaction logic for ViewForegroundControlHeader.xaml
	/// </summary>
	public partial class ViewForegroundControlHeader : UserControl
	{

        bool _isDragging;

        private struct ViewModelImagingLayerDragObject
        {
            public ViewModelImagingLayerDragObject(ViewModelImagingLayer vm, double width) 
            {
                ViewModel = vm;
                Width = width;
            }
            public readonly ViewModelImagingLayer ViewModel;
            public readonly double Width;
        }


		public ViewForegroundControlHeader()
		{
			this.InitializeComponent();
            _isDragging = false;
		}

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    ViewForegroundControlHeader control = sender as ViewForegroundControlHeader;
                    if (control == null)
                        return;
                    else if (control.DataContext == null)
                        return;

                    DependencyObject parentTabItemDependencyObject = Xvue.Framework.Views.WPF.VisualTreeBrowser.GetAncestorByType(this, typeof(TabItem));

                    if (parentTabItemDependencyObject == null)
                        return;

                    TabItem parentTabItem = parentTabItemDependencyObject as TabItem;

                    if (!parentTabItem.IsSelected)
                        return;

                    ViewModelImagingLayer toMoveImageLayer = control.DataContext as ViewModelImagingLayer;
                    toMoveImageLayer.ImagingComponent.InitMoveComponent();

                    DataObject data = new DataObject();
                    ViewModelImagingLayerDragObject dragObject = new ViewModelImagingLayerDragObject(toMoveImageLayer, control.ActualWidth);
                    data.SetData("stackPanelDragItem", dragObject);

                    DependencyObject parentTabControlDependencyObject = Xvue.Framework.Views.WPF.VisualTreeBrowser.GetAncestorByType(this, typeof(TabControl));

                    DragDrop.DoDragDrop(control, data, DragDropEffects.Move);

                    //Prevent popup from not closing
                    if (parentTabControlDependencyObject != null)
                    {
                        TabControl parentTabControl = parentTabControlDependencyObject as TabControl;
                        parentTabControl.CaptureMouse();
                        parentTabControl.ReleaseMouseCapture();
                    }
                }
            }
            catch { }
        }

        private void UserControl_DragEnter(object sender, DragEventArgs e)
        {
            try
            {                
                if (_isDragging)
                    return;

                if (e.Data.GetDataPresent("stackPanelDragItem"))
                {
                    ViewForegroundControlHeader control = sender as ViewForegroundControlHeader;
                    if (control == null)
                        return;
                    else if (control.DataContext == null)
                        return;

                    ViewModelImagingLayerDragObject dragObject = (ViewModelImagingLayerDragObject)e.Data.GetData("stackPanelDragItem");
                    ViewModelImagingLayer model = control.DataContext as ViewModelImagingLayer;
                    if (dragObject.ViewModel != model)
                    {
                        control.SetCurrentValue(BackgroundProperty, new SolidColorBrush(Color.FromRgb(100, 100, 100)));
                        _isDragging = true;
                    }
                }
            }
            catch { }
        }

        private void UserControl_DragLeave(object sender, DragEventArgs e)
        {
            try
            {
                if (_isDragging)
                {
                    _isDragging = false;
                    ViewForegroundControlHeader control = sender as ViewForegroundControlHeader;
                    control.SetCurrentValue(BackgroundProperty, new SolidColorBrush(Color.FromArgb(0,0,0,0)));
                }
            }
            catch { }
        }

        private void UserControl_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            try
            {
                if (e.Effects.HasFlag(DragDropEffects.Move))
                    Mouse.SetCursor(Cursors.ScrollWE);
                else
                    Mouse.SetCursor(Cursors.No);
            }
            catch { }

            e.Handled = true;
        }

        private void UserControl_DragOver(object sender, DragEventArgs e)
        {
            try
            {
                if (!_isDragging)
                    return;

                if (e.Data.GetDataPresent("stackPanelDragItem"))
                {
                    ViewForegroundControlHeader control = sender as ViewForegroundControlHeader;
                    if (control == null)
                        return;
                    else if (control.DataContext == null)
                        return;

                    ViewModelImagingLayer model = control.DataContext as ViewModelImagingLayer;

                    ViewModelImagingLayerDragObject dragObject = (ViewModelImagingLayerDragObject)e.Data.GetData("stackPanelDragItem");

                    if (dragObject.ViewModel != model)
                    {
                        if (model == null)
                        {
                            return;
                        }
                        ViewModelImagingComponent stableComponent = model.ImagingComponent;

                        int movingIndex = stableComponent.ImageProperties.DragComponentIndex;
                        int stableIndex = stableComponent.ParentListIndex;

                        if (movingIndex == stableIndex)
                            return;

                        if (Math.Abs(movingIndex - stableIndex) == 1)
                        {
                            Point delta = e.GetPosition(control);
                            if (movingIndex < stableIndex)
                            {
                                if (delta.X < 20)
                                    return;
                            }
                            else
                            {
                                if (delta.X > control.ActualWidth - 20)
                                    return;
                            }

                            double deadZone = control.ActualWidth - dragObject.Width;
                            if (deadZone > 0)
                            {
                                if (movingIndex < stableIndex)
                                {
                                    if (delta.X <= deadZone)
                                    {
                                        return;
                                    }
                                }
                                else if (movingIndex > stableIndex)
                                {
                                    if (delta.X >= dragObject.Width)
                                    {
                                        return;
                                    }
                                }
                            }
                        }
                        
                        stableComponent.MoveRelativeComponent(stableComponent);
                        e.Handled = true;
                    }
                }
            }
            catch { }
        }

        private void UserControl_Drop(object sender, DragEventArgs e)
        {
            UserControl_DragLeave(sender, e);
        }

        private void toggleButton_Click(object sender, RoutedEventArgs e)
        {
            bool? state = toggleButton.IsChecked;
            if (state == true)
            {
                selectParentTabItem();
            }
        }

        private void selectParentTabItem()
        {
            DependencyObject parentTabItemDependencyObject = Xvue.Framework.Views.WPF.VisualTreeBrowser.GetAncestorByType(this, typeof(TabItem));
            if (parentTabItemDependencyObject != null)
            {
                (parentTabItemDependencyObject as TabItem).SetCurrentValue(TabItem.IsSelectedProperty, true);
            }
        }

	}
}