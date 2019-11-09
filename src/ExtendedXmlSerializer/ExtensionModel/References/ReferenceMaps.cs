using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class ReferenceMaps : AssociationAwareReferenceMaps
	{
		public static ReferenceMaps Default { get; } = new ReferenceMaps();

		ReferenceMaps() : base(new ReferenceCache<object, IReferenceMap>(_ => new ReferenceMap())) {}
	}
}