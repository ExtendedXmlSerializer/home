using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	sealed class CachedContents : Cache<TypeInfo, ISerializer>, IContents
	{
		public CachedContents(IContents content) : base(content.Get) {}
	}
}