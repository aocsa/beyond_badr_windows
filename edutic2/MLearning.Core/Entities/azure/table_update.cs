using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLearning.Core.Entities
{
    public class table_update
    {
        public int id { get; set; }
        public string table_name { get; set; }

        public DateTime updated_at { get; set; }
    }
}
