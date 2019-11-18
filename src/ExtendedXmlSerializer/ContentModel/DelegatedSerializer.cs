using ExtendedXmlSerializer.ContentModel.Format;
using System;

namespace ExtendedXmlSerializer.ContentModel
{
	sealed class DelegatedSerializer<T> : ISerializer<T>
	{
		readonly Action<IFormatWriter, T> _serialize;
		readonly Func<IFormatReader, T>   _deserialize;

		public DelegatedSerializer(Action<IFormatWriter, T> serialize, Func<IFormatReader, T> deserialize)
		{
			_serialize   = serialize;
			_deserialize = deserialize;
		}

		public T Get(IFormatReader parameter) => _deserialize(parameter);

		public void Write(IFormatWriter writer, T instance)
		{
			_serialize(writer, instance);
		}
	}
}