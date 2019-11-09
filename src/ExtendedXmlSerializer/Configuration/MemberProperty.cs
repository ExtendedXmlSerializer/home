using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.Configuration
{
	sealed class MemberProperty<T> : MemberPropertyBase<MemberInfo, T>
	{
		public MemberProperty(IDictionary<MemberInfo, T> store, MemberInfo type) :
			this(new MemberTable<T>(store), type) {}

		public MemberProperty(IMetadataTable<MemberInfo, T> table, MemberInfo type) : base(table, type) {}
	}
}