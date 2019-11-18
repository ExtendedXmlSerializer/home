using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using System.Reflection;
using System.Xml.Serialization;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	/// <summary>
	/// A default implementation that selects the name for a member from metadata attributes, specifically the
	/// <see cref="XmlAttributeAttribute"/> and <see cref="XmlElementAttribute"/> attributes (in that order).
	/// </summary>
	public sealed class DeclaredMemberNames : INames
	{
		/// <summary>
		/// The default instance.
		/// </summary>
		public static DeclaredMemberNames Default { get; } = new DeclaredMemberNames();

		DeclaredMemberNames() {}

		/// <inheritdoc />
		public string Get(MemberInfo parameter)
			=> parameter.GetCustomAttribute<XmlAttributeAttribute>(false)?.AttributeName.NullIfEmpty()
			   ??
			   DefaultXmlElementAttribute.Default.Get(parameter)?.ElementName.NullIfEmpty();
	}
}