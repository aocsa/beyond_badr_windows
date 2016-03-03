using MLearning.Store.Components;
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
using Windows.UI.Xaml.Media.Imaging;

namespace MLReader.ManipulablePages
{
    public sealed partial class SingleManipulableImage : Grid, ISlideElement
    {
        public SingleManipulableImage()
        {
            initbar();
            init_1();
        }

        public double GetSize()
        {
            return 1600.0;
        }

        double _position ;
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
        //events for image animations
        public event BorderImageSelectedEventHandler BorderImageSelected;
        public event BorderImageReleasedEventHandler BorderImageReleased;

        Image _image;
        Border _border;
        Grid _content;
        CompositeTransform _transform;
        void init()
        {
            Width =  1600.0  ;
            Height = 900.0;
           // Background = new SolidColorBrush(Colors.PowderBlue);

            _transform = new CompositeTransform() { TranslateX = 998, TranslateY = 294};
            _content = new Grid() {
                Width= 402,
                Height = 372,
                RenderTransform = _transform,
                VerticalAlignment= Windows.UI.Xaml.VerticalAlignment.Top,
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left
            };
            Children.Add(_content);

            _image = new Image()
            {
                Stretch = Stretch.UniformToFill,
                Width = 374,
                Height = 344
            };
            _content.Children.Add(_image);

            _border = new Border() {
                Width = 402,
                Height = 372,
                CornerRadius = new Windows.UI.Xaml.CornerRadius(12),
                BorderThickness = new Windows.UI.Xaml.Thickness(14),
                BorderBrush = new SolidColorBrush(Colors.Red)
            };
            _content.Children.Add(_border);

            Background = new SolidColorBrush(Colors.Transparent);
            ManipulationMode = ManipulationModes.All;
        }

        AnimatedBorderImage _borderimage;
        void init_1()
        {
            Width = 1600.0;
            Height = 900.0;
            Background = new SolidColorBrush(Colors.Transparent);
            ManipulationMode = ManipulationModes.All;

            _borderimage = new AnimatedBorderImage();
            _borderimage.LoadComponent(998, 294, 374, 344, 14);
            _borderimage.BorderImageSelected += _borderimage_BorderImageSelected;
            _borderimage.BorderImageReleased += _borderimage_BorderImageReleased;
            Children.Add(_borderimage);
        }

        void _borderimage_BorderImageReleased(object sender, int index)
        {
            if (BorderImageReleased != null)
                BorderImageReleased(this, 0);
        }

        void _borderimage_BorderImageSelected(object sender, int index)
        {
            if (BorderImageSelected != null)
                BorderImageSelected(this, 0);
        }




         private LOSlideSource _source;

        public LOSlideSource Source
        {
            get { return _source; }
            set { _source = value; initcomponent_bytes(); initcomponent_2(); }
        }


        void initcomponent_bytes()
        {
            if (_source.ImageBytes != null)
                _borderimage.Image =  Constants.ByteArrayToImageConverter.Convert(_source.ImageBytes);
            _source.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "ImageBytes")
                    _borderimage.Image = Constants.ByteArrayToImageConverter.Convert(_source.ImageBytes);
            };
            _borderimage.BorderColor = Source.Style.TitleColor;

        }

        void initcomponent()
        {
           
            if(_source.Image!=null)
            _borderimage.Image = Source.Image;
            _source.PropertyChanged += (s, e) => { 
                if(e.PropertyName=="Image")
                    _borderimage.Image = Source.Image;
            };
            _borderimage.BorderColor = Source.Style.TitleColor;
        }


        private Color _bordercolor;

        public Color BorderColor
        {
            get { return _bordercolor; }
            set { _bordercolor = value;
            _borderimage.BorderColor = value;
            } //_border.BorderBrush = new SolidColorBrush(value); }
        }


        private BitmapImage _sourceimage;

        public BitmapImage SourceImage
        {
            get { return _sourceimage; }
            set { _sourceimage = value;  
            }//_image.Source = _sourceimage; }
        }


        IconSlideBar _iconbar;

        void initbar()
        {
            _iconbar = new IconSlideBar();
            Children.Add(_iconbar);
        }

         
        void initcomponent_2()
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

    }
}
