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
    public class CreateInstitutionViewModel : MvxViewModel
    {

        private IMLearningService _mLearningService;


        public CreateInstitutionViewModel(IMLearningService mlearningService)
        {
            _mLearningService = mlearningService;

            InstitutionsList = new ObservableCollection<Institution>();
            LoadInstitutions();
        }

        async void LoadInstitutions()
        {
            var list = await _mLearningService.GetInstitutions();

            InstitutionsList = new ObservableCollection<Institution>(list);
        }

        string _institutionName;
        public string InstitutionName
        {
            get { return _institutionName; }
            set { _institutionName = value; RaisePropertyChanged("InstitutionName"); }
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





        ObservableCollection<Institution> _institutionsList;
        public ObservableCollection<Institution> InstitutionsList
        {
            get { return _institutionsList; }
            set { _institutionsList = value; RaisePropertyChanged("InstitutionsList"); }
        }




        MvxCommand _createInstitutionCommand;
        public System.Windows.Input.ICommand CreateInstitutionCommand
        {
            get
            {
                _createInstitutionCommand = _createInstitutionCommand ?? new MvxCommand(DoCreateInstitutionCommand);
                return _createInstitutionCommand;
            }
        }

        void DoCreateInstitutionCommand()
        {
            string unknownString = "unknow";
            var institution = new Institution { name = InstitutionName, country = unknownString, region = unknownString, address_line_1 = unknownString, city = unknownString,email=unknownString ,created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow };
            InstitutionsList.Add(institution);
            var headUser = new User { name = FirstName, lastname = LastName, username = Username, password=EncryptionService.encrypt(Password),email = Email, created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow };
            var headInfo = new Head { title = "Sr." };
            _mLearningService.CreateInstitution(institution,headInfo,headUser);

          //  _mLearningService.CreateObject<Institution>(institution,i=>i.id);
        }



        MvxCommand<Institution> _managePublishersCommand;
        public System.Windows.Input.ICommand ManagePublishersCommand
        {
            get
            {
                _managePublishersCommand = _managePublishersCommand ?? new MvxCommand<Institution>(DoManagePublishersCommand);
                return _managePublishersCommand;
            }
        }

        void DoManagePublishersCommand(Institution inst)
        {
            ShowViewModel<ManagePublishersViewModel>(new { inst_id = inst.id });
        }

        


    }
}
