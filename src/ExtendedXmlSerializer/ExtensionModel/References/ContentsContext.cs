using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class ContentsContext : ReferenceCache<IInnerContent, object>, IContentsContext
	{
		public static ContentsContext Default { get; } = new ContentsContext();

		ContentsContext() : base(_ => null) {}
	}
}