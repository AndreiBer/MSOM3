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
    public struct WrapGridDimensions
    {
        public WrapGridDimensions(int rowCount, int columnCount, double rowHeight, double columnWidth)
        {
            RowCount = rowCount;
            ColumnCount = columnCount;
            RowHeight = rowHeight;
            ColumnWidth = columnWidth;
        }

        public int RowCount;
        public int ColumnCount;
        public double RowHeight;
        public double ColumnWidth;

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + RowCount;
            hash = (hash * 7) + ColumnCount;
            hash = (hash * 7) + RowHeight.GetHashCode();
            hash = (hash * 7) + ColumnWidth.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType() != typeof(WrapGridDimensions))
            {
                return false;
            }

            WrapGridDimensions otherObject = (WrapGridDimensions)obj;

            if (otherObject.RowCount == RowCount &&
                otherObject.ColumnCount == ColumnCount &&
                otherObject.RowHeight == RowHeight &&
                otherObject.ColumnWidth == ColumnWidth)
                return true;
            else
                return false;

        }

        public static bool operator ==(WrapGridDimensions objA, WrapGridDimensions objB)
        {
            bool aNull = ReferenceEquals(objA, null);
            bool bNull = ReferenceEquals(objB, null);
            if (aNull ^ bNull)
                return false;
            else if (aNull && bNull)
                return true;
            else
                return objA.Equals(objB);
        }


        public static bool operator !=(WrapGridDimensions objA, WrapGridDimensions objB)
        {
            return !(objA == objB);
        }

    }

    public class WrapGridSimple : Grid
    {
        public WrapGridDimensions Dimensions
        {
            get { return (WrapGridDimensions)GetValue(DimensionsProperty); }
            set { SetValue(DimensionsProperty, value); }
        }

        public static readonly DependencyProperty DimensionsProperty =
            DependencyProperty.Register(
          "Dimensions",
          typeof(WrapGridDimensions),
          typeof(WrapGridSimple),
          new FrameworkPropertyMetadata(
              new WrapGridDimensions(1, 1, 10, 10),
              new PropertyChangedCallback(ChangeDimensions)));

        private static void ChangeDimensions(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                (source as WrapGridSimple).updateGridDimensions();
            }
            catch
            {
            }
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
            reArrangItems();
        }

        void updateGridDimensions()
        {
            if (Dimensions.ColumnWidth < 0 || Dimensions.RowHeight < 0)
                return;

            ColumnDefinitions.Clear();
            RowDefinitions.Clear();

            int columnCount = Dimensions.ColumnCount > 0 ? Dimensions.ColumnCount : 1;
            int rowCount = Dimensions.RowCount > 0 ? Dimensions.RowCount : 1;

            for (int i = 0; i < columnCount; i++)
                ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(Dimensions.ColumnWidth) });

            for (int i = 0; i < rowCount; i++)
                RowDefinitions.Add(new RowDefinition() { Height = new GridLength(Dimensions.RowHeight) });

            reArrangItems();
        }

        void reArrangItems()
        {
            try
            {
                int columnCount = Dimensions.ColumnCount > 0 ? Dimensions.ColumnCount : 1;
                int rowCount = Dimensions.RowCount > 0 ? Dimensions.RowCount : 1;
                int maxElements = columnCount * rowCount;

                int rowIndex = 0;
                int columIndex = 0;
                int childAdded = 0;
                if (Children != null)
                {
                    foreach (UIElement child in Children)
                    {
                        if (child != null)
                        {
                            Grid.SetColumn(child, columIndex);
                            Grid.SetRow(child, rowIndex);

                            if (++columIndex >= columnCount)
                            {
                                rowIndex++;
                                columIndex = 0;
                            }
                            childAdded++;
                        }

                        if (childAdded >= maxElements)
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception in WrapGridSimple: " + ex.Message);
            }
        }

    }
}
