using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.File;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;
using MLearning.Core.File;

namespace Core.DownloadCache
{
    public class CacheService
    {

        

        //Needs MvxFile Plugin
        public class Entry
        {
            public string HttpSource { get; set; }
            public string DownloadedPath { get; set; }
            public DateTime WhenLastAccessedUtc { get; set; }
            public DateTime WhenDownloadedUtc { get; set; }
        }

        public class LocalResource
        {
            public bool InCache { get; set; }

            public string LocalPath { get; set; }

            public byte[] Bytes { get; set; }
        }


        string _indexFileName = "_cacheindex.txt";      
        Dictionary<string, Entry> _indexByHttp;
        static CacheService _instance = null;
        List<string> _persistentFiles;

        static string _entityPrefix = "kf_id_";

        TimeSpan _maxFileAge = TimeSpan.FromDays(3.0);



        IMvxFileStore storage;
        IAsyncStorageService asyncStorage;
        private CacheService()
        {
         
            storage = Mvx.Resolve<IMvxFileStore>();
            asyncStorage = Mvx.Resolve<IAsyncStorageService>();

            _indexByHttp = new Dictionary<string, Entry>();
            _persistentFiles = new List<string>();

            //Add Files that won't be erased by the CacheCleaner
            _persistentFiles.Add(_indexFileName);

            loadEntries();
            checkForUnindexedFiles();
            checkForOldFiles();
            writeEntries();
           
        }

        private CacheService(string credFileName) 
        {
            storage = Mvx.Resolve<IMvxFileStore>();
            asyncStorage = Mvx.Resolve<IAsyncStorageService>();
            _indexByHttp = new Dictionary<string, Entry>();
            _persistentFiles = new List<string>();

            //Add Files that won't be erased by the CacheCleaner
            _persistentFiles.Add(_indexFileName);
            _persistentFiles.Add(credFileName);

            loadEntries();
            checkForUnindexedFiles();
            checkForOldFiles();
            writeEntries();
           
        }

        private CacheService(string credFileName,string shared_prefs,string dbname)
        {
            storage = Mvx.Resolve<IMvxFileStore>();
            asyncStorage = Mvx.Resolve<IAsyncStorageService>();
            _indexByHttp = new Dictionary<string, Entry>();
            _persistentFiles = new List<string>();

            //Add Files that won't be erased by the CacheCleaner
            _persistentFiles.Add(_indexFileName);
            _persistentFiles.Add(credFileName);
            _persistentFiles.Add(shared_prefs);
            _persistentFiles.Add(dbname);

            loadEntries();
            checkForUnindexedFiles();
            checkForOldFiles();
            writeEntries();

        }

        private void checkForOldFiles()
        {
            var now = DateTime.UtcNow;
            var toRemove = _indexByHttp.Values.Where(x => (now - x.WhenDownloadedUtc) > _maxFileAge).ToList();

            foreach (var item in toRemove)
            { 
                _indexByHttp.Remove(item.HttpSource)   ;
            }

            deleteFile(toRemove.Select(x => x.DownloadedPath));
            
        }

        private void deleteFile(string name)
        {
            storage.DeleteFile(name);
        }

        private void deleteFile(IEnumerable<string> names)
        {
            foreach (var name in names)
            {
                deleteFile(name);
            }
        }

        private void checkForUnindexedFiles()
        {
            var files = storage.GetFilesIn("");
            List<string> filepaths = new List<string>();
            string npath = storage.NativePath("");
            npath += "/";
            //Standarization ---------GETFILESIN not reliable
            foreach (var f in files)
            { 
               
                //Removing native path 
                int index = f.IndexOf(npath);
              string cleanPath = (index < 0)
                ? f
                : f.Remove(index, npath.Length);
              filepaths.Add(cleanPath);
            }
           
            var cachedFiles = new Dictionary<string, Entry>();

            foreach (var entry in _indexByHttp)
            {
                cachedFiles[entry.Value.DownloadedPath] = entry.Value;
            }

            foreach (var file in filepaths)
            {

                bool endsWith = false;
                foreach (var pfilename in _persistentFiles)
                {
                    if (file.EndsWith(pfilename))
                        endsWith = true;
                }
                if (!cachedFiles.ContainsKey(file) && !endsWith)
                {
                    deleteFile(file);
                }
            }

        }

        public static CacheService Init()
        {
            if (_instance == null)
                _instance = new CacheService();

            return _instance;
        }

