using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;
using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class BlacklistMemberPolicy : InverseSpecification<MemberInfo>, IMemberPolicy
	{
		readonly static MemberComparer Comparer = MemberComparer.Default;

		public BlacklistMemberPolicy(params MemberInfo[] avoid) : this(Comparer, avoid) {}

		public BlacklistMemberPolicy(IMemberComparer comparer, params MemberInfo[] avoid)
			: this(new HashSet<MemberInfo>(avoid, comparer)) {}

		public BlacklistMemberPolicy(ICollection<MemberInfo> avoid) :
			base(new ContainsSpecification<MemberInfo>(avoid)) {}
	}
}