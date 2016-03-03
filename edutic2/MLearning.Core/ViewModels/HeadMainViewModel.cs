using Cirrious.MvvmCross.ViewModels;
using Core.Security;
using MLearning.Core.Entities;
using MLearning.Core.Services;
using MLearningDB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLearning.Core.ViewModels
{
    public class HeadMainViewModel : MvxViewModel
    {

        private IMLearningService _mLearningService;


        public HeadMainViewModel(IMLearningService mlearningService)
        {
            _mLearningService = mlearningService;
        }
        async public void Init(int user_id)
        {
            UserID = user_id;

            InstitutionID = await _mLearningService.GetHeadInstitutionID(UserID);

            LoadPublishersInInstitution(InstitutionID);
            LoadConsumersInInstitution(InstitutionID);
        }

        
        async void LoadPublishersInInstitution(int inst_id)
        {
            var list = await _mLearningService.GetPublishersByInstitution(inst_id);
            PublishersList = new ObservableCollection<publisher_by_institution>(list);
        }

        async void LoadConsumersInInstitution(int inst_id)
        {
            var list = await _mLearningService.GetConsumersByInstitution(inst_id);
            ConsumersList = new ObservableCollection<consumer_by_institution>(list);
        }


        void ClearProperties()
        {
            FirstName = "";
            LastName = "";
            Username = "";
            Password = "";
            Email = "";
        }

        int _userID;
        public int UserID
        {
            get { return _userID; }
            set { _userID = value; RaisePropertyChanged("UserID"); }
        }


        int _institutionID;
        public int InstitutionID
        {
            get { return _institutionID; }
            set { _institutionID = value; RaisePropertyChanged("InstitutionID"); }
        }

        string _firstName;
        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; RaisePropertyChanged("FirstName"); }
        }


        string _lastName;
        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; RaisePropertyChanged("LastName"); }
        }


        string _username;
        public string Username
        {
            get { return _username; }
            set { _username = value; RaisePropertyChanged("Username"); }
        }



        string _password;
        public string Password
        {
            get { return _password; }
            set { _password = value; RaisePropertyChanged("Password"); }
        }


        string _email;
        public string Email
        {
            get { return _email; }
            set { _email = value; RaisePropertyChanged("Email"); }
        }




        ObservableCollection<publisher_by_institution> _publishersList;
        public ObservableCollection<publisher_by_institution> PublishersList
        {
            get { return _publishersList; }
            set { _publishersList = value; RaisePropertyChanged("PublishersList"); }
        }


        ObservableCollection<consumer_by_institution> _consumersList;
        public ObservableCollection<consumer_by_institution> ConsumersList
        {
            get { return _consumersList; }
            set { _consumersList = value; RaisePropertyChanged("ConsumersList"); }
        }




        MvxCommand _createConsumerCommand;
        public System.Windows.Input.ICommand CreateConsumerCommand
        {
            get
            {
                _createConsumerCommand = _createConsumerCommand ?? new MvxCommand(DoCreateConsumerCommand);
                return _createConsumerCommand;
            }
        }

        void DoCreateConsumerCommand()
        {
            var user = new User { name = FirstName, lastname = LastName, username = Username, password = EncryptionService.encrypt(Password), email = Email };

            _mLearningService.CreateAndRegisterConsumer(user, InstitutionID);

            ConsumersList.Add(new consumer_by_institution { username = user.username });

            ClearProperties();
        }





        MvxCommand _createPublisherCommand;
        public System.Windows.Input.ICommand CreatePublisherCommand
        {
            get
            {
                _createPublisherCommand = _createPublisherCommand ?? new MvxCommand(DoCreatePublisherCommand);
                return _createPublisherCommand;
            }
        }

        void DoCreatePublisherCommand()
        {
            string unknown = "unknown";
            var user = new User { name = FirstName, lastname = LastName, username = Username, password = EncryptionService.encrypt(Password), email = Email };
            var publisher = new Publisher { country = unknown, region = unknown, telephone = 0, title = unknown };
            _mLearningService.CreateAndRegisterPublisher(user, publisher, InstitutionID);

            PublishersList.Add(new publisher_by_institution { username = user.username });
            ClearProperties();


        }







    }
}
