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
using Windows.UI.Text;

namespace MLearning.Store.Components
{
     
    public sealed partial class LoginGrid :  Grid
    {
        Grid _logogrid , _logingrid, _registergrid, _inputgrid;
        CompositeTransform _logotransform, _logintransform,_registertransform, _inputtransform;
        TextBlock _text_lt, _text_rt, _text_rb;
        
        public LoginGrid()
        {

            inittext();
            initlogo();

            initlogin();
            
            //initregister();
            

        }

        #region Login Properties

        TextBox _userBox;
        public TextBox UserTextBox { get { return _userBox; } }

        TextBox _passBox;
        public TextBox PassTextBox { get { return _passBox; } }

        Grid _dologin;
        public Grid DoLogin { get { return _dologin; } }


        Grid _dologinWithFacebook;
        public Grid DoLoginWithFacebook { get { return _dologinWithFacebook; } }


        #endregion

        #region Register Properties

        TextBox _newname;
        public TextBox NewName { get { return _newname; } }
        TextBox _newlastname;
        public TextBox NewLastName { get { return _newlastname; } }
        TextBox _newuser;
        public TextBox NewUser { get { return _newuser; } }

        TextBox _newpassword1;
        public TextBox NewPassword1 { get { return _newpassword1; } }

        TextBox _newpassword2;
        public TextBox NewPassword2 { get { return _newpassword2; } }

        TextBox _newemail;
        public TextBox NewEmail { get { return _newemail; } }

        

        #endregion


        void initregister()
        {
            _registergrid = new Grid() { Width = 366, Height = 408 };
            _registergrid.Opacity = 1.0;
            this.Children.Add(_registergrid);
            _registergrid.Children.Add(getbackimage());
            //header
            _registergrid.Children.Add(getTextBlock(116, 36, 134, 70, "Registrarse en Aplicación"));
            //separators
            for (int i = 0; i <= 5; i++)
                _registergrid.Children.Add(getSeparator(22, 144+i*44, 320));
            var list2 = new List<string>() { "Usuario", "Contraseña", "Confirmar contraseña", "Nombre", "Apellido", "Dirección de correo" };
            var listbox = new List<TextBox> { NewUser, NewPassword1, NewPassword2, NewName, NewLastName,NewEmail };
            for (int i = 0; i <= 5; i++)
            {
                listbox[i] = getTextBox(24, 108 + i * 44, 280, 30, list2[i]);
                _registergrid.Children.Add(listbox[i]);
            }

        }



        void initlogin()
        {
            _logingrid = new Grid() { Width = 500, Height = 268, Opacity = 0.0 };
            this.Children.Add(_logingrid);
            //_logingrid.Children.Add(getbackimage());
            //header
            _logingrid.Children.Add(getTextBlock(0,40,500,90, "Welcome to 2 Hijri (624 CE)...  We embark on a journey that will portray the Rise of the Islamic Golden Ages!", 18));

            _dologinWithFacebook = getFacebookButton(507 / 2.3, 80 / 2.3, 45+80, 100);
            _logingrid.Children.Add(_dologinWithFacebook);

            _logingrid.Children.Add(getTextBlock(40 + 70, 150, 250, 90, "Connect with your friends on social media. Want more information about the Comic"));
            //separators
            //_logingrid.Children.Add(getSeparator(34,158, 200));
            //_logingrid.Children.Add(getSeparator(34,206,200));

            //textinput
            /*_userBox = getTextBox(24, 124, 200, 36, "Usuario");
            _passBox = getTextBox(24, 170, 200, 36, "Contraseña");
            _logingrid.Children.Add(_userBox); 
            _logingrid.Children.Add(_passBox);*/

            //roundbutton
            //_dologin = getRoundButton(30, 30, 210, 170); 
            //_logingrid.Children.Add(_dologin);

            //roundbutton


            //animaciones
            _logintransform = new CompositeTransform();
            _logingrid.RenderTransform = _logintransform;
        }
         

        void animateOpacity(Grid g, double to)
        {
            Storyboard storyboard = new Storyboard();
            DoubleAnimation  animation = new DoubleAnimation();
            animation.Duration = TimeSpan.FromMilliseconds(350);
            storyboard.Children.Add(animation);
            animation.To = to;
            Storyboard.SetTarget(animation, g);
            Storyboard.SetTargetProperty(animation,"Opacity");
            storyboard.Begin();
        } 

        TextBlock getTextBlock( double x, double y, double width, double height, string text, int fontsize = 16 )
        {
            TextBlock tb = new TextBlock() { Width =  width, Height =  height, FontWeight = FontWeights.SemiLight,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Left ,
                                             TextWrapping = TextWrapping.Wrap,
                                             Foreground = new SolidColorBrush(ColorHelper.FromArgb(255, 190, 190, 190)),
                                             Text = text,
                                             FontSize = fontsize
            };
            tb.RenderTransform = new CompositeTransform() { TranslateX = x, TranslateY =  y };
            return tb;
        }

