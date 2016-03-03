using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using   System.Collections.ObjectModel;
using System.IO;

using System.Net;

using Cirrious.CrossCore;
using Core.Session;
using MLearning.Core.Configuration;










namespace Core.DownloadCache
{
    public class IncrementalDownload
    {

        public int CurrentSize{get;set;}
        int _next_block_tam;
        //Limits when to download the nextblock
        int _division_factor;

        public IncrementalDownload()
        {
            Clear();
        }

      
        public async Task <List<Tuple<int,byte[]>>> FillStream<T>( List<T> objs,Func<T,string> getUrl)
        {

            List<Tuple<int,byte[]>> toReturn = new List<Tuple<int,byte[]>>();

          
            for (int i = CurrentSize; i < CurrentSize + _next_block_tam; i++)
            {
                if (i < objs.Count)
                {
                   
                 
                        var bytes = await LoadBytes(objs[i], getUrl);
                        toReturn.Add(new Tuple<int, byte[]>(i, bytes));
                  
                   
                }
            }

            CurrentSize += _next_block_tam;


            return toReturn;
        }

        public async Task TryLoadByteVector<T>(int index, List<T> toLoadList, Action<int, byte[]> loadAction, Func<T, string> getUrl)
        {
            if (index >= CurrentSize / _division_factor)
            {
                //Download the next block  and add them to the Property
                var list = await FillStream<T>(toLoadList,getUrl);

                foreach (var item in list)
                {                 
                    //LocalsResult[item.Item1].image_bytes = bytes;
                    loadAction(item.Item1, item.Item2);
                }


            }
        }


        

      

        public async Task<byte[]> LoadBytes<T>(T toLoadObj, Func<T, string> getUrl)
        {
                byte[] toReturn = null;

                
                CacheService cache = CacheService.Init(SessionService.GetCredentialFileName(),Constants.PreferencesFileName,Constants.LocalDbName);
                try
                {
                      
                    //var request = (HttpWebRequest)WebRequest.Create(getUrl(toLoadObj));
                    //request.Method = "GET";
                    //await cache.makeRequest(request, stream => { BinaryReader r = new BinaryReader(stream); toReturn =  r.ReadBytes((int)stream.Length); }, error => { throw error; }, true);

                   var returnTuple = await cache.tryGetResource(getUrl(toLoadObj));
                   toReturn = returnTuple.Item1;
                }
                catch(Exception ex)
                {
                    Mvx.Trace("Resource not found");
                    
                }

                return toReturn;
       }

        public void Clear()
        {
            CurrentSize = 0;
            _next_block_tam = 50;
            _division_factor = 2;
        }
        
    }


    public class BlockDownload
    {

        public static async Task TryLoadByteVector<T>(List<T> toLoadList,Action<int,byte[]> loadAction, Func<T, string> getUrl)
        {
            CacheService cache = CacheService.Init(SessionService.GetCredentialFileName(), Constants.PreferencesFileName, Constants.LocalDbName);
            for (int i = 0; i < toLoadList.Count;i++ )
            {
               await cache.tryPutResourceIn<T>(getUrl(toLoadList[i]), (bytes) => { loadAction(i, bytes); });
            }
        }

        public static async Task TryPutBytesInVector<T>(List<T> toLoadList, Action<int, byte[]> loadAction, Func<T, string> getUrl)
        {
            CacheService cache = CacheService.Init(SessionService.GetCredentialFileName(), Constants.PreferencesFileName, Constants.LocalDbName);
            for (int i = 0; i < toLoadList.Count; i++)
            {
                int local_pos = i;
                await cache.tryPutResourceIn<T>(getUrl(toLoadList[i]), (bytes) => { loadAction(local_pos, bytes); });
            }
        }
    
    }

    
        




}
