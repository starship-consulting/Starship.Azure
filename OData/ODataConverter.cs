using System;

namespace Starship.Azure.OData {
    public static class ODataConverter {
        /*public static QueryableODataEndpoint<T> Get<T>() {
            return new QueryableODataEndpoint<T>();
        }*/

        public static string ConvertValue(object value) {
            if (value is DateTime) {
                var date = (DateTime) value;
                value = date.ToString("O");
            }

            return value.ToString();
        }

        public static string Format(Type type, object value) {
            value = ConvertValue(value);

            if (type == typeof(DateTime)) {
                value = "datetime'" + value + "'";
            }
            else if (type == typeof(string)) {
                value = "'" + value + "'";
            }

            return value.ToString();
        }
    }
}