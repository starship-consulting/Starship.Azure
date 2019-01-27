using System;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;

namespace Starship.Azure.Data {
    public class CosmosDocument : Document {
        
        [JsonProperty(PropertyName="owner")]
        public string Owner { get; set; }

        [JsonProperty(PropertyName="validUntil")]
        public DateTime? ValidUntil { get; set; }

        [JsonProperty(PropertyName = "$type")]
        public string Type { get; set; }
    }
}