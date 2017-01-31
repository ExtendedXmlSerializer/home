using System.Reflection;
using ExtendedXmlSerialization.Conversion.Model.Names;

namespace ExtendedXmlSerialization.Conversion
{
	public interface IMemberName : IName
	{
		MemberInfo Metadata { get; }

		TypeInfo MemberType { get; }
	}
}