using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Conversion
{
	/// <summary>
	/// A delegate-based converter used as a convenience for extension authors.
	/// </summary>
	/// <typeparam name="T">The type to convert.</typeparam>
	public class Converter<T> : ConverterBase<T>, IConverter
	{
		readonly Func<string, T> _deserialize;
		readonly Func<T, string> _serialize;

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="deserialize">The deserialization delegate used to create the instance from the provided text.</param>
		/// <param name="serialize">The serialization delegate used to create the text form of the provided instance.</param>
		public Converter(Func<string, T> deserialize, Func<T, string> serialize)
			: this(TypeEqualitySpecification<T>.Default, deserialize, serialize) {}


		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="specification">The specification that determines whether the created converter handles the candidate type.</param>
		/// <param name="deserialize">The deserialization delegate used to create the instance from the provided text.</param>
		/// <param name="serialize">The serialization delegate used to create the text form of the provided instance.</param>
		public Converter(ISpecification<TypeInfo> specification, Func<string, T> deserialize, Func<T, string> serialize)
			: base(specification)
		{
			_deserialize = deserialize;
			_serialize   = serialize;
		}

		/// <inheritdoc />
		public sealed override T Parse(string data) => _deserialize(data);

		/// <inheritdoc />
		public sealed override string Format(T instance) => _serialize(instance);

		object IConvert<object>.Parse(string data) => Parse(data);

		string IConvert<object>.Format(object instance) => instance != null ? Format((T)instance) : null;

		TypeInfo ISource<TypeInfo>.Get() => Support<T>.Metadata;
	}
}