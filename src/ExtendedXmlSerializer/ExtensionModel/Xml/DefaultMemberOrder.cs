using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	public sealed class DefaultMemberOrder : IParameterizedSource<MemberInfo, int>
	{
		public static DefaultMemberOrder Default { get; } = new DefaultMemberOrder();

		DefaultMemberOrder() {}

		public int Get(MemberInfo parameter) => DefaultXmlElementAttribute.Default.Get(parameter)
		                                                                  ?.Order ?? parameter.MetadataToken;
	}
}