using ExtendedXmlSerializer.ExtensionModel.Xml;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using XmlWriter = System.Xml.XmlWriter;

namespace ExtendedXmlSerializer.Tests.Support
{
	sealed class ClassicSerialization<T>
	{
		public static ClassicSerialization<T> Default { get; } = new ClassicSerialization<T>();

		ClassicSerialization() : this(new XmlSerializer(typeof(T)), new XmlReaderFactory()) {}

		readonly XmlSerializer     _serializer;
		readonly IXmlReaderFactory _factory;

		public ClassicSerialization(XmlSerializer serializer, IXmlReaderFactory factory)
		{
			_serializer = serializer;
			_factory    = factory;
		}

		public T Get(string data)

		{
			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(data)))
			{
				using (var reader = _factory.Get(stream))
				{
					var result = (T)_serializer.Deserialize(reader);
					return result;
				}
			}
		}

		public string Get(T instance)
		{
			using (var stream = new MemoryStream())
			{
				using (var writer = XmlWriter.Create(stream))
				{
					_serializer.Serialize(writer, instance);
					writer.Flush();
					stream.Seek(0, SeekOrigin.Begin);
					var result = new StreamReader(stream).ReadToEnd();
					return result;
				}
			}
		}
	}
}