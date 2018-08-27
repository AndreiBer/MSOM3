using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace Xvue.Framework.Views.WPF.Behaviors
{
    public class TextBoxAllowableCharactersBehavior : BehaviorsHelper
    {

        readonly DataObjectPastingEventHandler _dataObjectPastingEventDelegate;

        public static readonly DependencyProperty RegularExpressionProperty =
             DependencyProperty.Register(
                "RegularExpression", 
                typeof(string),
                typeof(TextBoxAllowableCharactersBehavior),
                new FrameworkPropertyMetadata(".*")
                );

        public string RegularExpression
        {
            get { return (string)GetValue(RegularExpressionProperty); }
            set { SetValue(RegularExpressionProperty, value); }
        }

        public TextBoxAllowableCharactersBehavior() {
            _dataObjectPastingEventDelegate = new DataObjectPastingEventHandler(OnPaste);
        }

        public TextBoxAllowableCharactersBehavior(DependencyObject obj) : base(obj)
        {
            _dataObjectPastingEventDelegate = new DataObjectPastingEventHandler(OnPaste);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewTextInput += AssociatedObject_PreviewTextInput;
            DataObject.AddPastingHandler(AssociatedObject, _dataObjectPastingEventDelegate);
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PreviewTextInput -= AssociatedObject_PreviewTextInput;
            DataObject.RemovePastingHandler(AssociatedObject, _dataObjectPastingEventDelegate);
            base.OnDetaching();
        }

        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(DataFormats.Text))
            {
                if (!IsValid(projectText(Convert.ToString(e.DataObject.GetData(DataFormats.Text)))))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private string projectText(string newText)
        {
            TextBox textBox = AssociatedObject as TextBox;
            string txt = textBox.Text;
            int ss = textBox.SelectionStart;
            int se = textBox.SelectionStart + textBox.SelectionLength;
            if (se > ss)
            { // i.e. there is some text selected
                return txt.Substring(0, ss) + newText + txt.Substring(se, txt.Count() - se);
            }
            else
            {
                return txt.Substring(0, textBox.CaretIndex) + newText + txt.Substring(textBox.CaretIndex, txt.Count() - textBox.CaretIndex);
            }
        }

        private void AssociatedObject_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !IsValid(projectText(e.Text));
        }

        public virtual bool IsValid(string newText)
        {
            return Regex.IsMatch(newText, RegularExpression);
        }
    }
}
