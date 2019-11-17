using ExtendedXmlSerializer.Core.Sources;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	/// <summary>
	/// A selector that is used to provide a specification for a member that will determine if it will emit or not.
	/// </summary>
	public interface IAllowedMemberValues : IParameterizedSource<MemberInfo, IAllowedValueSpecification> {}
}