namespace ExtendedXmlSerializer.ExtensionModel
{
	sealed class FixedRegistration<T> : IRegistration
	{
		readonly T _instance;

		public FixedRegistration(T instance) => _instance = instance;

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.RegisterInstance(_instance, _instance.GetType()
			                                                  .AssemblyQualifiedName);
	}
}