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
    public sealed partial class PopupView : Grid
    {
        TextBlock message = new TextBlock();
        public PopupView()
        {
            this.Height = 768;
            this.Width = 1366;
            this.Background = new SolidColorBrush(ColorHelper.FromArgb(170, 0, 0, 0));
            Grid g = new Grid() { Width = 300, Height = 500 };
            message.FontSize = 36;
            message.Foreground = new SolidColorBrush(Colors.White);
            g.Children.Add(message);
            this.Children.Add(g);
            this.Tapped += PopupView_Tapped;
            Canvas.SetZIndex(this, -10);
        }

        void PopupView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Canvas.SetZIndex(this,-10);
        }

        public string Message
        {
            set 
            {
                message.Text = value;
                Canvas.SetZIndex(this, 100);
            }
        }

    }
}
