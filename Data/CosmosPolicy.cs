using System;
using Newtonsoft.Json;

namespace Starship.Azure.Data
{
    public class CosmosPolicy {
        
        [JsonProperty(PropertyName="subject")]
        public string Subject { get; set; }
        
        [JsonProperty(PropertyName="actions")]
        public string[] Actions { get; set; }
    }
}
