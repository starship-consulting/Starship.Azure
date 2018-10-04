using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;
using Starship.Data.OData;

namespace Starship.Azure.Providers.Tables {

    public class AzureTableReference : IsQueryInvoker {

        public AzureTableReference(CloudTable table) {
            Entities = new List<ITableEntity>();
            Table = table;
        }

        public List<T> Get<T>(ODataQuery query) {
            var tableQuery = new TableQuery();
            tableQuery.FilterString = query.Filter;

            var results = Query(tableQuery);
            return AzureTableProvider.Convert<T>(results.ToArray());
        }

        public List<DynamicTableEntity> Query(TableQuery query) {
            TableQuerySegment<DynamicTableEntity> segment = null;
            var results = new List<DynamicTableEntity>();

            do {
                segment = Table.ExecuteQuerySegmented(query, segment != null ? segment.ContinuationToken : null);

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

        public void Save() {
            foreach (var entity in Entities) {
                Table.Execute(TableOperation.InsertOrReplace(entity));
            }
        }

        public List<ITableEntity> Entities { get; set; }

        private CloudTable Table { get; set; }
    }
}