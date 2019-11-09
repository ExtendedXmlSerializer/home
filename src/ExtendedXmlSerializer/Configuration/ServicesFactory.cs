using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ExtensionModel.Types;
using LightInject;
using System;
using System.Linq;
using IServiceProvider = System.IServiceProvider;

namespace ExtendedXmlSerializer.Configuration
{
	sealed class ServicesFactory : IServicesFactory
	{
		public static ServicesFactory Default { get; } = new ServicesFactory();

		ServicesFactory() : this(ConstructorSelector.Default, new ContainerOptions {EnablePropertyInjection = false}) {}

		readonly IConstructorSelector _selector;
		readonly ContainerOptions     _options;

		public ServicesFactory(IConstructorSelector selector, ContainerOptions options)
		{
			_selector = selector;
			_options  = options;
		}

		public IServices Get(IExtensionCollection parameter)
		{
			var result = new Services(new ServiceContainer(_options) {ConstructorSelector = _selector});

			var services = result.RegisterInstance(parameter)
			                     .RegisterInstance<IServiceProvider>(new Provider(result.GetService));

			var extensions = parameter.OrderBy(x => x, SortComparer<ISerializerExtension>.Default)
			                          .Fixed();
			extensions.Alter(services);

			foreach (var extension in extensions)
			{
				extension.Execute(result);
			}

			return result;
		}

		sealed class Provider : DelegatedSource<Type, object>, IServiceProvider
		{
			public Provider(Func<Type, object> source) : base(source) {}

			public object GetService(Type serviceType) => Get(serviceType);
		}
	}
}