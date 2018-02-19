using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace Starship.Azure.Providers {
    public class AzureDocumentDatabaseProvider {

        public AzureDocumentDatabaseProvider(Database database, DocumentClient client) {
            Database = database;
            Client = client;
            Collections = new ConcurrentDictionary<string, AzureDocumentCollectionProvider>();
        }

        public List<DocumentCollection> GetCollections() {
            return Client.CreateDocumentCollectionQuery(Database.SelfLink).ToList();
        }

        public AzureDocumentCollectionProvider GetCollection(string collectionName) {
            return Collections.GetOrAdd(collectionName, name =>
            {
                var collection = Client.CreateDocumentCollectionQuery(Database.SelfLink)
                    .Where(d => d.Id == name)
                    .AsEnumerable()
                    .FirstOrDefault();

                return new AzureDocumentCollectionProvider(collection, Client);
            });
        }

        private Database Database { get; set; }

        private DocumentClient Client { get; set; }

        private ConcurrentDictionary<string, AzureDocumentCollectionProvider> Collections { get; set; }
    }
}