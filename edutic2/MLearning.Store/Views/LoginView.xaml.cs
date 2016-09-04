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

using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using Core.Repositories;
using Cirrious.CrossCore;
using Windows.UI.Popups;
using Facebook;
using Windows.Security.Authentication.Web;
using System.Dynamic;
using MLearning.Core.Services;
using Newtonsoft.Json.Linq;

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
            //_logingrid.UserTextBox.TextChanged += (s , a) => { vm.Username = _logingrid.UserTextBox.Text; };
            //_logingrid.PassTextBox.TextChanged += (s, a) => { vm.Password = _logingrid.PassTextBox.Text; };

            /*_logingrid.DoLogin.Tapped += (s, a) =>
            {
                var command = ((LoginViewModel)this.ViewModel).LoginCommand;
                command.Execute(null);
                Canvas.SetZIndex(_loadingview, 10);
                _loadingview.Opacity = 1.0;
            };*/


            _logingrid.DoLoginWithFacebook.Tapped += async (s, a) =>
            {
                if ( await Authenticate(MobileServiceAuthenticationProvider.Facebook) ) { 
                    var command = ((LoginViewModel)this.ViewModel).FacebookLoginCommand;
                    command.Execute(null);
                    Canvas.SetZIndex(_loadingview, 10);
                    _loadingview.Opacity = 1.0;
                }
            };



        }

        string _facebookAppId = "350678345101158";// You must set your own AppId here
        string _permissions = "user_about_me,publish_actions"; // Set your permissions here


        FacebookClient _fb = new FacebookClient();

        private async System.Threading.Tasks.Task<bool> Authenticate(MobileServiceAuthenticationProvider provider)
        {
            string message = "";
            bool success = false;

            /*if (user == null)
            {
                try
                {
                    WAMSRepositoryService service = Mvx.Resolve<IRepositoryService>() as WAMSRepositoryService;
                    user = await service.MobileService.LoginAsync(provider);

                    message = string.Format("You are now signed in - {0}", user.UserId);
                    success = true;

                    //Console.WriteLine("Facebook : " + user.UserId + "  " + user.MobileServiceAuthenticationToken);
                }
                catch (InvalidOperationException e)
                {
                    message = "You must log in. Login Required";

                }
            }*/

            var redirectUrl = "https://www.facebook.com/connect/login_success.html";
            try
            {
                //fb.AppId = facebookAppId;
                var loginUrl = _fb.GetLoginUrl(new
                {
                    client_id = _facebookAppId,
                    redirect_uri = redirectUrl,
                    scope = _permissions,
                    display = "popup",
                    response_type = "token"
                });

                var endUri = new Uri(redirectUrl);

                WebAuthenticationResult WebAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(
                                                        WebAuthenticationOptions.None,
                                                        loginUrl,
                                                        endUri);
                if (WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
                {
                    var callbackUri = new Uri(WebAuthenticationResult.ResponseData.ToString());
                    var facebookOAuthResult = _fb.ParseOAuthCallbackUrl(callbackUri);
                    var accessToken = facebookOAuthResult.AccessToken;
                    if (String.IsNullOrEmpty(accessToken))
                    {
                        // User is not logged in, they may have canceled the login
                    }
                    else
                    {
                        // User is logged in and token was returned
                        LoginSucceded(accessToken);
                    }

                }
                else if (WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
                {
                    throw new InvalidOperationException("HTTP Error returned by AuthenticateAsync() : " + WebAuthenticationResult.ResponseErrorDetail.ToString());
                }
                else
                {
                    // The user canceled the authentication
                }
            }
            catch (Exception ex)
            {
                //
                // Bad Parameter, SSL/TLS Errors and Network Unavailable errors are to be handled here.
                //
                throw ex;
            }
       
        
            return success;

        }

    

        private async void LoginSucceded(string accessToken)
        {
            //IMLearningService _mLearningService = ServiceManager.GetService();
            MLearningAzureService service = Mvx.Resolve<IMLearningService>() as MLearningAzureService;


            dynamic parameters = new ExpandoObject();
            parameters.access_token = accessToken;
            parameters.fields = "id";

            dynamic result = await _fb.GetTaskAsync("me", parameters);
            parameters = new ExpandoObject();
            parameters.id = result.id;
            parameters.access_token = accessToken;

            var vm = this.ViewModel as LoginViewModel;

            WAMSRepositoryService service3 = new WAMSRepositoryService();
            JObject access_token = new JObject();
            access_token["access_token"] = accessToken;
            int provider = 3; // facebook
            MobileServiceUser user1 = await service.LoginProvider(provider, access_token);
            string socialId = user1.UserId;
             
            vm.CreateUserCommand.Execute(user1);

            //Frame.Navigate(typeof(FacebookInfoPage), (object)parameters);
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
