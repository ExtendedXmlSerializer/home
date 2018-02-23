using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Collections;
using ExtendedXmlSerializer.ExtensionModel.Services;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Alterations
{
	[GroupElement(nameof(Categories.Alterations)), Dependency(typeof(Instances.RootInstanceExtension))]
	sealed class ValueAlterationExtension : ISerializerExtension
	{
		public static ValueAlterationExtension Default { get; } = new ValueAlterationExtension();
		ValueAlterationExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.DecorateDefinition<IContents<object>, ValuePipelineContents<object>>()
			            .Decorate<IFormatWriters, ContentPipelineFormatWriters>();

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}
