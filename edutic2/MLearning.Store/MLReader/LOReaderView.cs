using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;


namespace MLReader
{
     

    public sealed partial class LOReaderView : Grid
    {
        double DeviceHeight = 900.0, DeviceWidth = 1600.0;
        public LOReaderPagedChangedEventHandler LOReaderPagedChanged;
        public event LOReaderAnimate2ThumbnailEventHandler LOReaderAnimate2Thumbnail;


        public LOReaderView()
        {
            init();
            //tmpload();
        }

        LOPageViewer _pageview;
        Grid _gridpage;
        CompositeTransform _pagetransform;

        ScrollViewer _mainscroll;
        StackPanel _contentpanel;
        CompositeTransform _ctrasnform;
        int _pointers = 0, _currentindex, _numberofitems;
        bool _forcemanipulation2end = false;
        double _initthreshold = 0.0, _finalthreshold = 0.0;
        double _currentposition = -0.0;

        void init()
        {
            Height = DeviceHeight;
            Width = DeviceWidth;

            _mainscroll = new ScrollViewer()
            {
                VerticalScrollMode = ScrollMode.Disabled,
                VerticalScrollBarVisibility = ScrollBarVisibility.Disabled,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden,
                HorizontalScrollMode = ScrollMode.Disabled,
                Width = DeviceWidth,
                Height = DeviceHeight
            };
            Children.Add(_mainscroll);

            _contentpanel = new StackPanel()
            {
                ManipulationMode = Windows.UI.Xaml.Input.ManipulationModes.All,
                Height = DeviceHeight,
                Orientation = Orientation.Horizontal
            };
            _mainscroll.Content = _contentpanel;

            _gridpage = new Grid() { Width = DeviceWidth, Height = DeviceHeight };
            Children.Add(_gridpage);
            //_gridpage.Opacity = 0.8;
            _pagetransform = new CompositeTransform();
            _gridpage.RenderTransform = _pagetransform;
            _pageview = new LOPageViewer();
            _gridpage.Children.Add(_pageview);

            _ctrasnform = new CompositeTransform();
            _contentpanel.RenderTransform = _ctrasnform;

            //events
            ManipulationMode = ManipulationModes.All;
            PointerPressed += LOReaderView_PointerPressed;
            PointerCanceled += LOReaderView_PointerCanceled;
            PointerReleased += LOReaderView_PointerReleased;
            ManipulationStarted += LOReaderView_ManipulationStarted;
            ManipulationDelta += LOReaderView_ManipulationDelta;
            ManipulationCompleted += LOReaderView_ManipulationCompleted;
            ManipulationInertiaStarting += LOReaderView_ManipulationInertiaStarting;
            //rigth tapped
            RightTapped += LOReaderView_RightTapped;
        }

        void LOReaderView_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (_currentindex == 0 && LOReaderAnimate2Thumbnail != null)
            {
                LOReaderAnimate2Thumbnail(this);
            }
        }


        #region properties

        private List<LOPageSource> _source;

        public List<LOPageSource> Source
        {
            get { return _source; }
            set { _source = value; loadsource(); }
        }


        private CompositeTransform _elementtransform = new CompositeTransform();

        public CompositeTransform ElementTransform
        {
            get { return _elementtransform; }
            set { _elementtransform = value; }
        }
        

        private int _chapterindex;

        public int ChapterIndex
        {
            get { return _chapterindex; }
            set { _chapterindex = value; }
        }


        private int _sectionindex;

        public int SectionIndex
        {
            get { return _sectionindex; }
            set { _sectionindex = value; }
        }

        private int _pageindex;

        public int PageIndex
        {
            get { return _pageindex; }
            set { _pageindex = value; }
        }


        #endregion

        #region functions

        void loadsource()
        {
            _numberofitems = _source.Count;
            for (int i = 0; i < _source.Count; i++)
            {
                Image img = new Image()
                {
                    Width = DeviceWidth,
                    Height = DeviceHeight,
                    Stretch = Windows.UI.Xaml.Media.Stretch.UniformToFill,
                    Source = _source[i].Cover
                };
                _contentpanel.Children.Add(img);
            }
            _pageview.Source = _source[0];
            _finalthreshold = -1.0 * _numberofitems * DeviceWidth;
        }
 


