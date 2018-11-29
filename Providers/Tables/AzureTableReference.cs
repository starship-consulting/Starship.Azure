using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Starship.Data.OData;

namespace Starship.Azure.Providers.Tables {

    public class AzureTableReference : IsQueryInvoker {

        public AzureTableReference(CloudTable table) {
            Entities = new List<ITableEntity>();
            Table = table;
        }

        public List<T> Get<T>(ODataQuery query) {
            return GetAsync<T>(query).Result;
        }

        public async Task<List<T>> GetAsync<T>(ODataQuery query) {
            var tableQuery = new TableQuery();
            tableQuery.FilterString = query.Filter;

            var results = await QueryAsync(tableQuery);
            return AzureTableProvider.Convert<T>(results.ToArray());
        }
        
        public async Task<List<DynamicTableEntity>> QueryAsync(TableQuery query) {
            TableQuerySegment<DynamicTableEntity> segment = null;
            var results = new List<DynamicTableEntity>();

            do {
                segment = await Table.ExecuteQuerySegmentedAsync(query, segment != null ? segment.ContinuationToken : null);

                if (segment.Results.Count > 0) {
                    results.AddRange(segment.Results);
                }
            } while (segment.ContinuationToken != null);

            return results;
        }

        public List<T> Query<T>(TableQuery<T> query) where T : ITableEntity, new() {
            TableQuerySegment<T> segment = null;
            var results = new List<T>();

            do {
                segment = Table.ExecuteQuerySegmented(query, segment != null ? segment.ContinuationToken : null);

                if (segment.Results.Count > 0) {
                    results.AddRange(segment.Results);
                }
            } while (segment.ContinuationToken != null);

            return results;
        }

        public T Add<T>(T entity) where T : ITableEntity {
            Entities.Add(entity);
            return entity;
        }

        public async Task SaveAsync() {
            foreach (var entity in Entities) {
                await Table.ExecuteAsync(TableOperation.InsertOrReplace(entity));
            }
        }

        public List<ITableEntity> Entities { get; set; }

        private CloudTable Table { get; set; }
    }
}