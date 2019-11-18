using ExtendedXmlSerializer.Core.Sources;
using System.Reflection;

// ReSharper disable UnusedTypeParameter

namespace ExtendedXmlSerializer.Configuration
{
	/// <summary>
	/// Represents a strongly-typed member configuration.  This is the harness on which all configurations to a type's
	/// member are applied.
	/// </summary>
	/// <typeparam name="T">The member's containing type.</typeparam>
	/// <typeparam name="TMember">The value type of the member.</typeparam>
	public interface IMemberConfiguration<T, TMember> : IMemberConfiguration, ITypeConfiguration<T> {}

	/// <summary>
	/// Represents a generalized member configuration.  This is the harness on which all configurations to a type's member
	/// are applied.
	/// </summary>
	// ReSharper disable once PossibleInterfaceMemberAmbiguity
	public interface IMemberConfiguration : ITypeConfiguration, ISource<MemberInfo> {}
}