using System.Collections.Generic;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	public sealed class ContentsHistory : ReferenceCache<IFormatReader, Stack<IInnerContent>>, IContentsHistory
	{
		public static ContentsHistory Default { get; } = new ContentsHistory();

		ContentsHistory() : base(_ => new Stack<IInnerContent>()) {}
	}
}