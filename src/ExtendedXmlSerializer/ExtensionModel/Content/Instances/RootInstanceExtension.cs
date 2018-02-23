using ExtendedXmlSerializer.Core.Collections;
using ExtendedXmlSerializer.ExtensionModel.Services;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Instances {
	[GroupElement(nameof(Categories.Framework))]
	sealed class RootInstanceExtension : ISerializerExtension
	{
		public static RootInstanceExtension Default { get; } = new RootInstanceExtension();
		RootInstanceExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.DecorateDefinition<IWriters<object>, RootInstanceAwareWriters<object>>();

		public void Execute(IServices parameter) {}
	}
}