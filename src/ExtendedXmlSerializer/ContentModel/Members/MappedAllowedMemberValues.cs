using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class MappedAllowedMemberValues
		: TableSource<MemberInfo, IAllowedValueSpecification>,
		  IAllowedMemberValues
	{
		public MappedAllowedMemberValues(IDictionary<MemberInfo, IAllowedValueSpecification> store) : base(store) {}

		public override IAllowedValueSpecification Get(MemberInfo parameter) => From(parameter);

		IAllowedValueSpecification From(MemberDescriptor parameter)
			=> base.Get(parameter.Metadata) ?? base.Get(parameter.MemberType);
	}
}