        TextBox getTextBox(double x, double y , double width, double height, string pholder)
        {
            TextBox textbox = new TextBox() { Width =  width, Height =  height, FontWeight = FontWeights.SemiLight, 
                VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Left ,
                                              Foreground = new SolidColorBrush(ColorHelper.FromArgb(255, 120, 120, 120)),
                                              FontSize = 16,
                                              PlaceholderText = pholder,
            Background = new SolidColorBrush(Colors.Transparent),BorderBrush= new SolidColorBrush(Colors.Transparent)
            };
            textbox.RenderTransform = new CompositeTransform() { TranslateX = x, TranslateY = y };
            return textbox;
        }

        Grid getSeparator( double x, double y, double width)
        {
            Grid sep = new Grid() {  VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Left ,
                Width = width, Height = 1, Background = new SolidColorBrush(ColorHelper.FromArgb(255, 179, 179, 179)) ,
            RenderTransform =  new CompositeTransform(){ TranslateX = x, TranslateY =  y}};
            return sep;
        }

        Grid getRoundButton( double width, double height , double x, double y)
        {
            Grid ig = new Grid() { VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Left, 
            Width = width, Height = height, RenderTransform = new TranslateTransform(){ X = x, Y= y}};
            Image img = new Image();
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.UriSource = new Uri("ms-appx:///Resources/boton.png", UriKind.Absolute);
            img.Source = bitmapImage;
            ig.Children.Add(img);
            return ig;
        }

        Grid getFacebookButton(double width, double height, double x, double y)
        {
            Grid ig = new Grid()
            {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Width = width,
                Height = height,
                RenderTransform = new TranslateTransform() { X = x, Y = y }
            };
            Image img = new Image();
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.UriSource = new Uri("ms-appx:///Resources/signupface.png", UriKind.Absolute);
            img.Source = bitmapImage;
            ig.Children.Add(img);
            return ig;
        }


        Image getbackimage()
        {
            Image img = new Image();
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.UriSource = new Uri("ms-appx:///Resources/formback.png", UriKind.Absolute);
            img.Source = bitmapImage;
            return img;
        }

        void initlogo()
        {
            _logotransform = new CompositeTransform() ;
            _logogrid = new Grid() { Width = 210, Height=210, RenderTransform = _logotransform};
            this.Children.Add(_logogrid);
            Image img = new Image();
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.UriSource = new Uri("ms-appx:///Resources/logo.png", UriKind.Absolute);
            img.Source = bitmapImage;
            _logogrid.Children.Add(img);

            Storyboard logo_story = new Storyboard();
            CubicEase ease = new CubicEase() { EasingMode = EasingMode.EaseIn };
            DoubleAnimation logo_animx = new DoubleAnimation() { Duration = TimeSpan.FromMilliseconds(600), EnableDependentAnimation =true, EasingFunction =ease };
            DoubleAnimation logo_animy = new DoubleAnimation() { Duration = TimeSpan.FromMilliseconds(600), EnableDependentAnimation = true, EasingFunction = ease };
            DoubleAnimation logo_scalex = new DoubleAnimation() { Duration = TimeSpan.FromMilliseconds(600), EnableDependentAnimation = true, EasingFunction = ease };
            DoubleAnimation logo_scaley = new DoubleAnimation() { Duration = TimeSpan.FromMilliseconds(600), EnableDependentAnimation = true, EasingFunction = ease };
            logo_story.Children.Add(logo_animx);
            logo_story.Children.Add(logo_animy);
            logo_story.Children.Add(logo_scalex);
            logo_story.Children.Add(logo_scaley);
            Storyboard.SetTarget(logo_animx, _logotransform);
            Storyboard.SetTargetProperty(logo_animx, "TranslateX");
            Storyboard.SetTarget(logo_animy, _logotransform);
            Storyboard.SetTargetProperty(logo_animy, "TranslateY");
            Storyboard.SetTarget(logo_scalex, _logotransform);
            Storyboard.SetTargetProperty(logo_scalex, "ScaleX");
            Storyboard.SetTarget(logo_scaley, _logotransform);
            Storyboard.SetTargetProperty(logo_scaley, "ScaleY");

            logo_animx.To = -560.0;
            logo_animy.To = 400.0;
            logo_scalex.To = 0.35;
            logo_scaley.To = 0.35;

            logo_story.Completed += logo_story_Completed;
            logo_story.Begin();
        }

        void logo_story_Completed(object sender, object e)
        {
            animateOpacity(_logingrid, 1.0);
        }

        void inittext()
        {
            _text_lt = new TextBlock()
            {
                FontSize = 18,
                Text = "Beyond Badr",
                Width = 120,
                Height = 50,
                TextAlignment = TextAlignment.Left,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(16) ,
                Foreground = new SolidColorBrush(ColorHelper.FromArgb(255, 200, 200, 200))
            };
            _text_rt = new TextBlock()
            {
                FontSize = 18,
                Text = " ",
                Width = 140,
                Height = 50,
                TextAlignment = TextAlignment.Right,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(16) ,
                Foreground = new SolidColorBrush(ColorHelper.FromArgb(255, 200, 200, 200))
            };
            _text_rb = new TextBlock()
            {
                FontSize = 14,
                Text = "Introducing the first epic battle in Islam!",
                Width = 640,
                Height = 30,
                TextAlignment = TextAlignment.Right,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(16) ,
                Foreground = new SolidColorBrush(ColorHelper.FromArgb(255, 200, 200, 200))
            };

            this.Children.Add(_text_lt);
            this.Children.Add(_text_rb);
            this.Children.Add(_text_rt);
        }


    }
}
