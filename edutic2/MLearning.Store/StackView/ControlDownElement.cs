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
    //Para el menu parte baja
    public delegate void ControlDownElementSelectedEventHandler(object sender, int index);
    public sealed partial class ControlDownElement : Grid
    {

        // public:
        public ControlDownElement()
        {
            init();
        }

        public event ControlDownElementSelectedEventHandler ControlDownElementSelected;

        public int Index
        {
            set { _index = value; }
            get { return _index; }
        }

        public ChapterDataSource Source
        {
            set { _source = value; updatevalues(); }
            get { return _source; }
        }

        public double ElementHeight
        {
            set { }
            get { return 0.0; }
        }

        public double ElementWidth
        {
            set { }
            get { return 0.0; }
        }

        public void Select()
        {
            _isselected = true;
           // animate2color(_source.ChapterColor);
            StyleConstants styles = new StyleConstants();
            animate2color(styles.MainAlphaColors[_index%4]);

            animate2double(1.04, "ScaleX");
            animate2double(1.04, "ScaleY");
        }

        public void Unselect()
        {
            _isselected = false;
            animate2color(ColorHelper.FromArgb(100, 0, 0, 0));
            animate2double(1.0, "ScaleX");
            animate2double(1.0, "ScaleY");
        }

        //private:
        int _index;
        double _height, _width;
        bool _isselected;
 

        ChapterDataSource _source;
        TextBlock _textname;
        Grid _container;
        CompositeTransform _transform;


        void init()
        {
            Height = 102.0;
            Width = 232.0;

            _container = new Grid();
            _container.Width = 228.0;
            _container.Height = 98.0;
            _container.Background = new SolidColorBrush(ColorHelper.FromArgb(100, 0, 0, 0));
            Children.Add(_container);
            _transform = new CompositeTransform();
            _transform.CenterX = 114.0;
            _transform.CenterY = 49.0;
            _container.RenderTransform = _transform;

            StackPanel panel = new StackPanel() { VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center, Orientation= Orientation.Vertical };

            _textname = new TextBlock();
            //_textname.Height = 30;
            _textname.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
            _textname.FontSize = 18;
            _textname.TextAlignment = TextAlignment.Center;
            _textname.TextWrapping = TextWrapping.Wrap;
            _textname.Text = "Capitulo n";
            _textname.SizeChanged += _textname_SizeChanged;
            _container.Children.Add(panel);
            panel.Children.Add(_textname);

            Tapped += OnTapped_1;
        }

        void _textname_SizeChanged(object sender, SizeChangedEventArgs e)
        {
           // _textname.h
        }

        void animate2color(Color c)
        {
            Storyboard story = new Storyboard();
            ColorAnimation animation = new ColorAnimation();

            animation.Duration = TimeSpan.FromMilliseconds(350);
            story.Children.Add(animation);
            animation.EnableDependentAnimation = true;

            Storyboard.SetTarget(animation, _container);
            Storyboard.SetTargetProperty(animation, "(Grid.Background).(SolidColorBrush.Color)");

            animation.To = c;
            story.Begin();
        }

        void animate2double(double to, string prop)
        {
            Storyboard story = new Storyboard();
            DoubleAnimation animation = new DoubleAnimation();

            animation.Duration = TimeSpan.FromMilliseconds(350);
            story.Children.Add(animation);
            animation.EnableDependentAnimation = true;

            Storyboard.SetTarget(animation, _transform);
            Storyboard.SetTargetProperty(animation, prop);

            animation.To = to;
            story.Begin();
        }

        void updatevalues()
        {
            if (_source.Title != null)
                _textname.Text = _source.Title;
            else _textname.Text = "NO TEXT FOUND";
        }

        void OnTapped_1(object sender, TappedRoutedEventArgs e)
        {
            ControlDownElementSelected(this, _index);
        }

    }
}
