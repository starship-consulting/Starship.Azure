using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Starship.Core.Data;
using Starship.Core.Interfaces;
using Starship.Data.OData;

namespace Starship.Azure.Providers.Tables {
    public class AzureTableStorageProvider : IsDataProvider {

        public AzureTableStorageProvider(string connectionstring)
            : this(CloudStorageAccount.Parse(connectionstring)) {
        }

        public AzureTableStorageProvider(CloudStorageAccount account) {
            Client = account.CreateCloudTableClient();
            Tables = new Dictionary<Type, AzureTableReference>();
        }

        public IQueryable<T> Get<T>() {
            return new ODataClientContext<T>(GetOrCreate<T>());
        }

        public T Add<T>(T entity) where T : HasId {
            GetOrCreate<T>().Add(AzureTableProvider.MakeTableEntity(entity));
            return entity;
        }

        public void Save() {
            SaveAsync().Wait();
        }

        public async Task SaveAsync() {
            foreach (var table in Tables) {
                await table.Value.SaveAsync();
            }
        }

        private AzureTableReference GetOrCreate<T>() {
            if (!Tables.ContainsKey(typeof(T))) {
                var table = Client.GetTableReference(typeof(T).Name.ToLower());
                table.CreateIfNotExistsAsync().Wait();
                Tables.Add(typeof(T), new AzureTableReference(table));
            }

            return Tables[typeof(T)];
        }
        
        private Dictionary<Type, AzureTableReference> Tables { get; set; }

        private CloudTableClient Client { get; set; }
    }
}