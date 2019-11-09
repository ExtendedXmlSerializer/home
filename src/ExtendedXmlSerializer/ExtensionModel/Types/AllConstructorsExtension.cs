using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ExtensionModel.Types
{
	sealed class AllConstructorsExtension : ISerializerExtension
	{
		public static AllConstructorsExtension Default { get; } = new AllConstructorsExtension();

		AllConstructorsExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.Decorate<IConstructors>((provider, constructors) => new AllConstructors(constructors));

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}