using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.Core
{
	sealed class Conditions : ReferenceCache<object, ConditionMonitor>, IConditions
	{
		public static Conditions Default { get; } = new Conditions();

		public Conditions() : base(_ => new ConditionMonitor()) {}
	}
}