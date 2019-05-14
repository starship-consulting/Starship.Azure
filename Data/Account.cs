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

            if(entity.Owner == Id || shares.Contains(entity.Owner) || shares.Contains(entity.Id)) {
                return true;
            }

            if(entity.Type == "account") {
                return entity.GetPropertyValue<List<string>>("groups").Any(group => groups.Contains(group));
            }

            return false;
        }

        public bool HasRight(CosmosDocument entity, AccessRight right) {

            var rights = GetRights(entity).ToList();

            if(rights.Contains(AccessRight.Full)) {
                return true;
            }

            if(right == AccessRight.Read && (rights.Contains(AccessRight.Update) || rights.Contains(AccessRight.Delete))) {
                return true;
            }

            return rights.Contains(right);
        }
        
        public IEnumerable<AccessRight> GetRights(CosmosDocument entity) {
            
            var rights = new List<AccessRight>();

            if(entity == null) {
                rights.Add(AccessRight.Full);
                return rights;
            }

            return GetClaims()
                .Where(each => each.Type == "*" || each.Type == entity.Type)
                .Where(each => each.Scope == "*" || each.Scope == entity.Owner || each.Scope == entity.Id)
                .SelectMany(each => each.Rights)
                .Select(each => (AccessRight) Enum.Parse(typeof(AccessRight), each));

            /*var permission = PermissionTypes.Full;

            if(entity.Type == "account") {
                permission = PermissionTypes.Partial;
            }

            if(string.IsNullOrEmpty(entity.Id) || string.IsNullOrEmpty(entity.Owner)) {
                return PermissionTypes.Full;
            }

            var claims = GetClaims();
            
            if(IsAdmin() || entity.Id == Id || claims.Contains(entity.Owner)) {
                return permission;
            }
            
            return PermissionTypes.None;*/
        }

        public bool HasClaim(string type, string scope, string right) {

            if(IsAdmin()) {
                return true;
            }

            return false;
            //return GetClaims().Contains(type);
        }

        private List<AccessClaim> GetDefaultClaims() {

            return new List<AccessClaim> {
                new AccessClaim("*", Id, "*"),
                new AccessClaim("*", GlobalDataSettings.SystemOwnerName, "read")
                //new AccessClaim("*", $"owner = '{Id}'", "*"),
                //new AccessClaim("*", $"owner = '{GlobalDataSettings.SystemOwnerName}'", "read")
            };
        }
        
        public List<AccessClaim> GetClaims() {

            var claims = GetDefaultClaims();

            if(Claims != null && Claims.Any()) {
                claims.AddRange(Claims.ToList());
            }

            return claims;
        }
        
        public string GetName() {
            return FirstName + " " + LastName;
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

        [Secure, JsonProperty(PropertyName="claims")]
        public List<AccessClaim> Claims {
            get => GetPropertyValue<List<AccessClaim>>("claims");
            set => SetPropertyValue("claims", value);
        }

        [Secure, JsonProperty(PropertyName="groups")]
        public List<string> Groups {
            get => GetPropertyValue<List<string>>("groups");
            private set => SetPropertyValue("groups", value);
        }
    }
}