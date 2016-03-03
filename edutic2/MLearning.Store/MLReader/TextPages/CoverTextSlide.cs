using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace MLReader
{
    public sealed partial class CoverTextSlide : Grid, ISlideElement
    {
        double DeviceHeight = 900.0, DeviceWidth = 1600.0;

        public CoverTextSlide()
        {
            init();
            Background = new SolidColorBrush(Colors.Transparent);
        }

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
        

        public event ISlideElementSizeChangedEventHandler ISlideElementSizeChanged;

        StackPanel _container;
        TextBlock _titleblock, _contentblock;
        Grid _linegrid; 

        void init()
        {
            Height = DeviceHeight;
            Width = DeviceWidth;

            _container = new StackPanel() 
            {
                VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Bottom,
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Right,
                RenderTransform = new TranslateTransform() {Y = -126, X=-116},
                Orientation = Orientation.Vertical,
                Width = 786
            };
            Children.Add(_container);

            _titleblock = new TextBlock()
            {
                Width = 786,  
                TextAlignment = Windows.UI.Xaml.TextAlignment.Right,
                TextWrapping = Windows.UI.Xaml.TextWrapping.Wrap,
                FontSize = 56
            };
            _container.Children.Add(_titleblock);
           
            _linegrid = new Grid()
            {
                Width = 776,
                Height = 1,
                Background = new SolidColorBrush(Colors.Gray),
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Right
            };
            _container.Children.Add(_linegrid);
            
            _contentblock = new TextBlock()
            {
                Width = 786,  
                TextAlignment = Windows.UI.Xaml.TextAlignment.Right,
                TextWrapping = Windows.UI.Xaml.TextWrapping.Wrap,
                FontSize = 33,
                FontWeight = Windows.UI.Text.FontWeights.Light, 
            };
            _container.Children.Add(_contentblock);

            
        }


        private LOSlideSource _source;

        public LOSlideSource Source
        {
            get { return _source; }
            set { _source = value; initcomponent(); }
        }

        void initcomponent()
        {
            if (Source != null)
            {
                _titleblock.Text = _source.Title.ToUpper();
                _contentblock.Text = _source.Paragraph;
                _titleblock.Foreground = new SolidColorBrush(Source.Style.TitleColor);
                _contentblock.Foreground = new SolidColorBrush(Source.Style.ContentColor);

                UpdateLayout(); 
            }
        }


     
    }
}
