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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Media.Imaging;
using System;


namespace StackView
{
    public delegate void ScrollViewItemChangedEventHandler(object sender);
    public sealed partial class ControlScrollView : Grid
    {
        //public:
        public ControlScrollView()
        {
            _imagelist = new List<Image>();
            initcontrols();
            initanimationproperties();
            _actualindex = 0;
           // Background = new SolidColorBrush(Colors.Black);
        }

        event ScrollViewItemChangedEventHandler ScrollViewItemChanged;

        #region Controls
        //private:
        //background
        Image _backimage;

        //Scroll z-index =  0
        ScrollViewer _scrollviewer;
        Grid _elementspanel;
        CompositeTransform _paneltransform;

        //Fog Level z-index  =  1
        Grid _foglevel;
        Image _fogimage;

        //Color Level z-index =  2
        Image _colorlevelimage;

        void initcontrols()
        {
            //level zero -  scroll backgrounds
            _elementspanel = new Grid();
            _elementspanel.Width = 1600;
            _elementspanel.Height = 900;
            _elementspanel.Background = new SolidColorBrush(Colors.Transparent);
            //_elementspanel.Orientation = Orientation.Horizontal ; 
            _elementspanel.ManipulationMode = ManipulationModes.All;
            _scrollviewer = new ScrollViewer();
            Children.Add(_scrollviewer);
            _scrollviewer.HorizontalScrollMode = Windows.UI.Xaml.Controls.ScrollMode.Enabled;
            _scrollviewer.HorizontalScrollBarVisibility = Windows.UI.Xaml.Controls.ScrollBarVisibility.Hidden;
            _scrollviewer.VerticalScrollMode = Windows.UI.Xaml.Controls.ScrollMode.Disabled;
            _scrollviewer.VerticalScrollBarVisibility = Windows.UI.Xaml.Controls.ScrollBarVisibility.Disabled;
            _scrollviewer.ZoomMode = ZoomMode.Disabled;
            _scrollviewer.Content = _elementspanel;
            _paneltransform = new CompositeTransform(); //transform fro panel scroll
            _elementspanel.RenderTransform = _paneltransform;



            //Fog Level 
            _foglevel = new Grid();
            Children.Add(_foglevel);
            _fogimage = new Image();
            _fogimage.Source = new BitmapImage(new Uri("ms-appx:///levelzero/marco.png"));
            //_foglevel.Children.Append(_fogimage);

            _foglevel.Opacity = 0.95;


            ///Color level  - image for color level
            _colorlevelimage = new Image();
            _colorlevelimage.Opacity = 0.75;
            //Children.Append(_colorlevelimage);

            _currentitem = 0;
        }

        public void settoindex(int index)
        {
            animateimage(0.0, _actualindex);
            animateimage(1.0, index);// _chaptercontroller.CurrentChapter);
            _actualindex = index;
            //animateimage()
        }

        #endregion

        #region Properties

        //public:

        public CompositeTransform ScrollTransform
        {
            set { }
            get { return _paneltransform; }
        }

        public int ItemsNumber
        {
            set
            {
                _itemsnumber = value;
                loadscroll();
            }
            get { return _itemsnumber; }
        }

        public int CurrentItem
        {
            set
            { _currentitem = value; }
            get { return _currentitem; }
        }

        public ObservableCollection<BitmapImage> Backgrouds
        {
            get { return _backgrouds; }
            set { _backgrouds = value; }
        }

        private BookDataSource _source;

        public BookDataSource Source
        {
            get { return _source; }
            set { _source = value; _itemsnumber = _source.Chapters.Count; loadscroll2(); }
        }
        


        //private:
        //Windows.Foundation.Collections.IVector<string> _backgrouds  ;
        private ObservableCollection<BitmapImage> _backgrouds;
        //Windows.Foundation.Collections.IVector<string> _backcolors ;
        //private ObservableCollection<string> _backcolors;
        string _fogsource;
        int _itemsnumber, _currentitem;
        int _actualindex;
        List<Image> _imagelist;

