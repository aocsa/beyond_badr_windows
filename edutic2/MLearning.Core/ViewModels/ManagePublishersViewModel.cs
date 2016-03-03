using Cirrious.MvvmCross.ViewModels;
using Core.Entities.json;
using Core.Security;
using MLearning.Core.Configuration;
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
    public class ManagePublishersViewModel : MvxViewModel
    {


        private IMLearningService _mLearningService;

        public ManagePublishersViewModel(IMLearningService mlearningService)
        {
            _mLearningService = mlearningService;
        }
        public void Init(int inst_id)
        {
            Institution_id = inst_id;
            PublishersList = new ObservableCollection<publisher_by_institution>();

            LoadPublishersList(inst_id);
        }

        async void LoadPublishersList(int inst_id)
        {
            var list = await _mLearningService.GetPublishersByInstitution(inst_id);

            PublishersList = new ObservableCollection<publisher_by_institution>(list);
        }


        int _institution_id;
        public int Institution_id
        {
            get { return _institution_id; }
            set { _institution_id = value; RaisePropertyChanged("Institution_id"); }
        }





        ObservableCollection<publisher_by_institution> _publishersList;
        public ObservableCollection<publisher_by_institution> PublishersList
        {
            get { return _publishersList; }
            set { _publishersList = value; RaisePropertyChanged("PublishersList"); }
        }


        //Publisher Forms properties


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








        //Creates a Publisher user and adds it to the current institution
        MvxCommand _addPublisherCommand;
        public System.Windows.Input.ICommand AddPublisherCommand
        {
            get
            {
                _addPublisherCommand = _addPublisherCommand ?? new MvxCommand(DoAddPublisherCommand);
                return _addPublisherCommand;
            }
        }

       async void DoAddPublisherCommand()
        {
            var user = new User {name=FirstName,lastname=LastName,username=Username,password= EncryptionService.encrypt(Password),email=Email};
            OperationResult r = await _mLearningService.CreateAccount<User>(user, u => u.id, UserType.Publisher);

            _mLearningService.CreateObject<Institution_has_User>(new Institution_has_User { institution_id = Institution_id, user_id = r.id, created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow },ins=>ins.id);

            Close(this);
        }




        MvxCommand<publisher_by_institution> _editPublisherCommand;
        public System.Windows.Input.ICommand EditPublisherCommand
        {
            get
            {
                _editPublisherCommand = _editPublisherCommand ?? new MvxCommand<publisher_by_institution>(DoEditPublisherCommand);
                return _editPublisherCommand;
            }
        }

        void DoEditPublisherCommand(publisher_by_institution publisher)
        {
           
        }



        MvxCommand _deletePublisherCommand;
        public System.Windows.Input.ICommand DeletePublisherCommand
        {
            get
            {
                _deletePublisherCommand = _deletePublisherCommand ?? new MvxCommand(DoDeletePublisherCommand);
                return _deletePublisherCommand;
            }
        }

        void DoDeletePublisherCommand()
        {

        }



        MvxCommand<publisher_by_institution> _selectPublisherCommand;
        public System.Windows.Input.ICommand SelectPublisherCommand
        {
            get
            {
                _selectPublisherCommand = _selectPublisherCommand ?? new MvxCommand<publisher_by_institution>(DoSelectPublisherCommand);
                return _selectPublisherCommand;
            }
        }

        void DoSelectPublisherCommand(publisher_by_institution publisher)
        {
            ShowViewModel<PublisherMainViewModel>(new { user_id = publisher.id});
        }




    }
}
