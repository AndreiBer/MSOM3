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

namespace Xvue.Framework.Views.WPF.Controls
{
    //Current implementation causes framework exception when WrapGrid is used as a templated parent and only one child is present.
    public class WrapGrid : Grid
    {
        //WrapGrid()
        //{
        //    DefaultStyleKeyProperty.OverrideMetadata(typeof(WrapGrid), new FrameworkPropertyMetadata(typeof(WrapGrid)));            
        //}

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
            reArrangItems();
        }

        void updateGridDimensions()
        {
            if (ColumnWidth < 0 || RowHeight < 0)
                return;

            ColumnDefinitions.Clear();
            RowDefinitions.Clear();
            for (int i = 0; i < ColumnCount; i++)            
                ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(ColumnWidth) });
            
            for(int i=0; i<RowCount; i++)
                RowDefinitions.Add(new RowDefinition() { Height = new GridLength(RowHeight) });
            System.Threading.Thread.Sleep(20);
            reArrangItems();
        }

        void reArrangItems()
        {
            try
            {
                //int rowCount = RowCount;
                int columCount = ColumnCount;
                int rowIndex = 0;
                int columIndex = 0;
                if (Children != null)
                {
                    foreach (UIElement child in Children)
                    {
                        if (child != null)
                        {
                            Grid.SetColumn(child, columIndex);
                            Grid.SetRow(child, rowIndex);

                            if (++columIndex >= columCount)
                            {
                                rowIndex++;
                                columIndex = 0;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception in WrapGrid: " + ex.Message);
            }
        }

        public int RowCount
        {
            get { return (int)GetValue(RowCountProperty); }
            set { SetValue(RowCountProperty, value); }
        }

        public static readonly DependencyProperty RowCountProperty =
            DependencyProperty.Register(
          "RowCount",
          typeof(int),
          typeof(WrapGrid),
          new FrameworkPropertyMetadata(
            new PropertyChangedCallback(ChangeRowCount)));

        private static void ChangeRowCount(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                (source as WrapGrid).updateGridDimensions();
            }
            catch
            {
            }
        }

        public double RowHeight
        {
            get { return (double)GetValue(RowHeightProperty); }
            set { SetValue(RowHeightProperty, value); }
        }

        public static readonly DependencyProperty RowHeightProperty =
            DependencyProperty.Register(
          "RowHeight",
          typeof(double),
          typeof(WrapGrid),
          new FrameworkPropertyMetadata(
            new PropertyChangedCallback(ChangeRowHeight)));

        private static void ChangeRowHeight(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                (source as WrapGrid).updateGridDimensions();
            }
            catch
            {
            }
        }

        public int ColumnCount
        {
            get { return (int)GetValue(ColumnCountProperty); }
            set { SetValue(ColumnCountProperty, value); }
        }

        public static readonly DependencyProperty ColumnCountProperty =
            DependencyProperty.Register(
          "ColumnCount",
          typeof(int),
          typeof(WrapGrid),
          new FrameworkPropertyMetadata(
            new PropertyChangedCallback(ChangeColumnCount)));

        private static void ChangeColumnCount(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                (source as WrapGrid).updateGridDimensions();
            }
            catch
            {
            }
        }

        public double ColumnWidth
        {
            get { return (double)GetValue(ColumnWidthProperty); }
            set { SetValue(ColumnWidthProperty, value); }
        }

        public static readonly DependencyProperty ColumnWidthProperty =
            DependencyProperty.Register(
          "ColumnWidth",
          typeof(double),
          typeof(WrapGrid),
          new FrameworkPropertyMetadata(
            new PropertyChangedCallback(ChangeColumnWidth)));

        private static void ChangeColumnWidth(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                (source as WrapGrid).updateGridDimensions();
            }
            catch
            {
            }
        }

    }
}
