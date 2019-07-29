using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Starship.Core.Data;
using Starship.Core.Extensions;
using Starship.Core.Security;
using Starship.Data.Configuration;

namespace Starship.Azure.Data {
    public class CosmosDocument : CosmosResource {
        
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
            get => Get<string>("owner");
            set => Set("owner", value);
        }
        
        [JsonProperty(PropertyName="validUntil")]
        public DateTime? ValidUntil {
            get => Get<DateTime?>("validUntil");
            set => Set("validUntil", value);
        }

        [Secure, JsonProperty(PropertyName="updatedBy")]
        public string UpdatedBy {
            get => Get<string>("updatedBy");
            set => Set("updatedBy", value);
        }

        [Secure, JsonProperty(PropertyName="updatedDate")]
        public DateTime? UpdatedDate => Get<long>("_ts").FromUnixTimestamp();
        
        [Secure, JsonProperty(PropertyName="creationDate")]
        public DateTime CreationDate {
            get => Get<DateTime>("creationDate");
            set => Set("creationDate", value);
        }
        
        [Secure, JsonProperty(PropertyName="externalId")]
        public string ExternalId {
            get => Get<string>("externalId");
            set => Set("externalId", value);
        }
        
        [Secure, JsonProperty(PropertyName="importDate")]
        public DateTime? ImportDate {
            get => Get<DateTime?>("importDate");
            set => Set("importDate", value);
        }

        [JsonProperty(PropertyName="participants")]
        public List<EntityParticipant> Participants {
            get => Get<List<EntityParticipant>>("participants");
            set => Set("participants", value);
        }

        [JsonProperty(PropertyName="permissions")]
        public List<CosmosPermission> Permissions {
            get => Get<List<CosmosPermission>>("permissions");
            set => Set("permissions", value);
        }
    }
}