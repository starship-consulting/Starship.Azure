using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using Starship.Azure.Json;
using Starship.Core.Extensions;
using Starship.Data.Configuration;

namespace Starship.Azure.Providers.Cosmos {
    public class AzureCosmosDbProvider {
        
        public AzureCosmosDbProvider(DataSettings settings) {

            Settings = settings;
            ForceCamelcase = true;
            
            Client = new CosmosClient(settings.Uri, settings.Key, new CosmosClientOptions {
                ConnectionMode = ConnectionMode.Direct
            });
            
            Database = Client.GetDatabase(settings.Database);

            SerializerSettings = new JsonSerializerSettings {
                ContractResolver = new DocumentContractResolver()
            };

            DefaultCollection = GetCollectionAsync(settings.Container).Result;
        }

        /*public async Task<List<Document>> GetLog(DateTime startDate) {

            var collection = await GetCollectionAsync(Settings.Container);

            var uri = collection.GetCollectionUri();

            var feed = await Client.ReadPartitionKeyRangeFeedAsync(uri, new FeedOptions {
                RequestContinuation = string.Empty,
                EnableCrossPartitionQuery = true,
                MaxItemCount = -1
            });
            
            var changeFeedQuery = Client.CreateDocumentChangeFeedQuery(uri, new ChangeFeedOptions {
                StartTime = startDate,
                PartitionKeyRangeId = feed.First().Id
            });

            var documents = new List<Document>();

            while (changeFeedQuery.HasMoreResults) {
                var response = await changeFeedQuery.ExecuteNextAsync<Document>();
                documents.AddRange(response);
            }

            return documents;
        }*/
        
        /*public IEnumerable<string> GetCollections() {
            var collections = Client.CreateDocumentCollectionQuery(DatabaseUri);
            return collections.Select(each => each.Id).ToList().OrderBy(each => each).ToList();
        }*/
        
        public async Task<ContainerResponse> CreateCollectionAsync(string collectionName) {
            collectionName = ResolveCollectionName(collectionName);
            return await Database.CreateContainerIfNotExistsAsync(collectionName, DefaultPartitionKeyPath, MinimumThroughput);
        }

        public async Task DeleteCollectionAsync(string collectionName) {
            var collection = await GetCollectionAsync(collectionName);
            await collection.DeleteAsync();
        }

        public async Task<AzureCosmosCollection> GetCollectionAsync(string containerName) {
            containerName = ResolveCollectionName(containerName);

            await Semaphore.WaitAsync();

            try {
                if(!Collections.ContainsKey(containerName)) {
                    var result = await CreateCollectionAsync(containerName);
                    Collections.Add(containerName, new AzureCosmosCollection(result.Container, Settings));
                }

                return Collections[containerName];
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

        public DataSettings Settings { get; set; }

        public AzureCosmosCollection DefaultCollection { get; set; }

        private Database Database { get; set; }

        private JsonSerializerSettings SerializerSettings { get; set; }
        
        private static readonly Dictionary<string, AzureCosmosCollection> Collections = new Dictionary<string, AzureCosmosCollection>();

        private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);

        private const string DefaultPartitionKeyPath = "/_partitionKey";

        private const int MinimumThroughput = 400;

        private static CosmosClient Client { get; set; }
    }
}