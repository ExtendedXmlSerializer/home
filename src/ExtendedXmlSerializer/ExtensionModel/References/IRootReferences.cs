using System.Collections.Immutable;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	interface IRootReferences : IParameterizedSource<IFormatWriter, ImmutableArray<object>> {}
}