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


namespace MLearning.Store.Components
{
    public sealed partial class LoadingView : Grid
    {
        ProgressRing pr;
        public LoadingView()
        {
            this.Height = 768;
            this.Width = 1366;

            this.Background = new SolidColorBrush(ColorHelper.FromArgb(170, 0, 0, 0));

            pr = new ProgressRing()
            {
                IsActive = true,
                Background = new SolidColorBrush(Colors.Transparent),
                Width = 80 ,
                Height = 80,
                BorderBrush = new SolidColorBrush(Colors.White),
                Foreground = new SolidColorBrush(Colors.White)
  
            }; 

            this.Children.Add(pr);
        }


        public double RingWidth
        {
            set { pr.Height = value;
            pr.Width = value;
            }
        }

        int _opacity = 170;
        public int BackOpacity
        {
            get { return _opacity; }
            set {
                _opacity = value;
                this.Background = new SolidColorBrush(ColorHelper.FromArgb((byte)_opacity, 0, 0, 0));
            }
        }
        
        
        public SolidColorBrush RingColor
        {
            set 
            {
                pr.BorderBrush = value;
                pr.Foreground = value;

            }
        }
        

    }
}
