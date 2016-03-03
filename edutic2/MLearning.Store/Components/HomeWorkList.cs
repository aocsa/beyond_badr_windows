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
using MLearning.Core.ViewModels;
using MLearning.Store.Components;
using System.Collections.ObjectModel;
using MLearningDB;

namespace MLearning.Store.Components
{
    public delegate void HWItemSelectedEventHandler(object sender, int index) ;

    public sealed partial class HomeWorkList : Grid
    {
        ScrollViewer _mainscroll;
        StackPanel _mainpanel; 
        String _icon;

        public HomeWorkList(double w, double h, string icon)
        {
            this.Width = w;
            this.Height = h;
            _icon = icon;
            _mainpanel = new StackPanel() 
            {
                Width = w,
                Orientation = Orientation.Vertical,
                VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Top                 
            };

            _mainscroll = new ScrollViewer()
            {
                Width = w,
                Height =h ,
                HorizontalScrollMode = ScrollMode.Disabled,
                VerticalScrollMode = ScrollMode.Enabled,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };

            this.Children.Add(_mainscroll);
            _mainscroll.Content = _mainpanel;
        }


        ObservableCollection<quiz_by_circle> _quizzesList;
        public ObservableCollection<quiz_by_circle> QuizzesList
        {
            get { return _quizzesList; }
            set {
                _quizzesList = value;
                addElements(0);
                _quizzesList.CollectionChanged += _quizzesList_CollectionChanged;
            }
        }

        void _quizzesList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            addElements(e.NewStartingIndex);
        }

        void addElements(int index)
        {
            for (int i = index; i < _quizzesList.Count; i++)
            {
                HomeWorkItem item = new HomeWorkItem() 
                {
                    Index = i,
                    IconUri = _icon,
                    WorkText = _quizzesList[i].content
                };
                _mainpanel.Children.Add(item);
            }
        }
        

    }

    #region ItemList

    public sealed partial class HomeWorkItem : Grid
    {
        Image _itemimage;
        TextBlock _itemtext;
        //public event HWItemSelectedEventHandler HWItemSelected;

        public HomeWorkItem()
        {
            Width = 316;
            Height = 52;
            init();
           // Tapped += HomeWorkItem_Tapped;
        }

        /**void HomeWorkItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            HWItemSelected(this,Index);
        }*/


        bool _isselected = false;
        public bool IsSelected
        {
            get { return _isselected; }
            set {
                _isselected = value;
                //if (_isselected) this.Background = new SolidColorBrush(ColorHelper.FromArgb(255, 78, 177, 223));
                //else this.Background = new SolidColorBrush(Colors.Transparent);
            }
        }

        int _index = 0;
        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }

        public string IconUri
        {
            set {
                _itemimage.Source = new BitmapImage(new Uri(value));
            }
        }

        public string WorkText
        {
            set { _itemtext.Text = value; }
        }

        void init()
        {
            _itemimage = new Image()
            {
                Width = 20,
                Height = 20,
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left,
                RenderTransform = new TranslateTransform() { X = 22 }
            };

            Children.Add(_itemimage);

            _itemtext = new TextBlock()
            {
                Width = 240,
                Height = 20,
                RenderTransform = new TranslateTransform() { X = 54 },
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left,
                FontSize = 14,
                Foreground = new SolidColorBrush(Colors.White)
            };
            Children.Add(_itemtext);
            
        }
        
    }

    #endregion
}
