using Cirrious.MvvmCross.ViewModels;
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

    /// <summary>
    /// List Circles
    /// Create Circles
    /// </summary>
    public class PublisherMainViewModel : MvxViewModel
    {


        private IMLearningService _mLearningService;


        public PublisherMainViewModel(IMLearningService mlearningService)
        {
            _mLearningService = mlearningService;

            
        }


        public void Init(int user_id)
        {
            UserID = user_id;
            LoadCircles(user_id);
        }

        async void LoadCircles(int user_id)
        {
            var list = await _mLearningService.GetCirclesByUser(user_id);

            CirclesList = new ObservableCollection<circle_by_user>(list);
        }




        int _userID;
        public int UserID
        {
            get { return _userID; }
            set { _userID = value; RaisePropertyChanged("UserID"); }
        }




        string _circleName;
        public string CircleName
        {
            get { return _circleName; }
            set { _circleName = value; RaisePropertyChanged("CircleName"); }
        }




        int _circleType;
        public int CircleType
        {
            get { return _circleType; }
            set { _circleType = value; RaisePropertyChanged("CircleType"); }
        }





        ObservableCollection<circle_by_user> _circlesList;
        public ObservableCollection<circle_by_user> CirclesList
        {
            get { return _circlesList; }
            set { _circlesList = value; RaisePropertyChanged("CirclesList"); }
        }




        MvxCommand _createCircleCommand;
        public System.Windows.Input.ICommand CreateCircleCommand
        {
            get
            {
                _createCircleCommand = _createCircleCommand ?? new MvxCommand(DoCreateCircleCommand);
                return _createCircleCommand;
            }
        }

        async void DoCreateCircleCommand()
        {
           
            int circle_id = await _mLearningService.CreateCircle(UserID, CircleName, CircleType);
           

            var circle = new circle_by_user { id=circle_id,name = CircleName, type = CircleType };

            CirclesList.Add(circle);

            //Register the Publisher as a user in a Circle
            _mLearningService.CreateObject<CircleUser>(new CircleUser { Circle_id = circle_id, User_id = UserID, created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow }, c => c.id);

            
        }



        MvxCommand<circle_by_user> _manageCirclesCommand;
        public System.Windows.Input.ICommand ManageCirclesCommand
        {
            get
            {
                _manageCirclesCommand = _manageCirclesCommand ?? new MvxCommand<circle_by_user>(DoManageCirclesCommand);
                return _manageCirclesCommand;
            }
        }

        void DoManageCirclesCommand(circle_by_user circle)
        {
            ShowViewModel<ManageCirclesViewModel>(new  {circle_id = circle.id,user_id=UserID});
        }



    }
}
