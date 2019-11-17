using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel
{
	/// <summary>
	/// A specialized implementation of <see cref="ISerializers"/> for runtime selection of a serializer when no other
	/// serializers can be located.
	/// </summary>
	public sealed class RuntimeSerializers : ISerializers
	{
		readonly ISpecification<TypeInfo> _specification;
		readonly ISerializers             _serializers;

		/// <summary>
		/// Creates an instance.
		/// </summary>
		/// <param name="serializers">The serializer selector to decorate.</param>
		public RuntimeSerializers(ISerializers serializers) : this(VariableTypeSpecification.Default, serializers) {}

		/// <summary>
		/// Creates an instance.
		/// </summary>
		/// <param name="specification">The specification to determine whether to use the provided serializer selector or this
		/// instance.</param>
		/// <param name="serializers">The serializer selector to decorate.</param>
		public RuntimeSerializers(ISpecification<TypeInfo> specification, ISerializers serializers)
		{
			_specification = specification;
			_serializers   = serializers;
		}

		/// <inheritdoc />
		public ISerializer Get(TypeInfo parameter)
		{
			var serializer = _serializers.Get(parameter);
			var result = _specification.IsSatisfiedBy(parameter)
				             ? new Serializer(serializer, new Writer(_serializers))
				             : serializer;
			return result;
		}

		sealed class Writer : IWriter
		{
			readonly ISerializers _serializers;

			public Writer(ISerializers serializers) => _serializers = serializers;

			public void Write(IFormatWriter writer, object instance)
			{
				_serializers.Get(instance.GetType())
				            .Write(writer, instance);
			}
		}
	}
}