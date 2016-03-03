using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.WindowsAzure.MobileServices;

namespace Core.Classes
{
    public static class Extensions
    {

        public static bool HasProperty(this Type obj, string propertyName)
        {
            
            return obj.GetTypeInfo().DeclaredProperties.Any(p => p.Name == propertyName);
            
        }

        public async static Task<List<T>> LoadAllAsync<T>(this IMobileServiceTableQuery<T> table, int bufferSize = 1000)
        {
            var query = table.IncludeTotalCount();
            var results = await query.ToEnumerableAsync();
            long count = ((ITotalCountProvider)results).TotalCount;
            var updates = new List<T>();
            if (results != null && count > 0)
            {
                
                while (updates.Count < count)
                {

                    var next = await query.Skip(updates.Count).Take(bufferSize).ToListAsync();
                    updates.AddRange(next);
                }
                
            }

            return updates;
        }
    }
}
