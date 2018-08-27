using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Xvue.Framework.Views.WPF.Controls
{
    /// <summary>
    /// Class DisabledContentWheelScrollViewer.
    /// Allow scroll using wheel only when mouse over the scroll bars.
    /// </summary>
    public class DisabledContentWheelScrollViewer : ScrollViewer
    {
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            ScrollBar sb;
            
            sb = Template.FindName("PART_VerticalScrollBar", this) as ScrollBar;
            if (sb != null)
            {
                if (sb.IsMouseOver)
                {
                    base.OnMouseWheel(e);
                    return;
                }
            }

            sb = Template.FindName("PART_HorizontalScrollBar", this) as ScrollBar;
            if (sb != null)
            {
                if (sb.IsMouseOver)
                {
                    base.OnMouseWheel(e);
                    return;
                }
            }
        }

    }
}
