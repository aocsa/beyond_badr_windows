using MLearning.Store.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace MLReader
{
    public sealed partial class BackgroundElement : Grid , ISlideElement, INotifyPropertyChanged
    {
        double DeviceHeight = 900.0, DeviceWidth = 1600.0; 

        public BackgroundElement()
        {
            init();
        }
        

        public event ISlideElementSizeChangedEventHandler ISlideElementSizeChanged;

        public double GetSize()
        {
            return DeviceHeight;
        }

        private double _position;

        public double Position
        {
            get { return _position; }
            set { _position = value; }
        }


        Image _backimage;
        Grid _blackgrid;
        void init()
        {
            Height = DeviceHeight;
            Width = DeviceWidth;

            _backimage = new Image() { Stretch = Stretch.UniformToFill, Width = DeviceWidth, Height = DeviceHeight };
            _backimage.ImageOpened += _backimage_ImageOpened;
            Children.Add(_backimage);
            Background = new SolidColorBrush(Colors.Transparent);

            _blackgrid = new Grid() { Width = DeviceWidth, Height = DeviceHeight, Background = new SolidColorBrush(Colors.Black), Opacity = 0.0 };
            Children.Add(_blackgrid);
        }

        void _backimage_ImageOpened(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("ImageOpened"));
        }


        private LOSlideSource _source;

        public LOSlideSource Source
        {
            get { return _source; }
            set { _source = value; init_sourcebytes(); }
        }


        void init_sourcebytes()
        {
            if (_source.ImageBytes != null && (_source.Type == 0 || _source.Type == 6))
            {
                _backimage.Source = Constants.ByteArrayToImageConverter.Convert(_source.ImageBytes);
            }

            _source.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "ImageBytes" && (_source.Type == 0 || _source.Type == 6))
                   _backimage.Source = Constants.ByteArrayToImageConverter.Convert(_source.ImageBytes);
            };


            if (_source.Type == 0)
            {
                _blackgrid.Opacity = 0.4;
            }
            else
            {
                Background = new SolidColorBrush(_source.Style.BackgroundColor);
            }
        }
        

        void initsource()
        {
            if (_source.Image != null && (_source.Type == 0 || _source.Type == 6))
            {
                _backimage.Source = _source.Image;
            }
             
            _source.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "Image" && (_source.Type == 0 || _source.Type == 6))
                    _backimage.Source = Source.Image;
            };

            Background = new SolidColorBrush(_source.Style.BackgroundColor);
            if (_source.Type == 0) _blackgrid.Opacity = 0.4;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
