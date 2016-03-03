using MLearning.Store.Components;
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
    public delegate void LOReaderPagedChangedEventHandler(object sender);
    public delegate void LOReaderRightTappedEventHandler(object sender);
    public delegate void LOReaderAnimate2ThumbnailEventHandler(object sender);
    public delegate void LOReaderOpenPageAtEventHandler(object sender, int chapter, int section, int page) ;
    public sealed partial class LOReaderScroll : Grid 
    {
        double DeviceHeight = 900.0, DeviceWidth = 1600.0;
        public event LOReaderPagedChangedEventHandler LOReaderPagedChanged;
        public event LOReaderAnimate2ThumbnailEventHandler LOReaderAnimate2Thumbnail;
        public event LOReaderRightTappedEventHandler LOReaderRightTapped;
        public event LOReaderOpenPageAtEventHandler LOReaderOpenPageAt;

        public LOReaderScroll()
        {
            initBackButton();
            init();
        }

        ScrollViewer _mainscroll;
        Grid _contentpanel;
        CompositeTransform _ctrasnform;

        int _pointers = 0, _currentindex, _numberofitems;
        bool _forcemanipulation2end = false;
        double _initthreshold = 0.0, _finalthreshold = 0.0;
        double _currentposition = -0.0;

        List<LOReaderScrollElement> _elementslist = new List<LOReaderScrollElement>();

        void init()
        {
            Height = DeviceHeight;
            Width = DeviceWidth;
            Background = new SolidColorBrush(Colors.Transparent);

            

            _mainscroll = new ScrollViewer()
            {
                VerticalScrollMode = ScrollMode.Disabled,
                VerticalScrollBarVisibility = ScrollBarVisibility.Disabled,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden,
                HorizontalScrollMode = ScrollMode.Disabled,
                Width = DeviceWidth,
                Height = DeviceHeight,
                Background = new SolidColorBrush(Colors.Transparent)
            };
            Children.Add(_mainscroll);

            _contentpanel = new Grid()
            {
                ManipulationMode = Windows.UI.Xaml.Input.ManipulationModes.All,
                Height = DeviceHeight, 
                Background = new SolidColorBrush(Colors.Transparent)
               // Orientation = Orientation.Horizontal
            };
            _mainscroll.Content = _contentpanel;
             
            _ctrasnform = new CompositeTransform();
            _contentpanel.RenderTransform = _ctrasnform;

            //events
            //ManipulationMode = ManipulationModes.All;
            //PointerPressed += LOReaderView_PointerPressed;
            //PointerCanceled += LOReaderView_PointerCanceled;
            //PointerReleased += LOReaderView_PointerReleased;
            //ManipulationStarted += LOReaderView_ManipulationStarted;
            //ManipulationDelta += LOReaderView_ManipulationDelta;
            //ManipulationCompleted += LOReaderView_ManipulationCompleted;
            //ManipulationInertiaStarting += LOReaderView_ManipulationInertiaStarting;
            //rigth tapped
            RightTapped += LOReaderView_RightTapped;
             
        }

        Image backimage;
        void initBackButton()
        {
            backimage = new Image()
            {
                Width = 60,
                Height = 60,
                VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Top,
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left,
                Stretch = Stretch.UniformToFill, 
                RenderTransform = new TranslateTransform() { X=40, Y=40}
            };
            //backimage.Source = new BitmapImage(new Uri("ms-appx:///Resources/back_icon.png"));
            backimage.Source = new BitmapImage(new Uri("ms-appx:///Resources/muro/tareas/btn_back.png"));
            Children.Add(backimage);
            Canvas.SetZIndex(backimage, 100);

            backimage.Tapped += backimage_Tapped;

            init_arrows();
        }

        Image get_arrow_image(string source)
        {
            Image image = new Image()
            {
                Width = 60,
                Height = 80,
                VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center, 
                Stretch = Stretch.UniformToFill,
                //RenderTransform = new TranslateTransform() { X = 40, Y = 40 },
                Source = new BitmapImage(new Uri(source))
            }; 
            return image;
        }

        Image rightimage, leftimage;

        void init_arrows()
        {
            rightimage = get_arrow_image("ms-appx:///Resources/adelante.png");
            rightimage.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Right;
            rightimage.Opacity = 0.8;
            //rightimage.RenderTransform = new CompositeTransform() { Rotation = 180, CenterX = 20, CenterY = 20};
            Children.Add(rightimage);
            Canvas.SetZIndex(rightimage, 1000);

            leftimage = get_arrow_image("ms-appx:///Resources/atras.png");
            leftimage.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left;
            leftimage.Opacity = 0.8;
            Children.Add(leftimage);
            Canvas.SetZIndex(leftimage, 1000);

            rightimage.Tapped += rightimage_Tapped;
            leftimage.Tapped += leftimage_Tapped;
        }

        void leftimage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (!_islocked)
            {
                _currentindex--;
                animate2index(_currentindex);
            }
        }

        void rightimage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (!_islocked)
            {
                _currentindex++;
                animate2index(_currentindex);
            }
        }

        void backimage_Tapped(object sender, TappedRoutedEventArgs e)
        {

            if (_elementslist[_currentindex].IsLoaded)
            {
                if (_ismanipulationenable && _elementslist[_currentindex].IsLoaded)
                {
                    animate_opacity(0.0);
                   
                } 
            }
            _pointers = 0;
        }

        private void LOReaderView_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (_elementslist[_currentindex].IsLoaded)
            {
                Opacity = 0.0;
                _pointers = 0;
                _elementslist[_currentindex].IsLoadingVisible = false;
                _elementslist[_currentindex].clearpage();
                if (LOReaderRightTapped != null)
                    LOReaderRightTapped(this);
            }
        }


        void loadsource()
        {
            _numberofitems = _source.Count;
            _contentpanel.Width = _numberofitems * DeviceWidth;
            for (int i = 0; i < _source.Count; i++)
            {
                LOReaderScrollElement elem = new LOReaderScrollElement();
                 elem.Source = _source[i];
                 elem.PositionX = i * DeviceWidth;
                _contentpanel.Children.Add(elem);
                _elementslist.Add(elem);
                elem.PropertyChanged += elem_PropertyChanged;
                elem.MLManipulationCompleted += elem_MLManipulationCompleted;
                elem.MLManipulationDelta += elem_MLManipulationDelta;
                elem.MLManipulationStarted += elem_MLManipulationStarted;
            } 
            _finalthreshold = -1.0 * _numberofitems * DeviceWidth;
        }

        void elem_MLManipulationStarted(object sender, int touches)
        {
            _pointers = touches;
        }

        void elem_MLManipulationDelta(object sender, MLManipulationArgs args)
        {
            if (_pointers > 1)
            {
                this.Opacity = 0.0;
                _elementtransform.TranslateX += args.X;
                _elementtransform.TranslateY += args.Y;
                _elementtransform.ScaleX *= args.Scale;
                _elementtransform.ScaleY *= args.Scale;
                _elementtransform.Rotation += args.Rotate;
            }
            else
            {
                if (_currentposition < _initthreshold && _currentposition > _finalthreshold) { delta_proportion = 1.0; }
                else { delta_proportion = 0.4; }
                _currentposition += (args.X * delta_proportion);
                _page_translation += (args.X * delta_proportion);
                _ctrasnform.TranslateX = _currentposition; 
            }
        }

        void elem_MLManipulationCompleted(object sender, double v)
        {
            if (_pointers > 1)
            { 
                    if (LOReaderAnimate2Thumbnail != null)
                        LOReaderAnimate2Thumbnail(this);
            }
            else
            {
                if (Math.Abs(v) > 3.5)
                {
                    if (v > 0) _currentindex -= 1;
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
        }

        void elem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Selected")
            {
                _ismanipulationenable = false;
                _islocked = true;
                backimage.Opacity = 0.0;
                leftimage.Opacity = 0.0;
                rightimage.Opacity = 0.0;
            }

            if (e.PropertyName == "Released")
            {
                _ismanipulationenable = true;
                _islocked = false;
                _pointers = 0;
                backimage.Opacity = 1.0;
                leftimage.Opacity = 0.8;
                rightimage.Opacity = 0.8;
            }
        }

        #region Event Functions

        private void LOReaderView_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _pointers += 1;
        }

        private void LOReaderView_PointerCanceled(object sender, PointerRoutedEventArgs e)
        {
            _forcemanipulation2end = true;
        }

        private void LOReaderView_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (!_maniuplation_done) { _pointers = 0; _maniuplation_done = false; }
        }

        private void LOReaderView_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            _maniuplation_done = true;
        }


        bool _deltatested = false, _maniuplation_done = false;
        bool _ismanipulationenable = true, _islocked = false;
        bool is_vertical = false, is_horizontal = false;

        double _page_translation = 0.0, delta_proportion = 1.0;
        int _lastindex = 0;

        private void LOReaderView_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (_forcemanipulation2end || (e.IsInertial && is_horizontal)) e.Complete();
            if (_ismanipulationenable && !_islocked)
            {
                if (_pointers < 2)
                {
                    if (!_deltatested)
                    {
                        _deltatested = true;
                        if (Math.Abs(e.Delta.Translation.Y / 2) < Math.Abs(e.Delta.Translation.X)) { is_horizontal = true; }
                        else { is_vertical = true; _ismanipulationenable = false; }
                    }

                    if (is_horizontal)
                    { 
                        if (_currentposition < _initthreshold && _currentposition > _finalthreshold) { delta_proportion = 1.0; }
                        else { delta_proportion = 0.4; }
                        _currentposition += (e.Delta.Translation.X * delta_proportion);
                        _page_translation += (e.Delta.Translation.X * delta_proportion);
                        _ctrasnform.TranslateX = _currentposition; 
                    }

                    if (is_vertical)
                    {
                        //_elementslist[_currentindex].DeltaY = e.Delta.Translation.Y;
                    }
                }
                else 
                {
                    //manipulation for the element to thumb
                    /*this.Opacity = 0.0;
                    _elementtransform.TranslateX += e.Delta.Translation.X;
                    _elementtransform.TranslateY += e.Delta.Translation.Y;
                    _elementtransform.ScaleX *= e.Delta.Scale;
                    _elementtransform.ScaleY *= e.Delta.Scale;
                    _elementtransform.Rotation += e.Delta.Rotation;*/
                }
            }
        }



        private void LOReaderView_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (_pointers > 1)
            {
                /*if (_ismanipulationenable && _elementslist[_currentindex].IsLoaded)
                    if (LOReaderAnimate2Thumbnail != null)
                        LOReaderAnimate2Thumbnail(this);*/
            }
            else
            {
                if (Math.Abs(e.Velocities.Linear.X) > 3.5)
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
            _maniuplation_done = false;
            _deltatested = false;
            is_horizontal = false;
            is_vertical = false;
            if (!_islocked)
                _ismanipulationenable = true;
        }

        private void LOReaderView_ManipulationInertiaStarting(object sender, ManipulationInertiaStartingRoutedEventArgs e)
        {
            
        }


        

        bool do_reset = true;

        void animate2index(int index)
        {
            if (_currentindex < 0) _currentindex = 0;
            if (_currentindex >= _numberofitems) _currentindex = _numberofitems - 1;
            _currentposition = -1.0 * DeviceWidth * _currentindex;//- _currentindex; only for try
            animate2double(_currentposition); 
            _forcemanipulation2end = false;
            _deltatested = false;

            if (_currentindex != _lastindex) do_reset = true;

            _lastindex = _currentindex;
            _page_translation = 0.0;
            //update page
            if (do_reset)
            {
                ChapterIndex = _source[_currentindex].LOIndex;
                SectionIndex = _source[_currentindex].StackIndex;
                PageIndex = _source[_currentindex].PageIndex;
                if (LOReaderPagedChanged != null)
                    LOReaderPagedChanged(this);
            }

            //solo visible cuando estan _lastindex imagenes de avanazar
            if (_currentindex == 0) leftimage.Opacity = 0.0; else leftimage.Opacity = 1.0;
            if (_currentindex == _numberofitems - 1) rightimage.Opacity = 0.0; else rightimage.Opacity = 1.0;
            
        }

        double _currentopacity = 0.0 ;
        public void animate_opacity(double to)
        {
            _currentopacity = to;
            Storyboard story = new Storyboard();

            DoubleAnimation animation = new DoubleAnimation();
            animation.Duration = TimeSpan.FromMilliseconds(350);
            animation.EnableDependentAnimation = true;

            Storyboard.SetTarget(animation, this);
            Storyboard.SetTargetProperty(animation, "Opacity");

            animation.To = _currentopacity;
            story.Children.Add(animation);
            
            story.Completed+=storyop_Completed;

            story.Begin();
        }

        private void storyop_Completed(object sender, object e)
        { 
            if(_currentopacity ==0.0)
            {
                if (LOReaderRightTapped != null)
                    LOReaderRightTapped(this);
                Opacity = 0.0;
            }
        }
         

        void animatepage2double(double to)
        {
            Storyboard story = new Storyboard();

            DoubleAnimation animation = new DoubleAnimation();
            animation.Duration = TimeSpan.FromMilliseconds(350);

            Storyboard.SetTarget(animation, _ctrasnform);
            Storyboard.SetTargetProperty(animation, "TranslateX");

            animation.To = to;
            story.Children.Add(animation);
            story.Begin();
        }

        void animate2double(double to)
        {
            Storyboard story = new Storyboard();

            DoubleAnimation animation = new DoubleAnimation();
            animation.Duration = TimeSpan.FromMilliseconds(360);
            animation.EasingFunction = new QuinticEase() { EasingMode = EasingMode.EaseOut };

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
            if (do_reset)
                //resetpageat(_currentindex);
                resetpage_bytes(_currentindex);
            do_reset = false;
        }


        public  void LoadCurrentPage(LOPageSource page)
        {
            if (do_reset)
            {
                _elementslist[_currentindex].resetpage(page);
                do_reset = false;
            }
        }

         

        #endregion

        #region properties and public functions

        public void SetVisible()
        {
            Opacity = 1.0;
        }

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


        public void SetAt(int lo, int stack, int page)
        {  
            int pos = 0;
            foreach (var item in _source)
            {
                if (item.LOIndex == lo && item.StackIndex == stack && item.PageIndex == page)
                {
                    pos = item.Index;
                    break;
                }
            }
            _currentindex = pos;
            if (_currentindex < 0) _currentindex = 0;
            if (_currentindex >= _numberofitems) _currentindex = _numberofitems - 1;
            _currentposition = -1.0 * DeviceWidth * _currentindex ;//- _currentindex; only for try
             
            //animate2double(_currentposition); 
            _ctrasnform.TranslateX = _currentposition;
            _forcemanipulation2end = false;
            _deltatested = false;
            _lastindex = _currentindex;
            _page_translation = 0.0;
            //update page
            ChapterIndex = _source[_currentindex].LOIndex;
            SectionIndex = _source[_currentindex].StackIndex;
            PageIndex = _source[_currentindex].PageIndex; 
            
             
        }


        public void LoadCurrentPage()
        {
            //resetpageat(_currentindex);
            resetpage_bytes(_currentindex);
            //solo visible cuando estan _lastindex imagenes de avanazar
            if (_currentindex == 0) leftimage.Opacity = 0.0; else leftimage.Opacity = 1.0;
            if (_currentindex == _numberofitems - 1) rightimage.Opacity = 0.0; else rightimage.Opacity = 1.0;
        }


        public void ClearAllPages()
        {
            for (int i = 0; i < _elementslist.Count; i++)
            {
                _elementslist[i].clearpage();
                _pointers = 0;
                _deltatested = false;
            }
        }

        public void resetpage_bytes(int index)
        {
            LOReaderOpenPageAt(this, _elementslist[index].Source.LOIndex, _elementslist[index].Source.StackIndex, _elementslist[index].Source.PageIndex);
            for (int i = index - 1; i <= index + 1; i++)
            {
                if (i >= 0 && i < _numberofitems)
                {
                    //LOReaderOpenPageAt(this, _elementslist[i].Source.LOIndex, _elementslist[i].Source.StackIndex, _elementslist[i].Source.PageIndex);
                    _elementslist[i].load_page();
                }
            }
            if (index - 2 >= 0) _elementslist[index - 2].clearpage();
            if (index + 2 < _numberofitems) _elementslist[index + 2].clearpage();
        }

        public void resetpageat(int index)
        {

            if (index - 3 >= 0 && index-3 < _numberofitems) _elementslist[index - 3].clearpage();


            for (int i = -2; i <= 2; i++)
            {
                if (index + i >= 0 && index +i < _numberofitems)
                {
                    _elementslist[index + 1].ResetValues();
                    if (!_elementslist[index + 1].IsLoaded && LOReaderOpenPageAt != null)
                        LOReaderOpenPageAt(this, _elementslist[index +i].Source.LOIndex, _elementslist[index +i].Source.StackIndex, _elementslist[index+i].Source.PageIndex);
                    else _elementslist[index+i].IsLoadingVisible = false;
                }
            }
           
            if (index + 3 >= 0 && index +3< _numberofitems) _elementslist[index + 3].clearpage();

            _pointers = 0;
            _forcemanipulation2end = false;
            _maniuplation_done = false;
            _deltatested = false;
            if (!_islocked)
                _ismanipulationenable = true;
        }

        #endregion


        
 
    }

}
