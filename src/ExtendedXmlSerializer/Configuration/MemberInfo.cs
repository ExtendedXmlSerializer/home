using ExtendedXmlSerializer.Core.Sources;
using System.Reflection;

namespace ExtendedXmlSerializer.Configuration
{
	/// <summary>
	/// Represents a strongly-typed member data. This is considered internal code and not to be used by external
	/// applications.
	/// </summary>
	/// <typeparam name="T">The member's value type.</typeparam>
	// ReSharper disable once UnusedTypeParameter
	public sealed class MemberInfo<T> : FixedInstanceSource<MemberInfo>
	{
		/// <inheritdoc />
		public MemberInfo(MemberInfo instance) : base(instance) {}
	}
}