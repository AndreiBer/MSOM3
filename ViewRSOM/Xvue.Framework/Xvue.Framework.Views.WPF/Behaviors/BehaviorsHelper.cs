using System.Windows;
using System.Windows.Interactivity;

namespace Xvue.Framework.Views.WPF.Behaviors
{
    public class BehaviorsHelper : Behavior<UIElement>
    {
        private DependencyObject _associatedObject;

        // Default contructor required for XAML usage
        public BehaviorsHelper(){}

        public BehaviorsHelper(DependencyObject obj)
        {
            _associatedObject = obj;
        }

        public void RegisterElement()
        {
            Interaction.GetBehaviors(_associatedObject).Add(this);
        }

        public void UnregisterElement()
        {
            Interaction.GetBehaviors(_associatedObject).Remove(this);
        }

        public bool IsElementRegistered()
        {
            return Interaction.GetBehaviors(_associatedObject).IndexOf(this) >= 0;
        }
    }
}
