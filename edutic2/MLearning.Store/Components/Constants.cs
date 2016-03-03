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
using System.Text.RegularExpressions;

namespace MLearning.Store.Components
{
    public class Constants
    {

        public static Border GetCircleImage(double w, double h, byte[] img)
        {
            Border border = new Border() { Width = w, Height = h };
            border.Background = new ImageBrush() { Stretch = Stretch.UniformToFill, ImageSource = ByteArrayToImageConverter.Convert(img) };
            return border;
        }

        public string GetPlainTextFromHtml(string htmlString)
        {
            string htmlTagPattern = "<.*?>";
            var regexCss = new Regex("(\\<script(.+?)\\</script\\>)|(\\<style(.+?)\\</style\\>)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            htmlString = regexCss.Replace(htmlString, string.Empty);
            htmlString = Regex.Replace(htmlString, htmlTagPattern, string.Empty);
            htmlString = Regex.Replace(htmlString, @"^\s+$[\r\n]*", "", RegexOptions.Multiline);
            htmlString = htmlString.Replace("&nbsp;", string.Empty);

            return htmlString;
        }

        public class ByteArrayToImageConverter
        {
            public static BitmapImage Convert(object value)
            {
                if (value == null || !(value is byte[]))
                    return null;

                using (InMemoryRandomAccessStream ms = new InMemoryRandomAccessStream())
                {
                    using (DataWriter writer = new DataWriter(ms.GetOutputStreamAt(0)))
                    {
                        writer.WriteBytes((byte[])value);
                        writer.StoreAsync().GetResults();
                    }

                    var image = new BitmapImage();
                    image.SetSource(ms);
                    return image;
                }
            }

        }

    


    }


}
