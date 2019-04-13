using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Starship.Azure.Data {

    public class CosmosEvent : CosmosDocument {

        [JsonProperty(PropertyName="name")]
        public string Name {
            get => GetPropertyValue<string>("name");
            set => SetPropertyValue("name", value);
        }

        [JsonProperty(PropertyName="source")]
        public CosmosEventSource Source {
            get => GetPropertyValue<CosmosEventSource>("source");
            set => SetPropertyValue("source", value);
        }

        [JsonProperty(PropertyName="parameters")]
        public Dictionary<string, object> Parameters {
            get => GetPropertyValue<Dictionary<string, object>>("parameters");
            set => SetPropertyValue("parameters", value);
        }

        public struct CosmosEventSource {

            [JsonProperty(PropertyName="id")]
            public string Id { get; set; }

            [JsonProperty(PropertyName="type")]
            public string Type { get; set; }
        }
    }
}