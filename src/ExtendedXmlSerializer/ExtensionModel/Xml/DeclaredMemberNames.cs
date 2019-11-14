using System.Reflection;
using System.Xml.Serialization;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	public sealed class DeclaredMemberNames : INames
	{
		public static DeclaredMemberNames Default { get; } = new DeclaredMemberNames();

		DeclaredMemberNames() {}

		public string Get(MemberInfo parameter)
			=> parameter.GetCustomAttribute<XmlAttributeAttribute>(false)
			            ?.AttributeName.NullIfEmpty() ??
			   DefaultXmlElementAttribute.Default.Get(parameter)
			                             ?.ElementName.NullIfEmpty();
	}
}