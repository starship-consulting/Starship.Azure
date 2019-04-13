using System;
using Newtonsoft.Json;
using Starship.Data.AccessControl;

namespace Starship.Azure.Data {
    public class PermissionEntity : CosmosDocument {

        [JsonProperty(PropertyName="issuer")]
        public string Issuer {
            get => GetPropertyValue<string>("issuer");
            set => SetPropertyValue("issuer", value);
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