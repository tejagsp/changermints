using System;
using System.IO;
using System.Xml.Serialization;
using System.Web;
using System.Xml.Linq;
using System.Xml;
using System.Text;

namespace ChangerMints {

    internal static class XmlActionResult {
        public static XDocument CreateXMLdoc(int ErrorNumber) {

            XDocument doc = new XDocument(
                new XDeclaration("1.0", "UTF-8", string.Empty),
                new XElement("CustomerRegistration",
                        new XElement("ResponseCode", ErrorNumber)
                        ));
            return doc;
        }
    }
}
