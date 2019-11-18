using ExtendedXmlSerializer.Core.Sources;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	/// <inheritdoc cref="IMemberTable{T}" />
	public class MemberTable<T> : TableSource<MemberInfo, T>, IMemberTable<T>
	{
		/// <inheritdoc />
		public MemberTable() : this(new ConcurrentDictionary<MemberInfo, T>(MemberComparer.Default)) {}

		/// <inheritdoc />
		public MemberTable(IDictionary<MemberInfo, T> store) : base(store) {}
	}
}