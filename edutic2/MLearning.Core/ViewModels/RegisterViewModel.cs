
using Cirrious.CrossCore;
using Cirrious.MvvmCross.ViewModels;
using Core.Entities.json;
using Core.Repositories;
using Core.Security;
using Core.Session;
using Microsoft.WindowsAzure.MobileServices;

using MLearning.Core.Services;
using MLearning.Core.ViewModels;
//using MLearningDBResult;
using Referee.Core.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MLearning.Core.Configuration;
using Cirrious.MvvmCross.Plugins.PictureChooser;
using System.IO;
using AzureBlobUploader;
using Microsoft.WindowsAzure.Storage;
using System.Net.Http;
using MLearningDBResult;


namespace MLearning.Core
{
	public class RegisterViewModel: MvxViewModel
	{


		private IMLearningService _mLearningService;


		public RegisterViewModel(IMLearningService mLearningService)
		{
			_mLearningService = mLearningService;
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



		string _statusLabel;
		public string StatusLabel
		{
			get { return _statusLabel; }
			set { _statusLabel = value; RaisePropertyChanged("StatusLabel"); }
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



		string _regUsername;
		public string RegUsername
		{
			get { return _regUsername; }
			set { _regUsername = value; RaisePropertyChanged("RegUsername"); }
		}



		string _regPassword;
		public string RegPassword
		{
			get { return _regPassword; }
			set { _regPassword = value; RaisePropertyChanged("RegPassword"); }
		}



		string _email;
		public string Email
		{
			get { return _email; }
			set { _email = value; RaisePropertyChanged("Email"); }
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


		bool _isPublisher;
		public bool IsPublisher
		{
			get { return _isPublisher; }
			set { _isPublisher = value; RaisePropertyChanged("IsPublisher"); ChangeSelectedUserType(_isPublisher); }
		}

		private void ChangeSelectedUserType(bool isPublisher)
		{
			if (isPublisher)
			{
				SelectedUserType = UserType.Publisher;
			}
			else
			{
				SelectedUserType = UserType.Consumer;
			}

		}


		string _userImageUrl;
		public string UserImageUrl
		{
			get { return _userImageUrl; }
			set { _userImageUrl = value; RaisePropertyChanged("UserImageUrl"); }
		}



		int _maxPixelDimension;

		int _percentQuality;



		/// <summary>
		/// Command for register With Institution by default
		/// </summary>
		MvxCommand _registerCommand ;
		public System.Windows.Input.ICommand RegisterCommand
		{
			get 
			{
				_registerCommand = _registerCommand ?? new MvxCommand (DoRegisterCommand1);
				return _registerCommand;
			}
		}


		async void DoRegisterCommand1()
		{
			try
			{
				MLearningDB.User newuser = new MLearningDB.User { email = Email, username = RegUsername, password = EncryptionService.encrypt(RegPassword), name = Name, lastname = ":)", image_url = "http://www.clinicatorielli.com/img/icons/no-user.png" ,created_at = DateTime.Now, updated_at = DateTime.Now };


				int idInstitution = 34;
				newuser.password = EncryptionService.encrypt(newuser.password);
				bool exists = await _mLearningService.CheckIfExistsNoLocale<MLearningDB.User>
					(usr => usr.username == newuser.username, (it) => it.updated_at, it => it.id);

				if(exists)
				{
					return ;
				}

				OperationResult op = await _mLearningService.CreateAndRegisterConsumer (newuser, idInstitution);

				MLearningDB.User user = new MLearningDB.User { username = newuser.username, password = newuser.password };

				SharedPreferences prefs = Constants.GetSharedPreferences(Constants.PreferencesFileName);
				prefs.PutString(Constants.UserFirstNameKey, Name);
				prefs.PutString(Constants.UserLastNameKey, Lastname);
				prefs.PutString(Constants.UserImageUrlKey, UserImageUrl);

				prefs.Commit();

				ClearProperties();

				ShowViewModel<MainViewModel>();

			}
			catch (MobileServiceInvalidOperationException e){
				ConnectionOK = false;
			}
			catch (WebException e)
			{
				ConnectionOK = false;
			}
		}



		bool _connectionOK;
		public bool ConnectionOK
		{
			get { return _connectionOK; }
			set { _connectionOK = value; RaisePropertyChanged("ConnectionOK"); }
		}



		bool _loginOK;
		public bool LoginOK
		{
			get { return _loginOK; }
			set { _loginOK = value; RaisePropertyChanged("LoginOK"); }
		}

		bool _operationOK;
		public bool OperationOK
		{
			get { return _operationOK; }
			set { _operationOK = value; RaisePropertyChanged("OperationOK"); }
		}



		bool _registrationOK;
		public bool RegistrationOK
		{
			get { return _registrationOK; }
			set { _registrationOK = value; RaisePropertyChanged("RegistrationOK"); }
		}




		bool _uploadFinished;
		public bool UploadFinished
		{
			get { return _uploadFinished; }
			set { _uploadFinished = value; RaisePropertyChanged("UploadFinished"); }
		}







		MvxCommand _loginCommand;
		public System.Windows.Input.ICommand LoginCommand
		{
			get
			{
				_loginCommand = _loginCommand ?? new MvxCommand(DoLoginCommand);
				return _loginCommand;
			}
		}


		string _noInternetMessage = "Sin Conexión";
		string _waitingMessage = "Espere por favor...";
		string _wrongCredentialsMessage = "Username o Password incorrectos";

		async void DoLoginCommand()
		{
			StatusLabel =_waitingMessage ;

			////Populate

			//var users = Builder<MLearningDB.User>.CreateListOfSize(5000).All().With(x => x.id = 0 ).And(x=>x.created_at = DateTime.UtcNow).And(x=>x.updated_at = DateTime.UtcNow).And(x=>x.social_id=null).Build();

			//foreach (var user in users)
			//{
			//    await _mLearningService.CreateAccount<MLearningDB.User>(user, u => u.id, UserType.Consumer);

			//    _mLearningService.AddUserToCircle(user.id, 45);
			//}



			////end populate



			try
			{
				User user = new User { username = Username, password = EncryptionService.encrypt(Password) };
				//   User user = new User { username = Username, password = Password };
				LoginOperationResult<User> result =  await _mLearningService.ValidateLogin<User>(user, u => u.password == user.password && u.username == user.username, u => u.id, u => u.type);
				// LoginOperationResult result = await _mLearningService.ValidateConsumerLogin(user.username,user.password);

				if (result.successful )
				{
					LoginOK = true;
					SessionService.LogIn(result.id, Username);

					SharedPreferences prefs = Constants.GetSharedPreferences(Constants.PreferencesFileName);
					prefs.PutString(Constants.UserFirstNameKey, result.account.name);
					prefs.PutString(Constants.UserLastNameKey, result.account.lastname);
					prefs.PutString(Constants.UserImageUrlKey, result.account.image_url);

					prefs.Commit();


					switch ((UserType)result.userType)
					{ 
					case UserType.Consumer:

						//Select corresponding user screen

						ShowViewModel<MainViewModel>();


						break;
						#if (ADMIN)
						case UserType.Head:


						ShowViewModel<HeadMainViewModel>(new { user_id=result.id});

						break;

						case UserType.Publisher:

						ShowViewModel<PublisherMainViewModel>(new { user_id = result.id });
						break;

						#endif

					default:
						StatusLabel = _wrongCredentialsMessage;
						LoginOK = false;
						break;

					}


				}
				else
				{
					StatusLabel = _wrongCredentialsMessage;
					LoginOK = false;

				}

			}
			catch (WebException e)
			{
				StatusLabel = _noInternetMessage;
				ConnectionOK = false;
			}
			catch(HttpRequestException e)
			{
				StatusLabel = _noInternetMessage;
				ConnectionOK = false;
			}
			catch (MobileServiceInvalidOperationException e)
			{
				Mvx.Trace("MobileServiceInvalidOperationException " + e.Message);
				OperationOK = false;
			}
		}





		//----------------------------------------------------------------------Registration Commands----------------------------------------------


		MvxCommand _createAccountCommand;
		public System.Windows.Input.ICommand CreateAccountCommand
		{
			get
			{
				_createAccountCommand = _createAccountCommand ?? new MvxCommand(DoCreateAccountCommand);
				return _createAccountCommand;
			}
		}

		async void DoCreateAccountCommand()
		{
			User user = new User { email = Email, username = Username, password = EncryptionService.encrypt(Password), name = Name, lastname = Lastname, image_url=UserImageUrl,created_at = DateTime.Now, updated_at = DateTime.Now };

			try
			{
				OperationResult result = await _mLearningService.CreateAccount<User>(user, (usr) => usr.id, _selectedUserType);
				SessionService.LogIn(result.id, Username);

				SharedPreferences prefs = Constants.GetSharedPreferences(Constants.PreferencesFileName);
				prefs.PutString(Constants.UserFirstNameKey, Name);
				prefs.PutString(Constants.UserLastNameKey, Lastname);
				prefs.PutString(Constants.UserImageUrlKey, UserImageUrl);

				prefs.Commit();


				ClearProperties();
				ShowViewModel<MainViewModel>();
				RegistrationOK = true;

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
				bool exists = await _mLearningService.CheckIfExists<User>(usr => usr.username == Username, (it) => it.updated_at, it => it.id);
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


		MvxCommand _takePictureCommand;
		public System.Windows.Input.ICommand TakePictureCommand
		{
			get
			{
				_takePictureCommand = _takePictureCommand ?? new MvxCommand(DoTakePictureCommand);
				return _takePictureCommand;
			}
		}

		void DoTakePictureCommand()
		{
			var task = Mvx.Resolve<IMvxPictureChooserTask>();

			task.TakePicture(_maxPixelDimension, _percentQuality, UploadUserImage, () => { });
		}

		async public void UploadUserImage(Stream s)
		{
			UserImageUrl = await _mLearningService.UploadResource(s,null);

			UploadFinished = true;
		}

		MvxCommand _choosePictureFromLibraryCommand;
		public System.Windows.Input.ICommand ChoosePictureFromLibraryCommand
		{
			get
			{
				_choosePictureFromLibraryCommand = _choosePictureFromLibraryCommand ?? new MvxCommand(DoChoosePictureFromLibraryCommand);
				return _choosePictureFromLibraryCommand;
			}
		}

		void DoChoosePictureFromLibraryCommand()
		{
			var task = Mvx.Resolve<IMvxPictureChooserTask>();

			task.ChoosePictureFromLibrary(_maxPixelDimension, _percentQuality, UploadUserImage, () => { });
		}
		void ClearProperties()
		{
			Email = Password = Name = Lastname = Username = "";

		}
	}
}

