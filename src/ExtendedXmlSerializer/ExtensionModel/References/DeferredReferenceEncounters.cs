using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ExtensionModel.Content;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class DeferredReferenceEncounters : IReferenceEncounters
	{
		readonly IReferenceEncounters _encounters;
		readonly IReservedItems       _reserved;

		public DeferredReferenceEncounters(IReferenceEncounters encounters) : this(encounters, ReservedItems.Default) {}

		public DeferredReferenceEncounters(IReferenceEncounters encounters, IReservedItems reserved)
		{
			_encounters = encounters;
			_reserved   = reserved;
		}

		public IEncounters Get(IFormatWriter parameter)
			=> new DeferredEncounters(_encounters.Get(parameter), _reserved.Get(parameter));
	}
}