        void LOReaderView_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            //   _forcemanipulation2end = true;
        }

        void LOReaderView_PointerCanceled(object sender, PointerRoutedEventArgs e)
        {
            _forcemanipulation2end = true;
        }

        void LOReaderView_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _pointers += 1;
        }


        void LOReaderView_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
        }

        bool _deltatested = false;
        bool _ismanipulationenable = true;
        double _page_translation = 0.0;
        int _lastindex = 0;

        void LOReaderView_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (_forcemanipulation2end || e.IsInertial)
            {
                e.Complete();
            }

            if (_ismanipulationenable)
                if (_pointers < 2)
                {
                    if (_deltatested)
                    {
                        //_ctrasnform.TranslateX += e.Delta.Translation.X;
                        if (_currentposition < _initthreshold && _currentposition > _finalthreshold)
                        {
                            _currentposition += e.Delta.Translation.X;
                            _page_translation += e.Delta.Translation.X;
                        }
                        else
                        {
                            _currentposition += (e.Delta.Translation.X * 0.4);
                            _page_translation += (e.Delta.Translation.X * 0.4);
                        }

                    }
                    else
                    {
                        if (Math.Abs(e.Delta.Translation.Y / 2  ) < Math.Abs(e.Delta.Translation.X))
                        {
                            _deltatested = true;
                            _currentposition += e.Delta.Translation.X;
                            _page_translation += e.Delta.Translation.X;
                        }
                        else
                        {
                            _ismanipulationenable = false;
                            // _forcemanipulation2end = true;
                        }
                    }
                    _ctrasnform.TranslateX = _currentposition;
                    _pagetransform.TranslateX = _page_translation;
                }
                else
                {
                   //manipulation for the element to thumb
                }
        }

        void LOReaderView_ManipulationInertiaStarting(object sender, ManipulationInertiaStartingRoutedEventArgs e)
        {
        }

        void LOReaderView_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (_pointers > 1)
            {
            }
            else
            {
                if (Math.Abs(e.Velocities.Linear.X) > 4.5)
                {
                    if (e.Velocities.Linear.X > 0) _currentindex -= 1;
                    else _currentindex += 1;
                }
                else
                {
                    if (_currentposition < -1.0 * (DeviceWidth * _currentindex + DeviceWidth / 2.0))
                        _currentindex += 1;

                    if (_currentposition > -1.0 * (DeviceWidth * _currentindex - DeviceWidth / 2.0))
                        _currentindex -= 1;
                }
                animate2index(_currentindex);
            }

            _pointers = 0;
            _forcemanipulation2end = false;
            _deltatested = false;
            _ismanipulationenable = true;
        }

        void animate2index(int index)
        {
            if (_currentindex < 0) _currentindex = 0;
            if (_currentindex >= _numberofitems) _currentindex = _numberofitems - 1;
            _currentposition = -1.0 * DeviceWidth * _currentindex;
            animate2double(_currentposition);
            if (_lastindex == _currentindex) animatepage2double(0.0);
            if (_lastindex > _currentindex){ animatepage2double(1600.0); loadpageat(_currentindex);}
            if (_lastindex < _currentindex) { animatepage2double(-1600.0); loadpageat(_currentindex); }
            _forcemanipulation2end = false;
            _deltatested = false;
            _lastindex = _currentindex;
            _page_translation = 0.0;
            //update page
            ChapterIndex = _source[_currentindex].LOIndex;
            SectionIndex = _source[_currentindex].StackIndex;
            PageIndex = _source[_currentindex].PageIndex;
            if (LOReaderPagedChanged != null)
                LOReaderPagedChanged(this);

            
        }


        void loadpageat(int index)
        {
            _gridpage.Children.Clear();
            _gridpage.Background = new SolidColorBrush(Colors.Red);
            _pageview = new LOPageViewer();
            _pageview.Source = _source[index];
            _gridpage.Children.Add(_pageview);
            _pagetransform.TranslateX = 0.0;
        }

        void animatepage2double(double to)
        {
            Storyboard story = new Storyboard();

            DoubleAnimation animation = new DoubleAnimation();
            animation.Duration = TimeSpan.FromMilliseconds(350);

            Storyboard.SetTarget(animation, _pagetransform);
            Storyboard.SetTargetProperty(animation, "TranslateX");

            animation.To = to;
            story.Children.Add(animation);
            story.Begin();
        }

        void animate2double(double to)
        {
            Storyboard story = new Storyboard();

            DoubleAnimation animation = new DoubleAnimation();
            animation.Duration = TimeSpan.FromMilliseconds(350);

            Storyboard.SetTarget(animation, _ctrasnform);
            Storyboard.SetTargetProperty(animation, "TranslateX");

            animation.To = to;
            story.Children.Add(animation);
            story.Begin();
            story.Completed += story_Completed;

        }

        void story_Completed(object sender, object e)
        {
            _forcemanipulation2end = false;
            _deltatested = false;
        }

        #endregion

    }
}
