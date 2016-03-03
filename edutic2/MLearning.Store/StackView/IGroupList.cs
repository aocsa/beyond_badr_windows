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

    public delegate void StackListScrollDeltaEventHandler(object sender, double delta);
    public delegate void StackListScrollCompletedEventHandler(object sender, int nextitem);

    public sealed partial class IGroupList : Grid, INotifyPropertyChanged
    {
        //public:
        public IGroupList()
        {
            _listvector = new List<IStackList>();
            _texto = new TextBlock();
            _texto.FontSize = 25;
            _texto.Width = 200;
            _texto.Height = 200;
            _texto.VerticalAlignment = VerticalAlignment.Top;
            _texto.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
            //Children.Add(_texto);
            initcontrols();
            initproperties();
            initanimationproperties();
            Background = new SolidColorBrush(Colors.Transparent);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event StackItemFullAnimationStartedEventHandler StackItemFullAnimationStarted;
        public event StackItemFullAnimationCompletedEventHandler StackItemFullAnimationCompleted;
        public event StackItemThumbAnimationStartedEventHandler StackItemThumbAnimationStarted;
        public event StackItemThumbAnimationCompletedEventHandler StackItemThumbAnimationCompleted;

        public event StackListScrollDeltaEventHandler StackListScrollDelta;
        public event StackListScrollCompletedEventHandler StackListScrollCompleted;


        #region Controls

        //private:
        Image _backimage;
        ScrollViewer _groupscroll;
        StackPanel _grouppanel;
        CompositeTransform _paneltransform;

        List<IStackList> _listvector;

        TextBlock _texto;


        void initcontrols()
        {
            _grouppanel = new StackPanel();
            _grouppanel.Background = new SolidColorBrush(Windows.UI.Colors.Transparent);
            _grouppanel.Orientation = Orientation.Horizontal;
            _grouppanel.ManipulationMode = ManipulationModes.All;
            _grouppanel.ManipulationDelta += Panel_ManipulationDelta_1;
            _grouppanel.ManipulationCompleted += Panel_ManipulationCompleted_1;
            _grouppanel.ManipulationInertiaStarting += Panel_ManipulationInertiaStarting_1;
            _grouppanel.PointerPressed += Panel_PointerPressed_1;
            _grouppanel.PointerReleased += Panel_PointerReleased_1;

            _groupscroll = new ScrollViewer();
            _groupscroll.Background = new SolidColorBrush(Windows.UI.Colors.Transparent);
            _groupscroll.HorizontalScrollMode = Windows.UI.Xaml.Controls.ScrollMode.Disabled;
            _groupscroll.VerticalScrollMode = Windows.UI.Xaml.Controls.ScrollMode.Disabled;
            _groupscroll.HorizontalScrollBarVisibility = Windows.UI.Xaml.Controls.ScrollBarVisibility.Hidden;
            _groupscroll.VerticalScrollBarVisibility = Windows.UI.Xaml.Controls.ScrollBarVisibility.Hidden;
            _groupscroll.ZoomMode = ZoomMode.Disabled;
            _groupscroll.IsHorizontalRailEnabled = false;
            Children.Add(_groupscroll);
            _groupscroll.Content = _grouppanel;

            _paneltransform = new CompositeTransform();
            _grouppanel.RenderTransform = _paneltransform;
        }


        #endregion

        #region Stack  Properties

        //public:  

        public string BorderSource
        {
            set
            {
                _bordersource = value;
                for (int i = 0; i < _listvector.Count; i++)
                    _listvector[i].BorderSource = value;
            }
            get { return null; }
        }

        public double MaxScale
        {
            set
            {
                _maxscale = value;
                for (int i = 0; i < _listvector.Count; i++)
                    _listvector[i].MaxScale = value;
            }
            get { return 0.0; }
        }

        public double ThumbHeight
        {
            set
            {
                _thumbheight = value;
                for (int i = 0; i < _listvector.Count; i++)
                    _listvector[i].ThumbHeight = value;
            }
            get { return 0.0; }
        }

        public double ThumbWidth
        {
            set
            {
                _thumbwidth = value;
                for (int i = 0; i < _listvector.Count; i++)
                    _listvector[i].ThumbWidth = value;
            }
            get { return 0.0; }
        }

        public double BorderHeight
        {
            set
            {
                _borderheight = value;
                for (int i = 0; i < _listvector.Count; i++)
                    _listvector[i].BorderHeight = value;
            }
            get { return 0.0; }
        }

        public double BorderWidth
        {
            set
            {
                _borderwidth = value;
                for (int i = 0; i < _listvector.Count; i++)
                    _listvector[i].BorderWidth = value;
            }
            get { return 0.0; }
        }

        public double StackVerticalPosition
        {
            set
            {
                _verticalposition = value;
                for (int i = 0; i < _listvector.Count; i++)
                    _listvector[i].StackVerticalPosition = value;
            }
            get { return _verticalposition; }
        }

        public double MinStackWidth
        {
            set
            {
                _minstackwidth = value;
                for (int i = 0; i < _listvector.Count; i++)
                    _listvector[i].MinStackWidth = value;
            }
            get { return _minstackwidth; }
        }

        public double SpaceBetweenItems
        {
            set
            {
                _spacebetweenitems = value;
                for (int i = 0; i < _listvector.Count; i++)
                    _listvector[i].SpaceBetweenItems = value;
            }
            get { return _spacebetweenitems; }
        }




        //private: 

        double _maxscale;
        string _bordersource;
        double _thumbheight, _thumbwidth, _borderheight, _borderwidth;
        double _verticalposition, _minstackwidth, _spacebetweenitems;

        //static properties
        double IThumbHeight = 150.0;
        double IThumbWidth = 267.0;
        double IFrameHeight = 305.0;
        double IFrameWidth = 210.0;
        double IDeviceWidth = 900.0;
        double IDeviceHeight = 1600.0;


        #endregion

        #region Properties
        //public:
        public BookDataSource Source
        {
            set
            {
                _datasource = value;
                loadcontrols();
            }
            get { return _datasource; }
        }

        public double ControlWidth
        {
            set
            {
                _controlwidth = value;
                Width = value;
                for (int i = 0; i < _listvector.Count; i++)
                    _listvector[i].MinListWidth = value;
            }
            get { return _controlwidth; }
        }

        public double ControlHeight
        {
            set
            {
                _controlheight = value;
                Height = value;
                for (int i = 0; i < _listvector.Count; i++)
                    _listvector[i].ListHeight = value;
            }
            get { return _controlheight; }
        }

        public int StartIndex
        {
            get { return _startindex; }
            set { _startindex = value; }
        }

        public IStackItem SelectedStackItem
        {
            set { }
            get { return _listvector[_selectedchapter].SelectedStack.SelectedStackItem; }
        }

        public int SelectedChapter
        {
            set { _selectedchapter = value; }
            get { return _selectedchapter; }
        }

        public int SelectedSection
        {
            set { _selectedsection = value; }
            get { return _selectedsection; }
        }

        public int SelectedPage
        {
            set { _selectedpage = value; }
            get { return _selectedpage; }
        }


        //private:
        BookDataSource _datasource;
        double _controlheight, _controlwidth;

        int _numberofitems, _startindex;
        int _selectedchapter, _selectedsection, _selectedpage;
        ///auxiliar variables
        int _touches;
        bool _manipulationenable, _forcemanipulationtoend;
        bool _ismanipulating, _isinertia, _isitemselected;
        SelectionType _typeselected;
        double _initthreshold, _finalthreshold;
        double _paneltranslate, _offsetdelta;
        //Constants for out manipulation
        double _leftconstant, _rigthconstant;
        #endregion


        #region Public Methods
        //public:
        public void LoadList(int number)
        {
            if (!_listvector[number].IsLoaded)
                _listvector[number].LoadDataSource();
        }

        public void SetToItem(int chapter, int section, int page)
        {
            _selectedchapter = chapter;
            _selectedsection = section;
            _selectedpage = page;
            //reset stacks
            for (int i = 0; i < _listvector.Count; i++)
                if (i == chapter)
                    _listvector[i].OpenStack(section);
                else
                    _listvector[i].OpenStack(0);

            updatelistproperties();
            computethresholds();
            //get item position
            double itempos = _listvector[chapter].Position + _listvector[chapter].GetItemPosition(section, page);
            double pos = -1 * (itempos - _controlwidth / 2 + _borderwidth / 2);
            if (pos > _initthreshold)
                pos = _initthreshold;
            if (pos < _finalthreshold)
                pos = _finalthreshold;
            _paneltranslate = pos;
            _paneltransform.TranslateX = _paneltranslate;
            updatelistproperties();

            //set item to full
            _listvector[chapter].SetItemToFull(section, page);
        }

        public void AnimateToChapter(int chapter)
        {
            animatetochapter(chapter, true);
        }

        public void TranslateTo(double value)
        {
            _paneltransform.TranslateX = _paneltranslate + value * _leftconstant;
        }
        #endregion

        #region Private Methods
        //private:
        void initproperties()
        {
            _maxscale = 6.0;
            //_bordersource = IControls.FrameSource;
            _thumbheight = Util.ThumbHeight;
            _thumbwidth = Util.ThumbWidth;
            _borderheight = Util.FrameHeight;
            _borderwidth = Util.FrameWidth;
            _verticalposition = 0.0;/////////300.0 ;
            _minstackwidth = 347.0;
            _spacebetweenitems = 20.0;

            _maxscale = Util.DeviceWidth / _thumbwidth;

            _controlwidth = Util.DeviceWidth;
            _controlheight = Util.DeviceHeight;
            _startindex = 0;
            _selectedchapter = 0;
            _selectedsection = 0;
            _selectedpage = 0;
            _typeselected = SelectionType.StackType;
            _manipulationenable = true;
            _forcemanipulationtoend = false;
            _isinertia = false;
            _ismanipulating = false;
            _isitemselected = false;
            _touches = 0;
            _offsetdelta = 0;
        }

        void loadcontrols()
        {
            if (_datasource != null)
            {

                Width = _controlwidth;
                Height = _controlheight;
                _numberofitems = _datasource.Chapters.Count;
                for (int i = 0; i < _numberofitems; i++)
                {
                    IStackList list = new IStackList();
                    list.Chapter = i;
                    list.ListNumber = i;
                    list.MaxScale = _maxscale;
                    list.BorderSource = _bordersource;
                    list.ThumbHeight = _thumbheight;
                    list.ThumbWidth = _thumbwidth;
                    list.BorderHeight = _borderheight;
                    list.BorderWidth = _borderwidth;
                    list.StackVerticalPosition = _verticalposition;
                    list.MinStackWidth = _minstackwidth;
                    list.SpaceBetweenItems = _spacebetweenitems;

                    list.MinListWidth = _controlwidth;
                    list.ListHeight = _controlheight;
                    list.Source = _datasource.Chapters[i];

                    list.StackItemFullAnimationStarted += StackItem_FullAnimationStarted;
                    list.StackItemFullAnimationCompleted += StackItem_FullAnimationCompleted;
                    list.StackItemThumbAnimationStarted += StackItem_ThumbAnimationStarted;
                    list.StackItemThumbAnimationCompleted += StackItem_ThumbAnimationCompleted;

                    list.StackListAnimateTo += StackList_AnimateTo;
                    list.StackListScrollTo += StackList_ScrollTo;
                    list.StackListWidthChanged += StackList_WidthChanged;
                    list.IControlsComponentSelected += IControls_ComponentSelected;
                    _grouppanel.Children.Add(list);
                    _listvector.Add(list);
                }
                LoadList(_startindex);
                ///open the fisrt the stacks
                //for (int i = 0; i < _listvector.Count; i++) 
                //_listvector[i].OpenStack(0);

                _selectedchapter = _startindex;
                updatelistproperties();
                computethresholds();

                for (int i = 0; i < _numberofitems; i++)
                    LoadList(i);

                _texto.Text = "" + _selectedchapter;
            }
        }

        void computethresholds()
        {
            /**paged scroll view*/
            _initthreshold = -1 * _listvector[_selectedchapter].Position;
            _finalthreshold = _initthreshold - _listvector[_selectedchapter].CurrentListWidth + _controlwidth;


            /** infite scroll 
            _initthreshold = -1 * 0.0;
            _finalthreshold = _initthreshold;
            for (size_t j = 0; j < _listvector.size(); j++)
                _finalthreshold -= _listvector[j].CurrentListWidth;

            _finalthreshold += _controlwidth; 

            end final scroll*/

            if (_finalthreshold > _initthreshold)
                _finalthreshold = _initthreshold;

            _leftconstant = _listvector[_selectedchapter].CurrentListWidth / 1600.0;
        }

        void animatetoposition(double pos)
        {
            double to = -1 * pos;
            if (to > _initthreshold)
                to = _initthreshold;

            if (to < _finalthreshold)
                to = _finalthreshold;
            _panelanimation.To = to;
            _panelstory.Begin();
        }

        void animatetochapter(int chapter, bool tobegin)
        {
            int ch = chapter;
            bool tb = tobegin;
            if (chapter < 0)
                ch = 0;
            if (chapter >= _numberofitems)
            {
                ch = _numberofitems - 1;
                tb = false;
            }
            if (StackListScrollCompleted != null)
                StackListScrollCompleted(this, ch);
            _selectedchapter = ch;
            if (tb)
            {
                _panelanimation.To = -1 * _listvector[_selectedchapter].Position;
                _panelstory.Begin();
            }
            else
            {
                _panelanimation.To = -1 * (_listvector[_selectedchapter].Position + _listvector[_selectedchapter].CurrentListWidth - _controlwidth);
                _panelstory.Begin();
            }

            _texto.Text = "" + _selectedchapter;
        }

        void updatelistproperties()
        {
            double tempos = 0.0;
            for (int i = 0; i < _listvector.Count; i++)
            {
                _listvector[i].Position = tempos;
                _listvector[i].DistanceToScreen = _paneltransform.TranslateX + tempos;
                _listvector[i].IsManipulating = false;
                _listvector[i].UpDateProperties();

                if (tempos < 0)
                {
                    double a = _listvector[i].CurrentListWidth;
                    double b = _listvector[i].Position;
                }
                tempos += _listvector[i].CurrentListWidth;
            }

            _texto.Text = "" + _selectedchapter;
        }

        #endregion

        #region Manipulation Events Functions
        //private :
        void Panel_PointerReleased_1(object sender, PointerRoutedEventArgs e)
        {
            if (_ismanipulating && !_isinertia)
            {
                _forcemanipulationtoend = true;
            }

            if (!_isitemselected)
            {
                _touches = 0;
                updatelistproperties();

            }

            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("Unlock"));
        }

        void Panel_PointerPressed_1(object sender, PointerRoutedEventArgs e)
        {
            _touches += 1;
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("Lock"));
        }

        void Panel_ManipulationDelta_1(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (_manipulationenable)
            {
                if (_touches < 2)
                {
                    if (_paneltranslate < _initthreshold && _paneltranslate > _finalthreshold)
                    {
                        _paneltranslate += e.Delta.Translation.X;
                        _offsetdelta = 0.0;
                        //compute chapter while scrolling
                        if (-1.0 * _paneltranslate < _listvector[_selectedchapter].Position)
                        {
                            _selectedchapter -= 1;
                            if (_selectedchapter < 0)
                                _selectedchapter = 0;
                            StackListScrollCompleted(this, _selectedchapter);
                        }
                        else
                        {
                            if (_selectedchapter < _listvector.Count - 1)
                                if (-1.0 * _paneltranslate > _listvector[_selectedchapter + 1].Position)
                                {
                                    _selectedchapter += 1;
                                    StackListScrollCompleted(this, _selectedchapter);
                                }
                        }

                    }
                    else
                    {
                        if (e.IsInertial)
                        {
                            e.Complete();
                            return;
                        }
                        _paneltranslate += (e.Delta.Translation.X * 0.7); //0.5
                        _offsetdelta = (e.Delta.Translation.X * 0.7); //0.5
                        //Scroll UPControl
                        if (StackListScrollDelta != null)
                            StackListScrollDelta(this, _offsetdelta);
                    }
                    _paneltransform.TranslateX = _paneltranslate;
                }
                else
                {
                    _isitemselected = true;
                    if (_typeselected == SelectionType.ItemType)
                    {
                        _listvector[_selectedchapter].SelectedStack.SelectedStackItem.ItemTransform.TranslateX += e.Delta.Translation.X;
                        _listvector[_selectedchapter].SelectedStack.SelectedStackItem.ItemTransform.TranslateY += e.Delta.Translation.Y;
                        _listvector[_selectedchapter].SelectedStack.SelectedStackItem.ItemTransform.ScaleX *= e.Delta.Scale;
                        _listvector[_selectedchapter].SelectedStack.SelectedStackItem.ItemTransform.ScaleY *= e.Delta.Scale;
                        _listvector[_selectedchapter].SelectedStack.SelectedStackItem.ItemTransform.Rotation += e.Delta.Rotation;
                    }
                    else
                    {
                        if (_typeselected == SelectionType.StackType)
                        {
                            if (_listvector[_selectedchapter].SelectedStack != null)
                                _listvector[_selectedchapter].SelectedStack.Proportion *= e.Delta.Scale;
                        }
                    }
                }
                _ismanipulating = true;
                _listvector[_selectedchapter].IsManipulating = true;
            }
            else
            {
                e.Complete();
                e.Handled = true;
                return;
            }

            if (_forcemanipulationtoend && !e.IsInertial)
            {
                e.Complete();
                return;
            }
        }

        void Panel_ManipulationCompleted_1(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (_manipulationenable)
            {
                if (_touches < 2 || !_isitemselected)
                {
                    int tchapter = _selectedchapter;
                    bool tobegin = true;
                    if (_paneltranslate > _initthreshold)
                    {
                        if (_paneltranslate > _initthreshold + _borderwidth)
                            tchapter -= 1;
                        //paged scroll
                        animatetochapter(tchapter, tobegin);

                        //infite scroll
                        //animatetochapter(0, true);
                    }
                    else
                    {
                        if (_paneltranslate < _finalthreshold)
                        {
                            if (_paneltranslate < _finalthreshold - _borderwidth)
                                tchapter += 1;
                            else
                                tobegin = false;
                            //paged scroll
                            animatetochapter(tchapter, tobegin);

                            //infite scroll
                            //animatetochapter(_listvector.size() - 1, false);
                        }
                        else
                        {
                            updatelistproperties();
                            _paneltranslate = _paneltransform.TranslateX;
                            _manipulationenable = true;
                            _touches = 0;
                        }
                    }

                    _leftconstant = _listvector[_selectedchapter].CurrentListWidth / 1600.0;

                    //			StackListScrollCompleted(this, tchapter);
                }
                else
                {
                    if (_typeselected == SelectionType.ItemType)
                    {
                        _listvector[_selectedchapter].SelectedStack.SelectedStackItem.ItemManipulationCompleted();
                    }
                    else
                    {
                        _listvector[_selectedchapter].SelectedStack.StackManipulationCompleted();
                    }
                }
            }

            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("Unlock"));
            _touches = 0;
            _offsetdelta = 0.0;
            _forcemanipulationtoend = false;
            _ismanipulating = false;
            _isinertia = false;
            _isitemselected = false;
        }

        void Panel_ManipulationInertiaStarting_1(object sender, ManipulationInertiaStartingRoutedEventArgs e)
        {
            _isinertia = true;
            if (_manipulationenable)
            {
                if (_touches > 1)
                {
                    e.TranslationBehavior.DesiredDeceleration = 300.0 * 96.0 / (1000.0 * 1000.0);
                    e.ExpansionBehavior.DesiredDeceleration = 10.0 * 96.0 / (1000.0 * 1000.0);
                    e.RotationBehavior.DesiredDeceleration = 300.0 * 96.0 / (1000.0 * 1000.0);
                }
            }
        }


        #endregion

        #region Stack Events Functions
        //private:
        void StackItem_FullAnimationStarted(object sender, int chapter, int section, int page)
        {
            _selectedchapter = chapter;
            _selectedpage = page;
            _selectedsection = section;
            StackItemFullAnimationStarted(this, chapter, section, page);
            _manipulationenable = false;
        }

        void StackItem_FullAnimationCompleted(object sender, int chapter, int section, int page)
        {
            StackItemFullAnimationCompleted(this, chapter, section, page);
            SetToItem(chapter, section, page);
            _manipulationenable = true;
        }

        void StackItem_ThumbAnimationStarted(object sender, int chapter, int section, int page)
        {
            _manipulationenable = false;
            _selectedchapter = chapter;
            _selectedpage = page;
            _selectedsection = section;
            if (StackItemThumbAnimationStarted != null)
                StackItemThumbAnimationStarted(this, chapter, section, page);
        }

        void StackItem_ThumbAnimationCompleted(object sender, int chapter, int section, int page)
        {
            _manipulationenable = true;
            _typeselected = SelectionType.StackType;
            if (StackItemThumbAnimationCompleted != null)
                StackItemThumbAnimationCompleted(this, chapter, section, page);
        }

        void StackList_ScrollTo(object sender, double _position)
        {
            double pos = -1 * (_listvector[_selectedchapter].Position + _position);
            if (pos > _initthreshold)
                pos = _initthreshold;
            if (pos < _finalthreshold)
                pos = _finalthreshold;
            _paneltranslate = pos;
            _paneltransform.TranslateX = pos;
        }

        void StackList_AnimateTo(object sender, double _position)
        {
            computethresholds();
            double p = _listvector[_selectedchapter].Position;
            animatetoposition(_listvector[_selectedchapter].Position + _position);
        }

        void StackList_WidthChanged(object sender, double _position)
        {

        }

        void IControls_ComponentSelected(object sender, SelectionType t, int index)
        {
            _selectedchapter = index;
            _typeselected = _listvector[index].TypeSelected;
            _selectedsection = _listvector[index].SelectedStack.StackNumber;
            _selectedpage = _listvector[index].SelectedStack.SelectedStackItem.ItemNumber;
        }

        #endregion


        #region Animation

        //private: 
        Storyboard _panelstory;
        DoubleAnimation _panelanimation;
        void initanimationproperties()
        {

            _panelstory = new Windows.UI.Xaml.Media.Animation.Storyboard();
            _panelanimation = new Windows.UI.Xaml.Media.Animation.DoubleAnimation();
            _panelanimation.Duration = TimeSpan.FromMilliseconds(600); //1000
            _panelstory.Children.Add(_panelanimation);
            Windows.UI.Xaml.Media.Animation.CubicEase ease1 = new Windows.UI.Xaml.Media.Animation.CubicEase();
            ease1.EasingMode = Windows.UI.Xaml.Media.Animation.EasingMode.EaseOut;
            _panelanimation.EasingFunction = ease1;
            Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(_panelanimation, "TranslateX");
            Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(_panelanimation, _paneltransform);
            _panelstory.Completed += Storyboard_Completed_1;
        }

        void Storyboard_Completed_1(object sender, object e)
        {
            _paneltranslate = _paneltransform.TranslateX;
            updatelistproperties();
            computethresholds();
            _manipulationenable = true;
            _touches = 0;
        }

        #endregion
    }
}
