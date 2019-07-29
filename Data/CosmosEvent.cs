using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Starship.Azure.Data {

    public class CosmosEvent : CosmosDocument {

        [JsonProperty(PropertyName="name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName="source")]
        public CosmosEventSource Source { get; set; }
        
        [JsonProperty(PropertyName="parameters")]
        public CosmosEventParameters Parameters { get; set; }
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