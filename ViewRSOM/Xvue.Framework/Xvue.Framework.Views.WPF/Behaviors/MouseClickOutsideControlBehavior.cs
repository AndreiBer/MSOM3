using System;
using System.Windows;
using System.Windows.Input;

namespace Xvue.Framework.Views.WPF.Behaviors
{
    // The object HandleClickOutsideOfControl function signature
    
    public class MouseClickOutsideControlBehavior : BehaviorsHelper
    {
        // The reference to the object HandleClickOutsideOfControl function
        private Action UIElementMouseDownEventHandlerProperty { get; set; }
        // Tunneling delegate
        Delegate _previewMouseDownEventOutsideCapturedElementDelegate;
        // The UI input element that is capturing the mouse at any instance
        IInputElement _capturedFrom = null;

        public MouseClickOutsideControlBehavior(Action handler, DependencyObject obj) : base(obj)
        {
            UIElementMouseDownEventHandlerProperty = handler;
            _previewMouseDownEventOutsideCapturedElementDelegate = new MouseButtonEventHandler(AssociatedObject_PreviewMouseDownEvent);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            Mouse.Capture(AssociatedObject, CaptureMode.SubTree);
            AssociatedObject.AddHandler(Mouse.PreviewMouseDownOutsideCapturedElementEvent, _previewMouseDownEventOutsideCapturedElementDelegate, false);
            AssociatedObject.LostMouseCapture += AssociatedObjectSubTree_LostMouseCapture;
        }

        private void AssociatedObjectSubTree_LostMouseCapture(object sender, MouseEventArgs e)
        {
            // Because of WPF bubbling routing, we may land here when the AssociatedObject or any of its child elements loses the mouse capture.
            // The Mouse.Captured field holds a reference to the element that 'stole' the mouse capture (only one element at a time can have it).
            // If the Mouse.Captured is null, then it means that no element 'stole' the mouse capture, which is the case that, currently, no element is capturing the mouse. Thus, the AssociatedObject needs to re-capture it.
            _capturedFrom = Mouse.Captured;
            if (_capturedFrom != null)
            {
                _capturedFrom.LostMouseCapture += ExternalInputElement_LostMouseCaptureEvent;
            }
            else // Re-capture case
            {
                Mouse.Capture(AssociatedObject, CaptureMode.SubTree);
            }
        }


        private void ExternalInputElement_LostMouseCaptureEvent(object sender, MouseEventArgs e)
        {
            _capturedFrom.LostMouseCapture -= ExternalInputElement_LostMouseCaptureEvent;
        }

        private void AssociatedObject_PreviewMouseDownEvent(object sender, MouseButtonEventArgs e)
        {
            // This is currently a registered tunneling event handler (see above in method OnAttached) There are two scenarios regarding its execution:
            // a) It will be called BEFORE the MouseDownOutsideCapturedElement event is handled by the control that raised it, iff the control is an DESCENDANT of the associatedObject in the visual tree.
            // b) It will NOT be called, iff a) holds AND an ancestral (in the visual tree) handler sets e.Handled = true AND the AssociatedObject.AddHandler argument handledEventsToo = false.
            // For more info at WPF routed events see http://www.wpftutorial.net/RoutedEvents.html
            if (_capturedFrom == null)
            {
                UIElementMouseDownEventHandlerProperty();
            }
        }

        protected override void OnDetaching()
        {
            AssociatedObject.RemoveHandler(Mouse.PreviewMouseDownOutsideCapturedElementEvent, _previewMouseDownEventOutsideCapturedElementDelegate);
            if (_capturedFrom != null && _capturedFrom.IsMouseCaptured)
            {
                _capturedFrom.ReleaseMouseCapture();
            }
            AssociatedObject.LostMouseCapture -= AssociatedObjectSubTree_LostMouseCapture;
            if (AssociatedObject.IsMouseCaptured)
            {
                AssociatedObject.ReleaseMouseCapture();
            }
            base.OnDetaching();
        }

    }

}