        public static CacheService Init(string credentialFilename)
        {
            if (_instance == null)
                _instance = new CacheService(credentialFilename);

            return _instance;
        }

        public static CacheService Init(string credentialFilename,string shared_prefs,string dbname)
        {
            if (_instance == null)
                _instance = new CacheService(credentialFilename,shared_prefs,dbname);

            return _instance;
        }

        private  async Task loadEntries()
        {


            if (storage.Exists(_indexFileName))
            {
                // Load It

                string contents;
                if (storage.TryReadTextFile(_indexFileName, out  contents))
                {
                    var list = JsonConvert.DeserializeObject<List<Entry>>(contents);
                    _indexByHttp = list.ToDictionary(x => x.HttpSource, x => x);
                }

                //try
                //{
                //    contents = await asyncStorage.TryReadTextFile(_indexFileName);
                //    var list = JsonConvert.DeserializeObject<List<Entry>>(contents);
                //    _indexByHttp = list.ToDictionary(x => x.HttpSource, x => x);

                //}
                //catch (Exception e)
                //{
                //    Mvx.Trace("Could not open file inside LoadEntries - CacheService : " + e.Message);
                //}

            }
          

        }

        private void writeEntries()
        {

            List<Entry> toSave;

            toSave = _indexByHttp.Values.ToList();

            var text = JsonConvert.SerializeObject(toSave);
          

            storage.WriteFile(_indexFileName, text); ;

           

        }


        public async Task<Dictionary<string, byte[]>> cachePropertiesFromObject(object obj)
        {
            var t = obj.GetType().GetTypeInfo();
            var properties = t.DeclaredProperties;

           var dnAttributes =  properties.Where(pi => pi.GetCustomAttributes(typeof(DownloadCacheAttribute), false).Count()> 0);

            //Indexed by property name
           Dictionary<string, byte[]> result = new Dictionary<string, byte[]>();
           foreach (var propInfo in dnAttributes)
           {
               string urlValue = propInfo.GetValue(obj) as string;

               Debug.WriteLine("Property Value: " + urlValue);

               var request = (HttpWebRequest)WebRequest.Create(urlValue);
               request.Method = "GET";
            //   request.Accept = "image/*";
               await makeRequest(request, (stream,url) => 
               {
                   MemoryStream ss = new MemoryStream();
                   stream.CopyToAsync(ss);
                   result.Add(propInfo.Name, ss.ToArray()); 
               }
                                                        //True : We want to cache it
               , (error) => { Debug.WriteLine("Error writing file inside cachePropertiesFromObject"); }, true);
           }

           return result;
            
        
        }
        //Returns localpath
        public   async Task<string> cacheResource(string url,byte[] bytes)
        {

                  await loadEntries();


              

               

                Entry entry = new Entry {  HttpSource = url, WhenDownloadedUtc = DateTime.UtcNow, WhenLastAccessedUtc = DateTime.UtcNow };

                Entry old;
                string filename;

                if (_indexByHttp.TryGetValue(url, out old))
                {
                    //If exist , ovewrite it
                    filename = old.DownloadedPath;

                    entry.DownloadedPath = filename;
                    _indexByHttp[url] = entry;
              
                }
                else
                {
                    //Doesn't exist

                    //Get new FileName            
                    filename = Guid.NewGuid().ToString("N");

                    entry.DownloadedPath = filename;
                    //  add it
                    _indexByHttp.Add(entry.HttpSource, entry);    
                }

             //   FileService.writeStreamAsync(stream, filename);
                storage.WriteFile(filename, bytes);
                

                writeEntries();

                return filename;

            
           
        }


        public async Task cacheObjectList<T>(string identifier,List<T> objList)
        {

            
            string json = JsonConvert.SerializeObject(objList);
            byte[] bytes = GetBytes(json);

            await cacheResource(_entityPrefix+ identifier,bytes);
         

        }

        public T deserializeObjectListFromBytes<T>(byte[] bytes)
        {
            string json = GetString(bytes);

            T result =  JsonConvert.DeserializeObject<T>(json);

            return result;
        }

        // Try to get from cache, if not in cache try to download it

