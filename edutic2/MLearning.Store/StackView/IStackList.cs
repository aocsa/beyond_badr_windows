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
using MLReader;

namespace StackView
{

    public delegate void StackListScrollToEventHandler(object sender, double _position);
    public delegate void StackListAnimateToEventHandler(object sender, double _position);
    public delegate void StackListWidthChangedEventHandler(object sender, double _position);

    public sealed partial class IStackList : Grid
    {

        //  public:
        public IStackList()
        {
            initcontrols();
            initproperties();
            Background = new SolidColorBrush(Windows.UI.Colors.Transparent);
        }

        public event StackItemFullAnimationStartedEventHandler StackItemFullAnimationStarted;
        public event StackItemFullAnimationCompletedEventHandler StackItemFullAnimationCompleted;
        public event StackItemThumbAnimationStartedEventHandler StackItemThumbAnimationStarted;
        public event StackItemThumbAnimationCompletedEventHandler StackItemThumbAnimationCompleted;

        public event StackListScrollToEventHandler StackListScrollTo;
        public event StackListAnimateToEventHandler StackListAnimateTo;
        public event StackListWidthChangedEventHandler StackListWidthChanged;
        public event IControlsComponentSelectedEventHandler IControlsComponentSelected;

        #region Paging Properties
        //public:

        public int Chapter
        {
            set { _chapter = value; }
            get { return _chapter; }
        }

        //private:
        int _chapter;

        #endregion

        #region Controls

        //private:
        StackPanel _panel;
        Grid _auxgrid;
        Grid _startgrid;
        //376
        Image _startimage;
        ///item replace image -
        ChapterHeaderControl _headercontrol;
        double STARTWIDTH = 500;

        List<IStackView> _stacksvector;

        void initcontrols()
        {
            _stacksvector = new List<IStackView>();

            _panel = new StackPanel();
            _panel.Orientation = Orientation.Horizontal;
            Children.Add(_panel);

            _auxgrid = new Grid();
            _auxgrid.Background = new SolidColorBrush(Colors.Transparent);
            _auxgrid.ManipulationMode = ManipulationModes.TranslateX;

            _startgrid = new Grid();
            _startgrid.Height = STARTWIDTH;
            _startgrid.Width = STARTWIDTH;
            _panel.Children.Add(_startgrid);

            _headercontrol = new ChapterHeaderControl();
            _headercontrol.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Right;
            _startgrid.Children.Add(_headercontrol);
        }

        #endregion

        #region Stack  Properties

        //public:  

        public string BorderSource
        {
            set
            {
                _bordersource = value;
                for (int i = 0; i < _stacksvector.Count; i++)
                    _stacksvector[i].BorderSource = value;
            }
            get { return null; }
        }

        public double MaxScale
        {
            set
            {
                _maxscale = value;
                for (int i = 0; i < _stacksvector.Count; i++)
                    _stacksvector[i].MaxScale = value;
            }
            get { return 0.0; }
        }

        public double ThumbHeight
        {
            set
            {
                _thumbheight = value;
                for (int i = 0; i < _stacksvector.Count; i++)
                    _stacksvector[i].ThumbHeight = value;
            }
            get { return 0.0; }
        }

        public double ThumbWidth
        {
            set
            {
                _thumbwidth = value;
                for (int i = 0; i < _stacksvector.Count; i++)
                    _stacksvector[i].ThumbWidth = value;
            }
            get { return 0.0; }
        }

        public double BorderHeight
        {
            set
            {
                _borderheight = value;
                for (int i = 0; i < _stacksvector.Count; i++)
                    _stacksvector[i].BorderHeight = value;
            }
            get { return 0.0; }
        }

        public double BorderWidth
        {
            set
            {
                _borderwidth = value;
                for (int i = 0; i < _stacksvector.Count; i++)
                    _stacksvector[i].BorderWidth = value;
            }
            get { return 0.0; }
        }

        public double StackVerticalPosition
        {
            set
            {
                _verticalposition = value;
                for (int i = 0; i < _stacksvector.Count; i++)
                    _stacksvector[i].VerticalPosition = value;
            }
            get { return _verticalposition; }
        }

        public double MinStackWidth
        {
            set
            {
                _minstackwidth = value;
                for (int i = 0; i < _stacksvector.Count; i++)
                    _stacksvector[i].MinStackWidth = value;
            }
            get { return _minstackwidth; }
        }

        public double SpaceBetweenItems
        {
            set
            {
                _spacebetweenitems = value;
                for (int i = 0; i < _stacksvector.Count; i++)
                    _stacksvector[i].SpaceBetweenItems = value;
            }
            get { return _spacebetweenitems; }
        }








        //private:

        double _initialangle, _maxscale;
        string _bordersource;
        double _thumbheight, _thumbwidth, _borderheight, _borderwidth;
        double _verticalposition, _minstackwidth, _spacebetweenitems;
        #endregion

        #region Properties
        //public:

