using System;
using Newtonsoft.Json;
using Starship.Data.AccessControl;

namespace Starship.Azure.Data {
    public class PermissionEntity : CosmosResource {

        [JsonProperty(PropertyName="issuer")]
        public string Issuer { get; set; }

        [JsonProperty(PropertyName="status")]
        public int Status { get; set; }

        [JsonProperty(PropertyName="level")]
        public AccessControlLevels Level { get; set; }
    }
}