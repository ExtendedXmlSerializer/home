using ExtendedXmlSerializer.Core.Specifications;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class IsAnonymousType : DelegatedSpecification<TypeInfo>
	{
		public static IsAnonymousType Default { get; } = new IsAnonymousType();

		IsAnonymousType() : base(x => x.Name.StartsWith("<>f__")) {}
	}
}
