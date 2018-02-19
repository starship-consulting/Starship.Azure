using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Starship.Core.Extensions;

namespace Starship.Azure.Converters {
    public class DynamicTableEntityConverter : JsonConverter {
        public static DynamicTableEntityConverter Instance = new DynamicTableEntityConverter();

        public DynamicTableEntityConverter(bool includeStorageProperties = true) {
            IncludeStorageProperties = includeStorageProperties;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            writer.WriteStartObject();

            WriteObjectProperties(writer, (DynamicTableEntity) value);

            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType) {
            return typeof (DynamicTableEntity).IsAssignableFrom(objectType);
        }

        protected virtual void WriteObjectProperties(JsonWriter writer, DynamicTableEntity entity) {

            if (IncludeStorageProperties) {
                WritePropertyNameValue(writer, "RowKey", entity.RowKey);
                WritePropertyNameValue(writer, "PartitionKey", entity.PartitionKey);
                WritePropertyNameValue(writer, "Timestamp", entity.Timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ"));
                WritePropertyNameValue(writer, "ETag", entity.ETag);
            }

            foreach (var property in entity.Properties) {
                WriteProperty(writer, property);
            }
        }

        private bool IsJsonStructure(string value) {
            if (value.StartsWith("{") && value.EndsWith("}")) {
                return true;
            }

            if (value.StartsWith("[") && value.EndsWith("]")) {
                return true;
            }

            return false;
        }

        protected virtual void WriteProperty(JsonWriter writer, KeyValuePair<string, EntityProperty> property) {
            switch (property.Value.PropertyType) {
                case EdmType.String:

                    if (property.Value.StringValue.IsEmpty()) {
                        WritePropertyNameValue(writer, property.Key, string.Empty);
                        return;
                    }

                    /*if (IsJsonStructure(property.Value.StringValue)) {
                        var value = JsonConvert.DeserializeObject(property.Value.StringValue);
                        var json = JsonConvert.SerializeObject(value);

                        writer.WritePropertyName(property.Key);
                        writer.WriteRawValue(json);
                        return;
                    }*/

                    WritePropertyNameValue(writer, property.Key, property.Value.StringValue);
                    break;
                case EdmType.Boolean:
                    WritePropertyNameValue(writer, property.Key, property.Value.BooleanValue);
                    break;
                case EdmType.DateTime:
                    WritePropertyNameValue(writer, property.Key, property.Value.DateTimeOffsetValue);
                    break;
                case EdmType.Double:
                    WritePropertyNameValue(writer, property.Key, property.Value.DoubleValue);
                    break;
                case EdmType.Guid:
                    WritePropertyNameValue(writer, property.Key, property.Value.GuidValue);
                    break;
                case EdmType.Int32:
                    WritePropertyNameValue(writer, property.Key, property.Value.Int32Value);
                    break;
                case EdmType.Int64:
                    WritePropertyNameValue(writer, property.Key, property.Value.Int64Value);
                    break;
                case EdmType.Binary:
                    WritePropertyNameValue(writer, property.Key, property.Value.BinaryValue);
                    break;
            }
        }

        private void WritePropertyNameValue<TValue>(JsonWriter writer, string key, TValue value)
            where TValue : class {
            // exclude null properties since objects may have different schemas
            if (value != null) {
                writer.WritePropertyName(key);
                writer.WriteValue(value);
            }
        }

        // forced into duplication by the type system...
        private void WritePropertyNameValue<TValue>(JsonWriter writer, string key, TValue? value)
            where TValue : struct {
            // exclude null properties since objects may have different schemas
            if (value != null) {
                writer.WritePropertyName(key);
                writer.WriteValue(value);
            }
        }

        public bool IncludeStorageProperties { get; set; }
    }
}