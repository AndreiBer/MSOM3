using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using ViewMSOT.UIControls;
using Xvue.Framework.Views.WPF.Behaviors;
using Xvue.Framework.Views.WPF.Controls;
using Xvue.MSOT.DataModels.Plugins.ProjectManager;
using Xvue.MSOT.ViewModels.ProjectManager.ImagingSession;

namespace ViewMSOTc
{

    public class SessionMenusContentControl : ContentControl
    {
        public UserControl ActiveSessionMenus { get; set; }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ActiveSessionMenus = GetTemplateChild("menus") as UserControl;
        }
    }

    /// <summary>
    /// Interaction logic for ViewImagingSuperSessionsGrid.xaml
    /// </summary>
    public partial class ViewImagingSuperSessionsGrid : UserControl
    {
        delegate void KeyboardShortcutDelegate();
        Dictionary<Key, KeyboardShortcutDelegate> _singleViewKeyBindings;
        Dictionary<Key, KeyboardShortcutDelegate> _multiViewKeyBindings;

        private ViewModelImagingSuperSession _model;
        private MouseClickInsideOfControlBehavior _mouseClickInsideImagePropertiesMenusBehavior;

        public ViewImagingSuperSessionsGrid()
        {
            this.InitializeComponent();

            _singleViewKeyBindings = new Dictionary<Key, KeyboardShortcutDelegate>()
            {
                { Key.NumPad1,  new KeyboardShortcutDelegate( IncreaseLowerThreshold ) },
                { Key.NumPad2,  new KeyboardShortcutDelegate( DecreaseUpperThreshold ) },
                { Key.NumPad3,  new KeyboardShortcutDelegate( IncreaseUpperThreshold ) },
                { Key.NumPad5,  new KeyboardShortcutDelegate( ToggleTransparency ) },
                { Key.NumPad6,  new KeyboardShortcutDelegate( ToggleLogScale ) },
                { Key.NumPad7,  new KeyboardShortcutDelegate( CycleImageLayers ) },
                { Key.NumPad8,  new KeyboardShortcutDelegate( DecreaseSoS ) },
                { Key.NumPad9,  new KeyboardShortcutDelegate( IncreaseSoS ) },
                { Key.Decimal,  new KeyboardShortcutDelegate( TagImage ) },
				//{ Key.Divide,  new KeyboardShortcutDelegate( ResetImageProperties ) },
				{ Key.Multiply,  new KeyboardShortcutDelegate( DecreaseLowerThreshold ) },
                { Key.Subtract,  new KeyboardShortcutDelegate( ToggleLayerVisibility ) },
                { Key.PageUp,  new KeyboardShortcutDelegate( ZoomIn ) },
                { Key.PageDown,  new KeyboardShortcutDelegate( ZoomOut ) },
            };

            _multiViewKeyBindings = new Dictionary<Key, KeyboardShortcutDelegate>()
            {
                { Key.PageUp,  new KeyboardShortcutDelegate( ZoomIn ) },
                { Key.PageDown,  new KeyboardShortcutDelegate( ZoomOut ) },
            };

            ((INotifyCollectionChanged)imagingSessionsItemsControl.Items).CollectionChanged += ViewImagingSuperSessionsGrid_CollectionChanged;
            _mouseClickInsideImagePropertiesMenusBehavior = new MouseClickInsideOfControlBehavior(HandleClickInsideImagePropertiesMenus, comparisonMenuStackPanel);
            _mouseClickInsideImagePropertiesMenusBehavior.RegisterElement();
        }

        private void HandleClickInsideImagePropertiesMenus()
        {
            try
            {
                ViewRulerControlToggleButtonBase activeRuler = ((SuperSessionViewingLayout)ComparisonViewingLayout == SuperSessionViewingLayout.Layout1 ? (imagePropertiesMenus.ActiveSessionMenus as ViewImagePropertiesGridMenus).rulerToggleButton as ViewRulerControlToggleButtonBase : (imagePropertiesMenus.ActiveSessionMenus as ViewImagePropertiesGridComparisonMenus).rulerToggleButton as ViewRulerControlToggleButtonBase);
                if (!activeRuler.IsMouseOver && activeRuler.IsChecked)
                    activeRuler.CancelRulerDrawing();
            }
            catch (Exception ex)
            {
                ViewMSOTcSystem.LogMessage(Xvue.MSOT.Services.Log.EnumLogType.Warning, "ViewImagingSuperSessionsGrid.HandleClickInsideImagePropertiesMenus()", ex.Message);
            }
        }

        #region KeyboardShortcutDelegates
        private void ZoomOut()
        {
            _affectedImagingSession.MainImageProperties.ZoomInfo -= _affectedImagingSession.MainImageProperties.ZoomInfo / 100;
            _affectedImagingSession.RaiseShortcutDrivenMainImagePropertiesChangedEvent();
        }

        private void ZoomIn()
        {
            _affectedImagingSession.MainImageProperties.ZoomInfo += _affectedImagingSession.MainImageProperties.ZoomInfo / 100;
            _affectedImagingSession.RaiseShortcutDrivenMainImagePropertiesChangedEvent();
        }

        private void ToggleLayerVisibility()
        {
            _affectedImagingSession.MainImageProperties.ToggleSelectedLayerVisibility();
            _affectedImagingSession.RaiseShortcutDrivenMainImagePropertiesChangedEvent();
        }

        private void DecreaseLowerThreshold()
        {
            _affectedImagingSession.MainImageProperties.DecreaseSelectedLayerLowerThreshold();
            _affectedImagingSession.RaiseShortcutDrivenMainImagePropertiesChangedEvent();
        }

        private void ResetImageProperties()
        {
            _affectedImagingSession.RaiseShortcutDrivenEvent();
        }

        private void TagImage()
        {
            if (_affectedImagingSession.CommandToggleIsImageTagged.CanExecute(null))
            {
                _affectedImagingSession.CommandToggleIsImageTagged.Execute(null);
                _affectedImagingSession.RaiseShortcutDrivenEvent();
            }
        }

        private void IncreaseSoS()
        {
            _affectedImagingSession.ReconPreset.SpeedOfSoundOffset += _model.MSOTService.IReconstructionService.SpeedOfSoundRoundingStep;
            _affectedImagingSession.RaiseReconPresetShortcutDrivenPropertyChangedEvent("ShortcutDrivenSpeedOfSoundChangedEvent");
        }

        private void DecreaseSoS()
        {
            _affectedImagingSession.ReconPreset.SpeedOfSoundOffset -= _model.MSOTService.IReconstructionService.SpeedOfSoundRoundingStep;
            _affectedImagingSession.RaiseReconPresetShortcutDrivenPropertyChangedEvent("ShortcutDrivenSpeedOfSoundChangedEvent");
        }

        private void CycleImageLayers()
        {
            _affectedImagingSession.MainImageProperties.CycleComponents();
            _affectedImagingSession.RaiseShortcutDrivenMainImagePropertiesChangedEvent();
        }

        private void ToggleLogScale()
        {
            _affectedImagingSession.MainImageProperties.ToggleSelectedLayerLogarithmicMode();
            _affectedImagingSession.RaiseShortcutDrivenMainImagePropertiesChangedEvent();
        }

        private void ToggleTransparency()
        {
            _affectedImagingSession.MainImageProperties.ToggleSelectedLayerTransparency();
            _affectedImagingSession.RaiseShortcutDrivenMainImagePropertiesChangedEvent();
        }

        private void IncreaseUpperThreshold()
        {
            _affectedImagingSession.MainImageProperties.IncreaseSelectedLayerUpperThreshold();
            _affectedImagingSession.RaiseShortcutDrivenMainImagePropertiesChangedEvent();
        }

        private void DecreaseUpperThreshold()
        {
            _affectedImagingSession.MainImageProperties.DecreaseSelectedLayerUpperThreshold();
            _affectedImagingSession.RaiseShortcutDrivenMainImagePropertiesChangedEvent();
        }

        private void IncreaseLowerThreshold()
        {
            _affectedImagingSession.MainImageProperties.IncreaseSelectedLayerLowerThreshold();
            _affectedImagingSession.RaiseShortcutDrivenMainImagePropertiesChangedEvent();
        }
        #endregion KeyboardShortcutDelegates

        void ViewImagingSuperSessionsGrid_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            refreshGrid();
        }

        public object ComparisonViewingLayout
        {
            get { return (object)GetValue(ComparisonViewingLayoutProperty); }
            set { SetValue(ComparisonViewingLayoutProperty, value); }
        }

        public static readonly DependencyProperty ComparisonViewingLayoutProperty =
            DependencyProperty.Register(
            "ComparisonViewingLayout",
            typeof(object),
            typeof(ViewImagingSuperSessionsGrid),
            new FrameworkPropertyMetadata(
                new PropertyChangedCallback(ComparisonViewingLayoutChanged)));

        private static void ComparisonViewingLayoutChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                return;

            ViewImagingSuperSessionsGrid control = source as ViewImagingSuperSessionsGrid;
            control.refreshGrid();
        }

        public ICommand CreateComparisonAnalysis
        {
            get { return (ICommand)GetValue(CreateComparisonAnalysisProperty); }
            set { SetValue(CreateComparisonAnalysisProperty, value); }
        }

        public static readonly DependencyProperty CreateComparisonAnalysisProperty =
            DependencyProperty.Register(
            "CreateComparisonAnalysis",
            typeof(ICommand),
            typeof(ViewImagingSuperSessionsGrid),
            new FrameworkPropertyMetadata());

        public WrapGridDimensions WrapGridDimensions
        {
            get { return (WrapGridDimensions)GetValue(WrapGridDimensionsProperty); }
            set { SetValue(WrapGridDimensionsProperty, value); }
        }

        public static readonly DependencyProperty WrapGridDimensionsProperty =
            DependencyProperty.Register(
          "WrapGridDimensions",
          typeof(WrapGridDimensions),
          typeof(ViewImagingSuperSessionsGrid),
          new FrameworkPropertyMetadata(
              new WrapGridDimensions(1, 1, 10, 10)));

        private void imagingSessionsItemsControl_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            refreshGrid();
        }

        private void viewingLayoutSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            refreshGrid();
        }

        private void refreshGrid()
        {
            if (DataContext == null)
                return;
            ViewModelImagingSuperSession model = DataContext as ViewModelImagingSuperSession;
            try
            {
                double containerHeight = imagingSessionsGrid.ActualHeight;
                double containerWidth = imagingSessionsGrid.ActualWidth - 10;
                int itemsCount = imagingSessionsItemsControl.Items.Count;

                int rows = 1;
                int columns = 1;
                double cellHeight = containerHeight;
                double cellWidth = containerWidth;

                if (model.ViewingLayout == Xvue.MSOT.DataModels.Plugins.ProjectManager.SuperSessionViewingLayout.Layout1)
                {
                    rows = itemsCount;
                }
                if (model.ViewingLayout == Xvue.MSOT.DataModels.Plugins.ProjectManager.SuperSessionViewingLayout.Layout2)
                {
                    columns = 2;
                    rows = (int)Math.Ceiling((double)itemsCount / 2);
                    cellWidth = (containerWidth - 20) / 2;
                }
                if (model.ViewingLayout == Xvue.MSOT.DataModels.Plugins.ProjectManager.SuperSessionViewingLayout.Layout4)
                {
                    columns = 2;
                    rows = (int)Math.Ceiling((double)itemsCount / 2);
                    cellWidth = (containerWidth - 20) / 2;
                    if (itemsCount > 2)
                        cellHeight = containerHeight / 2;
                }
                if (model.ViewingLayout == Xvue.MSOT.DataModels.Plugins.ProjectManager.SuperSessionViewingLayout.Layout6)
                {
                    columns = 3;
                    rows = (int)Math.Ceiling((double)itemsCount / 3);
                    cellWidth = (containerWidth - 20) / 3;
                    if (itemsCount > 3)
                        cellHeight = containerHeight / 2;
                }

                SetCurrentValue(WrapGridDimensionsProperty, new WrapGridDimensions(rows, columns, cellHeight, cellWidth));
            }
            catch { }
        }

        ViewModelImagingSessionBase _affectedImagingSession = null;
        internal void HandlePreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (_model == null)
                return;

            try
            {
                if (e.Key == Key.Return)
                {
                    //Use refrection to get IxExtendedKey
                    //For Numpad Enter Key, IsExtendedKey = True
                    //For Main Keyboard Enter Key, IsExtendedKey = False
                    bool isExtended = (bool)typeof(KeyEventArgs).InvokeMember("IsExtendedKey", System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, null, e, null);
                    if (!isExtended)
                    {
                        return;
                    }
                }

                Dictionary<Key, KeyboardShortcutDelegate> keyBindings;
                _affectedImagingSession = null;
                if (_model.ViewingLayout == SuperSessionViewingLayout.Layout1)
                {
                    foreach (ViewModelImagingSessionBase session in _model.ImagingSessions)
                    {
                        if (session.IsVisible)
                        {
                            _affectedImagingSession = session;
                            break;
                        }
                    }
                    keyBindings = _singleViewKeyBindings;
                }
                else
                {
                    for (int i = 0; i < imagingSessionsItemsControl.Items.Count; i++)
                    {
                        UIElement uiElement = (UIElement)imagingSessionsItemsControl.ItemContainerGenerator.ContainerFromIndex(i);
                        if (uiElement.IsMouseOver)
                        {
                            _affectedImagingSession = (uiElement as FrameworkElement).DataContext as ViewModelImagingSessionBase;
                        }
                    }
                    keyBindings = _multiViewKeyBindings;
                }

                if (_affectedImagingSession?.IsEmptyImagingSession ?? true)
                {
                    return;
                }

                if (_affectedImagingSession.IsVisualizationRefreshing)
                    return;

                ViewMSOTcSystem.LogMessage(Xvue.MSOT.Services.Log.EnumLogType.Info, "ViewImagingSuperSessionsGrid:HandlePreviewKeyDown", "Key: " + e.Key + ", SystemKey: " + e.SystemKey);
                KeyboardShortcutDelegate command;
                Key incomingKey;
                if (e.Key == Key.System)
                    incomingKey = e.SystemKey;
                else
                    incomingKey = e.Key;
                if (System.Windows.Forms.Control.IsKeyLocked(System.Windows.Forms.Keys.NumLock)) //show num lock status somewhere?
                {
                    if (keyBindings.TryGetValue(incomingKey, out command))
                    {
                        command();
                        e.Handled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ViewMSOTcSystem.LogMessage(Xvue.MSOT.Services.Log.EnumLogType.Warning, "ViewImagingSuperSessionsGrid", "Exception executing shortcut:" + ex.Message);
            }
        }

        private void imagingSessionsItemsControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            refreshGrid();
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer scrollViewer = sender as ScrollViewer;
            if (scrollViewer == null)
                return;

            if (WrapGridDimensions.RowHeight == 0)
                return;

            if (e.VerticalOffset % WrapGridDimensions.RowHeight != 0)
            {
                double newVerticalChange = e.VerticalChange > 0 ? WrapGridDimensions.RowHeight : -WrapGridDimensions.RowHeight;
                double multiplier = Math.Abs(Math.Floor(e.VerticalChange / WrapGridDimensions.RowHeight));
                if (multiplier == 0)
                    multiplier = 1;
                double correctedOffset = (e.VerticalOffset - e.VerticalChange) + newVerticalChange * multiplier;
                scrollViewer.ScrollToVerticalOffset(correctedOffset);
            }
            else
            {
                for (int i = 0; i < imagingSessionsItemsControl.Items.Count; i++)
                {
                    // position of your visual inside the scrollviewer    
                    GeneralTransform childTransform = (imagingSessionsItemsControl.ItemContainerGenerator.ContainerFromIndex(i) as ContentPresenter).TransformToAncestor(scrollViewer);
                    Rect rectangle = childTransform.TransformBounds(new Rect(new Point(0, 0), (imagingSessionsItemsControl.ItemContainerGenerator.ContainerFromIndex(i) as ContentPresenter).RenderSize));

                    Rect rectangleScrollViewer = new Rect(new Point(0, 0), scrollViewer.RenderSize);
                    rectangleScrollViewer.Height = rectangleScrollViewer.Height - 10;
                    rectangleScrollViewer.Y = rectangleScrollViewer.Y + 5;
                    //Check if the elements Rect intersects with that of the scrollviewer's
                    Rect result = Rect.Intersect(rectangleScrollViewer, rectangle);

                    //if result is Empty then the element is not in view
                    if (result == Rect.Empty)
                    {
                        (imagingSessionsItemsControl.Items[i] as ViewModelImagingSessionBase).IsVisible = false;
                    }
                    else
                    {
                        (imagingSessionsItemsControl.Items[i] as ViewModelImagingSessionBase).IsVisible = true;
                    }
                }
            }
        }

        private void layoutMenuItem_Checked(object sender, RoutedEventArgs e)
        {
            refreshGrid();
        }

        private void AddReplaceSession_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("snapshotDragItem"))
            {
                ViewImagingSessionGeneric item = sender as ViewImagingSessionGeneric;
                ViewModelImagingSessionBase substituteImagingSession = item.DataContext as ViewModelImagingSessionBase;
                ViewModelImagingSuperSession ss = substituteImagingSession.Parent as ViewModelImagingSuperSession;

                ViewModelImagingSuperSession genericVM = (ViewModelImagingSuperSession)e.Data.GetData("snapshotDragItem");

                ss.MSOTService.UIStaticDispatcher.BeginInvoke(new Action(() =>
                {
                    ss.ReplaceNewImagingSessionCopyThread(substituteImagingSession, genericVM.ImagingSessions[0], true);
                }));

                item.SessionBorder.SetCurrentValue(BorderBrushProperty, new SolidColorBrush(Colors.White));
                e.Handled = true;
            }

        }
        private void SessionGeneric_DragEnter(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent("snapshotDragItem"))
                {
                    ViewImagingSessionGeneric control = sender as ViewImagingSessionGeneric;
                    control.SessionBorder.SetCurrentValue(BorderBrushProperty, new SolidColorBrush(Colors.Cyan));
                }
            }
            catch { }
        }
        private void SessionGeneric_DragLeave(object sender, DragEventArgs e)
        {
            try
            {
                ViewImagingSessionGeneric control = sender as ViewImagingSessionGeneric;
                control.SessionBorder.SetCurrentValue(BorderBrushProperty, new SolidColorBrush(Colors.White));
            }
            catch { }
        }

        private void AddNewSession_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("snapshotDragItem"))
            {
                ViewModelImagingSuperSession genericVM = (ViewModelImagingSuperSession)e.Data.GetData("snapshotDragItem");

                ScrollViewer item = sender as ScrollViewer;
                ViewModelImagingSuperSession ss = item.DataContext as ViewModelImagingSuperSession;
                ss.MSOTService.UIStaticDispatcher.BeginInvoke(new Action(() =>
                {
                    ss.AddNewImagingSessionCopy(genericVM.ImagingSessions[0], true);
                }));
                e.Handled = true;
            }
        }

        private void comparisonSessionGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_model == null)
                return;

            ViewModelImagingSessionBase session = ((Grid)sender).DataContext as ViewModelImagingSessionBase;
            if (_model.CommandCopyPropertiesToAll.CanExecute(session))
            {
                _model.CommandCopyPropertiesToAll.Execute(session);
                floatingToolTip.IsOpen = _model.ShowCopyPropertiesMouseToolTip;
            }
            else if (_model.CommandPrepareCopyProperties.CanExecute(session))
            {
                _model.CommandPrepareCopyProperties.Execute(session);
                floatingToolTip.IsOpen = _model.ShowCopyPropertiesMouseToolTip;
            }
            else if (_model.CommandFinishCopyProperties.CanExecute(session))
            {
                _model.CommandFinishCopyProperties.Execute(session);
                floatingToolTip.IsOpen = _model.ShowCopyPropertiesMouseToolTip;
            }
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _model = e.NewValue as ViewModelImagingSuperSession;

            Visibility = _model == null ? Visibility.Collapsed : Visibility.Visible;

        }

        private void comparisonSessionGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            floatingToolTip.PlacementTarget = sender as FrameworkElement;
            if (_model.ShowCopyPropertiesMouseToolTip)
            {
                floatingToolTip.IsOpen = true;
            }
        }

        private void comparisonSessionGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            floatingToolTip.IsOpen = false;
        }

        private void comparisonSessionGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (_model.ShowCopyPropertiesMouseToolTip)
            {
                Point currentPos = e.GetPosition(sender as FrameworkElement);
                floatingToolTip.HorizontalOffset = currentPos.X + 20;
                floatingToolTip.VerticalOffset = currentPos.Y + 20;
                if (!floatingToolTip.IsOpen)
                {
                    floatingToolTip.IsOpen = true;
                }
            }
            else
            {
                if (floatingToolTip.IsOpen)
                    floatingToolTip.IsOpen = false;
            }
        }
    }
}