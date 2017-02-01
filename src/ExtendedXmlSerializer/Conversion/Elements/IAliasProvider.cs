using System.Reflection;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Conversion.Elements
{
	public interface IAliasProvider : IParameterizedSource<MemberInfo, string> {}
}