using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

// ReSharper disable UnusedTypeParameter

namespace ExtendedXmlSerializer.Configuration
{
	public interface IMemberConfiguration<T, TMember> : IMemberConfiguration, ITypeConfiguration<T> {}

	// ReSharper disable once PossibleInterfaceMemberAmbiguity
	public interface IMemberConfiguration : ITypeConfiguration, ISource<MemberInfo> {}
}