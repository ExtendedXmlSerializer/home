using System.Collections.Generic;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	interface IDeferredCommands : IParameterizedSource<IFormatReader, ICollection<IDeferredCommand>> {}
}