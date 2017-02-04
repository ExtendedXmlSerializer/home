using System.Xml;

namespace ExtendedXmlSerialization.Conversion.Members
{
	class MemberEmitter : IEmitter
	{
		readonly IMembers _members;
		public MemberEmitter(IMembers members)
		{
			_members = members;
		}

		public void Emit(XmlWriter writer, object instance)
		{
			foreach (var member in _members)
			{
				var value = member.Get(instance);
				if (value != null)
				{
					member.Emit(writer, value);
				}
			}
		}
	}
}