using ExtendedXmlSerializer.ContentModel.Reflection;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	/// <summary>
	/// Used to select a name from a provided member metadata.
	/// </summary>
	public interface INames : INames<MemberInfo> {}
}