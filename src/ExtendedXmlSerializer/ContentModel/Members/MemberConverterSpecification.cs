using System.Reflection;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class MemberConverterSpecification : DecoratedSpecification<MemberInfo>, IMemberConverterSpecification
	{
		public MemberConverterSpecification(ISpecification<MemberInfo> specification) : base(specification) {}
	}
}