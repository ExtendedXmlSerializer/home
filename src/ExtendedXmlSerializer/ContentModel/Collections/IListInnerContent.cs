using System.Collections;
using ExtendedXmlSerializer.ContentModel.Content;

namespace ExtendedXmlSerializer.ContentModel.Collections
{
	interface IListInnerContent : IInnerContent
	{
		IList List { get; }
	}
}