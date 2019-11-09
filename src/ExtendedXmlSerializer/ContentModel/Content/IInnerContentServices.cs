using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Collections;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Members;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	interface IInnerContentServices
		: IListContentsSpecification, IMemberHandler, ICollectionContentsHandler, IReaderFormatter
	{
		IReader Create(TypeInfo classification, IInnerContentHandler handler);
	}
}