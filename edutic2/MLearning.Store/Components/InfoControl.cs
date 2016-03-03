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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
using Windows.UI;
using Windows.UI.Xaml.Media.Animation;
using MLReader;

namespace MLearning.Store.Components
{
   
    public sealed partial class InfoControl : Grid
    {
        TextBlock _content;
        Image _stateimage;
        public InfoControl()
        {
            this.Width = 248;
            this.Height = 50;
            StackPanel panel = new StackPanel() { Width = 230, Height = 24, Orientation = Orientation.Horizontal}; 
            _content = new TextBlock() { FontSize =  14 , Width =  180, Height = 16};
            _stateimage = new Image() { Width = 34, Height=12  };
            this.Children.Add(panel) ;
            panel.Children.Add(_stateimage);
            panel.Children.Add(_content);          
        }


        int _index = 0;
        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }

        bool _canselect = true;
        public bool CanSelect
        {
            get { return _canselect; }
            set { _canselect = value; }
        }

        string _contentstring;
        public string ContentBlock
        {
            get { return _contentstring; }
            set { _contentstring = value;
            _content.Text = value;
            }
        }

        public bool _ison = false;
        public bool IsOn
        {
            get { return _ison; }
            set 
            {
                _ison = value;
                if (_ison) seton();
                else setoff();
            }
        }

        public bool _isselected = false;
        public bool IsSelected
        {
            get { return _isselected; }
            set 
            {
                if(CanSelect)
                    _isselected = value;
                if (_isselected)
                    select();
                else unselect();
            }
        }

        string _onimage;
        public string OnImage
        {
            get { return _onimage; }
            set { _onimage = value; }
        }

        string _offimage;
        public string OffImage
        {
            get { return _offimage; }
            set { _offimage = value; }
        }

        string _selectonimage;
        public string SelectOnImage
        {
            get { return _selectonimage; }
            set { _selectonimage = value; }
        }

        string _selectoffimage;
        public string SelectOffImage
        {
            get { return _selectoffimage; }
            set { _selectoffimage = value; }
        }

        

        void seton()
        {
            switch (IsSelected)
            {
                case true:
                    _stateimage.Source = new BitmapImage(new Uri(SelectOnImage));
                    _content.Foreground = new SolidColorBrush(Colors.White);
                    break;
                case false:
                    _stateimage.Source = new BitmapImage(new Uri(OnImage));
                    _content.Foreground = new SolidColorBrush(Colors.Black);
                    break;
            }
        }

        void setoff()
        {
            switch (IsSelected)
            {
                case true:
                    _stateimage.Source = new BitmapImage(new Uri(SelectOffImage));
                    _content.Foreground = new SolidColorBrush(Colors.White);
                    break;
                case false:
                    _stateimage.Source = new BitmapImage(new Uri(OnImage));
                    _content.Foreground = new SolidColorBrush(Colors.DarkGray);
                    break;
            }
        }


        private Color _infocolor;

        public Color InfoColor
        {
            get { return _infocolor; }
            set { _infocolor = value; if (IsSelected) this.Background = new SolidColorBrush(_infocolor); }
        }
        

        void select()
        {
            StyleConstants styles = new StyleConstants();

            this.Background = new SolidColorBrush(_infocolor);//ColorHelper.FromArgb(255,78,177,223));
            switch (IsOn)
            {
                case true:
                    _stateimage.Source = new BitmapImage(new Uri(SelectOnImage));
                    _content.Foreground = new SolidColorBrush(Colors.White);
                    break;
                case false:
                    _stateimage.Source = new BitmapImage(new Uri(SelectOffImage));
                    _content.Foreground = new SolidColorBrush(Colors.White);
                    break;
            }
        }


        void unselect()
        {
            this.Background =  new SolidColorBrush(Colors.Transparent);
            switch (IsOn)
            {
                case true:
                    _stateimage.Source = new BitmapImage(new Uri(OnImage));
                    _content.Foreground = new SolidColorBrush(Colors.Black);
                    break;
                case false:
                    _stateimage.Source = new BitmapImage(new Uri(OffImage));
                    _content.Foreground = new SolidColorBrush(Colors.DarkGray);
                    break;
            }
        }

    }
}
