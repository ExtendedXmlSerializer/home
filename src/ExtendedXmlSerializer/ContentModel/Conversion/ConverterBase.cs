using System.Reflection;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ContentModel.Conversion
{
	public abstract class ConverterBase<T> : DecoratedSpecification<TypeInfo>, IConverter<T>
	{
		protected readonly static ISpecification<TypeInfo> Specification = TypeEqualitySpecification<T>.Default;

		protected ConverterBase() : this(Specification) {}

		protected ConverterBase(ISpecification<TypeInfo> specification) : base(specification) {}

		public abstract T Parse(string data);

		public abstract string Format(T instance);
	}
}