using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Xvue.Framework.Views.WPF.Behaviors
{
    public class ListBoxKeyboardNavigationBehavior : BehaviorsHelper
    {
        Delegate _previewKeyDownEventDelegate;
        ListBox _listBox;

        // Default contructor required for XAML usage
        public ListBoxKeyboardNavigationBehavior() { }

        protected override void OnAttached()
        {
            base.OnAttached();
            _previewKeyDownEventDelegate = new KeyEventHandler(AssociatedObject_previewKeyDown);
            _listBox = AssociatedObject as ListBox;
            _listBox.AddHandler(Keyboard.PreviewKeyDownEvent, _previewKeyDownEventDelegate);
        }

        protected override void OnDetaching()
        {
            _listBox.RemoveHandler(Keyboard.PreviewKeyDownEvent, _previewKeyDownEventDelegate);
            _listBox = null;
            _previewKeyDownEventDelegate = null;
        }

        private void slicesGridArrangemenSelectItem(int index)
        {
            ListBoxItem listBoxItem = (ListBoxItem)_listBox.ItemContainerGenerator.ContainerFromItem(_listBox.Items.GetItemAt(index));
            listBoxItem.IsSelected = true;
            listBoxItem.Focus();
        }

        private int calculateRowOffsetIndex(int index, int rowOffset)
        {
            ListBoxItem listBoxItem = (ListBoxItem)_listBox.ItemContainerGenerator.ContainerFromItem(_listBox.Items.GetItemAt(index));
            int panelCols = (int)(_listBox.ActualWidth / listBoxItem.ActualWidth);
            int panelRows = _listBox.Items.Count / panelCols;
            if (_listBox.Items.Count % panelCols > 0)
                panelRows++;
            int indexRow = (index + 1) / panelCols;
            if ((index + 1) % panelCols > 0)
                indexRow++;
            int indexCol = (index % panelCols) + 1;
            int targetRow = indexRow - 1 + rowOffset;
            if (targetRow < 0)
            {
                targetRow = panelRows + targetRow;
            }
            else if (targetRow >= panelRows)
            {
                targetRow = targetRow - panelRows;
            }
            int targetIndex = targetRow * panelCols + indexCol - 1;
            if (targetIndex >= _listBox.Items.Count)
            {
                if (rowOffset < 0)
                    targetRow--;
                else
                    targetRow = 0;
                targetIndex = targetRow * panelCols + indexCol - 1;
            }
            return targetIndex;
        }

        // Currently, the following method produces the correct result only when the slicesGridArrangement WrapPanel Orientation is "Horizontal".
        private void AssociatedObject_previewKeyDown(object sender, KeyEventArgs e)
        {
            int i = _listBox.SelectedIndex;
            e.Handled = true;
            switch (e.Key)
            {
                case Key.Right:
                    slicesGridArrangemenSelectItem(i + 1 == _listBox.Items.Count ? 0 : i + 1);
                    break;
                case Key.Left:
                    slicesGridArrangemenSelectItem(i - 1 < 0 ? _listBox.Items.Count - 1 : i - 1);
                    break;
                case Key.Up:
                    slicesGridArrangemenSelectItem(calculateRowOffsetIndex(i, -1));
                    break;
                case Key.Down:
                    slicesGridArrangemenSelectItem(calculateRowOffsetIndex(i, 1));
                    break;
                default:
                    e.Handled = false;
                    break;
            }
        }
    }
}
