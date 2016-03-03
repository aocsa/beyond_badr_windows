using MLearning.Store.Components;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    public sealed partial class LOReaderScrollElement : Grid, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        //events for manipulations
        public event MLManipulationCompletedEventHandler MLManipulationCompleted;
        public event MLManipulationDeltaEventHandler MLManipulationDelta;
        public event MLManipulationStartedEventHandler MLManipulationStarted;

        double DeviceHeight = 900.0, DeviceWidth = 1600.0;
        public LOReaderScrollElement()
        {
            init();
        }

        Grid _pagegrid, _blackgrid;
        Image _backimage;
        CoverTextSlide _backtext;
        LoadingView _loadingview;
        CompositeTransform _transform;

        void init()
        {
            Background = new SolidColorBrush(Colors.Transparent);
            HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left;

            _transform = new CompositeTransform() { CenterX = DeviceWidth / 2, CenterY = DeviceHeight / 2 };
            RenderTransform = _transform;

            _backimage = new Image() { Stretch = Stretch.UniformToFill };
            Children.Add(_backimage);

            //just bytes
            //_blackgrid = new Grid() { Width = DeviceWidth, Height = DeviceHeight, Background = new SolidColorBrush(Colors.Black), Opacity = 0.4 };
            //Children.Add(_blackgrid);

            //just bytes
            //_backtext = new CoverTextSlide();
            //Children.Add(_backtext);

            _pagegrid = new Grid() { Width = DeviceWidth, Height = DeviceHeight };
            Children.Add(_pagegrid);
            Canvas.SetZIndex(_pagegrid, -10);

            //just bytes
            //_loadingview = new LoadingView() { Width = 1600, Height = 900 };
            //Children.Add(_loadingview);

        }


        private LOPageSource _source;

        public LOPageSource Source
        {
            get { return _source; }
            set
            {
                _source = value;
               //just bytes _source.PropertyChanged += _source_PropertyChanged;
               // _backtext.Source = _source.Slides[0];
            }
        }

        void _source_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsLoaded")
            {
                if (_source.IsLoaded && !IsLoaded) load_page();
                else clearpage();
            }
        }

        #region properties for translation

        private double _position_x;

        public double PositionX
        {
            get { return _position_x; }
            set { _position_x = value; _transform.TranslateX = _position_x; }
        }


        public CompositeTransform ElementTransform
        {
            get { return _transform; }
            set { _transform = value; }
        }


        private bool _isloaded = false;

        public bool IsLoaded
        {
            get { return _isloaded; }
            set { _isloaded = value; }
        }


        private bool _isloadingvisible;

        public bool IsLoadingVisible
        {
            get { return _isloadingvisible; }
            set 
            {
                _isloadingvisible = value;
                //if (_isloadingvisible) _loadingview.Visibility = Windows.UI.Xaml.Visibility.Visible;
                //else _loadingview.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }
        

        #endregion 
         

        public void load_page()
        {
            if (!IsLoaded)
            { 
                LOPageViewer page = new LOPageViewer();
                page.PropertyChanged += page_PropertyChanged;
                page.MLManipulationCompleted += page_MLManipulationCompleted;
                page.MLManipulationDelta += page_MLManipulationDelta;
                page.MLManipulationStarted += page_MLManipulationStarted;
                page.Source = _source; // set source
                _pagegrid.Children.Add(page);
                //setvisual values
                Canvas.SetZIndex(_pagegrid, 10);
                //Canvas.SetZIndex(_loadingview, -10);
                IsLoaded = true;
            }
        }

        void page_MLManipulationStarted(object sender, int touches)
        {
            MLManipulationStarted(sender, touches);
        }

        void page_MLManipulationDelta(object sender, MLManipulationArgs args)
        {
            MLManipulationDelta(sender, args);
        }

        void page_MLManipulationCompleted(object sender, double v)
        {
            MLManipulationCompleted(sender, v);
        }

        
        public void ResetValues()
        {
            if (_pagegrid.Children.Count > 0)
            {
                ((LOPageViewer)_pagegrid.Children[0]).ResetValues(); 
            }
        }


        public void resetpage( LOPageSource tmpsource )
        { 
            LOPageViewer page = new LOPageViewer();
            page.PropertyChanged += page_PropertyChanged;
            page.Source = _source; // set source
            _pagegrid.Children.Add(page);
            //setvisual values
            Canvas.SetZIndex(_pagegrid, 10);
            //Canvas.SetZIndex(_loadingview, -10);
            IsLoaded = true;
        }

        void page_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Selected")
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Selected"));
            }

            if (e.PropertyName == "Released")
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Released"));
            }

            if (e.PropertyName == "ImageOpened")
            {
               // Canvas.SetZIndex(_pagegrid, 10);
            }
        }


        public void clearpage()
        { 
            _pagegrid.Children.Clear();
            Canvas.SetZIndex(_pagegrid, -10);
            //Canvas.SetZIndex(_loadingview, 10);
            IsLoaded = false;
            //_loadingview.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }



        #region Manipulation properties

        private bool _islocked = false;

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
                ((LOPageViewer)_pagegrid.Children[0]).DeltaY = value;
            }
        }


        #endregion

    }
}
