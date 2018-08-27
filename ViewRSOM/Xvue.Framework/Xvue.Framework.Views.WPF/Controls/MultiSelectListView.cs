using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;

namespace Xvue.Framework.Views.WPF.Controls
{
    public class MultiselectListView : ListView
    {
        private bool m_isSelectionActive;
        public bool IsSelectionActive
        {
            get
            {
                return m_isSelectionActive;
            }
            private set
            {
                m_isSelectionActive = value;
            }
        }
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new MultiselectListViewItem(this);
        }
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            
            if (SelectionMode != SelectionMode.Single)
            {
                bool leftCtrlActive = (System.Windows.Input.Keyboard.GetKeyStates(Key.LeftCtrl) & KeyStates.Down) > 0;
                bool rightCtrlActive = (System.Windows.Input.Keyboard.GetKeyStates(Key.RightCtrl) & KeyStates.Down) > 0;
                if ( !leftCtrlActive && !rightCtrlActive)
                {
                    SelectedItems.Clear();                    
                }
            }
            IsSelectionActive = true;
        }
        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            IsSelectionActive = false;
        }

        public MultiselectListView() : base()
        {
            SelectionMode = SelectionMode.Multiple;
        }
    }

    public class MultiselectListViewItem : ListViewItem
    {
        private readonly MultiselectListView m_parent;

        public MultiselectListViewItem(MultiselectListView parent)
        {
            m_parent = parent;
        }

        protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
        {
            if (m_parent.IsSelectionActive)
                IsSelected = true;
        }
    }
}
