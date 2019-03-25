using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Starship.Azure.Data {
    public class Account : CosmosResource {
        
        public Account() {
            clientSettings = new Dictionary<string, string>();
        }

        public bool IsAdmin() {

            if(string.IsNullOrEmpty(Role)) {
                return false;
            }

            return Role.ToLower() == "admin";
        }
        
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

        [JsonProperty(PropertyName="isTrial")]
        public bool IsTrial { get; set; }

        [JsonProperty(PropertyName="subscriptionEndDate")]
        public DateTime SubscriptionEndDate { get; set; }

        [JsonProperty(PropertyName="signature")]
        public string Signature { get; set; }

        [JsonProperty(PropertyName="role")]
        public string Role { get; set; }
        
        public Dictionary<string, string> clientSettings { get; set; }
    }
}