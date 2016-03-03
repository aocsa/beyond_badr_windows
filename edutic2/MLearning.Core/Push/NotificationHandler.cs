using Cirrious.MvvmCross.ViewModels;
using Core.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Push
{
    public class NotificationHandler 
    {

         public static GlobalInfo Properties= new GlobalInfo();

        public static void HandleNotification(string value)
        {
            switch(value)
            {
            
                case "circle_by_user": 

                   Properties.RefreshCircles = true;

                    break;

                case "user_by_circle":

                    Properties.RefreshUsers = true;

                    break;

                case "post_with_username":

                    Properties.RefreshPosts = true;

                 break;

                case "lo_comment_with_username":

                 Properties.RefreshLOComments = true;

                 break;

                case "lo_by_circle":

                 Properties.RefreshLO = true;

                 break;

                case "quiz_by_circle":

                 Properties.RefreshQuizzes = true;

                 break;

                    
            }
        }

    }
}
