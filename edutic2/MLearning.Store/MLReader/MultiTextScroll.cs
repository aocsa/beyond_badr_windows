using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace MLReader
{
    public sealed partial class MultiTextScroll : Grid
    {

        double DeviceHeight = 900.0, DeviceWidth = 1600.0;

        public event ISlideElementSizeChangedEventHandler ISlideElementSizeChanged; 

        public MultiTextScroll()
        {
            init();
        }

        void init()
        {
            Width = DeviceWidth;
            Height = DeviceHeight;
            _elements = new List<ISlideElement>();
            //Scroll view
            _mainscroll = new ScrollViewer()
            {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollMode = ScrollMode.Enabled,
                Width = DeviceWidth,
                Height = DeviceHeight,
                ZoomMode = ZoomMode.Enabled
            };
            Children.Add(_mainscroll);

            _paneltransform = new CompositeTransform();

            _contentpanel = new StackPanel()
            {
                Orientation = Orientation.Vertical,
                Width = DeviceWidth,
                RenderTransform = _paneltransform 
            };
            _mainscroll.Content = _contentpanel;

        }

        #region Controls and variables

        ScrollViewer _mainscroll;
        StackPanel _contentpanel;
        CompositeTransform _paneltransform;

        double _currenttranslate = 0.0;
        int _currentindex = 0; 
        List<ISlideElement> _elements;

        #endregion


        #region properties
        public List<ISlideElement> Elements
        {
            get { return _elements; }
            set { _elements = value; }
        }
        


        private LOPageSource _source;

        public LOPageSource Source
        {
            get { return _source; }
            set { _source = value; initdatasource(); }
        }

        private double _proportion = 1.0;

        public double Proportion
        {
            get { return _proportion; }
            set { _proportion = value; }
        }

        private double _threshold;

        public double Threshold
        {
            get { return _threshold; }
            set { _threshold = value; }
        }


        private double _thresholddelta;

        public double ThresholdDelta
        {
            get { return _thresholddelta; }
            set
            {
                _thresholddelta = value;
                _paneltransform.TranslateY = _currenttranslate + TranslateDelta + ThresholdDelta * Proportion; 
            }
        }


        private double _translatedelta;

        public double TranslateDelta
        {
            get { return _translatedelta; }
            set
            {
                _translatedelta = value;
                _paneltransform.TranslateY = _currenttranslate + TranslateDelta + ThresholdDelta * Proportion; 
            }
        }
        

        #endregion


        #region fucntions


        void computeProperties()
        {
            double pos = 0.0;
            for (int i = 0; i < _elements.Count; i++)
            {
                _elements[i].Position = pos;
                pos -= _elements[i].GetSize();
            }
            ISlideElementSizeChanged(this);
        }

        public void Animate2Index(int index, bool tobegin)
        {
            double to = _elements[index].Position;
            bool change_th = false;
            if ((_currentindex == index && TranslateDelta < 0.0) || !tobegin)
            {
                to = _elements[index].Position - (_elements[index].GetSize() - DeviceHeight);
                change_th = true;
            }
            Storyboard story = new Storyboard();
            DoubleAnimation animation = new DoubleAnimation();
            animation.Duration = TimeSpan.FromMilliseconds(350);
            Storyboard.SetTarget(animation, _paneltransform);
            Storyboard.SetTargetProperty(animation, "TranslateY");
            story.Children.Add(animation);
            animation.To = to;
            story.Begin();

            _currenttranslate = _elements[index].Position;
            _currentindex = index;
            TranslateDelta = 0.0;
            if(!change_th )
                ThresholdDelta = 0.0;
            //else ThresholdDelta 
        }

        void slide_ISlideElementSizeChanged(object sender)
        {
            computeProperties();
        }


        void initdatasource()
        {
            for (int i = 0; i < _source.Slides.Count; i++)
            {
                if (_source.Slides[i].Type == 0 )
                {
                    CoverTextSlide slide = new CoverTextSlide() { Source = _source.Slides[i] };
                    slide.ISlideElementSizeChanged += slide_ISlideElementSizeChanged;
                    _contentpanel.Children.Add(slide);
                    _elements.Add(slide);
                }

                if (_source.Slides[i].Type == 1 || _source.Slides[i].Type == 6 || _source.Slides[i].Type == 7)
                {
                    LeftTextElement slide = new LeftTextElement() { Source = _source.Slides[i] };
                    slide.ISlideElementSizeChanged += slide_ISlideElementSizeChanged;
                    _contentpanel.Children.Add(slide);
                    _elements.Add(slide);
                }

                if (_source.Slides[i].Type == 2)
                {
                    RigthTextElement slide = new RigthTextElement() { Source = _source.Slides[i] };
                    slide.ISlideElementSizeChanged += slide_ISlideElementSizeChanged;
                    _contentpanel.Children.Add(slide);
                    _elements.Add(slide);
                }

                if (_source.Slides[i].Type == 3)
                {
                    ItemizeTextElement slide = new ItemizeTextElement() { Source = _source.Slides[i] };
                    slide.ISlideElementSizeChanged += slide_ISlideElementSizeChanged;
                    _contentpanel.Children.Add(slide);
                    _elements.Add(slide);
                }

                if (_source.Slides[i].Type == 4)
                {
                    TopSlideElement elem = new TopSlideElement() ; 
                    _contentpanel.Children.Add(elem);
                    _elements.Add(elem);
                    //CoverTextSlide slide = new CoverTextSlide() { Source = _source.Slides[i] };
                    //_contentpanel.Children.Add(slide);
                    //_elements.Add(slide);
                }

                if (_source.Slides[i].Type == 5)
                {
                    QuoteTextElement slide = new QuoteTextElement() { Source = _source.Slides[i] };
                    slide.ISlideElementSizeChanged += slide_ISlideElementSizeChanged;
                    _contentpanel.Children.Add(slide);
                    _elements.Add(slide);
                }

                computeProperties();
                 
            }
        }

        

        #endregion

    }
}
