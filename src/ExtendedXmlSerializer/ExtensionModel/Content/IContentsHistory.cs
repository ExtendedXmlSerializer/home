using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core.Sources;
using System.Collections.Generic;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	/// <summary>
	/// Component used to "walk the stack up" in the history of deserialized content.
	/// </summary>
	public interface IContentsHistory : IParameterizedSource<IFormatReader, Stack<IInnerContent>> {}
}