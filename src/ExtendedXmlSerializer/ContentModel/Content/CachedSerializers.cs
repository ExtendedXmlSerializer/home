using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	sealed class CachedSerializers : Cache<TypeInfo, ISerializer>, ISerializers
	{
		public CachedSerializers(ISerializers serializers) : base(serializers.Get) {}
	}
}