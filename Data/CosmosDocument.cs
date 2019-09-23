using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Cosmos.Linq;
using Newtonsoft.Json;
using Starship.Core.Extensions;
using Starship.Core.Security;
using Starship.Data.Configuration;
using Starship.Data.Entities;

namespace Starship.Azure.Data {

    public class CosmosDocument : DocumentEntity {
        
        public CosmosDocument() {
            CreationDate = DateTime.UtcNow;
        }

        public bool IsSystemType() {
            return Type == "account";
        }

        public bool IsPublicRecord() {
            return Owner == GlobalDataSettings.SystemOwnerName;
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
        
        [JsonProperty(PropertyName="creationDate")]
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

        [JsonProperty(PropertyName="permissions")]
        public List<CosmosPermission> Permissions {
            get => Get<List<CosmosPermission>>("permissions");
            set => Set("permissions", value);
        }
    }

    public static class CosmosDocumentExtensions {

        public static IQueryable<T> IsValid<T>(this IQueryable<T> queryable, DateTime? asOfDate = null) where T : CosmosDocument {

            var date = DateTime.UtcNow;

            if(asOfDate != null) {
                date = asOfDate.Value;
            }

            return queryable.Where(each => !each.ValidUntil.IsDefined() || each.ValidUntil == null || each.ValidUntil > date);
        }
    }
}