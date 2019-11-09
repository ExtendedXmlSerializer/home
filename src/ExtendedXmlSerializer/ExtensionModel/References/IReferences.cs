using System.Collections.Immutable;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	public interface IReferences : IParameterizedSource<object, ImmutableArray<object>> {}
}