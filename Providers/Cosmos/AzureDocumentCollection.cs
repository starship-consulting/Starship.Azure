﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;

namespace Starship.Azure.Providers.Cosmos {

    public class AzureDocumentCollection {

        public AzureDocumentCollection(DocumentClient client, string databaseName, DocumentCollection collection, JsonSerializerSettings settings) {
            Client = client;
            DatabaseName = databaseName;
            Collection = collection;
            Settings = settings;
            Options = new FeedOptions { EnableCrossPartitionQuery = true };
        }
        
        public async Task BulkDelete() {
            BulkDeletionState state;

            do {
                state = await BulkDelete("SELECT c._self FROM c");
            }
            while(state.Continuation);
        }

        public async Task<BulkDeletionState> BulkDelete(string query) {
            var procedure = await CallProcedure<BulkDeletionState>("bulkDelete", query);
            return procedure.Response;
        }
        
        public async Task<List<T>> CallProcedure<T>(string procedureName, List<Resource> parameters) {
            var uri = UriFactory.CreateStoredProcedureUri(DatabaseName, Collection.Id, procedureName);
            var result = await Client.ExecuteStoredProcedureAsync<string>(uri, parameters);
            return JsonConvert.DeserializeObject<List<T>>(result.Response);
        }

        public async Task<StoredProcedureResponse<T>> CallProcedure<T>(string procedureName, string parameter) {
            var uri = UriFactory.CreateStoredProcedureUri(DatabaseName, Collection.Id, procedureName);
            return await Client.ExecuteStoredProcedureAsync<T>(uri, parameter);
        }

        public async Task<List<Document>> CallProcedure(string procedureName, string partitionKey, List<Document> parameters) {
            var uri = UriFactory.CreateStoredProcedureUri(DatabaseName, Collection.Id, procedureName);
            var result = await Client.ExecuteStoredProcedureAsync<string>(uri, new RequestOptions { PartitionKey = new PartitionKey(partitionKey) }, parameters);
            return JsonConvert.DeserializeObject<List<Document>>(result.Response);
        }

        public async Task<Document> SaveAsync(object entity) {
            
            /*if(entity is Document) {
                throw new Exception("Never save objects inheriting from Document!  Data corruption will occur.");
            }*/

            var result = await Client.UpsertDocumentAsync(GetDocumentUri(), entity, new RequestOptions { JsonSerializerSettings = Settings });
            return result.Resource;
        }

        public IOrderedQueryable<T> Get<T>() {
            return Client.CreateDocumentQuery<T>(GetCollectionUri(), new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = -1 });
        }

        public IOrderedQueryable<dynamic> Get() {
            return Client.CreateDocumentQuery<dynamic>(GetCollectionUri(), new FeedOptions { EnableCrossPartitionQuery = true });
        }

        public IEnumerable<Document> Get(int take = 0, SqlQuerySpec sqlQuery = null) {

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
        }

        public async Task DeleteAsync() {
            await Client.DeleteDocumentCollectionAsync(GetCollectionUri());
        }

        public async Task DeleteAsync(string id) {
            await Client.DeleteDocumentAsync(GetDocumentUri(id));
        }

        public Document First() {
            return Client.CreateDocumentQuery(GetCollectionUri(), Options).Take(1).ToList().FirstOrDefault();
        }

        public List<T> Get<T>(params string[] ids) where T : Resource {
            return Client.CreateDocumentQuery<T>(GetCollectionUri(), Options).Where(each => ids.Contains(each.Id)).ToList();
        }

        public T Find<T>(string id) where T : Resource {
            return Client.CreateDocumentQuery<T>(GetCollectionUri(), Options).Where(each => each.Id == id).ToList().FirstOrDefault();
        }

        public Document Find(string id) {
            return Client.CreateDocumentQuery(GetCollectionUri(), Options).Where(each => each.Id == id).ToList().FirstOrDefault();
        }

        public IEnumerable<dynamic> Get(SqlQuerySpec sqlQuery) {

            if(sqlQuery != null) {
                return Client.CreateDocumentQuery(GetCollectionUri(), sqlQuery, Options);
            }

            return Client.CreateDocumentQuery(GetCollectionUri(), Options);
        }

        public Uri GetDocumentUri(string id = "") {
            return UriFactory.CreateDocumentUri(DatabaseName, Collection.Id, id);
        }

        public Uri GetCollectionUri() {
            return UriFactory.CreateDocumentCollectionUri(DatabaseName, Collection.Id);
        }

        public DocumentCollection Collection { get; set; }

        public string DatabaseName { get; set; }

        private DocumentClient Client { get; set; }

        private JsonSerializerSettings Settings { get; set; }

        private FeedOptions Options { get; set; }
    }
}