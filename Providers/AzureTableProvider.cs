using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Starship.Azure.Converters;
using Starship.Azure.Extensions;
using Starship.Core.Data;
using Starship.Core.Extensions;

namespace Starship.Azure.Providers {
    public static class AzureTableProvider {
        static AzureTableProvider() {
            TableCache = new Dictionary<string, object>();
        }

        public static List<T> Convert<T>(params DynamicTableEntity[] entities) {
            var serializer = new JsonSerializer();
            serializer.Converters.Add(new DynamicTableEntityConverter());

            var results = new List<T>();
            var partition = typeof(T).GetPartitionProperty();
            var primaryKey = typeof(T).GetPrimaryKeyProperty();

            foreach (var each in entities) {
                var converted = JObject.FromObject(each, serializer).ToObject<T>(serializer);

                if (partition != null) {
                    partition.SetValue(converted, each.PartitionKey.As(partition.PropertyType));
                }

                if (primaryKey != null) {
                    primaryKey.SetValue(converted, each.RowKey.As(primaryKey.PropertyType));
                }

                results.Add(converted);
            }

            return results;
        }

        public static DynamicTableEntity MakeTableEntity(object entity) {
            var partition = entity.GetType().GetPartitionProperty();
            var primaryKey = entity.GetType().GetPrimaryKeyProperty();
            var tableEntity = new DynamicTableEntity();

            if (partition != null) {
                tableEntity.PartitionKey = entity.GetPartition();
            }

            if (primaryKey != null) {
                tableEntity.RowKey = entity.GetPrimaryKey();
            }

            tableEntity.Apply(entity, partition != null ? partition.Name : string.Empty, primaryKey != null ? primaryKey.Name : string.Empty);

            return tableEntity;
        }

        public static AzureTableContext<T> GetContext<T>(string tableName) where T : ITableEntity, new() {
            lock (TableCache) {
                if (!TableCache.ContainsKey(tableName)) {
                    var context = new AzureTableContext<T>(GetAccount(), tableName);
                    TableCache.Add(tableName, context);
                    context.Create();
                }

                return TableCache[tableName] as AzureTableContext<T>;
            }
        }

        private static CloudStorageAccount GetAccount() {
            return CloudStorageAccount.Parse(ConnectionString);
        }

        public static string ConnectionString = string.Empty;

        private static Dictionary<string, object> TableCache { get; }
    }
}