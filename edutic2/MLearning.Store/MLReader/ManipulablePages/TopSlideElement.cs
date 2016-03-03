using MLearning.Store.MLReader.ManipulablePages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace MLReader
{
    public sealed partial class TopSlideElement : Grid, ISlideElement
    {

        double DeviceHeight = 900.0, DeviceWidth = 1600.0;
        IconSlideBar _iconbar;

        public TopSlideElement()
        {
            init();
            Background = new SolidColorBrush(Colors.Transparent);
            //Opacity = 0.6;
            ManipulationMode = ManipulationModes.All;
        }

        public double GetSize()
        {
            return DeviceHeight;
        }

        public event ISlideElementSizeChangedEventHandler ISlideElementSizeChanged;

        private double _position;

        public double Position
        {
            get { return _position; }
            set { _position = value; }
        }

        private LOSlideSource _source;

        public LOSlideSource Source
        {
            get { return _source; }
            set { _source = value; initcomponent(); }
        }

        void initcomponent()
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


        void init()
        {
            Height = DeviceHeight;
            Width = DeviceWidth;

            _iconbar = new IconSlideBar();
            Children.Add(_iconbar);

            Background = new SolidColorBrush(Colors.Transparent);
            if (ISlideElementSizeChanged != null)
                ISlideElementSizeChanged(this);


        }
    }
}
