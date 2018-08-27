using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Xvue.Framework.Views.WPF.Controls
{
    /// <summary>
    /// A modified slider control that update its value only when the user stops dragging the slider.
    /// Useful in case the Value update takes considerable time. 
    /// </summary>
    public class SlowMovingSlider:Slider
    {
        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);
            //check and allow value changes only from +/- and track repeatButtons when mouse pressed (excluding so the thumb dragging)
            if (Mouse.Captured is System.Windows.Controls.Primitives.Thumb)
            {                
                return;
            }
            BindingExpression be = this.GetBindingExpression(Slider.ValueProperty);
            if (be != null)
            {
                be.UpdateSource();
            }
        }

        protected override void OnThumbDragCompleted(System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            base.OnThumbDragCompleted(e);
            BindingExpression be = this.GetBindingExpression(Slider.ValueProperty);
            if (be != null)
            {
                be.UpdateSource();
            }
        }
    }
}
