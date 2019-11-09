using ExtendedXmlSerializer.ContentModel.Identification;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class Member : Identity, IMember
	{
		// ReSharper disable once TooManyDependencies
		public Member(string name, int order, MemberInfo metadata, TypeInfo memberType, bool isWritable)
			: base(name, string.Empty)
		{
			IsWritable = isWritable;
			Order      = order;
			Metadata   = metadata;
			MemberType = memberType;
		}

		public bool IsWritable { get; }
		public int Order { get; }

		public MemberInfo Metadata { get; }
		public TypeInfo MemberType { get; }
	}
}