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
using System.Collections.ObjectModel;
using MLearning.Store.MLStyles;


namespace MLearning.Store.Components
{
    public sealed partial class LOContainer : Grid
    {
        List<LOItem> _itemsList = new List<LOItem>();
        static double Separation = 8;
        static double ItemWitdh = 116, ItemHeight = 138;
        StackPanel _mainpanel;
        ScrollViewer _mainscroll;
        public LOContainer()
        {
            this.Width = 628;
            Height = 162;
            Background = new SolidColorBrush(Colors.Transparent);
        }

        public LOContainer(double w, double h)
        {
            Width = w;
            Height = h;
            _mainscroll = new ScrollViewer() 
            {
                Width = w ,
                Height = h,
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left,
                HorizontalScrollMode = ScrollMode.Enabled,
                VerticalScrollMode = ScrollMode.Disabled,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility = ScrollBarVisibility.Disabled,
                ZoomMode = ZoomMode.Disabled
            };
            this.Children.Add(_mainscroll);

            _mainpanel = new StackPanel() { Orientation = Orientation.Horizontal };
            _mainscroll.Content = _mainpanel;

        }

        #region Properties

        public event SelectedEventHandler LOSelected;
        public event DoLikeEventHandler LOLiked;

        ObservableCollection<MainViewModel.lo_by_circle_wrapper> _learningObjectsList;
        public ObservableCollection<MainViewModel.lo_by_circle_wrapper> LearningOjectsList
        {
            get { return _learningObjectsList; }
            set
            {
                _learningObjectsList = value;
                if (_learningObjectsList != null) additems(0);
                _learningObjectsList.CollectionChanged += _learningObjectsList_CollectionChanged;
            }
        }

        void _learningObjectsList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if(e.NewStartingIndex >=0)
                additems(e.NewStartingIndex);
        }



        private Color _likecolor;

        public Color LikeColor
        {
            get { return _likecolor; }
            set {
                _likecolor = value;
                for (int i = 0; i < _itemsList.Count; i++)
                {
                    _itemsList[i].LikeColor = _likecolor;
                }
            }
        }
        

        #endregion

         

        void additems(int index)
        {         
            for (int i = index; i < _learningObjectsList.Count; i++)
            {
                Grid container = new Grid() { Width = ItemWitdh + Separation , Height = ItemHeight + Separation };
                LOItem item = new LOItem()
                {
                    Index = i,
                    RenderTransform = new TranslateTransform()
                    {
                        X = Separation ,
                        Y = Separation
                    },
                    Title = _learningObjectsList[i].lo.title,
                    Author = _learningObjectsList[i].lo.name + " " + _learningObjectsList[i].lo.lastname,
                    LikeThis = _learningObjectsList[i].lo.like,
                    BorderColor = StaticStyles.Colors[_learningObjectsList[i].lo.color_id%4].MainColor
                };

                item.Selected += item_Selected;
                item.DoLike += item_DoLike;
                _itemsList.Add(item);
                //
                item.ImageBytes = _learningObjectsList[i].cover_bytes;
                _learningObjectsList[i].PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == "cover_bytes")
                        item.ImageBytes = (s as MainViewModel.lo_by_circle_wrapper).cover_bytes;

                    if (e.PropertyName == "background_bytes")
                    { }
                };

                container.Children.Add(item);
                _mainpanel.Children.Add(container);
            }
        }

        void item_DoLike(object sender, int index)
        {
            LOLiked(this, index);
        }

        void item_Selected(object sender, int index)
        {
            LOSelected(this,index);
            for (int i = 0; i < _itemsList.Count; i++)
            {
                if (i == index) _itemsList[i].IsSelected = true;
                else _itemsList[i].IsSelected = false;

                _itemsList[i].LikeColor = _itemsList[index].BorderColor;
            }
        }


        public void ResetSelection()
        {
            for (int i = 0; i < _itemsList.Count; i++) 
                _itemsList[i].IsSelected = false; 
        }

    }
}
