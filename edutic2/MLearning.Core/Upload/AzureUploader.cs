using Cirrious.CrossCore;
using Core.Entities;
using Core.Repositories;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AzureBlobUploader
{
    public class AzureUploader
    {



        //Register: Get SASQueryString (permissions) for Azure Uploading
        static async Task<Resource> registerResource(string filepath)
        {
            if (filepath == null)
            {
                filepath = getFileId();
            }

            Resource resource = new Resource { LocalPath = filepath, ContainerName = "mlresources" };
            IRepositoryService repo = new WAMSRepositoryService();

            await repo.InsertAsync<Resource>(resource);

            return resource;

        }

        //Register: Get SASQueryString (permissions) for Azure Uploading
        static async Task<Resource[]> registerResource(int nresources,List<string> filepaths)
        {
            Resource[] resourceList = new Resource[nresources];

            for (int i = 0; i < resourceList.Length; i++)
            {
                string filepath = getFileId();

                if (filepaths != null)
                    filepath = filepaths[i];

                resourceList[i] = new Resource { LocalPath = filepath, ContainerName = "mlresources" }; ;
            }

            if(nresources>0)
            {

                Resource r = await registerResource(resourceList[0].LocalPath);

                Uri uri = new Uri(r.CloudPath);
                var noLastSegment = string.Format("{0}://{1}", uri.Scheme, uri.Authority);

                for (int i = 0; i < uri.Segments.Length - 1; i++)
                {
                    noLastSegment += uri.Segments[i];
                }

                //Set CloudPath and SasQueryString for all resources
                for (int i = 0; i < resourceList.Length; i++)
                {                  

                    resourceList[i].SasQueryString = r.SasQueryString ;
                    resourceList[i].CloudPath = noLastSegment + resourceList[i].LocalPath;
                }
            }
            

             return resourceList;

        }

        static  void uploadResource(Resource resource,Stream stream)
        {


            if (!string.IsNullOrEmpty(resource.SasQueryString))
            {
                
                // Get the URI generated that contains the SAS 
                // and extract the storage credentials.
                StorageCredentials cred = new StorageCredentials(resource.SasQueryString);
                var imageUri = new Uri(resource.CloudPath);

                // Instantiate a Blob store container based on the info in the returned item.
                CloudBlobContainer container = new CloudBlobContainer(new Uri(string.Format("https://{0}/{1}", imageUri.Host, resource.ContainerName)), cred);




                // Upload the new image as a BLOB from the stream.
                CloudBlockBlob blobFromSASCredential = container.GetBlockBlobReference(resource.LocalPath);


                try
                {
                    stream.Position = 0;
                    blobFromSASCredential.UploadFromStream(stream);
                }
                catch (StorageException e)
                {

                }
                catch (ProtocolViolationException e)
                {

                }
            
                
                Mvx.Trace("Uploaded ");




                // When you request an SAS at the container-level instead of the blob-level,
                // you are able to upload multiple streams using the same container credentials.
            }
            else
            {
                Mvx.Trace("No SAS QUERY ");

            }
         
        }
        public static async Task<string> uploadFile(Stream stream,string filepath)
        {
          
            string cloudPath = String.Empty;
            try
            {

                Resource r = await registerResource(filepath);
                cloudPath = r.CloudPath;
                uploadResource(r, stream);
            }
            catch(WebException e)
            {
                Mvx.Trace("Error: "+e.Message);
                throw;
            }

            return cloudPath;
        }

        public static async Task<List<string>> uploadMultipleFiles(List<Stream> streamList,List<string> filepaths)
        {

            List<string> result = new List<string>();

            try
            {
                Resource [] resources = await registerResource(streamList.Count, filepaths);

                for (int i = 0; i < resources.Length; i++)
                {
                    result.Add(resources[i].CloudPath);
                }

                for (int i = 0; i < streamList.Count; i++)
                {

                    try
                    {
                        uploadResource(resources[i], streamList[i]);

                    }
					catch(StorageException e)
					{

					}
                }
            }
            catch (WebException e)
            {
                Mvx.Trace("Error " + e.Message);
                throw;
            }

           return result;
        }

        static string getFileId()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}
