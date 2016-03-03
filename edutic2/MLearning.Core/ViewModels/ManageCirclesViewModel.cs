using Cirrious.MvvmCross.ViewModels;
using Core.Entities.json;
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

    /// <summary>
    /// Create Consumers
    /// Create LOs
    /// </summary>
    public class ManageCirclesViewModel : MvxViewModel
    {


        private IMLearningService _mLearningService;


        public ManageCirclesViewModel(IMLearningService mlearningService)
        {
            _mLearningService = mlearningService;
        }
        async public void Init(int circle_id,int user_id)
        {
            CircleID = circle_id;
            InstitutionID = await _mLearningService.GetPublisherInstitutionID(user_id);
            LoadConsumersByCircle(circle_id);
            LoadConsumersByInstitution(InstitutionID);
            
            LoadLOByCircle(circle_id);
        }

        async void LoadConsumersByInstitution(int InstitutionID)
        {
            var list = await _mLearningService.GetConsumersByInstitution(InstitutionID);
            ConsumersInstitutionList = new ObservableCollection<consumer_by_institution>(list);
        }

        private void LoadLOByCircle(int circle_id)
        {
          
        }

        async void LoadConsumersByCircle(int circle_id)
        {
     
            var list = await _mLearningService.GetConsumersByCircle(circle_id);
            ConsumersCircleList = new ObservableCollection<consumer_by_circle>(list);



        }





      



        int _circleID;
        public int CircleID
        {
            get { return _circleID; }
            set { _circleID = value; RaisePropertyChanged("CircleID"); }
        }



        int _institutionID;
        public int InstitutionID
        {
            get { return _institutionID; }
            set { _institutionID = value; RaisePropertyChanged("InstitutionID"); }
        }











        ObservableCollection<consumer_by_circle> _consumersCircleList;
        public ObservableCollection<consumer_by_circle> ConsumersCircleList
        {
            get { return _consumersCircleList; }
            set { _consumersCircleList = value; RaisePropertyChanged("ConsumersCircleList"); }
        }




        ObservableCollection<consumer_by_institution> _consumersInstitutionList;
        public ObservableCollection<consumer_by_institution> ConsumersInstitutionList
        {
            get { return _consumersInstitutionList; }
            set { _consumersInstitutionList = value; RaisePropertyChanged("ConsumersInstitutionList"); }
        }





        ObservableCollection<lo_by_circle> _learningObjectsList;
        public ObservableCollection<lo_by_circle> LearningObjectsList
        {
            get { return _learningObjectsList; }
            set { _learningObjectsList = value; RaisePropertyChanged("LearningObjectsList"); }
        }





        /// <summary>
        /// Create consumer account and add it to a Circle
        /// </summary>
        MvxCommand<consumer_by_institution> _addConsumerCommand;
        public System.Windows.Input.ICommand AddConsumerCommand
        {
            get
            {
                _addConsumerCommand = _addConsumerCommand ?? new MvxCommand<consumer_by_institution>(DoAddConsumerCommand);
                return _addConsumerCommand;
            }
        }

        async void DoAddConsumerCommand(consumer_by_institution consumer)
        {

            

            //Update Local List
            var consumerbycircle = new consumer_by_circle { name = consumer.name, lastname = consumer.lastname, username = consumer.username };
            ConsumersCircleList.Add(consumerbycircle);


          

            //Add it to a Circle
            _mLearningService.AddUserToCircle(consumer.id, CircleID);

            

           
        }


        MvxCommand<consumer_by_circle> _unsubscribeConsumerCommand;
        public System.Windows.Input.ICommand UnsubscribeConsumerCommand
        {
            get
            {
                _unsubscribeConsumerCommand = _unsubscribeConsumerCommand ?? new MvxCommand<consumer_by_circle>(DoUnsubscribeConsumerCommand);
                return _unsubscribeConsumerCommand;
            }
        }

        void DoUnsubscribeConsumerCommand(consumer_by_circle consumer)
        {
            ConsumersCircleList.Remove(consumer);

            _mLearningService.UnSubscribeConsumerFromCircle(consumer.id,CircleID);
        }



        

    }
}
