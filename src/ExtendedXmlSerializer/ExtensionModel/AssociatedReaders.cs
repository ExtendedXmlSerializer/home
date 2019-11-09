using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel
{
	sealed class AssociatedReaders : ReferenceCache<IFormatReader, IFormatReader>, IAlteration<IFormatReader>
	{
		public static AssociatedReaders Default { get; } = new AssociatedReaders();

		AssociatedReaders() : base(x => x) {}
	}
}