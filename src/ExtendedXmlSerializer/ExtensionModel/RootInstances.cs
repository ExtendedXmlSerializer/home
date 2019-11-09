using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel
{
	sealed class RootInstances : ReferenceCache<object, object>, IRootInstances
	{
		public static RootInstances Default { get; } = new RootInstances();

		RootInstances() : base(_ => null) {}
	}
}