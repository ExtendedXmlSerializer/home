using System.Reflection;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Conversion.Model.Names
{
	public interface IAliasProvider : IParameterizedSource<MemberInfo, string> {}
}