using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ViewMSOT.UIControls;
using Xvue.MSOT.Services.Imaging;
using Xvue.MSOT.ViewModels.ProjectManager;
using Xvue.MSOT.ViewModels.ProjectManager.ImagingSession;
using Xvue.MSOT.ViewModels.ProjectManager.MsotProject;

namespace ViewMSOTc
{
    /// <summary>
    /// Interaction logic for ViewImagingSessionGenericFull.xaml
    /// </summary>
    public partial class ViewImagingSessionGenericFull : UserControl
    {
        ViewModelImagingSessionBase _model;
        double _previousScrollOffset;

        public ViewImagingSessionGenericFull()
        {
            InitializeComponent();
            _previousScrollOffset = 0;
        }

        public DataTemplate ImageThumbDataTemplate
        {
            get { return (DataTemplate)GetValue(ImageThumbDataTemplateProperty); }
            set { SetValue(ImageThumbDataTemplateProperty, value); }
        }

        public static readonly DependencyProperty ImageThumbDataTemplateProperty =
            DependencyProperty.Register(
            "ImageThumbDataTemplate",
            typeof(DataTemplate),
            typeof(ViewImagingSessionGenericFull),
            new FrameworkPropertyMetadata());

        public double MainImageAreaMaxWidth
        {
            get { return (double)GetValue(MainImageAreaMaxWidthProperty); }
            set { SetValue(MainImageAreaMaxWidthProperty, value); }
        }

        public static readonly DependencyProperty MainImageAreaMaxWidthProperty =
            DependencyProperty.Register(
            "MainImageAreaMaxWidth",
            typeof(double),
            typeof(ViewImagingSessionGenericFull),
            new FrameworkPropertyMetadata());

        private void userControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _model = e.NewValue as ViewModelImagingSessionBase;

            string resourceString = "";
            if (e.NewValue != null)
            {
                if (e.NewValue.GetType() == typeof(ViewModelMsp3DImagingSession))
                    resourceString = "Msp3DImageThumbDataTemplate";
                else if (e.NewValue.GetType() == typeof(ViewModelMspImagingSession))
                    resourceString = "MspImageThumbDataTemplate";
            }

            if (string.IsNullOrWhiteSpace(resourceString))
                SetCurrentValue(ImageThumbDataTemplateProperty, null);
            else
                SetCurrentValue(ImageThumbDataTemplateProperty, Resources[resourceString]);
        }