        //Returns:
        //Item1: bytes
        //Item2: local path
        public async Task<Tuple<byte[],string>> tryGetResource(string url)
        {

            bool isCached;

            string localpath = null;

           // var result = await Task.Run(() => tryReadFileFromLocalCache(url, out isCached, out localpath));

            var localResource = await tryReadFileFromLocalCache(url);

            var result = localResource.Bytes;
            isCached = localResource.InCache;

            localpath = localResource.LocalPath;
            

            if (!isCached)                      
            { 

              

                try
                {
                  //  url =  url.Replace(" ", "%20");
                    var request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "GET";
                    await makeRequest(request, (stream, lpath) => { var s = new MemoryStream(); stream.CopyTo(s); result = s.ToArray(); localpath = lpath; }, err => Debug.WriteLine("Trying to get resourse: "+err.ToString()), true);
                }
                catch
                {
                    
                    result = null;
                }
                
            }



            return new Tuple<byte[], string>(result,localpath);
            //return result;
        }

        public async Task tryPutResourceIn<T>(string url, Action<byte[]> loadAction)
        {

            bool isCached;

            string localpath;


           // var result  = await Task.Run(() => tryReadFileFromLocalCache(url, out isCached, out localpath));
            LocalResource localResource = await tryReadFileFromLocalCache(url);
            var result = localResource.Bytes;
             isCached = localResource.InCache;
             localpath = localResource.LocalPath;


            if (!isCached)          
            {
                


                try
                {
                    var request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "GET";
                    await makeRequest(request, (stream,lpath) => { var s = new MemoryStream(); stream.CopyTo(s); result = s.ToArray(); }, err => Debug.WriteLine(err.ToString()), true);
                }
                catch
                {
                    result = null;
                }

            }

            loadAction(result);
        }


         public async Task<LocalResource> tryReadFileFromLocalCache(string url) 
        {
            await loadEntries();


            Entry entry;
            try { 
                bool ok = _indexByHttp.TryGetValue(url, out entry);
                if(entry!=null && ok)
                {
                    string localpath = null;

                    //Update last accessed date
                    //entry.WhenLastAccessedUtc = DateTime.UtcNow;

                    byte[] content;     
           
                
                    // storage.TryReadBinaryFile(entry.DownloadedPath, out content);
              
                    content = await asyncStorage.TryReadBinaryFile(entry.DownloadedPath);

                    //Write modified entries
                    //  writeEntries();
                    localpath = entry.DownloadedPath;


                    return new LocalResource { Bytes = content, InCache = ok, LocalPath = localpath };
                }
                else
                {
                    return new LocalResource { Bytes = null, InCache = false, LocalPath = null};
                }
            }catch(Exception e)
            {
                Debug.WriteLine(e.Message);
                return new LocalResource { Bytes = null, InCache = false, LocalPath = null };
            }


        }


        //Cache: determines if the resource is going to be cached or not.
        //If there's no connection, try to read it from cache
        public async Task makeRequest(HttpWebRequest request, Action<Stream,string> successAction, Action<Exception> errorAction,bool cache)
        {

            WebException connectionError = null;

            try
            {

                
                
                using (var wresponse = await Task<WebResponse>.Factory.FromAsync(request.BeginGetResponse, request.EndGetResponse, request))
                {

                    using (var stream = wresponse.GetResponseStream())
                    {

                        //Using memory stream to reset Position and use the stream twice
                        MemoryStream ss = new MemoryStream();
                        await stream.CopyToAsync(ss);
                        string localpath = null;

                        // Write to Local Cache 

                        if (cache)
                        {
                           

                            byte[] bytes = ss.ToArray();
                           
                           localpath =  await cacheResource(request.RequestUri.OriginalString, bytes);
                            ss.Position = 0;
                        }



                       
                        successAction(ss, localpath);

                    }
                }
            }
            catch (WebException ex)
            {
                //Using Cache  when No connection
                Debug.WriteLine( "No connection");
                connectionError = ex;

              



            }

            if (connectionError!=null)
            {

                bool ok;
                string localpath;
                var localResource = await tryReadFileFromLocalCache(request.RequestUri.ToString());
                var content = localResource.Bytes;
                ok = localResource.InCache;
                localpath = localResource.LocalPath;

                if (ok)
                {

                    successAction(new MemoryStream(content), localpath);
                }
                else
                {
                    Mvx.Error("ERROR: '{0}' when making {1} request to {2}", connectionError.Message, request.Method, request.RequestUri.AbsoluteUri);
                    errorAction(connectionError);
                }
            }

         
        }

        public static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }


        public static string GetStringWithPrefix(string identifier)
        {
            return _entityPrefix + identifier;
        }

         public static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }








    }
}
