using System.Collections.Generic;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	public interface IContentsHistory : IParameterizedSource<IFormatReader, Stack<IInnerContent>> {}
}