using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.Configuration
{
	abstract class MemberPropertyBase<TMember, T> : FixedSource<TMember, T>, IProperty<T> where TMember : MemberInfo
	{
		readonly IMetadataTable<TMember, T> _table;
		readonly TMember                    _member;

		protected MemberPropertyBase(IMetadataTable<TMember, T> table, TMember member) : base(table, member)
		{
			_table  = table;
			_member = member;
		}

		public void Assign(T value) => _table.Assign(_member, value);
	}
}