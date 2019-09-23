using Newtonsoft.Json;
using Starship.Data.Entities;

namespace Starship.Azure.Data {

    public class WebHookSubscription : DocumentEntity {
        
        [JsonProperty("apiKey")]
        public string ApiKey {
            get => Get<string>("apiKey");
            set => Set("apiKey", value);
        }

        [JsonProperty("source")]
        public string Source {
            get => Get<string>("source");
            set => Set("source", value);
        }

        [JsonProperty("url")]
        public string Url {
            get => Get<string>("url");
            set => Set("url", value);
        }
    }
}