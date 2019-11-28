using ExtendedXmlSerializer.Core.Sources;
using System;
using System.Collections.Generic;
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

		DefaultMemberOrder() : this(Members.Default) {}


		readonly IParameterizedSource<MemberInfo, List<MemberInfo>> _members;

		/// <inheritdoc />
		public DefaultMemberOrder(IParameterizedSource<MemberInfo, List<MemberInfo>> members) => _members = members;

		/// <inheritdoc />
		public int Get(MemberInfo parameter)
			=> DefaultXmlElementAttribute.Default.Get(parameter)?.Order ?? Resolve(parameter);

		int Resolve(MemberInfo parameter)
		{
			try
			{
				return parameter.MetadataToken;
			}
			catch (InvalidOperationException) // UWP Throws, because UWP: https://github.com/ExtendedXmlSerializer/home/issues/269#issuecomment-559485714
			{
				return _members.Get(parameter).IndexOf(parameter);
			}
		}
	}
}