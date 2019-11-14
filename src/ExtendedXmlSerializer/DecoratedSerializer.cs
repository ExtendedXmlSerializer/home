using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Format;
using System;

namespace ExtendedXmlSerializer
{
	public class DecoratedSerializer<TRead, TWrite> : ISerializer<TRead, TWrite>
	{
		readonly ISerializer<TRead, TWrite> _serializer;

		public DecoratedSerializer(ISerializer<TRead, TWrite> serializer) => _serializer = serializer;

		public object Deserialize(TRead reader) => _serializer.Deserialize(reader);

		public void Serialize(TWrite writer, object instance)
		{
			_serializer.Serialize(writer, instance);
		}
	}

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