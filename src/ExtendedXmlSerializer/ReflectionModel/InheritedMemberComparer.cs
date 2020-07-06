using ExtendedXmlSerializer.Core.Sources;
using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class InheritedMemberComparer : FixedInstanceSource<IEqualityComparer<MemberInfo>>
	{
		public static InheritedMemberComparer Default { get; } = new InheritedMemberComparer();

		InheritedMemberComparer() : base(new MemberComparer(InheritedTypeComparer.Default)) {}
	}
}
