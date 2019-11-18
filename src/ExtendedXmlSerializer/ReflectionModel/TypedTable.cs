using ExtendedXmlSerializer.Core.Sources;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	/// <inheritdoc cref="ITypedTable{T}" />
	public class TypedTable<T> : TableSource<TypeInfo, T>, ITypedTable<T>
	{
		/// <inheritdoc />
		[UsedImplicitly]
		public TypedTable() {}

		/// <inheritdoc />
		public TypedTable(IDictionary<TypeInfo, T> store) : base(store) {}
	}
}