using System.Reflection;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class TypeEqualitySpecification<T> : TypeEqualitySpecification
	{
		public static TypeEqualitySpecification<T> Default { get; } = new TypeEqualitySpecification<T>();

		TypeEqualitySpecification() : base(typeof(T).GetTypeInfo()) {}
	}

	class TypeEqualitySpecification : DelegatedSpecification<TypeInfo>
	{
		public TypeEqualitySpecification(TypeInfo type) : base(type.Equals) {}
	}
}