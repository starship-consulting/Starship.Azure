using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace Starship.Azure.Providers {
    public class AzureDocumentCollectionProvider {
        public AzureDocumentCollectionProvider(DocumentCollection collection, DocumentClient client) {
            Collection = collection;
            Client = client;
        }

        public IOrderedQueryable<T> Query<T>() {
            return Client.CreateDocumentQuery<T>(Collection.DocumentsLink);
        }

        public async Task<T> ExecuteStoredProcedure<T>(string procedureName, params dynamic[] args) {
            var procedure = await GetStoredProcedure(procedureName);
            var result = await Client.ExecuteStoredProcedureAsync<T>(procedure.SelfLink, args);
            return result.Response;
        }

        public async Task<ResourceResponse<Document>> Create(object document) {
            return await Client.CreateDocumentAsync(Collection.DocumentsLink, document);
        }

        public async Task<ResourceResponse<Document>> Create(Dictionary<string, object> document) {
            return await Client.CreateDocumentAsync(Collection.DocumentsLink, document);
        }

        public List<Document> GetDocuments() {
            return Client.CreateDocumentQuery(Collection.DocumentsLink).ToList();
        }

        public async Task<FeedResponse<StoredProcedure>> GetStoredProcedures() {
            return await Client.ReadStoredProcedureFeedAsync(Collection.StoredProceduresLink);
        }

        public async Task<StoredProcedure> GetStoredProcedure(string id) {
            var procedures = await Client.ReadStoredProcedureFeedAsync(Collection.StoredProceduresLink);
            return procedures.FirstOrDefault(each => each.Id == id);
        }

        public async Task<StoredProcedure> CreateOrUpdateStoredProcedure(string id, string code) {

            var procedure = await GetStoredProcedure(id);
            ResourceResponse<StoredProcedure> task;

            if (procedure == null) {

                procedure = new StoredProcedure {
                    Id = id,
                    Body = code
                };

                task = await Client.CreateStoredProcedureAsync(Collection.SelfLink, procedure);
            }
            else {
                procedure.Body = code;

                task = await Client.ReplaceStoredProcedureAsync(procedure);
            }

            return task.Resource;
        }

        private DocumentCollection Collection { get; set; }

        private DocumentClient Client { get; set; }
    }
}