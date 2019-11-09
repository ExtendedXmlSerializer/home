using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel.Content.Members;

namespace ExtendedXmlSerializer.Configuration
{
	sealed class EmitBehavior : IEmitBehavior
	{
		readonly IAlteration<AllowedMemberValuesExtension> _alteration;

		public EmitBehavior(IAlteration<AllowedMemberValuesExtension> alteration) => _alteration = alteration;

		public IConfigurationContainer Get(IConfigurationContainer parameter)
			=> parameter.Extend(_alteration.Get(parameter.Root.Find<AllowedMemberValuesExtension>()));
	}
}