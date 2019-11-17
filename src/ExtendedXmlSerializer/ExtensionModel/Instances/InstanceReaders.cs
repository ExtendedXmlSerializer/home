using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Instances
{
	sealed class InstanceReaders : ReferenceCache<IExtendedXmlSerializer, IInstanceReader>
	{
		public static InstanceReaders Default { get; } = new InstanceReaders();

		InstanceReaders() : base(x => new InstanceReader(x)) {}
	}
}