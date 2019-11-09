using System;
using ExtendedXmlSerializer.ContentModel.Format;

namespace ExtendedXmlSerializer.ContentModel
{
	sealed class DeferredSerializer : ISerializer
	{
		readonly Func<ISerializer> _serializer;

		public DeferredSerializer(Func<ISerializer> serializer)
		{
			_serializer = serializer;
		}

		public object Get(IFormatReader parameter) => _serializer()
			.Get(parameter);

		public void Write(IFormatWriter writer, object instance) => _serializer()
			.Write(writer, instance);
	}
}