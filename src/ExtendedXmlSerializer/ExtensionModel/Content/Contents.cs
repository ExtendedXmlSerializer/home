using ExtendedXmlSerializer.ContentModel.Collections;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.References;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	public sealed class Contents : ISerializerExtension
	{
		public static Contents Default { get; } = new Contents();

		Contents() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.RegisterConstructorDependency<IContents>((provider, info) => provider.Get<DeferredContents>())
			            .Register<IContents, RuntimeContents>()
			            .DecorateContent<IActivatingTypeSpecification, MemberedContents>()
			            .DecorateContent<DefaultCollectionSpecification, DefaultCollections>()
			            .Register<IDictionaryEntries, DictionaryEntries>()
			            .DecorateContent<DictionaryContentSpecification, DictionaryContents>()
			            .DecorateContent<Arrays>(ArraySpecification.Default)
			            .Decorate<IContents, MappedArrayContents>()
			            .RegisterInstance(ReflectionSerializer.Default)
			            .DecorateContent<ReflectionContents>(ReflectionContentSpecification.Default)
			            .DecorateContent<NullableContents>(IsNullableTypeSpecification.Default)
			            .DecorateContent<ConverterSpecification, ConverterContents>()
			            .DecorateContent<RegisteredContentSpecification, RegisteredContents>();

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}