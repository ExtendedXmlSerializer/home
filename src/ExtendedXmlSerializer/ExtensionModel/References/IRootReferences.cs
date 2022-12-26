using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core.Sources;
using System.Collections.Generic;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	interface IRootReferences : IParameterizedSource<IFormatWriter, IReadOnlyCollection<object>> {}
}