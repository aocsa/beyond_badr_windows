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
    public sealed partial class HorizontalLoading : Grid
    {

        public HorizontalLoading()
        {
        }


        ProgressBar progressbar;

        void init()
        {
            Background = new SolidColorBrush(ColorHelper.FromArgb(70, 0, 0, 0));

            progressbar = new ProgressBar() { IsIndeterminate =true };
            Children.Add(progressbar);
        }
    }
}
