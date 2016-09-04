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
using DataSource;
using System.ComponentModel;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI;

namespace StackView
{
    public sealed partial class ChapterHeaderControl : Grid
    {
        //  public:
        public ChapterHeaderControl()
        {
            init();
        }


        public string Title
        {
            set { _title.Text =value.ToUpper(); }
            get { return _title.Text; }
        }

        public string Author
        {
            set { _tags.Text = "Beyond Badr"; }
            get { return _author.Text; }
        }

        public string Description
        {
            set { _description.Text = value; }
            get { return _description.Text; }
        }

        public Color ChapterColor
        {
            set { _chaptercolor = value; setColor(); }
            get { return _chaptercolor;   }
        }



        //private : 



        Color _chaptercolor;

        TextBlock _title, _tags, _author, _description;
        StackPanel panelgrid;

        void init()
        {
            Height = 376;
            Width = 376;

            panelgrid = new StackPanel();
            panelgrid.Orientation = Orientation.Vertical;
            Children.Add(panelgrid);

            _title = new TextBlock();
            _title.FontSize = 36;
            _title.FontWeight = Windows.UI.Text.FontWeights.SemiLight;
            _title.TextWrapping = TextWrapping.Wrap;
            _title.Height = 128.0; //88.0;
            _title.Width = 366.0;
            _title.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
            _title.Text = "EXPERIENCIA DE USUARIO";
            panelgrid.Children.Add(_title);

            _tags = new TextBlock();
            _tags.FontSize = 33;
            _tags.FontWeight = Windows.UI.Text.FontWeights.Normal;
            _tags.Height = 90.0; //52.0;
            _tags.Width = 366.0;
            _tags.TextWrapping = TextWrapping.Wrap;
            _tags.Foreground = new SolidColorBrush(Windows.UI.Colors.Aquamarine);
            _tags.Text = "Autor : "+ "INMERSIVA";
            panelgrid.Children.Add(_tags);

            _author = new TextBlock();
            _author.FontSize = 33;
            _author.TextWrapping = TextWrapping.Wrap;
            _author.Height = 58.0;
            _author.Width = 366.0;
            _author.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
            _author.Text = "Sensacional e intuitivo";
            ///panelgrid.Children.Add(_author);

            _description = new TextBlock();
            _description.FontSize = 22;
            _description.FontWeight = Windows.UI.Text.FontWeights.Normal;
            _description.TextWrapping = TextWrapping.Wrap;
            _description.Height = 178.0;
            _description.Width = 366.0;
            _description.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
            _description.Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.\n Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat";
            panelgrid.Children.Add(_description);


            Grid linev = new Grid();
            linev.Height = 350.0;
            linev.Width = 1.0;
            linev.Background = new SolidColorBrush(Windows.UI.Colors.Gray);
            linev.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Right;
            Children.Add(linev);
        }

        void setColor()
        {
            _tags.Foreground = new SolidColorBrush(_chaptercolor);
        }
    }
}
