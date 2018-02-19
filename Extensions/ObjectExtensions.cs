using System;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using Starship.Core.Extensions;

namespace Starship.Azure.Extensions {
    public static class ObjectExtensions {
        public static T ShallowClone<T>(this T source) where T : class {
            var instance = Activator.CreateInstance(source.GetType()) as T;

            // Perform a shallow clone to prevent cross-thread object reference tampering
            foreach (var property in source.GetType().GetProperties()) {
                if (property.GetSetMethod(true) == null) {
                    continue;
                }

                // Filter all navigation properties
                if (property.GetCustomAttributes(typeof(ForeignKeyAttribute), true).Any() &&
                    !property.PropertyType.FullName.StartsWith("System.")) {
                    continue;
                }

                if (property.PropertyType.IsGenericType && property.PropertyType.GetEnumerableType() != null) {
                    continue;
                }

                var value = property.GetValue(source);
                property.SetValue(instance, value);
            }

            return instance;
        }
    }
}