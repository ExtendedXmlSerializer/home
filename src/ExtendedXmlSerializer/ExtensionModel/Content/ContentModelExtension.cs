using ExtendedXmlSerializer.ContentModel.Collections;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;
using VariableTypeSpecification = ExtendedXmlSerializer.ReflectionModel.VariableTypeSpecification;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	public sealed class ContentModelExtension : ISerializerExtension
	{
		public static ContentModelExtension Default { get; } = new ContentModelExtension();

		ContentModelExtension() : this(ContentReaders.Default, ContentWriters.Default) {}

		readonly IContentReaders _readers;
		readonly IContentWriters _writers;

		public ContentModelExtension(IContentReaders readers, IContentWriters writers)
		{
			_readers = readers;
			_writers = writers;
		}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.Register<RuntimeElement>()
			            .Register<Element>()
			            .Register<IElement, Element>()
			            .DecorateElementWith<VariableTypeElement>()
			            .When(VariableTypeSpecification.Default)
			            .DecorateElementWith<GenericElement>()
			            .When(IsGenericTypeSpecification.Default)
			            .DecorateElementWith<ArrayElement>()
			            .When(IsArraySpecification.Default)
			            .Register<IClassification, Classification>()
			            .Register<IIdentityStore, IdentityStore>()
			            .Register<IInnerContentServices, InnerContentServices>()
			            .Register<IMemberHandler, MemberHandler>()
			            .Register<ICollectionContentsHandler, CollectionContentsHandler>()
			            .RegisterInstance(_writers)
			            .RegisterInstance(_readers)
			            .RegisterInstance<IAlteration<IInnerContentHandler>>(Self<IInnerContentHandler>.Default)
			            .RegisterInstance<IInnerContentResult>(InnerContentResult.Default)
			            .RegisterInstance<IMemberAssignment>(MemberAssignment.Default)
			            .RegisterInstance<ICollectionAssignment>(CollectionAssignment.Default)
			            .RegisterInstance<IListContentsSpecification>(ListContentsSpecification.Default);

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}