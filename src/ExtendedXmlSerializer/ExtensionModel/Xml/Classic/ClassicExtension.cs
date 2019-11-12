using ExtendedXmlSerializer.ContentModel.Collections;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.Core;

namespace ExtendedXmlSerializer.ExtensionModel.Xml.Classic
{
	sealed class ClassicExtension : ISerializerExtension
	{
		public static ClassicExtension Default { get; } = new ClassicExtension();

		ClassicExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.DecorateContentsWith<ClassicCollections>()
			            .When<DefaultCollectionSpecification>()
			            .DecorateContentsWith<ClassicDictionaryContents>()
			            .When<DictionaryContentSpecification>();

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}