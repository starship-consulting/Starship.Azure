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
        public string Name {
            get => GetPropertyValue<string>("name");
            set => SetPropertyValue("name", value);
        }

        [JsonProperty(PropertyName="value")]
        public string Value {
            get => GetPropertyValue<string>("value");
            set => SetPropertyValue("value", value);
        }

        [JsonProperty(PropertyName="status")]
        public int Status {
            get => GetPropertyValue<int>("status");
            set => SetPropertyValue("status", value);
        }

        [JsonProperty(PropertyName="level")]
        public AccessControlLevels Level {
            get => GetPropertyValue<AccessControlLevels>("level");
            set => SetPropertyValue("level", value);
        }
    }
}