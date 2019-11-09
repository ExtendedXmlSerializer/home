using System;

namespace ExtendedXmlSerializer.ExtensionModel
{
	sealed class Registration<T> : IRegistration
	{
		readonly static Type ServiceType = typeof(T);

		readonly Type _type;

		public Registration(Type type) => _type = type;

		public IServiceRepository Get(IServiceRepository parameter) => parameter.Register(ServiceType, _type);
	}
}