using System.IO;

namespace ExtendedXmlSerialization.Test.Support
{
	class ExtendedXmlSerializerTestSupport : IExtendedXmlSerializerTestSupport
	{
		public static ExtendedXmlSerializerTestSupport Default { get; } = new ExtendedXmlSerializerTestSupport();
		ExtendedXmlSerializerTestSupport() : this(new ExtendedXmlSerializer()) {}

		readonly IExtendedXmlSerializer _serializer;

		protected ExtendedXmlSerializerTestSupport(IExtendedXmlSerializer serializer)
		{
			_serializer = serializer;
		}

		public T Assert<T>(T instance, string expected)
		{
			var data = _serializer.Serialize(instance);
			Xunit.Assert.Equal(expected, data);
			var result = _serializer.Deserialize<T>(data);
			return result;
		}

		public void Serialize(Stream stream, object instance) => _serializer.Serialize(stream, instance);

		public object Deserialize(Stream stream) => _serializer.Deserialize(stream);
	}
}