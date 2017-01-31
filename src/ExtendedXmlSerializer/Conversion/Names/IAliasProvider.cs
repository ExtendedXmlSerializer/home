using System.Reflection;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Conversion.Names
{
	public interface IAliasProvider : IParameterizedSource<MemberInfo, string> {}
}