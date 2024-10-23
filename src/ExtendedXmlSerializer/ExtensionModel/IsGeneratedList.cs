using ExtendedXmlSerializer.Core.Specifications;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel;

sealed class IsGeneratedList : DelegatedSpecification<TypeInfo>
{
	public static IsGeneratedList Default { get; } = new();

	IsGeneratedList()
		: base(x => x.FullName != null &&
		            (x.FullName.StartsWith("<>z__ReadOnlySingleElementList") ||
		             x.FullName.StartsWith("<>z__ReadOnlyArray"))) {}
}