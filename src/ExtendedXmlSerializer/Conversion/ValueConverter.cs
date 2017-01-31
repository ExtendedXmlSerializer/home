using System;
using System.Reflection;

namespace ExtendedXmlSerialization.Conversion
{
	class ValueConverter<T> : ConverterBase<T>
	{
		readonly static TypeInfo Type = typeof(T).GetTypeInfo();

		readonly Func<string, T> _deserialize;
		readonly Func<T, string> _serialize;

		public ValueConverter(Func<string, T> deserialize, Func<T, string> serialize) : base(Type)
		{
			_deserialize = deserialize;
			_serialize = serialize;
		}

		public override void Emit(IWriter writer, T instance) => writer.Write(_serialize(instance));

		public override object Get(IReader reader) => _deserialize(reader.Value());
	}
}