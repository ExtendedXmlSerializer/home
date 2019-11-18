namespace ExtendedXmlSerializer.Configuration
{
	// ReSharper disable once UnusedTypeParameter
	/// <summary>
	/// A root-level component that represents a strongly-typed type configuration. A type configuration is used to
	/// customize and configure a type in a container, which is then further utilized when it creates a serializer.  These
	/// customizations are then used when an instance of the type configuration is serialized or deserialized by the
	/// serializer.
	/// </summary>
	/// <typeparam name="T">The type under configuration.</typeparam>
	public interface ITypeConfiguration<T> : ITypeConfiguration {}

	// ReSharper disable once PossibleInterfaceMemberAmbiguity
	/// <summary>
	/// A root-level component that represents a generalized type configuration. A type configuration is used to customize and configure a type in a
	/// container, which is then further utilized when it creates a serializer.  These customizations are then used when an
	/// instance of the type configuration is serialized or deserialized by the serializer.
	/// </summary>
	public interface ITypeConfiguration : IConfigurationContainer, ITypeConfigurationContext, IMemberConfigurations {}
}