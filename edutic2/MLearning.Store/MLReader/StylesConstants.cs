using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;

namespace MLReader
{
    public class StyleConstants
    {

        List<LOSlideStyle> styles = new List<LOSlideStyle>();
        public StyleConstants()
        {
            //load();
            loadnewstyles();
        }

         

        #region New Styles

        public List<List<LOSlideStyle>> stylesList = new List<List<LOSlideStyle>>();
        public List<Color> MainColors = new List<Color>();
        public List<Color> MainAlphaColors = new List<Color>();
        public List<Color> SecondColors = new List<Color>();

        public List<string> BackGroudns = new List<string>();

        void loadnewstyles()
        {


            BackGroudns.Add("ms-appx:///Resources/backgrounds/salkantay.png");
            BackGroudns.Add("ms-appx:///Resources/backgrounds/alternativas.png");
            BackGroudns.Add("ms-appx:///Resources/backgrounds/machupicchu.png");
            BackGroudns.Add("ms-appx:///Resources/backgrounds/flora.png");


            List<LOSlideStyle> greenStyle = new List<LOSlideStyle>();
            List<LOSlideStyle> redStyle = new List<LOSlideStyle>();
            List<LOSlideStyle> blueStyle = new List<LOSlideStyle>();
            List<LOSlideStyle> purpleStyle = new List<LOSlideStyle>();


            byte maxalpha = 255;
            byte midalpha = 146;


            //green colors
            Color green = Color.FromArgb(maxalpha, 112, 222, 23);
            Color green_mid_alpha = Color.FromArgb(midalpha, 112, 222, 23);
            Color light_green = Color.FromArgb(maxalpha, 202, 255, 62);
           

            greenStyle.Add(new LOSlideStyle { TitleColor = green, BorderColor = Colors.Black, BackgroundColor = Colors.Black, ContentColor = Colors.White });

            /* 1 */
            greenStyle.Add(new LOSlideStyle { TitleColor = green, BorderColor = green_mid_alpha, BackgroundColor = Colors.White, ContentColor = Colors.Black });
            greenStyle.Add(new LOSlideStyle { TitleColor = Colors.White, BorderColor = Colors.Black, BackgroundColor = green, ContentColor = Colors.Black });
            greenStyle.Add(new LOSlideStyle { TitleColor = light_green, BorderColor = Colors.Black, BackgroundColor = Colors.White, ContentColor = Colors.Black });
            greenStyle.Add(new LOSlideStyle { TitleColor = Colors.White, BorderColor = green, BackgroundColor = light_green, ContentColor = Colors.Black });
            /* 5 */
            greenStyle.Add(new LOSlideStyle { TitleColor = Colors.Black, BorderColor = Colors.Black, BackgroundColor = Colors.White, ContentColor = Colors.Black });
            greenStyle.Add(new LOSlideStyle { TitleColor = Colors.White, BorderColor = Colors.Black, BackgroundColor = Colors.Black, ContentColor = Colors.White });
            greenStyle.Add(new LOSlideStyle { TitleColor = green, BorderColor = green_mid_alpha, BackgroundColor = Colors.White, ContentColor = Colors.Black });

            //Add to StylesList

            stylesList.Add(greenStyle);


            //red colors
            Color red = Color.FromArgb(maxalpha, 255, 71, 69);
            Color red_mid_alpha = Color.FromArgb(midalpha, 255, 71, 69);
            Color light_red = Color.FromArgb(maxalpha, 250, 191, 57);
            

            redStyle.Add(new LOSlideStyle { TitleColor = red, BorderColor = Colors.Black, BackgroundColor = Colors.Black, ContentColor = Colors.White });

            /* 1 */
            redStyle.Add(new LOSlideStyle { TitleColor = red, BorderColor = red_mid_alpha, BackgroundColor = Colors.White, ContentColor = Colors.Black });
            redStyle.Add(new LOSlideStyle { TitleColor = Colors.White, BorderColor = Colors.Black, BackgroundColor = red, ContentColor = Colors.Black });
            redStyle.Add(new LOSlideStyle { TitleColor = light_red, BorderColor = Colors.Black, BackgroundColor = Colors.White, ContentColor = Colors.Black });
            redStyle.Add(new LOSlideStyle { TitleColor = Colors.White, BorderColor = red_mid_alpha, BackgroundColor = light_red, ContentColor = Colors.Black });
            /* 5 */
            redStyle.Add(new LOSlideStyle { TitleColor = Colors.Black, BorderColor = Colors.Black, BackgroundColor = Colors.White, ContentColor = Colors.Black });
            redStyle.Add(new LOSlideStyle { TitleColor = Colors.White, BorderColor = Colors.Black, BackgroundColor = Colors.Black, ContentColor = Colors.White });
            redStyle.Add(new LOSlideStyle { TitleColor = red, BorderColor = red_mid_alpha, BackgroundColor = Colors.White, ContentColor = Colors.Black });

            //Add to StylesList

            stylesList.Add(redStyle);


            //blue colors
            Color blue = Color.FromArgb(maxalpha, 92, 245, 255);
            Color blue_mid_alpha = Color.FromArgb(midalpha, 92, 245, 255);
            Color light_blue = Color.FromArgb(maxalpha, 0, 163, 151);
            


            blueStyle.Add(new LOSlideStyle { TitleColor = blue, BorderColor = Colors.Black, BackgroundColor = Colors.Black, ContentColor = Colors.White });

            /* 1 */
            blueStyle.Add(new LOSlideStyle { TitleColor = blue, BorderColor = blue_mid_alpha, BackgroundColor = Colors.White, ContentColor = Colors.Black });
            blueStyle.Add(new LOSlideStyle { TitleColor = Colors.White, BorderColor = Colors.Black, BackgroundColor = blue, ContentColor = Colors.Black });
            blueStyle.Add(new LOSlideStyle { TitleColor = light_blue, BorderColor = Colors.Black, BackgroundColor = Colors.White, ContentColor = Colors.Black });
            blueStyle.Add(new LOSlideStyle { TitleColor = Colors.White, BorderColor = blue, BackgroundColor = light_blue, ContentColor = Colors.Black });
            /* 5 */
            blueStyle.Add(new LOSlideStyle { TitleColor = Colors.Black, BorderColor = Colors.Black, BackgroundColor = Colors.White, ContentColor = Colors.Black });
            blueStyle.Add(new LOSlideStyle { TitleColor = Colors.White, BorderColor = Colors.Black, BackgroundColor = Colors.Black, ContentColor = Colors.White });
            blueStyle.Add(new LOSlideStyle { TitleColor = blue, BorderColor = blue_mid_alpha, BackgroundColor = Colors.White, ContentColor = Colors.Black });

            //Add to StylesList

            stylesList.Add(blueStyle);


            //purple colors
            Color purple = Color.FromArgb(maxalpha, 249, 98, 88);
            Color purple_mid_alpha = Color.FromArgb(midalpha, 249, 98, 88);
            Color light_purple = Color.FromArgb(maxalpha, 228, 42, 214);
           


            purpleStyle.Add(new LOSlideStyle { TitleColor = purple, BorderColor = Colors.Black, BackgroundColor = Colors.Black, ContentColor = Colors.White });

            /* 1 */
            purpleStyle.Add(new LOSlideStyle { TitleColor = purple, BorderColor = purple_mid_alpha, BackgroundColor = Colors.White, ContentColor = Colors.Black });
            purpleStyle.Add(new LOSlideStyle { TitleColor = Colors.White, BorderColor = Colors.Black, BackgroundColor = purple, ContentColor = Colors.Black });
            purpleStyle.Add(new LOSlideStyle { TitleColor = light_purple, BorderColor = Colors.Black, BackgroundColor = Colors.White, ContentColor = Colors.Black });
            purpleStyle.Add(new LOSlideStyle { TitleColor = Colors.White, BorderColor = purple, BackgroundColor = light_purple, ContentColor = Colors.Black });
            /* 5 */
            purpleStyle.Add(new LOSlideStyle { TitleColor = Colors.Black, BorderColor = Colors.Black, BackgroundColor = Colors.White, ContentColor = Colors.Black });
            purpleStyle.Add(new LOSlideStyle { TitleColor = Colors.White, BorderColor = Colors.Black, BackgroundColor = Colors.Black, ContentColor = Colors.White });
            purpleStyle.Add(new LOSlideStyle { TitleColor = purple, BorderColor = purple_mid_alpha, BackgroundColor = Colors.White, ContentColor = Colors.Black });

            //Add to StylesList

            stylesList.Add(purpleStyle);


            /////////////////////////////////

            MainColors.Add(red);
            MainAlphaColors.Add(red_mid_alpha);
            SecondColors.Add(light_red);

            MainColors.Add(blue);
            MainAlphaColors.Add(blue_mid_alpha);
            SecondColors.Add(light_blue);

            MainColors.Add(green);
            MainAlphaColors.Add(green_mid_alpha);
            SecondColors.Add(light_green);

            MainColors.Add(purple);
            MainAlphaColors.Add(purple_mid_alpha);
            SecondColors.Add(light_purple);
        }

        #endregion


        
    }
}
