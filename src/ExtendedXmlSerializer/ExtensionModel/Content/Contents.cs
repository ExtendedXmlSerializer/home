using ExtendedXmlSerializer.ContentModel.Collections;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.References;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	/// <summary>
	/// A default serializer extension.  This extension defines necessary default components for rendering contents of different content types, such as dictionaries and arrays, etc.
	/// </summary>
	public sealed class Contents : ISerializerExtension
	{
		/// <summary>
		/// The default instance.
		/// </summary>
		public static Contents Default { get; } = new Contents();

		Contents() {}

		/// <inheritdoc />
		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.RegisterConstructorDependency<IContents>((provider, _) => provider.Get<DeferredContents>())
			            .Register<IContents, RuntimeContents>()

			            .DecorateContentsWith<MemberedContents>()
			            .When<IActivatingTypeSpecification>()

			            .DecorateContentsWith<DefaultCollections>()
			            .When<DefaultCollectionSpecification>()

			            .Register<IDictionaryEntries, DictionaryEntries>()
			            .DecorateContentsWith<DictionaryContents>()
			            .When<DictionaryContentSpecification>()

			            .DecorateContentsWith<Arrays>()
			            .When(ArraySpecification.Default)

			            .DecorateContentsWith<MappedArrayContents>()
			            .Then()

			            .RegisterInstance(ReflectionSerializer.Default)

			            .DecorateContentsWith<ReflectionContents>()
			            .When(ReflectionContentSpecification.Default)

			            .DecorateContentsWith<ConverterContents>()
			            .When<ConverterSpecification>()
			            .DecorateContentsWith<RegisteredContents>()
			            .When<RegisteredContentSpecification>();

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}