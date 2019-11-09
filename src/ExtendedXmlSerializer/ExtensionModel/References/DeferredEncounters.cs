using System.Linq;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class DeferredEncounters : IEncounters
	{
		readonly IEncounters   _encounters;
		readonly ITrackedLists _tracked;

		public DeferredEncounters(IEncounters encounters, ITrackedLists tracked)
		{
			_encounters = encounters;
			_tracked    = tracked;
		}

		public bool IsSatisfiedBy(object parameter) => !_tracked.Get(parameter)
		                                                        .Any() && _encounters.IsSatisfiedBy(parameter);

		public Identifier? Get(object parameter) => _encounters.Get(parameter);
	}
}