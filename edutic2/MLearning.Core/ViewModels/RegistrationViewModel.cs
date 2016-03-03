using Cirrious.MvvmCross.ViewModels;
using Core.Session;
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

    //Register the current Consumer to a Circle -- Testing Purposes
    public class RegistrationViewModel : MvxViewModel
    {


        private IMLearningService _mLearningService;


        public RegistrationViewModel(IMLearningService mlearningService)
        {
            _mLearningService = mlearningService;
            UpdateCircleList(FilterCircleName);
        }



        async void UpdateCircleList(string filter)
        {
            var list = await _mLearningService.GetCircles(filter);
            CircleList = new ObservableCollection<Circle>(list);
        }



        string _filterCircleName;
        public string FilterCircleName
        {
            get { return _filterCircleName; }
            set { _filterCircleName = value; RaisePropertyChanged("FilterCircleName"); UpdateCircleList(_filterCircleName); }
        }



        ObservableCollection<Circle> _circleList;
        public ObservableCollection<Circle> CircleList
        {
            get { return _circleList; }
            set { _circleList = value; RaisePropertyChanged("CircleList"); }
        }



        MvxCommand<Circle> _registerToCircleCommand;
        public System.Windows.Input.ICommand RegisterToCircleCommand
        {
            get
            {
                _registerToCircleCommand = _registerToCircleCommand ?? new MvxCommand<Circle>(DoRegisterToCircleCommand);
                return _registerToCircleCommand;
            }
        }

        async void DoRegisterToCircleCommand(Circle circle)
        {
            await _mLearningService.AddUserToCircle(SessionService.GetUserId(),circle.id );

            ShowViewModel<MainViewModel>();
        }







    }
}
