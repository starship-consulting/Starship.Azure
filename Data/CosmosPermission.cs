using System;
using Newtonsoft.Json;

namespace Starship.Azure.Data {
    public class CosmosPermission {
        
        [JsonProperty(PropertyName="subject")]
        public string Subject { get; set; }
        
        [JsonProperty(PropertyName="actions")]
        public string[] Actions { get; set; }
    }
}