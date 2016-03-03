using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLearning.Core.Classes
{
    public class DataPage<T>
    {
        public IList<T> data { get; set; }
        public long totalCount { get; set; }
        public int take { get; set; }
        public int skip { get; set; }
    }
}
