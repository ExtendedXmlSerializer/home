using System.Reflection;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Markup
{
	sealed class Contents : DecoratedSource<TypeInfo, ISerializer>, IContents
	{
		public Contents(IEnhancer enhancer, IContents contents)
			: base(new AlteredSource<TypeInfo, ISerializer>(enhancer, contents)) {}
	}
}