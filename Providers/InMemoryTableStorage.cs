using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Starship.Azure.Providers.Tables;

namespace Starship.Azure.Providers {
    public class InMemoryTableStorage : ITableStorage {

        public InMemoryTableStorage() {
            Data = new ConcurrentDictionary<string, ConcurrentBag<object>>();
        }

        public async Task Save(string tableName, object entity) {
            var collection = Data.GetOrAdd(tableName, s => new ConcurrentBag<object>());

            collection.Add(entity);
        }

        public async Task<List<T>> All<T>(string tableName) {
            ConcurrentBag<object> collection;

            Data.TryGetValue(tableName, out collection);

            return collection != null ? collection.Cast<T>().ToList() : new List<T>();
        }


        public async Task<List<T>> Query<T>(string tableName, Func<T, bool> predicate) where T : IPartitionedEntity {
            ConcurrentBag<object> collection;

            Data.TryGetValue(tableName, out collection);

            return collection != null ? collection.Cast<T>().Where(predicate).ToList() : new List<T>().ToList();
        }

        private ConcurrentDictionary<string, ConcurrentBag<object>> Data { get; set; } 
    }
}