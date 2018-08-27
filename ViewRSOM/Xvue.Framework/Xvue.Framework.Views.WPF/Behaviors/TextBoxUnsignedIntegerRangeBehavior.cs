using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Xvue.Framework.Views.WPF.Behaviors
{
    public class TextBoxUnsignedIntegerRangeBehavior : TextBoxAllowableCharactersBehavior
    {

        public static readonly DependencyProperty MinimumProperty =
             DependencyProperty.Register(
                "Minimum",
                typeof(uint),
                typeof(TextBoxAllowableCharactersBehavior),
                new FrameworkPropertyMetadata((uint)0)
                );

        public uint Minimum
        {
            get { return (uint)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public static readonly DependencyProperty MaximumProperty =
             DependencyProperty.Register(
                "Maximum",
                typeof(uint),
                typeof(TextBoxAllowableCharactersBehavior),
                new FrameworkPropertyMetadata(uint.MaxValue)
                );

        public uint Maximum
        {
            get { return (uint)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public TextBoxUnsignedIntegerRangeBehavior() {
            SetCurrentValue(TextBoxUnsignedIntegerRangeBehavior.RegularExpressionProperty, "^[0-9]+$");
        }

        public TextBoxUnsignedIntegerRangeBehavior(DependencyObject obj) : base(obj)
        {
            SetCurrentValue(TextBoxUnsignedIntegerRangeBehavior.RegularExpressionProperty, "^[0-9]+$");
        }

        public override bool IsValid(string newText)
        {
            if (base.IsValid(newText))
            {
                uint v = uint.Parse(newText);
                return (v >= Minimum && v <= Maximum);
            }
            else
                return false;
        }

    }
}
