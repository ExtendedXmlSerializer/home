using ExtendedXmlSerializer.ContentModel.Format;

namespace ExtendedXmlSerializer.ContentModel
{
	sealed class GeneralSerializerAdapter<T> : ISerializer<T>
	{
		readonly ISerializer _serializer;

		public GeneralSerializerAdapter(ISerializer serializer) => _serializer = serializer;

		public T Get(IFormatReader parameter) => (T)_serializer.Get(parameter);

		public void Write(IFormatWriter writer, T instance)
		{
			_serializer.Write(writer, instance);
		}
	}

	sealed class GenericSerializerAdapter<T> : ISerializer
	{
		readonly ISerializer<T> _serializer;

		public GenericSerializerAdapter(ISerializer<T> serializer) => _serializer = serializer;

		public object Get(IFormatReader parameter) => _serializer.Get(parameter);

		public void Write(IFormatWriter writer, object instance) => _serializer.Write(writer, (T)instance);
	}
}