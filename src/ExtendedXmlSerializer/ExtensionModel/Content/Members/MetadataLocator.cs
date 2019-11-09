using System;
using System.Collections.Immutable;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Members;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	sealed class MetadataLocator : IMetadataLocator
	{
		readonly ImmutableArray<MemberInfo> _members;

		public MetadataLocator(ImmutableArray<MemberInfo> members) => _members = members;

		public MemberDescriptor? Get(string parameter)
		{
			for (int i = 0; i < _members.Length; i++)
			{
				var member = _members[i];
				if (member.Name.Equals(parameter, StringComparison.OrdinalIgnoreCase))
				{
					return member;
				}
			}

			return null;
		}
	}
}