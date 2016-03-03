using Cirrious.CrossCore;
using Cirrious.CrossCore.IoC;
using Core.Repositories;
using Core.Session;
using Core.ViewModels;
using KunFood.Core.ViewModels;
using MLearning.Core.ViewModels;

namespace MLearning.Core
{
    public class App : Cirrious.MvvmCross.ViewModels.MvxApplication
    {
        public override void Initialize()
        {
            CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();

        

            //Test if credentials are saved in storage
            string username;
            int user_id;
            bool login = SessionService.HasLoggedIn(out username, out user_id);

            if (login)
            {
                RegisterAppStart<MainViewModel>();                
            }
            else
            {
                //RegisterAppStart<CreateInstitutionViewModel>();      
                RegisterAppStart<LoginViewModel>();                
               // RegisterAppStart<AuthViewModel>();
            }
            
                       
            
        }
    }
}