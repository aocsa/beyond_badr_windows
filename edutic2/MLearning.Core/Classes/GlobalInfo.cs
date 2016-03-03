using Cirrious.MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Classes
{
    public class GlobalInfo : MvxNotifyPropertyChanged
    {


        bool _refreshPosts;
        public bool RefreshPosts
        {
            get { return _refreshPosts; }
            set { _refreshPosts = value; RaisePropertyChanged("RefreshPosts"); }
        }



        bool _refreshUsers;
        public bool RefreshUsers
        {
            get { return _refreshUsers; }
            set { _refreshUsers = value; RaisePropertyChanged("RefreshUsers"); }
        }



        bool _refreshCircles;
        public bool RefreshCircles
        {
            get { return _refreshCircles; }
            set { _refreshCircles = value; RaisePropertyChanged("RefreshCircles"); }
        }



        bool _refreshLO;
        public bool RefreshLO
        {
            get { return _refreshLO; }
            set { _refreshLO = value; RaisePropertyChanged("RefreshLO"); }
        }



        bool _refreshQuiz;
        public bool RefreshQuizzes
        {
            get { return _refreshQuiz; }
            set { _refreshQuiz = value; RaisePropertyChanged("RefreshQuizzes"); }
        }





        bool _refreshLOComments;
        public bool RefreshLOComments
        {
            get { return _refreshLOComments; }
            set { _refreshLOComments = value; RaisePropertyChanged("RefreshLOComments"); }
        }


        






    }
}
