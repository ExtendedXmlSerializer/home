using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.Core;

namespace ExtendedXmlSerializer.ExtensionModel.Markup
{
	sealed class MarkupExtension : ISerializerExtension
	{
		public static MarkupExtension Default { get; } = new MarkupExtension();

		MarkupExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.Register<IMarkupExtensions, MarkupExtensions>()
			            .Register<IEnhancer, Enhancer>()
			            .Decorate<ISerializers, Serializers>()
			            .Decorate<IContents, Contents>();

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}