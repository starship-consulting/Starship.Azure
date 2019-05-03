using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using Starship.Core.Security;
using Starship.Data.Configuration;

namespace Starship.Azure.Data {
    public class CosmosDocument : Document {

        public bool IsSystemType() {
            return Type == "group" || Type == "account";
        }

        public bool HasParticipant(string id) {
            return Participants != null && Participants.Any(participant => participant.Id == id);
        }

        public bool IsPublicRecord() {
            return Owner == GlobalDataSettings.SystemOwnerName;
        }
        
        [JsonProperty(PropertyName="owner")]
        public string Owner {
            get => GetPropertyValue<string>("owner");
            set => SetPropertyValue("owner", value);
        }
        
        [JsonProperty(PropertyName="validUntil")]
        public DateTime? ValidUntil {
            get => GetPropertyValue<DateTime?>("validUntil");
            set => SetPropertyValue("validUntil", value);
        }
        
        [JsonProperty(PropertyName="$type")]
        public string Type {
            get => GetPropertyValue<string>("$type");
            set => SetPropertyValue("$type", value);
        }
        
        [Secure, JsonProperty(PropertyName="creationDate")]
        public DateTime CreationDate {
            get => GetPropertyValue<DateTime>("creationDate");
            set => SetPropertyValue("creationDate", value);
        }
        
        [Secure, JsonProperty(PropertyName="externalId")]
        public string ExternalId {
            get => GetPropertyValue<string>("externalId");
            set => SetPropertyValue("externalId", value);
        }
        
        [Secure, JsonProperty(PropertyName="importDate")]
        public DateTime? ImportDate {
            get => GetPropertyValue<DateTime?>("importDate");
            set => SetPropertyValue("importDate", value);
        }

        [Secure, JsonProperty(PropertyName="participants")]
        public List<EntityParticipant> Participants {
            get => GetPropertyValue<List<EntityParticipant>>("participants");
            set => SetPropertyValue("participants", value);
        }
    }
}