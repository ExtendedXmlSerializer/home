using ExtendedXmlSerializer.ContentModel.Collections;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel.Services;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	public sealed class CompositeContentModelExtension : ISerializerExtension
	{
		public static CompositeContentModelExtension Default { get; } = new CompositeContentModelExtension();
		CompositeContentModelExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.Register<IInnerContentServices, InnerContentServices>()
			            .Register<IMemberHandler, MemberHandler>()
			            .Register<ICollectionContentsHandler, CollectionContentsHandler>()
			            .RegisterInstance<IAlteration<IInnerContentHandler>>(Self<IInnerContentHandler>.Default)
			            .RegisterInstance<IInnerContentResult>(InnerContentResult.Default)
			            .RegisterInstance<IMemberAssignment>(MemberAssignment.Default)
			            .RegisterInstance<ICollectionAssignment>(CollectionAssignment.Default)
			            .RegisterInstance<IListContentsSpecification>(ListContentsSpecification.Default);

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}