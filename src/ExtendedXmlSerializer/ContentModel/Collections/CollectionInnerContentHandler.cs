using ExtendedXmlSerializer.ContentModel.Content;

namespace ExtendedXmlSerializer.ContentModel.Collections
{
	sealed class CollectionInnerContentHandler : IInnerContentHandler
	{
		readonly IListContentsSpecification _specification;
		readonly IReader                    _item;
		readonly ICollectionContentsHandler _handler;

		public CollectionInnerContentHandler(IReader item, IInnerContentServices services)
			: this(services, item, services) {}

		public CollectionInnerContentHandler(IListContentsSpecification specification, IReader item,
		                                     ICollectionContentsHandler handler)
		{
			_specification = specification;
			_item          = item;
			_handler       = handler;
		}

		public bool IsSatisfiedBy(IInnerContent parameter)
		{
			var result = _specification.IsSatisfiedBy(parameter);
			if (result)
			{
				_handler.Handle((IListInnerContent)parameter, _item);
			}

			return result;
		}
	}
}