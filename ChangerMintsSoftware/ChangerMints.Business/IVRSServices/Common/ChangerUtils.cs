using System;
using System.Linq;
using System.Reflection;

namespace ChangerMints.Business {
    public static class ChangerUtils {

        // Splitter from web.config
        //private static char _splitter = Char.Parse(System.Configuration.ConfigurationManager.AppSettings["Split"]);
        private static char _splitter = '|';
        //gets string list as parameter and concatenate each string with '|'
        public static string ResponseConcatenater<T>(T obj) {

            var properties = obj.GetType().GetProperties();

            string Response = String.Empty;
            foreach (var property in properties) {
                PropertyInfo propertyInfo = obj.GetType().GetProperty(property.Name);
                Response += _splitter + ((propertyInfo.GetValue(obj, null) == null) ? "" : propertyInfo.GetValue(obj, null).ToString());
            }
            return Response.TrimStart('|');

        }

        //Gets string as input and split it with '|' and stores in the inputsParameters string array
        public static T URLSplitter<T>(string QueryString, out bool success) where T : new() {

            T obj = new T();
            var inputParameters = QueryString.Split(_splitter);
            var properties = obj.GetType().GetProperties();

            if (inputParameters.Length != properties.Count()) {
                success = false;
                return obj;
            }

            try {
                foreach (var property in properties.Select((x, i) => new { Value = x, Index = i })) {
                    PropertyInfo propertyInfo = obj.GetType().GetProperty(property.Value.Name);
                    var objectValue = Convert.ChangeType(inputParameters[property.Index], (propertyInfo.PropertyType.IsGenericType && 
                        propertyInfo.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)) ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType));
                    propertyInfo.SetValue(obj, objectValue, null);
                }
                success = true;
                return obj;
            } catch (Exception ex) {
                success = false;
                return obj;
            }

        }
    }
}
