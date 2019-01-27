using Newtonsoft.Json;
using Starship.Core.Json;

namespace Starship.Azure.Providers.Cosmos {
    public class CosmosDbSettings {
        
        public CosmosDbSettings() {
            SerializerSettings = new JsonSerializerSettings {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            };

            SerializerSettings.Converters.Add(new ConfigurableDictionaryJsonConverter {
                UseCamelCase = true,
                SortAlphabetically = true,
                ShowEmptyCollections = false,
                IncludeUnderscoreProperties = false,
                IdPropertyAlwaysFirst = true,
                IncludeDefaultValues = false
            });
        }

        public string Uri => $"https://{Account}.documents.azure.com:443/";

        public string Key { get; set; }

        public string Account { get; set; }

        public string Database { get; set; }

        public string Container { get; set; }

        public string OwnerIdPropertyName { get; set; }

        public string SystemOwnerName { get; set; }

        public string TypePropertyName { get; set; }

        public string SoftDeletionPropertyName { get; set; }

        public string SaveProcedureName { get; set; }

        public JsonSerializerSettings SerializerSettings { get; set; }
    }
}