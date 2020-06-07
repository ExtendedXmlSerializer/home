using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	/// <summary>
	/// Marker extension used for defining allowed and prohibited members.
	/// </summary>
	public interface IAllowedMembersExtension : ISerializerExtension
	{
		/// <summary>
		/// Prohibited members.
		/// </summary>
		ICollection<MemberInfo> Blacklist { get; }

		/// <summary>
		/// Allowed members.
		/// </summary>
		ICollection<MemberInfo> Whitelist { get; }
	}
}