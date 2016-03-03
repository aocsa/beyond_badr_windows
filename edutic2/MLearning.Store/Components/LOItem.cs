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
    public delegate void DoLikeEventHandler(object sender, int index);
    public delegate void DoCommentEventHandler(object sender, int index);
    public delegate void SelectedEventHandler(object sender, int index);
    public sealed partial class LOItem : Grid
    {
        TextBlock _liketext, textname;
        Border _loBorder , _selectBorder;
        Grid _tapGrid;

        public LOItem()
        {
            this.Width = 116;
            this.Height = 138;
            this.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Top;
            this.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left;
            initLOborder();
            initSelectBorder();
            _tapGrid = new Grid() { Width = 116, Height = 105, Background = new SolidColorBrush(Colors.Transparent),
                VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Top};
            this.Children.Add(_tapGrid);
            _tapGrid.Tapped += _tapGrid_Tapped;
        }

        void _tapGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Selected(this, _index);
        }

        #region Properties

        public event DoLikeEventHandler DoLike;
        public event DoCommentEventHandler DoComment;
        public event SelectedEventHandler Selected;

        bool _likethis = false;
        public bool LikeThis
        {
            get { return _likethis; }
            set { 
                _likethis = value;
                if (_likethis)
                    _liketext.Text = "Me Gusta";
                else _liketext.Text = "Te Gusta"; 
            }
        }

        bool _isSelected = false;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { 
                _isSelected = value;
                if (_isSelected) _selectBorder.Opacity = 1.0;
                else _selectBorder.Opacity = 0.0;
            }
        }

        int _index = 0;
        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }

        string _title = string.Empty;
        public string Title
        {
            get { return _title; }
            set {
                _title = value;
                textname.Text = _title; // +" by " + Author;
            }
        }

        string _author = string.Empty;
        public string Author
        {
            get { return _author; }
            set { _author = value;}// textname.Text = Title + " by " + _author; }
        }


        byte[] _imagebytes = null;
        public byte[] ImageBytes
        {
            set 
            {
                _imagebytes = value;
                if (_imagebytes != null)
                    _loBorder.Background = new ImageBrush() { Stretch = Stretch.UniformToFill, ImageSource = Constants.ByteArrayToImageConverter.Convert(_imagebytes) }; 
            }
        }


        private Color  _bordercolor;

        public Color  BorderColor
        {
            get { return _bordercolor; }
            set { _bordercolor = value; _selectBorder.BorderBrush = new SolidColorBrush(_bordercolor); }
        }


        private Color _likecolor;

        public Color LikeColor
        {
            get { return _likecolor; }
            set { _likecolor = value;

            _likeborder.BorderBrush = new SolidColorBrush(value);
            _likeborder.Background = new SolidColorBrush(value);
                //_commentborder
            _commentborder.BorderBrush = new SolidColorBrush(value);
            _commentborder.Background = new SolidColorBrush(value);
            }
        }
        


        #endregion

        void initLOborder() 
        {
            _loBorder = new Border() 
            {
                Width = 116,
                Height = 138 ,
                CornerRadius =  new CornerRadius(4),
                BorderThickness =  new Thickness(1)                
            };
            this.Children.Add(_loBorder);     
            ///Init 
            initShadow();
            initButtons();
        }

        void initSelectBorder()
        {
            _selectBorder = new Border()
            {
                Width = 116,
                Height = 138,
                CornerRadius = new CornerRadius(4),
                BorderThickness = new Thickness(3),
                BorderBrush = new SolidColorBrush(ColorHelper.FromArgb(255 , 78,177,223)),
                Opacity = 0
            };
            this.Children.Add(_selectBorder);
        }


        Border _shadow;

        void initShadow()
        {
            _shadow = new Border()
            {
                Background = new SolidColorBrush(ColorHelper.FromArgb(60,0,0,0)) ,
                VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Bottom,
                Width = 116,
                Height = 60,
                CornerRadius = new CornerRadius(0,0,4,4),
                BorderThickness = new Thickness(1)
            };
            Children.Add(_shadow);

            textname = new TextBlock()
            {
                FontSize = 12,
                Foreground = new SolidColorBrush(Colors.White),
                TextWrapping = TextWrapping.Wrap,
                Height = 34,
                Width = 106,
                VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Bottom,
                RenderTransform = new TranslateTransform() { Y = -22}
            };
            this.Children.Add(textname);
        }



        Border _likeborder, _commentborder;
        void initButtons()
        {
            _likeborder = new Border()
            {
                Width = 52,
                Height = 18,  
                VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Bottom,
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Right,
                RenderTransform = new TranslateTransform() { Y = -6, X = -4 },
                CornerRadius= new CornerRadius(4),
                BorderBrush = new SolidColorBrush(ColorHelper.FromArgb(255,78,177,223)),
                Background = new SolidColorBrush(ColorHelper.FromArgb(255, 78, 177, 223))
            };
            this.Children.Add(_likeborder);
            _likeborder.Tapped += _likeborder_Tapped;
            _liketext = new TextBlock() { FontSize = 9, TextAlignment = TextAlignment.Center, Text = "Me Gusta", VerticalAlignment = VerticalAlignment.Center };
            _likeborder.Child = _liketext;

            _commentborder = new Border()
            {
                Width =52,Height=18,
                VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Bottom,
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Right,
                RenderTransform = new TranslateTransform() { Y = -10, X = -4 },
                CornerRadius = new CornerRadius(4),
                BorderBrush = new SolidColorBrush(ColorHelper.FromArgb(255, 78, 177, 223)),
                Background = new SolidColorBrush(ColorHelper.FromArgb(255, 78, 177, 223))
            };
            //this.Children.Add(_commentborder);
            _commentborder.Tapped += _commentborder_Tapped;
            TextBlock _commentext = new TextBlock() { FontSize=9, TextAlignment = TextAlignment.Center, Text = "Comentar", VerticalAlignment =VerticalAlignment.Center };
            _commentborder.Child = _commentext;
            
        }

        void _commentborder_Tapped(object sender, TappedRoutedEventArgs e)
        {
            DoComment(this, _index);
        }

        void _likeborder_Tapped(object sender, TappedRoutedEventArgs e)
        {
            _likethis = !_likethis;
            if (_likethis)
                _liketext.Text = "Me Gusta";
            else _liketext.Text = "Te Gusta";
           
            DoLike(this, _index);
        }

    }
}
