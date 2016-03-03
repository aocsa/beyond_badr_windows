using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;

namespace MLearning.Store.MLReader.ManipulablePages
{
    public sealed partial class IconSlideBar : Grid
    {
        public IconSlideBar()
        {
            init();
        }

        Grid top_line, botton_line;
        Image icon_image;

        void init()
        {
            Height = 900.0;
            Width = 60.0;
            HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left;
            RenderTransform = new TranslateTransform() { X = 200};

            icon_image = new Image() { Width = 60, Height = 60 };
            Children.Add(icon_image);

            top_line = new Grid() { Width = 3, Height = 376, VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Top };
            Children.Add(top_line);

            botton_line = new Grid() { Width = 3, Height = 376, VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Bottom };
            Children.Add(botton_line);
        }

        private string _imageurl;

        public string ImageUrl
        {
            get { return _imageurl; }
            set
            {
                _imageurl = value;
                icon_image.Source = new BitmapImage(new Uri(_imageurl));
            }
        }


        private Color _linecolor;

        public Color LineColor
        {
            get { return _linecolor; }
            set
            {
                _linecolor = value;
                top_line.Background = new SolidColorBrush(_linecolor);
                botton_line.Background = new SolidColorBrush(_linecolor);
            }
        }

    }
}
