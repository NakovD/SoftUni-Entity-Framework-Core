using System.Text;
using System.Xml.Serialization;

namespace ProductShop.Helpers
{
    public class XMLHelper
    {
        public T Deserialize<T>(string inputXML, string rootName)
        {
            var root = new XmlRootAttribute(rootName);
            var serializer = new XmlSerializer(typeof(T), root);

            using var reader = new StringReader(inputXML);

            var deserializedData = (T)serializer.Deserialize(reader)!;

            return deserializedData;
        }

        public string Serialize<T>(T data, string rootName)
        {
            var sb = new StringBuilder();

            var root = new XmlRootAttribute(rootName);

            var serializer = new XmlSerializer(typeof (T), root);

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();

            namespaces.Add(string.Empty, string.Empty);

            using var writer = new StringWriter(sb);

            serializer.Serialize(writer, data, namespaces);

            return sb.ToString().Trim();
        }
    }
}
