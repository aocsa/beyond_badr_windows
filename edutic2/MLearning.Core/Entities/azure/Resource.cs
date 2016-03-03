using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Core.Entities
{
  public  class Resource
    {
		 public string Id {get; set;}
         public string LocalPath { get; set; }
         public string CloudPath { get; set; }
         public string ContainerName { get; set; }  
         public string SasQueryString { get; set; }
		
    }
}