        public int ListNumber
        {
            set { _listnumber = value; }//if (value % 2 == 0) Background = new SolidColorBrush(Windows.UI.Colors.Blue); }
            get { return _listnumber; }
        }

        public ChapterDataSource Source
        {
            set
            {
                _datasource = value;
                loadcontrols();
            }
            get { return _datasource; }
        }

        public double MinListWidth
        {
            set
            {
                _minlistwidth = value;
                for (int i = 0; i < _stacksvector.Count; i++)
                    _stacksvector[i].DeviceWidth = value;
            }
            get { return _minlistwidth; }
        }

        public double ListHeight
        {
            set
            {
                _listheight = value;
                for (int i = 0; i < _stacksvector.Count; i++)
                    _stacksvector[i].StackHeight = value;
            }
            get { return _listheight; }
        }

        public double CurrentListWidth
        {
            set { _currentlistwidth = value; }
            get { return _currentlistwidth; }
        }


        public double Position
        {
            set
            {
                _position = value;
            }
            get { return _position; }
        }

        public double DistanceToScreen
        {
            set
            {
                _distancetoscreen = value;
                for (int i = 0; i < _stacksvector.Count; i++)
                    _stacksvector[i].DistanceToScreen = _stacksvector[i].Position + _distancetoscreen;
            }
            get { return _distancetoscreen; }
        }

        public bool IsLoaded
        {
            set { }
            get { return _isloaded; }
        }

        public bool IsManipulating
        {
            set
            {
                _ismanipulating = value;
                if (!value)
                    for (int i = 0; i < _stacksvector.Count; i++)
                        _stacksvector[i].IsManipulating = false;
            }
            get { return _ismanipulating; }
        }

        public IStackView SelectedStack
        {
            set { }
            get { return _selectedstack; }
        }

        public SelectionType TypeSelected
        {
            set { }
            get { return _selectiontype; }
        }

        //private:
        ChapterDataSource _datasource;
        double _minlistwidth, _listheight, _currentlistwidth;
        double _position, _distancetoscreen;
        bool _isloaded, _ismanipulating;
        int _listnumber, _selectedindex;
        IStackView _selectedstack;
        SelectionType _selectiontype;
        //Auxiliar propeties
        int _numberofstacks;
        double _tempdistoscreen;

        #endregion

        #region Public Methods
        //public:
        public void LoadDataSource()
        {
            _isloaded = true;
            for (int i = 0; i < _stacksvector.Count; i++)
                _stacksvector[i].LoadFirst();
            for (int i = 0; i < _stacksvector.Count; i++)
                _stacksvector[i].LoadBorder();
            for (int i = 0; i < _stacksvector.Count; i++)
                _stacksvector[i].LoadDataSource();
        }

        public void UpDateProperties()
        {
            updatewidth();
            _selectiontype = SelectionType.StackType;
        }

        public void OpenStack(int index)
        {
            if (_stacksvector.Count == 0)
                return;
            for (int i = 0; i < _stacksvector.Count; i++)
            {
                if (_stacksvector[i].IsOpen)
                    _stacksvector[i].SetToClose();
                if (_stacksvector[i].NumberOfItems == 1)
                    _stacksvector[i].SetToOpen();
            }

            _stacksvector[index].SetToOpen();
            updatewidth();
            _selectedstack = _stacksvector[index];
            _selectedindex = index;
        }

        public double GetItemPosition(int section, int page)
        {
            return _stacksvector[section].Position + _stacksvector[section].GetItemPosition(page);
        }

        public void SetItemToFull(int section, int page)
        {
            _stacksvector[section].SetItemToFull(page);
            _selectedindex = section;
            _selectedstack = _stacksvector[section];
        }

        #endregion

        #region Private Methods
        //private:

        void loadcontrols()
        {
            if (_datasource != null)
            {
                _numberofstacks = _datasource.Sections.Count;
                for (int i = 0; i < _numberofstacks; i++)
                {
                    IStackView stack = new IStackView();
                    stack.StackNumber = i;
                    stack.Section = i;
                    stack.BorderSource = _bordersource;
                    stack.ThumbHeight = _thumbheight;
                    stack.ThumbWidth = _thumbwidth;
                    stack.BorderHeight = _borderheight;
                    stack.BorderWidth = _borderwidth;
                    stack.MaxScale = _maxscale;
                    stack.StackHeight = _listheight;
                    stack.VerticalPosition = _verticalposition;
                    stack.MinStackWidth = _minstackwidth;
                    stack.SpaceBetweenItems = _spacebetweenitems;
                    stack.DeviceWidth = _minlistwidth;
                    stack.Source = _datasource.Sections[i];
                    //header
                    _headercontrol.Author = _datasource.Author;
                    _headercontrol.Title = _datasource.Title;
                    _headercontrol.Description = _datasource.Description;
                    StyleConstants styles = new StyleConstants();
                    _headercontrol.ChapterColor = styles.SecondColors[_listnumber % 4];

                    stack.StackItemFullAnimationStarted += StackItem_FullAnimationStarted;
                    stack.StackItemFullAnimationCompleted += StackItem_FullAnimationCompleted;
                    stack.StackItemThumbAnimationStarted += StackItem_ThumbAnimationStarted;
                    stack.StackItemThumbAnimationCompleted += StackItem_ThumbAnimationCompleted;

                    stack.StackSizeChangeStarted += Stack_SizeChangeStarted;
                    stack.StackSizeChangeDelta += Stack_SizeChangeDelta;
                    stack.StackSizeChangeCompleted += Stack_SizeChangeCompleted;
                    stack.StackSizeAnimationStarted += Stack_SizeAnimationStarted;
                    stack.StackSizeAnimationCompleted += Stack_SizeAnimationCompleted;

                    stack.IControlsComponentSelected += IControls_ComponentSelected;

                    //_startimage.Source = new BitmapImage(new Uri("ms-appx:///roadsdata/text"+(int)(i+1)+".png"));

                    if (stack.NumberOfItems == 1)
                        stack.SetToOpen();

                    _panel.Children.Add(stack);
                    _stacksvector.Add(stack);
                }
                _panel.Children.Add(_auxgrid);
                updatewidth();
            }
        }

