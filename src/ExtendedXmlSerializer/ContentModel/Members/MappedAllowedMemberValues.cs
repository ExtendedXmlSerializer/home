using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class MappedAllowedMemberValues : IAllowedMemberValues
	{
		readonly static IMemberComparer Comparer = new MemberComparer(InheritedTypeComparer.Default);

		readonly Table _primary, _secondary;

		public MappedAllowedMemberValues(IDictionary<MemberInfo, IAllowedValueSpecification> primary)
			: this(primary, primary.ToDictionary(x => x.Key, x => x.Value, Comparer)) {}

		public MappedAllowedMemberValues(IDictionary<MemberInfo, IAllowedValueSpecification> primary,
		                                 IDictionary<MemberInfo, IAllowedValueSpecification> secondary)
			: this(new Table(new TableSource<MemberInfo, IAllowedValueSpecification>(primary)),
			       new Table(new TableSource<MemberInfo, IAllowedValueSpecification>(secondary))) {}

		MappedAllowedMemberValues(Table primary, Table secondary)
		{
			_primary   = primary;
			_secondary = secondary;
		}

		public IAllowedValueSpecification Get(MemberInfo parameter)
			=> _primary.Get(parameter) ?? _secondary.Get(parameter);

		sealed class Table : IParameterizedSource<MemberDescriptor, IAllowedValueSpecification>
		{
			readonly IParameterizedSource<MemberInfo, IAllowedValueSpecification> _source;

			public Table(IParameterizedSource<MemberInfo, IAllowedValueSpecification> source) => _source = source;

			public IAllowedValueSpecification Get(MemberDescriptor parameter)
				=> _source.Get(parameter.Metadata) ?? _source.Get(parameter.MemberType);
		}
	}
}