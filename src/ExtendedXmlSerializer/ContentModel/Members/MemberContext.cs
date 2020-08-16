using System.Collections.Immutable;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	readonly struct MemberContext
	{
		public MemberContext(TypeInfo reflectedType, ImmutableArray<IMember> members)
		{
			ReflectedType = reflectedType;
			Members       = members;
		}

		public TypeInfo ReflectedType { get; }

		public ImmutableArray<IMember> Members { get; }
	}
}