        void initproperties()
        {
            _isloaded = false;
            _ismanipulating = false;
            _selectiontype = SelectionType.StackType;
        }

        void updatewidth()
        {
            double tempwidth = STARTWIDTH; //0.0 ;
            for (int i = 0; i < _stacksvector.Count; i++)
            {
                _stacksvector[i].Position = tempwidth;
                tempwidth += _stacksvector[i].CurrentWidth;
            }
            if (tempwidth < _minlistwidth)
            {
                _auxgrid.Width = _minlistwidth - tempwidth;
                _currentlistwidth = _minlistwidth;
                Width = _minlistwidth;
            }
            else
            {
                _auxgrid.Width = 0.0;
                _currentlistwidth = tempwidth;
                Width = tempwidth;
            }
        }

        #endregion

        #region Events Methods
        //private:
        void Stack_SizeChangeStarted(object sender, double pos)
        {
            _tempdistoscreen = -1 * _distancetoscreen;
        }

        void Stack_SizeChangeDelta(object sender, double pos)
        {
            StackListScrollTo(this, _tempdistoscreen - pos);
        }

        void Stack_SizeChangeCompleted(object sender, double pos)
        {
        }

        void Stack_SizeAnimationStarted(object sender, bool toopen)
        {
            if (toopen)
            {
                updatewidth();
                StackListAnimateTo(this, _stacksvector[_selectedindex].Position);
            }
            else
            {
                double tempanimation = _stacksvector[_selectedindex].Position - (_minstackwidth / 2) - _borderwidth;
                double tempwidth = 0.0;
                for (int i = 0; i < _stacksvector.Count; i++)
                    tempwidth += _stacksvector[i].CurrentWidth;
                _currentlistwidth = tempwidth;
                if ((tempanimation + (_minstackwidth / 2)) > tempwidth)
                    tempanimation = tempwidth - _minstackwidth;
                StackListAnimateTo(this, tempanimation);
            }
        }

        void Stack_SizeAnimationCompleted(object sender, bool toopen)
        {
            updatewidth();
            _ismanipulating = false;
            IControlsComponentSelected(this, SelectionType.StackType, _listnumber);
            _selectedindex = ((IStackView)sender).StackNumber;
        }

        void IControls_ComponentSelected(object sender, SelectionType t, int index)
        {
            if (!_ismanipulating)
            {
                Canvas.SetZIndex(_stacksvector[index], 10);
                _selectiontype = t;
                _selectedstack = _stacksvector[index];
                _selectedindex = index;
                IControlsComponentSelected(this, t, _listnumber);
                Canvas.SetZIndex(this, 100);
            }
        }

        void resetZindexlist(int sindex)
        {
            for (int i = 0; i < _stacksvector.Count; i++)
                if(i==sindex) Canvas.SetZIndex(_stacksvector[i], 10);
                else Canvas.SetZIndex(_stacksvector[i], 0);
            
        }

        void StackItem_FullAnimationStarted(object sender, int chapter, int section, int page)
        {
            resetZindexlist(section); 
            StackItemFullAnimationStarted(sender, _listnumber, section, page);
        }

        void StackItem_FullAnimationCompleted(object sender, int chapter, int section, int page)
        { 
            StackItemFullAnimationCompleted(sender, _listnumber, section, page);
        }

        void StackItem_ThumbAnimationStarted(object sender, int chapter, int section, int page)
        {
            StackItemThumbAnimationStarted(sender, _listnumber, section, page);
        }

        void StackItem_ThumbAnimationCompleted(object sender, int chapter, int section, int page)
        {
            resetZindexlist(-1); 
            StackItemThumbAnimationCompleted(sender, _listnumber, section, page);
            _selectiontype = SelectionType.StackType;
            _ismanipulating = false;
            Canvas.SetZIndex(this, 0);
        }


        #endregion


    }
}
