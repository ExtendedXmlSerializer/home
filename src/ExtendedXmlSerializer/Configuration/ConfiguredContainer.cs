namespace ExtendedXmlSerializer.Configuration
{
	public static class ConfiguredContainer
	{
		public static IConfigurationContainer New<T>() where T : class, IConfigurationProfile
			=> new ConfigurationContainer().Configured<T>();
	}
}