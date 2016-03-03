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

namespace MLearning.Store.Components
{
    public sealed partial class GridResource :  Grid
    {
        Rectangle aRectangle = new Rectangle();
        LinearGradientBrush gradientBrush = new LinearGradientBrush();

        // Create gradient stops for the brush.
        GradientStop stop1 = new GradientStop();
        GradientStop stop2 = new GradientStop();

        //Color for the animation
        List<Color> topcolors = new List<Color>(), bottoncolors =  new List<Color>(); 

        //animations
        Storyboard story = new Storyboard();
        ColorAnimation topanimation = new ColorAnimation();
        ColorAnimation bottonanimation = new ColorAnimation();

        //helpers
        int _indexcolor = 1;

        public GridResource()
        {
            this.Background = new SolidColorBrush(Windows.UI.Colors.Black);
                        
            aRectangle.Width = 1366;
            aRectangle.Height = 768;
           
            
            aRectangle.Opacity = 0.6;
            initcolors();
            initanimations();
            initbrush();
            animate_to(_indexcolor);
            this.Children.Add(aRectangle);


            Children.Add(new Image()
            {
                Width = 1366,
                Height = 768,
                Source = new BitmapImage(new Uri("ms-appx:///Resources/brackgroundlogin.jpg")),
                Stretch = Stretch.UniformToFill
                , Opacity =0.15
            });

           

            Image img = new Image();
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.UriSource = new Uri("ms-appx:///Resources/grilla.png",UriKind.Absolute);
            img.Opacity = .25;
            img.Source = bitmapImage;
            this.Children.Add(img);

            //this.Children.Add(aRectangle);
        }

        void initanimations()
        {  
            topanimation.Duration = TimeSpan.FromSeconds(10);
            story.Children.Add(topanimation);
            topanimation.EnableDependentAnimation = true;
            Storyboard.SetTarget(topanimation, stop1);
            Storyboard.SetTargetProperty(topanimation, "Color");
            bottonanimation.Duration = TimeSpan.FromSeconds(6);
            bottonanimation.EnableDependentAnimation = true;
            Storyboard.SetTarget(bottonanimation, stop2);
            Storyboard.SetTargetProperty(bottonanimation, "Color");
            story.Children.Add(bottonanimation);

            story.Completed += story_Completed;
        }

        void story_Completed(object sender, object e)
        {
            _indexcolor++;
            if (_indexcolor > 4)
                _indexcolor = 0;

            animate_to(_indexcolor);
        }

        void animate_to(int to)
        { 
            topanimation.To = topcolors[to]; 
            bottonanimation.To = bottoncolors[to];
            story.Begin();
        }

        void initbrush()
        {

            //add stops
            stop1.Color = topcolors[0];
            stop1.Offset = 0.0;
            stop2.Color = bottoncolors[0];
            stop2.Offset = 1.0;
            gradientBrush.GradientStops.Add(stop1);
            gradientBrush.GradientStops.Add(stop2);
            //add gradient
            gradientBrush.StartPoint = new Point(0.5, 0);
            gradientBrush.EndPoint = new Point(0.5,1.0);
            aRectangle.Fill = gradientBrush;
            
        }

        public void initcolors()
        {
            topcolors.Add(ColorHelper.FromArgb(255, 45, 189, 212));            
            topcolors.Add(ColorHelper.FromArgb(255, 224, 50, 115));
            topcolors.Add(ColorHelper.FromArgb(255, 37, 191, 44));
            topcolors.Add(ColorHelper.FromArgb(255, 137, 25, 178));
            topcolors.Add(ColorHelper.FromArgb(255, 32, 98, 229));            

            bottoncolors.Add(ColorHelper.FromArgb(255, 187, 210, 198));
            bottoncolors.Add(ColorHelper.FromArgb(255, 251, 147, 66));
            bottoncolors.Add(ColorHelper.FromArgb(255, 216, 204, 121));
            bottoncolors.Add(ColorHelper.FromArgb(255, 237, 168, 152));
            bottoncolors.Add(ColorHelper.FromArgb(255, 30, 197, 206));
        }

    }
}
