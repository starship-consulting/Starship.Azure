using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Starship.Core.Security;

namespace Starship.Azure.Data {
    public class Account : CosmosDocument {
        
        public bool IsAdmin() {

            if(string.IsNullOrEmpty(Role)) {
                return false;
            }

            return Role.ToLower() == "admin";
        }
        
        public PermissionTypes GetPermission(CosmosDocument entity) {
            
            if(entity == null) {
                return PermissionTypes.Full;
            }

            var permission = PermissionTypes.Full;

            if(entity.Type == "account") {
                permission = PermissionTypes.Partial;
            }

            if(string.IsNullOrEmpty(entity.Id) || string.IsNullOrEmpty(entity.Owner)) {
                return PermissionTypes.Full;
            }
            
            if(IsAdmin() || entity.Id == Id || entity.Owner == Id || entity.IsSystemData()) {
                return permission;
            }
            
            return PermissionTypes.None;
        }
        
        [Secure, JsonProperty(PropertyName="email")]
        public string Email {
            get => GetPropertyValue<string>("email");
            set => SetPropertyValue("email", value);
        }

        [Secure, JsonProperty(PropertyName="chargeBeeId")]
        public string ChargeBeeId {
            get => GetPropertyValue<string>("chargeBeeId");
            set => SetPropertyValue("chargeBeeId", value);
        }

        [Secure, JsonProperty(PropertyName="outboundEmail")]
        public string OutboundEmail {
            get => GetPropertyValue<string>("outboundEmail");
            set => SetPropertyValue("outboundEmail", value);
        }

        [Secure, JsonProperty(PropertyName="outboundEmailId")]
        public int OutboundEmailId {
            get => GetPropertyValue<int>("outboundEmailId");
            set => SetPropertyValue("outboundEmailId", value);
        }

        [JsonProperty(PropertyName="outboundEmailBCC")]
        public bool OutboundEmailBCC {
            get => GetPropertyValue<bool>("outboundEmailBCC");
            set => SetPropertyValue("outboundEmailBCC", value);
        }

        [JsonProperty(PropertyName="firstName")]
        public string FirstName {
            get => GetPropertyValue<string>("firstName");
            set => SetPropertyValue("firstName", value);
        }

        [JsonProperty(PropertyName="lastName")]
        public string LastName {
            get => GetPropertyValue<string>("lastName");
            set => SetPropertyValue("lastName", value);
        }

        [JsonProperty(PropertyName="photo")]
        public string Photo {
            get => GetPropertyValue<string>("photo");
            set => SetPropertyValue("photo", value);
        }

        [Secure, JsonProperty(PropertyName="lastLogin")]
        public DateTime LastLogin {
            get => GetPropertyValue<DateTime>("lastLogin");
            set => SetPropertyValue("lastLogin", value);
        }

        [Secure, JsonProperty(PropertyName="isTrial")]
        public bool IsTrial {
            get => GetPropertyValue<bool>("isTrial");
            set => SetPropertyValue("isTrial", value);
        }

        [Secure, JsonProperty(PropertyName="subscriptionEndDate")]
        public DateTime SubscriptionEndDate {
            get => GetPropertyValue<DateTime>("subscriptionEndDate");
            set => SetPropertyValue("subscriptionEndDate", value);
        }

        [Secure, JsonProperty(PropertyName="signature")]
        public string Signature {
            get => GetPropertyValue<string>("signature");
            set => SetPropertyValue("signature", value);
        }

        [Secure, JsonProperty(PropertyName="role")]
        public string Role {
            get => GetPropertyValue<string>("role");
            set => SetPropertyValue("role", value);
        }
    }
}