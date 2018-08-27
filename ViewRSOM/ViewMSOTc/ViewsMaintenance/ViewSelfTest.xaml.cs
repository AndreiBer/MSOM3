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
using Xvue.Framework.Views.WPF.Converters;

namespace ViewMSOTc
{
    /// <summary>
    /// Interaction logic for ViewOAMLaserSelfTest.xaml
    /// </summary>
    public partial class ViewSelfTest : UserControl
    {
        public ViewSelfTest()
        {
            InitializeComponent();
        }

        private void userControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                (Parent as FrameworkElement).SetBinding(ModalChildWindow.WindowStateProperty,
                    new Binding("SelfTestOpen")
                    {   Source = e.NewValue,
                        Mode = BindingMode.OneWay,
                        Converter = new BooleanToWpfToolkitWindowStateConverter()
                    });
            }
        }
    }
}
