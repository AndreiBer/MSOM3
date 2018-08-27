using System;
using System.Collections.Generic;
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

namespace ViewMSOT.UIControls
{
	/// <summary>
	/// Interaction logic for ViewFrameRateControl.xaml
	/// </summary>
	public partial class ViewFrameRateControl : UserControl
	{        
		public ViewFrameRateControl()
		{
			this.InitializeComponent();          
		}       
            
		public ToggleButton PreviewVisibleImagesToggleButton
		{
			get {return previewVisibleImagesToggleButton;}
		}

        private void TextBox_KeyEnterUpdate(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox textBox = (TextBox)sender;
                DependencyProperty depProperty = TextBox.TextProperty;

                BindingExpression binding = BindingOperations.GetBindingExpression(textBox, depProperty);
                if (binding != null)
                {
                    if (textBox.Text.Length > 0)
                        binding.UpdateSource();
                    else {
                        binding.UpdateTarget();
                        binding.UpdateSource();
                    }
                }
            }
        }		      		
	}
}