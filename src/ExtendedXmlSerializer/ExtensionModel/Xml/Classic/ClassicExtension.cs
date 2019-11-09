using ExtendedXmlSerializer.ContentModel.Collections;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.Content;

namespace ExtendedXmlSerializer.ExtensionModel.Xml.Classic
{
	sealed class ClassicExtension : ISerializerExtension
	{
		public static ClassicExtension Default { get; } = new ClassicExtension();

		ClassicExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.DecorateContent<DefaultCollectionSpecification, ClassicCollections>()
			            .DecorateContent<DictionaryContentSpecification, ClassicDictionaryContents>();

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}