        private void Border_DragEnter(object sender, System.Windows.DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent("snapshotDragItem"))
                {
                    Border control = sender as Border;
                    control.SetCurrentValue(BorderBrushProperty, new SolidColorBrush(Colors.Cyan));
                }
            }
            catch { }
        }

        private void Border_DragLeave(object sender, System.Windows.DragEventArgs e)
        {
            try
            {
                Border control = sender as Border;
                control.SetCurrentValue(BorderBrushProperty, new SolidColorBrush(Colors.White));
            }
            catch { }
        }

        private void EmptySuperSessionOrReplaceSession_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("snapshotDragItem"))
            {
                Border item = sender as Border;
                ViewModelImagingSessionBase substituteImagingSession = item.DataContext as ViewModelImagingSessionBase;
                ViewModelImagingSuperSession ss = substituteImagingSession.Parent as ViewModelImagingSuperSession;
                ViewModelImagingSuperSession genericVM = (ViewModelImagingSuperSession)e.Data.GetData("snapshotDragItem");
                if (ss.VisualizationAndAnalysisType == Xvue.MSOT.DataModels.Plugins.ProjectManager.VisualizationAndAnalysis.Visualization)
                {
                    ss.MSOTService.UIStaticDispatcher.BeginInvoke(new Action(() =>
                    {
                        ss.ReplaceNewImagingSessionCopyThread(substituteImagingSession, genericVM.ImagingSessions[0], true);
                    }));
                }
                else
                {
                    if (ss.VerifySavingOfDataModelChanges(true))
                    {
                        ss.MSOTService.UIStaticDispatcher.BeginInvoke(new Action(() =>
                        {
                            ViewModelStudyNode study = ss.Parent as ViewModelStudyNode;
                            if (study != null)
                            {
                                study.SelectedSuperSession = genericVM;
                                study.LoadSelectedSuperSession();
                            }
                        }));
                    }
                }
                item.SetCurrentValue(BorderBrushProperty, new SolidColorBrush(Colors.White));
                e.Handled = true;
            }

        }

        private void ListBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            if (_model != null)
                if (_model.PreviewVisibleImages)
                    return;

            (sender as FrameworkElement).BringIntoView();
        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            ContextMenu gridContextMenu = sender as ContextMenu;
            if (gridContextMenu.Visibility != System.Windows.Visibility.Visible)
                gridContextMenu.SetCurrentValue(ContextMenu.IsOpenProperty, false);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement menuItem = sender as FrameworkElement;
            if (menuItem.DataContext == null)
                return;

            IVisualization session = menuItem.DataContext as IVisualization;
            if (session == null)
                return;

            bool continueF = session.PrepareCreateComparisonFromSnapshot();

            if (continueF)
            {
                DateTime currentDateTime = DateTime.Now;
                string suggestedName = "Comparison_" + currentDateTime.ToShortDateString() + "_" + currentDateTime.ToShortTimeString();
                ModalChildWindow.ShowDialog(
                    "Save comparison as",
                    new ViewSaveAs("Comparison name", session.CommandNewComparisonSuperSession, suggestedName),
                    session);
            }
        }
        

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer scrollViewer = sender as ScrollViewer;
            if (scrollViewer == null)
                return;

            if (imageSizeSlider.Value == 0)
                return;
            bool isVisibilityChanged = false;
            
            for (int i = 0; i < slicesGridArrangement.Items.Count; i++)
            {
                // position of your visual inside the scrollviewer    
                GeneralTransform childTransform = (slicesGridArrangement.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem).TransformToAncestor(scrollViewer);
                Rect rectangle = childTransform.TransformBounds(new Rect(new Point(0, 0), (slicesGridArrangement.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem).RenderSize));

                Rect rectangleScrollViewer = new Rect(new Point(0, 0), scrollViewer.RenderSize);
                rectangleScrollViewer.Height = rectangleScrollViewer.Height - 10;
                rectangleScrollViewer.Y = rectangleScrollViewer.Y + 5;
                //Check if the elements Rect intersects with that of the scrollviewer's
                Rect result = Rect.Intersect(rectangleScrollViewer, rectangle);

                //if result is Empty then the element is not in view
                if (result == Rect.Empty)
                {
                    if ((slicesGridArrangement.Items[i] as ViewModelSliceInfo).IsVisible)
                        isVisibilityChanged = true;
                    (slicesGridArrangement.Items[i] as ViewModelSliceInfo).IsVisible = false;
                }
                else
                {
                    if (!(slicesGridArrangement.Items[i] as ViewModelSliceInfo).IsVisible)
                        isVisibilityChanged = true;
                    (slicesGridArrangement.Items[i] as ViewModelSliceInfo).IsVisible = true;
                }
            }
            if(isVisibilityChanged)
            {
                ViewModelImagingSessionBase imagingSession = this.DataContext as ViewModelImagingSessionBase;
                if (_previousScrollOffset < scrollViewer.ContentVerticalOffset)
                    imagingSession.ReorderImagesForProcessing(true);
                else
                    imagingSession.ReorderImagesForProcessing();

                _previousScrollOffset = scrollViewer.ContentVerticalOffset;
            }
            e.Handled = true;
        }

        private void slicesGridArrangement_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
          /*  EventHandler eventHandler = null;
            eventHandler = new EventHandler(delegate
            {
                if (slicesGridArrangement.ItemContainerGenerator.Status == System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
                {
                    for (int i = 0; i < slicesGridArrangement.Items.Count; i++)
                    {
                        // position of your visual inside the scrollviewer    
                        GeneralTransform childTransform = (slicesGridArrangement.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem).TransformToAncestor(slicesScrollViewer as ScrollViewer);
                        Rect rectangle = childTransform.TransformBounds(new Rect(new Point(0, 0), (slicesGridArrangement.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem).RenderSize));

                        Rect rectangleScrollViewer = new Rect(new Point(0, 0), (slicesScrollViewer as ScrollViewer).RenderSize);
                        rectangleScrollViewer.Height = rectangleScrollViewer.Height - 10;
                        rectangleScrollViewer.Y = rectangleScrollViewer.Y + 5;
                        //Check if the elements Rect intersects with that of the scrollviewer's
                        Rect result = Rect.Intersect(rectangleScrollViewer, rectangle);

                        //if result is Empty then the element is not in view
                        if (result == Rect.Empty)
                        {
                            (slicesGridArrangement.Items[i] as ViewModelSliceInfo).IsVisible = false;
                        }
                        else
                        {
                            (slicesGridArrangement.Items[i] as ViewModelSliceInfo).IsVisible = true;
                        }
                    }
                    ViewModelImagingSessionBase imagingSession = this.DataContext as ViewModelImagingSessionBase;
                    imagingSession.OrderImagesForProcessing();
                    slicesGridArrangement.ItemContainerGenerator.StatusChanged -= eventHandler;
                }
            });
            slicesGridArrangement.ItemContainerGenerator.StatusChanged += eventHandler;            
            */
        }

        private void mainGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                SetCurrentValue(MainImageAreaMaxWidthProperty, (sender as FrameworkElement).ActualWidth - (imageSizeSlider.ActualWidth + 35));
            }
            catch (Exception ex)
            {
                ViewMSOTcSystem.LogMessage(Xvue.MSOT.Services.Log.EnumLogType.Warning, "ViewImagingSessionGenericFull", "Exception setting MainImageAreaMaxWidthProperty: " + ex.Message);
            }
        }

    }      
}
