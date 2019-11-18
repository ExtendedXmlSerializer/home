using ExtendedXmlSerializer.Core.Sources;
using System.Reflection;
using System.Xml.Serialization;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	/// <summary>
	/// Default order selector that looks for a <see cref="XmlElementAttribute.Order"/> and if not specified, uses the <see cref="MemberInfo.MetadataToken"/>.
	/// </summary>
	public sealed class DefaultMemberOrder : IParameterizedSource<MemberInfo, int>
	{
		/// <summary>
		/// The default instance.
		/// </summary>
		public static DefaultMemberOrder Default { get; } = new DefaultMemberOrder();

		DefaultMemberOrder() {}

		/// <inheritdoc />
		public int Get(MemberInfo parameter)
			=> DefaultXmlElementAttribute.Default.Get(parameter)?.Order ?? parameter.MetadataToken;
	}
}