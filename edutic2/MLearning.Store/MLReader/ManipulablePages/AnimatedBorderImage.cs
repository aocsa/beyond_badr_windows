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

namespace MLReader.ManipulablePages
{

    public delegate void BorderImageSelectedEventHandler(object sender, int index);
    public delegate void BorderImageReleasedEventHandler(object sender, int index);

    public sealed partial class AnimatedBorderImage : Grid
    {
        double DeviceWidth = 1600.0, DeviceHeight = 900.0;
        double image_width, image_height, border_thick, max_scale = 1.0, delta_offset = 0.0;
        double translate_x, translate_y, proportional_width, proportion;

        public AnimatedBorderImage()
        {
            init();
            initevents();
        }

        public event BorderImageSelectedEventHandler BorderImageSelected;
        public event BorderImageReleasedEventHandler BorderImageReleased;

        CompositeTransform _transform, _c_transform;
        ScrollViewer _scroll;
        Grid _content;
        Image _image;
        Border _border;

        void init()
        {
            Background = new SolidColorBrush(Colors.Transparent);
            VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Top;
            HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left;
            _transform = new CompositeTransform();
            _c_transform = new CompositeTransform();
            RenderTransform = _transform;

            //scroll
            _scroll = new ScrollViewer()
            {
                HorizontalScrollMode = ScrollMode.Enabled,
                VerticalScrollMode = ScrollMode.Enabled,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden,
                VerticalScrollBarVisibility = ScrollBarVisibility.Hidden
            };
            Children.Add(_scroll);
            //content
            _content = new Grid() { HorizontalAlignment = HorizontalAlignment.Left, ManipulationMode = ManipulationModes.All };
            _content.RenderTransform = _c_transform;
            _image = new Image() { Stretch = Stretch.UniformToFill };
            _border = new Border() { CornerRadius = new Windows.UI.Xaml.CornerRadius(10) };
            _content.Children.Add(_image);
            _content.Children.Add(_border);
            _scroll.Content = _content;
        }

        //w , h => image
        public void LoadComponent(double x, double y, double w, double h, double t)
        {
            image_height = h;
            image_width = w;
            translate_x = x;
            translate_y = y;
            border_thick = t;
            max_scale = DeviceHeight / image_height;
            proportion = h / DeviceHeight;
            delta_offset = ((proportion * DeviceWidth) - w) / 2;

            proportional_width = DeviceWidth * proportion + t * 2;
            _c_transform.TranslateX = -1.0 * delta_offset;
            _c_transform.CenterX = w / 2;
            _c_transform.CenterY = h / 2;
            _transform.TranslateX = translate_x;
            _transform.TranslateY = translate_y;
            _border.BorderThickness = new Windows.UI.Xaml.Thickness(t);

            Width = image_width + border_thick * 2;
            Height = image_height + border_thick * 2;
            _scroll.Width = image_width + border_thick * 2;
            _scroll.Height = image_height + border_thick * 2;

            _content.Width = DeviceWidth * proportion + border_thick * 2; //image_width + border_thick;
            _content.Height = image_height + border_thick * 2;

            _border.Width = image_width + border_thick * 2;
            _border.Height = image_height + border_thick * 2;

            _image.Width = DeviceWidth * proportion;
            _image.Height = image_height;


        }


        #region properties

        private int _index;

        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }


        private string _imageurl;

        public string ImageUrl
        {
            get { return _imageurl; }
            set { _imageurl = value;
            /**if (!_imageurl.Contains("http")) _imageurl = "ms-appx:///" + _imageurl;
                _image.Source = new BitmapImage(new Uri( _imageurl));*/
            }
        }


        private BitmapImage _imagesource    ;

        public BitmapImage Image
        {
            get { return _imagesource; }
            set { _imagesource = value; _image.Source = _imagesource; }
        }
        


        private Color _bordercolor;

        public Color BorderColor
        {
            get { return _bordercolor; }
            set { _bordercolor = value; _border.BorderBrush = new SolidColorBrush(value); }
        }


        #endregion


        #region event functions

        bool _isopen = false;

        void initevents()
        {
            ManipulationMode = ManipulationModes.All;
            Tapped += AnimatedBorderImage_Tapped;
        }

        void AnimatedBorderImage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (_isopen)
            {
                _isopen = false;
                _setcompletedevent = true;
                animateproperty2double(this, "(Grid.Width)", image_width + border_thick * 2);
                animateproperty2double(_scroll, "(ScrollViewer.Width)", image_width + border_thick * 2);
                animateproperty2double(_border, "(Border.Width)", image_width + border_thick * 2);
                animateproperty2double(_border, "Opacity", 1.0);
                animatetransform2double(_c_transform, "TranslateX", -1.0 * delta_offset);
                animatetransform2double(_transform, "TranslateX", translate_x);
                animatetransform2double(_transform, "TranslateY", translate_y);
                animatetransform2double(_transform, "ScaleX", 1.0);
                animatetransform2double(_transform, "ScaleY", 1.0);
                
                if (BorderImageReleased != null)
                    BorderImageReleased(this, _index);
            }
            else
            {
                animateproperty2double(this, "(Grid.Width)", proportional_width);
                animateproperty2double(_scroll, "(ScrollViewer.Width)", proportional_width);
                animateproperty2double(_border, "(Border.Width)", proportional_width);
                animateproperty2double(_border, "Opacity", 0.0);
                animatetransform2double(_c_transform, "TranslateX", 0.0);
                animatetransform2double(_transform, "TranslateX", -1.02 * border_thick * max_scale);
                animatetransform2double(_transform, "TranslateY", -1.03 * border_thick * max_scale);
                animatetransform2double(_transform, "ScaleX", max_scale + 0.01);
                animatetransform2double(_transform, "ScaleY", max_scale + 0.01);
                Canvas.SetZIndex(this, 10);
                if (BorderImageSelected != null)
                    BorderImageSelected(this, _index);
                _isopen = true;
            }
        }

        #endregion


        bool _setcompletedevent = false;

        void animateproperty2double(FrameworkElement el, string path, double to)
        {
            Storyboard story = new Storyboard();
            DoubleAnimation animation = new DoubleAnimation();
            animation.Duration = TimeSpan.FromMilliseconds(350);
            animation.EnableDependentAnimation = true;
            Storyboard.SetTarget(animation, el);
            Storyboard.SetTargetProperty(animation, path);
            animation.To = to;
            story.Children.Add(animation);

            if(_setcompletedevent)
                story.Completed += story_Completed;
            _setcompletedevent = false;
            story.Begin();
        }

        void story_Completed(object sender, object e)
        {
            Canvas.SetZIndex(this, 1); 
        }


        void animatetransform2double(CompositeTransform t, string path, double to)
        {
            Storyboard story = new Storyboard();
            DoubleAnimation animation = new DoubleAnimation();
            animation.Duration = TimeSpan.FromMilliseconds(350);
            animation.EnableDependentAnimation = true;
            Storyboard.SetTarget(animation, t);
            Storyboard.SetTargetProperty(animation, path);
            animation.To = to;
            story.Children.Add(animation);
            story.Begin();
        }
    }
}
