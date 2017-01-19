using System.Xml;
using System.Xml.Linq;

namespace ExtendedXmlSerialization.Configuration
{
    public interface IExtendedXmlSerializerCustomSerializer<T>
    {
        T Deserialize(XElement xElement);

        void Serializer(XmlWriter xmlWriter, T obj);
    }
}