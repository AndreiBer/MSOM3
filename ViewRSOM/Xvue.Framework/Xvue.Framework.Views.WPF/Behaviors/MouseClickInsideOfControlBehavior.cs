using System;
using System.Windows;
using System.Windows.Input;

namespace Xvue.Framework.Views.WPF.Behaviors
{
    public class MouseClickInsideOfControlBehavior : BehaviorsHelper
    {
        // The reference to the object HandleClickOutsideOfControl function
        private Action UIElementMouseDownEventHandlerProperty { get; set; }
        // Tunneling delegate
        Delegate _previewMouseDownEventDelegate;

        public MouseClickInsideOfControlBehavior(Action handler, DependencyObject obj) : base(obj)
        {
            UIElementMouseDownEventHandlerProperty = handler;
            _previewMouseDownEventDelegate = new MouseButtonEventHandler(AssociatedObject_PreviewMouseDownEvent);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            Mouse.Capture(AssociatedObject, CaptureMode.SubTree);
            AssociatedObject.AddHandler(Mouse.PreviewMouseDownEvent, _previewMouseDownEventDelegate, false);
        }

        private void AssociatedObject_PreviewMouseDownEvent(object sender, MouseButtonEventArgs e)
        {
            UIElementMouseDownEventHandlerProperty();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.RemoveHandler(Mouse.PreviewMouseDownEvent, _previewMouseDownEventDelegate);
            AssociatedObject.ReleaseMouseCapture();
            base.OnDetaching();
        }
    }
}
