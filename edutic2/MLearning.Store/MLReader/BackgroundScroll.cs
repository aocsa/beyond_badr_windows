using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace MLReader
{
    public sealed partial class BackgroundScroll : Grid , INotifyPropertyChanged
    {
        double DeviceHeight = 900.0, DeviceWidth = 1600.0;
        public event PropertyChangedEventHandler PropertyChanged;
        public BackgroundScroll()
        {
            init();
        }

        void init()
        {
            Width = DeviceWidth;
            Height = DeviceHeight;
            _elements = new List<ISlideElement>();
            //Scrol view
            _mainscroll = new ScrollViewer()
            {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                VerticalScrollBarVisibility = ScrollBarVisibility.Hidden,
                VerticalScrollMode = ScrollMode.Auto,
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

        double _currenttranslate = 0;
        int _currentindex;

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

        double _threshold;
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

        void initdatasource()
        {
            double pos = 0.0;
            for (int i = 0; i < _source.Slides.Count; i++)
            {
                BackgroundElement elem = new BackgroundElement();
                //if(i==0)
                //    elem.PropertyChanged += elem_PropertyChanged;
                elem.Source = _source.Slides[i];
                elem.Position = pos;
                _contentpanel.Children.Add(elem);
                _elements.Add(elem);
                pos -= 900.0;
            }
            _currenttranslate = 0.0;
            _currentindex = 0;
        }

        void elem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName=="ImageOpened")
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ImageOpened"));
            }
        }


        public void Animate2Index(int index)
        {
           

            Storyboard story = new Storyboard();
            DoubleAnimation animation = new DoubleAnimation();
            animation.Duration = TimeSpan.FromMilliseconds(350);
            Storyboard.SetTarget(animation, _paneltransform);
            Storyboard.SetTargetProperty(animation, "TranslateY");
            story.Children.Add(animation);
            animation.To = _elements[index].Position;
            story.Begin();

            _currenttranslate = _elements[index].Position;
            _translatedelta = 0.0;

        }

        #endregion


        
    }
}
