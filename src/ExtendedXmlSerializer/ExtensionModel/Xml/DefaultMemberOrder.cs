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

		DefaultMemberOrder() : this(DefaultXmlElementAttribute.Default.Get, Members.Default.Get) {}

		readonly Func<MemberInfo, XmlElementAttribute> _attribute;
		readonly Func<MemberInfo, List<MemberInfo>> _members;

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="attribute"></param>
		/// <param name="members"></param>
		public DefaultMemberOrder(Func<MemberInfo, XmlElementAttribute> attribute,
		                          Func<MemberInfo, List<MemberInfo>> members)
		{
			_attribute = attribute;
			_members = members;
		}

		/// <inheritdoc />
		public int Get(MemberInfo parameter) => _attribute(parameter)?.Order ?? Resolve(parameter);

		int Resolve(MemberInfo parameter)
		{
			try
			{
				return parameter.MetadataToken;
			}
			catch (InvalidOperationException) // UWP Throws, because UWP: https://github.com/ExtendedXmlSerializer/home/issues/269#issuecomment-559485714
			{
				return _members(parameter).IndexOf(parameter);
			}
		}
	}
}