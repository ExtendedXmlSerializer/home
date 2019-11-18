using ExtendedXmlSerializer.Core.Sources;
using System.Collections.Immutable;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	/// <summary>
	/// Specialized selector used to access references.
	/// </summary>
	public interface IReferences : IParameterizedSource<object, ImmutableArray<object>> {}
}