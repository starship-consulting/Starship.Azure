using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Starship.Azure.Data {

    public class CosmosEvent : CosmosDocument {

        [JsonProperty(PropertyName="name")]
        public string Name {
            get => Get<string>("name");
            set => Set("name", value);
        }

        [JsonProperty(PropertyName="source")]
        public CosmosEventSource Source {
            get => Get<CosmosEventSource>("source");
            set => Set("source", value);
        }
        
        [JsonProperty(PropertyName="parameters")]
        public CosmosEventParameters Parameters {
            get => Get<CosmosEventParameters>("parameters");
            set => Set("parameters", value);
        }
    }

    public class CosmosEventParameters: Dictionary<string, object> {

        [JsonProperty(PropertyName="date")]
        public DateTime? Date { get; set; }
    }
    
    public struct CosmosEventSource {

        [JsonProperty(PropertyName="id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName="type")]
        public string Type { get; set; }
    }
}