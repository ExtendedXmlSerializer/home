using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.Configuration
{
	class TypeProperty<T> : MemberPropertyBase<TypeInfo, T>
	{
		public TypeProperty(IDictionary<TypeInfo, T> store, TypeInfo type) : this(new TypedTable<T>(store), type) {}

		public TypeProperty(ITypedTable<T> table, TypeInfo type) : base(table, type) {}
	}
}