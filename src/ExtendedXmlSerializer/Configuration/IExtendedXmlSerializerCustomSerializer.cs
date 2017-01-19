using System.Xml;
using System.Xml.Linq;

namespace ExtendedXmlSerialization.Configuration
{
    public interface IExtendedXmlCustomSerializer<T>
    {
        T Deserialize(XElement xElement);

        void Serializer(XmlWriter xmlWriter, T obj);
    }
}