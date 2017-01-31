using System.Reflection;
using ExtendedXmlSerialization.Conversion.Model.Names;

namespace ExtendedXmlSerialization.Conversion
{
	class MemberName : Name, IMemberName
	{
		public MemberName(string displayName, MemberInfo metadata, TypeInfo memberType)
			: base(displayName, metadata.DeclaringType)
		{
			Metadata = metadata;
			MemberType = memberType;
		}

		public MemberInfo Metadata { get; }
		public TypeInfo MemberType { get; }
	}
}