using System;
using Newtonsoft.Json;

namespace Starship.Azure.Data {
    public class UserEntity : CosmosResource {
        
        [JsonProperty(PropertyName="email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName="firstName")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName="lastName")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName="photo")]
        public string Photo { get; set; }

        [JsonProperty(PropertyName="lastLogin")]
        public DateTime LastLogin { get; set; }

        [JsonProperty(PropertyName="subscriptionEndDate")]
        public DateTime SubscriptionEndDate { get; set; }

        [JsonProperty(PropertyName="signature")]
        public string Signature { get; set; }
    }
}