using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core.Sources;
using System.Collections.Generic;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	/// <inheritdoc cref="IContentsHistory" />
	public sealed class ContentsHistory : ReferenceCache<IFormatReader, Stack<IInnerContent>>, IContentsHistory
	{
		/// <summary>
		/// The default instance.
		/// </summary>
		public static ContentsHistory Default { get; } = new ContentsHistory();

		ContentsHistory() : base(_ => new Stack<IInnerContent>()) {}
	}
}