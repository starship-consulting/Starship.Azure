using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Starship.Azure.Interfaces;
using Starship.Data.OData;

namespace Starship.Azure.Providers.Tables {
    public class AzureTableStorageProvider : IsDataProvider {

        public AzureTableStorageProvider(CloudStorageAccount account) {
            Client = account.CreateCloudTableClient();
            Tables = new Dictionary<Type, AzureTableReference>();
        }

        public IQueryable<T> Get<T>() {
            return new ODataClientContext<T>(GetOrCreate<T>());
        }

        public T Add<T>(T entity) {
            GetOrCreate<T>().Add(AzureTableProvider.MakeTableEntity(entity));
            return entity;
        }

        public void Save() {
            foreach (var table in Tables) {
                table.Value.Save();
            }
        }

        private AzureTableReference GetOrCreate<T>() {
            if (!Tables.ContainsKey(typeof(T))) {
                var table = Client.GetTableReference(typeof(T).Name.ToLower());
                table.CreateIfNotExists();
                Tables.Add(typeof(T), new AzureTableReference(table));
            }

            return Tables[typeof(T)];
        }
        
        private Dictionary<Type, AzureTableReference> Tables { get; set; }

        private CloudTableClient Client { get; set; }
    }
}