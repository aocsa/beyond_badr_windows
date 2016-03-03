using Cirrious.MvvmCross.ViewModels;
using Core.Entities.json;
using Core.Security;
using Core.Session;
using Core.ViewModels;
using Microsoft.WindowsAzure.MobileServices;

using MLearning.Core.Services;
using MLearning.Core.ViewModels;
using MLearningDB;
using Sha2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MLearning.Core.Configuration;


namespace KunFood.Core.ViewModels
{

    //Not Used
    public class CreateAccountViewModel :MvxViewModel
    {

        private IMLearningService _mLearningService;
        public CreateAccountViewModel(IMLearningService mLearningService)
        {
            _mLearningService = mLearningService;
            ClearProperties();

            SelectedUserType = UserType.Consumer;
        }


        string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; RaisePropertyChanged("Name"); }
        }

        string _lastname;
        public string Lastname
        {
            get { return _lastname; }
            set { _lastname = value; RaisePropertyChanged("Lastname"); }
        }

        string _username;
        public string Username
        {
            get { return _username; }
            set { _username = value; RaisePropertyChanged("Username"); }
        }





        string _email;
        public string Email
        {
            get { return _email; }
            set { _email = value; RaisePropertyChanged("Email"); }
        }


        string _password;
        public string Password
        {
            get { return _password; }
            set { _password = value; RaisePropertyChanged("Password"); }
        }


        bool _connectionOK;
        public bool ConnectionOK
        {
            get { return _connectionOK; }
            set { _connectionOK = value; RaisePropertyChanged("ConnectionOK"); }
        }


        bool _operationOK;
        public bool OperationOK
        {
            get { return _operationOK; }
            set { _operationOK = value; RaisePropertyChanged("OperationOK"); }
        }



        bool _isUsernameValid;
        public bool IsUsernameValid
        {
            get { return _isUsernameValid; }
            set { _isUsernameValid = value; RaisePropertyChanged("IsUsernameValid"); }
        }




        UserType _selectedUserType;
        public UserType SelectedUserType
        {
            get { return _selectedUserType; }
            set { _selectedUserType = value; RaisePropertyChanged("SelectedUserType"); }
        }



        MvxCommand _selectPublisherCommand;
        public System.Windows.Input.ICommand SelectPublisherCommand
        {
            get
            {
                _selectPublisherCommand = _selectPublisherCommand ?? new MvxCommand(DoSelectPublisherCommand);
                return _selectPublisherCommand;
            }
        }

        void DoSelectPublisherCommand()
        {
            SelectedUserType = UserType.Publisher;
        }



        MvxCommand _selectConsumerCommand;
        public System.Windows.Input.ICommand SelectConsumerCommand
        {
            get
            {
                _selectConsumerCommand = _selectConsumerCommand ?? new MvxCommand(DoSelectConsumerCommand);
                return _selectConsumerCommand;
            }
        }

        void DoSelectConsumerCommand()
        {
            SelectedUserType = UserType.Consumer;
        }








        MvxCommand _createAccountCommand;
        public System.Windows.Input.ICommand CreateAccountCommand
        {
            get
            {
                _createAccountCommand = _createAccountCommand ?? new MvxCommand(DoCreateAccountCommand);
                return _createAccountCommand;
            }
        }

       async  void DoCreateAccountCommand()
        {
            User user = new User { email = Email, username = Username, password = EncryptionService.encrypt(Password), name = Name, lastname = Lastname ,created_at = DateTime.Now,updated_at=DateTime.Now};

            try
            {
               OperationResult result =  await _mLearningService.CreateAccount<User>(user,(usr)=>usr.id,SelectedUserType);
               SessionService.LogIn(result.id, Username);

                ClearProperties();
                ShowViewModel<MainViewModel>();

            }
            catch (WebException e)
            {
                ConnectionOK = false;
            }
            catch (MobileServiceInvalidOperationException ex)
            {
                OperationOK = false;
            }
        }


       MvxCommand _testUsernameCommand;
       public System.Windows.Input.ICommand TestUsernameCommand
       {
           get
           {
               _testUsernameCommand = _testUsernameCommand ?? new MvxCommand(DoTestUsernameCommand);
               return _testUsernameCommand;
           }
       }

       async void DoTestUsernameCommand()
       {
           try
           {
               bool exists = await _mLearningService.CheckIfExists<User>(usr=>usr.username==Username,(it)=>it.updated_at,it=>it.id);
               IsUsernameValid = !exists;

           }
           catch (WebException e)
           {
               ConnectionOK = false;
           }
           catch (MobileServiceInvalidOperationException ex)
           {
               OperationOK = false;
           }
       }



      

       void ClearProperties()
       {
           Email = Password = Name = Lastname = Username = "";
           
       }



      

      

        MvxCommand _backCommand;
        public System.Windows.Input.ICommand BackCommand
        {
            get
            {
                _backCommand = _backCommand ?? new MvxCommand(DoBackCommand);
                return _backCommand;
            }
        }

        void DoBackCommand()
        {
            Close(this);
        }




        MvxCommand _navigateToLoginCommand;
        public System.Windows.Input.ICommand NavigateToLoginCommand
        {
            get
            {
                _navigateToLoginCommand = _navigateToLoginCommand ?? new MvxCommand(DoNavigateToLoginCommand);
                return _navigateToLoginCommand;
            }
        }

        void DoNavigateToLoginCommand()
        {
            ShowViewModel<LoginViewModel>();
        }











    }
}
