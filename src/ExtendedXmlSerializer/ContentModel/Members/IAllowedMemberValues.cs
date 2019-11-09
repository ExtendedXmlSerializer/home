using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	public interface IAllowedMemberValues : IParameterizedSource<MemberInfo, IAllowedValueSpecification> {}
}