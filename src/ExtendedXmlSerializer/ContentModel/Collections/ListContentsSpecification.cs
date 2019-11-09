using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ContentModel.Collections
{
	sealed class ListContentsSpecification : DecoratedSpecification<IInnerContent>, IListContentsSpecification
	{
		public static ListContentsSpecification Default { get; } = new ListContentsSpecification();

		ListContentsSpecification() : this(IsTypeSpecification<IListInnerContent>.Default) {}

		public ListContentsSpecification(ISpecification<IInnerContent> specification) : base(specification) {}
	}
}