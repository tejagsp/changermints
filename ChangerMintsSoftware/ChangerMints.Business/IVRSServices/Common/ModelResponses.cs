using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ChangerMints.Business {
    public class ModelResponses<T, U> {

        public Func<U, bool> _predicate;
        public long ErrorNumber { get; private set; }
        public string Message{get; private set;}
 
        public ModelResponses(Func<U, bool> predicate, int errorNumber) {
            _predicate = predicate;
            ErrorNumber = errorNumber;
            Message = System.Configuration.ConfigurationManager.AppSettings[errorNumber.ToString()];
        }

        public bool IsValid(U model) {
            return _predicate(model);
        }

        public T FillErrorDTO<T>() where T : new() {
            T obj = new T();
            var properties = obj.GetType().BaseType.GetProperties();

            PropertyInfo propertyInfoNumber = obj.GetType().GetProperty(properties[0].Name);
            var objectValue = Convert.ChangeType(ErrorNumber, (propertyInfoNumber.PropertyType.IsGenericType &&
                        propertyInfoNumber.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)) ? Nullable.GetUnderlyingType(propertyInfoNumber.PropertyType) : propertyInfoNumber.PropertyType));
            propertyInfoNumber.SetValue(obj, objectValue, null);

            PropertyInfo propertyInfoMessage = obj.GetType().GetProperty(properties[1].Name);

            var objectMessage = Convert.ChangeType(Message, (propertyInfoMessage.PropertyType.IsGenericType &&
                        propertyInfoMessage.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)) ? Nullable.GetUnderlyingType(propertyInfoMessage.PropertyType) : propertyInfoMessage.PropertyType));
            propertyInfoMessage.SetValue(obj, objectMessage, null);

            return obj;
        }
    }
}
