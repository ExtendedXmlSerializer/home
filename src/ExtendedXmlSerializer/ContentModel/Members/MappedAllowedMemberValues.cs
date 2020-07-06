using ExtendedXmlSerializer.Core.Sources;
using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class MappedAllowedMemberValues : IAllowedMemberValues
	{
		readonly Table _primary;

		public MappedAllowedMemberValues(IDictionary<MemberInfo, IAllowedValueSpecification> primary)
			: this(new Table(new TableSource<MemberInfo, IAllowedValueSpecification>(primary))) {}

		MappedAllowedMemberValues(Table primary) => _primary   = primary;

		public IAllowedValueSpecification Get(MemberInfo parameter)
			=> _primary.Get(parameter);

		sealed class Table : IParameterizedSource<MemberDescriptor, IAllowedValueSpecification>
		{
			readonly IParameterizedSource<MemberInfo, IAllowedValueSpecification> _source;

			public Table(IParameterizedSource<MemberInfo, IAllowedValueSpecification> source) => _source = source;

			public IAllowedValueSpecification Get(MemberDescriptor parameter)
				=> _source.Get(parameter.Metadata) ?? _source.Get(parameter.MemberType);
		}
	}
}