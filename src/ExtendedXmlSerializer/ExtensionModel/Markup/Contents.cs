using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.Core.Sources;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Markup
{
	sealed class Contents : DecoratedSource<TypeInfo, ContentModel.ISerializer>, IContents
	{
		public Contents(IEnhancer enhancer, IContents contents)
			: base(new AlteredSource<TypeInfo, ContentModel.ISerializer>(enhancer, contents)) {}
	}
}