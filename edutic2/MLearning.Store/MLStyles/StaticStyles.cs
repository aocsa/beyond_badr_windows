using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace MLearning.Store.MLStyles
{
    public class StaticStyles
    {

        public StaticStyles()
        {
 
        }

        public static Color DefaultColor = ColorHelper.FromArgb(255,78,177,223);
        
        public static string DefaultMuroBackground = "ms-appx:///Resources/brackgroundlogin.jpg";

        public static string DefaultLogoUri = "ms-appx:///Resources/muro/logo.png";


        public static List<MLColorStyle> Colors = new List<MLColorStyle> { 
                new MLColorStyle(){MainColor = ColorHelper.FromArgb(255,78,177,223), MainColorA = ColorHelper.FromArgb(130,255,71,69), 
                SecondColor = ColorHelper.FromArgb(255,250,191,57), SecondColorA = ColorHelper.FromArgb(130,250,191,57)}, // celeste, rojo,  amarillo, amarillo

                new MLColorStyle(){MainColor = ColorHelper.FromArgb(255,114,173,66), MainColorA = ColorHelper.FromArgb(130,114,173,66), 
                SecondColor = ColorHelper.FromArgb(255,195,216,72), SecondColorA = ColorHelper.FromArgb(130,195,216,72)}, // verder, verde, verderamarillo, verderamarillo

                new MLColorStyle(){MainColor = ColorHelper.FromArgb(255,0,163,151), MainColorA = ColorHelper.FromArgb(130,0,163,151), 
                SecondColor = ColorHelper.FromArgb(255,97,217,226), SecondColorA = ColorHelper.FromArgb(130,97,217,226)}, // aqua, 

                new MLColorStyle(){MainColor = ColorHelper.FromArgb(255,244,195,56), MainColorA = ColorHelper.FromArgb(130,244,195,56), 
                SecondColor = ColorHelper.FromArgb(255,247,82,149), SecondColorA = ColorHelper.FromArgb(130,247,82,149)}, //  amarillo
                 
        }; 

         
        public static List<string> OpenLOUrls = new List<string> 
        {
            "ms-appx:///Resources/openbts/btn_abrir-00.png",
            "ms-appx:///Resources/openbts/btn_abrir-00.png",
            "ms-appx:///Resources/openbts/btn_abrir-00.png",
            "ms-appx:///Resources/openbts/btn_abrir-00.png"
        };

        public static List<string> LogosMuro = new List<string> 
        {
            "ms-appx:///Resources/muro/logos/logo_ml-11.png", 
            "ms-appx:///Resources/muro/logos/logo_ml-12.png", 
            "ms-appx:///Resources/muro/logos/logo_ml-13.png", 
            "ms-appx:///Resources/muro/logos/logo_ml-14.png"
        };

        public static List<string> LogosScroll = new List<string> 
        {
            "ms-appx:///Resources/logo/logo-0.png", 
            "ms-appx:///Resources/logo/logo-1.png", 
            "ms-appx:///Resources/logo/logo-2.png", 
            "ms-appx:///Resources/logo/logo-3.png"
        };
        
    }
}
