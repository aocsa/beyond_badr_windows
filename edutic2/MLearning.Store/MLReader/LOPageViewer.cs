using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace MLReader
{
    public sealed partial class LOPageViewer : Grid, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        //events for manipulations
        public event MLManipulationCompletedEventHandler MLManipulationCompleted;
        public event MLManipulationDeltaEventHandler MLManipulationDelta;
        public event MLManipulationStartedEventHandler MLManipulationStarted;

        double DeviceHeight = 900.0, DeviceWidth = 1600.0;
        public LOPageViewer()
        {
            init();
            Background = new SolidColorBrush(Colors.Transparent);
        }


        #region Controls and variables

        ManipulableScroll _manipulablescroll;
        MultiTextScroll _textscroll;
        BackgroundScroll _backscroll;

        int _currentIndex, _lastindex;

        #endregion

        void init()
        {
            Width = DeviceWidth;
            Height = DeviceHeight;

            _backscroll = new BackgroundScroll();
            _backscroll.PropertyChanged += _backscroll_PropertyChanged; 
            Children.Add(_backscroll);
            _textscroll = new MultiTextScroll();
            _textscroll.ISlideElementSizeChanged += _textscroll_ISlideElementSizeChanged;
            Children.Add(_textscroll);
            _manipulablescroll = new ManipulableScroll();
            _manipulablescroll.PropertyChanged += _manipulablescroll_PropertyChanged;
            _manipulablescroll.Animate2IndexEvent += _manipulablescroll_Animate2IndexEvent;
            _manipulablescroll.MLManipulationStarted += _manipulablescroll_MLManipulationStarted;
            _manipulablescroll.MLManipulationDelta += _manipulablescroll_MLManipulationDelta;
            _manipulablescroll.MLManipulationCompleted += _manipulablescroll_MLManipulationCompleted;
            Children.Add(_manipulablescroll);
        }

        void _manipulablescroll_MLManipulationCompleted(object sender, double v)
        {
            MLManipulationCompleted(sender, v);
        }

        void _manipulablescroll_MLManipulationDelta(object sender, MLManipulationArgs args)
        {
            MLManipulationDelta(sender, args);
        }

        void _manipulablescroll_MLManipulationStarted(object sender, int touches)
        {
            MLManipulationStarted(sender, touches);
        }

        void _backscroll_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("ImageOpened"));
        }

        void _manipulablescroll_Animate2IndexEvent(object sender, int index, bool tobegin)
        {
            _currentIndex = index;
            bool to_begin = true;
            if (_lastindex > _currentIndex)
                to_begin = false;
            _textscroll.Animate2Index(_currentIndex, to_begin);
            _backscroll.Animate2Index(_currentIndex);
            computeThresholds(); 
            _lastindex = _currentIndex;
              
        }

        void _textscroll_ISlideElementSizeChanged(object sender)
        {
            computeThresholds();
        }

        void computeThresholds()
        {
            if (_manipulablescroll.Elements.Count > 0)
                _manipulablescroll.Threshold = 900.0 - _textscroll.Elements[_currentIndex].GetSize();
        }

        void computeThresholds1()
        {
            double maxsize = DeviceHeight;
            if (_backscroll.Elements[_currentIndex].GetSize() > maxsize) maxsize = _backscroll.Elements[_currentIndex].GetSize();
            if (_textscroll.Elements[_currentIndex].GetSize() > maxsize) maxsize = _textscroll.Elements[_currentIndex].GetSize();
            if (_manipulablescroll.Elements.Count > 0)
                if (_manipulablescroll.Elements[_currentIndex].GetSize() > maxsize) maxsize = _manipulablescroll.Elements[_currentIndex].GetSize();

            _backscroll.Threshold = -1.0 * _backscroll.Elements[_currentIndex].GetSize() + 900.0;
            _backscroll.Proportion = 0.0;
            

            //_textscroll.Proportion = 1.0; 
            _textscroll.Proportion = 0.0;

            _manipulablescroll.Proportion = 0.0;
            //used to the current scroll delta
            _manipulablescroll.Threshold = DeviceHeight - maxsize;
        }


        private LOPageSource _source;

        public LOPageSource Source
        {
            get { return _source; }
            set { _source = value; initsource(); }
        }
        

        public void ResetValues()
        {
            _manipulablescroll.ResetValues();
        }


        void initsource()
        {
            _backscroll.Source = _source;
            _textscroll.Source = _source;
            _manipulablescroll.Source = _source;
            _currentIndex = 0;
            _lastindex = 0;
        }


        void _manipulablescroll_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ThresholdDelta")
            {
                _textscroll.ThresholdDelta = _manipulablescroll.ThresholdDelta;
            }

            if (e.PropertyName == "TranslateDelta")
            {
                _textscroll.TranslateDelta = _manipulablescroll.TranslateDelta;
                _backscroll.TranslateDelta = _manipulablescroll.TranslateDelta;
            }

            if (e.PropertyName == "ActualPage_1")
            {
                _currentIndex = _manipulablescroll.ActualPage;
                bool to_begin = true;
                if (_lastindex > _currentIndex)
                    to_begin = false; 
                _textscroll.Animate2Index(_currentIndex, to_begin);
                _backscroll.Animate2Index(_currentIndex);
                computeThresholds();
                _lastindex = _currentIndex;
            }

            if (e.PropertyName == "Selected")
            {
                IsLocked = true;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Selected"));
            }

            if (e.PropertyName == "Released")
            {
                IsLocked = false;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Released"));
            }

        }


        #region Manipulation properties

        private bool _islocked = false ;

        public bool IsLocked
        {
            get { return _islocked; }
            set { _islocked = value; }
        }
        


        private double _delta_y;

        public double DeltaY
        {
            get { return _delta_y; }
            set
            {
                _delta_y = value; 

            }
        }

        double MaxThreshold = 0.0, MaxTranslate = 0.0;



        #endregion



    }
}
