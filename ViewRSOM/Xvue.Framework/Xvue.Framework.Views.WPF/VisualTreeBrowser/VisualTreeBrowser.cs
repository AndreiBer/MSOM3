using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;

namespace Xvue.Framework.Views.WPF
{
    public static class VisualTreeBrowser
    {
        public static Visual GetDescendantByName(Visual element, string name)
        {
            if (element == null) return null;
            FrameworkElement elem = element as FrameworkElement;
            if (elem?.Name == name)
                return element;
            Visual result = null;
            if (elem != null)
                elem.ApplyTemplate();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                Visual visual = VisualTreeHelper.GetChild(element, i) as Visual;
                result = GetDescendantByName(visual, name);
                if (result != null)
                    break;
            }
            return result;
        }

        public static Visual GetDescendantByType(Visual element, Type type)
        {
            if (element == null)
                return null;
            if (element.GetType() == type)
                return element;
            Visual foundElement = null;
            FrameworkElement elem = element as FrameworkElement;
            if (elem != null)
                elem.ApplyTemplate();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                Visual visual = VisualTreeHelper.GetChild(element, i) as Visual;
                foundElement = GetDescendantByType(visual, type);
                if (foundElement != null)
                    break;
            }
            return foundElement;
        }

        public static DependencyObject GetAncestorByType(DependencyObject element, Type type)
        {
            if (element == null)
                return null;
            if (element.GetType() == type)
                return element;
            return GetAncestorByType(VisualTreeHelper.GetParent(element), type);
        }

        public static void EnumVisual(DependencyObject myVisual)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(myVisual); i++)
            {
                // Retrieve child visual at specified index value.
                Visual childVisual = (Visual)VisualTreeHelper.GetChild(myVisual, i);
                Console.WriteLine(childVisual.ToString());
                // Do processing of the child visual object. 
                // Enumerate children of the child visual object.
                EnumVisual(childVisual);
            }
        }
    }
}
