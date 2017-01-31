using System.Reflection;
using ExtendedXmlSerialization.Conversion.Names;

namespace ExtendedXmlSerialization.Conversion.Members
{
	public interface IMemberName : IName
	{
		MemberInfo Metadata { get; }

		TypeInfo MemberType { get; }
	}
}