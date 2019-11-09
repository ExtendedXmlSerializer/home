using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ReflectionModel
{
	public class MemberTable<T> : TableSource<MemberInfo, T>, IMemberTable<T>
	{
		public MemberTable() : this(new ConcurrentDictionary<MemberInfo, T>(MemberComparer.Default)) {}

		public MemberTable(IDictionary<MemberInfo, T> store) : base(store) {}
	}
}