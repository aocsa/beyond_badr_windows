using Referee.Core.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLearning.Core.Configuration
{
    //UserType Constants also defined in Server Backend
    public enum UserType{Publisher,Consumer,SuperAdmin,Head};


    public enum LOType { Public, Private};
    public enum CircleType { Public,Private};

    public enum QuestionType { Word,SingleChoice,MultipleChoice,Match }
    public class Constants
    {

        public const  string PublisherRole = "Publisher";
        public const string ConsumerRole = "Consumer";
        public const string SuperAdminRole = "SuperAdmin";
        public const string HeadRole = "Head";
		public static SharedPreferences GetSharedPreferences(string prefFilename)
        {
            return new SharedPreferences(prefFilename);
        }
		
        public static int PublisherIdentification = 0;
        public static int ConsumerIdentification = 1;

        public static int NoSelection = -1;

        public static int NoInstitution= -1;

		
		 public static string PreferencesFileName = "user_pref";
		 
		  public static string LocalDbName = "cache.db";

          public static string UserFirstNameKey = "first_name";
          public static string UserLastNameKey = "last_name";
          public static string UserImageUrlKey = "image_url";



        //Notification Hub
          public const string SenderID = "368473764117";
          // Azure app specific connection string and hub path
          public const string ConnectionString = "Endpoint=sb://mlearningservicehub-ns.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=8Rf54DI4X4Ug2cU++7s3myl0jFtwsSWQghBVktMyKfs=";

          public const string NotificationHubPath = "mlearningservicehub";

          


    }
}
