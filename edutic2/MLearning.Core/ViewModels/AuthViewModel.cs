using Cirrious.MvvmCross.ViewModels;
using Core.Session;
using Core.ViewModels;
using Microsoft.WindowsAzure.MobileServices;
using MLearning.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLearning.Core.ViewModels
{
	public class AuthViewModel  : MvxViewModel
	{


		private IMLearningService _mLearningService;


		public AuthViewModel(IMLearningService mLearningService)
		{
			_mLearningService = mLearningService;
		}


		#region SocialProperties
		bool _facebookFlag;
		public bool FacebookFlag
		{
			get { return _facebookFlag; }
			set { _facebookFlag = value; RaisePropertyChanged("FacebookFlag"); }
		}



		bool _twitterFlag;
		public bool TwitterFlag
		{
			get { return _twitterFlag; }
			set { _twitterFlag = value; RaisePropertyChanged("TwitterFlag"); }
		}


		bool _linkedInFlag;
		public bool LinkedInFlag
		{
			get { return _linkedInFlag; }
			set { _linkedInFlag = value; RaisePropertyChanged("LinkedInFlag"); }
		}

		#endregion


		#region SocialCommands
		MvxCommand _facebookLoginCommand;
		public System.Windows.Input.ICommand FacebookLoginCommand
		{
			get
			{
				_facebookLoginCommand = _facebookLoginCommand ?? new MvxCommand(DoFacebookLoginCommand);
				return _facebookLoginCommand;
			}
		}

		void DoFacebookLoginCommand()
		{
			FacebookFlag = true;
		}


		MvxCommand _twitterLoginCommand;
		public System.Windows.Input.ICommand TwitterLoginCommand
		{
			get
			{
				_twitterLoginCommand = _twitterLoginCommand ?? new MvxCommand(DoTwitterLoginCommand);
				return _twitterLoginCommand;
			}
		}

		void DoTwitterLoginCommand()
		{
			TwitterFlag = true;
		}



		MvxCommand _linkedInLoginCommand;
		public System.Windows.Input.ICommand LinkedInLoginCommand
		{
			get
			{
				_linkedInLoginCommand = _linkedInLoginCommand ?? new MvxCommand(DoLinkedInLoginCommand);
				return _linkedInLoginCommand;
			}
		}

		void DoLinkedInLoginCommand()
		{
			LinkedInFlag = true;
		}



		MvxCommand _appAccountLoginCommand;
		public System.Windows.Input.ICommand AppAccountLoginCommand
		{
			get
			{
				_appAccountLoginCommand = _appAccountLoginCommand ?? new MvxCommand(DoAppAccountLoginCommand);
				return _appAccountLoginCommand;
			}
		}

		void DoAppAccountLoginCommand()
		{
			ShowViewModel<LoginViewModel>();
		}



		MvxCommand _signUpCOmmand;
		public System.Windows.Input.ICommand SignUpCommand
		{
			get
			{
				_signUpCOmmand = _signUpCOmmand ?? new MvxCommand(DoSignUpCommand);
				return _signUpCOmmand;
			}
		}

		void DoSignUpCommand()
		{
			ShowViewModel<LoginViewModel>();
		}


		MvxCommand<MobileServiceUser> _createUserCommand;
		public System.Windows.Input.ICommand CreateUserCommand
		{
			get
			{
				_createUserCommand = _createUserCommand ?? new MvxCommand<MobileServiceUser>(DoCreateUserCommand);
				return _createUserCommand;
			}
		}



		async void DoCreateUserCommand(MobileServiceUser mobile)
		{
			int user_id = await _mLearningService.TryCreateUser(mobile.UserId, 34);

			SessionService.LogIn(user_id, mobile.UserId, mobile.MobileServiceAuthenticationToken);

			ShowViewModel<MainViewModel>();
		}


		#endregion

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
			ShowViewModel<AuthViewModel>();
		}




	}
}
