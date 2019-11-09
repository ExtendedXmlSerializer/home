using System.Collections.Generic;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class DeferredCommands : ReferenceCache<IFormatReader, ICollection<IDeferredCommand>>, IDeferredCommands
	{
		public static DeferredCommands Default { get; } = new DeferredCommands();

		DeferredCommands() : base(_ => new HashSet<IDeferredCommand>()) {}
	}
}