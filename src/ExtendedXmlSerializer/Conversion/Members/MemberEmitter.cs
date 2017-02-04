using System.Collections.Immutable;
using System.Xml;

namespace ExtendedXmlSerialization.Conversion.Members
{
	class MemberEmitter : IEmitter
	{
		readonly ImmutableArray<IMember> _members;
		public MemberEmitter(ImmutableArray<IMember> members)
		{
			_members = members;
		}

		public void Emit(XmlWriter writer, object instance)
		{
			for (var i = 0; i < _members.Length; i++)
			{
				var member = _members[i];
				var value = member.Get(instance);
				if (value != null)
				{
					member.Emit(writer, value);
				}
			}
		}
	}
}