using System;
using System.Collections.Generic;
using System.Linq;
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

namespace ViewMSOTc
{
    /// <summary>
    /// Interaction logic for ViewOAMPresetsPopup.xaml
    /// </summary>
    public partial class ViewOAMPresetsPopup : UserControl
    {
        CollectionViewSource src;
        PropertyGroupDescription groupByType = new PropertyGroupDescription("Origin");
        public ViewOAMPresetsPopup()
        {
            InitializeComponent();
        }

        public bool CloseControl
        {
            get { return (bool)GetValue(CloseControlProperty); }
            set { SetValue(CloseControlProperty, value); }
        }

        public static readonly DependencyProperty CloseControlProperty =
            DependencyProperty.Register(
            "CloseControl",
            typeof(bool),
            typeof(ViewOAMPresetsPopup));

        public bool? UserReply
        {
            get { return (bool)GetValue(UserReplyProperty); }
            set { SetValue(UserReplyProperty, value); }
        }

        public static readonly DependencyProperty UserReplyProperty =
            DependencyProperty.Register(
            "UserReply",
            typeof(bool?),
            typeof(ViewOAMPresetsPopup), new PropertyMetadata(null));

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            UserReply = true;
            CloseControl = true;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            UserReply = null;
            CloseControl = true;
        }

        private void SelectCurrentItem(object sender, KeyboardFocusChangedEventArgs e)
        {
            ListBoxItem mysender = sender as ListBoxItem;
            if (mysender != null)
                mysender.IsSelected = true;
        }

        private void preset_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ListBoxItem item = sender as ListBoxItem;
            if (item != null)
            {
                if (okButton.Command.CanExecute(item.DataContext))
                {
                    okButton.Command.Execute(item.DataContext);
                    UserReply = true;
                    CloseControl = true;
                }
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            src = this.Resources["groupablePresetsSource"] as CollectionViewSource;
            refreshPresetsListViewsGroups();
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            refreshPresetsListViewsGroups();
        }

        private void refreshPresetsListViewsGroups()
        {
            if (src != null)
                using (src.DeferRefresh())
                {
                    src.GroupDescriptions.Clear();
                }
            if (src != null)
                using (src.DeferRefresh())
                {
                    src.GroupDescriptions.Add(groupByType);
                }

            if (factoryPresetsListBox != null)
            {
                factoryPresetsListBox.GroupStyle.Clear();
                factoryPresetsListBox.GroupStyle.Add(new GroupStyle() { ContainerStyle = (Style)this.FindResource("PresetsGroupStyle") });
            }

        }

        private void factoryPresetsListBox_ManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }

        private void factoryPresetsListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up || e.Key==Key.Down || e.Key == Key.Left || e.Key == Key.Right)
                e.Handled = true;
        }

        private void userControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                focusMainElement();
            }
        }

        private void focusMainElement()
        {
            FrameworkElement element = factoryPresetsListBox;

            int index = factoryPresetsListBox.SelectedIndex;
            if (index >= 0)
            {
                ListBoxItem lbi = factoryPresetsListBox.ItemContainerGenerator.ContainerFromIndex(index) as ListBoxItem;
                if (lbi != null)
                    element = lbi;
            }

            giveElementFocus(element);
        }

        private void giveElementFocus(FrameworkElement element)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Input, new Action(delegate()
            {
                element.Focus();
                Keyboard.Focus(element);
            }));
        }

    }
}
