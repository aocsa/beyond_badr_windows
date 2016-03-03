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

    public enum StackManipulationType
    {
        StackManipulation,
        ItemManipulation,
        NoneManipulation
    };

    public delegate void StackSizeChangeStartedEventHandler(object sender, double pos);
    public delegate void StackSizeChangeDeltaEventHandler(object sender, double pos);
    public delegate void StackSizeChangeCompletedEventHandler(object sender, double pos);
    public delegate void StackSizeAnimationStartedEventHandler(object sender, bool toopen);
    public delegate void StackSizeAnimationCompletedEventHandler(object sender, bool toopen);


    public sealed partial class IStackView : Grid
    {

        public IStackView()
        {
            _itemsvector = new List<IStackItem>();
            initproperties();
            initcontrols();
            initanimationproperties();
        }

        // public: 

        public event StackItemFullAnimationStartedEventHandler StackItemFullAnimationStarted;
        public event StackItemFullAnimationCompletedEventHandler StackItemFullAnimationCompleted;
        public event StackItemThumbAnimationStartedEventHandler StackItemThumbAnimationStarted;
        public event StackItemThumbAnimationCompletedEventHandler StackItemThumbAnimationCompleted;

        public event IControlsComponentSelectedEventHandler IControlsComponentSelected;

        public event StackSizeChangeStartedEventHandler StackSizeChangeStarted;
        public event StackSizeChangeDeltaEventHandler StackSizeChangeDelta;
        public event StackSizeChangeCompletedEventHandler StackSizeChangeCompleted;
        public event StackSizeAnimationStartedEventHandler StackSizeAnimationStarted;
        public event StackSizeAnimationCompletedEventHandler StackSizeAnimationCompleted;


        #region Controls

        //private:
        StackPanel _itemspanel;
        Grid _itemsgrid;
        Grid _begingrid;
        Grid _endgrid;

        CompositeTransform _paneltransform;

        List<IStackItem> _itemsvector;
        void initcontrols()
        {
            HorizontalAlignment = HorizontalAlignment.Left;
            Background =  new SolidColorBrush(Windows.UI.Colors.Transparent);
            _itemspanel = new StackPanel();
            Children.Add(_itemspanel);
            _itemspanel.Orientation = Orientation.Horizontal;

            _paneltransform = new CompositeTransform();
            _itemspanel.RenderTransform = _paneltransform;

            _begingrid = new Grid();
            _itemspanel.Children.Add(_begingrid);
            _itemsgrid = new Grid();
            _itemsgrid.Background = new SolidColorBrush(Colors.Transparent);
            _itemspanel.Children.Add(_itemsgrid);
            _endgrid = new Grid();
            _itemspanel.Children.Add(_endgrid);

            _itemspanel.PointerPressed += ItemsPanel_PointerPressed_1;
            _itemspanel.PointerReleased += ItemsPanel_PointerReleased_1;
            _itemspanel.Tapped += ItemsPanel_Tapped_1;
        }

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


        //private:
        int _chapter, _section;
        #endregion

        #region Stack Item Properties

        //public:  

        public string BorderSource
        {
            set
            {
                _bordersource = value;
                for (int i = 0; i < _itemsvector.Count; i++)
                    _itemsvector[i].BorderSource = value;
            }
            get { return null; }
        }

        public double MaxScale
        {
            set
            {
                _maxscale = value;
                for (int i = 0; i < _itemsvector.Count; i++)
                    _itemsvector[i].MaxScale = value;
            }
            get { return 0.0; }
        }

        public double ThumbHeight
        {
            set
            {
                _thumbheight = value;
                for (int i = 0; i < _itemsvector.Count; i++)
                    _itemsvector[i].ThumbHeight = value;
            }
            get { return 0.0; }
        }

        public double ThumbWidth
        {
            set
            {
                _thumbwidth = value;
                for (int i = 0; i < _itemsvector.Count; i++)
                    _itemsvector[i].ThumbWidth = value;
            }
            get { return 0.0; }
        }

        public double BorderHeight
        {
            set
            {
                _borderheight = value;
                for (int i = 0; i < _itemsvector.Count; i++)
                    _itemsvector[i].BorderHeight = value;
                _itemspanel.Height = value;
            }
            get { return 0.0; }
        }

        public double BorderWidth
        {
            set
            {
                _borderwidth = value;
                for (int i = 0; i < _itemsvector.Count; i++)
                    _itemsvector[i].BorderWidth = value;
                _itemsgrid.Width = _borderwidth + _spacebetweenitems;
                Width = _borderwidth + _spacebetweenitems + 2 * _auxgridwidth;
                _currentstackwidth = 2 * _auxgridwidth + _borderwidth + _spacebetweenitems;
            }
            get { return 0.0; }
        }

        //private:
        double _initialangle, _maxscale;
        string _bordersource;
        double _thumbheight, _thumbwidth, _borderheight, _borderwidth;

        #endregion

        #region Properties

        //public:
        public int StackNumber
        {
            set
            {
                _stacknumber = value;
            }
            get { return _stacknumber; }
        }

        public int NumberOfItems
        {
            set { _numberofitems = value; }
            get { return _numberofitems; }
        }


        public double StackHeight
        {
            set
            {
                _stackheight = value;
                Height = value;

            }
            get { return _stackheight; }
        }

        public double VerticalPosition
        {
            set
            {
                _verticalposition = value;
                _paneltransform.TranslateY = value;
                for (int i = 0; i < _itemsvector.Count; i++)
                    _itemsvector[i].FullPositionY = -1 * value;
            }
            get { return _verticalposition; }
        }

        public double MinStackWidth
        {
            set
            {
                _minstackwidth = value;
                _auxgridwidth = (value - _borderwidth) / 2;
                _begingrid.Width = _auxgridwidth;
                _endgrid.Width = _auxgridwidth;
                Width = _borderwidth + _spacebetweenitems + 2 * _auxgridwidth;
                _currentstackwidth = 2 * _auxgridwidth + _borderwidth + _spacebetweenitems;
            }
            get { return _minstackwidth; }
        }

        public double CurrentWidth
        {
            set { }
            get { return _currentstackwidth; }
        }

        public double SpaceBetweenItems
        {
            set
            {
                _spacebetweenitems = value;
                _itemsgrid.Width = _borderwidth + _spacebetweenitems;
                Width = _borderwidth + _spacebetweenitems + 2 * _auxgridwidth;
                _currentstackwidth = 2 * _auxgridwidth + _borderwidth + _spacebetweenitems;
            }
            get { return _spacebetweenitems; }
        }

        public bool IsOpen
        {
            set { }
            get { return _isopen; }
        }


        public SectionDataSource Source
        {
            set
            {
                _datasource = value;
                initcomponent();
            }
            get { return _datasource; }
        }

        public double Position
        {
            set { _position = value; }
            get { return _position; }
        }

        public double DeviceWidth
        {
            set { _devicewidth = value; }
            get { return _devicewidth; }
        }

        public double DistanceToScreen
        {
            set
            {
                _distancetoscreen = value;
                updatehorizontalposition();
            }
            get { return _distancetoscreen; }
        }

        public bool IsManipulating
        {
            set
            {
                _touches = 0;
                if (!value)
                    for (int i = 0; i < _itemsvector.Count; i++)
                        _itemsvector[i].IsManipulating = false;

                Canvas.SetZIndex(this, 0);
            }
            get { return true; }
        }


        //private:


        bool _isopen;
        int _stacknumber, _numberofitems;
        double _minstackwidth, _currentstackwidth, _spacebetweenitems, _stackheight, _verticalposition;

        SectionDataSource _datasource;
        double _position, _distancetoscreen, _devicewidth;

        ///Auxiliar varibales
        int _touches, _selectedindex, _tempindex; //tempindex for stack size change
        //double _angles[3] ;
        List<double> _angles;
        double _auxgridwidth, _tempposition, _tmpproportion; //position to the tempindex from 0
        bool _itemslocked;
        #endregion

        #region Controls
        //public:
        public IStackItem SelectedStackItem
        {
            set { }
            get { return _selecteditem; }
        }

        public SelectionType TypeSelected
        {
            set { }
            get { return _selectiontype; }
        }

        public double Proportion
        {
            set
            {
                _proportion = value;
                if (value < 1.0)
                    _proportion = 1.0;
                if (value > _numberofitems)
                    _proportion = _numberofitems;
                updateproportion();
            }
            get { return _proportion; }
        }

        //private:
        IStackItem _selecteditem;
        SelectionType _selectiontype;
        double _proportion;

        #endregion

        #region Public Methods
        //public:
        public void LoadDataSource()
        {
            for (int i = 1; i < _itemsvector.Count; i++)
                _itemsvector[i].LoadThumbSource();
        }

        public void LoadFirst()
        {
            if (_itemsvector.Count > 0)
            {
                _itemsvector[0].LoadThumbSource();
                _itemsvector[0].LoadBorder();
            }
        }

        public void LoadBorder()
        {
            for (int i = 1; i < _itemsvector.Count; i++)
                _itemsvector[i].LoadBorder();
        }

        public void LoadSeparationItem() { }
        public void StackManipulationCompleted()
        {
            if (_selectiontype == SelectionType.StackType)
                if (_proportion > _numberofitems / 2)
                    openstack();
                else
                    closestack();
        }

        public void SetToOpen()
        {
            _itemsgrid.Width = _numberofitems * (_borderwidth + _spacebetweenitems);
            Width = _numberofitems * (_borderwidth + _spacebetweenitems) + 2 * _auxgridwidth;
            for (int i = 0; i < _itemsvector.Count; i++)
                _itemsvector[i].SetToOpen();
            _isopen = true;
            _proportion = _numberofitems;
            _currentstackwidth = 2 * _auxgridwidth + _itemsgrid.Width;
            this.Width = _currentstackwidth;
            _selectiontype = SelectionType.StackType;
            Canvas.SetZIndex(this, 0);
        }

        public void SetToClose()
        {
            _itemsgrid.Width = _borderwidth + _spacebetweenitems;
            Width = _borderwidth + _spacebetweenitems + 2 * _auxgridwidth;
            for (int i = 0; i < _itemsvector.Count; i++)
                _itemsvector[i].SetToClose();

            _isopen = false;
            _proportion = 1.0;
            _currentstackwidth = 2 * _auxgridwidth + _itemsgrid.Width;
            this.Width = _currentstackwidth;
            _selectiontype = SelectionType.StackType;
            Canvas.SetZIndex(this, 0);
        }

        public double GetItemPosition(int page)
        {
            //only if is open
            return (_auxgridwidth / 2) + _itemsvector[page].FinalPosition;
        }

        public void SetItemToFull(int page)
        {
            for (int i = 0; i < _itemsvector.Count; i++)
                _itemsvector[i].ZIndex = 100 - i;
            _itemsvector[page].SetToFull();
            _selectedindex = page;
            _selecteditem = _itemsvector[page];
            Canvas.SetZIndex(this, 10);
        }

        #endregion

        #region Private Methods

        //private:
        void initproperties()
        {
            _stacknumber = 0;
            _numberofitems = 0;
            _minstackwidth = 0.0;
            _currentstackwidth = 0.0;
            _spacebetweenitems = 0.0;

            //auxiliar variables
            _touches = 0;
            _auxgridwidth = 0.0;
            _isopen = false;

            _angles = new List<double>();
            _angles.Add (0.0);
            _angles.Add(8.0);
            _angles.Add( 15.0);

            _selectiontype = SelectionType.None;
            _proportion = 1.0;
            _itemslocked = false;
        }

        void initcomponent()
        {
            if (_datasource != null)
            {
                _numberofitems = _datasource.Pages.Count;
                //_numberofitems = 6;
                for (int i = 0; i < _numberofitems; i++)
                {
                    IStackItem sitem = new IStackItem();
                    sitem.ItemNumber = i;
                    sitem.InitialAngle = _angles[i % 3];
                    sitem.ThumbHeight = _thumbheight;
                    sitem.ThumbWidth = _thumbwidth;
                    sitem.BorderWidth = _borderwidth;
                    sitem.BorderHeight = _borderheight;
                    sitem.MaxScale = _maxscale;
                    sitem.InitialPosition = _spacebetweenitems / 2;
                    sitem.FinalPosition = _borderwidth * i + _spacebetweenitems * (2 * i + 1) / 2;
                    sitem.BorderSource = _bordersource;
                    sitem.FullPositionX = 0.0;
                    sitem.FullPositionY = -1 * _verticalposition;
                    sitem.Source = _datasource.Pages[i];

                    sitem.StackItemSelected += StackItem_Selected;
                    sitem.StackItemTapped += StackItem_Tapped;

                    sitem.StackItemFullAnimationStarted += StackItem_FullAnimationStarted;
                    sitem.StackItemFullAnimationCompleted += StackItem_FullAnimationCompleted;
                    sitem.StackItemThumbAnimationStarted += StackItem_ThumbAnimationStarted;
                    sitem.StackItemThumbAnimationCompleted += StackItem_ThumbAnimationCompleted;

                    _itemsgrid.Children.Add(sitem);
                    _itemsvector.Add(sitem);
                }
                if (_itemsvector.Count > 0)
                    _selecteditem = _itemsvector[0];
                _currentstackwidth = 2 * _auxgridwidth + _borderwidth + _spacebetweenitems;
                this.Width = _currentstackwidth;
            }
        }

        void openstack()
        {
            for (int i = 0; i < _itemsvector.Count; i++)
                _itemsvector[i].AnimateToOpen();

            _itemgridanimation.To = _numberofitems * (_spacebetweenitems + _borderwidth);
            _itemsgridstory.Begin();
            _viewanimation.To = 2 * _auxgridwidth + _numberofitems * (_spacebetweenitems + _borderwidth);
            _viewstory.Begin();

            _currentstackwidth = 2 * _auxgridwidth + _numberofitems * (_spacebetweenitems + _borderwidth);
            StackSizeAnimationStarted(this, true); //true . animate to open
            _isopen = true;
            _proportion = _numberofitems;
            _itemslocked = true;
        }

        void closestack()
        {
            for (int i = 0; i < _itemsvector.Count; i++)
                _itemsvector[i].AnimateToClose();

            _itemgridanimation.To = _spacebetweenitems + _borderwidth;
            _viewanimation.To = 2 * _auxgridwidth + _spacebetweenitems + _borderwidth;
            _itemsgridstory.Begin();
            _viewstory.Begin();

            _currentstackwidth = 2 * _auxgridwidth + _spacebetweenitems + _borderwidth;
            StackSizeAnimationStarted(this, false);
            _itemslocked = true;
            _isopen = false;
            _proportion = 1.0;
        }


        void updateproportion()
        {
            for (int i = 0; i < _itemsvector.Count; i++)
            {
                _itemsvector[i].ItemTransform.TranslateX = (_proportion - 1) * (_itemsvector[i].FinalPosition - _itemsvector[i].InitialPosition) / (_numberofitems - 1) + _itemsvector[i].InitialPosition;
                _itemsvector[i].ItemTransform.Rotation = (_proportion - 1) * _itemsvector[i].InitialAngle / (1 - _numberofitems) + _itemsvector[i].InitialAngle;
            }

            _itemsgrid.Width = _proportion * (_borderwidth + _spacebetweenitems);
            Width = _itemsgrid.Width + 2 * _auxgridwidth;

            //Change the size of the stack . Update the positon of the stack to keep this in this initial position
            if (_isopen)
                StackSizeChangeDelta(this, _tempposition - (_tmpproportion * Width));
            if (_proportion < _numberofitems)
            {
                for (int i = 0; i < _itemsvector.Count; i++)
                    _itemsvector[i].HiddeText();
            }
        }

        void updatehorizontalposition()
        {
            double center = (_devicewidth - _auxgridwidth - _borderwidth - _spacebetweenitems) / 2 - _distancetoscreen;//0=(_spacebetweenitems / 2)
            for (int i = 0; i < _itemsvector.Count; i++)
                _itemsvector[i].FullPositionX = center;
        }

        #endregion

        #region Events Methods

        void resetzposition(int index)
        {
            for (int i = 0; i < _itemsvector.Count; i++)
            {
                if (i == index) Canvas.SetZIndex( _itemsvector[i],10);
                else Canvas.SetZIndex(_itemsvector[i], 0);
            }
        }

        //private:
        void StackItem_Selected(object sender, int _itemnumber)
        {
            if (_selectiontype != SelectionType.ItemType && !_itemslocked)
            {
                resetzposition(_itemnumber);
                _selecteditem = _itemsvector[_itemnumber];
                _selectiontype = SelectionType.ItemType;
                _selectedindex = _itemnumber;
                _selecteditem.ZIndex = 100;
                _selecteditem.LoadFullSource();
                IControlsComponentSelected(this, SelectionType.ItemType, _stacknumber);
                Canvas.SetZIndex(this, 100);
            }

            _itemslocked = true;
            /**
            else
            {
                _itemsvector[_itemnumber].ZIndex -= _itemnumber ;
                _itemsvector[_itemnumber].DeleteFullSource();
            }*/
        }

        int count = 0;

        void StackItem_Tapped(object sender, int _itemnumber)
        {
            count++;
            if (count > 1)
            {
                int a = 3;
            }
            if (!_itemslocked)
            {
                _itemslocked = true;
                if (_selectiontype != SelectionType.ItemType || _touches < 2)
                {
                    resetzposition(_itemnumber);
                    _selecteditem = _itemsvector[_itemnumber];
                    _selectiontype = SelectionType.ItemType;
                    _selectedindex = _itemnumber;
                    _selecteditem.ZIndex = 100;
                    _selecteditem.LoadFullSource();
                    _selecteditem.AnimateToFull();
                    IControlsComponentSelected(this, SelectionType.ItemType, _stacknumber);
                    Canvas.SetZIndex(this, 10);
                }
                else
                { 
                    _itemsvector[_itemnumber].DeleteFullSource();
                }
            }
        }

        void ItemsPanel_PointerReleased_1(object sender, PointerRoutedEventArgs e)
        {
            _touches = 0;
        }


        void ItemsPanel_PointerPressed_1(object sender, PointerRoutedEventArgs e)
        {
            _touches += 1;
            if (_touches < 2)
            {
                _selectiontype = SelectionType.StackType;
                _tempindex = (int)Math.Ceiling(e.GetCurrentPoint(_itemsgrid).Position.X / (_borderwidth + _spacebetweenitems));
                _tempposition = e.GetCurrentPoint(this).Position.X + _thumbwidth; /// _tempindex * (_itemsgrid.Width / _numberofitems) + _auxgridwidth ;
                _tmpproportion = _tempposition / _currentstackwidth;
                StackSizeChangeStarted(this, _tempposition);
                IControlsComponentSelected(this, SelectionType.StackType, _stacknumber);
                Canvas.SetZIndex(this, 10);
                //hidde all text 
            }
        }

        void ItemsPanel_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            if (!_isopen)
            {///open the stack
                _selectiontype = SelectionType.StackType;
                openstack();
            }
        }

        void StackItem_FullAnimationStarted(object sender, int chapter, int section, int page)
        {
            StackItemFullAnimationStarted(sender, chapter, _stacknumber, page);
            _itemslocked = true;
            Canvas.SetZIndex(this, 100);
        }

        void StackItem_FullAnimationCompleted(object sender, int chapter, int section, int page)
        {
            StackItemFullAnimationCompleted(sender, chapter, _stacknumber, page);
        }

        void StackItem_ThumbAnimationStarted(object sender, int chapter, int section, int page)
        {
            StackItemThumbAnimationStarted(this, chapter, _stacknumber, page);
            _selectiontype = SelectionType.ItemType;
            _itemslocked = true;
        }

        void StackItem_ThumbAnimationCompleted(object sender, int chapter, int section, int page)
        {
            StackItemThumbAnimationCompleted(this, chapter, _stacknumber, page);
            _selectiontype = SelectionType.StackType;
            _touches = 0;
            _itemslocked = false;
            Canvas.SetZIndex(this, 0);
            resetzposition(-1);
            count = 0;
        }

        #endregion

        #region Animation

        //private:
        Storyboard _itemsgridstory;
        DoubleAnimation _itemgridanimation;

        Storyboard _viewstory;
        DoubleAnimation _viewanimation;

        void initanimationproperties()
        {

            _itemsgridstory = new Windows.UI.Xaml.Media.Animation.Storyboard();
            _itemgridanimation = new Windows.UI.Xaml.Media.Animation.DoubleAnimation();
            _itemgridanimation.Duration = TimeSpan.FromMilliseconds(350);
            _itemgridanimation.EnableDependentAnimation = true;
            _itemsgridstory.Children.Add(_itemgridanimation);
            Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(_itemgridanimation, "(Grid.Width)");
            Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(_itemgridanimation, _itemsgrid);

            _viewstory = new Windows.UI.Xaml.Media.Animation.Storyboard();
            _viewanimation = new Windows.UI.Xaml.Media.Animation.DoubleAnimation();
            _viewanimation.Duration = TimeSpan.FromMilliseconds(350);
            _viewanimation.EnableDependentAnimation = true;
            _viewstory.Children.Add(_viewanimation);
            Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(_viewanimation, "(Grid.Width)");
            Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(_viewanimation, this);

            _viewstory.Completed += Storyboard_Completed_1;
        }

        void Storyboard_Completed_1(object sender, object e)
        {
            StackSizeChangeCompleted(this, 0.0);
            if (_proportion > 1.0)
            {
                StackSizeAnimationCompleted(this, true);
                //show the text of every item stack
                for (int i = 0; i < _itemsvector.Count; i++)
                    _itemsvector[i].ShowText();
            }
            else
            {
                StackSizeAnimationCompleted(this, false);
            }
            this.Width = _currentstackwidth;
            _selectiontype = SelectionType.StackType;
            _touches = 0;
            Canvas.SetZIndex(this, 0);
            _itemslocked = false;
        }
        #endregion

    }
}
