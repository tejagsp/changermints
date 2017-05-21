using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;

namespace ChangerMints.Business {

    public static class ChangerUtils {

        // Splitter from web.config
        //private static char _splitter = Char.Parse(System.Configuration.ConfigurationManager.AppSettings["Split"]);
        private static char _splitter = '|';
        private static char _responseStart = '$';
        private static char _responseEnd = '#';

        // Create an XML response for the IVR response acknowledgment
        public static string CreateXMLDoc(int ErrorNumber) {
            //string XMLInfo = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
            string rootStartingTag = "<CustomerRegistration>";
            string errorNumber = "<ResponseCode>" + ErrorNumber + "</ResponseCode>";
            string rootendTag = "</CustomerRegistration>";
            string XML = rootStartingTag + System.Environment.NewLine + errorNumber + System.Environment.NewLine + rootendTag;
            return XML;
        }

        // Create an XML response for the IVR call log acknowledgment
        public static string CreateXMLDoc(string status) {
            string rootStartingTag = "<CustomerRegistration>";
            string errorMessage = "<ResponseCode>" + status + "</ResponseCode>";
            string rootendTag = "</CustomerRegistration>";
            string XML = rootStartingTag + System.Environment.NewLine + errorMessage + System.Environment.NewLine + rootendTag;
            return XML;
        }

        //gets string list as parameter and concatenate each string with '|'
        public static string ResponseConcatenater<T>(T obj, bool endTags = false) {
            var properties = obj.GetType().GetProperties().Reverse();

            string Response = String.Empty;
            foreach (var property in properties) {
                PropertyInfo propertyInfo = obj.GetType().GetProperty(property.Name);
                Response += _splitter + ((propertyInfo.GetValue(obj, null) == null) ? "" : propertyInfo.GetValue(obj, null).ToString());
            }
            return endTags ? _responseStart + Response.TrimStart('|') + _responseEnd : Response.TrimStart('|');
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

        public static void CreateGetRequest(string parameters) {
            string sURL = "https://txtguru.in/imobile/api.php?" + parameters;

            WebRequest wrGETURL;
            wrGETURL = WebRequest.Create(sURL);

            WebProxy myProxy = new WebProxy("myproxy", 80);
            myProxy.BypassProxyOnLocal = true;

            wrGETURL.Proxy = WebRequest.GetSystemWebProxy();

            Stream objStream;
            objStream = wrGETURL.GetResponse().GetResponseStream();

            StreamReader objReader = new StreamReader(objStream);

            string sLine = "";
            int i = 0;

            while (sLine != null) {
                i++;
                sLine = objReader.ReadLine();
                if (sLine != null)
                    Console.WriteLine("{0}:{1}", i, sLine);
            }
            Console.ReadLine();
        }
    }
}