using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Conversion
{
	sealed class CachedConverters : Cache<TypeInfo, IConverter>, IConverters
	{
		public CachedConverters(IConverters converters) : base(converters.Get) {}
	}
}