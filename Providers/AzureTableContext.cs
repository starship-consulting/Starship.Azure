using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.Queryable;
using Starship.Azure.ErrorHandling;
using Starship.Azure.Providers.Tables;

namespace Starship.Azure.Providers {
    public class AzureTableContext<T> : ITableContext<T> where T : ITableEntity, new() {
        protected AzureTableContext() {
        }

        public AzureTableContext(CloudStorageAccount account, string tableName) {
            TableName = tableName;
            Client = account.CreateCloudTableClient();
        }
        
        public async Task<IEnumerable<T>> Get(string partitionKey) {
            return await QueryAsync(new TableQuery<T> {
                FilterString = PartitionKeyName + " eq '" + partitionKey + "'"
            });
        }

        public async Task<T> Get(string partitionKey, string rowKey) {
            var result = await GetTable().ExecuteAsync(TableOperation.Retrieve<T>(partitionKey, rowKey));
            return (T) result.Result;
        }

        public async Task CreateAsync() {
            await GetTable().CreateIfNotExistsAsync();
        }

        public void Create() {
            GetTable().CreateIfNotExists();
        }

        public async Task<T> Insert(T entity) {
            return await ExecuteOperationAsync(TableOperation.Insert(entity));
        }

        public async Task<T> SaveAsync(T entity) {
            return await ExecuteOperationAsync(TableOperation.InsertOrReplace(entity));
        }

        public T Save(T entity) {
            return ExecuteOperation(TableOperation.InsertOrReplace(entity));
        }

        private T ExecuteOperation(TableOperation operation) {
            var result = GetTable().Execute(operation);

            if (result == null || result.Result == null) {
                return default(T);
            }

            return (T) result.Result;
        }

        private async Task<T> ExecuteOperationAsync(TableOperation operation) {
            var result = await GetTable().ExecuteAsync(operation);

            if (result == null || result.Result == null) {
                return default(T);
            }

            return (T) result.Result;
        }

        public async Task<List<ITableResult>> Save(List<T> entities, bool insertOrReplace = true) {
            await CreateAsync();

            var batches = new List<TableBatchOperation>();
            var currentBatch = new TableBatchOperation();
            var index = 0;

            batches.Add(currentBatch);

            entities.ForEach(entity => {
                currentBatch.Add(insertOrReplace ? TableOperation.InsertOrReplace(entity) : TableOperation.Insert(entity));

                index += 1;

                if (index == MaxBatchSize) {
                    index = 0;
                    currentBatch = new TableBatchOperation();
                    batches.Add(currentBatch);
                }
            });

            var exceptions = new List<Exception>();

            var results = await Task.WhenAll(batches.Where(each => each.Any()).Select(async batch => {
                IList<TableResult> tableResults = null;

                try {
                    tableResults = await GetTable().ExecuteBatchAsync(batch);
                }
                catch (StorageException exception) {
                    exceptions.Add(new BatchOperationException(exception, batch));
                }

                return tableResults;
            }));

            return results.SelectMany(each => each)
                .Select(each => new AzureTableResult(each))
                .Cast<ITableResult>()
                .ToList();
        }

        public async Task ClearAsync() {
            var table = GetTable();

            foreach (var entity in await AllAsync()) {
                await table.ExecuteAsync(TableOperation.Delete(entity));
            }
        }

        public void Clear() {
            var table = GetTable();

            foreach (var entity in All()) {
                table.Execute(TableOperation.Delete(entity));
            }
        }

        /*public IList<TableResult> Batch(string partitionKey, List<TableOperation> operations)
        {
            var results = new List<TableResult>();

            while (operations.Any())
            {
                var batch = new TableBatchOperation();
                var next = operations.Take(MaxBatchSize).ToList();
                operations.RemoveRange(0, next.Count());

                foreach (var operation in next)
                {
                    batch.Add(operation);
                }

                results.AddRange(Table.ExecuteBatch(batch));
            }

            return results;
        }*/

        public async Task<int> Count() {
            return (await QueryAsync(new TableQuery())).Count();
        }

        public IQueryable<T> Query() {
            return GetTable().CreateQuery<T>();
        }

        public IEnumerable<T> Query(TableQuery<T> query) {
            return GetTable().ExecuteQuery(query);
        }

        public IEnumerable<DynamicTableEntity> Query(TableQuery query) {
            return GetTable().ExecuteQuery(query);
        }

        public async Task<IEnumerable<T>> QueryAsync(IQueryable<T> query) {
            return await QueryAsync(query.AsTableQuery());
        }

        public async Task<IEnumerable<DynamicTableEntity>> AllAsync() {
            return await QueryAsync(new TableQuery());
        }

        public IEnumerable<DynamicTableEntity> All() {
            return Query(new TableQuery());
        }

        public async Task<IEnumerable<DynamicTableEntity>> QueryAsync(TableQuery query) {
            TableQuerySegment<DynamicTableEntity> segment = null;
            var results = new List<DynamicTableEntity>();

            do {
                segment = await GetTable().ExecuteQuerySegmentedAsync(query, segment != null ? segment.ContinuationToken : null);

                if (segment.Results.Count > 0) {
                    results.AddRange(segment.Results);
                }
            } while (segment.ContinuationToken != null);

            return results;
        }

        public async Task<IEnumerable<T>> QueryAsync(TableQuery<T> query) {
            TableQuerySegment<T> segment = null;
            var results = new List<T>();

            do {
                segment = await GetTable().ExecuteQuerySegmentedAsync(query, segment != null ? segment.ContinuationToken : null);

                if (segment.Results.Count > 0) {
                    results.AddRange(segment.Results);
                }
            } while (segment.ContinuationToken != null);

            return results;
        }

        public async Task<IEnumerable<DynamicTableEntity>> QueryAsync(string partitionKey, string rowKey) {
            var query = new TableQuery<DynamicTableEntity>()
                .Where(each => each.PartitionKey == partitionKey && each.RowKey == rowKey)
                .AsTableQuery();

            return await GetTable().ExecuteQuerySegmentedAsync(query, new TableContinuationToken());
        }

        public async Task<TableResultSegment> GetTablesAsync() {
            return await Client.ListTablesSegmentedAsync(new TableContinuationToken());
        }

        public bool Exists() {
            return Client.GetTableReference(TableName).Exists();
        }

        private CloudTable GetTable() {
            lock (Tables) {
                if (!Tables.ContainsKey(TableName)) {
                    Tables.Add(TableName, Client.GetTableReference(TableName));
                    Create();
                }
            }

            return Tables[TableName];
        }


        public const string PartitionKeyName = "PartitionKey";

        public const string RowKeyName = "RowKey";

        public const int MaxBatchSize = 100;

        private String TableName { get; set; }

        private readonly Dictionary<string, CloudTable> Tables = new Dictionary<string, CloudTable>();

        private CloudTableClient Client { get; set; }
    }
}