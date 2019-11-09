using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ExtensionModel.Types
{
	sealed class SingletonActivationExtension : ISerializerExtension
	{
		public static SingletonActivationExtension Default { get; } = new SingletonActivationExtension();

		SingletonActivationExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.RegisterInstance<ISingletonCandidates>(SingletonCandidates.Default)
			            .RegisterInstance<ISingletonLocator>(SingletonLocator.Default)
			            .Decorate<IActivators, Activators>();

		public void Execute(IServices parameter) {}
	}
}