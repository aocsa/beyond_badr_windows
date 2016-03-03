using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.File;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Referee.Core.Session
{

     
    public class SharedPreferences
    {



        public class PrefEntry
        {
            public string key;
            public string value;

        }

        Dictionary<string, PrefEntry> _preferences;


        string _prefFileName;

        public SharedPreferences(string prefFile)
        {
            _prefFileName = prefFile;

            loadPrefFile(prefFile);
        }



        void loadPrefFile(string filename )
        {
            var storage = Mvx.Resolve<IMvxFileStore>();
            if (storage.Exists(filename))
            {
                // Load It

                string contents;
                if (storage.TryReadTextFile(filename, out  contents))
                {
                    var list = JsonConvert.DeserializeObject<List<PrefEntry>>(contents);
                    _preferences = list.ToDictionary(x => x.key, x => x);
                }

            }
            else
            {
                _preferences = new Dictionary<string, PrefEntry>();
            }
        }


        void savePrefFile(string filename)
        {
            var storage = Mvx.Resolve<IMvxFileStore>();
            List<PrefEntry> toSave;

            toSave = _preferences.Values.ToList();

            var text = JsonConvert.SerializeObject(toSave);

            storage.WriteFile(filename, text); ;
        }



        public string GetString(string key)
        {
            if (_preferences.ContainsKey(key))
                return _preferences[key].value;

            return string.Empty;
        }

        public void PutString(string key, string value)
        {
            _preferences[key] = new PrefEntry { key = key, value = value};
        }

          public bool GetBoolean(string p)
        {
              if(_preferences.ContainsKey(p))
                return Convert.ToBoolean(_preferences[p]);

              return false;
        }

         public void PutBoolean(string key, bool value)
        {
            _preferences[key] = new PrefEntry { key = key, value = value.ToString() };
        
        }


       public void Commit()
        {
            savePrefFile(_prefFileName);
        
        }
    }
}
