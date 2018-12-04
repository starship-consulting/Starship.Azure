using System;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace Starship.Azure.Providers.Cosmos {

    public class AzureDocumentCollection {

        public AzureDocumentCollection(string databaseName, DocumentCollection collection) {
            DatabaseName = databaseName;
            Collection = collection;
        }

        public Uri GetDocumentUri(string id = "") {
            return UriFactory.CreateDocumentUri(DatabaseName, Collection.Id, id);
        }

        public Uri GetCollectionUri() {
            return UriFactory.CreateDocumentCollectionUri(DatabaseName, Collection.Id);
        }

        public DocumentCollection Collection { get; set; }

        public string DatabaseName { get; set; }
    }
}