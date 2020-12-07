using ExtendedXmlSerializer.Core.Specifications;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class IsInterface : DelegatedSpecification<TypeInfo>
	{
		public static IsInterface Default { get; } = new IsInterface();

		IsInterface() : base(x => x.IsInterface) {}
	}
}
