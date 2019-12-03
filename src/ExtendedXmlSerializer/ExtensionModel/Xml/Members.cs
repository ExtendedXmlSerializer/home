using ExtendedXmlSerializer.Core.Sources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class Members : ReferenceCache<Type, List<MemberInfo>>, IParameterizedSource<MemberInfo, List<MemberInfo>>
	{
		public static Members Default { get; } = new Members();

		Members() : base(x => x.GetMembers().ToList()) {}

		public List<MemberInfo> Get(MemberInfo parameter) => base.Get(parameter.ReflectedType);
	}
}