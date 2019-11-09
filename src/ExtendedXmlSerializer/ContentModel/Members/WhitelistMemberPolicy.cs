using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class WhitelistMemberPolicy : ContainsSpecification<MemberInfo>, IMemberPolicy
	{
		readonly static MemberComparer Comparer = MemberComparer.Default;

		public WhitelistMemberPolicy(params MemberInfo[] avoid) : this(Comparer, avoid) {}

		public WhitelistMemberPolicy(IMemberComparer comparer, params MemberInfo[] avoid)
			: this(new HashSet<MemberInfo>(avoid, comparer)) {}

		public WhitelistMemberPolicy(ICollection<MemberInfo> avoid) : base(avoid) {}
	}
}