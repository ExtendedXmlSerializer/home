using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class Members : ReferenceCache<MemberInfo, List<MemberInfo>>
	{
		public static Members Default { get; } = new Members();

		Members() : base(x => x.ReflectedType.GetMembers().ToList()) {}
	}
}