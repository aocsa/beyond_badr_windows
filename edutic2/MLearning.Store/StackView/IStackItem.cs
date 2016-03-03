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

    public delegate void StackItemSelectedEventHandler(object sender, int _itemnumber);
    public delegate void StackItemTappedEventHandler(object sender, int _itemnumber);

    public sealed partial class IStackItem : Grid
    {
        public IStackItem()
        {
            initcontrols();
            initproperties();
            inititemanimations();
            Background = new SolidColorBrush(Windows.UI.Colors.Transparent);
        }

        #region Controls

        //private:  
        Image _thumbimage;
        Image _borderimage;
        Border _bordercolor;


        Grid _itemgrid;
        Image _itemimage;

        CompositeTransform _transform;

        //init the control
        void initcontrols()
        {
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
            this.Width = _borderwidth;
            this.Height = _borderheight;

            this._transform = new CompositeTransform();
            this.RenderTransform = this._transform;


            //init the border Color
            _bordercolor = new Border();
            this._bordercolor.Width = _borderwidth;
            this._bordercolor.Height = _borderheight;
            _bordercolor.BorderThickness = new Thickness(10);
            _bordercolor.CornerRadius = new CornerRadius(4);
            _bordercolor.Background = new SolidColorBrush(ColorHelper.FromArgb(150,112,138,144));

            this.Children.Add(_bordercolor);

            this._itemgrid = new Grid();
            this.Children.Add(_itemgrid);
            this._itemgrid.Height = _thumbheight;
            this._itemgrid.Width = _thumbwidth;

            this._thumbimage = new Image();
            this._thumbimage.Height = _thumbheight;
            this._thumbimage.Width = _thumbwidth;
            this._thumbimage.Stretch = Stretch.Fill;
            this.Children.Add(_thumbimage);

            //for touch mainpualtions
            this.PointerPressed += StackItem_PointerPressed;
            this.PointerReleased += StackItem_PointerReleased;
            this.Tapped += StackItem_Tapped;

            //for text and description
            _textpanel = new StackPanel();
            _textpanel.VerticalAlignment = VerticalAlignment.Center;
            _textpanel.HorizontalAlignment = HorizontalAlignment.Center;
            _textpanel.Orientation = Orientation.Vertical;
            _textpanel.Height = 160;
            _textpanel.Width = 140;
            _textpanel.Opacity = 0.0;

            _titletext = new TextBlock();
            _titletext.Height = 40;
            _titletext.Width = 140;
            _titletext.FontSize = 16;
            _titletext.Text = "Nombre de Item";
            _titletext.TextWrapping = TextWrapping.Wrap;
            _titletext.TextAlignment = TextAlignment.Center;
            _textpanel.Children.Add(_titletext);

            _descriptiontext = new TextBlock();
            _descriptiontext.Height = 120;
            _descriptiontext.Width = 140;
            _descriptiontext.FontSize =12 ;
            _descriptiontext.Text = "Descripcion del Item";
            _descriptiontext.TextWrapping = TextWrapping.Wrap;
            _descriptiontext.TextAlignment = TextAlignment.Center;
            _textpanel.Children.Add(_descriptiontext);

            _texttransform = new CompositeTransform();
            _textpanel.RenderTransform = _texttransform;
            //
            this.Children.Add(_textpanel);
        }


        //Controls for text
        StackPanel _textpanel;
        TextBlock _titletext;
        TextBlock _descriptiontext;
        CompositeTransform _texttransform;

        #endregion

        #region Events

        //public:
        public event StackItemSelectedEventHandler StackItemSelected;
        public event StackItemTappedEventHandler StackItemTapped;

        public event StackItemFullAnimationStartedEventHandler StackItemFullAnimationStarted;
        public event StackItemFullAnimationCompletedEventHandler StackItemFullAnimationCompleted;
        public event StackItemThumbAnimationStartedEventHandler StackItemThumbAnimationStarted;
        public event StackItemThumbAnimationCompletedEventHandler StackItemThumbAnimationCompleted;
        #endregion

        #region Paging Properties
        //public:
        public int Chapter
        {
            set { _chapter = value; }
            get { return _chapter; }
        }

        public int Section
        {
            set { _section = value; }
            get { return _section; }
        }

        public int Page
        {
            set { _page = value; }
            get { return _page; }
        }

        //private:
        int _chapter, _section, _page;
        #endregion


        #region Properties

        //public:

        public int ItemNumber
        {
            set
            {
                _itemnumber = value;
                ZIndex = _maxthreshold - _itemnumber;
            }
            get { return _itemnumber; }
        }

        public double InitialAngle
        {
            set
            {
                _initialangle = value;
                _transform.Rotation = value;
            }
            get { return _initialangle; }
        }

        public int ZIndex
        {
            set { Canvas.SetZIndex(this, value); }
            get { return Canvas.GetZIndex(this); }
        }

        public PageDataSource Source
        {
            set
            {
                _datasource = value; 
            }
            get { return _datasource; }
        }
 

        public string BorderSource
        {
            set { _bordersource = value; }
            get { return _bordersource; }
        }

        public CompositeTransform ItemTransform
        {
            set { }
            get { return _transform; }
        }

        public double MaxScale
        {
            set { _maxscale = value + 0.01 ; }
            get { return _maxscale; }
        }

        public double ThumbHeight
        {
            set
            {
                _thumbheight = value;
                _thumbimage.Height = value;
                _bordercolor.Height = value + 12;
                _itemgrid.Height = value;
            }
            get { return _thumbheight; }
        }

        public double ThumbWidth
        {
            set
            {
                _thumbwidth = value;
                _thumbimage.Width = value;
                _bordercolor.Width = value + 12;
                _itemgrid.Width = value;
            }
            get { return _thumbwidth; }
        }

        public double BorderHeight
        {
            set
            {
                _borderheight = value;
                Height = value;
                //_borderimage.Height = value;

                _transform.CenterY = value / 2;
            }
            get { return _thumbheight; }
        }

        public double BorderWidth
        {
            set
            {
                _borderwidth = value;
                Width = value;
                //_borderimage.Width =  value  ;

                _transform.CenterX = value / 2;
            }
            get { return _thumbwidth; }
        }


        public double InitialPosition
        {
            set
            {
                _initialposition = value;
                if (!_isopen)
                    _transform.TranslateX = value;
            }
            get { return _initialposition; }
        }

        public double FinalPosition
        {
            set
            {
                _finalposition = value;
                if (_isopen && !_isfull)
                    _transform.TranslateX = value;
            }
            get { return _finalposition; }
        }

        public double FullPositionX
        {
            set
            {
                _fullpositionx = value;
            }
            get { return _fullpositionx; }
        }

        public double FullPositionY
        {
            set
            {
                _fullpositiony = value;
            }
            get { return _fullpositiony; }
        }

        public bool IsOpen
        {
            set { _isopen = value; }
            get { return _isopen; }
        }

        public bool IsFull
        {
            set { _isfull = value; }
            get { return _isfull; }
        }

        public bool IsManipulating
        {
            set { _touches = 0; }
            get { return true; }
        }


        //private: 

        //Color of the border brush
        SolidColorBrush _borderbrushcolor;

        int _itemnumber;
        double _initialangle, _maxscale;
        PageDataSource _datasource;
        string _bordersource;

        double _thumbheight, _thumbwidth, _borderheight, _borderwidth;
        double _initialposition, _finalposition;
        double _fullpositionx, _fullpositiony;

        bool _isopen, _isfull, _isselected;

        //auxiliar variables
        int _touches, _maxthreshold;
        bool _ismfull;

        #endregion

        #region Public Methods
        //public:
        public void LoadThumbSource()
        {
            if (_datasource.ImageContent != null)
                this._thumbimage.Source = _datasource.ImageContent;
            _titletext.Text = _datasource.Name;
            _descriptiontext.Text = _datasource.Description;
            _borderbrushcolor = new SolidColorBrush(_datasource.BorderColor);
            _bordercolor.BorderBrush = _borderbrushcolor;

            _datasource.PropertyChanged += datasourcePropertyChanged;
        }

        public void LoadBorder() { }

        public void LoadFullSource() { }

        public void DeleteFullSource()
        {
            Canvas.SetZIndex(_itemgrid, 0);
            _itemimage = null;
            _itemgrid.Children.Clear();
            _touches = 0;
        }

        public void ItemManipulationCompleted()
        {
            if (_transform.ScaleX < _maxscale / 2)
                AnimateToThumb();
            else
                AnimateToFull();
        }

        public void AnimateToOpen()
        {
            _translateXanimation.To = _finalposition;
            _rotateanimation.To = 0.0;
            _rotatestory.Begin();
            _translatestory.Begin();
            _isopen = true;
        }

        public void AnimateToClose()
        {
            _translateXanimation.To = _initialposition;
            _rotateanimation.To = _initialangle;
            _rotatestory.Begin();
            _translatestory.Begin();
            _isopen = false;
        }

        public void SetToOpen()
        {
            _transform.TranslateX = _finalposition;
            _transform.TranslateY = 0.0;
            _transform.Rotation = 0.0;
            _transform.ScaleX = 1.0;
            _transform.ScaleY = 1.0;
            _isopen = true;
            _texttransform.TranslateY = 180.0;//text position
            _textpanel.Opacity = 1.0;
            DeleteFullSource();
        }

        public void SetToClose()
        {
            _transform.TranslateX = _initialposition;
            _transform.TranslateY = 0.0;
            _transform.Rotation = _initialangle;
            _transform.ScaleX = 1.0;
            _transform.ScaleY = 1.0;
            _isopen = false;
            _texttransform.TranslateY = 0.0;
            _textpanel.Opacity = 0.0;
            DeleteFullSource();
        }

        public void AnimateToFull()
        {
            ZIndex = _maxthreshold;
            _translatexanimation1.To = _fullpositionx;
            _translateyanimation1.To = _fullpositiony;
            _rotateanimation1.To = 0.0;
            _scalexanimation1.To = _maxscale + 1.5;
            _scaleyanimation1.To = _maxscale + 1.5;
            _translatestory1.Begin();
            _rotatestory1.Begin();
            _scalestory1.Begin();
            StackItemFullAnimationStarted(this, _chapter, _section, _itemnumber);
            _ismfull = true;
        }

        public void SetToFull()
        {
            _transform.TranslateX = _fullpositionx;
            _transform.TranslateY = _fullpositiony;
            _transform.Rotation = 0.0;
            _transform.ScaleX = _maxscale;
            _transform.ScaleY = _maxscale;
            _isfull = true;
            //LoadFullSource();
            ZIndex = _maxthreshold;
        }

        public void AnimateToThumb()
        {
            ZIndex = _maxthreshold;
            _translatexanimation1.To = _finalposition;
            _translateyanimation1.To = 0.0;
            _rotateanimation1.To = 0.0;
            _scalexanimation1.To = 1.0;
            _scaleyanimation1.To = 1.0;
            _translatestory1.Begin();
            _rotatestory1.Begin();
            _scalestory1.Begin();
            StackItemThumbAnimationStarted(this, _chapter, _section, _itemnumber);
        }

        public void SetToThumb()
        {
            _transform.TranslateX = _finalposition;
            _transform.TranslateY = 0.0;
            _transform.Rotation = 0.0;
            _transform.ScaleX = 1.0;
            _transform.ScaleY = 1.0;
            _isfull = false;
            ZIndex = _maxthreshold - _itemnumber;
        }

        public void ShowText()
        {
            animate2double(180.0);//text position
            animate2opacity(1.0);
        }

        public void HiddeText()
        {
            if (_textpanel.Opacity > 0.5)
            {
                animate2double(0.0);
                //animate2opacity(0.0);
                _textpanel.Opacity = 0.0;
            }
        }

        #endregion

        #region Private Methods
        //private:
        private void initproperties()
        {
            _itemnumber = 0;
            _initialangle = 0.0;
            _datasource = null;
            _bordersource = null;

            _thumbheight = 0;
            _thumbwidth = 0;
            _borderheight = 0;
            _borderwidth = 0.0;
            _initialposition = 0;
            _finalposition = 0;

            _isopen = false;
            _isfull = false;

            _touches = 0;
            _maxthreshold = 100;

            _ismfull = false;
        }

        private void animatecolor()
        {
            Storyboard story = new Storyboard();
            ColorAnimation animation = new ColorAnimation();

            animation.Duration = TimeSpan.FromMilliseconds(850);
            story.Children.Add(animation);
            animation.EnableDependentAnimation = true;

            Storyboard.SetTarget(animation, _bordercolor);
            Storyboard.SetTargetProperty(animation, "(Border.BorderBrush).(SolidColorBrush.Color)");

            animation.To = _datasource.BorderColor;
            story.Begin();
        }

        private void animate2double(double to)
        {
            Storyboard story = new Storyboard();
            DoubleAnimation animation = new DoubleAnimation();

            animation.Duration = TimeSpan.FromMilliseconds(450);
            story.Children.Add(animation);
            animation.EnableDependentAnimation = true;

            Storyboard.SetTarget(animation, _texttransform);
            Storyboard.SetTargetProperty(animation, "TranslateY");

            animation.To = to;
            story.Begin();
        }

        private void animate2opacity(double to)
        {
            Storyboard story = new Storyboard();
            DoubleAnimation animation = new DoubleAnimation();

            animation.Duration = TimeSpan.FromMilliseconds(450);
            story.Children.Add(animation);
            animation.EnableDependentAnimation = true;
            Storyboard.SetTarget(animation, _textpanel);
            Storyboard.SetTargetProperty(animation, "Opacity");

            animation.To = to;
            story.Begin();
        }

        #endregion

        #region Events Methods

        //private:
        void StackItem_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _touches += 1;
            if (_touches > 1 && _isopen)
            {
                StackItemSelected(this, _itemnumber);
                _textpanel.Opacity = 0.0;
            }
        }

        void StackItem_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _touches = 0;
        }

        void StackItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (_isopen)
            {
                StackItemTapped(this, _itemnumber);
                ZIndex = _maxthreshold;
                _textpanel.Opacity = 0.0;
                //LoadFullSource();
            }
            _touches = 0;
        }

        void StackItem_StoryboardCompleted(object sender, object e)
        {
            _touches = 0;
        }

        void StackItem_StoryboardFullCompleted(object sender, object e)
        {
            _touches = 0;

            if (_ismfull)
            {
                _scalexanimation1.To = _maxscale;
                _scaleyanimation1.To = _maxscale;
                //_borderimage.Opacity = 0.0;
                _scalestory1.Begin();
                _ismfull = false;
            }
            else
            {
                if (_transform.ScaleX > 1.0)
                {
                    ZIndex = _maxthreshold;
                    _isfull = true;
                    StackItemFullAnimationCompleted(this, _chapter, _section, _itemnumber);
                    _textpanel.Opacity = 0.0;
                }
                else
                {
                    ZIndex = _maxthreshold - _itemnumber;
                    _isfull = false;
                    StackItemThumbAnimationCompleted(this, _chapter, _section, _itemnumber);
                    _textpanel.Opacity = 1.0;
                    DeleteFullSource();
                    //_borderimage.Opacity = 1.0;
                }
            }
        }

        void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            animatecolor();
        }


        void datasourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ImageSource")
            {
                if (_datasource.ImageContent != null)
                    _thumbimage.Source = _datasource.ImageContent;
            }

            if (e.PropertyName == "BorderColor")
            {
                animatecolor();
            }
        }

        #endregion

        #region Animations

        //private :  
        //open close
        Storyboard _translatestory;
        DoubleAnimation _translateXanimation;
        Storyboard _rotatestory;
        DoubleAnimation _rotateanimation;

        //to full
        Storyboard _translatestory1;
        DoubleAnimation _translatexanimation1;
        DoubleAnimation _translateyanimation1;

        Storyboard _scalestory1;
        DoubleAnimation _scalexanimation1;
        DoubleAnimation _scaleyanimation1;

        Storyboard _rotatestory1;
        DoubleAnimation _rotateanimation1;
        void inititemanimations()
        {

            _translatestory = new Windows.UI.Xaml.Media.Animation.Storyboard();
            _translateXanimation = new Windows.UI.Xaml.Media.Animation.DoubleAnimation();
            _translateXanimation.Duration = TimeSpan.FromMilliseconds(350);
            _translatestory.Children.Add(_translateXanimation);
            Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(_translateXanimation, _transform);
            Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(_translateXanimation, "TranslateX");
            Windows.UI.Xaml.Media.Animation.QuinticEase ease1 = new Windows.UI.Xaml.Media.Animation.QuinticEase();
            ease1.EasingMode = Windows.UI.Xaml.Media.Animation.EasingMode.EaseOut;
            _translateXanimation.EasingFunction = ease1;

            _rotatestory = new Windows.UI.Xaml.Media.Animation.Storyboard();
            _rotateanimation = new Windows.UI.Xaml.Media.Animation.DoubleAnimation(); ;
            _rotateanimation.Duration = TimeSpan.FromMilliseconds(350);
            _rotatestory.Children.Add(_rotateanimation);
            Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(_rotateanimation, _transform);
            Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(_rotateanimation, "Rotation");
            _translatestory.Completed += StackItem_StoryboardCompleted;



            _translatestory1 = new Windows.UI.Xaml.Media.Animation.Storyboard();
            _translatexanimation1 = new Windows.UI.Xaml.Media.Animation.DoubleAnimation();
            _translatexanimation1.Duration = TimeSpan.FromMilliseconds(400);
            _translatestory1.Children.Add(_translatexanimation1);
            Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(_translatexanimation1, "TranslateX");
            Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(_translatexanimation1, _transform);

            _translateyanimation1 = new Windows.UI.Xaml.Media.Animation.DoubleAnimation();
            _translateyanimation1.Duration = TimeSpan.FromMilliseconds(400);
            _translatestory1.Children.Add(_translateyanimation1);
            Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(_translateyanimation1, "TranslateY");
            Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(_translateyanimation1, _transform);

            _scalestory1 = new Windows.UI.Xaml.Media.Animation.Storyboard();
            _scalexanimation1 = new Windows.UI.Xaml.Media.Animation.DoubleAnimation();
            _scalexanimation1.Duration = TimeSpan.FromMilliseconds(400);
            _scalestory1.Children.Add(_scalexanimation1);
            Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(_scalexanimation1, "ScaleX");
            Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(_scalexanimation1, _transform);

            Windows.UI.Xaml.Media.Animation.QuinticEase easex = new Windows.UI.Xaml.Media.Animation.QuinticEase();
            easex.EasingMode = Windows.UI.Xaml.Media.Animation.EasingMode.EaseInOut;
            //_scalexanimation1.EasingFunction = easex ;

            _scaleyanimation1 = new Windows.UI.Xaml.Media.Animation.DoubleAnimation();
            _scaleyanimation1.Duration = TimeSpan.FromMilliseconds(400);
            _scalestory1.Children.Add(_scaleyanimation1);
            Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(_scaleyanimation1, "ScaleY");
            Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(_scaleyanimation1, _transform);
            Windows.UI.Xaml.Media.Animation.QuinticEase easey = new Windows.UI.Xaml.Media.Animation.QuinticEase();
            easey.EasingMode = Windows.UI.Xaml.Media.Animation.EasingMode.EaseInOut;
            //_scaleyanimation1.EasingFunction = easey ;

            _rotatestory1 = new Windows.UI.Xaml.Media.Animation.Storyboard();
            _rotateanimation1 = new Windows.UI.Xaml.Media.Animation.DoubleAnimation();
            _rotateanimation1.Duration = TimeSpan.FromMilliseconds(400);
            _rotatestory1.Children.Add(_rotateanimation1);
            Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(_rotateanimation1, "Rotation");
            Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(_rotateanimation1, _transform);

            _scalestory1.Completed += StackItem_StoryboardFullCompleted;
        }

        #endregion


    }
}
