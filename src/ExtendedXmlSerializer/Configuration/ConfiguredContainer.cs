namespace ExtendedXmlSerializer.Configuration
{
	/// <summary>
	/// Entry point for creating containers by way of profiles, which are pre-configured container configurations.
	/// </summary>
	public static class ConfiguredContainer
	{
		/// <summary>
		/// Creates a new configuration with the referenced profile type applied to it.
		/// </summary>
		/// <typeparam name="T">The referenced profile type.</typeparam>
		/// <returns>A configured configuration container with the referenced configuration profile applied to it.
		/// </returns>
		public static IConfigurationContainer New<T>() where T : class, IConfigurationProfile
			=> new ConfigurationContainer().Configured<T>();
	}
}