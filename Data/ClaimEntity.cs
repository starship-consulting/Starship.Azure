using System;
using Newtonsoft.Json;
using Starship.Data.AccessControl;

namespace Starship.Azure.Data {
    public class ClaimEntity : CosmosDocument {

        public ClaimEntity() {
            Type = "claim";
        }

        public ClaimEntity(string ownerId, string name, string value) : this() {
            Owner = ownerId;
            Name = name;
            Value = value;
        }

        [JsonProperty(PropertyName="name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName="value")]
        public string Value { get; set; }

        [JsonProperty(PropertyName="status")]
        public int Status { get; set; }

        [JsonProperty(PropertyName="level")]
        public AccessControlLevels Level { get; set; }
    }
}