using System.Linq;
using Microsoft.WindowsAzure.Storage.Table;

namespace Starship.Azure.Extensions {
    public static class DynamicTableEntityExtensions {

        public static void Apply(this DynamicTableEntity instance, object source, params string[] skipProperties) {
            foreach (var property in source.GetType().GetProperties()) {
                if (skipProperties.Contains(property.Name)) {
                    continue;
                }

                instance.Properties.Add(property.Name, EntityProperty.CreateEntityPropertyFromObject(property.GetValue(source, null)));
            }
        }
    }
}