using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.References;

sealed class MultipleReferencesAllowed : FixedInstanceSource<bool>, IMultipleReferencesAllowed
{
	public MultipleReferencesAllowed(bool instance) : base(instance) {}

	public bool Allowed => Get();
}