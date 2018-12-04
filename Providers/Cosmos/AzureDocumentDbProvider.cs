using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using Starship.Azure.Json;

namespace Starship.Azure.Providers.Cosmos {
    public class AzureDocumentDbProvider {
        
        public AzureDocumentDbProvider(string uri, string authenticationKey, string databaseName) {
            Client = new DocumentClient(new Uri(uri), authenticationKey);
            DatabaseName = databaseName;
            DatabaseUri = UriFactory.CreateDatabaseUri(DatabaseName);
            Settings = new JsonSerializerSettings {
                ContractResolver = new DocumentContractResolver()
            };
        }

        public async Task<string> Save(string collectionName, object entity) {
            var collection = await GetCollection(collectionName);
            var result = await Client.UpsertDocumentAsync(collection.GetDocumentUri(), entity, new RequestOptions { JsonSerializerSettings = Settings });
            return result.Resource.Id;
        }

        public IEnumerable<string> GetCollections() {
            var collections = Client.CreateDocumentCollectionQuery(DatabaseUri);
            return collections.Select(each => each.Id).ToList();
        }

        public async Task<IEnumerable<T>> Get<T>(string collectionName) {
            var collection = await GetCollection(collectionName);
            return Client.CreateDocumentQuery<T>(collection.GetCollectionUri());
        }

        public async Task<IEnumerable<Document>> Get(string collectionName, int take = 0, SqlQuerySpec sqlQuery = null) {
            var collection = await GetCollection(collectionName);
            var query = Client.CreateDocumentQuery<Document>(collection.GetCollectionUri(), sqlQuery);

            if(take > 0) {
                return query.Take(take);
            }

            return query;
        }

        public async Task<Document> Get(string collectionName, string id) {
            var collection = await GetCollection(collectionName);
            return Client.CreateDocumentQuery(collection.GetCollectionUri()).Where(each => each.Id == id).ToList().FirstOrDefault();
        }

        public async Task<Document> First(string collectionName) {
            var collection = await GetCollection(collectionName);
            return Client.CreateDocumentQuery(collection.GetCollectionUri()).Take(1).ToList().FirstOrDefault();
        }

        public async Task DeleteCollection(string collectionName) {
            var collection = await GetCollection(collectionName);
            await Client.DeleteDocumentCollectionAsync(collection.GetCollectionUri());
        }

        public async Task Delete(string collectionName, string id) {
            var collection = await GetCollection(collectionName);
            await Client.DeleteDocumentAsync(collection.GetDocumentUri(id));
        }

        public async Task<ResourceResponse<DocumentCollection>> CreateCollection(string collectionName) {
            return await Client.CreateDocumentCollectionIfNotExistsAsync(DatabaseUri, new DocumentCollection { Id =  collectionName });
        }

        private async Task<AzureDocumentCollection> GetCollection(string collectionName) {
            
            await Semaphore.WaitAsync();

            try {
                if(!Collections.ContainsKey(collectionName)) {
                    var result = await CreateCollection(collectionName);
                    Collections.Add(collectionName, new AzureDocumentCollection(DatabaseName, result.Resource));
                }

                return Collections[collectionName];
            }
            finally {
                Semaphore.Release();
            }
        }

        private string DatabaseName { get; set; }

        private Uri DatabaseUri { get; set; }

        private DocumentClient Client { get; set; }

        private static JsonSerializerSettings Settings { get; set; }

        private static readonly Dictionary<string, AzureDocumentCollection> Collections = new Dictionary<string, AzureDocumentCollection>();

        private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);
    }
}