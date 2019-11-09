using System.Reflection;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class NamespaceSpecification<T> : DecoratedSpecification<TypeInfo>
	{
		public static NamespaceSpecification<T> Default { get; } = new NamespaceSpecification<T>();

		NamespaceSpecification() : base(new NamespaceSpecification(typeof(T).GetTypeInfo())) {}
	}

	sealed class NamespaceSpecification : ISpecification<TypeInfo>
	{
		readonly TypeInfo _target;

		public NamespaceSpecification(TypeInfo target)
		{
			_target = target;
		}

		public bool IsSatisfiedBy(TypeInfo parameter)
			=> Equals(parameter.Assembly, _target.Assembly) && parameter.Namespace == _target.Namespace;
	}
}