using MLearning.Store.MLReader.ManipulablePages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace MLReader.ManipulablePages
{
    public sealed partial class RightAvatarSlide : Grid  , ISlideElement
    {
        public RightAvatarSlide()
        {
            Width = 1600.0;
            Height = 900.0;

            initbar();

            Image img = new Image()
            {
                Width = 566,
                Height = 746,
                RenderTransform = new TranslateTransform() { X = 900, Y = 154 },
                VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Top,
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left,
                Source = new BitmapImage(new Uri("ms-appx:///MLReader/rightimg.png"))
            };
            Children.Add(img);
            Background = new SolidColorBrush(Colors.Transparent);
            ManipulationMode = ManipulationModes.All;
        }


        IconSlideBar _iconbar;

        void initbar()
        {
            _iconbar = new IconSlideBar();
            Children.Add(_iconbar);
        }


        private LOSlideSource _source;

        public LOSlideSource Source
        {
            get { return _source; }
            set { _source = value; initcomponent_1(); }
        }


        void initcomponent_1()
        {
            if (_source.Type != 0)
            {
                if (_source.Style.ColorNumber != 0)
                    _iconbar.ImageUrl = "ms-appx:///Resources/ricons/estilo" + _source.Style.ID + "_color" + _source.Style.ColorNumber + "-0" + _source.Type + ".png";
                else
                    _iconbar.ImageUrl = "ms-appx:///Resources/ricons/tema5_colorblanco-0" + _source.Type + ".png";
                _iconbar.LineColor = _source.Style.TitleColor;
            }
        }


        public double GetSize()
        {
            return 1600.0;
        }

        double _position = 0;
        public double Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
            }
        }

        public event ISlideElementSizeChangedEventHandler ISlideElementSizeChanged;

    }
}
