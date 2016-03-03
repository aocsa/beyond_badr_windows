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
using Cirrious.MvvmCross.ViewModels;
using Windows.UI;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MLearning.Store.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary> 
    public sealed partial class LoginView : MvxWindowsPage
    {
        LoginGrid _logingrid = new LoginGrid();
        LoadingView _loadingview = new LoadingView();
        PopupView popup = new PopupView();

         
        public LoginView()
        {
            this.InitializeComponent();
           // 
            MainGrid.Children.Add(new GridResource());
             
           
            MainGrid.Children.Add(_logingrid);
            MainGrid.Children.Add(_loadingview);
            _loadingview.Opacity = 0.0;
            Canvas.SetZIndex(_loadingview,-10);
            MainGrid.Children.Add(popup);
            this.Loaded += LoginView_Loaded;
        }

        void LoginView_Loaded(object sender, RoutedEventArgs e)
        {
            var vm = (LoginViewModel)this.ViewModel;
            ((LoginViewModel)this.ViewModel).PropertyChanged += LoginView_PropertyChanged;

            /**Binding myBinding = new Binding() { Source = vm.Username, Mode=BindingMode.TwoWay };
            _logingrid.UserTextBox.SetBinding(TextBox.TextProperty, myBinding);
            Binding myBinding2 = new Binding() { Source = vm.Password ,Mode=BindingMode.TwoWay};
            _logingrid.PassTextBox.SetBinding(TextBox.TextProperty, myBinding2);**/

            ///Login
            _logingrid.UserTextBox.TextChanged += (s , a) => { vm.Username = _logingrid.UserTextBox.Text; };
            _logingrid.PassTextBox.TextChanged += (s, a) => { vm.Password = _logingrid.PassTextBox.Text; };
            _logingrid.DoLogin.Tapped += (s, a) =>
            {
                var command = ((LoginViewModel)this.ViewModel).LoginCommand;
                command.Execute(null);
                Canvas.SetZIndex(_loadingview, 10);
                _loadingview.Opacity = 1.0;
            };



        }

         
        void LoginView_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var vm = (LoginViewModel)this.ViewModel;
            if (e.PropertyName == "LoginOK")
            {
                if (!vm.LoginOK)
                {
                    popup.Message = "Ingrese datos correctos";
                    Canvas.SetZIndex(_loadingview, -10);
                    _loadingview.Opacity = 0.0;
                }  
            }

            if (e.PropertyName == "ConnectionOK")
            {
                if (!vm.ConnectionOK)
                {
                    popup.Message = "Verifique su conexión de Internet";
                    Canvas.SetZIndex(_loadingview, -10);
                    _loadingview.Opacity = 0.0;
                }  
            }
        }
         
    }
}
