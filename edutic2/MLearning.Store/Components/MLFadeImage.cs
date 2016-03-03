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
namespace MLearning.Store.Components
{
    public sealed partial class MLFadeImage : Grid 
    {

        public MLFadeImage()
        {
            init();
        }

        Image animatedimg, backimg;
        Grid topbrush;

        void init()
        {
            animatedimg = new Image() { Stretch  = Stretch.UniformToFill};
            backimg = new Image() { Stretch = Stretch.UniformToFill };
            animatedimg.Opacity = 0.0;
            Children.Add(backimg);
            Children.Add(animatedimg);


            topbrush = new Grid();
            topbrush.Background = new SolidColorBrush(Colors.Black);
            Children.Add(topbrush);
            topbrush.Opacity = 0.2;
        }


        private BitmapImage _backsource;

        public BitmapImage BackSource
        {
            get { return _backsource; }
            set { _backsource = value; backimg.Source = _backsource; }
        }


        private BitmapImage _newsource;

        public BitmapImage NewSource
        {
            get { return _newsource; }
            set {
                backimg.Source = _newsource;
                backimg.Opacity = 1.0;
                animatedimg.Opacity = 0.0;
                _newsource = value;
                animatedimg.Source = _newsource;

                animateimage(animatedimg, 1.0);
                animateimage(backimg, 0.0);
            }
        }

        private double _brushopacity;

        public double BrushOpacity
        {
            get { return _brushopacity; }
            set { _brushopacity = value; animatebrush(_brushopacity); }
        }
        


        private bool _brushvisible;

        public bool BrushVisible
        {
            get { return _brushvisible; }
            set
            {
                _brushvisible = value;
                if (_brushvisible) topbrush.Visibility = Windows.UI.Xaml.Visibility.Visible;
                else topbrush.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }
        

        void animateimage(Image img, double to)
        {
            Storyboard story = new Storyboard();
            DoubleAnimation animation = new DoubleAnimation();
            animation.EnableDependentAnimation = true;
            animation.Duration = TimeSpan.FromMilliseconds(400);
            Storyboard.SetTarget(animation, img);
            Storyboard.SetTargetProperty(animation, "Opacity");
            animation.To = to;
            story.Children.Add(animation);
            story.Begin();
        }

        void animatebrush(double to)
        {
            Storyboard story = new Storyboard();
            DoubleAnimation animation = new DoubleAnimation();
            animation.EnableDependentAnimation = true;
            animation.Duration = TimeSpan.FromMilliseconds(400);
            Storyboard.SetTarget(animation, topbrush);
            Storyboard.SetTargetProperty(animation, "Opacity");
            animation.To = to;
            story.Children.Add(animation);
            story.Begin();
        }
         



    }
}
