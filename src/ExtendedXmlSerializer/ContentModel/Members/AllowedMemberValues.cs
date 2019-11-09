using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class AllowedMemberValues : DecoratedSource<MemberInfo, IAllowedValueSpecification>, IAllowedMemberValues
	{
		public AllowedMemberValues(IParameterizedSource<MemberInfo, IAllowedValueSpecification> source) :
			base(source) {}
	}
}