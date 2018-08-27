using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Xvue.Framework.Views.WPF.Imaging
{
    public class ImagingHelper
    {
        public static RenderTargetBitmap ComposeImage(DataTemplate imageTemplate, object imageContent, out int xDimension, out int yDimension, double width = 0, double height = 0)
        {
            int _xDimension = 0;
            int _yDimension = 0;
            ContentControl element = new ContentControl { ContentTemplate = imageTemplate, Content = imageContent };
            // Measure and arrange the tile
            if (width != 0)
                element.Width = width;
            if (height != 0)
                element.Height = height;
            element.Measure(new Size { Height = double.PositiveInfinity, Width = double.PositiveInfinity });
            _xDimension = (int)element.DesiredSize.Width;
            _yDimension = (int)element.DesiredSize.Height;
            element.Arrange(new Rect(0, 0, element.DesiredSize.Width, element.DesiredSize.Height));
            element.UpdateLayout();
            if (_xDimension % 2 == 1) _xDimension++;
            if (_yDimension % 2 == 1) _yDimension++;
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap((Int32)_xDimension, (Int32)_yDimension, 96d, 96d, PixelFormats.Pbgra32);
            renderBitmap.Render(element);
            xDimension = _xDimension;
            yDimension = _yDimension;
            return renderBitmap;
        }
    }
}
