using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
using Windows.UI;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Text;
using Windows.Storage.Streams;
using System.ComponentModel;

namespace MLearning.Store.Components
{
    public sealed partial class UpMenuController : Grid , INotifyPropertyChanged
    {
        public UpMenuController()
        {
            init();
        }

        public void Animate2Color(Color c)
        {
            Storyboard story = new Storyboard();
            ColorAnimation animation = new ColorAnimation();
            animation.Duration = TimeSpan.FromMilliseconds(600);
            story.Children.Add(animation);
            animation.EnableDependentAnimation = true;
            Storyboard.SetTarget(animation, _border);
            Storyboard.SetTargetProperty(animation, "(Border.Background).(SolidColorBrush.Color)");
            animation.To = c;
            story.Begin();
        }

        public void SEtColor(Color c)
        {
            _border.Background = new SolidColorBrush(c);
            animate2double(new Storyboard(), 1.0, "Opacity", this);
            
        }

        Border _border;
        Grid _controlgrid, _homegrid, _commentgrid, _sharegrid;
        Grid _buttonsgrid;
        void init()
        {
            Opacity = 0.0;
            Height = 64.0;
            Width = 1038.0;
            VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Top;
            HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left;
            RenderTransform = new CompositeTransform() { TranslateX = 82.0, TranslateY = 136.0 };

            _border = new Border()
            {
                Height = 64.0,
                Width = 1038.0,
                CornerRadius = new CornerRadius(16),
                Background = new SolidColorBrush(Colors.Transparent),
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left
            };

            Children.Add(_border);

            _buttonsgrid = new Grid() { Width = 1038, Height = 64 };
            Children.Add(_buttonsgrid);

            //control grid
            _controlgrid = new Grid()
            {
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left,
                Height = 60,
                Width = 80
            };
            _controlgrid.Tapped += _controlgrid_Tapped;
            Children.Add(_controlgrid);
            Image circleimage = new Image() { Width = 36, Source = new BitmapImage(new Uri("ms-appx:///Resources/menu/CIRCULO.png")) };
            _controlgrid.Children.Add(circleimage);

            //home grid
            _homegrid = new Grid()
            {
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left,
                Height = 60,
                Width = 80,
                RenderTransform = new TranslateTransform() { X = 80 }
            };
            _homegrid.Tapped += _homegrid_Tapped;
            _buttonsgrid.Children.Add(_homegrid);
            Image homeimage = new Image() { Width = 36, Source = new BitmapImage(new Uri("ms-appx:///Resources/menu/HOME.png")) };
            _homegrid.Children.Add(homeimage);

            //home grid
            _sharegrid = new Grid()
            {
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Right,
                Height = 60,
                Width = 60,
                RenderTransform = new TranslateTransform() { X = -60 }
            };
            _sharegrid.Tapped += _sharegrid_Tapped;
            _buttonsgrid.Children.Add(_sharegrid);
            Image shareimage = new Image() { Width = 36, Source = new BitmapImage(new Uri("ms-appx:///Resources/menu/SHARE.png")) };
            _sharegrid.Children.Add(shareimage);

            //home grid
            _commentgrid = new Grid()
            {
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Right,
                Height = 60,
                Width = 60,
                RenderTransform = new TranslateTransform() { X = 0 }
            };
            _commentgrid.Tapped += _commentgrid_Tapped;
            _buttonsgrid.Children.Add(_commentgrid);
            Image commentimage = new Image() { Width = 36, Source = new BitmapImage(new Uri("ms-appx:///Resources/menu/HELP.png")) };
            _commentgrid.Children.Add(commentimage);

            double initpos = 286.0;
            Grid button1 = getnewtextbt("Beyond Badr: Introducing the first epic battle in Islam!", initpos);
            button1.Tapped += button1_Tapped;
            _buttonsgrid.Children.Add(button1);
            //2
            initpos += 132;
            Grid button2 = getnewtextbt(" ", initpos);
            button2.Tapped += button2_Tapped;
            _buttonsgrid.Children.Add(button2);
            //3
            initpos += 132;
            Grid button3 = getnewtextbt(" ", initpos);
            button3.Tapped += button3_Tapped;
            _buttonsgrid.Children.Add(button3);
            //4
            initpos += 132;
            Grid button4 = getnewtextbt(" ", initpos);
            button4.Tapped += button4_Tapped;
            _buttonsgrid.Children.Add(button4);

        }

        void button4_Tapped(object sender, TappedRoutedEventArgs e)
        {
        }

        void button3_Tapped(object sender, TappedRoutedEventArgs e)
        {
        }

        void button2_Tapped(object sender, TappedRoutedEventArgs e)
        {
        }

        void button1_Tapped(object sender, TappedRoutedEventArgs e)
        {
        }


        Grid getnewtextbt(string text, double x)
        {
            Grid g = new Grid()
            {
                Width = 500,
                Height = 28,
                Background = new SolidColorBrush(Colors.Transparent),
                RenderTransform = new TranslateTransform() { X = x },
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left
            };
            TextBlock tb = new TextBlock() { Text = text, FontSize = 21 };
            g.Children.Add(tb);

            return g;
        }

        void _commentgrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
        }

        void _sharegrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
        }

        void _homegrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("Home"));
        }

        void _controlgrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (_isopen)
                animate2close();
            else animate2open();
        }


        private bool _isopen = true;

        public bool IsOpen
        {
            get { return _isopen; }
            set { _isopen = value; }
        }


        public void animate2open()
        {
            Storyboard s = new Storyboard();
            s.Completed += s_Completed_1;
            animate2double(s, 1.0, "Opacity", _border);
            IsOpen = true;
        }

        void s_Completed_1(object sender, object e)
        {
            Storyboard s = new Storyboard();
            s.Completed += s_Completed;
            animate2double(s, 1038.0, "Boder.Width", _border);
        }

        void s_Completed(object sender, object e)
        {
            animate2double(new Storyboard(), 1.0, "Opacity", _buttonsgrid);
        }

        public void animate2close()
        {
            Storyboard s = new Storyboard();
            s.Completed += s_Completed_2;
            animate2double(s, 0.0, "Opacity", _buttonsgrid);
            IsOpen = false;
        }

        void s_Completed_2(object sender, object e)
        {
            Storyboard s = new Storyboard();
            s.Completed += s_Completed21;
            animate2double(s, 80.0, "Border.Width", _border);
        }

        private void s_Completed21(object sender, object e)
        {
            animate2double(new Storyboard(), 0.0, "Opacity", _border);
        }


        void animate2double(Storyboard story, double to, string prop, FrameworkElement el)
        {
            DoubleAnimation animation = new DoubleAnimation();
            animation.Duration = TimeSpan.FromMilliseconds(400);
            animation.EnableDependentAnimation = true;
            animation.EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseIn };
            Storyboard.SetTarget(animation, el);
            Storyboard.SetTargetProperty(animation, prop);
            animation.To = to;
            story.Children.Add(animation);
            story.Begin();
        }


        public event PropertyChangedEventHandler PropertyChanged;
    }
}
