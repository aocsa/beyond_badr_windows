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

namespace MLearning.Store.Components
{
    public sealed partial class CommentsContainer : Grid
    {
        public CommentsContainer()
        {
            this.Width = 720;
            this.Height =  328 ;
            init();
        }


        #region Properties

        bool _isCirle = true;
        public bool IsCircle
        {
            get { return _isCirle;}
            set { _isCirle = value; }
        }

        ObservableCollection<MainViewModel.post_with_username_wrapper> _postsList;
        public ObservableCollection<MainViewModel.post_with_username_wrapper> PostsList
        {
            get { return _postsList; }
            set { _postsList = value;
            if (_postsList != null)
            {
                foreach (MainViewModel.post_with_username_wrapper c in _postsList)
                {
                    var newcom = new CommentItem();
                    newcom.UserImage.Background = new ImageBrush()
                    {
                        Stretch = Stretch.UniformToFill,
                        ImageSource = Constants.ByteArrayToImageConverter.Convert(c.userImage)
                    };
                    newcom.CommentText.Text = c.post.text;
                    newcom.NameText.Text = c.post.name + " " + c.post.lastname;
                    newcom.DateText.Text = c.post.created_at.ToString();
                    _mainpanel.Children.Add(newcom);
                }
            }

            _postsList.CollectionChanged += _postsList_CollectionChanged;
            }
        }

        void _postsList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var comments = sender as ObservableCollection<MainViewModel.post_with_username_wrapper>;
            if (comments != null)
            {
                //start from new startindex
                for (int i = e.NewStartingIndex; i < comments.Count; i++)
                {
                    MainViewModel.post_with_username_wrapper c = comments[i];
                    var newcom = new CommentItem();
                    newcom.UserImage.Background = new ImageBrush() { Stretch = Stretch.UniformToFill, ImageSource = Constants.ByteArrayToImageConverter.Convert(c.userImage) };
                    c.PropertyChanged += (s, a) =>
                    {
                        newcom.UserImage.Background = new ImageBrush() { Stretch = Stretch.UniformToFill, ImageSource = Constants.ByteArrayToImageConverter.Convert(c.userImage) };
                    };
                    newcom.CommentText.Text = c.post.text;
                    newcom.NameText.Text = c.post.name + " " + c.post.lastname;
                    newcom.DateText.Text = "    " + c.post.created_at.Date.ToString();
                    _mainpanel.Children.Add(newcom);
                }
            }
                
        }


        ObservableCollection<MainViewModel.lo_comment_with_username_wrapper> _loCommentsList;
        public ObservableCollection<MainViewModel.lo_comment_with_username_wrapper> LOCommentsList
        {
            get { return _loCommentsList; }
            set
            {
                _loCommentsList = value;
                if (_loCommentsList != null)
                {
                    foreach(MainViewModel.lo_comment_with_username_wrapper c in _loCommentsList)
                    {
                        var newcom = new CommentItem();
                        newcom.UserImage.Background = new ImageBrush()
                        {
                            Stretch = Stretch.UniformToFill,
                            ImageSource = Constants.ByteArrayToImageConverter.Convert(c.userImage)
                        };
                        newcom.CommentText.Text = c.lo_comment.text;
                        newcom.NameText.Text = c.lo_comment.name + " " + c.lo_comment.lastname;
                        newcom.DateText.Text = c.lo_comment.created_at.ToString();
                        _mainpanel.Children.Add(newcom);
                    }
                }
                _loCommentsList.CollectionChanged +=_loCommentsList_CollectionChanged;
            }
        }


