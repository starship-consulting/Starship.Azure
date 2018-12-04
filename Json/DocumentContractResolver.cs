using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Starship.Azure.Json {
    public class DocumentContractResolver : CamelCasePropertyNamesContractResolver {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization) {
            var properties = base.CreateProperties(type, memberSerialization);
            return properties.Where(each => !each.PropertyName.StartsWith("_")).ToList();
        }
    }
}