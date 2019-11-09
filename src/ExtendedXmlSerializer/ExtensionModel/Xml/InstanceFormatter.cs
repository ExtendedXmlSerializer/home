using System;
using System.IO;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	class InstanceFormatter : IInstanceFormatter
	{
		readonly static Func<Stream> Stream = DefaultActivators.Default.New<MemoryStream>;

		readonly IExtendedXmlSerializer _serializer;
		readonly IXmlWriterFactory      _factory;
		readonly Func<Stream>           _stream;

		public InstanceFormatter(IExtendedXmlSerializer serializer) : this(serializer, Stream) {}

		public InstanceFormatter(IExtendedXmlSerializer serializer, Func<Stream> stream)
			: this(serializer, XmlWriterFactory.Default, stream) {}

		public InstanceFormatter(IExtendedXmlSerializer serializer, IXmlWriterFactory factory, Func<Stream> stream)
		{
			_serializer = serializer;
			_factory    = factory;
			_stream     = stream;
		}

		public string Get(object parameter)
		{
			var stream = _stream();
			using (var writer = _factory.Get(stream))
			{
				_serializer.Serialize(writer, parameter);
				writer.Flush();
				stream.Seek(0, SeekOrigin.Begin);
				var result = new StreamReader(stream).ReadToEnd();
				return result;
			}
		}
	}
}