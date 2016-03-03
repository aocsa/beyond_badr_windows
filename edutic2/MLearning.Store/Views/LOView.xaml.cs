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
using StackView;
using DataSource;
using MLearning.Core.ViewModels;
using Windows.UI.Xaml.Media.Imaging;
using MLearning.Store.Components;
using Windows.UI;
using System.Collections.ObjectModel;
using MLReader;
using MLearning.Store.MLStyles;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MLearning.Store.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LOView : MvxWindowsPage
    {
        BookDataSource booksource;
        IGroupList lo_list;
        ControlDownMenu down_menu;
        LoadingView loading_view;

        MLFadeImage logo_image;

        int _currentLO = 0;

        public LOView()
        {
            this.InitializeComponent();
            this.Loaded += LOView_Loaded;

             /**loading_view = new LoadingView() { Width = 1600, Height = 900 };
            loading_view.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            MainGrid.Children.Add(loading_view);*/
        }

        async void LOView_Loaded(object sender, RoutedEventArgs e)
        {

            initbackground(); 
             await init();
            initreader();
            LoadPagesDataSource();
            Background = new SolidColorBrush(Windows.UI.Colors.Transparent);

           
        }

        private void Image_ImageOpened(object sender, RoutedEventArgs e)
        {
            int a = 0;
        }



        async Task init()
        {
            booksource = new BookDataSource();
            lo_list = new IGroupList();

            lo_list.StackListScrollCompleted += lo_list_StackListScrollCompleted;
            lo_list.StackItemFullAnimationCompleted += lo_list_StackItemFullAnimationCompleted;
            lo_list.StackItemFullAnimationStarted += lo_list_StackItemFullAnimationStarted;
            lo_list.StackItemThumbAnimationStarted += lo_list_StackItemThumbAnimationStarted;
            lo_list.StackItemThumbAnimationCompleted += lo_list_StackItemThumbAnimationCompleted;
            MainGrid.Children.Add(lo_list);

            down_menu = new ControlDownMenu();
            MainGrid.Children.Add(down_menu);
            down_menu.ControlDownElementSelected += menu_ControlDownElementSelected;


            //init data
            var vm = ViewModel as LOViewModel;
            await vm.InitLoad();


            loadLOsInCircle(0);
            lo_list.Source = booksource;
            down_menu.Source = booksource;
            //vm = this.ViewModel as LOViewModel;
            vm.PropertyChanged += vm_PropertyChanged;

        }

        void lo_list_StackItemThumbAnimationCompleted(object sender, int chapter, int section, int page)
        {
            Canvas.SetZIndex(_readerview, -100);
            _readerview.Opacity = 0.0;
           
            _menucontroller.Opacity = 1.0;
            down_menu.Opacity = 1.0;

            var vm = ViewModel as LOViewModel;
            vm.OpenPageCommand.Execute(vm.LOsInCircle[chapter].stack.StacksList[section].PagesList[page]);

            //_readerview.ClearAllPages();
        }

        void lo_list_StackItemThumbAnimationStarted(object sender, int chapter, int section, int page)
        {

        }

        void lo_list_StackItemFullAnimationStarted(object sender, int chapter, int section, int page)
        { 
            //_menucontroller.animate2close();
            _menucontroller.Opacity = 0.0;
            down_menu.Opacity = 0.0;
            _readerview.ElementTransform = lo_list.SelectedStackItem.ItemTransform; 
            _readerview.SetAt(chapter, section, page);
        }

        void lo_list_StackItemFullAnimationCompleted(object sender, int chapter, int section, int page)
        { 
            //_readerview.Opacity = 1.0;
            _readerview.animate_opacity(1.0);
            Canvas.SetZIndex(_readerview, 1000);
            
            //open page
            _readerview.LoadCurrentPage();
        }

        private void lo_list_StackListScrollCompleted(object sender, int nextitem)
        {
            _currentLO = nextitem; 
            var vm = ViewModel as LOViewModel ;
            down_menu.SelectElement(nextitem);
            _backgroundscroll.settoindex(nextitem);
            booksource.TemporalColor = StaticStyles.Colors[vm.LOsInCircle[_currentLO].lo.color_id].MainColor;
           
            _menucontroller.Animate2Color(StaticStyles.Colors[vm.LOsInCircle[_currentLO].lo.color_id].MainColorA);
            
            logo_image.NewSource = new BitmapImage(new Uri(StaticStyles.LogosScroll[vm.LOsInCircle[_currentLO].lo.color_id]));
        }

        void menu_ControlDownElementSelected(object sender, int index)
        {
            lo_list.AnimateToChapter(index);
            var vm = ViewModel as LOViewModel;
            // vm.LoadStackImagesCommand.Execute(index);
        }

        void vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var vm = this.ViewModel as LOViewModel;
            if (e.PropertyName == "LOsInCircle")
            {
                if (vm.LOsInCircle != null)
                {
                   vm.LOsInCircle.CollectionChanged += LOsInCircle_CollectionChanged;
                }
            }


            if (e.PropertyName == "CurrentPage")
            {
                //loadcurrentpage();
            }


        }



        void LOsInCircle_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            StyleConstants styles = new StyleConstants(); 
            loadLOsInCircle(e.NewStartingIndex);
            var vm = ViewModel as LOViewModel;
            down_menu.SelectElement(vm.LOCurrentIndex);
            booksource.TemporalColor = styles.MainAlphaColors[vm.LOCurrentIndex % 4];  //Util.GetColorbyIndex(vm.LOCurrentIndex);
        }

        void loadLOsInCircle(int index)
        { 
            var vm = ViewModel as LOViewModel;
            if (vm.LOsInCircle != null)
            {

                for (int i = index; i < vm.LOsInCircle.Count; i++)
                {
                    ChapterDataSource newchapter = new ChapterDataSource();
                    newchapter.Title = vm.LOsInCircle[i].lo.title;
                    newchapter.Author = vm.LOsInCircle[i].lo.name + "\n" + vm.LOsInCircle[i].lo.lastname;
                    //newchapter.Description = vm.LOsInCircle[i].lo.description;
                    newchapter.Description = GetPlainTextFromHtml(vm.LOsInCircle[i].lo.description);
                    newchapter.ChapterColor = StaticStyles.Colors[vm.LOsInCircle[i].lo.color_id].MainColor;
                    newchapter.TemporalColor = StaticStyles.Colors[vm.LOsInCircle[index].lo.color_id].MainColor;

                    if (vm.LOsInCircle[i].background_bytes != null)
                        newchapter.BackgroundImage = Constants.ByteArrayToImageConverter.Convert(vm.LOsInCircle[i].background_bytes);

                    vm.LOsInCircle[i].PropertyChanged += (s1, e1) =>
                            {
                                if (e1.PropertyName == "background_bytes")
                                {
                                    newchapter.BackgroundImage = Constants.ByteArrayToImageConverter.Convert((s1 as MLearning.Core.ViewModels.MainViewModel.lo_by_circle_wrapper ).background_bytes);
                                }
                            };

                    //loading the stacks
                    if (vm.LOsInCircle[i].stack.IsLoaded)
                    {
                        var s_list = vm.LOsInCircle[i].stack.StacksList;
                        for (int j = 0; j < s_list.Count; j++)
                        {
                            SectionDataSource stack = new SectionDataSource();

                            stack.Name = s_list[j].TagName;
                            for (int k = 0; k < s_list[j].PagesList.Count; k++)
                            {
                                var page = new PageDataSource();
                                page.Name = s_list[j].PagesList[k].page.title;
                                page.Description = GetPlainTextFromHtml(s_list[j].PagesList[k].page.description);
                                if (s_list[j].PagesList[k].cover_bytes != null)
                                    page.ImageContent = Constants.ByteArrayToImageConverter.Convert(s_list[j].PagesList[k].cover_bytes);
                                s_list[j].PagesList[k].PropertyChanged += (s2, e2) =>
                                {
                                    if (e2.PropertyName == "cover_bytes")
                                        page.ImageContent = Constants.ByteArrayToImageConverter.Convert((s2 as MLearning.Core.ViewModels.LOViewModel.page_wrapper).cover_bytes);
                                };
                                stack.Pages.Add(page);
                            }
                            newchapter.Sections.Add(stack);
                        }

                    }
                    else
                    {

                        vm.LOsInCircle[i].stack.PropertyChanged += (s3, e3) =>
                            {
                                var s_list = vm.LOsInCircle[i].stack.StacksList;
                                for (int j = 0; j < s_list.Count; j++)
                                {
                                    SectionDataSource stack = new SectionDataSource();

                                    stack.Name = s_list[j].TagName;
                                    for (int k = 0; k < s_list[j].PagesList.Count; k++)
                                    {
                                        PageDataSource page = new PageDataSource();
                                        page.Name = s_list[j].PagesList[k].page.title;
                                        page.Description = GetPlainTextFromHtml(s_list[j].PagesList[k].page.description);
                                        if (s_list[j].PagesList[k].cover_bytes != null)
                                        {
                                            page.ImageContent = Convert(s_list[j].PagesList[k].cover_bytes, 267, 150); 
                                        }
                                        s_list[j].PagesList[k].PropertyChanged += (s2, e2) =>
                                        {
                                            if (e2.PropertyName == "cover_bytes")
                                            {
                                                page.ImageContent = Convert(s_list[j].PagesList[k].cover_bytes,267, 150); 
                                            }
                                        };
                                        stack.Pages.Add(page);
                                    }
                                    newchapter.Sections.Add(stack);
                                }
                            };
                    }
                    booksource.Chapters.Add(newchapter);
                }
                down_menu.SelectElement(vm.LOCurrentIndex);
                booksource.TemporalColor = StaticStyles.Colors[vm.LOsInCircle[_currentLO].lo.color_id].MainColor;
                _backgroundscroll.Source = booksource;
                logo_image.BackSource = new BitmapImage(new Uri(StaticStyles.LogosScroll[vm.LOsInCircle[_currentLO].lo.color_id]));
                _menucontroller.SEtColor(StaticStyles.Colors[vm.LOsInCircle[_currentLO].lo.color_id].MainColorA);
            }
        }


        public BitmapImage Convert(object value, int dx, int dy)
        {
            if (value == null || !(value is byte[]))
                return null;

            using (InMemoryRandomAccessStream ms = new InMemoryRandomAccessStream())
            {
                using (DataWriter writer = new DataWriter(ms.GetOutputStreamAt(0)))
                {
                    writer.WriteBytes((byte[])value);
                    writer.StoreAsync().GetResults();
                }

                var image = new BitmapImage();
                image.DecodePixelWidth = dx;
                image.DecodePixelHeight = dy;
                image.SetSource(ms);
                return image;
            }
        }

        #region Controls background

        ControlScrollView _backgroundscroll;
        UpMenuController _menucontroller;
        void initbackground()
        {
            _backgroundscroll = new ControlScrollView();
            MainGrid.Children.Add(_backgroundscroll);
            Canvas.SetZIndex(_backgroundscroll, -10);

            logo_image = new MLFadeImage()
            {
                Width = 190.0,
                Height = 114.0,
                RenderTransform = new TranslateTransform() { X = 1280, Y=112},
                VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Top,
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left,
                BrushVisible = false
            };

            Grid brushgrid = new Grid() {Background= new SolidColorBrush(Colors.Black), Opacity=0.5 };
            MainGrid.Children.Add(brushgrid);
            Canvas.SetZIndex(brushgrid,-9);

            MainGrid.Children.Add(logo_image);
            Canvas.SetZIndex(logo_image, -8);

            _menucontroller = new UpMenuController();
            MainGrid.Children.Add(_menucontroller);
            _menucontroller.PropertyChanged += _menucontroller_PropertyChanged;
            Canvas.SetZIndex(_menucontroller, 100);
        }

        void _menucontroller_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Home")
            {
                var vm = ViewModel as LOViewModel;
                vm.BackCommand.Execute(null);
            }
        }

        #endregion


        #region Reader Datasource Load

        List<LOPageSource> pagelistsource = new List<LOPageSource>();

        void LoadPagesDataSource()
        {
            var vm = ViewModel as LOViewModel;
            //var styles = new StyleConstants();
            int scroll_index = 0;

            //for styles
            bool white_style = false, is_main = true;

            for (int i = 0; i < vm.LOsInCircle.Count; i++)
            {
                var s_list = vm.LOsInCircle[i].stack.StacksList;
                for (int j = 0; j < s_list.Count; j++)
                {

                    for (int k = 0; k < s_list[j].PagesList.Count; k++)
                    {
                        LOPageSource page = new LOPageSource();
                        var content = s_list[j].PagesList[k].content;

                        //page.Cover = Constants.ByteArrayToImageConverter.Convert(s_list[j].PagesList[k].cover_bytes);
                        page.CoverBytes = s_list[j].PagesList[k].cover_bytes;
                        page.PageIndex = k;
                        page.StackIndex = j;
                        page.LOIndex = i;
                        //to set postion in the scroll
                        page.Index = scroll_index;
                        scroll_index += 1;
                        var slides = s_list[j].PagesList[k].content.lopage.loslide;


                        //vm.OpenPageCommand.Execute(s_list[j].PagesList[k]);

                        var currentpage = s_list[j].PagesList[k];
                        page.Slides = new List<LOSlideSource>();
                        currentpage.PropertyChanged += (sender, args) => {
                            if (args.PropertyName == "IsLoaded") 
                            {
                                page.IsLoaded = currentpage.IsLoaded;
                            }
                        };
                        for (int m = 0; m < slides.Count; m++)
                        {
                            LOSlideSource slidesource = new LOSlideSource();
                            var _id_ = vm.LOsInCircle[i].lo.color_id;
                           
                            slidesource.Style = getSlideStyle(white_style, is_main, _id_ , slides[m].lotype);
                            slidesource.Style.ID = _id_+1;
                            
                            is_main = !is_main;
                            if (slides[m].lotype != 0 && slides[m].lotype != 6)
                                white_style = !white_style;
                            slidesource.Type = slides[m].lotype;
                            if (slides[m].lotitle != null) slidesource.Title = slides[m].lotitle;
                            if (slides[m].loparagraph != null) slidesource.Paragraph = GetPlainTextFromHtml(slides[m].loparagraph);
                            if (slides[m].loimage != null) slidesource.ImageUrl = slides[m].loimage;
                            if (slides[m].lotext != null) slidesource.Paragraph = GetPlainTextFromHtml(slides[m].lotext);
                            if (slides[m].loauthor != null) slidesource.Author = slides[m].loauthor;
                            if (slides[m].lovideo != null) slidesource.VideoUrl = slides[m].lovideo;
                            //imagebytes
                            if (slides[m].image_bytes != null) slidesource.ImageBytes = slides[m].image_bytes; 
                            
                            var c_slide = slides[m];
                            c_slide.PropertyChanged+=(s,e)=>{
                                if (e.PropertyName == "image_bytes" && c_slide.image_bytes != null)
                                    slidesource.ImageBytes = c_slide.image_bytes; 
                            };

                            if (c_slide.loitemize != null)
                            {
                                slidesource.Itemize = new ObservableCollection<LOItemSource>();
                                var items = c_slide.loitemize.loitem;
                                for (int n = 0; n < items.Count; n++)
                                { 
                                    LOItemSource item = new LOItemSource();
                                    if (items[n].loimage != null) item.ImageUrl = items[n].loimage;
                                    if (items[n].lotext != null) item.Text = GetPlainTextFromHtml(items[n].lotext);
                                    //imagebytes
                                    if (items[n].image_bytes != null) item.ImageBytes = items[n].image_bytes; 

                                    var c_item_ize = items[n];
                                    c_item_ize.PropertyChanged += (s1, e1) =>
                                    {
                                        if (e1.PropertyName == "image_bytes" && c_item_ize.image_bytes != null)
                                            item.ImageBytes = c_item_ize.image_bytes; 
                                    };
                                    slidesource.Itemize.Add(item);
                                }
                            }
                            page.Slides.Add(slidesource);
                             
                             
                        } 
                        //pages

                        pagelistsource.Add(page);
                    }
                }
            }

            //add pages
            _readerview.Source = pagelistsource;
            //Canvas.SetZIndex(_readerview, 10);
        }


        #endregion


        #region Reader View

        LOReaderScroll _readerview;

        void initreader()
        {
            _readerview = new LOReaderScroll();
            _readerview.LOReaderAnimate2Thumbnail += _readerview_LOReaderAnimate2Thumbnail;
            _readerview.LOReaderPagedChanged += _readerview_LOReaderPagedChanged;
            //_readerview.RightTapped += _readerview_RightTapped;
            _readerview.LOReaderRightTapped += _readerview_LOReaderRightTapped;
            _readerview.LOReaderOpenPageAt += _readerview_LOReaderOpenPageAt;
            MainGrid.Children.Add(_readerview);
            Canvas.SetZIndex(_readerview, -100);
        }

        void _readerview_LOReaderRightTapped(object sender)
        {
            lo_list.SelectedStackItem.AnimateToThumb();
        }

        void _readerview_LOReaderOpenPageAt(object sender, int chapter, int section, int page)
        {
            var vm = ViewModel as LOViewModel;
            vm.OpenPageCommand.Execute(vm.LOsInCircle[chapter].stack.StacksList[section].PagesList[page]);
        }

        void _readerview_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            //lo_list.SelectedStackItem.AnimateToThumb();
        }

        private void _readerview_LOReaderPagedChanged(object sender)
        {
            lo_list.SetToItem(_readerview.ChapterIndex, _readerview.SectionIndex, _readerview.PageIndex);
            _readerview.ElementTransform = lo_list.SelectedStackItem.ItemTransform;

            var vm = ViewModel as LOViewModel;
           
            //vm.OpenPageCommand.Execute(vm.LOsInCircle[_readerview.ChapterIndex].stack.StacksList[_readerview.SectionIndex].PagesList[_readerview.PageIndex]);

            
        }

        //void loadcurrentpage()
        //{
        //    var vm = ViewModel as LOViewModel;
        //    LOPageSource page = new LOPageSource();
        //    if (vm.CurrentPage != null)
        //    {
        //        var currentpage = vm.CurrentPage;
        //        page.Slides = new List<LOSlideSource>();
        //        var slides = vm.CurrentPage.content.lopage.loslide;
        //        for (int m = 0; m < slides.Count; m++)
        //        {
        //            LOSlideSource slidesource = new LOSlideSource();

        //            slidesource.Type = slides[m].lotype;
        //            if (slides[m].lotitle != null) slidesource.Title = slides[m].lotitle;
        //            if (slides[m].loparagraph != null) slidesource.Paragraph = slides[m].loparagraph;
        //            if (slides[m].loimage != null) slidesource.ImageUrl = slides[m].loimage;
        //            if (slides[m].lotext != null) slidesource.Paragraph = slides[m].lotext;
        //            if (slides[m].loauthor != null) slidesource.Author = slides[m].loauthor;
        //            if (slides[m].lovideo != null) slidesource.VideoUrl = slides[m].lovideo;
        //            if (slides[m].image_bytes != null) slidesource.Image = Constants.ByteArrayToImageConverter.Convert(slides[m].image_bytes);
        //            if (slides[m].loitemize != null)
        //            {
        //                slidesource.Itemize = new ObservableCollection<LOItemSource>();
        //                var items = slides[m].loitemize.loitem;
        //                for (int n = 0; n < items.Count; n++)
        //                {
        //                    LOItemSource item = new LOItemSource();
        //                    if (items[n].loimage != null) item.ImageUrl = items[n].loimage;
        //                    if (items[n].lotext != null) item.Text = items[n].lotext;
        //                    if (items[n].image_bytes != null) item.Image = Constants.ByteArrayToImageConverter.Convert(items[n].image_bytes);
        //                    slidesource.Itemize.Add(item);
        //                }
        //            }
        //            page.Slides.Add(slidesource); 
        //        }
                 
        //        _readerview.LoadCurrentPage(page);
        //    }
        //}

        void _readerview_LOReaderAnimate2Thumbnail(object sender)
        {
            if (_readerview.ElementTransform.ScaleX < 3)
            {
                lo_list.SelectedStackItem.AnimateToThumb();
                
            }
            else
            {
                lo_list.SelectedStackItem.AnimateToFull();
                
            }
        }


        LOSlideStyle getSlideStyle(bool iswhite,bool ismain, int colorid, int type)
        { 
            LOSlideStyle style = new LOSlideStyle();
            //default
            style.BorderColor = StaticStyles.Colors[colorid].SecondColor; 
            style.ContentColor = Colors.Black ;

            if (ismain) style.TitleColor = StaticStyles.Colors[colorid].MainColor; else style.TitleColor = StaticStyles.Colors[colorid].SecondColor;
            if (ismain) style.ColorNumber = 1; else style.ColorNumber = 2;
            style.BackgroundColor = Colors.White;
            if (!iswhite)
            {
                if (ismain) style.BackgroundColor = StaticStyles.Colors[colorid].MainColor; else style.BackgroundColor = StaticStyles.Colors[colorid].SecondColor;
                if (type != 0) style.TitleColor = Colors.White;
                style.ColorNumber = 0;
                if (type == 6) style.BorderColor = Colors.White;
            }

            //especial cases
            if (type == 0 || type == 6) style.ContentColor = Colors.White;
            if (type == 6) style.TitleColor = Colors.White;
            if (type == 5) style.TitleColor = Colors.Black;

            
            return style;
        }


        #endregion


        public string GetPlainTextFromHtml(string htmlString)
        {
            string htmlTagPattern = "<.*?>";
            var regexCss = new Regex("(\\<script(.+?)\\</script\\>)|(\\<style(.+?)\\</style\\>)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            htmlString = regexCss.Replace(htmlString, string.Empty);
            htmlString = Regex.Replace(htmlString, htmlTagPattern, string.Empty);
            htmlString = Regex.Replace(htmlString, @"^\s+$[\r\n]*", "", RegexOptions.Multiline);
            htmlString = htmlString.Replace("&nbsp;", string.Empty);

            return htmlString;
        }
    }
}
