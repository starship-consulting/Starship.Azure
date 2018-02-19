using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace Starship.Azure.Providers {
    public static class AzureDocumentDbProvider {

        static AzureDocumentDbProvider() {
            Databases = new ConcurrentDictionary<string, AzureDocumentDatabaseProvider>();
            //Client = new DocumentClient(new Uri(TopiaSettings.DocumentDbUrl), TopiaSettings.DocumentDbPrimaryKey);
            //Databases = new ConcurrentDictionary<string, AzureDocumentDatabaseProvider>();
        }

        public static AzureDocumentCollectionProvider GetCollection(string documentDbUrl, string documentDbPrimaryKey, string databaseName, string collectionName) {
            var client = new DocumentClient(new Uri(documentDbUrl), documentDbPrimaryKey);
            var database = client.CreateDatabaseQuery().Where(each => each.Id == databaseName).AsEnumerable().FirstOrDefault();
            var collection = client.CreateDocumentCollectionQuery(database.SelfLink)
                    .Where(d => d.Id == collectionName)
                    .AsEnumerable()
                    .FirstOrDefault();

            return new AzureDocumentCollectionProvider(collection, client);
        }

        public static AzureDocumentCollectionProvider GetCollection(string databaseName, string collectionName) {
            return GetDatabase(databaseName).GetCollection(collectionName);
        }

        public static List<Database> GetDatabases() {
            return Client.CreateDatabaseQuery().ToList();
        }

        public static AzureDocumentDatabaseProvider GetDatabase(string databaseName) {
            return Databases.GetOrAdd(databaseName, name => {

                var database = Client.CreateDatabaseQuery()
                    .Where(d => d.Id == name)
                    .AsEnumerable()
                    .FirstOrDefault();

                return new AzureDocumentDatabaseProvider(database, Client);
            });
        }

        private static DocumentClient Client { get; set; }

        private static ConcurrentDictionary<string, AzureDocumentDatabaseProvider> Databases { get; set; }
    }
}