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

using Cirrious.MvvmCross.WindowsCommon.Views;
using Windows.UI.Xaml.Media.Imaging;

using MLearning.Store.Components;
using Core.ViewModels;
using MLearning.Core.ViewModels;
using Windows.UI;
using Windows.UI.Xaml.Media.Animation;
using MLearning.Store.MLStyles;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MLearning.Store.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainView : MvxWindowsPage
    {

        bool _isCircleSelected = true;
        int _loIndexSelected = 0;
        LOContainer _lo_container;
        LoadingView _loadingview;
        public MainView()
        {
            this.InitializeComponent();
            Loaded += MainView_Loaded;
        }

        void MainView_Loaded(object sender, RoutedEventArgs e)
        {
            ListGrid.Background = new SolidColorBrush(ColorHelper.FromArgb(220,246,240,234));
            DetailGrid.Background = new SolidColorBrush(ColorHelper.FromArgb(200, 255, 255, 255));
            
            BackgroundImage.BackSource = new BitmapImage(new Uri(StaticStyles.DefaultMuroBackground));
            BackgroundImage.BrushOpacity = 0.1;

            LogoFadeImage.BackSource = new BitmapImage(new Uri(StaticStyles.DefaultLogoUri));

            inituserproperties();
            initCircleScroll();
            initPeopleScroll();
            resetMLOs();
            if ((ViewModel as MainViewModel).PostsList != null)
                resetComments();
            if ((ViewModel as MainViewModel).LOCommentsList != null)
                resetMLOComments();
            if ((ViewModel as MainViewModel).PendingQuizzesList != null)
                resetPendingQuizzes();
            if ((ViewModel as MainViewModel).CompletedQuizzesList != null)
                resetCompleteQuizzes();

            _loadingview = new LoadingView();
            MainGrid.Children.Add(_loadingview);
            Canvas.SetZIndex(_loadingview, -10);
        }

        void inituserproperties()
        {
            var vm = this.ViewModel as MainViewModel;
            NameTextBlock.Text = vm.UserFirstName;
            LastnameTextBlock.Text = vm.UserLastName;
            UserImageView.ImageSource = Constants.ByteArrayToImageConverter.Convert(vm.UserImage);
            vm.PropertyChanged += vm_PropertyChanged;
           
        }

        void vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var vm = this.ViewModel as MainViewModel;
            string property = e.PropertyName;
            switch (property)
            {
                case "UserFirstName":
                    NameTextBlock.Text = vm.UserFirstName;
                    break;
                case "UserLastName":
                    LastnameTextBlock.Text = vm.UserLastName;
                    break;
                case "UserImage":
                    UserImageView.ImageSource = Constants.ByteArrayToImageConverter.Convert(vm.UserImage);
                    break;
                case "CirclesList":
                    populateCircleScroll(0);
                    (ViewModel as MainViewModel).CirclesList.CollectionChanged += CirclesList_CollectionChanged;
                    break;
                case "LoadCirclePosts":
                    _isCircleSelected = true;
                    if (_lo_container != null) _lo_container.ResetSelection();
                    setUnidadInvisible();
                    resetComments(); 
                     UnidadNumber.Text = "?"; 
                     CursoNumber.Text = "" + vm.PendingQuizzesList.Count;
                    break;
                case "UsersList":
                    populatePeopleScroll(0);
                    (ViewModel as MainViewModel).UsersList.CollectionChanged += UsersList_CollectionChanged;
                    break;
                case "LearningOjectsList":
                    resetMLOs();
                    break;
                case "PostsList":
                    resetComments();
                    break;
                case "LOCommentsList":
                    resetMLOComments();
                    break;
                case "PendingQuizzesList":
                    resetPendingQuizzes();
                    break;
                case "CompletedQuizzesList":
                    resetCompleteQuizzes();
                    break;
                case "BackgroundImage":
                    resetbackground() ;
                    break;
                default:
                    break;
            }
        }


        void resetbackground()
        {
            BackgroundImage.NewSource = Constants.ByteArrayToImageConverter.Convert((ViewModel as MainViewModel).BackgroundImage);
            BackgroundImage.BrushOpacity = 0.7;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            (this.ViewModel as MainViewModel).CircleFilterString = SearchBox.Text;
            
        }

 



        #region Circle and People Views

        ScrollViewer _circlescroll, _peoplescroll; 
        List<InfoControl> _currentCircles;
        void initCircleScroll()
        {   
            _circlescroll = new ScrollViewer()
            {
                Width = 248,
                Height = 308,
                HorizontalScrollMode = ScrollMode.Disabled,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                VerticalScrollMode = ScrollMode.Enabled,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };
            
            CirclesGrid.Children.Add(_circlescroll);
            populateCircleScroll(0); 

        }

        void CirclesList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            populateCircleScroll(e.NewStartingIndex);
        }

        void populateCircleScroll(int index)
        {
            var vm = ViewModel as MainViewModel;
            _currentCircles = new List<InfoControl>();
            if (vm.CirclesList != null)
            {
                CirclesRing.Visibility = Visibility.Collapsed;
                StackPanel circlepanel = new StackPanel() { Orientation = Orientation.Vertical };
                for (int i = index; i < vm.CirclesList.Count; i++)
                {
                    var newcircle = new InfoControl()
                    {
                        Index = i,
                        CanSelect = true,
                        OnImage = "ms-appx:///Resources/muro/greenpop.png",
                        OffImage = "ms-appx:///Resources/muro/graypop.png",
                        SelectOnImage = "ms-appx:///Resources/muro/greenpop.png",
                        SelectOffImage = "ms-appx:///Resources/muro/whitepop.png",
                        IsSelected = false,
                        ContentBlock = vm.CirclesList[i].name,
                        IsOn = true ,
                        InfoColor = StaticStyles.DefaultColor 
                    };
                    if (i == 0)
                    {
                        newcircle.IsSelected = true;
                        CircleNameText.Text = vm.CirclesList[i].name;
                        _isCircleSelected = true;
                    }
                    circlepanel.Children.Add(newcircle);
                    newcircle.Tapped +=newcircle_Tapped;
                    _currentCircles.Add(newcircle);
                }
                _circlescroll.Content = circlepanel;

                UnidadNumber.Text = "?";
                if(vm.PendingQuizzesList!=null)
                    CursoNumber.Text = "" + vm.PendingQuizzesList.Count;
            }
        }

        private void newcircle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            _peoplescroll.Content = new Grid();
            PeopleRing.Visibility = Visibility.Visible;
            CommentsRing.Visibility = Visibility.Visible;
            PendingRing.Visibility = Visibility.Visible;
            CompleteRing.Visibility = Visibility.Visible;
            CircleCommentsGrid.Children.Clear();
            var circle = sender as InfoControl;
            circle.IsSelected = true;
            var vm = ViewModel as MainViewModel ;
            vm.SelectCircleCommand.Execute(vm.CirclesList[circle.Index]);
            for (int i = 0; i < _currentCircles.Count; i++)
                if(circle.Index != i)
                    _currentCircles[i].IsSelected = false;

            CircleNameText.Text = vm.CirclesList[circle.Index].name;
            UnitsText.Visibility = Visibility.Visible;
            SingleUnitPanel.Visibility = Visibility.Collapsed;
            _isCircleSelected = true;

            setUnidadInvisible();
        }


        void initPeopleScroll()
        { 
            _peoplescroll = new ScrollViewer()
            {
                Width = 248,
                Height = 302,
                HorizontalScrollMode = ScrollMode.Disabled,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                VerticalScrollMode = ScrollMode.Enabled,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };

            PeopleGrid.Children.Add(_peoplescroll);
            populatePeopleScroll(0); 
        }

        void UsersList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            populatePeopleScroll(e.NewStartingIndex);
        }


        void populatePeopleScroll(int index)
        {
            var vm = ViewModel as MainViewModel;
            if (vm.UsersList != null)
            { 
                StackPanel peoplepanel = new StackPanel() { Orientation = Orientation.Vertical };
                for (int i = 0; i < vm.UsersList.Count; i++)
                {
                    var newinfo = new InfoControl()
                    {
                        Index = i,
                        CanSelect = false,
                        OnImage = "ms-appx:///Resources/muro/greencircle.png",
                        OffImage = "ms-appx:///Resources/muro/whitecircle.png",
                        SelectOnImage = "ms-appx:///Resources/muro/greencircle.png",
                        SelectOffImage = "ms-appx:///Resources/muro/whitecircle.png",
                        IsSelected = false,
                        ContentBlock = vm.UsersList[i].user.name +" "+  vm.UsersList[i].user.lastname,
                        IsOn = true,
                        InfoColor = StaticStyles.DefaultColor
                    };
                    peoplepanel.Children.Add(newinfo); 
                }
                _peoplescroll.Content = peoplepanel;
                PeopleRing.Visibility = Visibility.Collapsed;
            }
        }
         

        #endregion

         

        #region MLOs Scroll and Comment by Circle 
        void resetMLOs()
        {
            MLOsGrid.Children.Clear();
            var vm = ViewModel as MainViewModel;
            if(vm.LearningOjectsList!=null)
            {                
                var lo_container = new LOContainer(MLOsGrid.Width, MLOsGrid.Height);
                MLOsGrid.Children.Add(lo_container); 
                lo_container.LearningOjectsList = vm.LearningOjectsList;
                lo_container.LOSelected += lo_container_LOSelected;
                lo_container.LOLiked += lo_container_LOLiked;
                _lo_container = lo_container;
            }
            MLOsRing.Visibility = Visibility.Collapsed;
            UnidadNumber.Text = "?";
            if (vm.PendingQuizzesList != null)
                CursoNumber.Text = "" + vm.PendingQuizzesList.Count;

            BackgroundImage.NewSource = new BitmapImage(new Uri(StaticStyles.DefaultMuroBackground));
            BackgroundImage.BrushOpacity = 0.1;

            //reset los colors and logo image
            for (int i = 0; i < _currentCircles.Count; i++) 
                _currentCircles[i].InfoColor = StaticStyles.DefaultColor;
            LogoFadeImage.NewSource = new BitmapImage(new Uri("ms-appx:///Resources/muro/logo.png"));
        }

        void lo_container_LOLiked(object sender, int index)
        {
            var vm = ViewModel as MainViewModel;
            vm.LikeLOCommand.Execute(vm.LearningOjectsList[index]);
        }

        void lo_container_LOSelected(object sender, int index)
        {
            PendingRing.Visibility = Visibility.Visible;
            CompleteRing.Visibility = Visibility.Visible;

            PendingGrid.Children.Clear();
            CompleteGrid.Children.Clear();

            CommentsRing.Visibility = Visibility.Visible;
            CircleCommentsGrid.Visibility = Visibility.Collapsed;
            MLOCommentsGrid.Visibility = Visibility.Collapsed;

            var vm = ViewModel as MainViewModel;

            if (vm.PendingQuizzesList != null)
                UnidadNumber.Text = "" + vm.PendingQuizzesList.Count;
            CursoNumber.Text = "?";
            setUnidadVisible(index);
            //execute
            vm.SelectLOCommand.Execute(vm.LearningOjectsList[index]);
            //set color of lo
            OpenLOImage.Source = new BitmapImage(new Uri(StaticStyles.OpenLOUrls[index%4]));
            //reset color of circle
            for (int i = 0; i < _currentCircles.Count; i++) 
                _currentCircles[i].InfoColor = StaticStyles.Colors[index % 4].SecondColorA; 
            //set logo image
            LogoFadeImage.NewSource = new BitmapImage(new Uri(StaticStyles.LogosMuro[index%4]));

            _isCircleSelected = false;
            _loIndexSelected = index;
           
        }

        void resetComments()
        {
            _isCircleSelected = true;
            CircleCommentsGrid.Children.Clear();
            BarTarea.Source = new BitmapImage(new Uri("ms-appx:///Resources/muro/tareas/barra_curso.png"));
            CommentsContainer comm_byList = new CommentsContainer();
            comm_byList.PostsList = (ViewModel as MainViewModel).PostsList;            
            CircleCommentsGrid.Children.Add(comm_byList);
            MLOCommentsGrid.Visibility = Visibility.Collapsed;
            CircleCommentsGrid.Visibility = Visibility.Visible;
            CommentsRing.Visibility = Visibility.Collapsed;
            
        }

        void resetMLOComments()
        {
            _isCircleSelected = false;
            MLOCommentsGrid.Children.Clear();
            BarTarea.Source = new BitmapImage(new Uri("ms-appx:///Resources/muro/tareas/barra_unidad.png"));
            CommentsContainer comm_byList = new CommentsContainer();
            comm_byList.LOCommentsList = (ViewModel as MainViewModel).LOCommentsList;
            MLOCommentsGrid.Children.Add(comm_byList);
            MLOCommentsGrid.Visibility = Visibility.Visible;
            CircleCommentsGrid.Visibility = Visibility.Collapsed;
            CommentsRing.Visibility = Visibility.Collapsed;
            
        }

        #endregion 


        #region Tareas

        void resetPendingQuizzes()
        {
            PendingGrid.Children.Clear();
            HomeWorkList hlist = new HomeWorkList(316, 358, "ms-appx:///Resources/muro/tareas/incomplete_icon.png");
            hlist.QuizzesList = (ViewModel as MainViewModel).PendingQuizzesList;
            PendingGrid.Children.Add(hlist);
            PendingRing.Visibility = Visibility.Collapsed;

            if (_isCircleSelected)
            {
                UnidadNumber.Text = "?";
                CursoNumber.Text = "" + (ViewModel as MainViewModel).PendingQuizzesList.Count;
            }
            else
            {
                UnidadNumber.Text = "" + (ViewModel as MainViewModel).PendingQuizzesList.Count;
                CursoNumber.Text = "?";
            }
        }

        void resetCompleteQuizzes()
        {
            CompleteGrid.Children.Clear();
            HomeWorkList hlist = new HomeWorkList(316, 358, "ms-appx:///Resources/muro/tareas/complete_icon.png");
            hlist.QuizzesList = (ViewModel as MainViewModel).CompletedQuizzesList;
            CompleteGrid.Children.Add(hlist);
            CompleteRing.Visibility = Visibility.Collapsed;
        }

        #endregion
         


        private void Unidad_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        private void Open_Tapped(object sender, TappedRoutedEventArgs e)
        {

            Canvas.SetZIndex(_loadingview, 100);

            var vm = ViewModel as MainViewModel;
            vm.OpenLOCommand.Execute(vm.LearningOjectsList[_loIndexSelected]);
            //loading view
            
        }

        private void CircleNameText_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var cmd = (ViewModel as MainViewModel).BackToCirclePostsCommand;
            cmd.Execute(null);
        }

        void setUnidadVisible(int index)
        {
            UnitNameBlock.Text = (ViewModel as MainViewModel).LearningOjectsList[index].lo.title;
            SingleUnitPanel.Visibility = Visibility.Visible;
            UnitsText.Visibility = Visibility.Collapsed;
        }


        void setUnidadInvisible()
        { 
            SingleUnitPanel.Visibility = Visibility.Collapsed;
            UnitsText.Visibility = Visibility.Visible;
        }

        private void DoComment_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var vm = ViewModel as MainViewModel;
            if (_isCircleSelected)
            {                
                vm.NewPost = NewCommentBox.Text;
                vm.PostCommand.Execute(null);
            }
            else
            {
                vm.NewLOComment = NewCommentBox.Text;
                vm.CreateLOCommentCommand.Execute(null);
            }
            NewCommentBox.Text = string.Empty;
        }

        private void back_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var cmd = (ViewModel as MainViewModel).LogoutCommand;
            cmd.Execute(null);
        }

        private void reload_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var cmd = (ViewModel as MainViewModel).RefreshCommentsCommand;
            cmd.Execute(null);
        }

    }
}
