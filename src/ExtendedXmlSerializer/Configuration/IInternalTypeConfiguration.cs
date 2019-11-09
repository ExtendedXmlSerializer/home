using System.Reflection;

namespace ExtendedXmlSerializer.Configuration
{
	interface IInternalTypeConfiguration
	{
		ITypeConfiguration Name(string name);

		IMemberConfiguration Member(MemberInfo member);
	}
}