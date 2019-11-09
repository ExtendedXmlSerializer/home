using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	class AssociationAwareReferenceMaps : DecoratedSource<IFormatReader, IReferenceMap>, IReferenceMaps
	{
		public AssociationAwareReferenceMaps(IParameterizedSource<IFormatReader, IReferenceMap> maps)
			: base(maps.In(AssociatedReaders.Default)) {}
	}
}