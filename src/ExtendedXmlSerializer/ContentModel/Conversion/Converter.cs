using System;
using System.Reflection;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ContentModel.Conversion
{
	public class Converter<T> : ConverterBase<T>, IConverter
	{
		readonly Func<string, T> _deserialize;
		readonly Func<T, string> _serialize;

		public Converter(Func<string, T> deserialize, Func<T, string> serialize) :
			this(Specification, deserialize, serialize) {}

		public Converter(ISpecification<TypeInfo> specification, Func<string, T> deserialize, Func<T, string> serialize)
			: base(specification)
		{
			_deserialize = deserialize;
			_serialize   = serialize;
		}

		public sealed override T Parse(string data) => _deserialize(data);

		public sealed override string Format(T instance) => _serialize(instance);

		object IConvert<object>.Parse(string data) => Parse(data);

		string IConvert<object>.Format(object instance) => instance != null ? Format((T)instance) : null;

		public TypeInfo Get() => Support<T>.Metadata;
	}
}