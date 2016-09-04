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
    public sealed partial class RigthTextElement : Grid, ISlideElement
    {

        public RigthTextElement()
        {
            init();
            SizeChanged += RigthTextElement_SizeChanged;
            //Background = new SolidColorBrush(Colors.Pink);
        }

        void RigthTextElement_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            if (this.ActualHeight != _actualheight && this.ActualHeight > 900.0)
            {
                _actualheight = ActualHeight;
                if (ISlideElementSizeChanged != null)
                    ISlideElementSizeChanged(this);
            }
        }

        void RigthTextElement_LayoutUpdated(object sender, object e)
        {
            if (this.ActualHeight != _actualheight && this.ActualHeight > 900.0)
            {
                _actualheight = ActualHeight;
                Height = _actualheight;
                if (ISlideElementSizeChanged != null)
                    ISlideElementSizeChanged(this);
            }
        }

        public event ISlideElementSizeChangedEventHandler ISlideElementSizeChanged;

        StackPanel _contentpanel;
        TextBlock _titleblock, _contentblock;
        double _currentHeight;
        double _titleheight = 0.0, _contentheight = 0.0, _actualheight = 900.0;

        void init()
        {
            Width = 1600.0;
            Height = 900.0;
            _currentHeight = 900.0;

            _contentpanel = new StackPanel() { Orientation = Orientation.Vertical };
            _contentpanel.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Right;
            _contentpanel.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Top;
            _contentpanel.RenderTransform = new CompositeTransform() { TranslateX = -160.0 };
            _contentpanel.Width = 560.0;
            //_contentpanel.Background = new SolidColorBrush(Colors.YellowGreen);
            _contentpanel.SizeChanged += _contentpanel_SizeChanged;
            Children.Add(_contentpanel);

            Grid header = new Grid() { Width = 100.0, Height = 182.0 };
            Grid footer = new Grid() { Width = 100.0, Height = 182.0 };
            Grid separation = new Grid() { Width = 100.0, Height = 66.0 };
            _titleblock = new TextBlock() { TextWrapping = Windows.UI.Xaml.TextWrapping.Wrap, FontSize = 56 , TextAlignment = Windows.UI.Xaml.TextAlignment.Right};
            _titleblock.LayoutUpdated +=_titleblock_LayoutUpdated;
            _contentblock = new TextBlock() { TextWrapping = Windows.UI.Xaml.TextWrapping.Wrap, FontSize = 33, FontWeight = Windows.UI.Text.FontWeights.Light, TextAlignment = Windows.UI.Xaml.TextAlignment.Right };
            _contentblock.LayoutUpdated += _contentblock_LayoutUpdated;

            //childrens of content
            _contentpanel.Children.Add(header);
            _contentpanel.Children.Add(_titleblock);
            _contentpanel.Children.Add(separation);
            _contentpanel.Children.Add(_contentblock);
            _contentpanel.Children.Add(footer);
        }

        void _contentpanel_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            if (_contentpanel.ActualHeight > 900.0)
                Height = _contentpanel.ActualHeight;
        }

        void _contentblock_LayoutUpdated(object sender, object e)
        {
            _contentheight = _contentblock.ActualHeight; 
        }

        private void _titleblock_LayoutUpdated(object sender, object e)
        {
            _titleheight = _contentblock.ActualHeight; 
        }


        #region Properties

        private LOSlideSource _source;

        public LOSlideSource Source
        {
            get { return _source; }
            set { _source = value; initcomponent(); }
        }

        private double _position;

        public double Position
        {
            get { return _position; }
            set { _position = value; }
        }

        private Color _titlecolor;

        public Color TitleColor
        {
            get { return _titlecolor; }
            set { _titlecolor = value; }
        }

        private Color _contentcolor;

        public Color ContentColor
        {
            get { return _contentcolor; }
            set { _contentcolor = value; }
        }





        #endregion


        #region Functions


        public double GetSize()
        {
            return _actualheight;
        }


        void initcomponent()
        {
            if (Source != null)
            {
                if (Source.Title != null)
                    _titleblock.Text = _source.Title;
                _contentblock.Text = _source.Paragraph;
                _titleblock.Foreground = new SolidColorBrush(Source.Style.TitleColor);
                _contentblock.Foreground = new SolidColorBrush(Source.Style.ContentColor);

                double h = 2 * 182 + 66 + _titleblock.ActualHeight + _contentblock.ActualHeight;
                if (h > 900.0)
                    this.Height = h;
            }
        }

        #endregion
    }
}
