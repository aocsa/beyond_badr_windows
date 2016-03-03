using MLearning.Core.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;

namespace MLearning.Store.File
{
    public class AsyncStorageStoreService : IAsyncStorageService
    {


        public async Task<byte[]> TryReadBinaryFile(string filename)
        {
            StorageFile storageFile =await ApplicationData.Current.LocalFolder.GetFileAsync(filename);            
            IBuffer buffer = await FileIO.ReadBufferAsync(storageFile);                        
            byte[] bytes = buffer.ToArray(); 
            return bytes;
            
        }

    

        public async Task<string> TryReadTextFile(string filename)
        {
            StorageFile storageFile = await ApplicationData.Current.LocalFolder.GetFileAsync(filename); 
            string text = await FileIO.ReadTextAsync(storageFile);

            return text;
        }
    }
}
