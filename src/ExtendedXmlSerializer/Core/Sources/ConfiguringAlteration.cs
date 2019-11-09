namespace ExtendedXmlSerializer.Core.Sources
{
	sealed class ConfiguringAlteration<T> : IAlteration<T>
	{
		readonly ICommand<T> _configuration;

		public ConfiguringAlteration(ICommand<T> configuration) => _configuration = configuration;

		public T Get(T parameter)
		{
			_configuration.Execute(parameter);
			return parameter;
		}
	}
}