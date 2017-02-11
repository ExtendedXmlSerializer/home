using System;
using System.Reflection;
using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.ConverterModel
{
	public interface IConverter : ISpecification<TypeInfo>
	{
		object Load(string data);

		string Save(object instance);
	}

	public interface IConverter<T> : ISpecification<TypeInfo>
	{
		T Load(string data);

		string Save(T instance);
	}

	abstract class ConverterBase<T> : DecoratedSpecification<TypeInfo>, IConverter<T>, IConverter
	{
		readonly Func<string, T> _deserialize;
		readonly Func<T, string> _serialize;

		protected ConverterBase(Func<string, T> deserialize, Func<T, string> serialize)
			: this(TypeEqualitySpecification<T>.Default, deserialize, serialize) {}

		protected ConverterBase(ISpecification<TypeInfo> specification, Func<string, T> deserialize,
		                         Func<T, string> serialize) : base(specification)
		{
			_deserialize = deserialize;
			_serialize = serialize;
		}

		public T Load(string data) => _deserialize(data);

		public string Save(T instance) => _serialize(instance);

		object IConverter.Load(string data) => Load(data);
		string IConverter.Save(object instance) => Save((T) instance);
	}
}