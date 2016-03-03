using Core.DownloadCache;
using Core.Session;
using MLearning.Core.Configuration;
using MLearning.Core.Entities.json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLearning.Core.Classes
{
    public class PageParser
    {

        static string cachePrefix = "files/";
        async public static Task DownloadSetBytes(LOContent content)
        {

           await  ProcessSlides(content);
         
        
        
        }

        public static void NullBytes(LOContent content)
        {
            foreach (var slide in content.lopage.loslide)
            {
                

                if (slide.image_bytes != null)
                {
                    slide.image_bytes = null;

                }

                if (slide.loitemize != null)
                {
                    foreach (var itemize in slide.loitemize.loitem)
                    {
                        if (itemize.image_bytes != null)
                        {
                           

                            itemize.image_bytes = null;
                        }
                    }
                }
            }
        
        }

        async private static Task ProcessSlides(LOContent content)
        {

            CacheService cache = CacheService.Init(SessionService.GetCredentialFileName(), Constants.PreferencesFileName, Constants.LocalDbName);

            

            foreach (var slide in content.lopage.loslide)
            {
                


                bool isUrl = Uri.IsWellFormedUriString(slide.loimage,UriKind.Absolute);

                if (slide.loimage != null && isUrl)
                {
                     var bytesAndPath = await cache.tryGetResource(slide.loimage);


                    //Replace Cloud url with local url
                     //slide.loimage = cachePrefix+bytesAndPath.Item2;

                     slide.image_bytes = bytesAndPath.Item1;


                }

                if (slide.loitemize != null)
                {

                    foreach (var itemize in slide.loitemize.loitem)
                    {
                        if (itemize.loimage != null)
                        {
                            var bytesAndPath = await cache.tryGetResource(itemize.loimage);

                            //Replace Cloud url with local url
                            //itemize.loimage = cachePrefix + bytesAndPath.Item2;

                            itemize.image_bytes = bytesAndPath.Item1;
                        }
                    }

                }

            }
        }

      


    }
}
