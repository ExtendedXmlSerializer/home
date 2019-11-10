using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel
{
	public sealed class RuntimeSerializers : ISerializers
	{
		readonly ISpecification<TypeInfo> _specification;
		readonly ISerializers             _serializers;

		public RuntimeSerializers(ISerializers serializers) : this(VariableTypeSpecification.Default, serializers) {}

		public RuntimeSerializers(ISpecification<TypeInfo> specification, ISerializers serializers)
		{
			_specification = specification;
			_serializers   = serializers;
		}

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