        #endregion

        #region Methods
        //public:
        public void ResetBackground(int citem)
        { }

        public void AnimateToCurrentItem()
        {
            if (_currentitem > _itemsnumber - 1)
                _currentitem = _itemsnumber - 1;
            if (_currentitem < 0)
                _currentitem = 0;
            _panelanimation.To = -1600.0 * _currentitem;
            _panelstory.Begin();
            ///ResetBackground(_currentitem) ;
        }
        //private:

        void loadscroll()
        {
            for (int i = 0; i < _itemsnumber; i++)
            {
                Image img = new Image();
                img.Source = _backgrouds[i]; ///new BitmapImage(new Uri("ms-appx:///roadsdata/back" + (int)(i + 1) + ".png"));
                img.Width = 1600.0;
                img.Height = 900.0;
                img.Stretch = Stretch.Fill;
                img.Opacity = 0.0;
                _elementspanel.Children.Add(img);
                _imagelist.Add(img);
            }

            _imagelist[0].Opacity = 1.0;
        }

        void loadscroll2()
        {

            for (int i = 0; i < _itemsnumber; i++)
            {
                Image img = new Image();
                img.Source = _source.Chapters[i].BackgroundImage;
                img.Width = 1600.0;
                img.Height = 900.0;
                img.Stretch = Stretch.Fill;
                img.Opacity = 0.0;
                _source.Chapters[i].PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == "BackgroundImage")
                        {
                            var ch_source = s as ChapterDataSource;
                            img.Source = ch_source.BackgroundImage;
                        }
                    };
                _elementspanel.Children.Add(img);
                _imagelist.Add(img);
            }
            animateimage(1.0, 0);
           // _imagelist[0].Opacity = 1.0;
        }

        void ControlScrollView_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            
        }

        

        void loadimages()
        {
            _panelanimation.To = -1600.0 * _currentitem;
        }

        #endregion

        #region Panel Scroll Animations


        void animateimage(double to, int index)
        {


            Windows.UI.Xaml.Media.Animation.Storyboard story = new Windows.UI.Xaml.Media.Animation.Storyboard();
            Windows.UI.Xaml.Media.Animation.DoubleAnimation animation = new Windows.UI.Xaml.Media.Animation.DoubleAnimation();
            animation.Duration = TimeSpan.FromMilliseconds(650);
            story.Children.Add(animation);
            Windows.UI.Xaml.Media.Animation.CubicEase ease1 = new Windows.UI.Xaml.Media.Animation.CubicEase();
            //ease1.EasingMode = Windows.UI.Xaml.Media.Animation.EasingMode.EaseIn ;
            animation.EnableDependentAnimation = true;
            animation.EasingFunction = ease1;
            animation.To = to;
            Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(animation, "Opacity");
            Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(animation, _imagelist[index]);
            story.Begin();

        }

        //private:
        Storyboard _panelstory;
        DoubleAnimation _panelanimation;
        void initanimationproperties()
        {

            _panelstory = new Windows.UI.Xaml.Media.Animation.Storyboard();
            _panelanimation = new Windows.UI.Xaml.Media.Animation.DoubleAnimation();
            _panelanimation.Duration = TimeSpan.FromMilliseconds(350);
            _panelstory.Children.Add(_panelanimation);
            Windows.UI.Xaml.Media.Animation.CubicEase ease1 = new Windows.UI.Xaml.Media.Animation.CubicEase();
            //ease1.EasingMode = Windows.UI.Xaml.Media.Animation.EasingMode.EaseIn ;
            _panelanimation.EasingFunction = ease1;
            Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(_panelanimation, "TranslateX");
            Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(_panelanimation, _paneltransform);

            _panelstory.Completed += Storyboard_Completed_1;
        }
        void Storyboard_Completed_1(object sender, object e)
        { }

        #endregion

    }
}
