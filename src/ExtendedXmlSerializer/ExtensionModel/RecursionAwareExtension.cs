using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.Core;

namespace ExtendedXmlSerializer.ExtensionModel
{
	sealed class RecursionAwareExtension : ISerializerExtension
	{
		public static RecursionAwareExtension Default { get; } = new RecursionAwareExtension();

		RecursionAwareExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.RegisterInstance<IRecursionContents>(RecursionContents.Default)
			            .Decorate<IContents, RecursionAwareContents>();

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}
