using ExtendedXmlSerializer.Configuration;
using FluentAssertions;
using System;

namespace ExtendedXmlSerializer.Tests.ReportedIssues.Support
{
	sealed class SerializationSupport : ISerializationSupport
	{
		readonly IExtendedXmlSerializer _serializer;

		public SerializationSupport() : this(new ConfigurationContainer()) {}

		public SerializationSupport(IConfigurationContainer configuration) : this(configuration.Create()) {}

		public SerializationSupport(IExtendedXmlSerializer serializer) => _serializer = serializer;

		public T Assert<T>(T instance, string expected)
		{
			var data = _serializer.Serialize(instance);
			data?.Replace("\r\n", string.Empty)
			    .Replace("\n", string.Empty)
			    .Should()
			    .Be(expected?.Replace("\r\n", string.Empty)
			                .Replace("\n", string.Empty));
			var result = _serializer.Deserialize<T>(data);
			return result;
		}

		public void Serialize(System.Xml.XmlWriter writer, object instance) => _serializer.Serialize(writer, instance);

		public object Deserialize(System.Xml.XmlReader stream) => _serializer.Deserialize(stream);

		// Used for a simple way to emit instances as text in tests:
		public void WriteLine(object instance) => throw new InvalidOperationException(_serializer.Serialize(instance));
	}
}