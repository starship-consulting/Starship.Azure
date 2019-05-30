using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using Starship.Core.Security;
using Starship.Data.Configuration;

namespace Starship.Azure.Data {
    public class CosmosDocument : Document {
        
        public CosmosDocument() {
            CreationDate = DateTime.UtcNow;
        }

        public bool IsSystemType() {
            return Type == "group" || Type == "account";
        }

        public bool IsPublicRecord() {
            return Owner == GlobalDataSettings.SystemOwnerName;
        }

        public bool HasParticipant(string id) {
            return GetParticipants().Any(participant => participant.Id == id);
        }

        public List<EntityParticipant> GetParticipants() {
            if(Participants == null) {
                return new List<EntityParticipant>();
            }

            return Participants.ToList();
        }

        public void RemoveParticipant(string key) {
            Participants = GetParticipants().Where(each => each.Id != key).ToList();
        }

        public void AddParticipant(string key, string value = "") {
            AddParticipant(new EntityParticipant(key, value));
        }

        public void AddParticipant(EntityParticipant participant) {
            var accountClaims = GetParticipants();
            accountClaims.Add(participant);
            Participants = accountClaims.ToList();
        }
        
        [Secure, JsonProperty(PropertyName="owner")]
        public string Owner {
            get => GetPropertyValue<string>("owner");
            set => SetPropertyValue("owner", value);
        }
        
        [JsonProperty(PropertyName="validUntil")]
        public DateTime? ValidUntil {
            get => GetPropertyValue<DateTime?>("validUntil");
            set => SetPropertyValue("validUntil", value);
        }

        [Secure, JsonProperty(PropertyName="updatedBy")]
        public string UpdatedBy {
            get => GetPropertyValue<string>("updatedBy");
            set => SetPropertyValue("updatedBy", value);
        }

        [Secure, JsonProperty(PropertyName="updatedDate")]
        public DateTime? UpdatedDate => Timestamp;

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

        [JsonProperty(PropertyName="participants")]
        public List<EntityParticipant> Participants {
            get => GetPropertyValue<List<EntityParticipant>>("participants");
            private set => SetPropertyValue("participants", value);
        }

        [JsonProperty(PropertyName="permissions")]
        public List<CosmosPermission> Permissions {
            get => GetPropertyValue<List<CosmosPermission>>("permissions");
            set => SetPropertyValue("permissions", value);
        }
    }
}