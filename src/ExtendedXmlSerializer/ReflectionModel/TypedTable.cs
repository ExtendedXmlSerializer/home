using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;
using JetBrains.Annotations;

namespace ExtendedXmlSerializer.ReflectionModel
{
	public class TypedTable<T> : TableSource<TypeInfo, T>, ITypedTable<T>
	{
		[UsedImplicitly]
		public TypedTable() {}

		public TypedTable(IDictionary<TypeInfo, T> store) : base(store) {}
	}
}