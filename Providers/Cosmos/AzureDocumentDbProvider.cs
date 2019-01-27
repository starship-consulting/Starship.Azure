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
        
        public AzureDocumentDbProvider(CosmosDbSettings settings) {

            Settings = settings;
            ForceCamelcase = true;
            Client = new DocumentClient(new Uri(settings.Uri), settings.Key);
            DatabaseName = settings.Database;
            DatabaseUri = UriFactory.CreateDatabaseUri(DatabaseName);

            SerializerSettings = new JsonSerializerSettings {
                ContractResolver = new DocumentContractResolver()
            };

            DefaultCollection = GetCollectionAsync(settings.Container).Result;
        }
        
        public IEnumerable<string> GetCollections() {
            var collections = Client.CreateDocumentCollectionQuery(DatabaseUri);
            return collections.Select(each => each.Id).ToList().OrderBy(each => each).ToList();
        }
        
        public async Task<ResourceResponse<DocumentCollection>> CreateCollectionAsync(string collectionName) {
            collectionName = ResolveCollectionName(collectionName);
            return await Client.CreateDocumentCollectionIfNotExistsAsync(DatabaseUri, new DocumentCollection { Id =  collectionName }, new RequestOptions{ OfferThroughput = MinimumThroughput });
        }

        public async Task DeleteCollectionAsync(string collectionName) {
            var collection = await GetCollectionAsync(collectionName);
            await collection.DeleteAsync();
        }

        public async Task<AzureDocumentCollection> GetCollectionAsync(string collectionName) {
            collectionName = ResolveCollectionName(collectionName);

            await Semaphore.WaitAsync();

            try {
                if(!Collections.ContainsKey(collectionName)) {
                    var result = await CreateCollectionAsync(collectionName);
                    Collections.Add(collectionName, new AzureDocumentCollection(Client, DatabaseName, result.Resource, SerializerSettings));
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

        public CosmosDbSettings Settings { get; set; }

        public AzureDocumentCollection DefaultCollection { get; set; }
        
        private JsonSerializerSettings SerializerSettings { get; set; }

        private string DatabaseName { get; set; }

        private Uri DatabaseUri { get; set; }

        private DocumentClient Client { get; set; }

        private static readonly Dictionary<string, AzureDocumentCollection> Collections = new Dictionary<string, AzureDocumentCollection>();

        private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);

        private const int MinimumThroughput = 400;
    }
}