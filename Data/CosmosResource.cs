﻿using System;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using Starship.Core.Security;
using Starship.Data.Configuration;

namespace Starship.Azure.Data {
    /*public class CosmosResource : Resource {

        public bool IsSystemData() {
            return Owner == GlobalDataSettings.SystemOwnerName;
        }

        [JsonProperty(PropertyName="owner")]
        public string Owner { get; set; }

        [JsonProperty(PropertyName="validUntil")]
        public DateTime? ValidUntil { get; set; }

        [JsonProperty(PropertyName = "$type")]
        public string Type { get; set; }

        [Secure, JsonProperty(PropertyName="creationDate")]
        public DateTime? CreationDate { get; set; }

        [Secure, JsonProperty(PropertyName="externalId")]
        public string ExternalId { get; set; }

        [Secure, JsonProperty(PropertyName="importDate")]
        public DateTime? ImportDate { get; set; }
    }*/
}