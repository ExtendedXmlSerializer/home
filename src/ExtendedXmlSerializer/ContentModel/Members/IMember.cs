using ExtendedXmlSerializer.ContentModel.Identification;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	/// <summary>
	/// Represents a member, which could be a field or property.
	/// </summary>
	public interface IMember : IIdentity
	{
		/// <summary>
		/// The backing metadata for the member.
		/// </summary>
		MemberInfo Metadata { get; }

		/// <summary>
		/// The value type of the member.
		/// </summary>
		TypeInfo MemberType { get; }

		/// <summary>
		/// Specifies if the member can be written (true) or if it's read-only (false).
		/// </summary>
		bool IsWritable { get; }

		/// <summary>
		/// The order value of the member, used during serialization to appropriately order this member with other members to
		/// emit in a particular order.
		/// </summary>
		int Order { get; }
	}
}