using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Starship.Azure.Data;
using Starship.Core.Data;
using Starship.Core.Utility;
using Starship.Data.Configuration;

namespace Starship.Azure.Providers.Cosmos {

    public class AzureCosmosCollection : IsDataProvider {

        internal AzureCosmosCollection(Container container, DataSettings settings) {
            ChangeSet = new List<object>();
            Container = container;
            Settings = settings;

            Options = new QueryRequestOptions {
                MaxItemCount = 1000
            };
            
            /*Options = new FeedOptions {
                EnableCrossPartitionQuery = true,
                MaxItemCount = 1000,
                MaxDegreeOfParallelism = 16
            };*/
        }
        
        /*public async Task BulkDelete() {
            await BulkDelete("SELECT c._self FROM c");
        }*/

        public T Add<T>(T entity) {
            ChangeSet.Add(entity);
            return entity;
        }

        public void Save() {
            lock(ChangeSet) {
                foreach(var entity in ChangeSet) {
                    Save(entity);
                }

                ChangeSet.Clear();
            }
        }

        public async Task SaveAsync() {
            List<object> items;

            lock(ChangeSet) {
                items = ChangeSet.ToList();
                ChangeSet.Clear();
            }

            foreach(var entity in items) {
                await SaveAsync(entity);
            }
        }

        public async Task BulkDelete(string query) {

            BulkDeletionState state;

            do {
                state = await CallProcedure<BulkDeletionState>("bulkDelete", query);
            }
            while(state.Continuation);
        }
        
        public async Task<List<T>> CallProcedure<T>(string procedureName, List<dynamic> documents) {

            var result = await Container.Scripts.ExecuteStoredProcedureAsync<List<T>>(procedureName, PartitionKey.None, documents.ToArray());
            return result.Resource;
            //var result = await Client.ExecuteStoredProcedureAsync<string>(uri, parameters);
            //return JsonConvert.DeserializeObject<List<T>>(result.Response);
        }

        public async Task<T> CallProcedure<T>(string procedureName, params dynamic[] parameters) {

            var result = await Container.Scripts.ExecuteStoredProcedureAsync<T>(procedureName, PartitionKey.None, parameters);
            return result.Resource;

            //var uri = UriFactory.CreateStoredProcedureUri(DatabaseName, Collection.Id, procedureName);
            //return await Client.ExecuteStoredProcedureAsync<T>(uri, parameters);
        }

        /*public async Task<List<Document>> CallProcedure(string procedureName, string partitionKey, List<Document> parameters) {
            var uri = UriFactory.CreateStoredProcedureUri(DatabaseName, Collection.Id, procedureName);
            var result = await Client.ExecuteStoredProcedureAsync<string>(uri, new RequestOptions { PartitionKey = new PartitionKey(partitionKey) }, parameters);
            return JsonConvert.DeserializeObject<List<Document>>(result.Response);
        }*/

        public T Save<T>(T entity) {
            var result = Container.UpsertItemAsync(entity).Result;
            return result.Resource;
        }

        public async Task<List<T>> SaveAsync<T>(List<dynamic> documents) {
            return await CallProcedure<T>(Settings.SaveProcedureName, documents);
        }
        
        public async Task<T> SaveAsync<T>(T entity) {
            var result = await Container.UpsertItemAsync(entity);
            return result.Resource;
        }

        public IQueryable<CosmosDocument> Get(Type type) {
            var method = GetType().GetMethod("InternalGet", BindingFlags.NonPublic | BindingFlags.Instance);
            return method.MakeGenericMethod(type).Invoke(this, null) as IOrderedQueryable<CosmosDocument>;
        }

        public IQueryable<T> GetAsync<T>() {
            return Container.GetItemLinqQueryable<T>(false, Options);
        }

        public IQueryable<T> Get<T>() {
            return InternalGet<T>();
        }

        private IQueryable<T> InternalGet<T>() {
            return Container.GetItemLinqQueryable<T>(true, Options).Select(each => each);
        }

        public IQueryable<dynamic> Get() {
            return InternalGet<dynamic>();
        }

        private string ToSqlQueryText(IQueryable queryable) {
            return queryable.InvokeMethod("ToSqlQueryText").ToString();
        }

        /*public IEnumerable<Document> Get(int take = 0, SqlQuerySpec sqlQuery = null) {

            IQueryable<Document> query;

            if(sqlQuery != null) {
                query = Client.CreateDocumentQuery<Document>(GetCollectionUri(), sqlQuery, Options);
            }
            else {
                query = Client.CreateDocumentQuery<Document>(GetCollectionUri(), Options);
            }
            
            if(take > 0) {
                return query.Take(take);
            }

            return query;
        }*/

        public async Task DeleteAsync() {
            await Container.DeleteContainerAsync();
        }

        /*public async Task DeleteAsync(CosmosDocument document) {
            await DeleteAsync(document.Id);
        }*/

        public async Task DeleteAsync(string id) {
            await Container.DeleteItemAsync<dynamic>(id, PartitionKey.None);
        }

        /*public Document First() {
            return Client.CreateDocumentQuery(GetCollectionUri(), Options).Take(1).ToList().FirstOrDefault();
        }*/

        /*public List<T> Get<T>(params string[] ids) where T : Resource {
            return Client.CreateDocumentQuery<T>(GetCollectionUri(), Options).Where(each => ids.Contains(each.Id)).ToList();
        }*/

        public async Task<T> FindAsync<T>(string id) {
            var item = await Container.ReadItemAsync<T>(id, PartitionKey.None);
            return item.Resource;
        }

        public T Find<T>(string id) {
            return FindAsync<T>(id).Result;
        }

        public dynamic Find(string id) {
            return Find<dynamic>(id);
        }

        /*public IEnumerable<dynamic> Get(SqlQuerySpec sqlQuery) {

            if(sqlQuery != null) {
                return Client.CreateDocumentQuery(GetCollectionUri(), sqlQuery, Options);
            }

            return Client.CreateDocumentQuery(GetCollectionUri(), Options);
        }*/

        /*public Uri GetDocumentUri(string id = "") {
            return UriFactory.CreateDocumentUri(DatabaseName, Collection.Id, id);
        }

        public Uri GetCollectionUri() {
            return UriFactory.CreateDocumentCollectionUri(DatabaseName, Collection.Id);
        }*/
        
        public string DatabaseName { get; set; }

        private Container Container { get; set; }

        //private CosmosClient Client { get; set; }

        private DataSettings Settings { get; set; }

        private QueryRequestOptions Options { get; set; }

        private List<object> ChangeSet { get; set; }
    }
}