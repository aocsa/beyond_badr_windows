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

namespace StackView
{
    public sealed partial class ControlDownMenu : Grid
    {
        // public:
        public ControlDownMenu()
        {
            init();
        }

        public event ControlDownElementSelectedEventHandler ControlDownElementSelected;

        public BookDataSource Source
        {
            set
            {
                _source = value;
                initstack();

            }
            get { return _source; }
        }


        public void SelectElement(int index)
        {
            if (index != _currentindex && _elements.Count > 0)
            {
                if (_currentindex >= 0)
                    _elements[_currentindex].Unselect();
                _currentindex = index;
                
                _elements[_currentindex].Select();
            }

        }

        //private:

        BookDataSource _source;
        int _currentindex;
        ScrollViewer _mainscroll;
        StackPanel _mainpanel;
        List<ControlDownElement> _elements = new List<ControlDownElement>();

        void init()
        {
            _currentindex = 0 ;

            Height = 102.0;
            Width = 1600.0;
            VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Bottom;

            _mainscroll = new ScrollViewer();
            _mainscroll.HorizontalScrollMode = ScrollMode.Enabled;
            _mainscroll.VerticalScrollMode = ScrollMode.Disabled;
            _mainscroll.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            _mainscroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            _mainscroll.Height = 102.0;
            _mainscroll.Width = 1600.0;
            Children.Add(_mainscroll);

            _mainpanel = new StackPanel();
            _mainpanel.Orientation = Orientation.Horizontal;
            _mainpanel.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left;
            _mainscroll.Content = _mainpanel;

            _iscomponentinit = false;
        }

        bool _iscomponentinit;
        void initstack()
        {
            for (int i = 0; i < _source.Chapters.Count; i++)
            {
                ControlDownElement elem = new ControlDownElement();
                elem.Index = i;
                elem.Source = _source.Chapters[i];
                elem.ControlDownElementSelected += ControlDown_ElementSelected;
                _mainpanel.Children.Add(elem);
                _elements.Add(elem);
                if (i == 0) elem.Select();
            }
            _iscomponentinit = true;
        }

        void ControlDown_ElementSelected(object sender, int index)
        {
            SelectElement(index);
            ControlDownElementSelected(this, index);
        }

    }
}
