using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.File;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Session
{
    public class SessionService
    {
        public class SessionCredentials
        {
            public string username { get; set; }
            public int user_id { get; set; }

            public string auth_token { get; set; }
        }


        static string _credentialFileName = "_usercred";


        static SessionCredentials cred;

        static int _noUserFlag = 3;

        static public int GetUserId()
        {
            if (cred != null)
                return cred.user_id;

            //No User
            return _noUserFlag;

        }


        static public string GetUsername()
        {
            if (cred != null)
                return cred.username;

            return "";
        }

        static public string GetAuthToken()
        {

            if (cred != null)
                return cred.auth_token;

            return "";
        }


        static public string GetCredentialFileName()
        {
            return _credentialFileName;
        }





        public static bool HasLoggedIn(out string username, out int user_id)
        {
            var storage = Mvx.Resolve<IMvxFileStore>();

            if (storage.Exists(_credentialFileName))
            {
                string contents;
                if (storage.TryReadTextFile(_credentialFileName, out contents))
                {

                    try
                    {
                        var sessionObj = JsonConvert.DeserializeObject<SessionCredentials>(contents);
                        username = sessionObj.username;
                        user_id = sessionObj.user_id;

                        //Set global variable UserId 
                        cred = new SessionCredentials { username = username, user_id = user_id };

                        return true;
                    }
                    catch (Exception e)
                    {
                        Mvx.Trace("Could not deserialize credentials " + e.Message);
                    }
                }
                else
                {
                    Mvx.Trace("Could not open file! inside HasLoggedIn");

                }

            }

            //Hasn't logged in
            username = "";
            user_id = 0;

            return false;
        }


        public static void LogIn(int userId, string username)
        {
            cred = new SessionCredentials { username = username, user_id = userId };


            string serialized = JsonConvert.SerializeObject(cred);

            var storage = Mvx.Resolve<IMvxFileStore>();

            storage.WriteFile(_credentialFileName, serialized);
        }

        public static void LogIn(int userId, string username, string authToken)
        {
            cred = new SessionCredentials { username = username, user_id = userId, auth_token = authToken };

            writeToFile(cred);

        }

        private static void writeToFile(SessionCredentials cred)
        {
            string serialized = JsonConvert.SerializeObject(cred);

            var storage = Mvx.Resolve<IMvxFileStore>();

            storage.WriteFile(_credentialFileName, serialized);
        }

        public static void LogOut()
        {
            var storage = Mvx.Resolve<IMvxFileStore>();
            storage.DeleteFile(_credentialFileName);
            cred.user_id = _noUserFlag;
            cred.username = "";

        }
    }
}
