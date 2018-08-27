using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Xvue.Framework.Views.WPF.Behaviors
{
    public class TextBoxSelectAllBehavior : BehaviorsHelper
    {

        public TextBoxSelectAllBehavior(DependencyObject obj) : base(obj)
        { }

        public TextBoxSelectAllBehavior() { }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.GotMouseCapture += AssociatedObject_GotMouseCapture;
        }

        private void AssociatedObject_GotMouseCapture(object sender, MouseEventArgs e)
        {
            TextBox textBox = AssociatedObject as TextBox;
            if (textBox.SelectedText.Length == 0)
            {
                textBox.SelectAll();
            }
        }
 
        protected override void OnDetaching()
        {
            AssociatedObject.GotMouseCapture -= AssociatedObject_GotMouseCapture;
            base.OnDetaching();
        }
    }
}
