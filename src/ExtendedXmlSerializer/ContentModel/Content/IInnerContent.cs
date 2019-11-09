using System.Collections;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	public interface IInnerContent : ISource<IFormatReader>, IEnumerator {}
}