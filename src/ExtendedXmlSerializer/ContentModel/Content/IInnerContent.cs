using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core.Sources;
using System.Collections;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	/// <summary>
	/// Used during deserialization to enumerate any inner content an element might have.
	/// </summary>
	public interface IInnerContent : ISource<IFormatReader>, IEnumerator {}
}