using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using Starship.Azure.Json;
using Starship.Core.Extensions;

namespace Starship.Azure.Providers.Cosmos {
    public class AzureDocumentDbProvider {
        
        public AzureDocumentDbProvider(string uri, string authenticationKey, string databaseName) {
            ForceCamelcase = true;
            Client = new DocumentClient(new Uri(uri), authenticationKey);
            DatabaseName = databaseName;
            DatabaseUri = UriFactory.CreateDatabaseUri(DatabaseName);
            Settings = new JsonSerializerSettings {
                ContractResolver = new DocumentContractResolver()
            };
        }

        public async Task<Document> Save(string collectionName, object entity) {
            var collection = await GetCollection(collectionName);
            var result = await Client.UpsertDocumentAsync(collection.GetDocumentUri(), entity, new RequestOptions { JsonSerializerSettings = Settings });
            return result.Resource;
        }

        public IEnumerable<string> GetCollections() {
            var collections = Client.CreateDocumentCollectionQuery(DatabaseUri);
            return collections.Select(each => each.Id).ToList().OrderBy(each => each).ToList();
        }

        public async Task<IEnumerable<T>> Get<T>(string collectionName) {
            var collection = await GetCollection(collectionName);
            return Client.CreateDocumentQuery<T>(collection.GetCollectionUri());
        }

        public async Task<IEnumerable<dynamic>> Get(string collectionName, SqlQuerySpec sqlQuery) {
            var collection = await GetCollection(collectionName);
            IQueryable<dynamic> query;

            if(sqlQuery != null) {
                query = Client.CreateDocumentQuery(collection.GetCollectionUri(), sqlQuery);
            }
            else {
                query = Client.CreateDocumentQuery(collection.GetCollectionUri());
            }
            
            return query;
        }

        public async Task<IEnumerable<Document>> Get(string collectionName, int take = 0, SqlQuerySpec sqlQuery = null) {
            var collection = await GetCollection(collectionName);
            IQueryable<Document> query;

            if(sqlQuery != null) {
                query = Client.CreateDocumentQuery<Document>(collection.GetCollectionUri(), sqlQuery);
            }
            else {
                query = Client.CreateDocumentQuery<Document>(collection.GetCollectionUri());
            }
            
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
            collectionName = ResolveCollectionName(collectionName);
            return await Client.CreateDocumentCollectionIfNotExistsAsync(DatabaseUri, new DocumentCollection { Id =  collectionName }, new RequestOptions{ OfferThroughput = MinimumThroughput });
        }

        private async Task<AzureDocumentCollection> GetCollection(string collectionName) {
            collectionName = ResolveCollectionName(collectionName);

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

        private string ResolveCollectionName(string name) {
            if(ForceCamelcase) {
                return name.CamelCase();
            }
            return name;
        }

        public bool ForceCamelcase { get; set; }
        
        private string DatabaseName { get; set; }

        private Uri DatabaseUri { get; set; }

        private DocumentClient Client { get; set; }

        private static JsonSerializerSettings Settings { get; set; }

        private static readonly Dictionary<string, AzureDocumentCollection> Collections = new Dictionary<string, AzureDocumentCollection>();

        private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);

        private const int MinimumThroughput = 400;
    }
}