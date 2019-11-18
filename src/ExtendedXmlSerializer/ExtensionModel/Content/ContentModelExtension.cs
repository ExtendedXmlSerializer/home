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
	/// <summary>
	/// A default serializer extension. This configures the content model, and registers all necessary components to
	/// resolve serializers for different types of content.
	/// </summary>
	public sealed class ContentModelExtension : ISerializerExtension
	{
		/// <summary>
		/// The default instance.
		/// </summary>
		public static ContentModelExtension Default { get; } = new ContentModelExtension();

		ContentModelExtension() : this(ContentReaders.Default, ContentWriters.Default) {}

		readonly IContentReaders _readers;
		readonly IContentWriters _writers;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:ExtendedXmlSerializer.ExtensionModel.Content.ContentModelExtension"/> class.
		/// </summary>
		/// <param name="readers">The readers.</param>
		/// <param name="writers">The writers.</param>
		public ContentModelExtension(IContentReaders readers, IContentWriters writers)
		{
			_readers = readers;
			_writers = writers;
		}

		/// <inheritdoc />
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