        void _loCommentsList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var comments = sender as ObservableCollection<MainViewModel.lo_comment_with_username_wrapper>;
            if (comments != null)
            {
                //start from new startindex
                for (int i = e.NewStartingIndex; i < comments.Count; i++)
                {
                    MainViewModel.lo_comment_with_username_wrapper c = comments[i];
                    var newcom = new CommentItem();
                    newcom.UserImage.Background = new ImageBrush() { Stretch = Stretch.UniformToFill, ImageSource = Constants.ByteArrayToImageConverter.Convert(c.userImage) };
                    c.PropertyChanged += (s, a) =>
                    {
                        newcom.UserImage.Background = new ImageBrush() { Stretch = Stretch.UniformToFill, ImageSource = Constants.ByteArrayToImageConverter.Convert(c.userImage) };
                    };
                    newcom.CommentText.Text = c.lo_comment.text;
                    newcom.NameText.Text = c.lo_comment.name + " " + c.lo_comment.lastname;
                    newcom.DateText.Text = "    " + c.lo_comment.created_at.Date.ToString();
                    _mainpanel.Children.Add(newcom);
                }
            }
                
        }


        #endregion


        ScrollViewer _mainScroll;
        StackPanel _mainpanel;
        void init()
        {
            _mainScroll = new ScrollViewer()
            {
                Width = 720,
                Height = 328,
                VerticalScrollMode = ScrollMode.Auto,
                HorizontalScrollMode = ScrollMode.Disabled,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility =  ScrollBarVisibility.Disabled
            };
            this.Children.Add(_mainScroll);

            _mainpanel = new StackPanel()
            {
                Width = 720,
                Orientation = Orientation.Vertical,
            };

            _mainScroll.Content = _mainpanel;
        }
 







        #region Comment Item

        public sealed partial class CommentItem : Grid
        {
            Border _userimage;
            TextBlock _nameText, _dateText, _commentText;
            StackPanel mainpanel;
            

            public CommentItem()
            {
                this.Height = 80;
                this.Width = 720;
                init();
            }

            #region Item Properties

            string _imagepath, _name, _date, _comment;
             
            public Border UserImage
            {
                get { return _userimage ;}
            }

            public TextBlock NameText
            {
                get { return _nameText; }
            }

            public TextBlock DateText
            {
                get { return _dateText; }
            }

            public TextBlock CommentText
            {
                get { return _commentText; }
            }

            #endregion

            void init()
            {
                _userimage = new Border()
                {
                    Width = 50,
                    Height = 50,
                    RenderTransform = new TranslateTransform() { X=32 },
                    HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left,
                    CornerRadius = new CornerRadius(25)
                };
                this.Children.Add(_userimage);

                mainpanel = new StackPanel()
                {
                    Width = 580,
                    Height = 44,
                    Orientation = Orientation.Vertical,
                    VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center,
                    HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left ,
                    RenderTransform = new TranslateTransform() {X = 106}
                };
                this.Children.Add(mainpanel);
                StackPanel namepanel = new StackPanel() { Width = 580, Height = 22, Orientation = Orientation.Horizontal };
                
                _nameText = new TextBlock() { Foreground = new SolidColorBrush(Colors.Black) , FontSize =  14, TextAlignment = TextAlignment.Left ,
                VerticalAlignment= Windows.UI.Xaml.VerticalAlignment.Center};
                _dateText = new TextBlock() { Foreground =  new SolidColorBrush(Colors.Gray), FontSize = 10 , TextAlignment = TextAlignment.Left,
                VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center};
                _commentText = new TextBlock() { Foreground =  new SolidColorBrush(Colors.Gray), TextAlignment = TextAlignment.Left , FontSize =  14,
                HorizontalAlignment =  Windows.UI.Xaml.HorizontalAlignment.Left , VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center, TextWrapping = TextWrapping.Wrap};

                namepanel.Children.Add(_nameText);
                namepanel.Children.Add(_dateText);

                mainpanel.Children.Add(namepanel);
                mainpanel.Children.Add(_commentText);

                _commentText.LayoutUpdated += _commentText_LayoutUpdated;

                
            }

            void _commentText_LayoutUpdated(object sender, object e)
            {  
                if (_commentText.ActualHeight > 22)
                {
                    mainpanel.Height = 22 + _commentText.ActualHeight;
                    Height = 80 + (_commentText.ActualHeight - 22);
                }
            }
            
        }

        #endregion

    }
}
