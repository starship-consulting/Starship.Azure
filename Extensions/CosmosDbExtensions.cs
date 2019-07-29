using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Linq;

namespace Starship.Azure.Extensions {
    public static class CosmosDbExtensions {

        public static async Task<List<T>> ToListAsync<T>(this IOrderedQueryable<T> query) {
            
            var iterator = query.ToFeedIterator();
            var results = new List<T>();

            while (iterator.HasMoreResults) {
                foreach(var item in await iterator.ReadNextAsync()) {
                    results.Add(item);
                }
            }

            return results;
        }
    }
}