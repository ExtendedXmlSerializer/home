using System.Reflection;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.ContentModel
{
	sealed class Aliases : Cache<TypeInfo, string>, IAliases
	{
		public Aliases(IParameterizedSource<TypeInfo, string> source) : base(source.Get) {}
	}
}