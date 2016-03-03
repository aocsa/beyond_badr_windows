using MLearning.Store.Components;
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
    public sealed partial class ManipulableImageCollection : Grid, ISlideElement
    {

        double DeviceWidth = 1600;
        double DeviceHeight = 900;

        public ManipulableImageCollection()
        {
            init();
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

        //events for image animations
        public event BorderImageSelectedEventHandler BorderImageSelected;
        public event BorderImageReleasedEventHandler BorderImageReleased;

        TextBlock _title;
        TextBlock _name1, _name2, _name3;
        Grid _content1, _content2, _content3;
        Border _border1, _border2, _border3;
        Image _image1, _image2, _image3;

        AnimatedBorderImage borderimage1, borderimage2, borderimage3;
         
        void init()
        {
            Width = DeviceWidth;
            Height = DeviceHeight;

            initbar();

            //initGrid();
            initTextBlock();

            

            _title = new TextBlock()
            {
                Height = 60,
                Width = 800,
                RenderTransform = new TranslateTransform() { X = 322, Y = 136 },
                FontSize = 48,

                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left,
                VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Top
            };
            Children.Add(_title);

            Background = new SolidColorBrush(Colors.Transparent);
            ManipulationMode = ManipulationModes.All;


            borderimage1 = new AnimatedBorderImage();
            borderimage1.LoadComponent(276, 332, 250, 228, 10);
            Children.Add(borderimage1);

            borderimage2 = new AnimatedBorderImage();
            borderimage2.LoadComponent(276 + 320, 332, 250, 228, 10);
            Children.Add(borderimage2);

            borderimage3 = new AnimatedBorderImage();
            borderimage3.LoadComponent(276 + 640, 332, 250, 228, 10);
            Children.Add(borderimage3);

            borderimage1.BorderImageSelected += borderimage1_BorderImageSelected;
            borderimage1.BorderImageReleased += borderimage2_BorderImageReleased;

            borderimage2.BorderImageSelected += borderimage1_BorderImageSelected;
            borderimage2.BorderImageReleased += borderimage2_BorderImageReleased;

            borderimage3.BorderImageSelected += borderimage1_BorderImageSelected;
            borderimage3.BorderImageReleased += borderimage2_BorderImageReleased;
        }

        void borderimage2_BorderImageReleased(object sender, int index)
        {
            if (BorderImageReleased != null)
                BorderImageReleased(this, 0);
        }

        void borderimage1_BorderImageSelected(object sender, int index)
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
            if (_source != null)
            {
                _title.Text = Source.Title.ToUpper();
                _title.Foreground = new SolidColorBrush(Source.Style.TitleColor);
                int a = 0;

                foreach (LOItemSource item in Source.Itemize)
                {
                    if (a == 0)
                    {
                        borderimage1.BorderColor = Source.Style.TitleColor; _name1.Text = item.Text;
                        if (item.ImageBytes != null)
                            borderimage1.Image = Constants.ByteArrayToImageConverter.Convert(item.ImageBytes);
                        _source.PropertyChanged += (s, e) =>
                        {
                            if (e.PropertyName == "ImageBytes")
                                borderimage1.Image = Constants.ByteArrayToImageConverter.Convert(item.ImageBytes);
                        };
                    }
                    if (a == 1)
                    {
                        borderimage2.BorderColor = Source.Style.TitleColor; _name2.Text = item.Text;
                        if (item.ImageBytes != null)
                            borderimage2.Image = Constants.ByteArrayToImageConverter.Convert(item.ImageBytes);
                        _source.PropertyChanged += (s, e) =>
                        {
                            if (e.PropertyName == "ImageBytes")
                                borderimage2.Image = Constants.ByteArrayToImageConverter.Convert(item.ImageBytes);
                        };
                    }
                    if (a == 2)
                    {
                        borderimage3.BorderColor = Source.Style.TitleColor; _name3.Text = item.Text;
                        if (item.ImageBytes != null)
                            borderimage3.Image = Constants.ByteArrayToImageConverter.Convert(item.ImageBytes);
                        _source.PropertyChanged += (s, e) =>
                        {
                            if (e.PropertyName == "ImageBytes")
                                borderimage3.Image = Constants.ByteArrayToImageConverter.Convert(item.ImageBytes);
                        };
                    }
                    a++;
                }
            }
        }


        void initcomponent_1()
        {
            if (_source != null)
            {
                _title.Text = Source.Title.ToUpper();
                _title.Foreground = new SolidColorBrush(Source.Style.TitleColor);
                int a = 0;
                 
                foreach (LOItemSource item in Source.Itemize)
                {
                    if (a == 0)
                    {
                        borderimage1.BorderColor = Source.Style.TitleColor; _name1.Text = item.Text;
                        if (item.Image != null)
                            borderimage1.Image = item.Image;
                        _source.PropertyChanged += (s, e) =>
                        {
                            if (e.PropertyName == "Image")
                                borderimage1.Image = Source.Image;
                        };
                    }
                    if (a == 1)
                    {
                        borderimage2.BorderColor = Source.Style.TitleColor; _name2.Text = item.Text;
                        if (item.Image != null)
                            borderimage2.Image = item.Image;
                        _source.PropertyChanged += (s, e) =>
                        {
                            if (e.PropertyName == "Image")
                                borderimage2.Image = Source.Image;
                        };
                    }
                    if (a == 2)
                    {
                        borderimage3.BorderColor = Source.Style.TitleColor; _name3.Text = item.Text;
                        if (item.Image != null)
                            borderimage3.Image = item.Image;
                        _source.PropertyChanged += (s, e) =>
                        {
                            if (e.PropertyName == "Image")
                                borderimage3.Image = Source.Image;
                        };
                    }
                    a++;
                }
            }
        }

        void initcomponent()
        {
            if(_source!=null)
            {
                _title.Text = Source.Title.ToUpper();
                _title.Foreground = new SolidColorBrush(Source.Style.TitleColor);
                int a = 0;

                SolidColorBrush scolor = new SolidColorBrush(Source.Style.ContentColor) ;

                /*foreach (LOItemSource item in Source.Itemize)
                {
                    if (a == 0) { _image1.Source = item.Image; _name1.Text = item.Text; _border1.BorderBrush = scolor; }
                    if (a == 1) { _image2.Source = item.Image; _name2.Text = item.Text; _border2.BorderBrush = scolor; }
                    if (a == 2) { _image3.Source = item.Image; _name3.Text = item.Text; _border3.BorderBrush = scolor; }
                    a++;
                }
                 */
            }
        }


        void initTextBlock()
        {
            _name1 = new TextBlock()
            {
                Width = 270,
                Height = 30,
                RenderTransform = new TranslateTransform() { X = 276, Y = 332 + 280 },
                Foreground = new SolidColorBrush(Colors.Black),
                FontSize = 22,
                FontWeight = FontWeights.Light,
                VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Top,
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left,
                TextAlignment= Windows.UI.Xaml.TextAlignment.Center
            };
            Children.Add(_name1);

            _name2 = new TextBlock()
            {
                Width = 270,
                Height = 30,
                RenderTransform = new TranslateTransform() { X = 276 + 320, Y = 332 + 280 },
                Foreground = new SolidColorBrush(Colors.Black),
                FontSize = 22,
                FontWeight = FontWeights.Light,
                VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Top,
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left,
                TextAlignment = Windows.UI.Xaml.TextAlignment.Center
            };
            Children.Add(_name2);


            _name3 = new TextBlock()
            {
                Width = 270,
                Height = 30,
                RenderTransform = new TranslateTransform() { X = 276 + 640, Y = 332 + 280 },
                Foreground = new SolidColorBrush(Colors.Black),
                FontSize = 22,
                FontWeight = FontWeights.Light,
                VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Top,
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left,
                TextAlignment = Windows.UI.Xaml.TextAlignment.Center
            };
            Children.Add(_name3);


        }

        void initGrid()
        {
            _content1 = new Grid()
            {
                Width = 270,
                Height = 248,
                RenderTransform = new TranslateTransform() { X = 276, Y = 332 },
                VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Top,
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left
            };
            Children.Add(_content1);
            _image1 = new Image()
            {
                Width = 250,
                Height = 228,
                Stretch = Stretch.UniformToFill
            };
            _content1.Children.Add(_image1);
            _border1 = new Border()
            {
                Width = 270,
                Height = 248,
                CornerRadius = new Windows.UI.Xaml.CornerRadius(10),
                BorderThickness = new Windows.UI.Xaml.Thickness(10),
                BorderBrush = new SolidColorBrush(Colors.Black)
            };
            _content1.Children.Add(_border1);

            _content2 = new Grid()
            {
                Width = 270,
                Height = 248,
                RenderTransform = new TranslateTransform() { X = 276 + 320, Y = 332 },
                VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Top,
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left
            };
            Children.Add(_content2);
            _image2 = new Image()
            {
                Width = 250,
                Height = 228,
                Stretch = Stretch.UniformToFill
            };
            _content2.Children.Add(_image2);
            _border2 = new Border()
            {
                Width = 270,
                Height = 248,
                CornerRadius = new Windows.UI.Xaml.CornerRadius(10),
                BorderThickness = new Windows.UI.Xaml.Thickness(10),
                BorderBrush = new SolidColorBrush(Colors.Black)
            };
            _content2.Children.Add(_border2);

            _content3 = new Grid()
            {
                Width = 270,
                Height = 248,
                RenderTransform = new TranslateTransform() { X = 276+640, Y = 332 },
                VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Top,
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left
            };
            Children.Add(_content3);
            _image3 = new Image()
            {
                Width = 250,
                Height = 228,
                Stretch = Stretch.UniformToFill
            };
            _content3.Children.Add(_image3);
            _border3 = new Border()
            {
                Width = 270,
                Height = 248,
                CornerRadius = new Windows.UI.Xaml.CornerRadius(10),
                BorderThickness = new Windows.UI.Xaml.Thickness(10),
                BorderBrush = new SolidColorBrush(Colors.Black)
            };
            _content3.Children.Add(_border3);


        }


        ///////bar

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
