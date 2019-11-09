using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel.Content;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class ReservedItems : ReferenceCache<IFormatWriter, ITrackedLists>, IReservedItems
	{
		public static ReservedItems Default { get; } = new ReservedItems();

		ReservedItems() : base(_ => new TrackedLists()) {}
	}
}