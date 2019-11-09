namespace ExtendedXmlSerializer.Configuration
{
	// ReSharper disable once UnusedTypeParameter
	public interface ITypeConfiguration<T> : ITypeConfiguration {}

	// ReSharper disable once PossibleInterfaceMemberAmbiguity
	public interface ITypeConfiguration : IConfigurationContainer, ITypeConfigurationContext, IMemberConfigurations {}
}