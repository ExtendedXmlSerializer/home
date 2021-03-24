using ExtendedXmlSerializer.Core.Sources;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	sealed class RecursionAwareContents : DecoratedSource<TypeInfo, ISerializer>, IContents
	{
		public RecursionAwareContents(IContents contents, IRecursionContents recursion)
			: base(recursion.Get(contents)) {}
	}
}