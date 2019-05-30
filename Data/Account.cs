using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Starship.Core.Extensions;
using Starship.Core.Security;
using Starship.Data.Configuration;

namespace Starship.Azure.Data {
    public class Account : CosmosDocument {
        
        public Account() {
            Type = "account";
        }

        public bool HasGroup(string id) {
            return GetGroups().Contains(id);
        }

        public List<string> GetGroups() {
            if(Groups == null) {
                return new List<string>();
            }

            return Groups.ToList();
        }

        public void RemoveGroup(string id) {
            if(HasGroup(id)) {
                var groups = GetGroups();
                groups.Remove(id);
                Groups = groups.ToList();
            }
        }

        public void AddGroup(string id) {
            var groups = GetGroups();
            groups.Add(id);
            Groups = groups.ToList();
        }
        
        public bool IsCoordinator() {

            if(string.IsNullOrEmpty(Role)) {
                return false;
            }

            return Role.ToLower() == "coordinator";
        }

        public bool IsManager() {

            if(string.IsNullOrEmpty(Role)) {
                return false;
            }

            return Role.ToLower() == "manager";
        }

        public bool IsAdmin() {

            if(string.IsNullOrEmpty(Role)) {
                return false;
            }

            return Role.ToLower() == "admin";
        }

        public bool CanDelete(CosmosDocument entity) {

            if(entity.IsSystemType() || entity.Owner == GlobalDataSettings.SystemOwnerName) {
                return false;
            }

            if(IsAdmin()) {
                return true;
            }
            
            return entity.Owner == Id;
        }

        public bool CanUpdate(CosmosDocument entity, List<string> shares) {

            if(entity.Type == "account" && Id == entity.Id) {
                return true;
            }

            if(IsAdmin()) {
                return true;
            }
            
            return CanRead(entity, shares);
        }

        public bool CanRead(CosmosDocument entity, List<string> shares) {

            if(IsAdmin()) {
                return true;
            }
            
            var groups = GetGroups();

            if(entity.Owner == Id || shares.Contains(entity.Owner) || shares.Contains(entity.Id) || entity.Participants.Any(participant => participant.Id == Id)) {
                return true;
            }
            
            if(entity.Type == "account") {
                return entity.GetPropertyValue<List<string>>("groups").Any(group => groups.Contains(group));
            }

            return false;
        }
        
        public string GetName() {

            if(LastName.Contains("@") && FirstName.Contains("@")) {
                return FirstName;
            }

            return FirstName + " " + LastName;
        }
        
        public Account UpdateComponent<T>(Action<T> action) where T : new() {
            var component = GetComponent<T>();
            action(component);
            SetComponent(component);
            return this;
        }

        public T GetComponent<T>() where T : new() {

            var components = GetPropertyValue<AccountComponents>("components");

            if(components == null) {
                return new T();
            }
            
            var key = GetComponentKey(typeof(T));

            if(components.ContainsKey(key)) {
                return components[key].Clone<T>();
            }
            
            return new T();
        }

        public void SetComponent<T>(T component) where T : new() {
            
            var components = GetPropertyValue<AccountComponents>("components");

            if(components == null) {
                components = new AccountComponents();
            }

            var key = GetComponentKey(typeof(T));

            if(!components.ContainsKey(key)) {
                components.Add(key, component);
            }
            else {
                components[key] = component;
            }

            SetPropertyValue("components", components);
        }

        private string GetComponentKey(Type type) {
            return type.Name.Replace("Component", "").CamelCase();
        }

        [Secure, JsonProperty(PropertyName="email")]
        public string Email {
            get => GetPropertyValue<string>("email");
            set => SetPropertyValue("email", value);
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

        [Secure, JsonProperty(PropertyName="photo")]
        public string Photo {
            get => GetPropertyValue<string>("photo");
            set => SetPropertyValue("photo", value);
        }

        [Secure, JsonProperty(PropertyName="lastLogin")]
        public DateTime? LastLogin {
            get => GetPropertyValue<DateTime?>("lastLogin");
            set => SetPropertyValue("lastLogin", value);
        }

        [JsonProperty(PropertyName="signature")]
        public string Signature {
            get => GetPropertyValue<string>("signature");
            set => SetPropertyValue("signature", value);
        }

        [Secure, JsonProperty(PropertyName="role")]
        public string Role {
            get => GetPropertyValue<string>("role");
            set => SetPropertyValue("role", value);
        }
        
        [Secure, JsonProperty(PropertyName="groups")]
        public List<string> Groups {
            get => GetPropertyValue<List<string>>("groups");
            private set => SetPropertyValue("groups", value);
        }

        [Secure, JsonProperty(PropertyName="policies")]
        public List<CosmosPolicy> Policies {
            get => GetPropertyValue<List<CosmosPolicy>>("policies");
            set => SetPropertyValue("policies", value);
        }
    }
}